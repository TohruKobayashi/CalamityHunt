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
    public class DivinePinky : ModProjectile
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
        public ref float AttackCD => ref Projectile.ai[2];

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

            if (iAmInAir && Main.rand.NextBool(3)) {
                Color color = Color.Lerp(new Color(130, 170, 255, 60), new Color(255, 110, 255, 60), Main.rand.Next(2));
                Dust sparkle = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(13, 12), DustID.SparkForLightDisc, Main.rand.NextVector2Circular(1, 1), 0, color, 0.2f + Main.rand.NextFloat());
                sparkle.noGravity = Main.rand.NextBool(3);
                sparkle.shader = GameShaders.Armor.GetSecondaryShader(Player.cMinion, Player);
            }

            if (AttackCD > 0) {
                AttackCD--;
            }
        }

        public Vector2 HomePosition => InAir ? Player.Bottom + new Vector2(-90 * Player.direction, -100) : Player.Bottom + new Vector2(-160 * Player.direction, -20);

        public bool InAir => !Collision.SolidCollision(Player.MountedCenter - new Vector2(20, 0), 40, 150, true);

        public bool iAmInAir;

        public void Idle()
        {
            Time = 0;
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

            Projectile.velocity.X *= 0.95f;

            if (Math.Abs(Projectile.Center.X - HomePosition.X) > 4 || InAir) {
                State = (int)SlimeMinionState.IdleMoving;
                if (!InAir) {
                    Projectile.velocity.X = (HomePosition.X - Projectile.Center.X) * 0.05f;
                }
                else {
                    Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, (HomePosition.X - Projectile.Center.X) * 0.1f, 0.1f);
                }
            }

            if (InAir) {
                Projectile.tileCollide = false;

                if (Projectile.Distance(HomePosition) > 14) {
                    Projectile.velocity += Projectile.DirectionTo(HomePosition).SafeNormalize(Vector2.Zero) * MathF.Max(0.1f, Projectile.Distance(HomePosition) * 0.005f);
                }
                else {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Main.rand.NextVector2Circular(5, 5), 0.05f);
                    Projectile.netUpdate = true;
                }
                Projectile.velocity *= 0.95f;
                Projectile.rotation = Projectile.velocity.X * 0.04f;
            }
            else {
                Projectile.tileCollide = true;
            }

            if (InAir) {
                if (++Projectile.frameCounter >= 5) {
                    Projectile.frameCounter = 0;
                    Projectile.frame = Math.Clamp(Projectile.frame + 1, 6, 12);
                    if (Projectile.frame == 12) {
                        Projectile.frame = 6;
                    }
                }
            }
            else {
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
            }

            if (!iAmInAir) {
                Projectile.velocity.Y += 0.5f;
            }

            if (Math.Abs(Projectile.velocity.X) < 1f) {
                Projectile.direction = Player.direction;
            }
            else {
                Projectile.direction = Math.Sign(Projectile.velocity.X);
            }
        }

        public void Attack(int whoAmI)
        {
            NPC target = Main.npc[whoAmI];
            int maxTime = Player.GetModPlayer<SlimeCanePlayer>().ValueFromSlimeRank(9, 9, 8, 7, 7);
            int attackCD = Player.GetModPlayer<SlimeCanePlayer>().ValueFromSlimeRank(60, 50, 45, 40, 30);
            float radius = Player.GetModPlayer<SlimeCanePlayer>().ValueFromSlimeRank(80, 120, 180, 240, 280);
            iAmInAir = true;
            Projectile.tileCollide = false;

            if (Projectile.Distance(target.Center) <= 300) {
                State = (int)SlimeMinionState.Attacking;
            }

            if (Projectile.Distance(target.Center) > 300 || State == (int)SlimeMinionState.IdleMoving || AttackCD > 0) {
                State = (int)SlimeMinionState.IdleMoving;

                if (++Projectile.frameCounter >= 5) {
                    Projectile.frameCounter = 0;
                    Projectile.frame = Math.Clamp(Projectile.frame + 1, 6, 12);
                    if (Projectile.frame == 12) {
                        Projectile.frame = 6;
                    }
                }

                Projectile.velocity += Projectile.DirectionTo(target.Center).SafeNormalize(Vector2.Zero) * MathF.Max(0.1f, Projectile.Distance(target.Center) * 0.05f);
                Projectile.velocity *= 0.9f;
            }

            if (State == (int)SlimeMinionState.Attacking && AttackCD == 0) {
                if (Projectile.Distance(target.Center) > 100) {
                    Projectile.velocity += Projectile.DirectionTo(target.Center).SafeNormalize(Vector2.Zero) * Projectile.Distance(target.Center) * 0.03f;
                }
                else if (Main.myPlayer == Projectile.owner) {
                    Projectile.velocity += Main.rand.NextVector2Circular(2, 2);
                    Projectile.netUpdate = true;
                }

                Projectile.velocity *= 0.85f;


                Time++;

                if (++Projectile.frameCounter >= maxTime) {
                    Projectile.frameCounter = 0;
                    Projectile.frame = Math.Clamp(Projectile.frame + 1, 11, 17);
                    if (Projectile.frame == 17) {
                        Projectile.frame = 11;
                    }
                }

                if (Time == maxTime * 3 + 5) {
                    //117, 67, 68
                    SoundStyle ray = SoundID.Item15 with { MaxInstances = 0, Pitch = -1f, PitchVariance = 0.2f, Volume = 0.5f };
                    SoundEngine.PlaySound(ray, Projectile.Center);

                    Projectile shine = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PinkyLight>(), Projectile.damage, Projectile.knockBack, Player.whoAmI);
                    shine.ai[1] = radius;
                    shine.ai[2] = Projectile.whoAmI;
                }

                if (Time >= maxTime * 6) {
                    Time = 0;
                    AttackCD = attackCD;
                }
            }

            if (Math.Abs(Projectile.velocity.X) > 0) {
                Projectile.direction = Math.Sign(Projectile.velocity.X);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (State == (int)SlimeMinionState.IdleMoving) {
                if (Projectile.velocity.Y >= 0) {
                    Jump(-7 - Math.Max(Math.Abs(HomePosition.X - Projectile.Center.X) * 0.01f + (iAmInAir ? Math.Abs(HomePosition.Y - Projectile.Center.Y) * 0.026f : 0) + 0.5f, 0), iAmInAir);
                }
            }

            return false;
        }

        public void Jump(float height, bool air)
        {
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
                SoundEngine.PlaySound(SoundID.NPCDeath9 with { MaxInstances = 0, Pitch = -0.6f, PitchVariance = 0.3f, Volume = 0.3f }, Projectile.Center);
            }

            Projectile.frame = 0;

            if (Math.Abs(Projectile.Center.X - HomePosition.X) < 4 && !air) {
                State = (int)SlimeMinionState.Idle;
            }
            else {
                Projectile.velocity.Y = iAmInAir ? height * 0.9f : height;
            }

            if (AttackCD >= 3) {
                AttackCD = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.Lerp(lightColor, Color.White, 0.4f);

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(5, 17, Player.GetModPlayer<SlimeCanePlayer>().SlimeRank(), Projectile.frame, -2, -2);
            SpriteEffects direction = Projectile.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 scale = Projectile.scale * Vector2.One;

            DrawData data = new DrawData(texture, Projectile.Bottom + Vector2.UnitY * 12 - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() * new Vector2(0.5f, 1f), scale, direction, 0);
            data.shader = Player.cPet;
            Main.EntitySpriteDraw(data);

            return false;
        }
    }
}
