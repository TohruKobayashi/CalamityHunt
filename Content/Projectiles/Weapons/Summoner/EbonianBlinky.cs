using System;
using CalamityHunt.Common.Players;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Summoner
{
    public class EbonianBlinky : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            Main.projFrames[Type] = 11;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.manualDirectionChange = true;
        }

        public override bool? CanDamage() => false;

        public ref float State => ref Projectile.ai[0];
        public ref float Time => ref Projectile.ai[1];
        public ref float AttackCount => ref Projectile.ai[2];

        public Player Player => Main.player[Projectile.owner];

        public override void AI()
        {
            if (!Player.GetModPlayer<SlimeCanePlayer>().slimes || Player.dead) {
                Projectile.Kill();
            }
            else {
                Projectile.timeLeft = 2;
            }

            if (Projectile.Distance(HomePosition) > 1600) {
                State = (int)SlimeMinionState.Idle;
                Projectile.Center = HomePosition;
                Projectile.tileCollide = false;
            }

            iAmInAir = false;

            Projectile.rotation = 0f;
            int target = -1;
            Projectile.Minion_FindTargetInRange(1200, ref target, false);
            bool hasTarget = false;
            if (target > -1) {
                hasTarget = true;
                if (Main.npc[target].active && Main.npc[target].CanBeChasedBy(Projectile)) {
                    Attack(target);
                }
                else {
                    hasTarget = false;
                }
            }
            if (!hasTarget) {
                Idle();
            }

            Projectile.velocity.Y += iAmInAir ? 0.3f : 0.4f;

            if (iAmInAir && Main.rand.NextBool() && jumpTime > 0) {
                Color color = new Color(150, 160, 255, 60);
                Dust sparkle = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(13, 12), DustID.SparkForLightDisc, Main.rand.NextVector2Circular(1, 1), 0, color, 0.2f + Main.rand.NextFloat());
                sparkle.noGravity = Main.rand.NextBool(3);
                sparkle.shader = GameShaders.Armor.GetSecondaryShader(Player.cMinion, Player);
            }

            if (jumpTime > 0) {
                jumpTime--;
            }
        }

        public Vector2 HomePosition => Player.Bottom + new Vector2(-70 * Player.direction, -28);

        public bool InAir => !Collision.SolidCollision(Player.MountedCenter - new Vector2(20, 0), 40, 150, true);

        public bool iAmInAir;

        public int jumpTime;

        public void Idle()
        {
            Time = 0;
            Projectile.tileCollide = true;
            if (InAir) {
                iAmInAir = true;
            }

            bool tooFar = Projectile.Distance(HomePosition) > 900 && State != (int)SlimeMinionState.Attacking;
            if (tooFar) {
                State = (int)SlimeMinionState.IdleMoving;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(HomePosition).SafeNormalize(Vector2.Zero) * Projectile.Distance(HomePosition) * 0.1f, 0.2f);
                Projectile.rotation = Projectile.velocity.X * 0.02f;
                if (Projectile.velocity.Y > 0) {
                    Projectile.frame = 5;
                }
                else {
                    Projectile.frame = 0;
                }
            }

            Projectile.velocity.X *= 0.6f;

            if (Math.Abs(Projectile.Center.X - HomePosition.X) > 4 || InAir) {
                State = (int)SlimeMinionState.IdleMoving;
                Projectile.velocity.X = (HomePosition.X - Projectile.Center.X) * 0.1f;
            }

            if (Projectile.Bottom.Y > HomePosition.Y + 30 && InAir && Projectile.velocity.Y >= 0) {
                Jump(-6 - Math.Max(Math.Abs(HomePosition.X - Projectile.Center.X) * 0.01f + (iAmInAir ? Math.Abs(HomePosition.Y - Projectile.Center.Y) * 0.026f : 0) + 0.5f, 0), iAmInAir);
            }

            if (State == (int)SlimeMinionState.IdleMoving) {
                if (++Projectile.frameCounter >= 7) {
                    Projectile.frameCounter = 0;
                    Projectile.frame = Math.Clamp(Projectile.frame + 1, 0, 5);
                }
            }
            else {
                Projectile.frameCounter = 0;
                Projectile.frame = 0;
            }

            Projectile.direction = Math.Abs(Projectile.velocity.X) < 3f ? Player.direction : Math.Sign(Projectile.velocity.X);
        }

        public void Attack(int whoAmI)
        {
            NPC target = Main.npc[whoAmI];
            int maxTime = Player.GetModPlayer<SlimeCanePlayer>().ValueFromSlimeRank(9, 8, 7, 6, 5);
            int shootCount = 1;

            bool targetInAir = !Collision.SolidCollision(target.position - new Vector2(5, 150), target.width + 10, target.height + 300);
            if (targetInAir) {
                iAmInAir = true;
            }

            if (Projectile.Distance(target.Center) > 400 || State == (int)SlimeMinionState.IdleMoving) {
                State = (int)SlimeMinionState.IdleMoving;

                if (++Projectile.frameCounter >= 7) {
                    Projectile.frameCounter = 0;
                    Projectile.frame = Math.Clamp(Projectile.frame + 1, 0, 5);
                }
            }

            if (Projectile.Distance(target.Center) < 350 && AttackCount < 3) {
                State = (int)SlimeMinionState.Attacking;
            }

            if (Projectile.Bottom.Y > target.Center.Y + 30 && targetInAir && Projectile.velocity.Y >= 0 && Time % maxTime == 0) {
                Jump(-6 - Math.Max(Math.Abs(HomePosition.X - Projectile.Center.X) * 0.01f + (iAmInAir ? Math.Abs(target.Center.Y - Projectile.Center.Y) * 0.02f : 0) + 0.5f, 0), iAmInAir);
            }

            if (Math.Abs(Projectile.Center.X - target.Center.X) > 40) {
                Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, State == (int)SlimeMinionState.Attacking ? 0f : (target.Center.X - Projectile.Center.X) * 0.1f, 0.05f);
            }

            if (State == (int)SlimeMinionState.Attacking && AttackCount < 3) {
                Projectile.velocity.X *= 0.96f;
                if (Projectile.velocity.Y > 0) {
                    Projectile.velocity.Y *= 0.9f;
                }

                Projectile.direction = Projectile.Center.X > target.Center.X ? -1 : 1;

                Time++;
                if (++Projectile.frameCounter >= maxTime) {
                    Projectile.frameCounter = 0;
                    Projectile.frame = Math.Clamp(Projectile.frame + 1, 6, 11);
                    if (Projectile.frame == 11) {
                        Projectile.frame = 6;
                    }
                }

                if (Time == maxTime * 3 + 2) {
                    SoundStyle burp = SoundID.NPCDeath12 with { MaxInstances = 0, Pitch = 0.9f, PitchVariance = 0.2f, Volume = 0.3f };
                    SoundEngine.PlaySound(burp, Projectile.Center);

                    for (int i = 0; i < shootCount; i++) {
                        Projectile spikeBall = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.DirectionTo(target.Center).SafeNormalize(Vector2.Zero), ModContent.ProjectileType<BlinkySpikeBall>(), Projectile.damage, Projectile.knockBack, Player.whoAmI);
                        spikeBall.ai[0] = -Main.rand.Next(20, 30);
                        spikeBall.ai[2] = target.whoAmI;
                    }
                }

                if (Time >= maxTime * 5) {
                    if (iAmInAir) {
                        AttackCount++;
                    }
                    else {
                        AttackCount = 0;
                    }

                    Time = 0;
                }
            }
            else {
                State = (int)SlimeMinionState.IdleMoving;
                Time = 0;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (State == (int)SlimeMinionState.IdleMoving) {
                if (Projectile.velocity.Y >= 0) {
                    Jump(-6 - Math.Max(Math.Abs(HomePosition.X - Projectile.Center.X) * 0.01f + (iAmInAir ? Math.Abs(HomePosition.Y - Projectile.Center.Y) * 0.026f : 0) + 0.5f, 0), iAmInAir);
                }
            }

            return false;
        }

        public void Jump(float height, bool air)
        {
            jumpTime = 22;
            if (air) {
                Color color = new Color(150, 160, 255, 60);
                color.A = 0;
                CalamityHunt.particles.Add(Particle.Create<MicroShockwave>(particle => {
                    particle.position = Projectile.Bottom;
                    particle.velocity = Vector2.Zero;
                    particle.scale = 1.5f;
                    particle.color = color;
                    particle.secondColor = new Color(255, 255, 168, 120);
                    particle.shader = GameShaders.Armor.GetSecondaryShader(Player.cMinion, Player);
                }));

                for (int i = 0; i < Main.rand.Next(3, 7); i++) {
                    Dust sparkle = Dust.NewDustPerfect(Projectile.Bottom + Main.rand.NextVector2Circular(9, 4), DustID.SparkForLightDisc, Main.rand.NextVector2Circular(3, 1) - Vector2.UnitY * (i + 1) * 0.7f, 0, color, 1f + Main.rand.NextFloat());
                    sparkle.noGravity = Main.rand.NextBool(3);
                    sparkle.shader = GameShaders.Armor.GetSecondaryShader(Player.cMinion, Player);

                }

                SoundEngine.PlaySound(SoundID.Item24 with { MaxInstances = 0, Pitch = 0.9f, PitchVariance = 0.3f, Volume = 0.4f }, Projectile.Center);
            }
            else {
                SoundEngine.PlaySound(SoundID.NPCDeath9 with { MaxInstances = 0, Pitch = 0.5f, PitchVariance = 0.3f, Volume = 0.2f }, Projectile.Center);
            }

            if (State != (int)SlimeMinionState.Attacking) {
                Projectile.frame = 0;
            }

            if (Math.Abs(Projectile.Center.X - HomePosition.X) < 4 && !air && State != (int)SlimeMinionState.Attacking) {
                State = (int)SlimeMinionState.Idle;
            }
            else {
                Projectile.velocity.Y = iAmInAir ? height * 0.9f : height;
            }

            if (AttackCount >= 3) {
                AttackCount = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(5, 11, Player.GetModPlayer<SlimeCanePlayer>().SlimeRank(), Projectile.frame, -2, -2);
            SpriteEffects direction = Projectile.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 scale = Projectile.scale * Vector2.One;

            DrawData data = new DrawData(texture, Projectile.Bottom + Vector2.UnitY * 2 - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() * new Vector2(0.5f + 0.08f * Projectile.direction, 1f), scale, direction, 0);
            data.shader = Player.cPet;
            Main.EntitySpriteDraw(data);

            return false;
        }
    }
}
