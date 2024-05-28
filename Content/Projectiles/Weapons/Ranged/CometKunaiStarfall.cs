using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged
{
    public class CometKunaiStarfall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Throwing;
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                DamageClass d;
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.TryFind("RogueDamageClass", out d);
                Projectile.DamageType = d;
            }
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 80;
            Projectile.extraUpdates = 4;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            float p = Utils.GetLerpValue(0, 50, Projectile.timeLeft, true) * Utils.GetLerpValue(80, 30, Projectile.timeLeft, true);
            Color randomColor = Color.Lerp(Color.Blue, Color.RoyalBlue, Main.rand.NextFloat());
            randomColor.A = 0;
            Dust d = Dust.NewDustPerfect(Projectile.Center - Projectile.velocity * Main.rand.NextFloat(), DustID.SparkForLightDisc, Projectile.velocity * 0.001f, 0, randomColor, p * (1f - p) * 3f);
            d.noGravity = true;
            Projectile.localAI[0] += Projectile.direction;
        }

        public override bool? CanDamage() => true;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D sparkle = AssetDirectory.Textures.Sparkle.Value;
            Rectangle sparkleFrame = sparkle.Frame(1, 2, 0, 0);
            float p = Utils.GetLerpValue(0, 50, Projectile.timeLeft, true) * Utils.GetLerpValue(90, 30, Projectile.timeLeft, true);
            Vector2 scale = new Vector2(p * 0.8f, MathF.Pow(p + 0.3f, 2) * 3f) * Projectile.scale;
            Main.EntitySpriteDraw(sparkle, Projectile.Center - Main.screenPosition, sparkleFrame, new Color(10, 20, 180, 10), Projectile.rotation - MathHelper.PiOver2, sparkleFrame.Size() * new Vector2(0.5f, 1f), scale * 2.5f, 0, 0);
            Main.EntitySpriteDraw(sparkle, Projectile.Center - Main.screenPosition, sparkleFrame, new Color(90, 200, 255, 0), Projectile.rotation - MathHelper.PiOver2, sparkleFrame.Size() * new Vector2(0.5f, 1f), scale * new Vector2(1.5f, 1f), 0, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), new Color(255, 255, 255, 20), Projectile.localAI[0], texture.Size() * new Vector2(0.5f, 0.55f), Projectile.scale * p * 1.66f, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), new Color(10, 20, 180, 10), Projectile.localAI[0], texture.Size() * new Vector2(0.5f, 0.55f), Projectile.scale * p * 3f, 0, 0);

            return false;
        }
    }
}
