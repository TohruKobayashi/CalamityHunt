using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityHunt.Content.Projectiles.Weapons.Summoner
{
    public class BlinkySpikeBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            if (Projectile.localAI[1] == 0) {
                WeightedRandom<int> randomEyes = new WeightedRandom<int>();
                randomEyes.Add(0, 1f);
                randomEyes.Add(1, 0.1f);
                randomEyes.Add(2, 0.1f);
                randomEyes.Add(3, 0.1f);
                randomEyes.Add(4, 0.1f);
                randomEyes.Add(5, 0.1f);
                randomEyes.Add(6, 0.1f);
                randomEyes.Add(7, 0.1f);
                Projectile.localAI[0] = randomEyes.Get();
                Projectile.localAI[1] = 1;
            }

            if (Projectile.ai[0] < 5) {
                int target = (int)Projectile.ai[2];
                bool hasTarget = false;
                if (target > -1) {
                    hasTarget = true;
                    if (Main.npc[target].active && Main.npc[target].CanBeChasedBy(Projectile)) {
                        if (Main.myPlayer == Projectile.owner) {
                            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[target].Center).SafeNormalize(Vector2.Zero).RotatedByRandom(0.2f) * 20, 0.07f);
                            Projectile.netUpdate = true;
                        }
                        if (Projectile.Distance(Main.npc[target].Center) < 100 && Projectile.ai[1] < 1) {
                            Projectile.ai[1]++;
                        }
                    }
                    else {
                        hasTarget = false;
                    }
                }

                if (!hasTarget) {
                    Projectile.Minion_FindTargetInRange(800, ref target, false);
                }
            }

            if (Projectile.ai[1] > 0) {
                Projectile.ai[0]++;
            }

            if (Projectile.ai[0] > 5) {
                Projectile.velocity *= 0.95f;

                if (Projectile.ai[1] < 2) {
                    Projectile.ai[1]++;
                    Projectile.timeLeft = 50;
                }

                if (Projectile.ai[0] == 16) {
                    SoundStyle spike = SoundID.NPCDeath12 with { MaxInstances = 0, Pitch = 0.6f, PitchVariance = 0.2f, Volume = 0.8f };
                    SoundEngine.PlaySound(spike, Projectile.Center);
                }

                if (++Projectile.frameCounter > 5) {
                    Projectile.frameCounter = 0;
                    Projectile.frame = Math.Clamp(Projectile.frame + 1, 0, 5);
                }
            }

            if (Projectile.timeLeft < 10) {
                Projectile.scale *= 0.96f;
                Dust gibs = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16), Main.rand.NextBool(2) ? DustID.Bone : DustID.CorruptGibs, Main.rand.NextVector2Circular(2, 2), 0, Color.White, 1f);
                gibs.noGravity = Main.rand.NextBool();
            }

            Projectile.rotation += Projectile.velocity.X * 0.1f * Main.rand.NextFloat(0.9f, 1.1f);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++) {
                Dust gibs = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16), Main.rand.NextBool(2) ? DustID.Bone : DustID.CorruptGibs, Main.rand.NextVector2Circular(2, 2) - Vector2.UnitY, 0, Color.White, 0.31f + Main.rand.NextFloat());
                gibs.noGravity = Main.rand.NextBool(5);
            }

            SoundStyle death = SoundID.NPCDeath11 with { MaxInstances = 0, Pitch = 0.3f, PitchVariance = 0.2f, Volume = 0.4f };
            SoundEngine.PlaySound(death, Projectile.Center);
        }

        public override bool? CanDamage() => Projectile.ai[0] > 16 && Projectile.ai[1] == 2;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[1] < 3 && Main.myPlayer == Projectile.owner) {
                Projectile.ai[1]++;
                Projectile.netUpdate = true;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float radius = MathHelper.Clamp(Projectile.Center.Distance(targetHitbox.Center.ToVector2()), 0, 80);
            Vector2 checkPoint = Projectile.Center + (targetHitbox.Center.ToVector2() - Projectile.Center).SafeNormalize(Vector2.Zero) * radius;
            if (targetHitbox.Contains(checkPoint.ToPoint())) {
                return true;
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(8, 6, (int)Projectile.localAI[0], Projectile.frame, -2, -2);
            SpriteEffects direction = Projectile.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, direction, 0);

            return false;
        }
    }
}
