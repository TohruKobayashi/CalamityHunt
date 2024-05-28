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
            Projectile.DamageType = DamageClass.Melee;
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

            Color flameColor = Color.Lerp(Color.Chartreuse, Color.GreenYellow, Main.rand.NextFloat());
            flameColor.A = 0;
            ParticleBehavior.NewParticle(ModContent.GetInstance<MegaFlameParticleBehavior>(), Projectile.Center, Projectile.velocity, flameColor, 0.2f + Main.rand.NextFloat());

        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
