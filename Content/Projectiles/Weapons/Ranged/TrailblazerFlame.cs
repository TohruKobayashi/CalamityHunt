using System;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Buffs;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged
{
    public class TrailblazerFlame : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Flames);
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 80;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Target => ref Projectile.ai[1];

        public override void AI()
        {
            if (Time == 0) {
                Projectile.rotation = Main.rand.NextFloat(-2f, 2f);
                Target = -1;
            }

            Projectile.Resize((int)(300 * Utils.GetLerpValue(0, 80, Time, true)), (int)(300 * Utils.GetLerpValue(0, 80, Time, true)));
            Projectile.scale = MathF.Pow(Utils.GetLerpValue(5, 80, Time, true), 0.8f) * 3f;
            if (Main.myPlayer == Projectile.owner) {
                Projectile.position += (Main.player[Projectile.owner].position - Main.player[Projectile.owner].oldPosition) * Utils.GetLerpValue(20, 10, Time, true) * 0.2f;
                Projectile.netUpdate = true;
            }
            float expand = Utils.GetLerpValue(0, 80, Time, true) * 2f;

            Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0] + 3) with { A = 128 };

            Lighting.AddLight(Projectile.Center, glowColor.ToVector3() * 0.5f);

            if (Time > 75) {
                CalamityHunt.particles.Add(Particle.Create<FusionFlameParticle>(particle => {
                    particle.position = Projectile.Center;
                    particle.velocity = Projectile.velocity * Main.rand.NextFloat();
                    particle.rotation = Projectile.velocity.ToRotation();
                    particle.scale = MathF.Pow(Projectile.scale, 2.3f) + Main.rand.NextFloat(1f, 2f);
                    particle.maxTime = Main.rand.Next(25, 40);
                    particle.color = (glowColor * 0.8f);
                    particle.fadeColor = (glowColor * 0.4f);
                    particle.emitLight = true;
                    particle.dissolveSize = 0.5f;
                    particle.dissolvePower = -0.9f;
                }));
            }
            else if (Time > 6) {
                if (Main.rand.NextBool(10)) {
                    CalamityHunt.particles.Add(Particle.Create<ChromaticEnergyDust>(particle => {
                        particle.position = Projectile.Center + Main.rand.NextVector2Circular(100, 100) * expand;
                        particle.velocity = Projectile.velocity * Main.rand.NextFloat(3f);
                        particle.scale = Main.rand.NextFloat(1f, 2f);
                        particle.color = glowColor;
                        particle.colorData = new ColorOffsetData(true, Projectile.localAI[0]);
                    }));
                }

                if (Main.rand.NextBool(10)) {
                    CalamityHunt.particles.Add(Particle.Create<CrossSparkle>(particle => {
                        particle.position = Projectile.Center + Main.rand.NextVector2Circular(100, 100) * expand;
                        particle.velocity = Vector2.Zero;
                        particle.scale = Main.rand.NextFloat(1.5f);
                        particle.color = glowColor;
                    }));
                }

                CalamityHunt.particles.Add(Particle.Create<FusionFlameParticle>(particle => {
                    particle.position = Projectile.Center;
                    particle.velocity = Projectile.velocity * Main.rand.NextFloat();
                    particle.rotation = Projectile.velocity.ToRotation();
                    particle.scale = MathF.Pow(Projectile.scale, 2.3f) + Main.rand.NextFloat(1f, 2f);
                    particle.maxTime = Main.rand.Next(25, 40);
                    particle.color = (glowColor * 0.8f) with { A = (byte)(50 + Time - 6) };
                    particle.fadeColor = (glowColor * 0.4f) with { A = (byte)Math.Clamp(15 - Time, 0, 255)};
                    particle.emitLight = true;
                    particle.dissolveSize = 0.5f;
                    particle.dissolvePower = -0.9f;
                }));

                if (Main.rand.NextBool(5) && Projectile.velocity.LengthSquared() > 2f) {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(180, 180) * expand, DustID.Sand, Projectile.velocity * Main.rand.NextFloat(3f), 0, Color.Black, 0.3f + Main.rand.NextFloat());
                    dust.noGravity = true;
                }
            }

            if (Time > 20) {
                //if (Target > -1) {
                //    Projectile.velocity += Projectile.DirectionTo(Main.npc[(int)Target].Center) * Utils.GetLerpValue(1200, 0, Projectile.Distance(Main.npc[(int)Target].Center), true);
                //    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.oldVelocity.Length();
                //}
                //else {
                //    Target = Projectile.FindTargetWithLineOfSight(1200);
                //    if (Target > -1 && Main.netMode != NetmodeID.MultiplayerClient) {
                //        Projectile.netUpdate = true;
                //    }
                //}

                if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(5)) {
                    Projectile.velocity += Main.rand.NextVector2Circular(1, 1);
                    Projectile.netUpdate = true;
                }
            }

            if (Time == 30 || Time > 70) {
                Projectile.velocity += Main.rand.NextVector2Circular(1, 1);
            }

            Projectile.frame = (int)(Utils.GetLerpValue(8, 30, Time, true) * 4f + Utils.GetLerpValue(40, 90, Time, true) * 3f);
            Time++;
            Projectile.localAI[0] = Main.GlobalTimeWrappedHourly * 70f - Time * 0.5f;
        }

        /*public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0.1f;
            return false;
        }*/

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Projectile.velocity *= 0.85f;
            target.AddBuff(ModContent.BuffType<FusionBurn>(), 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Texture2D texture = TextureAssets.Projectile[Type].Value;
            //Rectangle frame = texture.Frame(1, 7, 0, Projectile.frame);

            //Color backColor = (new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]) * 0.9f) with { A = 128 };
            //Color glowColor = (Color.Lerp(new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]), Color.White, Utils.GetLerpValue(40, 20, Time, true)) * Utils.GetLerpValue(60, 30, Time, true)) with { A = 0 };

            //Color backDrawColor = backColor * Utils.GetLerpValue(80, 70, Time, true);
            //Color drawColor = glowColor * Utils.GetLerpValue(80, 50, Time, true);

            //for (int i = 0; i < 4; i++) {
            //    Color trailColor = backDrawColor * (1f - i / 4f);
            //    Vector2 off = Projectile.velocity * i * 3.5f * MathF.Sqrt(Projectile.scale) * Utils.GetLerpValue(1, 15, Time, true);
            //    Main.EntitySpriteDraw(texture, Projectile.Center - off - Main.screenPosition, frame, trailColor, Projectile.rotation + Main.GlobalTimeWrappedHourly * 5f * (1f + i / 4f) * -Projectile.direction, frame.Size() * 0.5f, Projectile.scale * 1.05f, 0, 0);
            //    Main.EntitySpriteDraw(texture, Projectile.Center - off - Main.screenPosition, frame, trailColor, Projectile.rotation + Main.GlobalTimeWrappedHourly * 5f * (1f + i / 4f) * -Projectile.direction, frame.Size() * 0.5f, Projectile.scale, 0, 0);
            //    Main.EntitySpriteDraw(texture, Projectile.Center - off - Main.screenPosition, frame, drawColor * (1f - i / 4f) * 0.66f, Projectile.rotation + Main.GlobalTimeWrappedHourly * 5f * (1f + i / 4f) * -Projectile.direction, frame.Size() * 0.5f, Projectile.scale, 0, 0);
            //}

            //Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, backDrawColor, Projectile.rotation + Main.GlobalTimeWrappedHourly * 9f * -Projectile.direction, frame.Size() * 0.5f, Projectile.scale, 0, 0);
            //Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, drawColor, Projectile.rotation + Main.GlobalTimeWrappedHourly * 9f * -Projectile.direction, frame.Size() * 0.5f, Projectile.scale * 0.6f, 0, 0);

            return false;
        }
    }
}
