using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class HolyExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.manualDirectionChange = true;
            Projectile.hide = true;
        }

        public ref float Time => ref Projectile.ai[0];
        
        public override void AI()
        {
            if (Time > 85)
                Projectile.Kill();

            if (Config.Instance.epilepsy)
            {
                float strength = Utils.GetLerpValue(0, 30, Time, true) * Utils.GetLerpValue(80, 40, Time, true);
                if (Time % 3 == 0)
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2CircularEdge(1, 1), strength * 70, 15, 5));
            }

            if (Time == 0)
            {
                SoundStyle explosion = AssetDirectory.Sounds.Slime.PixieBallExplode;
                SoundEngine.PlaySound(explosion, Projectile.Center);
            }
            if (Time == 35)
            {
                SoundStyle ringing = AssetDirectory.Sounds.Goozma.EarRinging;
                SoundEngine.PlaySound(ringing.WithVolumeScale(2f), Projectile.Center);
            }

            bool shouldParticle = !Config.Instance.epilepsy ? Time % 5 == 0 : true;

            if (Time < 50 && shouldParticle)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(100, 100);
                    float distanceScale = 2f / Math.Max(vel.Length(), 0.9f) + Main.rand.NextFloat();
                    ParticleBehavior.NewParticle(ModContent.GetInstance<Particles.PrettySparkleParticleBehavior>(), Projectile.Center, vel, Main.hslToRgb(Time / 50f, 0.5f, 0.5f, 128), 1f + distanceScale);
                }

                for (int i = 0; i < 2; i++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(200, 200);
                    float distanceScale = 3f / Math.Max(vel.Length(), 0.9f) + Main.rand.NextFloat(2f);
                    ParticleBehavior.NewParticle(ModContent.GetInstance<Particles.CrossSparkleParticleBehavior>(), Projectile.Center + vel, Vector2.One, Main.hslToRgb(Time / 50f, 0.5f, 0.5f, 128), distanceScale + Main.rand.NextFloat(2f));
                }

            }

            Time++;

            Projectile.localAI[0]++;

            Projectile.rotation += (85 - Time) / 800f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overWiresUI.Add(index);

        public static Texture2D sparkleTexture;

        public override void Load()
        {
            sparkleTexture = ModContent.Request<Texture2D>(Texture + "Sparkle", AssetRequestMode.ImmediateLoad).Value;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Config.Instance.epilepsy)
            {
                Texture2D texture = TextureAssets.Projectile[Type].Value;

                Color bloomColor = Main.hslToRgb(Time / 50f % 1f, 0.5f, 0.7f, 0) * Utils.GetLerpValue(60, 30, Time, true);
                Color sparkleColor = Main.hslToRgb(Time / 50f % 1f, 0.5f, 0.7f, 0) * Utils.GetLerpValue(70, 60, Time, true);
                float time = Utils.GetLerpValue(0, 45, Time, true);

                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, bloomColor * 0.2f, Projectile.rotation, texture.Size() * 0.5f, MathF.Pow(time * 2f, 3f) * 8f, 0, 0);
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, bloomColor * 0.3f, Projectile.rotation, texture.Size() * 0.5f, MathF.Pow(time * 2f, 3f) * 4f, 0, 0);
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, bloomColor * 0.5f, Projectile.rotation, texture.Size() * 0.5f, MathF.Pow(time * 2f, 3f) * 2f, 0, 0);

                for (int i = 0; i < 4; i++)
                    DrawSparkle(MathHelper.TwoPi / 4f * i, 4f, Projectile.Center, sparkleColor, time);


                for (int i = 0; i < 4; i++)
                    DrawSparkle(MathHelper.TwoPi / 4f * i + MathHelper.PiOver4, 2f, Projectile.Center, sparkleColor * 0.8f, time);
            
                for (int i = 0; i < 8; i++)
                    DrawSparkle(MathHelper.TwoPi / 8f * i + MathHelper.Pi / 8f, 1.5f, Projectile.Center, sparkleColor * 0.03f, time);
            }
            return false;
        }

        private void DrawSparkle(float rotation, float scale, Vector2 position, Color color, float progress)
        {
            Vector2 outward = new Vector2(1000 * MathF.Pow(progress, 3f), 0).RotatedBy(rotation);
            Vector2 realScale = new Vector2(1.5f - MathF.Pow(progress, 1.5f) * 1.5f, 1f + MathF.Pow(progress, 5f) * 5f) * scale * MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(0, 0.2f, progress, true)) * Utils.GetLerpValue(1f, 0.8f, progress, true);
            Main.EntitySpriteDraw(sparkleTexture, position + outward - Main.screenPosition, null, new Color(255, 255, 255, 0), rotation + MathHelper.PiOver2, sparkleTexture.Size() * 0.5f, 0.95f * realScale * new Vector2(0.4f, 1f), 0, 0);
            Main.EntitySpriteDraw(sparkleTexture, position + outward - Main.screenPosition, null, color, rotation + MathHelper.PiOver2, sparkleTexture.Size() * 0.5f, realScale * new Vector2(0.4f, 1f), 0, 0);
            Main.EntitySpriteDraw(sparkleTexture, position + outward - Main.screenPosition, null, color * 0.2f, rotation + MathHelper.PiOver2, sparkleTexture.Size() * 0.5f, realScale * new Vector2(0.7f, 1.5f), 0, 0);
            Main.EntitySpriteDraw(sparkleTexture, position + outward - Main.screenPosition, null, color * 0.1f, rotation + MathHelper.PiOver2, sparkleTexture.Size() * 0.5f, realScale * new Vector2(1f, 3f), 0, 0);
        }
    }
}
