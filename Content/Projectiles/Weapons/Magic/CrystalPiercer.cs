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
    public class CrystalPiercer : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 5;
        }

        public ref float Time => ref Projectile.ai[0];

        public override void AI()
        {
            if (Time > 25)
            {
                Projectile.velocity *= 0.96f;
                Projectile.scale *= 0.96f;
            }

            MagicParticles();

            Time++;
            Projectile.localAI[0]++;
            Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        private void MagicParticles()
        {
            if (Time == 0)
            {

            }

            for (int i = 0; i < 3; i++)
            {
                Color glowColor = Main.hslToRgb(Projectile.localAI[0] * 0.03f % 1f, 0.5f, 0.6f, 0);
                Dust mainGlow = Dust.NewDustPerfect(Projectile.Center - Projectile.velocity * (i / 3f), DustID.PortalBoltTrail, Projectile.velocity * 0.2f, 0, glowColor, 0.8f * Projectile.scale);
                mainGlow.noGravity = true;
                mainGlow.noLightEmittence = true;
            }

            //for (int i = 0; i < 4; i++)
            //{
            //    Color glowColor = Main.hslToRgb((Time * 0.03f + i * 0.1f) % 1f, 0.5f, 0.5f, 128);
            //    Vector2 off = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(Math.Sin(Time * 0.2f));
            //    Dust mainGlow = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowRod, off.SafeNormalize(Vector2.Zero) * MathF.Pow(Projectile.scale, 1.5f) + Projectile.velocity, 0, glowColor, 1.1f * Projectile.scale);
            //    mainGlow.noGravity = true;
            //    mainGlow.noLightEmittence = true;
            //}
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D glow = AssetDirectory.Textures.Glow.Value;
            float scale = MathF.Sqrt(Projectile.scale) * (1f + MathF.Sin(Time * 0.9f) * 0.08f) * 1.5f * Utils.GetLerpValue(0, 20, Time, true);

            Color rainbowColor = Main.hslToRgb(Projectile.localAI[0] * 0.03f % 1f, 0.5f, 0.55f, 0);


            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), rainbowColor, Projectile.rotation + MathHelper.PiOver2, texture.Size() * new Vector2(0.5f, 0.3f), scale * new Vector2(1f, 2f), 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), rainbowColor * 0.03f, Projectile.rotation + MathHelper.PiOver2, glow.Size() * new Vector2(0.5f, 0.3f), scale * new Vector2(4f, 5f), 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), rainbowColor * 0.08f, Projectile.rotation + MathHelper.PiOver2, glow.Size() * new Vector2(0.5f, 0.4f), scale * new Vector2(3f, 5f), 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), rainbowColor * 0.1f, Projectile.rotation + MathHelper.PiOver2, glow.Size() * 0.5f, scale * new Vector2(1f, 1.5f), 0, 0);

            return false;
        }
    }
}
