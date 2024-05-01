using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Items.Misc;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityHunt.Content.Projectiles
{
    public class ChromaticBombProjectile : ModProjectile
    {
        public override string Texture => $"{nameof(CalamityHunt)}/Content/Items/Misc/ChromaticBomb";

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            // explosives don't scale with any damage
            Projectile.damage = ChromaticBomb.Damage;
            Projectile.knockBack = ChromaticBomb.Knockback;

            for (int i = 0; i < 6; i++)
            {
                var smoke = ParticleBehavior.NewParticle(ModContent.GetInstance<CosmicSmokeParticleBehavior>(), Projectile.Center + Main.rand.NextVector2Circular(100, 100) * Projectile.scale + Projectile.velocity * (i / 6f) * 0.5f, (Main.rand.NextVector2Circular(6, 6) + Projectile.velocity * (i / 8f)) * Projectile.scale, Color.White, (1f + Main.rand.NextFloat(2f)) * Projectile.scale);
                smoke.Add(new ParticleData<string> { Value = "Cosmos" });
            }

            Projectile.rotation += 0.2f;

            if (Projectile.owner == Main.myPlayer)
            {
                DynamoRodProjectile.ExplodeandDestroyTiles(Projectile, 6, false, new List<int>() { }, new List<int>() { });
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> shadowTexture = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Content/Bosses/Goozma/Projectiles/BlackHoleBlenderShadow");

            Main.EntitySpriteDraw(shadowTexture.Value, Projectile.Center - Main.screenPosition, shadowTexture.Frame(), Color.Black * 0.5f, -Projectile.rotation * 0.7f, shadowTexture.Size() * 0.5f, Projectile.scale * 0.3f, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(shadowTexture.Value, Projectile.Center - Main.screenPosition, shadowTexture.Frame(), Color.Black * 0.2f, -Projectile.rotation * 0.5f, shadowTexture.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(shadowTexture.Value, Projectile.Center - Main.screenPosition, shadowTexture.Frame(), Color.Black, Projectile.rotation * 0.33f, shadowTexture.Size() * 0.5f, Projectile.scale * 0.2f, 0, 0);

            return false;
        }
    }
}
