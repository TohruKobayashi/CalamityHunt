using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityHunt.Content.NPCs.Bosses.GoozmaBoss.Projectiles
{
    public class EbonstoneTooth : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ebonstone Tooth");
        }

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
        public ref float OutTime => ref Projectile.ai[1];

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.direction = Projectile.velocity.X > 0 ? -1 : 1;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity = Vector2.Zero;
            Projectile.localAI[0] = Main.rand.Next(3);
        }

        public override void AI()
        {
            Projectile.scale = (float)Math.Sqrt(Utils.GetLerpValue(6, 10, Time, true) * Utils.GetLerpValue(OutTime, OutTime - 20, Time, true));
            Projectile.localAI[1]++;
            Time++;

            if (Time > OutTime) {
                Projectile.Kill();
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0;
            bool onLine = Collision.CheckAABBvLineCollision(targetHitbox.TopRight(), targetHitbox.Size(), Projectile.Center, Projectile.Center + new Vector2(0, -180 * Projectile.scale).RotatedBy(Projectile.rotation), 6, ref collisionPoint);
            return onLine && Time > 10;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ReLogic.Content.Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            ReLogic.Content.Asset<Texture2D> sparkle = TextureAssets.Extra[98];
            ReLogic.Content.Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Glow");
            Rectangle frame = texture.Frame(4, 1, (int)Projectile.localAI[0], 0);

            SpriteEffects flip = Projectile.direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Color glowColor = new Color(100, 60, 255, 0);

            if (Time < 10) {
                Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, frame, new Color(0, 0, 0, 60), Projectile.rotation, frame.Size() * new Vector2(0.5f, 1f), Utils.GetLerpValue(20, 40, Projectile.localAI[1], true) * 0.8f, flip, 0);
                Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, frame, glowColor * Utils.GetLerpValue(20, 80, Projectile.localAI[1], true), Projectile.rotation, frame.Size() * new Vector2(0.5f, 1f), Utils.GetLerpValue(20, 40, Projectile.localAI[1], true), flip, 0);
            }

            for (int i = 0; i < 4; i++) {
                Vector2 off = new Vector2(2, 0).RotatedBy(MathHelper.TwoPi / 4f * i + Projectile.rotation);
                Main.EntitySpriteDraw(glow.Value, Projectile.Center + off - Main.screenPosition, frame, glowColor, Projectile.rotation, frame.Size() * new Vector2(0.5f, 1f), Projectile.scale, flip, 0);
            }

            float sparklePower = (float)Math.Sqrt(Utils.GetLerpValue(8, 11, Time, true) * Utils.GetLerpValue(OutTime - 60, OutTime - 70, Time, true));
            Main.EntitySpriteDraw(sparkle.Value, Projectile.Center - Main.screenPosition, null, glowColor * sparklePower, Projectile.rotation, sparkle.Size() * new Vector2(0.5f, 0.66f), new Vector2(2f, 5f) * Projectile.scale, 0, 0);

            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, frame, Lighting.GetColor(Projectile.Top.ToTileCoordinates()), Projectile.rotation, frame.Size() * new Vector2(0.5f, 1f), Projectile.scale, flip, 0);

            return false;
        }
    }
}
