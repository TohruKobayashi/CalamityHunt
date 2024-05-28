using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Linq;
using Arch.Core.Extensions;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
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
            //int owner = -1;
            //if (!Main.npc.Any(n => n.type == ModContent.NPCType<StellarGeliath>() && n.active))
            //{
            //    Projectile.active = false;
            //    return;
            //}
            //else
            //    owner = Main.npc.First(n => n.type == ModContent.NPCType<StellarGeliath>() && n.active).whoAmI;

            Projectile.velocity *= 0.9f;
            Projectile.scale += 0.1f;
            Color flameColor = Color.Lerp(Color.Lerp(new Color(150, 70, 35, 0), new Color(30, 15, 10, 0), Utils.GetLerpValue(1, 3, Time, true)), Color.DarkBlue * 0.1f, Utils.GetLerpValue(3, 8, Time, true)) * Utils.GetLerpValue(15, 5, Time, true);
            flameColor.A = 10;
            var interstellarFlame = ParticleBehavior.NewParticle(ModContent.GetInstance<CosmicSmokeParticleBehavior>(), Projectile.Center + Main.rand.NextVector2Circular(30, 30), Main.rand.NextVector2Circular(5, 5) + Projectile.velocity * Utils.GetLerpValue(0, 8, Time, true), flameColor, (1f + Main.rand.NextFloat()) * Projectile.scale);
            interstellarFlame.Add(new ParticleData<string> { Value = "Interstellar" });

            Time++;

            if (Time > 20)
                Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
