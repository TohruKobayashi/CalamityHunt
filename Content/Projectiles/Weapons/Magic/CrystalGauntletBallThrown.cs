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
using CalamityHunt.Core;
using Terraria.GameContent;

namespace CalamityHunt.Content.Projectiles.Weapons.Magic
{
    public class CrystalGauntletBallThrown : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.timeLeft = 1000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.DamageType = DamageClass.Magic;
        }

        public ref float Time => ref Projectile.ai[0];

        public ref Player Owner => ref Main.player[Projectile.owner];

        public override void AI()
        {
            int target = Projectile.FindTargetWithLineOfSight(1000);
            if (target >= 0)
            {
                if (Time % 8 == 4 && Projectile.Distance(Main.npc[target].Center) < 400)
                {
                    ParticleBehavior.NewParticle(ModContent.GetInstance<CrossSparkleParticleBehavior>(), Projectile.Center + Projectile.velocity * 5f + Main.rand.NextVector2Circular(16, 16), Vector2.Zero, Main.hslToRgb(Time * 0.03f % 1f, 0.5f, 0.5f, 128), 0.5f + Main.rand.NextFloat());
                    Projectile shock = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity * 5f + Main.rand.NextVector2Circular(16, 16), Projectile.DirectionTo(Main.npc[target].Center).SafeNormalize(Vector2.Zero), ModContent.ProjectileType<CrystalLightning>(), Projectile.damage / 3, 1f, Owner.whoAmI, ai1: Projectile.whoAmI);
                    shock.ai[2] = Projectile.Distance(Main.npc[target].Center);
                }

                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 3f;
                Projectile.velocity += Projectile.DirectionTo(Main.npc[target].Center).SafeNormalize(Vector2.Zero) * 0.1f;
            }

            MagicParticles();

            Time++;
            Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
            Projectile.rotation -= Projectile.direction * 0.2f;
        }

        private void MagicParticles()
        {
            for (int i = 0; i < 2; i++)
            {
                Color glowColor = new GradientColor(CrystalGauntlets.SpectralColor, 0.1f, 0.1f).ValueAt(Time * 0.5f);
                Vector2 off = new Vector2(6f + MathF.Sin(Time * 0.5f - i * 0.02f) * 6f, 0).RotatedBy((Time - i / 2f) * 0.15f * Projectile.direction) * Projectile.scale;
                Dust mainGlow = Dust.NewDustPerfect(Projectile.Center + off, DustID.PortalBoltTrail, off.SafeNormalize(Vector2.Zero) * 1.5f * MathF.Pow(Projectile.scale, 1.5f) + Projectile.velocity, 0, glowColor, 1.5f * Projectile.scale);
                mainGlow.noGravity = true;
                mainGlow.noLightEmittence = true;
            }

            for (int i = 0; i < 3; i++)
            {
                Color glowColor = Main.hslToRgb((Time * 0.03f + i * 0.1f) % 1f, 0.5f, 0.5f, 128);
                Vector2 off = new Vector2(15 + MathF.Sin(Time * 0.1f - i * MathHelper.TwoPi / 3f) * 12f, 0).RotatedBy((Time * 0.14f + i * MathHelper.PiOver2 / 5f) * (i % 2 == 1 ? (-1f) : 1f) * Projectile.direction) * Projectile.scale;
                Dust mainGlow = Dust.NewDustPerfect(Projectile.Center + off, DustID.RainbowRod, off.SafeNormalize(Vector2.Zero) * MathF.Pow(Projectile.scale, 1.5f) + Projectile.velocity, 0, glowColor, 1.1f * Projectile.scale);
                mainGlow.noGravity = true;
                mainGlow.noLightEmittence = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D glow = AssetDirectory.Textures.Glow.Value;
            SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float scale = MathF.Sqrt(Projectile.scale) * (1f + MathF.Sin(Time * 0.8f) * 0.1f) * 0.8f;

            Color rainbowColor = Main.hslToRgb(Time * 0.03f % 1f, 0.5f, 0.7f, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), new Color(rainbowColor.R, rainbowColor.G, rainbowColor.B), Projectile.rotation, texture.Size() * 0.5f, scale * 1.3f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), rainbowColor, Projectile.rotation * 1.3f, texture.Size() * 0.5f, scale, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), rainbowColor * 0.6f, Projectile.rotation * 0.7f, texture.Size() * 0.5f, scale * 1.2f, spriteEffects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), rainbowColor, Projectile.rotation * 0.5f, glow.Size() * 0.5f, scale * 1.5f, 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), rainbowColor * 0.2f, Projectile.rotation * 0.5f, glow.Size() * 0.5f, scale * 4f, 0, 0);

            return false;
        }
    }
}
