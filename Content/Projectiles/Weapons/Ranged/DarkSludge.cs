using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
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
            Projectile.penetrate = 5;
            Projectile.tileCollide = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.noEnchantmentVisuals = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Grounded => ref Projectile.ai[1];
        public ref float StickHost => ref Projectile.ai[2];

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[2] = reader.ReadSingle();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[2]);
        }

        public override void OnSpawn(IEntitySource source)
        {
            StickHost = -1;
            Projectile.localAI[0] = Main.rand.Next(0, 2);
            Projectile.localAI[1] = Main.rand.NextFloat(0.9f, 1.1f);
            Projectile.spriteDirection = Main.rand.NextBool() ? 1 : -1;
        }

        public override void AI()
        {
            Projectile bottom = Main.projectile[(int)Projectile.localAI[2]];
            bool bottomValid = bottom != null && bottom.active && bottom.type == Type && bottom.Distance(Projectile.Center) < 30 && bottom.position.Y > Projectile.position.Y;
            
            Projectile.scale = MathF.Sqrt(Utils.GetLerpValue(3, 7, Time, true) * Utils.GetLerpValue(550, 510, Time, true)) * Projectile.localAI[1];

            Grounded = (int)Math.Clamp(Grounded, 0, 1);

            if (Grounded == 0)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();

                if (Main.rand.NextBool(50))
                    CalamityHunt.particles.Add(Particle.Create<DarkSludgeChunk>(particle => {
                        particle.position = Projectile.Top + Main.rand.NextVector2Circular(20, 10) * Projectile.scale;
                        particle.velocity = (-Vector2.UnitY.RotatedByRandom(1f) * 3 + Projectile.velocity) * Main.rand.NextFloat(0.5f, 1f);
                        particle.color = Color.White;
                        particle.scale = 0.1f + Main.rand.NextFloat();
                    }));
            }
            else
            {
                Projectile.velocity *= 0.05f;

                if (Main.rand.NextBool(150))
                    CalamityHunt.particles.Add(Particle.Create<DarkSludgeChunk>(particle => {
                        particle.position = Projectile.Top + Main.rand.NextVector2Circular(20, 10) * Projectile.scale;
                        particle.velocity = (-Vector2.UnitY.RotatedByRandom(1f) * 6 + Projectile.velocity) * Main.rand.NextFloat(0.5f, 1f);
                        particle.color = Color.White;
                        particle.scale = 0.1f + Main.rand.NextFloat();
                    }));
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
                if (!bottomValid || bottom.ai[0] > 500 || bottom.velocity != Vector2.Zero) {
                    Projectile.velocity.Y += Time > 35 ? 1f : 0.3f;
                }
            else {
                    Projectile.velocity = Vector2.Zero;
                }


            if (Projectile.velocity.Length() > 25f)
                Projectile.velocity *= 0.98f;

            //if (IgnitionLevel > 0 && Main.rand.NextBool(Math.Max(13 - (int)IgnitionLevel * 2, 1) + (int)(Utils.GetLerpValue(450, 550, Time, true) * 30))) {
            //    if (Time > 30 && Time < 500)
            //        Time--;

            //    Color flameColor = Color.Lerp(Color.Chartreuse, Color.GreenYellow, Main.rand.NextFloat());
            //    flameColor.A = 0;
            //    Particle.NewParticle(ModContent.GetInstance<MegaFlame>(), Projectile.Top + Main.rand.NextVector2Circular(40, 30) * Projectile.scale, Main.rand.NextVector2Circular(3, 2) - Vector2.UnitY, flameColor, Main.rand.NextFloat());

            //    if (Main.rand.NextBool(5)) {
            //        Dust torch = Dust.NewDustPerfect(Projectile.Top + Main.rand.NextVector2Circular(40, 30) * Projectile.scale, DustID.CursedTorch, -Vector2.UnitY.RotatedByRandom(1f) * Main.rand.NextFloat(2f), 0, Color.White, 1f + Main.rand.NextFloat(2f));
            //        torch.noGravity = true;
            //    }
            //}
            if (StickHost <= -1 && !bottomValid) {
                float pushForce = 0.05f;
                for (int k = 0; k < Main.maxProjectiles; k++) {
                    Projectile otherProj = Main.projectile[k];
                    // Short circuits to make the loop as fast as possible
                    if (!otherProj.active || k == Projectile.whoAmI)
                        continue;

                    // If the other projectile is indeed the same owned by the same player and they're too close, nudge them away.
                    bool sameProjType = otherProj.type == Projectile.type;
                    float taxicabDist = Vector2.Distance(Projectile.Center, otherProj.Center);
                    float distancegate = 44f;
                    if (sameProjType && taxicabDist < distancegate) {
                        if (Projectile.Center.Y < otherProj.Center.Y) {
                            Projectile.localAI[2] = otherProj.whoAmI;
                            break;
                        }
                    }
                }
            }

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
                    CalamityHunt.particles.Add(Particle.Create<DarkSludgeChunk>(particle => {
                        particle.position = Projectile.Top + Main.rand.NextVector2Circular(20, 10) * Projectile.scale;
                        particle.velocity = (-Vector2.UnitY.RotatedByRandom(1f) * 9 + Projectile.velocity) * Main.rand.NextFloat(0.5f, 1f);
                        particle.color = Color.White;
                        particle.scale = 0.2f + Main.rand.NextFloat();
                    }));
            }

            if (Grounded == 0 && MathF.Abs(Projectile.velocity.Y - oldVelocity.Y) > 0)
                Projectile.velocity.Y *= -1;

            return false;
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
