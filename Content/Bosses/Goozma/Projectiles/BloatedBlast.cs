using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Arch.Core.Extensions;
using CalamityHunt.Core;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class BloatedBlast : ModProjectile, IDieWithGoozma
    {
        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 56;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.localAI[0] += Main.rand.NextFloat(20f);
            Projectile.frame = Main.rand.Next(3);
        }

        public float colOffset;

        public ref float Time => ref Projectile.ai[0];
        public ref float Speed => ref Projectile.ai[2];

        public override void AI()
        {
            if (Time <= 0)
                Speed = Projectile.velocity.Length();

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.scale = (float)Math.Sqrt(Utils.GetLerpValue(-1, 8, Projectile.localAI[0], true));

            Time++;

            if (Projectile.ai[1] == 0)
            {
                int target = -1;
                if (Main.player.Any(n => n.active && !n.dead))
                    target = Main.player.First(n => n.active && !n.dead).whoAmI;

                if (target > -1 && Time < 55)
                {
                    Projectile.velocity += Projectile.DirectionTo(Main.player[target].MountedCenter).SafeNormalize(Vector2.Zero) * (0.13f + Time * 0.015f);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.player[target].MountedCenter).SafeNormalize(Vector2.Zero) * 20f, 0.03f);
                    Projectile.Center += Main.player[target].velocity * 0.15f * Utils.GetLerpValue(100, 0, Time, true);
                }
                else
                    Projectile.velocity *= 0.866f;

                if (Time > 75)
                    Projectile.Kill();
            }
            else
            {
                Projectile.Resize(24, 24);
                if (Time <= 100)
                    Projectile.velocity = Projectile.oldVelocity.SafeNormalize(Vector2.Zero) * MathF.Pow(Utils.GetLerpValue(0, 100, Time, true), 1.2f) * Speed * 1.75f;

                Projectile.velocity.Y *= 1.007f;

                if (Main.rand.NextBool(5))
                {
                    var hue = ParticleBehavior.NewParticle(ModContent.GetInstance<HueLightDustParticleBehavior>(), Projectile.Center + Projectile.velocity, Main.rand.NextVector2Circular(2, 2), Color.White, 1f);
                    hue.Add(new ParticleData<float> { Value = Projectile.localAI[0]});
                }

                if (Main.rand.NextBool())
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5) + Projectile.velocity * 0.8f, DustID.TintableDust, Projectile.velocity * 0.4f, 100, Color.Black, 1f + Main.rand.NextFloat()).noGravity = true;

                if (Time > 250)
                    Projectile.Kill();
            }

            if (oldVels == null)
            {
                oldVels = new Vector2[10];
                for (int i = 0; i < oldVels.Length; i++)
                    oldVels[i] = Projectile.velocity;
            }
            else
            {
                for (int i = 9; i > 0; i--)
                    oldVels[i] = Vector2.Lerp(oldVels[i], oldVels[i - 1] * 1.2f, 0.6f);
                oldVels[0] = Vector2.Lerp(oldVels[0], (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2(), 0.6f);
            }

            Projectile.localAI[0]++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[1] == 0)
                return false;
            return base.Colliding(projHitbox, targetHitbox);
        }

        public Vector2[] oldVels;

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[1] == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    float size = Main.rand.NextFloat(4f, 5f) + i * 2f;
                    float rotation = Main.rand.NextFloat(-2f, 2f);

                    for (int j = 0; j < 30; j++)
                    {
                        Color glowColor = new GradientColor(SlimeUtils.GoozColors, 2f, 2f).ValueAt(j * 30f + i);
                        glowColor.A /= 2;
                        Vector2 outward = new Vector2(size + Main.rand.NextFloat(), 0).RotatedBy(MathHelper.TwoPi / 30f * j);
                        outward.X *= 0.4f;

                        var hue = ParticleBehavior.NewParticle(ModContent.GetInstance<HueLightDustParticleBehavior>(), Projectile.Center, outward.RotatedBy(rotation), Color.White, 1.5f);
                        hue.Add(new ParticleData<float> { Value = (j * 30f + i) / 10f });
                    }
                }

                for (int i = 0; i < 80; i++)
                    Dust.NewDustPerfect(Projectile.Center, DustID.TintableDust, Main.rand.NextVector2Circular(15, 15), 100, Color.Black, 2f).noGravity = true;

                int dartCount = (int)Common.Systems.DifficultySystem.DifficultyBasedValue(8, 12, 16, 18);
                for (int i = 0; i < dartCount; i++)
                {
                    Projectile dart = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(20, 0).RotatedBy(MathHelper.TwoPi / dartCount * i), Type, Projectile.damage, 0, ai1: 1);
                    dart.ai[1] = 1;
                    dart.localAI[0] = Projectile.localAI[0] + i / dartCount * 90f;
                }

                SoundStyle dartSound = AssetDirectory.Sounds.Goozma.Dart with { Pitch = 0.3f };
                SoundEngine.PlaySound(dartSound.WithVolumeScale(0.8f));
            }

            for (int i = 0; i < 10; i++)
            {
                var hue = ParticleBehavior.NewParticle(ModContent.GetInstance<HueLightDustParticleBehavior>(), Projectile.Center, Main.rand.NextVector2Circular(10, 10), Color.White, 1f);
                hue.Add(new ParticleData<float> { Value = Projectile.localAI[0] });

                Dust.NewDustPerfect(Projectile.Center, DustID.TintableDust, Main.rand.NextVector2Circular(4, 4), 100, Color.Black, 1.5f).noGravity = true;
            }
        }

        public static Texture2D textureSmall; 

        public override void Load()
        {
            textureSmall = ModContent.Request<Texture2D>(Texture + "Small", AssetRequestMode.ImmediateLoad).Value;
            textureTentacle = ModContent.Request<Texture2D>(Texture + "Tentacle", AssetRequestMode.ImmediateLoad).Value;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D glow = AssetDirectory.Textures.Glow.Value;
            Texture2D ring = AssetDirectory.Textures.GlowRing.Value;
            Vector2 squishFactor = new Vector2(1f - Projectile.velocity.Length() * 0.0045f, 1f + Projectile.velocity.Length() * 0.0075f);

            Color bloomColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);
            bloomColor.A = 0;

            if (Projectile.ai[1] == 0)
            {
                Color growColor = Color.Lerp(Color.Transparent, bloomColor, Utils.GetLerpValue(3, 40, Projectile.localAI[0], true));
                Rectangle baseFrame = texture.Frame(3, 1, 0, 0);
                Rectangle glowFrame = texture.Frame(3, 1, 1, 0);
                Rectangle outlineFrame = texture.Frame(3, 1, 2, 0);

                float aboutToExplode = 0.4f + (float)Math.Cbrt(Utils.GetLerpValue(50, 65, Time, true)) * 0.7f;
                Main.EntitySpriteDraw(glow, Projectile.Center + Projectile.velocity * 0.2f - Main.screenPosition, null, growColor * 0.3f * aboutToExplode, 0, glow.Size() * 0.5f, Projectile.scale * 5f * squishFactor, 0, 0);
                Main.EntitySpriteDraw(ring, Projectile.Center + Projectile.velocity * 0.2f - Main.screenPosition, null, growColor * 0.1f * aboutToExplode, 0, ring.Size() * 0.5f, Projectile.scale * 2.5f * aboutToExplode, 0, 0);

                //for (int i = 0; i < 4; i++)
                //{
                //    Vector2 off = new Vector2(2).RotatedBy(MathHelper.TwoPi / 4f * i + Projectile.rotation);
                //    Main.EntitySpriteDraw(texture.Value, Projectile.Center + off - Main.screenPosition, outlineFrame, growColor * 0.7f, Projectile.rotation, outlineFrame.Size() * 0.5f, Projectile.scale * squishFactor, 0, 0);
                //}

                if (oldVels != null)
                    DrawTentacles(lightColor, growColor);

                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, baseFrame, lightColor, Projectile.rotation, baseFrame.Size() * 0.5f, Projectile.scale * squishFactor, 0, 0);
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, glowFrame, growColor, Projectile.rotation, glowFrame.Size() * 0.5f, Projectile.scale * squishFactor, 0, 0);
                Main.EntitySpriteDraw(glow, Projectile.Center + Projectile.velocity * 0.2f - Main.screenPosition, null, growColor * 0.1f, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 2f * squishFactor, 0, 0);
            }
            if (Projectile.ai[1] == 1)
            {
                Rectangle baseFrame = textureSmall.Frame(3, 1, 0, 0);
                Rectangle glowFrame = textureSmall.Frame(3, 1, 1, 0);
                //Rectangle outlineFrame = textureSmall.Frame(3, 1, 2, 0);

                Main.EntitySpriteDraw(textureSmall, Projectile.Center - Main.screenPosition, baseFrame, lightColor, Projectile.rotation, baseFrame.Size() * 0.5f, Projectile.scale * squishFactor, 0, 0);
                Main.EntitySpriteDraw(textureSmall, Projectile.Center - Main.screenPosition, glowFrame, bloomColor, Projectile.rotation, glowFrame.Size() * 0.5f, Projectile.scale * squishFactor, 0, 0);
                Main.EntitySpriteDraw(glow, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 12f - Main.screenPosition, null, bloomColor * 0.1f, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * squishFactor * new Vector2(1.5f, 2f), 0, 0);
            }

            return false;
        }

        public static Texture2D textureTentacle;

        public void DrawTentacles(Color lightColor, Color growColor)
        {
            Texture2D glow = AssetDirectory.Textures.Glow.Value;

            float tentaCount = 2;
            for (int j = 0; j < tentaCount; j++)
            {
                int dir = j > 0 ? 1 : -1;

                float rot = Projectile.rotation + MathHelper.PiOver2;
                Vector2 pos = Projectile.Center + new Vector2(10 * dir, 18).RotatedBy(Projectile.rotation);
                Vector2 stick = (rot.ToRotationVector2() * 12 - Projectile.velocity * 0.05f) * (0.5f + Projectile.scale * 0.5f);
                int segments = 10;

                Vector2 lastPos = pos;

                for (int i = 0; i < segments; i++)
                {
                    float prog = i / (float)segments;
                    int segFrame = Math.Clamp((int)(prog * 5f), 1, 3);
                    if (i == 0)
                        segFrame = 0;
                    if (i == segments - 1)
                        segFrame = 4;

                    Rectangle frame = textureTentacle.Frame(3, 5, 0, segFrame);
                    Rectangle glowFrame = textureTentacle.Frame(3, 5, 1, segFrame);

                    Vector2 nextStick = stick.RotatedBy(oldVels[i].RotatedBy(-Projectile.rotation).ToRotation() + MathHelper.PiOver2).RotatedBy((float)Math.Sin((Projectile.localAI[0] * 0.2 - i * 0.8f) % MathHelper.TwoPi) * dir * 0.15f * (0.5f + i * 0.5f) - dir * 0.1f);
                    float stickRot = lastPos.AngleTo(lastPos + nextStick);
                    Vector2 stretch = new Vector2(1f, 0.5f + lastPos.Distance(lastPos + nextStick) / 16f) * MathHelper.Lerp(Projectile.scale, 1f, i / (float)segments);
                    lastPos += nextStick;

                    float bloomScale = (float)Math.Pow(prog, 1.25f);
                    Main.EntitySpriteDraw(textureTentacle, lastPos - Main.screenPosition, frame, lightColor, stickRot - MathHelper.PiOver2, frame.Size() * 0.5f, stretch, 0, 0);
                    Main.EntitySpriteDraw(textureTentacle, lastPos - Main.screenPosition, glowFrame, growColor * bloomScale * 1.2f, stickRot - MathHelper.PiOver2, frame.Size() * 0.5f, stretch, 0, 0);
                    Main.EntitySpriteDraw(glow, lastPos + oldVels[i] * 0.2f - Main.screenPosition, null, growColor * bloomScale * 0.2f, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * new Vector2(1f, 1.5f) * bloomScale, 0, 0);

                }
            }
        }
    }
}
