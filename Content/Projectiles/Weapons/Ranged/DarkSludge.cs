using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged
{
    public partial class DarkSludge : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = 10;
            Projectile.tileCollide = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.noEnchantmentVisuals = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Grounded => ref Projectile.ai[1];
        public ref float StickHost => ref Projectile.ai[2];

        public override void OnSpawn(IEntitySource source)
        {
            StickHost = -1;
            Projectile.localAI[0] = Main.rand.Next(0, 2);
            Projectile.localAI[1] = Main.rand.NextFloat(0.9f, 1.1f);
            Projectile.spriteDirection = Main.rand.NextBool() ? 1 : -1;
        }

        public override void AI()
        {
            Projectile.scale = MathF.Sqrt(Utils.GetLerpValue(3, 7, Time, true) * Utils.GetLerpValue(550, 510, Time, true)) * Projectile.localAI[1];

            Grounded = (int)Math.Clamp(Grounded, 0, 1);

            if (Grounded == 0)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();

                if (Main.rand.NextBool(50))
                    ParticleBehavior.NewParticle(ModContent.GetInstance<DarkSludgeChunkParticleBehavior>(), Projectile.Top + Main.rand.NextVector2Circular(20, 10) * Projectile.scale, (-Vector2.UnitY.RotatedByRandom(1f) * 3 + Projectile.velocity) * Main.rand.NextFloat(0.5f, 1f), Color.White, 0.1f + Main.rand.NextFloat());
            }
            else
            {
                Projectile.velocity *= 0.05f;

                if (Main.rand.NextBool(150))
                    ParticleBehavior.NewParticle(ModContent.GetInstance<DarkSludgeChunkParticleBehavior>(), Projectile.Top + Main.rand.NextVector2Circular(20, 10) * Projectile.scale, (-Vector2.UnitY.RotatedByRandom(1f) * 6 + Projectile.velocity) * Main.rand.NextFloat(0.5f, 1f), Color.White, 0.1f + Main.rand.NextFloat());
            }

            if (StickHost > -1)
            {
                Grounded = 1;

                if (Time < 400)
                    Time = 400;

                if (!Main.npc[(int)StickHost].active)
                    Projectile.Kill();

                Projectile.velocity = Vector2.Zero;
                Projectile.rotation = Projectile.AngleTo(Main.npc[(int)StickHost].Center);
                Projectile.Center += Main.npc[(int)StickHost].position - Main.npc[(int)StickHost].oldPosition;
            }

            if (Time == 40)
                Projectile.velocity += Main.rand.NextVector2Circular(8, 12).RotatedBy(Projectile.velocity.ToRotation());

            Projectile.extraUpdates = Time < 25 ? 1 : 0;

            if (Grounded == 0)
                Projectile.velocity.Y += Time > 35 ? 1f : 0.3f;


            if (Projectile.velocity.Length() > 25f)
                Projectile.velocity *= 0.98f;

            //if (IgnitionLevel > 0 && Main.rand.NextBool(Math.Max(13 - (int)IgnitionLevel * 2, 1) + (int)(Utils.GetLerpValue(450, 550, Time, true) * 30)))
            //{
            //    if (Time > 30 && Time < 500)
            //        Time--;

            //    Color flameColor = Color.Lerp(Color.Chartreuse, Color.GreenYellow, Main.rand.NextFloat());
            //    flameColor.A = 0;
            //    Particle.NewParticle(ModContent.GetInstance<MegaFlame>(), Projectile.Top + Main.rand.NextVector2Circular(40, 30) * Projectile.scale, Main.rand.NextVector2Circular(3, 2) - Vector2.UnitY, flameColor, Main.rand.NextFloat());

            //    if (Main.rand.NextBool(5))
            //    {
            //        Dust torch = Dust.NewDustPerfect(Projectile.Top + Main.rand.NextVector2Circular(40, 30) * Projectile.scale, DustID.CursedTorch, -Vector2.UnitY.RotatedByRandom(1f) * Main.rand.NextFloat(2f), 0, Color.White, 1f + Main.rand.NextFloat(2f));
            //        torch.noGravity = true;
            //    }
            //}

            if (Collision.SolidCollision(Projectile.Center - new Vector2(20), 40, 40))
                Grounded = 1;
            else
                Grounded = 0;

            if (Time > 550)
                Projectile.Kill();

            Time++;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Width = 80;
            hitbox.Height = 80;
            hitbox.Location = (Projectile.Center - new Vector2(40)).ToPoint();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            StickHost = target.whoAmI;
            Projectile.velocity *= 0.9f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Grounded == 0 && Projectile.velocity.Y >= 0)
            {
                Grounded = 1;

                if (Time < 200)
                    Time = 200;

                SoundEngine.PlaySound(SoundID.NPCDeath9, Projectile.Center);

                for (int i = 0; i < 2; i++)
                    ParticleBehavior.NewParticle(ModContent.GetInstance<DarkSludgeChunkParticleBehavior>(), Projectile.Top + Main.rand.NextVector2Circular(20, 10) * Projectile.scale, (-Vector2.UnitY.RotatedByRandom(1f) * 9) * Main.rand.NextFloat(0.5f, 1f), Color.White, 0.2f + Main.rand.NextFloat(0.6f));
            }

            if (Grounded == 0 && MathF.Abs(Projectile.velocity.Y - oldVelocity.Y) > 0)
                Projectile.velocity.Y *= -1;

            return false;
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
