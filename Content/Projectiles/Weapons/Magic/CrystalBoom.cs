using CalamityHunt.Content.Items.Rarities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CalamityHunt.Content.Items.Weapons.Magic;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Terraria.GameContent;

namespace CalamityHunt.Content.Projectiles.Weapons.Magic
{
    public class CrystalBoom : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 1;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Size => ref Projectile.ai[1];

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.scale = MathF.Sqrt(Utils.GetLerpValue(0, 35, Time, true)) * (Size + 1f);

            if (Time == 0)
                Projectile.localAI[1] = Main.rand.NextFloat(-1f, 1f);

            if (Time > 35)
                Projectile.Kill();

            Time++;
            Projectile.localAI[0]++;
            Projectile.rotation += Projectile.localAI[1] * 0.1f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Point checkPoint = (Projectile.Center + Projectile.DirectionTo(targetHitbox.Center()) * 120f * Projectile.scale).ToPoint();
            if (targetHitbox.Contains(checkPoint))
                return true;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D glow = AssetDirectory.Textures.Glow.Value;
            float scale = Projectile.scale * 3f;

            Color rainbowColor = Main.hslToRgb(Projectile.localAI[0] * 0.03f % 1f, 0.5f, 0.55f, 0) * Utils.GetLerpValue(35, 15, Time, true) * 0.9f;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), rainbowColor, Projectile.rotation * 0.6f, texture.Size() * 0.5f, scale, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), rainbowColor, -Projectile.rotation, texture.Size() * 0.5f, scale * new Vector2(1.1f, 0.8f), 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), rainbowColor, Projectile.rotation * 1.3f, texture.Size() * 0.5f, scale * new Vector2(1f, 0.9f) * 1.1f, 0, 0);

            return false;
        }
    }
}
