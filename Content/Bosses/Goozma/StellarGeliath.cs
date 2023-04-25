﻿using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma.Projectiles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityHunt.Content.Bosses.Goozma
{
    public class StellarGeliath : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.MustAlwaysDraw[Type] = true;
            NPCID.Sets.TrailCacheLength[Type] = 10;
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.ShouldBeCountedAsBoss[Type] = false;
            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.OnFire,
                    BuffID.Ichor,
                    BuffID.CursedInferno,
                    BuffID.Poisoned,
                    BuffID.Confused
				}
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);

            //NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { };
            //NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            database.FindEntryByNPCID(Type).UIInfoProvider = new HighestOfMultipleUICollectionInfoProvider(new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<Goozma>()], true));
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("Mods.CalamityHunt.Bestiary.StellarGeliath"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.SlimeRain,
                ModLoader.HasMod("CalamityMod") ? null : BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 150;
            NPC.height = 100;
            NPC.damage = 12;
            NPC.defense = 10;
            NPC.lifeMax = 3000000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 5);
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            NPC.aiStyle = -1;
            NPC.immortal = true;
            NPC.takenDamageMultiplier = 0.33f;
            if (ModLoader.HasMod("CalamityMod"))
            {
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.Call("SetDebuffVulnerabilities", "poison", false);
                calamity.Call("SetDebuffVulnerabilities", "heat", true);
                calamity.Call("SetDefenseDamageNPC", Type, true);
                SpawnModBiomes = new int[1] { calamity.Find<ModBiome>("AbovegroundAstralBiome").Type };
            }
        }

        private enum AttackList
        {
            StarSigns,
            CosmicStomp,
            BlackHole,
            TooFar,
            Interrupt
        }

        public ref float Time => ref NPC.ai[0];
        public ref float Attack => ref NPC.ai[1];
        public ref NPC Host => ref Main.npc[(int)NPC.ai[2]];
        public ref float RememberAttack => ref NPC.ai[3];

        public NPCAimedTarget Target => NPC.GetTargetData();
        public Vector2 squishFactor = Vector2.One;

        public override void AI()
        {
            if (NPC.scale > 0.1f)
            {
                for (int i = 0; i < 6; i++)
                {
                    Particle smoke = Particle.NewParticle(Particle.ParticleType<CosmicSmoke>(), NPC.Center + Main.rand.NextVector2Circular(90, 60) * NPC.scale + NPC.velocity * (i / 6f) * 0.3f, Main.rand.NextVector2Circular(4, 4) + NPC.velocity * (i / 6f) * 0.5f, Color.White, (1.5f + Main.rand.NextFloat()) * NPC.scale);
                    smoke.data = "Cosmos";
                }
                if (Main.rand.NextBool(15))
                    Particle.NewParticle(Particle.ParticleType<PrettySparkle>(), NPC.Center + Main.rand.NextVector2Circular(90, 60) * NPC.scale, Main.rand.NextVector2Circular(3, 3), new Color(30, 15, 10, 0), (0.2f + Main.rand.NextFloat()) * NPC.scale);

                if (Main.rand.NextBool(4))
                {
                    Vector2 discOff = (Main.rand.NextVector2CircularEdge(150, 50) + Main.rand.NextVector2Circular(18, 18)).RotatedBy(discRot) * discScale;
                    Particle.NewParticle(Particle.ParticleType<PrettySparkle>(), discPos + discOff, discOff.RotatedBy(MathHelper.PiOver4) * 0.01f, new Color(30, 15, 10, 0), (0.1f + Main.rand.NextFloat(0.5f)) * NPC.scale);
                }
            }

            if (!Main.npc.Any(n => n.type == ModContent.NPCType<Goozma>() && n.active))
            {
                //NPC.active = false;
                NPC.localAI[0]++;
                Attack = (int)AttackList.CosmicStomp;
            }
            else
                NPC.ai[2] = Main.npc.First(n => n.type == ModContent.NPCType<Goozma>() && n.active).whoAmI;

            NPC.realLife = Host.whoAmI;

            if (!NPC.HasPlayerTarget)
                NPC.TargetClosestUpgraded();
            if (!NPC.HasPlayerTarget)
                NPC.active = false;

            NPC.damage = GetDamage(0);

            if (Time < 0)
            {
                NPC.velocity *= 0.9f;
                NPC.damage = 0;
                squishFactor = new Vector2(1f - (float)Math.Pow(Utils.GetLerpValue(-10, -45, Time, true), 2) * 0.5f, 1f + (float)Math.Pow(Utils.GetLerpValue(-10, -45, Time, true), 2) * 0.4f);
                if (Time == -2 && NPC.Distance(Target.Center) > 1000)
                {
                    RememberAttack = Attack;
                    Attack = (int)AttackList.TooFar;
                }
                NPC.frameCounter++;
            }

            else switch (Attack)
                {
                    case (int)AttackList.StarSigns:
                        StarSigns();
                        break;
                    case (int)AttackList.CosmicStomp:
                        CosmicStomp();
                        break;
                    case (int)AttackList.BlackHole:
                        BlackHole();
                        break;

                    case (int)AttackList.Interrupt:
                        NPC.noTileCollide = true;
                        NPC.damage = 0;
                        NPC.velocity *= 0.5f;

                        if (Time < 15)
                            Host.ai[0] = 0;

                        break;

                    case (int)AttackList.TooFar:

                        if (Time < 5)
                            saveTarget = Target.Center;

                        Vector2 midPoint = new Vector2((NPC.Center.X + saveTarget.X) / 2f, NPC.Center.Y);
                        Vector2 jumpTarget = Vector2.Lerp(Vector2.Lerp(NPC.Center, midPoint, Utils.GetLerpValue(0, 15, Time, true)), Vector2.Lerp(midPoint, saveTarget, Utils.GetLerpValue(5, 30, Time, true)), Utils.GetLerpValue(0, 30, Time, true));
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(jumpTarget).SafeNormalize(Vector2.Zero) * NPC.Distance(jumpTarget) * 0.5f * Utils.GetLerpValue(0, 35, Time, true), (float)Math.Pow(Utils.GetLerpValue(0, 40, Time, true), 2f));
                        Host.ai[0] = 0;

                        if (Time < 40)
                            squishFactor = Vector2.Lerp(Vector2.One, new Vector2(0.6f, 1.3f), Time / 40f);
                        else
                            squishFactor = Vector2.Lerp(new Vector2(1.4f, 0.5f), Vector2.One, Utils.GetLerpValue(40, 54, Time, true));

                        if (Time > 55)
                        {
                            NPC.velocity *= 0f;
                            Time = 0;
                            Attack = RememberAttack;
                        }
                        break;
                }

            NPC.localAI[0]++;
            Time++;
        }

        private void Reset()
        {
            Time = 0;
            Attack = (int)AttackList.Interrupt;
            squishFactor = Vector2.One;
            if (Main.netMode == NetmodeID.MultiplayerClient)
                NPC.netUpdate = true;

        }

        private Vector2 saveTarget;
        private float opacity;

        public void StarSigns()
        {
            NPC.damage = 0;
            NPC.dontTakeDamage = true;
            NPC.velocity *= 0.8f;

            if (Time > 35 && Time < 600)
            {
                foreach (Player player in Main.player.Where(n => n.active && !n.dead && n.Distance(saveTarget) > 1700))
                    player.velocity += player.DirectionTo(saveTarget) * Utils.GetLerpValue(1700, 1900, player.Distance(saveTarget));
            }

            if (Time == 35)
            {
                saveTarget = NPC.Center;

                int count = 17 + Main.rand.Next(5, 8);
                for (int i = 0; i < count; i++)
                {
                    Vector2 target = NPC.Center + new Vector2(Main.rand.Next(1600, 2000), 0).RotatedBy(MathHelper.TwoPi / count * i).RotatedByRandom(0.1f);
                    Projectile star = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center + Main.rand.NextVector2Circular(10, 10), HuntOfTheOldGodUtils.GetDesiredVelocityForDistance(NPC.Center, target, 0.95f, 40), ModContent.ProjectileType<ConstellationStar>(), GetDamage(1), 0);
                    star.direction = Main.rand.NextBool() ? -1 : 1;
                    star.localAI[0] = i / (float)count;
                    star.ai[0] = 0;
                    star.ai[1] = 0;
                }
            }
            if (Time == 53)
            {
                int count = 15 + Main.rand.Next(4, 8);
                for (int i = 0; i < count; i++)
                {
                    Vector2 target = NPC.Center + new Vector2(Main.rand.Next(1000, 1500), 0).RotatedBy(MathHelper.TwoPi / count * i).RotatedByRandom(0.2f);
                    Projectile star = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center + Main.rand.NextVector2Circular(10, 10), HuntOfTheOldGodUtils.GetDesiredVelocityForDistance(NPC.Center, target, 0.95f, 40), ModContent.ProjectileType<ConstellationStar>(), GetDamage(1), 0);
                    star.direction = Main.rand.NextBool() ? -1 : 1;
                    star.localAI[0] = i / (float)count;
                    star.ai[0] = -18;
                    star.ai[1] = 1;
                }
            }
            if (Time == 70)
            {
                NPC.scale = 0f;

                int count = 12 + Main.rand.Next(8, 12);
                for (int i = 0; i < count; i++)
                {
                    Vector2 target = NPC.Center + new Vector2(Main.rand.Next(50, 1000), 0).RotatedBy(MathHelper.TwoPi / count * i).RotatedByRandom(0.3f);
                    Projectile star = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center + Main.rand.NextVector2Circular(10, 10), HuntOfTheOldGodUtils.GetDesiredVelocityForDistance(NPC.Center, target, 0.95f, 40), ModContent.ProjectileType<ConstellationStar>(), GetDamage(1), 0);
                    star.direction = Main.rand.NextBool() ? -1 : 1;
                    star.localAI[0] = i / (float)count;
                    star.ai[0] = i * 2;
                    star.ai[1] = 2;
                }

                for (int i = 0; i < 8; i++)
                {
                    Vector2 target = NPC.Center + new Vector2(Main.rand.Next(10, 500), 0).RotatedBy(MathHelper.TwoPi / count * i).RotatedByRandom(0.3f);

                    Projectile star = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center + Main.rand.NextVector2Circular(10, 10), HuntOfTheOldGodUtils.GetDesiredVelocityForDistance(NPC.Center, target, 0.95f, 40), ModContent.ProjectileType<ConstellationStar>(), GetDamage(1), 0);
                    star.direction = Main.rand.NextBool() ? -1 : 1;
                    star.localAI[0] = i / (float)count;
                    star.ai[0] = -35;
                    star.ai[1] = 2;
                }
            }

            if (Time > 58 && Time < 70)
            {
                NPC.scale = MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(70, 62, Time, true));
                int debrisCount = Main.rand.Next(1, 2);
                if (Main.expertMode)
                    debrisCount++;
                for (int i = 0; i < debrisCount; i++)
                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center + Main.rand.NextVector2Circular(70, 60), Main.rand.NextVector2Circular(9, 9), ModContent.ProjectileType<StellarDebris>(), GetDamage(3), 0);

            }

            if (Time > 70)
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * NPC.Distance(Target.Center) * 0.05f, 0.1f) * Utils.GetLerpValue(500, 560, Time, true);

                if (Time < 521)
                {
                    if ((Time - 70) % 150 == 5)
                        SpawnConstellation(0, 7);
                    if ((Time - 70) % 150 == 25)
                        SpawnConstellation(1, 10);
                    if ((Time - 70) % 150 == 45)
                        SpawnConstellation(2, 8);
                }
            }

            if (Time > 600)
            {
                NPC.scale = MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(600, 680, Time, true));
                squishFactor = Vector2.Lerp(new Vector2(1f + (float)Math.Sin((Time - 600) * 0.1f) * 0.9f, 1f - (float)Math.Sin((Time - 600) * 0.1f) * 0.9f) * 3f, Vector2.One, NPC.scale);
            }
            if (Time > 680)
                Reset();
        }

        private void SpawnConstellation(int checkType, int lineCount)
        {
            switch (checkType)
            {
                case 0:

                    for (int i = 0; i < lineCount; i++)
                    {
                        Func<Projectile, bool> check1 = n =>
                        n.active && n.type == ModContent.ProjectileType<ConstellationStar>() &&
                        n.ai[1] == 0 &&
                        n.ai[2] == 0;                        
                        if (Main.projectile.Any(check1))
                        {
                            Projectile firstStar = Main.rand.Next(Main.projectile.Where(check1).ToArray());
                            firstStar.ai[2] = 1;

                            for (int j = 0; j < Main.rand.Next(1, 2); j++)
                            {
                                Func<Projectile, bool> check2 = n =>
                                n.active && n.type == ModContent.ProjectileType<ConstellationStar>() &&
                                n.Distance(firstStar.Center) > 250 &&
                                n.Distance(firstStar.Center) < 1200 &&
                                n.ai[1] < 2 &&
                                n.ai[2] == 0;

                                if (!Main.projectile.Any(check2))
                                    break;

                                Projectile secondStar = Main.rand.Next(Main.projectile.Where(check2).ToArray());
                                secondStar.ai[2] = 1;

                                Projectile line = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ConstellationLine>(), GetDamage(2), 0);
                                line.ai[1] = firstStar.whoAmI;
                                line.ai[2] = secondStar.whoAmI;
                            }
                        }
                    }
                    break;

                case 1:

                    for (int i = 0; i < lineCount; i++)
                    {
                        Func<Projectile, bool> check1 = n => n.active && n.type == ModContent.ProjectileType<ConstellationStar>() && 
                        n.ai[1] == 0 &&
                        n.ai[2] == 1;
                        if (Main.projectile.Any(check1))
                        {
                            Projectile firstStar = Main.rand.Next(Main.projectile.Where(check1).ToArray());
                            firstStar.ai[2] = 2;

                            for (int j = 0; j < Main.rand.Next(1, 3); j++)
                            {
                                Func<Projectile, bool> check2 = n =>
                                n.active && n.type == ModContent.ProjectileType<ConstellationStar>() &&
                                n.Distance(firstStar.Center) > 250 &&
                                n.Distance(firstStar.Center) < 1000 &&
                                n.ai[1] == 1 &&
                                n.ai[2] == 0;

                                if (!Main.projectile.Any(check2))
                                    break;

                                Projectile secondStar = Main.rand.Next(Main.projectile.Where(check2).ToArray());
                                secondStar.ai[2] = 1;

                                Projectile line = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ConstellationLine>(), GetDamage(2), 0);
                                line.ai[1] = firstStar.whoAmI;
                                line.ai[2] = secondStar.whoAmI;
                            }
                        }
                    }

                    break;

                case 2:

                    for (int i = 0; i < lineCount; i++)
                    {
                        Func<Projectile, bool> check1 = n => n.active && n.type == ModContent.ProjectileType<ConstellationStar>() && 
                        n.ai[1] == 1 &&
                        n.ai[2] == 1;
                        if (Main.projectile.Any(check1))
                        {
                            Projectile firstStar = Main.rand.Next(Main.projectile.Where(check1).ToArray());
                            firstStar.ai[2] = 2;

                            for (int j = 0; j < Main.rand.Next(1, 3); j++)
                            {
                                Func<Projectile, bool> check2 = n =>
                                n.active && n.type == ModContent.ProjectileType<ConstellationStar>() &&
                                n.Distance(firstStar.Center) > 250 &&
                                n.Distance(firstStar.Center) < 1000 &&
                                ((n.ai[1] == 1 && n.ai[2] == 0) ||
                                (n.ai[1] == 2 && n.ai[2] == 0));

                                if (!Main.projectile.Any(check2))
                                    break;

                                Projectile secondStar = Main.rand.Next(Main.projectile.Where(check2).ToArray());
                                secondStar.ai[2] = 1;

                                Projectile line = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ConstellationLine>(), GetDamage(2), 0);
                                line.ai[1] = firstStar.whoAmI;
                                line.ai[2] = secondStar.whoAmI;
                            }
                        }
                    }                    
                    
                    for (int i = 0; i < lineCount; i++)
                    {
                        Func<Projectile, bool> check1 = n => n.active && n.type == ModContent.ProjectileType<ConstellationStar>() && 
                        n.ai[1] == 2 &&
                        n.ai[2] == 0;
                        if (Main.projectile.Any(check1))
                        {
                            Projectile firstStar = Main.rand.Next(Main.projectile.Where(check1).ToArray());
                            firstStar.ai[2] = 2;

                            for (int j = 0; j < Main.rand.Next(1, 3); j++)
                            {
                                Func<Projectile, bool> check2 = n =>
                                n.active && n.type == ModContent.ProjectileType<ConstellationStar>() &&
                                n.Distance(firstStar.Center) > 250 &&
                                n.Distance(firstStar.Center) < 1200 &&
                                ((n.ai[1] == 1 && n.ai[2] == 1) ||
                                (n.ai[1] == 2 && n.ai[2] == 0));

                                if (!Main.projectile.Any(check2))
                                    break;

                                Projectile secondStar = Main.rand.Next(Main.projectile.Where(check2).ToArray());
                                secondStar.ai[2] = 1;

                                Projectile line = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ConstellationLine>(), GetDamage(2), 0);
                                line.ai[1] = firstStar.whoAmI;
                                line.ai[2] = secondStar.whoAmI;
                            }
                        }
                    }

                    break;

            }
        }

        public void CosmicStomp()
        {
            int waitTime = 100;
            if (Main.expertMode)
                waitTime = 80;
            if (Time < 40)
            {
                squishFactor = Vector2.SmoothStep(Vector2.One, new Vector2(1.5f, 0.6f), Time / 40f);
                if (Time > 38)
                    NPC.velocity = new Vector2(Math.Sign(NPC.Center.X - Target.Center.X) * 40f, -30f);
                else
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * (NPC.Distance(Target.Center) - 140) * 0.2f, 0.2f) * Utils.GetLerpValue(40, 20, Time, true);
            }
            else if (Time < 40 + waitTime)
            {
                if (Time < 40 + waitTime * 0.8f)
                    saveTarget = Target.Center + Target.Velocity * new Vector2(2f, 5f);

                squishFactor = new Vector2(0.8f, 1.2f);
                float newAngle = MathHelper.WrapAngle(NPC.AngleTo(saveTarget) + MathHelper.TwoPi * Utils.GetLerpValue(40, 40 + waitTime * 0.2f, Time, true) * NPC.direction + MathHelper.PiOver2);// Utils.AngleLerp(NPC.velocity.ToRotation(), NPC.AngleTo(saveTarget), Utils.GetLerpValue(waitTime * 0.4f, waitTime * 0.3f, Time, true));
                NPC.rotation = NPC.rotation.AngleLerp(newAngle, 0.2f);
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center - Vector2.UnitY * 360 + Target.Velocity * new Vector2(2f, 5f)).SafeNormalize(Vector2.Zero) * NPC.Distance(Target.Center - Vector2.UnitY * 360) * 0.15f, 0.1f);
                NPC.velocity = NPC.velocity.RotatedBy(NPC.AngleTo(Target.Center) * 0.001f) * Utils.GetLerpValue(40 + waitTime, 40 + waitTime * 0.7f, Time, true);

                NPC.localAI[0] += Utils.GetLerpValue(40, 40 + waitTime, Time, true) * 2f;
            }
            else if (Time < 40 + waitTime + 12)
            {
                squishFactor = new Vector2(1.3f - NPC.velocity.Length() * 0.01f, 0.7f + NPC.velocity.Length() * 0.01f);
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(NPC.FindSmashSpot(saveTarget)).SafeNormalize(Vector2.Zero) * NPC.Distance(NPC.FindSmashSpot(saveTarget)) * 0.5f, 0.4f);
                NPC.scale = Utils.GetLerpValue(40 + waitTime + 10, 40 + waitTime + 5, Time, true);
                if (Time > 40 + waitTime + 2)
                {
                    NPC.localAI[0] = 0;
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2CircularEdge(3, 3), 8f, 10, 12));

                    int count = 2;
                    if (Main.expertMode)
                        count = 4;
                    for (int i = 0; i < count; i++)
                    {
                        Vector2 starPosition = new Vector2(Main.rand.Next(-50, 50), Main.rand.Next(-10, 10));
                        Vector2 starVelocity = new Vector2(starPosition.X * 0.2f, -Main.rand.Next(15, 25)) + NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * Main.rand.Next(5);
                        Projectile starbit = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center + starPosition, starVelocity, ModContent.ProjectileType<StellarGelatine>(), GetDamage(4), 0f);
                        starbit.ai[0] = (int)(Time * 0.2f) - (40 + waitTime + 12) + Utils.GetLerpValue(0, count, i, true) * 40 - 20;
                    }
                }
            }
            else if (Time < 40 + waitTime + 100)
            {
                NPC.damage = 0;
                NPC.dontTakeDamage = true;
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * (NPC.Distance(Target.Center) - 200) * 0.3f, 0.5f);
            }
            else
            {
                NPC.damage = 0;

                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * (NPC.Distance(Target.Center) - 300) * 0.07f, 0.02f) * Utils.GetLerpValue(40 + waitTime + 130, 40 + waitTime + 150, Time, true);
                NPC.rotation = 0;
                squishFactor = Vector2.Lerp(squishFactor, Vector2.One, Utils.GetLerpValue(40 + waitTime + 130, 40 + waitTime + 150, Time, true) * 0.2f);
                NPC.scale = MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(40 + waitTime + 180, 40 + waitTime + 260, Time, true));

                NPC.velocity += NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * NPC.Distance(Target.Center) * 0.001f * Utils.GetLerpValue(40 + waitTime + 150, 40 + waitTime + 260, Time, true) * Utils.GetLerpValue(40 + waitTime + 130, 40 + waitTime + 150, Time, true); ;
            }

            if (Time > 40 && Time < 40 + waitTime + 10)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 rocketPosition = NPC.Center + (NPC.rotation + MathHelper.PiOver2).ToRotationVector2() * 30 + Main.rand.NextVector2Circular(70, 20).RotatedBy(NPC.rotation);
                    Vector2 rocketVelocity = (NPC.rotation + MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(1.5) * Main.rand.Next(25, 35);
                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), rocketPosition, rocketVelocity, ModContent.ProjectileType<InterstellarFlame>(), GetDamage(4), 0f);
                }
            }

            if (Time > 40 + waitTime + 280)
                Reset();
        }

        public void BlackHole()
        {
            float holeSize = (float)Math.Sqrt(Utils.GetLerpValue(5, 550, Time, true) * Utils.GetLerpValue(600, 550, Time, true)) * 300;
            if (Time < 60)
                NPC.scale = (float)Math.Pow(Utils.GetLerpValue(90, 0, Time, true), 2f);

            if (Time == 2)
            {
                Projectile hole = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BlackHoleBlender>(), 9999, 0);
                hole.ai[1] = 595;
            }

            if (Time > 50 && Time < 600)
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * NPC.Distance(Target.Center) * 0.03f, 0.2f);
                NPC.velocity *= 0.75f;

                foreach (Player player in Main.player.Where(n => n.active && !n.dead))
                {
                    if (player.Distance(NPC.Center) < holeSize)
                        player.Hurt(PlayerDeathReason.ByCustomReason($"{player.name} was shredded by gravity."), 9999, -1, false, true, -1, false, 0, 0, 0);

                    if (player.Distance(NPC.Center) > holeSize + 200)
                        player.velocity += player.DirectionTo(NPC.Center) * Utils.GetLerpValue(holeSize + 200, holeSize + 500, player.Distance(NPC.Center));
                }

                if (Time % 14 == 5 && Time < 410)
                {
                    WeightedRandom<float> size = new WeightedRandom<float>();
                    size.Add(1f, 0.8f);
                    size.Add(2f, 0.3f);
                    size.Add(3f, 0.1f);
                    float chosenSize = size.Get();
                    Projectile rock = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), Target.Center + NPC.DirectionTo(Target.Center + Target.Velocity * 30).SafeNormalize(Vector2.Zero).RotatedByRandom(1.6f) * Main.rand.Next(1000, 1500), Vector2.Zero, ModContent.ProjectileType<SpaceRock>(), GetDamage(6 + (int)chosenSize), 0);
                    rock.ai[1] = chosenSize + 1 + Main.rand.NextFloat(-0.1f, 0.1f);
                }

                if (Time % 2 == 0)
                {
                    float strength = Utils.GetLerpValue(50, 500, Time, true) * 10f;
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2CircularEdge(1, 1), strength, 4, 30, 20000));
                }
            }
            if (Time > 600)
            {
                NPC.velocity *= 0.92f;
                NPC.scale = 1f + (float)Math.Sqrt(Utils.GetLerpValue(630, 600, Time, true));
            }

            foreach (Projectile projectile in Main.projectile.Where(n => n.active && n.type == ModContent.ProjectileType<SpaceRock>()))
            {
                projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.DirectionTo(NPC.Center).SafeNormalize(Vector2.Zero) * (projectile.Distance(NPC.Center) + 500) * 0.0013f * (4f - projectile.ai[1] * 0.66f) * 4.5f, 0.3f);
                projectile.velocity += NPC.velocity * 0.1f;
                if (projectile.Distance(NPC.Center) < holeSize - 40)
                    projectile.Kill();
            }

            if (Time > 670)
                Reset();
        }

        //public void StarTrail()
        //{
        //    int dashCount = 6;
        //    int dashTime = 70;
        //    if (Main.expertMode)
        //    {
        //        dashCount = 10;
        //        dashTime = 58;
        //    }
        //    if (Time < 2)
        //        saveTarget = Target.Center;

        //    if (Time < dashCount * dashTime)
        //    {
        //        NPC.rotation = NPC.rotation.AngleLerp(NPC.AngleTo(Target.Center) - MathHelper.PiOver2, 0.08f);
        //        squishFactor = new Vector2(1f - NPC.velocity.Length() * 0.005f, 1f + NPC.velocity.Length() * 0.006f);
        //        if (Time % dashTime < (int)(dashTime * 0.6f))
        //        {
        //            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * (NPC.Distance(Target.Center) - 300) * 0.16f, 0.07f);
        //            NPC.velocity *= 0.94f;

        //            saveTarget = NPC.Center + (NPC.DirectionTo(Target.Center + Target.Velocity * 50).SafeNormalize(Vector2.Zero) * NPC.Distance(Target.Center + Target.Velocity * 40));//Vector2.Lerp(saveTarget, NPC.Center + (NPC.DirectionTo(Target.Center + Target.Velocity * 22).SafeNormalize(Vector2.Zero) * NPC.Distance(Target.Center + Target.Velocity * 22)).RotatedByRandom(0.1f) * 1.7f, 0.5f);
        //        }
        //        else
        //        {
        //            NPC.rotation = NPC.rotation.AngleLerp(NPC.AngleTo(saveTarget) - MathHelper.PiOver2, 0.4f);
        //            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(saveTarget).SafeNormalize(Vector2.Zero) * NPC.Distance(saveTarget) * 0.2f, 0.33f);
        //            if (Time % dashTime > (int)(dashTime * 0.95f))
        //                NPC.velocity = Main.rand.NextVector2CircularEdge(30, 30) + NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * 30f;

        //            //if (Time % dashTime == (int)(dashTime * 0.62f))
        //            //{
        //            //    int distanceCount = (int)(NPC.Distance(saveTarget) / 90f);
        //            //    for (int i = 0; i < distanceCount; i++)
        //            //    {
        //            //        Projectile line = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), Vector2.Lerp(NPC.Center, saveTarget, i / (float)distanceCount), Vector2.Zero, ModContent.ProjectileType<CosmicDust>(), GetDamage(4), 0);
        //            //        line.ai[0] = -50 - i * 8;
        //            //        line.ai[1] = (0.5f + (i / (float)distanceCount) * 0.5f) + Utils.GetLerpValue(dashTime * 0.6f, dashTime, Time % dashTime, true) * 0.5f;
        //            //        line.ai[2] = NPC.rotation + MathHelper.PiOver2;
        //            //    }
        //            //}

        //            Particle.NewParticle(Particle.ParticleType<PrettySparkle>(), NPC.Center + Main.rand.NextVector2Circular(100, 100), Main.rand.NextVector2Circular(2, 2) + NPC.velocity * 0.3f, new Color(30, 15, 10, 0), (0.5f + Main.rand.NextFloat()));
        //        }
        //    }
        //    else
        //    {
        //        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * (NPC.Distance(Target.Center) - 400) * 0.16f, 0.07f);
        //        NPC.velocity *= 0.8f;
        //        NPC.rotation *= 0.9f;
        //    }

        //    if (Time > dashCount * dashTime + 30)
        //        Reset();
        //}

        //public void Starfall()
        //{
        //    NPC.damage = 0;
        //    NPC.dontTakeDamage = true;

        //    int gelTime = 30;
        //    if (Main.expertMode)
        //        gelTime = 20;

        //    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * (NPC.Distance(Target.Center) - 300) * 0.2f, 0.06f) - Vector2.UnitY * 0.6f;

        //    if (Time < 40)
        //    {
        //        NPC.velocity *= 0.8f;
        //        squishFactor = Vector2.Lerp(Vector2.One, new Vector2(1.5f, 0.4f), (float)Math.Sqrt(Time / 40f));
        //    }

        //    else if (Time < 100)
        //    {
        //        NPC.velocity *= 0.9f;

        //        squishFactor = Vector2.SmoothStep(new Vector2(1.5f, 0.4f), new Vector2(0.7f, 1.5f), (float)Math.Pow(Utils.GetLerpValue(40, 80, Time, true), 2));

        //        if (Time > 80)
        //        {
        //            int count = 3;
        //            if (Main.expertMode)
        //                count = 5;

        //            for (int i = 0; i < count; i++)
        //            {
        //                Projectile gelatin = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center + Main.rand.NextVector2Circular(30, 40), Main.rand.NextVector2Circular(10, 10), ModContent.ProjectileType<StellarGelatine>(), GetDamage(1), 0);
        //                gelatin.ai[0] = -50 - i * gelTime + (100 - Time);
        //            }
        //        }
        //        NPC.scale = Utils.GetLerpValue(99, 80, Time, true);
        //    }
        //}

        private int GetDamage(int attack, float modifier = 1f)
        {
            int damage = attack switch
            {
                0 => 70,//contact
                1 => 40,//constellation star
                2 => 50,//constellation line
                3 => 30,//constellation debris
                4 => 30,//rocket flame
                5 => 30,//star bit
                6 => 60,//black hole large
                7 => 50,//black hole medium
                8 => 30,//black hole small
                _ => 0
            };

            return (int)(damage * modifier);
        }

        public int npcFrame;

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 7)
            {
                NPC.frameCounter = 0;
                npcFrame = (npcFrame + 1) % Main.npcFrameCount[Type];
            }
        }

        public Vector2 discPos;
        public float discRot;
        public Vector2 discScale;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> ring = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/ConstellationArea");
            Asset<Texture2D> flash = TextureAssets.Extra[98];
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoftBig");

            Rectangle frame = texture.Frame(1, 4, 0, npcFrame);
            
            Color color = Color.White;

            Vector2 oldDiscScale = new Vector2(1f + (float)Math.Sin(NPC.localAI[0] * 0.05f % MathHelper.TwoPi) * 0.05f, 1.5f + (float)Math.Sin(NPC.localAI[0] * 0.05f % MathHelper.TwoPi) * 0.3f);
            discScale = oldDiscScale;

            int cosStompWaitTime = 100;
            if (Main.expertMode)
                cosStompWaitTime = 80;
            switch (Attack)
            {
                case (int)AttackList.TooFar:
                    color = Color.Lerp(new Color(100, 100, 100, 0), Color.White, Math.Clamp(NPC.Distance(Target.Center), 100, 300) / 200f);
                    break;

                case (int)AttackList.StarSigns:

                    float scaleIn = MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(37, 200, Time, true));
                    float scaleOut = (float)Math.Sqrt(Utils.GetLerpValue(600, 400, Time, true));
                    float ringScale = (2.8f + (float)Math.Sin(Time * 0.04f) * 0.1f) * scaleIn;

                    //spriteBatch.End();
                    //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.Transform);

                    spriteBatch.Draw(ring.Value, saveTarget - screenPos, ring.Frame(), new Color(15, 5, 35, 0) * 0.6f * scaleIn * scaleOut, Time * 0.002f, ring.Size() * 0.5f, ringScale, 0, 0);
                    spriteBatch.Draw(ring.Value, saveTarget - screenPos, ring.Frame(), new Color(15, 5, 35, 0) * 0.6f * scaleIn * scaleOut, Time * 0.004f, ring.Size() * 0.5f, ringScale, 0, 0);

                    //spriteBatch.End();
                    //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

                    break;

                case (int)AttackList.CosmicStomp:

                    if (Time < 40 + cosStompWaitTime)
                    {
                        Asset<Texture2D> ray = TextureAssets.Extra[27];
                        float rayPower = Utils.GetLerpValue(50, 40 + cosStompWaitTime, Time, true);
                        Color rayColor = Color.Lerp(new Color(80, 20, 230), new Color(200, 80, 30), rayPower);
                        rayColor.A /= 3;
                        spriteBatch.Draw(ray.Value, NPC.Center - screenPos, ray.Frame(), rayColor * Utils.GetLerpValue(40, 80, Time, true) * (1f - rayPower), NPC.rotation - MathHelper.PiOver2, ray.Size() * new Vector2(0.2f, 0.5f), 9f * (float)Math.Sqrt(rayPower), 0, 0);
                    }
                    else if (Time < 40 + cosStompWaitTime + 40)
                        discScale = Vector2.Lerp(oldDiscScale, new Vector2(5f), Utils.GetLerpValue(40 + cosStompWaitTime, 40 + cosStompWaitTime + 40, Time, true));

                    break;

                case (int)AttackList.BlackHole:
                    
                    color = Color.White * (Utils.GetLerpValue(50, 20, Time, true) + Utils.GetLerpValue(600, 630, Time, true));

                    Asset<Texture2D> hollowRing = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowRing");
                    Asset<Texture2D> shockRing = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/ShockRing");

                    for (int i = 0; i < 7; i++)
                    {
                        float exist = Utils.GetLerpValue(20, 120, Time + i * 3f, true) * Utils.GetLerpValue(600, 500, Time - i * 3f, true);
                        float comeIn = (float)Math.Pow(1f - ((Time + i / 7f * 70) % 70) / 70f, 2f);
                        spriteBatch.Draw(hollowRing.Value, NPC.Center - screenPos, hollowRing.Frame(), Color.Black * Utils.PingPongFrom01To010(comeIn) * exist, (Time + i * 5f) * 0.04f, hollowRing.Size() * 0.5f, 70f * comeIn, 0, 0);
                        spriteBatch.Draw(shockRing.Value, NPC.Center - screenPos, shockRing.Frame(), Color.Black * Utils.PingPongFrom01To010(comeIn) * exist, (Time + i * 5f) * 0.04f, shockRing.Size() * 0.5f, 35f * comeIn, 0, 0);
                    }

                    break;
            }

            if (NPC.IsABestiaryIconDummy)
            {
                //RasterizerState priorRrasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
                //Rectangle priorScissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
                //spriteBatch.End();
                //spriteBatch.GraphicsDevice.RasterizerState = priorRrasterizerState;
                //spriteBatch.GraphicsDevice.ScissorRectangle = priorScissorRectangle;
                //effect.Parameters["uPosition"].SetValue(Vector2.Zero);

                //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.UIScaleMatrix);

                spriteBatch.Draw(texture.Value, NPC.Center - screenPos, frame, color, NPC.rotation, frame.Size() * 0.5f, NPC.scale * squishFactor, 0, 0);

                //spriteBatch.End();
                //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
            }
            else
            {
                discPos = NPC.Center - NPC.velocity * 1.5f + new Vector2((float)Math.Sin(NPC.localAI[0] * 0.03f % MathHelper.TwoPi) * 12, 22 + (float)Math.Cos(NPC.localAI[0] * 0.02f % MathHelper.TwoPi) * 16f) * squishFactor;
                discRot = (float)Math.Sin(NPC.localAI[0] * 0.013f % MathHelper.TwoPi) * 0.15f;

                Effect lightningEffect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/StellarRing", AssetRequestMode.ImmediateLoad).Value;
                lightningEffect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
                lightningEffect.Parameters["uTexture0"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/SpaceTrail").Value);
                lightningEffect.Parameters["uTexture1"].SetValue(ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/Stars").Value);
                lightningEffect.Parameters["uColorLight"].SetValue(new Color(255, 225, 170).ToVector3());
                lightningEffect.Parameters["uColorAura"].SetValue(new Color(120, 50, 5).ToVector3());
                lightningEffect.Parameters["uColorDark"].SetValue(new Color(80, 20, 200).ToVector3());//new Color(80, 50, 35)
                lightningEffect.Parameters["uDarkSpecial"].SetValue(new Vector2(0.1f, 0.8f));
                lightningEffect.Parameters["uTime"].SetValue(-NPC.localAI[0] * 0.01f % 1f);

                VertexStrip bottomStrip = new VertexStrip();
                VertexStrip topStrip = new VertexStrip();

                Color ColorFunction(float progress) => Color.White * NPC.scale;
                float WidthFunction(float progress) => (35f + (float)Math.Sin(progress * MathHelper.TwoPi - NPC.localAI[0] * 0.5f) * 7f) * NPC.scale;

                List<Vector2> bottomDiscPos = new List<Vector2>();
                List<float> bottomDiscRot = new List<float>();
                for (int i = 100; i <= 200; i++)
                {
                    Vector2 rotatedRing = new Vector2(170 * discScale.X, 0).RotatedBy(MathHelper.TwoPi / 200f * i) * (0.7f + NPC.scale * 0.3f);
                    rotatedRing.Y *= 0.18f * discScale.Y;
                    bottomDiscPos.Add(rotatedRing.RotatedBy(discRot));
                    bottomDiscRot.Add(MathHelper.TwoPi / 200f * i + MathHelper.PiOver2 + discRot);
                }
                bottomStrip.PrepareStrip(bottomDiscPos.ToArray(), bottomDiscRot.ToArray(), ColorFunction, WidthFunction, discPos - screenPos, bottomDiscRot.Count, true);
                                
                List<Vector2> topDiscPos = new List<Vector2>();
                List<float> topDiscRot = new List<float>();
                for (int i = 0; i <= 100; i++)
                {
                    Vector2 rotatedRing = new Vector2(170 * discScale.X, 0).RotatedBy(MathHelper.TwoPi / 200f * i) * (0.7f + NPC.scale * 0.3f);
                    rotatedRing.Y *= 0.2f * discScale.Y;
                    topDiscPos.Add(rotatedRing.RotatedBy(discRot));
                    topDiscRot.Add(MathHelper.TwoPi / 200f * i + MathHelper.PiOver2 + discRot);
                }
                topStrip.PrepareStrip(topDiscPos.ToArray(), topDiscRot.ToArray(), ColorFunction, WidthFunction, discPos - screenPos, topDiscPos.Count, true);

                lightningEffect.CurrentTechnique.Passes[0].Apply();
                bottomStrip.DrawTrail();
                Main.pixelShader.CurrentTechnique.Passes[0].Apply();

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

                spriteBatch.Draw(bloom.Value, NPC.Center - screenPos, bloom.Frame(), new Color(20, 0, 50, 0) * NPC.scale, NPC.rotation, bloom.Size() * 0.5f, NPC.scale * squishFactor * new Vector2(1.1f, 0.8f), 0, 0);
                spriteBatch.Draw(bloom.Value, NPC.Center - screenPos, bloom.Frame(), new Color(30, 0, 70, 0) * NPC.scale, NPC.rotation, bloom.Size() * 0.5f, NPC.scale * squishFactor * new Vector2(1.1f, 0.8f), 0, 0);
                spriteBatch.Draw(texture.Value, NPC.Center - screenPos, frame, color, NPC.rotation, frame.Size() * 0.5f, NPC.scale * squishFactor, 0, 0);

                for (int i = 0; i < 5; i++)
                {
                    Vector2 off = new Vector2(20 + (float)Math.Sin(NPC.localAI[0] * 0.1f) * 3).RotatedBy(MathHelper.TwoPi / 5f * i + NPC.rotation * 0.5f);
                    spriteBatch.Draw(texture.Value, NPC.Center + off - screenPos, frame, new Color(80, 40, 35, 0).MultiplyRGBA(color) * 0.3f, NPC.rotation, frame.Size() * 0.5f, NPC.scale * squishFactor, 0, 0);
                }

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

                lightningEffect.CurrentTechnique.Passes[0].Apply();
                topStrip.DrawTrail();
                Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            }

            switch (Attack)
            {
                case (int)AttackList.CosmicStomp:

                    if (Time > 40 + cosStompWaitTime - 10 && Time < 40 + cosStompWaitTime + 30)
                    {
                        Vector2 off = new Vector2(0, 800 - 800 * Utils.GetLerpValue(40 + cosStompWaitTime, 40 + cosStompWaitTime + 12, Time, true)).RotatedBy(NPC.rotation);
                        float stompFlashScale = Utils.GetLerpValue(40 + cosStompWaitTime - 5, 40 + cosStompWaitTime + 3, Time, true) * Utils.GetLerpValue(40 + cosStompWaitTime + 15, 40 + cosStompWaitTime + 10, Time, true);
                        Main.EntitySpriteDraw(flash.Value, NPC.Center + off - screenPos, flash.Frame(), new Color(80, 50, 35, 0) * stompFlashScale, NPC.rotation, flash.Size() * 0.5f, new Vector2(3f, 22f) * (0.5f + stompFlashScale * 0.5f), 0, 0);
                        Main.EntitySpriteDraw(flash.Value, NPC.Center + off - screenPos, flash.Frame(), new Color(255, 225, 170, 0) * stompFlashScale, NPC.rotation, flash.Size() * 0.5f, new Vector2(2f, 15f) * stompFlashScale, 0, 0);
                        Main.EntitySpriteDraw(flash.Value, NPC.Center + new Vector2(30, 0).RotatedBy(NPC.rotation) + off * 1.2f - screenPos, flash.Frame(), new Color(80, 50, 35, 0) * 0.5f * stompFlashScale, NPC.rotation, flash.Size() * 0.5f, new Vector2(3f, 22f) * (0.5f + stompFlashScale * 0.5f) * 0.7f, 0, 0);
                        Main.EntitySpriteDraw(flash.Value, NPC.Center - new Vector2(30, 0).RotatedBy(NPC.rotation) + off * 1.2f - screenPos, flash.Frame(), new Color(80, 50, 35, 0) * 0.5f * stompFlashScale, NPC.rotation, flash.Size() * 0.5f, new Vector2(3f, 22f) * (0.5f + stompFlashScale * 0.5f) * 0.7f, 0, 0);
                    }
                    break;

                case (int)AttackList.BlackHole:

                    float flashScale = Utils.GetLerpValue(3, 18, Time, true) * Utils.GetLerpValue(65, 40, Time, true) * 0.5f;
                    float flashRotation = (float)Math.Sqrt(Time * 0.17f) * 5f;
                    Main.EntitySpriteDraw(flash.Value, NPC.Center - screenPos, flash.Frame(), new Color(80, 50, 35, 0), flashRotation, flash.Size() * 0.5f, new Vector2(3f, 15f) * flashScale, 0, 0);
                    Main.EntitySpriteDraw(flash.Value, NPC.Center - screenPos, flash.Frame(), new Color(80, 50, 35, 0), flashRotation + MathHelper.PiOver2, flash.Size() * 0.5f, new Vector2(3f, 15f) * flashScale, 0, 0);
                    Main.EntitySpriteDraw(flash.Value, NPC.Center - screenPos, flash.Frame(), new Color(255, 225, 170, 0), flashRotation, flash.Size() * 0.5f, new Vector2(1f, 5f) * flashScale, 0, 0);
                    Main.EntitySpriteDraw(flash.Value, NPC.Center - screenPos, flash.Frame(), new Color(255, 225, 170, 0), flashRotation + MathHelper.PiOver2, flash.Size() * 0.5f, new Vector2(1f, 5f) * flashScale, 0, 0);
                    Main.EntitySpriteDraw(bloom.Value, NPC.Center - screenPos, bloom.Frame(), new Color(30, 15, 10, 0), flashRotation + MathHelper.PiOver2, bloom.Size() * 0.5f, 5f * flashScale, 0, 0);

                    break;
            }

            return false;
        }
    }
}