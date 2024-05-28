using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.NPCs.Bosses.GoozmaBoss.Projectiles
{
    public class ToxicSludge : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Toxic Sludge");
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 180;
        }

        public ref float Time => ref Projectile.ai[0];

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.localAI[0] = Main.rand.NextFloat(0.9f, 1.15f);
            Projectile.localAI[1] = Main.rand.Next(250);
            Projectile.rotation = Main.rand.NextFloat(-1f, 1f);
        }

        public override void AI()
        {
            Projectile.scale = (float)Math.Sqrt(Utils.GetLerpValue(-5, 12, Time, true)) * Projectile.localAI[0];
            Projectile.velocity.Y -= Main.rand.NextFloat(0.05f, 0.4f);

            if (Main.rand.NextBool(9)) {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24), DustID.Corruption, Main.rand.NextVector2Circular(5, 5), 0, Color.White, 1.5f).noGravity = true;
            }

            if (Main.rand.NextBool()) {
                Color color = Color.Lerp(Color.DarkGreen, Color.DarkOrchid, 0.7f + Main.rand.NextFloat(0.3f));
                color.A /= 2;
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24), 4, Projectile.velocity * 0.5f, 150, color, 2f).noGravity = true;
            }

            Time++;
            Projectile.rotation += 0.07f * Projectile.direction;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Time > 20) {
                return base.Colliding(projHitbox, targetHitbox);
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D eyeTexture = EbonianBehemuck.eyeTexture;
            Texture2D bloom = AssetDirectory.Textures.Glow[0].Value;
            Rectangle frame = texture.Frame(1, 2, 0, 0);
            Rectangle glowFrame = texture.Frame(1, 2, 0, 1);

            Color glowColor = new Color(100, 60, 255, 0);
            float opacity = 0.98f;
            float scaleUp = 1f;
            if (Projectile.localAI[1] > 195) {
                texture = eyeTexture;
                frame = texture.Frame(8, 1, (int)(Projectile.localAI[1] % 8), 0);
                glowFrame = frame;
                scaleUp = 1.1f;
                opacity = 1f;
                glowColor *= 2;
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.Black * 0.05f, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * scaleUp * 1.33f, 0, 0);
            Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, null, glowColor * 0.5f, Projectile.rotation, bloom.Size() * 0.5f, Projectile.scale * scaleUp * 2f, 0, 0);
            for (int i = 0; i < 4; i++) {
                Vector2 off = new Vector2(2, 0).RotatedBy(MathHelper.TwoPi / 4f * i + ProjAIStyleID.Rainbow);
                Main.EntitySpriteDraw(texture, Projectile.Center + off - Main.screenPosition, glowFrame, glowColor, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * scaleUp, 0, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, lightColor * opacity, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * scaleUp, 0, 0);

            return false;
        }
    }
}
