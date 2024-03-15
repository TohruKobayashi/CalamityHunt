using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles
{
    public class ChromaticGooztickProjectile : ModProjectile
    {
        public Color rainbowGlow => new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Main.GlobalTimeWrappedHourly * 100f);
        public override string Texture => $"{nameof(CalamityHunt)}/Content/Items/Misc/ChromaticGooztick";

        public override void SetDefaults()
        {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3600;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.Length() * 0.09f * Projectile.direction;

            if (Main.rand.NextBool(5)) {
                CalamityHunt.particles.Add(Particle.Create<ChromaticEnergyDust>(particle => {
                    particle.position = Projectile.Center;
                    particle.velocity = -Vector2.UnitY;
                    particle.scale = 1f;
                    particle.color = Color.White;
                    particle.colorData = new ColorOffsetData(true, Main.GlobalTimeWrappedHourly);
                }));
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            if (Projectile.velocity.X != oldVelocity.X) {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y) {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }
    }
}
