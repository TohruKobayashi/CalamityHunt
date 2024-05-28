using CalamityHunt.Common.GlobalNPCs;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged;

public class AntiMassBioStrike : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.friendly = true;
        Projectile.timeLeft = 60;
        Projectile.penetrate = -1;
        Projectile.tileCollide = true;
        Projectile.manualDirectionChange = true;
        Projectile.idStaticNPCHitCooldown = 30;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.ignoreWater = true;
        Projectile.extraUpdates = 1;
    }

    public ref float Time => ref Projectile.ai[0];
    public ref float Target => ref Projectile.ai[1];
    public ref float Owner => ref Projectile.ai[2];

    public override void AI()
    {
        Projectile.velocity.Y += 0.001f;

        Projectile.scale = Utils.GetLerpValue(-15, 10, Time, true);

        if (Time == 0) {
            Projectile.localAI[0] = Main.rand.NextFloat(20f);
        }

        Projectile.localAI[0] += 0.01f;

        if (Main.myPlayer == Projectile.owner) {
            if (Main.projectile.IndexInRange((int)Owner)) {
                Projectile ownerProjectile = Main.projectile[(int)Owner];
                if (ownerProjectile.type != ModContent.ProjectileType<AntiMassBioBall>() || !ownerProjectile.active) {
                    Projectile.Kill();
                }

                if (Main.npc.IndexInRange((int)Target)) {
                    NPC nPC = Main.npc[(int)Target];
                    if (!nPC.active || !nPC.CanBeChasedBy(ownerProjectile)) {
                        Projectile.Kill();
                    }

                    if (nPC.active && nPC.CanBeChasedBy(ownerProjectile) && nPC.Distance(ownerProjectile.Center) < AntiMassBioBall.MAX_RANGE * 1.5f) {
                        Projectile.Center = nPC.Center;
                        Projectile.velocity = nPC.velocity;
                        Projectile.netUpdate = true;
                    }
                }

                if (Main.rand.NextBool(40) && Projectile.timeLeft < 10) {
                    Projectile.timeLeft = 60;
                    Projectile.netUpdate = true;

                    rope = new Rope(ownerProjectile.Center, Projectile.Center, 40, 0.5f, Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(0.3f, 0.6f));
                }

                rope ??= new Rope(ownerProjectile.Center, Projectile.Center, 40, 0.5f, Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(0.3f, 0.6f));

                rope.StartPos = ownerProjectile.Center;
                rope.EndPos = Projectile.Center;
                rope.segmentLength = rope.StartPos.Distance(rope.EndPos) * 0.0261f;
                rope.gravity = Vector2.Lerp(rope.gravity, Vector2.Zero, 0.1f);
                rope.Update();
            }
        }

        if (Main.rand.NextBool(20) && rope != null) {
            Vector2 point = Main.rand.Next(rope.GetPoints());
            Color color = Color.Lerp(Color.Turquoise, AntiMassAccumulatorProj.MainColor, Main.rand.NextBool(20).ToInt()) with { A = 20 };
            Dust sparks = Dust.NewDustPerfect(point, 278, Main.rand.NextVector2Circular(2, 2), 0, color * 0.5f, Main.rand.NextFloat(0.9f));
            sparks.noGravity = true;
        }

        Time++;

        Lighting.AddLight(Projectile.Center, Color.MediumTurquoise.ToVector3() * 0.6f);
    }

    public override void OnKill(int timeLeft)
    {
        SoundStyle deathSound = SoundID.DD2_KoboldIgnite;
        deathSound.MaxInstances = 0;
        deathSound.PitchVariance = 0.1f;
        SoundEngine.PlaySound(deathSound.WithPitchOffset(0.5f).WithVolumeScale(0.2f), Projectile.Center);

        for (int i = 0; i < 10; i++) {
            CalamityHunt.particles.Add(Particle.Create<LightningParticle>(particle => {
                particle.position = Projectile.Center + Main.rand.NextVector2Circular(10, 10).RotatedBy(Projectile.rotation) * Projectile.scale;
                particle.velocity = Main.rand.NextVector2Circular(16, 16);
                particle.scale = Main.rand.NextFloat(0.5f, 1.5f) * Projectile.scale;
                particle.color = Color.Lerp(AntiMassAccumulatorProj.MainColor, Color.MediumAquamarine, (!Main.rand.NextBool((int)(Time / 30f + 8))).ToInt()) with { A = 0 };
                particle.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                particle.anchor = () => Projectile.velocity * 0.3f;
            }));

            if (!Main.rand.NextBool(4)) {
                Color color = Color.Lerp(AntiMassAccumulatorProj.MainColor, Color.Turquoise, (!Main.rand.NextBool(10)).ToInt()) with { A = 40 };
                Dust sparks = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16), 278, Main.rand.NextVector2Circular(5, 5), 0, color * 0.7f, Main.rand.NextFloat());
                sparks.noGravity = true;
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.GetGlobalNPC<DoomedNPC>().doomCount = 240;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return base.Colliding(projHitbox, targetHitbox);
    }

    public Rope rope;

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D glow = AssetDirectory.Textures.Glow[0].Value;
        Texture2D sparkle = AssetDirectory.Textures.Sparkle.Value;

        Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), (Color.MediumTurquoise * 0.1f) with { A = 0 }, 0, glow.Size() * 0.5f, Projectile.scale * 1.5f, 0, 0);
        Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), (Color.MediumTurquoise * 0.2f) with { A = 0 }, 0, glow.Size() * 0.5f, Projectile.scale, 0, 0);
        Main.EntitySpriteDraw(sparkle, Projectile.Center - Main.screenPosition, sparkle.Frame(), Color.Turquoise with { A = 0 }, MathHelper.PiOver2, sparkle.Size() * 0.5f, Projectile.scale * new Vector2(0.2f, 2f), 0, 0);

        if (rope != null) {
            Vector2[] points = rope.GetPoints().ToArray();
            float[] rotations = new float[points.Length];
            float totalLength = 0;
            for (int i = 0; i < points.Length - 1; i++) {
                totalLength += points[i].Distance(points[i + 1]);
                rotations[i] = points[i].AngleFrom(points[i + 1]);
            }
            rotations[^1] = rotations[^2];

            //for (int i = 0; i < points.Length - 2; i++) {
            //    Vector2 stretch = new Vector2(points[i].Distance(points[i + 1]) + 0.1f, 1f);
            //    Main.EntitySpriteDraw(TextureAssets.BlackTile.Value, points[i] - Main.screenPosition, new Rectangle(0, 0, 1, 2), Color.Turquoise with { A = 0 }, rotations[i], Vector2.UnitX, stretch, 0, 0);
            //}

            Effect lightningEffect = AssetDirectory.Effects.LightningBeam.Value;
            lightningEffect.Parameters["transformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            lightningEffect.Parameters["uTexture0"].SetValue(AssetDirectory.Textures.Noise[7].Value);
            lightningEffect.Parameters["uTexture1"].SetValue(TextureAssets.Extra[197].Value);
            lightningEffect.Parameters["uColor"].SetValue((Color.LightCyan with { A = 0 }).ToVector4());
            lightningEffect.Parameters["uBloomColor"].SetValue((Color.Turquoise with { A = 40 }).ToVector4());
            lightningEffect.Parameters["uLength"].SetValue(totalLength / 128f);
            lightningEffect.Parameters["uNoiseThickness"].SetValue(1f);
            lightningEffect.Parameters["uNoiseSize"].SetValue(5f);
            lightningEffect.Parameters["uTime"].SetValue(Projectile.localAI[0]);
            lightningEffect.CurrentTechnique.Passes[0].Apply();

            VertexStrip strip = new VertexStrip();
            strip.PrepareStrip(points, rotations, (p) => Color.White, (p) => 16f, -Main.screenPosition, points.Length / 2, true);
            strip.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

        return false;
    }
}
