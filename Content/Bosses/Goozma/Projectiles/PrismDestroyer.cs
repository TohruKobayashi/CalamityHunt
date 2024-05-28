using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class PrismDestroyer : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.manualDirectionChange = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float WhoAmI => ref Projectile.ai[1];
        public ref float Spin => ref Projectile.ai[2];
        public static readonly float Rays = 5;

        public override void AI()
        {
            int owner = -1; 
            if (!Main.npc.Any(n => n.type == ModContent.NPCType<DivineGargooptuar>() && n.active))
                Projectile.active = false;
            else
                owner = Main.npc.First(n => n.type == ModContent.NPCType<DivineGargooptuar>() && n.active).whoAmI;

            if (owner > -1)
            {
                int count = (int)Common.Systems.DifficultySystem.DifficultyBasedValue(3, 4, 5);

                Vector2 myHome = Main.npc[owner].Center + new Vector2(400 * (float)Math.Sqrt(Utils.GetLerpValue(-65, -40, Time, true)), 0).RotatedBy(Spin + MathHelper.TwoPi / count * WhoAmI);
                Projectile.Center = Vector2.Lerp(Projectile.Center, myHome, 0.2f);

            }

            if (Time < 0)
                Projectile.rotation += Projectile.direction * 0.16f * Utils.GetLerpValue(-18, -50, Time, true);
            else
                Projectile.rotation += -Projectile.direction * 0.002f;

            Projectile.scale = Utils.GetLerpValue(-2, 15, Time, true) * Utils.GetLerpValue(80, 60, Time, true) * 0.9f;
            Projectile.velocity *= 0.5f;      

            for (int i = 0; i < Rays; i++)
            {
                Color rayColor = Main.hslToRgb((Projectile.localAI[0] * 0.01f + i / Rays) % 1f, 1f, 0.7f, 0);
                DelegateMethods.v3_1 = rayColor.ToVector3() * 0.4f * Projectile.scale;

                if (Main.rand.NextBool(15))
                    ParticleBehavior.NewParticle(ModContent.GetInstance<Particles.PrettySparkleParticleBehavior>(), Projectile.Center, new Vector2(15 * Projectile.scale).RotatedBy(Projectile.rotation - MathHelper.PiOver4 + MathHelper.TwoPi / Rays * i).RotatedByRandom(0.3f), rayColor, 0.1f + Main.rand.NextFloat());

                Utils.PlotTileLine(Projectile.Center, Projectile.Center + new Vector2(400 * Projectile.scale).RotatedBy(Projectile.rotation - MathHelper.PiOver4 + MathHelper.TwoPi / Rays * i), 1f, DelegateMethods.CastLight);
            }

            Time++;

            if (Time > 85 + WhoAmI)
                Projectile.Kill();

            Projectile.localAI[0]++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Time > 0)
            {
                float point = 0;
                for (int i = 0; i < Rays; i++)
                {
                    bool wide = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + new Vector2(70 * Projectile.scale).RotatedBy(Projectile.rotation - MathHelper.PiOver4 + MathHelper.TwoPi / Rays * i), 20f, ref point);
                    bool mid = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + new Vector2(180 * Projectile.scale).RotatedBy(Projectile.rotation - MathHelper.PiOver4 + MathHelper.TwoPi / Rays * i), 12f, ref point);
                    bool tip = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + new Vector2(320 * Projectile.scale).RotatedBy(Projectile.rotation - MathHelper.PiOver4 + MathHelper.TwoPi / Rays * i), 5f, ref point);
                    if (wide || mid || tip)
                        return true;
                }
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D tellTexture = AssetDirectory.Textures.Sparkle.Value;
            Texture2D bloom = AssetDirectory.Textures.Glow.Value;

            if (Time <= 3)
            {
                float tellScale = (float)Math.Cbrt(Utils.GetLerpValue(-60, -40, Time, true) * Utils.GetLerpValue(80, -50, Time, true)) * 0.7f;
                for (int i = 0; i < Rays; i++)
                {
                    Color rainbow = Main.hslToRgb((Projectile.localAI[0] * 0.01f + i / Rays) % 1f, 1f, 0.7f, 0);

                    float rotation = Projectile.rotation + MathHelper.PiOver2 + MathHelper.TwoPi / Rays * i;
                    Main.EntitySpriteDraw(tellTexture, Projectile.Center - Main.screenPosition, null, rainbow, rotation, tellTexture.Size() * new Vector2(0.5f, 0.7f), (float)Math.Sqrt(tellScale) * new Vector2(1f, 4f), 0, 0);
                    Main.EntitySpriteDraw(tellTexture, Projectile.Center - Main.screenPosition, null, rainbow * 0.2f, rotation, tellTexture.Size() * new Vector2(0.5f, 0.75f), tellScale * new Vector2(1.3f, 15f), 0, 0);
                    Main.EntitySpriteDraw(tellTexture, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), rotation, tellTexture.Size() * new Vector2(0.5f, 0.7f), tellScale * new Vector2(1f, 3f), 0, 0);
                    float numBetweens = (int)(8f - MathHelper.Min(Rays / 2f, 8f));
                    for (int j = 1; j < numBetweens - 1; j++)
                    {
                        Color betweenRainbow = Main.hslToRgb((Projectile.localAI[0] * 0.01f + (i + (j / numBetweens)) / Rays) % 1f, 0.7f, 0.6f, 0);
                        float lerpRotation = rotation + (MathHelper.TwoPi / Rays * (j / (numBetweens - 1)));
                        float betweenScale = (2.5f - (j / (numBetweens - 1f)) * (1f - j / (numBetweens - 1f)) * 5f) * Utils.GetLerpValue(20, -30, Time, true);
                        Main.EntitySpriteDraw(tellTexture, Projectile.Center - Main.screenPosition, null, betweenRainbow * 0.2f, lerpRotation, tellTexture.Size() * new Vector2(0.5f, 0.7f), tellScale * new Vector2(0.8f, 2.2f) * betweenScale, 0, 0);
                        Main.EntitySpriteDraw(tellTexture, Projectile.Center - Main.screenPosition, null, betweenRainbow * 0.5f, lerpRotation, tellTexture.Size() * new Vector2(0.5f, 0.7f), tellScale * new Vector2(0.8f, 1.33f) * betweenScale, 0, 0);
                    }
                }
            }

            float attackScale = Projectile.scale;
            Rectangle baseFrame = texture.Frame(1, 2, 0, 0);
            Rectangle glowFrame = texture.Frame(1, 2, 0, 1);
            for (int i = 0; i < Rays; i++)
            {
                Color rainbowSolid = Main.hslToRgb((Projectile.localAI[0] * 0.01f + i / Rays) % 1f, 1f, 0.6f, 200);
                Color rainbow = Main.hslToRgb((Projectile.localAI[0] * 0.01f + i / Rays) % 1f, 1f, 0.7f, 0);

                float rotation = Projectile.rotation + MathHelper.TwoPi / Rays * i;
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, glowFrame, rainbowSolid, rotation, baseFrame.Size() * new Vector2(0.01f, 0.5f), attackScale * new Vector2(0.5f, 0.3f), 0, 0);
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, glowFrame, rainbow, rotation, baseFrame.Size() * new Vector2(0.01f, 0.5f), attackScale * new Vector2(0.6f, 0.4f), 0, 0);
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, glowFrame, rainbowSolid * 0.3f, rotation, glowFrame.Size() * new Vector2(0.05f, 0.5f), attackScale * new Vector2(0.8f, 0.5f), 0, 0);
                Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, null, rainbow * 0.3f, rotation, bloom.Size() * new Vector2(0.25f, 0.5f), 10 * attackScale * new Vector2(1.5f, 0.4f), 0, 0);
                Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, null, rainbow * 0.5f, rotation, bloom.Size() * new Vector2(0.25f, 0.5f), attackScale * new Vector2(8f, 2f), 0, 0);
            }

            Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, null, new Color(20, 5, 10, 0), Projectile.rotation, bloom.Size() * 0.5f, 10 * attackScale * 3f, 0, 0);
            Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, null, Main.hslToRgb((Projectile.localAI[0] * 0.01f + 0.3f) % 1f, 1f, 0.5f, 0) * 0.15f, Projectile.rotation, bloom.Size() * 0.5f, 10 * attackScale * 1.4f, 0, 0);
            Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, null, Main.hslToRgb((Projectile.localAI[0] * 0.01f + 0.5f) % 1f, 1f, 0.7f, 0) * 0.15f, Projectile.rotation, bloom.Size() * 0.5f, 10 * attackScale * 0.8f, 0, 0);


            return false;
        }
    }
}
