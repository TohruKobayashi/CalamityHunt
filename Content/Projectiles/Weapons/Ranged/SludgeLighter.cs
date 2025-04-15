using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged
{
    public class SludgeLighter : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 1;
            Projectile.noEnchantmentVisuals = true;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.02f;

            foreach (Projectile sludge in Main.projectile.Where(n => n.active && n.type == ModContent.ProjectileType<DarkSludge>() && n.whoAmI != Projectile.whoAmI))
            {
                if (sludge.Distance(Projectile.Center) < 20)
                {
                    sludge.ai[2]++;
                    Projectile.Kill();
                }
            }

            Dust torch = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(2, 2), DustID.CursedTorch, Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(1f, 3f), 0, Color.White, 0.1f + Main.rand.NextFloat(2f));
            torch.noGravity = true;

            CalamityHunt.Particles.SpawnParticle<FlameParticle>(particle => {
                particle.Position = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 80;
                particle.Scale = new Vector2(Main.rand.NextFloat(1f, 3f));
                particle.Velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(3f, 20f);
                particle.maxTime = Main.rand.Next(35, 40);
                particle.Color = Color.Chartreuse with { A = 20 };
                particle.fadeColor = Color.GreenYellow with { A = 30 };
                particle.emitLight = true;
            });

        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
