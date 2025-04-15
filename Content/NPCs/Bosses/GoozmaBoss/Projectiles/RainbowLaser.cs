using System;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Common.Utilities.Interfaces;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.NPCs.Bosses.GoozmaBoss.Projectiles
{
    public class RainbowLaser : ModProjectile, ISubjectOfNPC<Goozma>, ISubjectOfNPC<Goozmite>
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 55;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 200;
            Projectile.manualDirectionChange = true;
            Projectile.extraUpdates = 3;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Owner => ref Projectile.ai[1];
        public ref float Speed => ref Projectile.ai[2];

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.scale = (float)Math.Sqrt(Utils.GetLerpValue(0, 2, Time, true) * Utils.GetLerpValue(0, 20, Projectile.timeLeft, true));

            if (Time <= 0) {
                if (Main.myPlayer == Projectile.owner && Owner > -1) {
                    Projectile.oldVelocity = Projectile.velocity;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[(int)Owner].GetTargetData().Center) * Projectile.oldVelocity.Length(), 0.05f);
                    Projectile.direction = Main.rand.NextBool() ? -1 : 1;
                    Speed = Projectile.Distance(Main.npc[(int)Owner].GetTargetData().Center) * 0.02f * Main.rand.NextFloat(0.9f, 1.3f);
                    Projectile.netUpdate = true;
                }
            }

            if (Time == 0) {
                SoundStyle shootSound = AssetDirectory.Sounds.Goozma.GoozmiteShoot;
                SoundEngine.PlaySound(shootSound.WithVolumeScale(0.5f), Projectile.Center);
            }

            for (int i = 0; i < 3; i++) {
                if (Main.rand.NextBool(15)) {
                    CalamityHunt.Particles.Add(Particle.Create<ChromaticEnergyDust>(particle => {
                        particle.position = Projectile.Center - Projectile.velocity * i / 3f;
                        particle.velocity = Projectile.velocity * 0.5f;
                        particle.scale = 0.5f;
                        particle.color = Color.White;
                        particle.colorData = new ColorOffsetData(true, Projectile.localAI[0]);
                    }));
                }
            }

            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * Speed * Utils.GetLerpValue(-10, 150, Projectile.timeLeft, true);

            Time++;
            Projectile.localAI[0]++;
            Projectile.localAI[1]++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Time > 1) {
                return base.Colliding(projHitbox, targetHitbox);
            }
            return false;
        }

        private VertexStrip vertexStrip;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = AssetDirectory.Textures.Sparkle.Value;
            Texture2D glow = AssetDirectory.Textures.Glow[0].Value;
            Texture2D ray = AssetDirectory.Textures.Glow[2].Value;

            Color glowColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]) with { A = 0 };

            float tellStrength = (float)Math.Sqrt(Utils.GetLerpValue(0, 40, Projectile.localAI[1], true));
            Vector2 telegraphScale = new Vector2(tellStrength * 3f, (1f - tellStrength) * 0.1f);
            Main.EntitySpriteDraw(ray, Main.npc[(int)Owner].Center - Main.screenPosition, ray.Frame(), glowColor * 0.5f, Projectile.rotation, ray.Size() * new Vector2(0f, 0.5f), telegraphScale, 0, 0);
            Main.EntitySpriteDraw(ray, Main.npc[(int)Owner].Center - Main.screenPosition, ray.Frame(), glowColor * Utils.GetLerpValue(0, 20, Projectile.localAI[1], true), Projectile.rotation, ray.Size() * new Vector2(0f, 0.5f), telegraphScale * new Vector2(2f, 1f), 0, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), glowColor, Projectile.rotation + MathHelper.PiOver2, texture.Size() * 0.5f, Projectile.scale * new Vector2(1.33f, 3f), 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.White with { A = 0 }, Projectile.rotation + MathHelper.PiOver2, texture.Size() * 0.5f, Projectile.scale * new Vector2(1f, 1.66f), 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), glowColor * 0.33f, Projectile.rotation + MathHelper.PiOver2, glow.Size() * new Vector2(0.5f, 0.4f), Projectile.scale * new Vector2(1.33f, 1.66f), 0, 0);

            Color StripColor(float progress) => (new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0] + progress * 15)) with { A = 100 };
            float StripWidth(float progress) => MathF.Sqrt(1f - progress) * 30f * Projectile.scale;

            vertexStrip ??= new VertexStrip();

            vertexStrip.PrepareStrip(Projectile.oldPos, Projectile.oldRot, StripColor, StripWidth, -Main.screenPosition + Projectile.Size * 0.5f, Projectile.oldPos.Length, true);

            Effect lightningEffect = AssetDirectory.Effects.LightningBeam.Value;
            lightningEffect.Parameters["transformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            lightningEffect.Parameters["uTexture0"].SetValue(AssetDirectory.Textures.Noise[7].Value);
            lightningEffect.Parameters["uTexture1"].SetValue(AssetDirectory.Textures.Goozma.LightningGlow.Value);
            lightningEffect.Parameters["uColor"].SetValue(Color.White.ToVector4());
            lightningEffect.Parameters["uBloomColor"].SetValue(Color.DarkGray.ToVector4());
            lightningEffect.Parameters["uLength"].SetValue(Projectile.Distance(Projectile.oldPos[^1]) / 128f);
            lightningEffect.Parameters["uNoiseThickness"].SetValue(0.5f);
            lightningEffect.Parameters["uNoiseSize"].SetValue(2.5f);
            lightningEffect.Parameters["uTime"].SetValue(Projectile.localAI[0] * 0.01f);
            lightningEffect.CurrentTechnique.Passes[0].Apply();
            vertexStrip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            return false;
        }
    }
}
