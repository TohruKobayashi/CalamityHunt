using System;
using CalamityHunt.Common.GlobalNPCs;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged;

public class AntiMassDeathLaser : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 15000;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.timeLeft = 500;
        Projectile.penetrate = -1;
        Projectile.tileCollide = true;
        Projectile.ownerHitCheck = true;
        Projectile.extraUpdates = 15;
        Projectile.localNPCHitCooldown = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.ignoreWater = true;
    }

    public ref float Time => ref Projectile.ai[0];
    public ref float DrillTime => ref Projectile.ai[1];

    public override void AI()
    {
        if (Projectile.velocity.Length() > 0) {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        if (Time == 0) {
            Projectile.localAI[0] = Main.rand.NextFloat(20f);
            firstPosition = Projectile.Center;
        }

        if (DrillTime <= 0) {

            Color color = Color.Lerp(AntiMassAccumulatorProj.MainColor, Color.Orange, Main.rand.NextBool((int)(Projectile.timeLeft / 1200f * 10) + 1).ToInt()) with { A = 20 };
            Dust sparks = Dust.NewDustPerfect(Projectile.Center, 278, Projectile.velocity.RotatedByRandom(0.2f) * Main.rand.NextFloat(), 0, color * 0.8f, Main.rand.NextFloat(0.9f));
            sparks.noGravity = true;

            if (Time % 9 == 5) {
                CalamityHunt.particles.Add(Particle.Create<StraightLightningParticle>(particle => {
                    particle.position = Projectile.Center - Projectile.velocity * 0.5f;
                    particle.velocity = Projectile.velocity * 0.15f;
                    particle.scale = Main.rand.NextFloat(0.5f, 1f) * Projectile.scale;
                    particle.color = Color.Lerp(AntiMassAccumulatorProj.MainColor, Color.Orange, Main.rand.NextBool((int)(Projectile.timeLeft / 1200f * 10) + 1).ToInt()) with { A = 20 };
                    particle.rotation = Projectile.rotation;
                    particle.maxTime = (int)(Utils.GetLerpValue(500, 240, Projectile.timeLeft, true) * 25) + Main.rand.Next(3, 6);
                    particle.stretch = Projectile.velocity.Length() * 0.25f / particle.scale * Projectile.scale;
                    particle.flickerSpeed = Main.rand.NextFloat(0.1f, 2f);
                }));
            }

            if (Time % 6 == 3) {
                CalamityHunt.particles.Add(Particle.Create<StraightLightningParticle>(particle => {
                    particle.position = Projectile.Center - Projectile.velocity * 0.5f;
                    particle.velocity = Projectile.velocity * 0.1f;
                    particle.scale = Main.rand.NextFloat(1f, 2f) * Projectile.scale;
                    particle.color = Color.Lerp(AntiMassAccumulatorProj.MainColor, Color.Turquoise, Main.rand.NextBool(3).ToInt()) with { A = 20 };
                    particle.rotation = Projectile.rotation;
                    particle.maxTime = (int)(Utils.GetLerpValue(500, 240, Projectile.timeLeft, true) * 5) + Main.rand.Next(5, 10);
                    particle.stretch = Projectile.velocity.Length() * 0.11f * Projectile.scale;
                    particle.flickerSpeed = Main.rand.NextFloat(0.5f, 2f);
                }));
            }

            if (Main.rand.NextBool(18)) {
                CalamityHunt.particles.Add(Particle.Create<LightningParticle>(particle => {
                    particle.position = Projectile.Center;
                    particle.velocity = Projectile.velocity.RotatedByRandom(1f) * Main.rand.NextFloat(0.2f);
                    particle.scale = Main.rand.NextFloat(0.1f, 0.5f) * Projectile.scale;
                    particle.color = AntiMassAccumulatorProj.MainColor with { A = 40 };
                    particle.rotation = Projectile.rotation;
                }));
            }
        }

        if (Projectile.timeLeft > 240) {
            if (Main.myPlayer == Projectile.owner) {
                if (DrillTime-- <= 0) {
                    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 15;

                    Time++;
                    DrillTime = 0;

                    visualSpeed = Projectile.Distance(firstPosition);
                    Projectile.netUpdate = true;
                }
                else {
                    Projectile.timeLeft++;
                    if (DrillTime % 3 == 0) {
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, -Projectile.rotation.ToRotationVector2().RotatedByRandom(0.2f), 15, 1f, 20, 2000, "AntiMass"));
                    }
                }
            }
        }
        else {
            Projectile.velocity *= 0.8f;
        }

        if (Projectile.timeLeft < 240 || DrillTime > 0) {
            if (Main.rand.NextBool(3)) {
                CalamityHunt.particles.Add(Particle.Create<LightningParticle>(particle => {
                    particle.position = Projectile.Center;
                    particle.velocity = Main.rand.NextVector2Circular(20, 20);
                    particle.scale = Main.rand.NextFloat(1f, 2f);
                    particle.color = Color.Lerp(AntiMassAccumulatorProj.MainColor, Color.MediumTurquoise, Main.rand.NextBool(3).ToInt()) with { A = 40 };
                    particle.rotation = particle.velocity.ToRotation();
                }));
            }
        }

        Projectile.localAI[0] += 0.002f;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.tileCollide = false;
        Projectile.velocity = Vector2.Zero;

        Projectile.timeLeft = (int)MathHelper.Min(Projectile.timeLeft, 240);

        return false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        //actual laser
        //Vector2 startPoint = Projectile.Center - Projectile.velocity * Time;
        //float collisionPoint = 0;
        //bool laser = Collision.CheckAABBvLineCollision(targetHitbox.Location.ToVector2(), targetHitbox.Size(), startPoint, Projectile.Center, Projectile.width * 2, ref collisionPoint);

        bool laser = targetHitbox.Intersects(projHitbox);
        if (Main.myPlayer == Projectile.owner) {
            if (laser) {
                DrillTime = 10;
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 0.01f;
                Projectile.netUpdate = true;
            }
        }

        if (laser) {
            for (int i = 0; i < 20; i++) {
                CalamityHunt.particles.Add(Particle.Create<LightningParticle>(particle => {
                    particle.position = Projectile.Center;
                    particle.velocity = Main.rand.NextVector2Circular(40, 40);
                    particle.scale = Main.rand.NextFloat(1f, 2f);
                    particle.color = Color.Lerp(AntiMassAccumulatorProj.MainColor, Color.MediumTurquoise, Main.rand.NextBool(3).ToInt()) with { A = 40 };
                    particle.flickerSpeed = 2f;
                }));
            }
        }

        return laser;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (target.GetGlobalNPC<DoomedNPC>().doomCount > 0) {
            modifiers.FinalDamage *= 5f;
            target.GetGlobalNPC<DoomedNPC>().doomCount = 0;
        }
        Projectile.damage = (int)(Projectile.damage * 0.9f);
    }

    private float visualSpeed;
    private Vector2 firstPosition;

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Rectangle half = texture.Frame(1, 2, 0, 0);

        float thickness = Utils.GetLerpValue(20, 240, Projectile.timeLeft, true);
        Vector2 trailDrawScale = new Vector2(thickness * 0.5f, visualSpeed / (texture.Height * 2f));
        Vector2 drawPosition = Projectile.Center + Main.rand.NextVector2Circular(3, 3);

        Main.EntitySpriteDraw(texture, drawPosition - Main.screenPosition, half, AntiMassAccumulatorProj.MainColor, Projectile.rotation - MathHelper.PiOver2, half.Size() * new Vector2(0.5f, 1f), trailDrawScale, 0, 0);
        Main.EntitySpriteDraw(texture, drawPosition - Main.screenPosition, half, AntiMassAccumulatorProj.MainColor, Projectile.rotation + MathHelper.PiOver2, half.Size() * new Vector2(0.5f, 1f), thickness, 0, 0);

        Vector2 glowScale = new Vector2(thickness * 0.5f, 1f);
        Main.EntitySpriteDraw(texture, drawPosition - Main.screenPosition, half, Color.White with { A = 0 }, Projectile.rotation - MathHelper.PiOver2, half.Size() * new Vector2(0.5f, 1f), glowScale * trailDrawScale, 0, 0);
        Main.EntitySpriteDraw(texture, drawPosition - Main.screenPosition, half, Color.White with { A = 0 }, Projectile.rotation + MathHelper.PiOver2, half.Size() * new Vector2(0.5f, 1f), glowScale, 0, 0);

        Vector2[] points = new Vector2[64];
        points[0] = Projectile.Center;
        float[] rotations = new float[64];
        for (int i = 0; i < points.Length; i++) {
            points[i] = Vector2.Lerp(Projectile.Center, Projectile.Center - new Vector2(visualSpeed, 0).RotatedBy(Projectile.rotation), (float)i / points.Length);
            rotations[i] = Projectile.rotation;
        }

        Effect lightningEffect = AssetDirectory.Effects.LightningBeam.Value;
        lightningEffect.Parameters["transformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
        lightningEffect.Parameters["uTexture0"].SetValue(AssetDirectory.Textures.Noise[7].Value);
        lightningEffect.Parameters["uTexture1"].SetValue(TextureAssets.Extra[197].Value);
        lightningEffect.Parameters["uColor"].SetValue((Color.White with { A = 0 }).ToVector4());
        lightningEffect.Parameters["uBloomColor"].SetValue((Color.DarkOrange with { A = 20 }).ToVector4());
        lightningEffect.Parameters["uLength"].SetValue(visualSpeed / 128f);
        lightningEffect.Parameters["uNoiseThickness"].SetValue(0.5f);
        lightningEffect.Parameters["uNoiseSize"].SetValue(2f);
        lightningEffect.Parameters["uTime"].SetValue(Projectile.localAI[0]);
        lightningEffect.CurrentTechnique.Passes[0].Apply();

        VertexStrip strip = new VertexStrip();
        strip.PrepareStrip(points, rotations, (p) => Color.White, (p) => 110f * thickness * MathF.Sqrt(1f - p) * Utils.GetLerpValue(0, 0.03f, p, true), -Main.screenPosition, points.Length, true);
        strip.DrawTrail();

        Main.pixelShader.CurrentTechnique.Passes[0].Apply();

        return false;
    }
}
