using System;
using System.Linq;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.NPCs.Bosses.GoozmaBoss.Projectiles;

public class SlimeBomb : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 60;
        Projectile.height = 60;
        Projectile.tileCollide = false;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.penetrate = -1;
        Projectile.aiStyle = -1;

        initializedLocal = false;
    }

    public override void OnSpawn(IEntitySource source)
    {
        
    }

    private bool initializedLocal = false;

    public ref float Time => ref Projectile.ai[0];

    public override void AI()
    {
        if (!initializedLocal) {
            Projectile.localAI[1] = Main.rand.NextFloat(30f);
            Projectile.rotation = Main.rand.NextFloat(-1f, 1f);

            SoundStyle shootSound = AssetDirectory.Sounds.Goozma.Shot with { Pitch = -0.3f };
            SoundEngine.PlaySound(shootSound, Projectile.Center);

            if (!Main.dedServ) {
                for (int i = 0; i < 5; i++) {
                    Color glowColor = new GradientColor(SlimeUtils.GoozColors, 0.4f, 0.5f).ValueAt(Projectile.localAI[1]);
                    glowColor.A /= 2;
                    Dust.NewDustPerfect(Projectile.Center, DustID.RainbowMk2, Projectile.velocity + Main.rand.NextVector2Circular(5, 5), 0, glowColor, 1.5f).noGravity = true;
                }
            }

            initializedLocal = true;
        }

        if (Projectile.ai[1] == 0) {
            if (Time > 8) {
                Projectile.velocity *= 0.955f;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                Projectile.rotation = (float)Math.Sin(Projectile.localAI[1] * 0.03f) * Projectile.direction * 0.1f + Projectile.velocity.X * 0.1f;
                Projectile.scale = (float)Math.Sqrt(Utils.GetLerpValue(0, 17, Projectile.localAI[0], true)) + (float)Math.Pow(Utils.GetLerpValue(97, 100, Time, true), 2f) * 0.5f;
                Projectile.netUpdate = true;
            }
            

            int target = -1;
            if (Main.player.Any(n => n.active && !n.dead)) {
                target = Main.player.First(n => n.active && !n.dead).whoAmI;
            }

            if (target > -1) {
                Projectile.velocity += Projectile.DirectionTo(Main.player[target].MountedCenter).SafeNormalize(Vector2.Zero).RotatedByRandom(0.2f) * 0.01f * (float)Math.Pow(Math.Sin(Time * 0.05f), 2f);
            }

            if (Main.rand.NextBool(8)) {
                CalamityHunt.Particles.Add(Particle.Create<ChromaticEnergyDust>(particle => {
                    particle.position = Projectile.Center + Main.rand.NextVector2Circular(40, 40);
                    particle.velocity = -Vector2.UnitY * Main.rand.NextFloat(2f);
                    particle.scale = 1.2f;
                    particle.color = Color.White;
                    particle.colorData = new ColorOffsetData(true, Projectile.localAI[1]);
                }));
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 7) {
                Projectile.frame = (Projectile.frame + 1) % 4;
                Projectile.frameCounter = 0;
            }

            if (Time > 100) {
                Time = 0;
                Projectile.ai[1]++;
            }
            if (Time == 1) {
                SoundStyle chargeSound = AssetDirectory.Sounds.Goozma.BombCharge;
                SoundEngine.PlaySound(chargeSound.WithVolumeScale(0.33f), Projectile.Center);
            }
        }
        else {
            Projectile.scale = 1f;

            if (Time == 1) {
                SoundStyle explodeSound = AssetDirectory.Sounds.Goozma.BloatedBlastShoot;
                SoundEngine.PlaySound(explodeSound.WithVolumeScale(0.7f), Projectile.Center);
                SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion.WithVolumeScale(0.8f), Projectile.Center);
            }

            if (Time < 3) {
                for (int i = 0; i < 5; i++) {
                    Vector2 gooVelocity = new Vector2(1, 0).RotatedBy(MathHelper.TwoPi / 5f * i).RotatedByRandom(0.2f);
                    CalamityHunt.Particles.Add(Particle.Create<ChromaticGooBurst>(particle => {
                        particle.position = Projectile.Center + gooVelocity * 2;
                        particle.velocity = gooVelocity;
                        particle.scale = 3f - Time + Main.rand.NextFloat();
                        particle.color = Color.White;
                        particle.colorData = new ColorOffsetData(true, Projectile.localAI[1]);
                    }));
                }
            }

            if (Time < 10) {
                CalamityHunt.Particles.Add(Particle.Create<ChromaticGooBurst>(particle => {
                    particle.position = Projectile.Center + Main.rand.NextVector2Circular(20, 20);
                    particle.velocity = Main.rand.NextVector2Circular(25, 25);
                    particle.scale = 1.5f;
                    particle.color = Color.White;
                    particle.colorData = new ColorOffsetData(true, Projectile.localAI[1]);
                }));

                for (int i = 0; i < 10; i++) {
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(20, 20), DustID.TintableDust, Main.rand.NextVector2Circular(10, 10), 100, Color.Black, 1f + Main.rand.NextFloat(2)).noGravity = true;
                }
            }

            if (Time > 60) {
                Projectile.Kill();
            }
        }

        Time++;
        Projectile.localAI[0]++;
        Projectile.localAI[1]++;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float maxDist = Math.Min(Projectile.Distance(targetHitbox.Center()), 250);
        //Dust.QuickDust(Projectile.Center + new Vector2(maxDist, 0).RotatedBy(Projectile.Center.AngleTo(targetHitbox.Center())), Color.Red);

        if (Projectile.ai[1] == 1) {
            Vector2 radius = Projectile.Center + new Vector2(maxDist, 0).RotatedBy(Projectile.Center.AngleTo(targetHitbox.Center()));
            if (Time < 25) {
                return targetHitbox.Contains(radius.ToPoint());
            }

            return false;
        }
        else {
            return projHitbox.Intersects(targetHitbox);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Texture2D glow = AssetDirectory.Textures.Glow[1].Value;
        Texture2D ring = AssetDirectory.Textures.Glow[3].Value;
        Rectangle baseFrame = texture.Frame(3, 4, 0, Projectile.frame);
        Rectangle glowFrame = texture.Frame(3, 4, 1, Projectile.frame);
        Rectangle outlineFrame = texture.Frame(3, 4, 2, Projectile.frame);

        Color bloomColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[1]) with { A = 0 };

        if (Projectile.ai[1] == 0) {
            float ringScale = (float)Math.Cbrt(Utils.GetLerpValue(0, 100, Time, true));
            float ringPower = 1f + (float)Math.Sin(Math.Pow(Time * 0.027f, 3f)) * 0.4f;

            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, bloomColor * 0.3f * ringScale * ringPower, Projectile.rotation, glow.Size() * 0.5f, (250f / glow.Height * 2f + 1f) * ringScale, 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, bloomColor * 0.4f * ringScale * ringPower, 0, glow.Size() * 0.5f, 250f / glow.Height * 2f * 0.5f * ringScale, 0, 0);
            Main.EntitySpriteDraw(ring, Projectile.Center - Main.screenPosition, null, bloomColor * 0.1f * ringScale * ringPower, Projectile.rotation * 0.1f, ring.Size() * 0.5f, (250f / ring.Height * 2f + 0.5f) * ringScale, 0, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, baseFrame, lightColor, Projectile.rotation, baseFrame.Size() * 0.5f, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, glowFrame, bloomColor, Projectile.rotation, glowFrame.Size() * 0.5f, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, glowFrame, bloomColor * 0.8f, Projectile.rotation, glowFrame.Size() * 0.5f, Projectile.scale * 1.05f, 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, bloomColor * 0.4f, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale, 0, 0);

        }
        else {
            float ringScale = 1.01f + (float)Math.Sqrt(Time / 60f) * 0.3f;
            float ringPower = (float)Math.Pow(Utils.GetLerpValue(30, 0, Time, true), 3f);

            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, bloomColor * 0.2f * Utils.GetLerpValue(30, 0, Time, true), Projectile.rotation, glow.Size() * 0.5f, (250f / glow.Height * 2f + 5f) * ringScale * ringPower, 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, bloomColor * 0.5f * Utils.GetLerpValue(20, 0, Time, true), 0, glow.Size() * 0.5f, 250f / glow.Height * 2f * 0.6f * ringScale * ringPower, 0, 0);
            Main.EntitySpriteDraw(ring, Projectile.Center - Main.screenPosition, null, bloomColor * 0.1f * ringPower, Projectile.rotation * 0.1f, ring.Size() * 0.5f, (250f / ring.Height * 2f + 0.5f) * ringScale, 0, 0);
        }

        return false;
    }
}
