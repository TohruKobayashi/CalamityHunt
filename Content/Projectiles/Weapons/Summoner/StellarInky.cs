using System;
using System.IO;
using CalamityHunt.Common.Players;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
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
    public class StellarInky : ModProjectile
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

        public override bool? CanDamage() => State == (int)SlimeMinionState.Attacking && AttackCount > 0 && Time >= 0;

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

            Projectile.tileCollide = false;

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

            if (iAmInAir) {
                if (Math.Abs(Projectile.velocity.Length()) > 3) {
                    Projectile.rotation += Projectile.velocity.X * 0.02f;
                    Projectile.rotation = MathHelper.WrapAngle(Projectile.rotation);
                }
                else {
                    Projectile.rotation = Utils.AngleLerp(Projectile.rotation, 0, 0.1f);
                }
            }
            else {
                Projectile.rotation = Utils.AngleLerp(Projectile.rotation, 0f, 0.5f);
            }

            if ((Main.rand.NextBool(3) && teleportTime > 0) || State == (int)SlimeMinionState.Attacking) {
                Color color = new Color(5, 30, 200, 0);
                Dust sparkle = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(13, 12), DustID.SparkForLightDisc, Main.rand.NextVector2Circular(1, 1), 0, color, 0.2f + Main.rand.NextFloat());
                sparkle.noGravity = Main.rand.NextBool(3);
                sparkle.shader = GameShaders.Armor.GetSecondaryShader(Player.cMinion, Player);
            }

            if (teleportTime > 0) {
                teleportTime--;
            }

            if (AttackCount < 0) {
                AttackCount++;
            }

            if (ringFrameCounter++ > 4) {
                ringFrameCounter = 0;
                ringFrame = (ringFrame + 1) % 4;
            }
        }

        public Vector2 HomePosition => InAir ? Player.Bottom + new Vector2(-160 * Player.direction, -60) : Player.Bottom + new Vector2(-200 * Player.direction, -20);

        public bool InAir => !Collision.SolidCollision(Player.MountedCenter - new Vector2(20, 0), 40, 150, true);

        public bool iAmInAir;

        public int teleportTime;

        public void Idle()
        {
            Time = 0;
            AttackCount = 0;

            if (InAir) {
                iAmInAir = true;
            }

            bool tooFar = Projectile.Distance(HomePosition) > 900 && State != (int)SlimeMinionState.Attacking;
            if (tooFar) {
                State = (int)SlimeMinionState.IdleMoving;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(HomePosition).SafeNormalize(Vector2.Zero) * Projectile.Distance(HomePosition) * 0.05f, 0.1f);
                {
                    Projectile.rotation += Projectile.direction * 0.2f;
                    Projectile.rotation = MathHelper.WrapAngle(Projectile.rotation);
                }

                Projectile.frame = Projectile.velocity.Y > 0 ? 5 : 0;
            }

            Projectile.velocity.X *= 0.95f;

            if (Math.Abs(Projectile.Center.X - HomePosition.X) > 4 || InAir) {
                State = (int)SlimeMinionState.IdleMoving;
                if (!InAir) {
                    Projectile.velocity.X = (HomePosition.X - Projectile.Center.X) * 0.05f;
                }
                else {
                    Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, (HomePosition.X - Projectile.Center.X) * 0.05f, 0.002f);
                    if (Projectile.velocity.Length() > 5 && Main.myPlayer == Projectile.owner) {
                        if (teleportTime++ > 150 && Main.rand.NextBool(20)) {
                            Color color = new Color(5, 10, 100, 0);
                            CalamityHunt.particles.Add(Particle.Create<MicroPortal>(particle => {
                                particle.position = Projectile.Center + Projectile.velocity * 0.1f;
                                particle.velocity = Vector2.Zero;
                                particle.scale = 1f;
                                particle.color = color;
                                particle.secondColor = new Color(255, 200, 90, 120);
                                particle.shader = GameShaders.Armor.GetSecondaryShader(Player.cPet, Player);
                            }));

                            teleportTime = 0;
                            Projectile.Center -= Projectile.velocity.RotatedByRandom(2f) * Main.rand.Next(8, 15);
                            Projectile.netUpdate = true;

                            CalamityHunt.particles.Add(Particle.Create<MicroPortal>(particle => {
                                particle.position = Projectile.Center - Projectile.velocity * 0.1f;
                                particle.velocity = Vector2.Zero;
                                particle.scale = 1f;
                                particle.color = color;
                                particle.secondColor = new Color(255, 200, 90, 120);
                                particle.shader = GameShaders.Armor.GetSecondaryShader(Player.cPet, Player);
                            }));

                            //SoundStyle warpSound = SoundID.Item135;
                        }
                    }
                }
            }

            if (InAir) {
                if (Projectile.Distance(HomePosition) > 14) {
                    Projectile.velocity += Projectile.DirectionTo(HomePosition).SafeNormalize(Vector2.Zero) * MathF.Max(0.1f, Projectile.Distance(HomePosition) * 0.005f);
                }
                else {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Main.rand.NextVector2Circular(5, 5), 0.1f);
                    Projectile.netUpdate = true;
                }
                Projectile.velocity *= 0.95f;
            }
            else {
                Projectile.tileCollide = true;
            }

            if (InAir) {
                Projectile.frame = 6;
            }
            else {
                if (State == (int)SlimeMinionState.IdleMoving) {
                    if (++Projectile.frameCounter >= 9) {
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
                Projectile.velocity.Y += 0.2f;
            }

            Projectile.direction = Math.Abs(Projectile.velocity.X) < 2f ? Player.direction : Math.Sign(Projectile.velocity.X);
        }

        public Vector2 targetPositionOffset;

        public void Attack(int whoAmI)
        {
            NPC target = Main.npc[whoAmI];
            int attackWaitTime = Player.GetModPlayer<SlimeCanePlayer>().ValueFromSlimeRank(60, 55, 42, 30, 20);
            int hitCD = Player.GetModPlayer<SlimeCanePlayer>().ValueFromSlimeRank(40, 35, 30, 25, 24);
            int maxAttacks = Player.GetModPlayer<SlimeCanePlayer>().ValueFromSlimeRank(1, 2, 3, 4, 5);

            iAmInAir = true;
            Projectile.tileCollide = false;

            if (Projectile.Distance(target.Center) < 350) {
                State = (int)SlimeMinionState.Attacking;
            }

            if (Projectile.Distance(target.Center) > 400 || State == (int)SlimeMinionState.IdleMoving) {
                State = (int)SlimeMinionState.IdleMoving;

                Projectile.frame = 6;

                Projectile.velocity += Projectile.DirectionTo(target.Center).SafeNormalize(Vector2.Zero) * Projectile.Distance(target.Center) * 0.01f;
                Projectile.velocity *= 0.94f;
                Time = 0;
            }

            if (State == (int)SlimeMinionState.Attacking) {
                //Projectile.velocity += Projectile.DirectionTo(target.Center).SafeNormalize(Vector2.Zero) * Projectile.Distance(target.Center) * 0.01f;
                //Projectile.velocity *= 0.93f;
                if (Math.Abs(Projectile.velocity.X) > 0) {
                    Projectile.direction = Math.Sign(Projectile.velocity.X);
                }

                Projectile.frame = 7;

                if (AttackCount < 0) {
                    Projectile.velocity *= 0.85f;
                }

                if (AttackCount == 0) {
                    if (Main.myPlayer == Projectile.owner) {
                        if (Time < 3) {
                            targetPositionOffset = new Vector2((Projectile.Center.X > target.Center.X ? 1 : -1) * (target.width + Main.rand.Next(95, 105)), Main.rand.Next(-25, 25));
                            Projectile.netUpdate = true;
                        }
                    }

                    if (Time >= 2) {
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center + targetPositionOffset).SafeNormalize(Vector2.Zero) * Projectile.Distance(target.Center + targetPositionOffset) * 0.1f, 0.2f);
                        Projectile.velocity += Projectile.DirectionFrom(target.Center) * 0.3f;
                        if (Time > attackWaitTime) {
                            AttackCount++;
                            Time = 0;
                        }
                    }
                }

                Time++;

                if (AttackCount > 0) {
                    if (Time == 0) {
                        if (AttackCount > maxAttacks) {
                            AttackCount = -hitCD;
                        }
                        else {
                            teleportTime = 10;

                            Color color = new Color(5, 10, 100, 0);

                            CalamityHunt.particles.Add(Particle.Create<MicroPortal>(particle => {
                                particle.position = Projectile.Center + Projectile.velocity * 0.1f;
                                particle.velocity = Vector2.Zero;
                                particle.scale = 1f;
                                particle.color = color;
                                particle.secondColor = new Color(255, 200, 90, 120);
                                particle.shader = GameShaders.Armor.GetSecondaryShader(Player.cPet, Player);
                            }));

                            targetPositionOffset += Main.rand.NextVector2Circular(1, 5);
                            Projectile.Center = target.Center + targetPositionOffset;

                            CalamityHunt.particles.Add(Particle.Create<MicroPortal>(particle => {
                                particle.position = Projectile.Center - Projectile.velocity * 0.1f;
                                particle.velocity = Vector2.Zero;
                                particle.scale = 1f;
                                particle.color = color;
                                particle.secondColor = new Color(255, 200, 90, 120);
                                particle.shader = GameShaders.Armor.GetSecondaryShader(Player.cPet, Player);
                            }));

                            //
                        }
                    }

                    if (Time > 0) {
                        Projectile.velocity = Projectile.DirectionTo(target.Center).SafeNormalize(Vector2.Zero) * Time * Projectile.Distance(target.Center) * 0.05f;
                    }
                }
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(teleportTime);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            teleportTime = reader.Read();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (State == (int)SlimeMinionState.Attacking && AttackCount > 0) {
                Time = -MathHelper.Clamp(Time, 10, 30);
                AttackCount++;
                if (Main.myPlayer == Projectile.owner) {
                    Projectile.velocity = Projectile.velocity.RotatedByRandom(1f);
                    Projectile.velocity *= 0.94f;
                    Projectile.netUpdate = true;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (State == (int)SlimeMinionState.IdleMoving) {
                if (Projectile.velocity.Y >= 0) {
                    Jump(-4 - Math.Max(Math.Abs(HomePosition.X - Projectile.Center.X) * 0.01f + (iAmInAir ? Math.Abs(HomePosition.Y - Projectile.Center.Y) * 0.026f : 0) + 0.5f, 0), iAmInAir);
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
                    particle.scale = 1f;
                    particle.color = color;
                    particle.secondColor = new Color(255, 200, 90, 120);
                    particle.shader = GameShaders.Armor.GetSecondaryShader(Player.cPet, Player);
                }));

                for (int i = 0; i < Main.rand.Next(3, 7); i++) {
                    Dust sparkle = Dust.NewDustPerfect(Projectile.Bottom + Main.rand.NextVector2Circular(9, 4), DustID.SparkForLightDisc, Main.rand.NextVector2Circular(3, 1) - Vector2.UnitY * (i + 1) * 0.7f, 0, color, 1f + Main.rand.NextFloat());
                    sparkle.noGravity = Main.rand.NextBool(3);
                }

                SoundEngine.PlaySound(SoundID.Item24 with { MaxInstances = 0, Pitch = 0.6f, PitchVariance = 0.3f, Volume = 0.4f }, Projectile.Center);
            }
            else {
                SoundEngine.PlaySound(SoundID.NPCDeath9 with { MaxInstances = 0, Pitch = -0.3f, PitchVariance = 0.3f, Volume = 0.3f }, Projectile.Center);
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

        public int ringFrame;

        public int ringFrameCounter;

        public static Texture2D hatTexture;

        public static Texture2D ringTexture;

        public override void Load()
        {
            hatTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "Hats").Value;
            ringTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "Rings").Value;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.Lerp(lightColor, Color.White, 0.5f);
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            int ringXFrame = Player.GetModPlayer<SlimeCanePlayer>().SlimeRank() switch
            {
                0 => 0,
                1 => 0,
                2 => 2,
                3 => 2,
                4 => 4,
                _ => 0
            };
            Rectangle ringFrontFrame = ringTexture.Frame(6, 4, ringXFrame, ringFrame);
            Rectangle ringBackFrame = ringTexture.Frame(6, 4, ringXFrame + 1, ringFrame);
            float ringRotation = -Projectile.velocity.X * 0.02f - Projectile.velocity.Y * 0.04f;

            Rectangle frame = texture.Frame(5, 8, Player.GetModPlayer<SlimeCanePlayer>().SlimeRank(), Projectile.frame, -2, -2);
            SpriteEffects direction = Projectile.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 scale = Projectile.scale * Vector2.One;

            int yOff = Player.GetModPlayer<SlimeCanePlayer>().SlimeRank() > 3 ? -2 : 0;
            bool ringAllowed = State != (int)SlimeMinionState.Attacking || AttackCount <= 0;
            Vector2 ringOffset = new Vector2(0, 8);
            if (ringAllowed) {
                DrawData ringBackData = new DrawData(ringTexture, Projectile.Center + ringOffset - Main.screenPosition, ringBackFrame, lightColor, ringRotation, ringBackFrame.Size() * 0.5f, scale, direction, 0);
                ringBackData.shader = Player.cPet;
                Main.EntitySpriteDraw(ringBackData);
            }

            if (Projectile.frame > 5) {
                Rectangle hatFrame = hatTexture.Frame(1, 4, 0, Player.GetModPlayer<SlimeCanePlayer>().SlimeRank() - 1);
                Vector2 hatOffset = new Vector2(0, -(18 + Projectile.velocity.Length() - Projectile.velocity.Y * 0.5f)).RotatedBy(-Projectile.velocity.X * 0.04f + (-0.75f + Projectile.velocity.Y * 0.05f) * Projectile.direction) * scale;
                float hatRotation = hatOffset.AngleFrom(Vector2.Zero) + MathHelper.PiOver2 + 0.5f * Projectile.direction;//
                DrawData hatData = new DrawData(hatTexture, Projectile.Center + hatOffset - Projectile.velocity - Main.screenPosition, hatFrame, lightColor, hatRotation, hatFrame.Size() * 0.5f, scale, direction, 0);
                hatData.shader = Player.cPet;
                Main.EntitySpriteDraw(hatData);
            }

            DrawData data = new DrawData(texture, Projectile.Bottom - Vector2.UnitY * (10 - yOff) - Main.screenPosition, frame, lightColor, Projectile.rotation, new Vector2(frame.Width * (0.5f + 0.1f * Projectile.direction), 34 + yOff), scale, direction, 0);
            data.shader = Player.cPet;
            Main.EntitySpriteDraw(data);

            if (ringAllowed) {
                DrawData ringFrontData = new DrawData(ringTexture, Projectile.Center + ringOffset - Main.screenPosition, ringFrontFrame, lightColor, ringRotation, ringFrontFrame.Size() * 0.5f, scale, direction, 0);
                ringFrontData.shader = Player.cPet;
                Main.EntitySpriteDraw(ringFrontData);
            }

            return false;
        }
    }
}
