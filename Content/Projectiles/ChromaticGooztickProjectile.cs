using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Arch.Core.Extensions;
using CalamityHunt.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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

            if (Main.rand.NextBool(5))
            {
                var hue = ParticleBehavior.NewParticle(ModContent.GetInstance<HueLightDustParticleBehavior>(), Projectile.Center, new Vector2(0, -1), rainbowGlow, 1f);
                hue.Add(new ParticleData<float> { Value = Main.GlobalTimeWrappedHourly });
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }
    }
}
