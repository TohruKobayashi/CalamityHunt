using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.NPCs.Bosses.GoozmaBoss.Projectiles
{
    public class InterstellarFlame : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 130;
            Projectile.height = 130;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.manualDirectionChange = true;
        }

        public ref float Time => ref Projectile.ai[0];

        public override void AI()
        {
            Projectile.velocity *= 0.9f;
            Projectile.scale += 0.1f;
            CalamityHunt.particles.Add(Particle.Create<FusionFlameParticle>(particle => {
                particle.position = Projectile.Center + Main.rand.NextVector2Circular(10, 10);
                particle.velocity = Main.rand.NextVector2Circular(6, 6) + Projectile.velocity * Utils.GetLerpValue(0, 8, Time, true);
                particle.scale = Main.rand.NextFloat(1f, 3f) * Projectile.scale;
                particle.maxTime = Main.rand.Next(16, 30);
                particle.color = Color.Lerp(Color.Coral, Color.Goldenrod, 0.45f) with { A = 115 };
                particle.fadeColor = Color.Lerp(Color.Coral * 0.3f, Color.MidnightBlue, Main.rand.NextFloat(0.4f, 0.7f)) with { A = 100 };
            }));

            Time++;

            if (Time > 20) {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
