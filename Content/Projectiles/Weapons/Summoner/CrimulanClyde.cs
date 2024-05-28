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
    public class CrimulanClyde : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            Main.projFrames[Type] = 1;
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

        public override bool? CanDamage() => State == (int)SlimeMinionState.Attacking && Time > 0;

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
            Projectile.damage = Player.GetModPlayer<SlimeCanePlayer>().highestOriginalDamage;
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

            if (iAmInAir && Main.rand.NextBool() && jumpTime > 0) {
                Color color = new Color(255, 150, 150, 60);
                Dust sparkle = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(13, 12), DustID.SparkForLightDisc, Main.rand.NextVector2Circular(1, 1), 0, color, 0.2f + Main.rand.NextFloat());
                sparkle.noGravity = Main.rand.NextBool(3);
                sparkle.shader = GameShaders.Armor.GetSecondaryShader(Player.cMinion, Player);
            }

            if (jumpTime > 0) {
                jumpTime--;
            }
        }

        public Vector2 HomePosition => Player.Bottom + new Vector2(-116 * Player.direction, -28);

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
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(HomePosition).SafeNormalize(Vector2.Zero) * Projectile.Distance(HomePosition) * 0.05f, 0.1f);
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
                Projectile.velocity.X = (HomePosition.X - Projectile.Center.X) * 0.07f;
            }

            if (Projectile.Bottom.Y > HomePosition.Y + 30 && InAir && Projectile.velocity.Y >= 0) {
                Jump(-5 - Math.Max(Math.Abs(HomePosition.X - Projectile.Center.X) * 0.01f + (iAmInAir ? Math.Abs(HomePosition.Y - Projectile.Center.Y) * 0.026f : 0) + 0.5f, 0), iAmInAir);
            }

            if (State == (int)SlimeMinionState.IdleMoving) {
                if (++Projectile.frameCounter >= 8) {
                    Projectile.frameCounter = 0;
                    Projectile.frame = Math.Clamp(Projectile.frame + 1, 0, 5);
                }
            }
            else {
                Projectile.frameCounter = 0;
                Projectile.frame = 0;
            }

            Projectile.velocity.Y += iAmInAir ? 0.25f : 0.3f;

            if (Math.Abs(Projectile.velocity.X) < 3f) {
                Projectile.direction = Player.direction;
            }
            else {
                Projectile.direction = Math.Sign(Projectile.velocity.X);
            }
        }

        public void Attack(int whoAmI)
        {
            NPC target = Main.npc[whoAmI];
            iAmInAir = true;

            if (Projectile.Distance(target.Center) < 250) {
                State = (int)SlimeMinionState.Attacking;
            }

            if (Projectile.Distance(target.Center) > 400 || State == (int)SlimeMinionState.IdleMoving) {
                State = (int)SlimeMinionState.IdleMoving;

                if (++Projectile.frameCounter >= 7) {
                    Projectile.frameCounter = 0;
                    Projectile.frame = Math.Clamp(Projectile.frame + 1, 0, 5);
                }

                if (Projectile.Bottom.Y > target.Center.Y + 30 && Projectile.velocity.Y >= 0) {
                    Jump(-5 - Math.Max(Math.Abs(HomePosition.X - Projectile.Center.X) * 0.01f + (iAmInAir ? Math.Abs(target.Center.Y - Projectile.Center.Y) * 0.02f : 0) + 0.5f, 0), iAmInAir);
                }
            }

            Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, State == (int)SlimeMinionState.Attacking ? 0f : (target.Center.X - Projectile.Center.X) * 0.03f, 0.1f);

            if (State == (int)SlimeMinionState.Attacking) {
                Time++;
                Projectile.tileCollide = false;

                if (Time < 0) {
                    if (++Projectile.frameCounter >= 8) {
                        Projectile.frameCounter = 0;
                        Projectile.frame = Math.Clamp(Projectile.frame + 1, 9, 10);
                    }

                    Projectile.velocity *= 0.99f;
                    Vector2 top = target.Top - new Vector2(0, 60);
                    Vector2 newVel = Projectile.DirectionTo(top).SafeNormalize(Vector2.Zero) * Projectile.Distance(top) * 0.01f;
                    newVel.X *= 0.1f;
                    Projectile.velocity += newVel;
                }
                else {
                    Projectile.damage = 2;
                    Projectile.tileCollide = false;
                    Projectile.velocity = Projectile.DirectionTo(target.Center - new Vector2(0, 10)) * (Time + 0.1f) * Projectile.Distance(target.Center) * 0.02f;
                    Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.velocity.ToRotation() - MathHelper.PiOver2, 0.5f);

                    if (Time == 0) {
                        Projectile.frame = 5;
                    }

                    if (++Projectile.frameCounter >= 4) {
                        Projectile.frameCounter = 0;
                        Projectile.frame = Math.Clamp(Projectile.frame + 1, 6, 8);
                    }
                }
            }
            else {
                State = (int)SlimeMinionState.IdleMoving;
                Time = 0;
            }

            if (State != (int)SlimeMinionState.Attacking) {
                Projectile.velocity.Y++;
            }

            if (Math.Abs(Projectile.velocity.X) > 0) {
                Projectile.direction = Math.Sign(Projectile.velocity.X);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (State == (int)SlimeMinionState.Attacking) {
                Projectile.frame = 9;
                Projectile.velocity.Y = -Math.Sign(Projectile.velocity.Y) * Math.Max(Math.Abs(Projectile.velocity.Y), 8) * Main.rand.NextFloat(0.6f, 1.1f);
                Projectile.velocity.X = Math.Sign(Projectile.velocity.X) * Math.Max(Math.Abs(Projectile.velocity.X), 8) * Main.rand.NextFloat(0.7f, 3f);
                Time = -Player.GetModPlayer<SlimeCanePlayer>().ValueFromSlimeRank(25, 20, 18, 16, 15);

                SoundStyle bounce = SoundID.Item154 with { MaxInstances = 0, Pitch = 0.8f, PitchVariance = 0.2f, Volume = 0.5f };
                SoundEngine.PlaySound(bounce, Projectile.Center);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (State == (int)SlimeMinionState.IdleMoving) {
                if (Projectile.velocity.Y >= 0) {
                    Jump(-5 - Math.Max(Math.Abs(HomePosition.X - Projectile.Center.X) * 0.01f + (iAmInAir ? Math.Abs(HomePosition.Y - Projectile.Center.Y) * 0.026f : 0) + 0.5f, 0), iAmInAir);
                }
            }

            return false;
        }

        public void Jump(float height, bool air)
        {
            jumpTime = 24;
            if (air) {
                Color color = new Color(255, 150, 150, 0);

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

                SoundEngine.PlaySound(SoundID.Item24 with { MaxInstances = 0, Pitch = 0.6f, PitchVariance = 0.3f, Volume = 0.4f }, Projectile.Center);
            }
            else {
                SoundEngine.PlaySound(SoundID.NPCDeath9 with { MaxInstances = 0, Pitch = -0.1f, PitchVariance = 0.3f, Volume = 0.3f }, Projectile.Center);
            }

            Projectile.frame = 0;

            if (Math.Abs(Projectile.Center.X - HomePosition.X) < 4 && !air) {
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

            DrawData data = new DrawData(texture, Projectile.Bottom + Vector2.UnitY * 2 - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() * new Vector2(0.5f, 1f), scale, direction, 0);
            data.shader = Player.cPet;
            Main.EntitySpriteDraw(data);

            return false;
        }
    }
}
