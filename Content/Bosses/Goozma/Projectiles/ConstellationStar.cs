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
    public class ConstellationStar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.manualDirectionChange = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float WhoAmI => ref Projectile.ai[1];
        public ref float Selected => ref Projectile.ai[2];

        public override void AI()
        {
            Projectile.velocity *= 0.95f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.velocity = Projectile.velocity.RotatedBy(0.017f * Projectile.direction);

            if (Time % 83 < 4 && Time > 40)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.velocity + Main.rand.NextVector2CircularEdge(8, 8) + Main.rand.NextVector2Circular(5, 5), 0.2f);

            if (Time > 50 && Time + WhoAmI < 500 && Time % 2 == 0)
            {
                foreach (Projectile otherStar in Main.projectile.Where(n => n.active && n.type == Type && n.whoAmI != Projectile.whoAmI && n.Distance(Projectile.Center) < 300))
                {
                    otherStar.velocity += otherStar.DirectionFrom(Projectile.Center).SafeNormalize(Vector2.Zero) * 0.2f * (3f - WhoAmI);
                    Projectile.velocity += Projectile.DirectionFrom(otherStar.Center).SafeNormalize(Vector2.Zero) * 0.2f * (3f - WhoAmI);
                }
                if (Main.npc.Any(n => n.active && n.type == ModContent.NPCType<StellarGeliath>()))
                {
                    NPC owner = Main.npc.First(n => n.active && n.type == ModContent.NPCType<StellarGeliath>());

                    Projectile.velocity += Projectile.DirectionTo(owner.Center).RotatedBy(MathHelper.PiOver2) * 0.1f;
                }
                else
                    Time = Math.Max(Time, 540 - (int)(WhoAmI * 0.5f));

            }

            if (Time + (int)(WhoAmI * 0.5f) > 550)
            {
                Projectile.velocity *= 0.9f;
                Projectile.scale *= 0.9f;
            }

            if ((Time - 70) % 50 > 48)
                Selected = 0;

            if (Main.rand.NextBool(10))
                ParticleBehavior.NewParticle(ModContent.GetInstance<PrettySparkleParticleBehavior>(), Projectile.Center, Main.rand.NextVector2Circular(4, 4), new Color(30, 15, 8, 0), (0.15f + Main.rand.NextFloat()) * Projectile.scale);

            var smoke = ParticleBehavior.NewParticle(ModContent.GetInstance<CosmicSmokeParticleBehavior>(), Projectile.Center + Projectile.velocity * 2f, Main.rand.NextVector2Circular(6, 6), Color.White, (1f + Main.rand.NextFloat()) * Projectile.scale);
            smoke.Add(new ParticleData<string> { Value = "Cosmos" });

            if (Time + (int)(WhoAmI * 0.3f) > 640)
                Projectile.Kill();

            Projectile.localAI[0]++;
            Time++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Time > 30 && Time < 550)
                return base.Colliding(projHitbox, targetHitbox);
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D sparkle = AssetDirectory.Textures.Sparkle.Value;
            Texture2D glow = AssetDirectory.Textures.Glow.Value;

            float wobble = 1f + (float)Math.Sin((Projectile.localAI[0] * 0.3f) % MathHelper.TwoPi) * 0.05f;

            for (int i = 1; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                float prog = 1f - (i / (float)ProjectileID.Sets.TrailCacheLength[Type]);
                Vector2 stretch;
                if (i == 0)
                    stretch = new Vector2(prog, Projectile.position.Distance(Projectile.oldPos[i]) / texture.Height * 3.5f);
                else
                    stretch = new Vector2(prog, Projectile.oldPos[i].Distance(Projectile.oldPos[i - 1]) / texture.Height * 3.5f);

                Main.EntitySpriteDraw(sparkle, Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition, null, new Color(30, 15, 10, 0) * prog, Projectile.oldRot[i] + MathHelper.PiOver2, sparkle.Size() * 0.5f, Projectile.scale * stretch * (0.5f + prog * 0.5f), 0, 0);
            }

            Main.EntitySpriteDraw(sparkle, Projectile.Center - Main.screenPosition, null, new Color(90, 50, 35, 0), 0, sparkle.Size() * 0.5f, Projectile.scale * new Vector2(0.5f, 1.5f), 0, 0);
            Main.EntitySpriteDraw(sparkle, Projectile.Center - Main.screenPosition, null, new Color(90, 50, 35, 0), 0 + MathHelper.PiOver2, sparkle.Size() * 0.5f, Projectile.scale * new Vector2(0.5f, 2f), 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(255, 225, 170, 0), 0, texture.Size() * 0.5f, Projectile.scale * new Vector2(1f, 1f) * wobble, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(255, 225, 170, 0), 0 + MathHelper.PiOver2, texture.Size() * 0.5f, Projectile.scale * new Vector2(1f, 1.5f) * wobble, 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, new Color(50, 17, 10, 0), 0, glow.Size() * 0.5f, Projectile.scale * 2f * wobble, 0, 0);

            return false;
        }
    }
}
