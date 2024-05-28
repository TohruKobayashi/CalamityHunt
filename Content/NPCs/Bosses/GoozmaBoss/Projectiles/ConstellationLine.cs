using System;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.NPCs.Bosses.GoozmaBoss.Projectiles
{
    public class ConstellationLine : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2000;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.manualDirectionChange = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Start => ref Projectile.ai[1];
        public ref float End => ref Projectile.ai[2];

        public override void AI()
        {
            if (Start < 0 || Start > Main.maxProjectiles || End < 0 || End > Main.maxProjectiles) {
                Projectile.Kill();
                return;
            }
            if (!Main.projectile[(int)Start].active || Main.projectile[(int)Start].type != ModContent.ProjectileType<ConstellationStar>() || !Main.projectile[(int)End].active || Main.projectile[(int)End].type != ModContent.ProjectileType<ConstellationStar>()) {
                Projectile.Kill();
            }

            Projectile.Center = Main.projectile[(int)Start].Center;
            Projectile.rotation = Main.projectile[(int)Start].AngleTo(Main.projectile[(int)End].Center);
            Projectile.velocity = Projectile.rotation.ToRotationVector2();

            if (Time > 95 && Time < 180 && Main.rand.NextBool(5)) {
                CalamityHunt.particles.Add(Particle.Create<PrettySparkle>(particle => {
                    particle.position = Vector2.Lerp(Main.projectile[(int)Start].Center, Main.projectile[(int)End].Center, Main.rand.NextFloat());
                    particle.velocity = Main.rand.NextVector2Circular(1, 1);
                    particle.scale = Main.rand.NextFloat(0.2f, 1.2f);
                    particle.color = new Color(30, 15, 10, 0);
                }));
            }
            if (Time > 180) {
                Projectile.Kill();
            }

            Projectile.localAI[0]++;
            Time++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Time > 95 && Time < 180) {
                float point = 0f;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopRight(), targetHitbox.Size(), Main.projectile[(int)Start].Center, Main.projectile[(int)End].Center, 90, ref point);
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Microsoft.Xna.Framework.Graphics.Texture2D texture = TextureAssets.Projectile[Type].Value;
            Microsoft.Xna.Framework.Graphics.Texture2D glow = AssetDirectory.Textures.Glow[0].Value;
            Rectangle half = texture.Frame(1, 2, 0, 0);
            Rectangle glowHalf = glow.Frame(1, 2, 0, 0);

            float power = Utils.GetLerpValue(70, 95, Time, true) * Utils.GetLerpValue(180, 170, Time, true);
            Color lineColor = Color.Lerp(new Color(80, 50, 35, 0), new Color(255, 215, 180, 0), Utils.GetLerpValue(55, 65, Time, true) * 0.6f) * MathHelper.SmoothStep(0.15f + (float)Math.Sin(Projectile.localAI[0] * 0.5f) * 0.1f, 1f, Utils.GetLerpValue(80, 95, Time, true)) * power;
            Color lineGlowColor = new Color(80, 50, 35, 0) * power;
            Color bloomColor = new Color(30, 14, 8, 0) * Utils.GetLerpValue(0, 20, Time, true) * Utils.GetLerpValue(180, 175, Time, true);
            if (Time > 1) {
                float wobble = 1f + (float)Math.Sin(Projectile.localAI[0] * 0.5f % MathHelper.TwoPi) * 0.2f;

                //double end
                Vector2 distance = new Vector2(wobble * 0.3f + power * 0.1f, Projectile.Distance(Main.projectile[(int)End].Center) / (half.Height * 1.5f));
                Vector2 glowDistance = new Vector2(wobble * 0.7f * (0.4f + Utils.GetLerpValue(70, 80, Time, true) * 0.6f), Projectile.Distance(Main.projectile[(int)End].Center) / glowHalf.Height);
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, half, lineGlowColor, Projectile.rotation + MathHelper.PiOver2, half.Size() * new Vector2(0.5f, 1f), Projectile.scale * distance, 0, 0);
                Main.EntitySpriteDraw(texture, Main.projectile[(int)End].Center - Main.screenPosition, half, lineGlowColor, Projectile.rotation - MathHelper.PiOver2, half.Size() * new Vector2(0.5f, 1f), Projectile.scale * distance, 0, 0);

                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, half, lineColor, Projectile.rotation + MathHelper.PiOver2, half.Size() * new Vector2(0.5f, 1f), Projectile.scale * new Vector2(0.33f, 1f) * distance, 0, 0);
                Main.EntitySpriteDraw(texture, Main.projectile[(int)End].Center - Main.screenPosition, half, lineColor, Projectile.rotation - MathHelper.PiOver2, half.Size() * new Vector2(0.5f, 1f), Projectile.scale * new Vector2(0.33f, 2f) * distance, 0, 0);

                Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glowHalf, bloomColor * wobble, Projectile.rotation + MathHelper.PiOver2, glowHalf.Size() * new Vector2(0.5f, 1f), Projectile.scale * glowDistance, 0, 0);
                Main.EntitySpriteDraw(glow, Main.projectile[(int)End].Center - Main.screenPosition, glowHalf, bloomColor * wobble, Projectile.rotation - MathHelper.PiOver2, glowHalf.Size() * new Vector2(0.5f, 1f), Projectile.scale * glowDistance, 0, 0);

                //single
                //Vector2 distance = new Vector2(wobble * 0.2f, Projectile.Distance(Main.projectile[(int)End].Center) / texture.Height());
                //Vector2 glowDistance = new Vector2(0.5f + wobble * 0.2f, Projectile.Distance(Main.projectile[(int)End].Center) / glow.Height());
                //Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, lineColor, Projectile.rotation + MathHelper.PiOver2, texture.Size() * new Vector2(0.5f, 1f), Projectile.scale * distance, 0, 0);
                //Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition, null, bloomColor, Projectile.rotation + MathHelper.PiOver2, glow.Size() * new Vector2(0.5f, 1f), Projectile.scale * glowDistance, 0, 0);
            }

            return false;
        }
    }
}
