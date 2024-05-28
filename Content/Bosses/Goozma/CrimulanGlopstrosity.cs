using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma.Projectiles;
using CalamityHunt.Content.Particles;
using static CalamityHunt.Common.Systems.DifficultySystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma
{
    [AutoloadBossHead]
    public class CrimulanGlopstrosity : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.TrailCacheLength[Type] = 10;
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.ShouldBeCountedAsBoss[Type] = false;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Ichor] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            if (Common.ModCompatibility.Calamity.IsLoaded)
            {
                NPCID.Sets.SpecificDebuffImmunity[Type][Common.ModCompatibility.Calamity.Mod.Find<ModBuff>("MiracleBlight").Type] = true;
            }

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                PortraitScale = 0.75f,
                PortraitPositionYOverride = -10,
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            database.FindEntryByNPCID(Type).UIInfoProvider = new HighestOfMultipleUICollectionInfoProvider(new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<Goozma>()], true));
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement($"Mods.{nameof(CalamityHunt)}.Bestiary.CrimulanGlopstrosity"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.SlimeRain,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 210;
            NPC.height = 150;
            NPC.damage = 12;
            NPC.defense = 100;
            NPC.lifeMax = 3500000;
            NPC.takenDamageMultiplier = 0.75f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 5);
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            NPC.aiStyle = -1;
            NPC.chaseable = false;
            if (ModLoader.HasMod("CalamityMod"))
            {
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.Call("SetDebuffVulnerabilities", "poison", false);
                calamity.Call("SetDebuffVulnerabilities", "heat", true);
                calamity.Call("SetDefenseDamageNPC", NPC, true);
            }
        }

        public enum AttackList
        {
            SlamRain,
            CollidingCrush,
            EndlessChase,
            TooFar,
            Interrupt,
            SlamDown = -1
        }

        public ref float Time => ref NPC.ai[0];
        public ref float Attack => ref NPC.ai[1];
        public ref NPC Host => ref Main.npc[(int)NPC.ai[2]];
        public ref float RememberAttack => ref NPC.ai[3];

        public NPCAimedTarget Target => NPC.GetTargetData();
        public Vector2 squishFactor = Vector2.One;

        public override void AI()
        {
            useNinjaSlamFrame = false;

            if (!Main.npc.Any(n => n.type == ModContent.NPCType<Goozma>() && n.active))
                NPC.active = false;
            else
                NPC.ai[2] = Main.npc.First(n => n.type == ModContent.NPCType<Goozma>() && n.active).whoAmI;

            NPC.realLife = Host.whoAmI;

            if (!NPC.HasPlayerTarget)
                NPC.TargetClosestUpgraded();
            if (!NPC.HasPlayerTarget)
                NPC.active = false;

            //if (Attack != (int)AttackList.TooFar && Attack != (int)AttackList.SlamDown)
            //{
            //    foreach (Player player in Main.player.Where(n => n.active && !n.dead))
            //    {
            //        float distance = 0;
            //        for (int i = 0; i < Main.tile.Height; i++)
            //        {
            //            Point playerTile = player.MountedCenter.ToTileCoordinates();
            //            if (WorldGen.InWorld(playerTile.X, playerTile.Y + i))
            //            {
            //                if (WorldGen.SolidTileAllowTopSlope(playerTile.X, playerTile.Y + i))
            //                {
            //                    distance = i;
            //                    break;
            //                }
            //            }
            //            else
            //            {
            //                distance = i;
            //                break;
            //            }
            //        }
            //        if (distance > 36)
            //        {
            //            player.velocity.Y += distance * 0.3f;
            //            player.maxFallSpeed += distance;
            //            player.gravity *= 10;
            //        }
            //        //inflict debuff
            //    }
            //}

            NPC.damage = GetDamage(0);

            if (Time < 0)
            {
                NPC.velocity *= 0.9f;
                NPC.damage = 0;
                squishFactor = new Vector2(1f - (float)Math.Pow(Utils.GetLerpValue(-10, -45, Time, true), 2) * 0.5f, 1f + (float)Math.Pow(Utils.GetLerpValue(-10, -45, Time, true), 2) * 0.4f);
                if (Time == -2)
                {
                    StoreAttack();

                    if (NPC.Distance(Target.Center) > 1000)
                        SetAttack(AttackList.TooFar);

                    float distance = 0;
                    for (int i = 0; i < Main.tile.Height; i++)
                    {
                        Point playerTile = Target.Center.ToTileCoordinates();
                        if (WorldGen.InWorld(playerTile.X, playerTile.Y + i))
                        {
                            if (WorldGen.SolidTile3(playerTile.X, playerTile.Y + i))
                            {
                                distance = i;
                                break;
                            }
                        }
                        else
                        {
                            distance = i;
                            break;
                        }
                    }
                    if (distance > 36)
                        SetAttack(AttackList.SlamDown);
                }

                NPC.frameCounter++;
            }
            else switch (Attack)
                {
                    case (int)AttackList.SlamRain:
                        SlamRain();
                        break;

                    case (int)AttackList.CollidingCrush:
                        CollidingCrush();
                        break;

                    case (int)AttackList.EndlessChase:
                        EndlessChase();
                        break;                    
                    
                    case (int)AttackList.SlamDown:
                        SlamDown();
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
                            SetAttack((int)RememberAttack, true);
                        }
                        break;

                }

            Time++;
        }

        private void Reset()
        {
            SetAttack(AttackList.Interrupt, true);
            squishFactor = Vector2.One;
            if (Main.netMode == NetmodeID.MultiplayerClient)
                NPC.netUpdate = true;
        }

        private Vector2 saveTarget;

        private void SlamRain()
        {
            int ceilingCount = (int)DifficultyBasedValue(2, 2, 3, 4);

            if (Time <= ceilingCount * 170)
            {
                float localTime = Time % 170;

                if (localTime == 35)
                {
                    SoundStyle createSound = AssetDirectory.Sounds.Slime.CrimslimeTelegraph;
                    SoundEngine.PlaySound(createSound.WithVolumeScale(1.5f), NPC.Center);
                }

                if (localTime < 32)
                {
                    NPC.velocity *= 0.1f;
                    squishFactor = new Vector2(1f + (float)Math.Cbrt(Utils.GetLerpValue(10, 26, localTime, true)) * 0.4f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(10, 26, localTime, true)) * 0.5f);
                    if (localTime == 25)
                    {
                        SoundStyle hop = AssetDirectory.Sounds.Goozma.SlimeJump;
                        SoundEngine.PlaySound(hop, NPC.Center);
                        SoundEngine.PlaySound(SoundID.QueenSlime, NPC.Center);
                    }

                }

                else if (localTime < 35)
                {
                    NPC.velocity.Y = -14;
                    squishFactor = new Vector2(0.5f, 1.4f);
                }

                else if (localTime < 140)
                {
                    Vector2 airTarget = Target.Center - Vector2.UnitY * 450 + Target.Velocity * 4;

                    if (localTime < 70)
                    {
                        squishFactor = Vector2.Lerp(Vector2.One, new Vector2(0.5f, 1.4f), (float)Math.Cbrt(Utils.GetLerpValue(58, 40, localTime, true)));
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(airTarget).SafeNormalize(Vector2.Zero) * Math.Max(2, NPC.Distance(airTarget)) * 0.2f, 0.08f) * Utils.GetLerpValue(90, 75, localTime, true);
                        saveTarget = Target.Center + Target.Velocity * 5;
                        NPC.rotation = NPC.rotation.AngleLerp(NPC.Top.AngleTo(NPC.FindSmashSpot(saveTarget)) - MathHelper.PiOver2, 0.5f) * 0.4f;
                    }
                    else
                    {
                        NPC.rotation = NPC.rotation.AngleLerp(NPC.Top.AngleTo(NPC.FindSmashSpot(saveTarget)) - MathHelper.PiOver2, 0.2f);
                        NPC.Center = Vector2.Lerp(NPC.Center, NPC.FindSmashSpot(saveTarget), Utils.GetLerpValue(75, 100, localTime, true));
                        NPC.velocity *= 0.5f;
                        useNinjaSlamFrame = true;

                        if (localTime < 100)
                        {
                            squishFactor = Vector2.Lerp(Vector2.One, new Vector2(0.5f, 1.5f), (float)Math.Pow(Utils.GetLerpValue(75, 85, localTime, true), 2));
                            NPC.frameCounter++;
                        }

                        if (localTime > 100)
                        {
                            NPC.rotation = 0;
                            squishFactor = Vector2.Lerp(Vector2.One, new Vector2(0.5f, 1.5f), (float)Math.Pow(Utils.GetLerpValue(90, 93, localTime, true), 2));
                            squishFactor = new Vector2(1f + (float)Math.Pow(Utils.GetLerpValue(118, 103, localTime, true), 2) * 0.6f, 1f - (float)Math.Pow(Utils.GetLerpValue(118, 103, localTime, true), 4) * 0.8f);
                        }

                        if (localTime == 100)
                        {
                            int extension = (int)DifficultyBasedValue(10, 12, 16, 18, 20);

                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom, Vector2.Zero, ModContent.ProjectileType<CrimulanShockwave>(), 0, 0, ai1: 2500);

                            for (int i = 0; i < extension; i++)
                            {
                                Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2((Target.Center.X - NPC.Center.X) * 0.02f + i * Main.rand.NextFloat(-10f, 10f), 0), ModContent.ProjectileType<CrimulanSmasher>(), GetDamage(1), 0);
                                proj.ai[0] = -40 - i;
                                proj.ai[1] = -1;
                                proj.ai[2] = 1;
                                proj.localAI[0] = 1;
                            }

                            foreach (Player player in Main.player.Where(n => n.active && !n.dead && n.Distance(NPC.Center) < 600))
                                player.velocity += player.DirectionFrom(NPC.Bottom + Vector2.UnitY * 10) * 5;

                            SoundStyle slam = AssetDirectory.Sounds.Slime.SlimeSlam;
                            SoundEngine.PlaySound(slam, NPC.Center);

                            for (int i = 0; i < Main.rand.Next(30, 40); i++)
                            {
                                Vector2 velocity = Main.rand.NextVector2Circular(8, 1) - Vector2.UnitY * Main.rand.NextFloat(7f, 15f);
                                Vector2 position = NPC.Center + Main.rand.NextVector2Circular(1, 50) + new Vector2(velocity.X * 15f, 32f);
                                ParticleBehavior.NewParticle(ModContent.GetInstance<CrimBombChunkParticleBehavior>(), position, velocity, Color.White, 0.1f + Main.rand.NextFloat(2f));
                            }
                        }

                        if (Time >= 106 && Time % 2 == 0)
                            Main.instance.CameraModifiers.Add(new PunchCameraModifier(saveTarget, Main.rand.NextVector2CircularEdge(3, 3), 5f, 10, 12));
                    }
                }
            }
            

            if (Time > ceilingCount * 170 + 10)
                Reset();
        }
        
        private void CollidingCrush()
        {
            int waitTime = 70;
            if (Time == waitTime + 5)
            {
                SoundStyle createSound = AssetDirectory.Sounds.Slime.CrimslimeTelegraph;
                SoundEngine.PlaySound(createSound.WithVolumeScale(1.5f), NPC.Center);
            }

            if (Time < 62)
            {
                NPC.velocity *= 0.1f;
                squishFactor = new Vector2(1f + (float)Math.Cbrt(Utils.GetLerpValue(15, 56, Time, true)) * 0.4f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(15, 56, Time, true)) * 0.5f);
                if (Time == 58)
                {
                    SoundStyle hop = AssetDirectory.Sounds.Goozma.SlimeJump;
                    SoundEngine.PlaySound(hop, NPC.Center);
                    SoundEngine.PlaySound(SoundID.QueenSlime, NPC.Center);
                }
            }

            else if (Time < 65)
            {
                NPC.velocity.Y = -14;
                squishFactor = new Vector2(0.5f, 1.4f);
            }

            else if (Time < waitTime + 80)
            {
                useNinjaSlamFrame = true;

                Vector2 airTarget = Target.Center - Vector2.UnitY * 320;

                if (Time < waitTime + 20)
                    squishFactor = new Vector2(1f - Utils.GetLerpValue(waitTime + 18, waitTime, Time, true) * 0.5f, 1f + (float)Math.Cbrt(Utils.GetLerpValue(waitTime + 18, waitTime, Time, true)) * 0.4f);

                if (Time < waitTime + 50)
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(airTarget).SafeNormalize(Vector2.Zero) * Math.Max(2, NPC.Distance(airTarget)) * 0.1f, 0.06f) * Utils.GetLerpValue(waitTime + 50, waitTime + 35, Time, true);
                    saveTarget = Target.Center;
                    NPC.rotation = NPC.rotation.AngleLerp(NPC.Top.AngleTo(NPC.FindSmashSpot(saveTarget)) - MathHelper.PiOver2, 0.5f) * 0.4f;
                }
                else
                {
                    NPC.rotation = NPC.rotation.AngleLerp(NPC.Top.AngleTo(NPC.FindSmashSpot(saveTarget)) - MathHelper.PiOver2, 0.2f);
                    NPC.Center = Vector2.Lerp(NPC.Center, NPC.FindSmashSpot(saveTarget), Utils.GetLerpValue(waitTime + 53, waitTime + 60, Time, true));
                    NPC.velocity *= 0.5f;

                    if (Time < waitTime + 60)
                        squishFactor = new Vector2(1f - (float)Math.Pow(Utils.GetLerpValue(waitTime + 50, waitTime + 53, Time, true), 2) * 0.5f, 1f + (float)Math.Pow(Utils.GetLerpValue(waitTime + 50, waitTime + 52, Time, true), 2) * 0.5f);

                    if (Time > waitTime + 60)
                    {
                        NPC.rotation = 0;
                        squishFactor = new Vector2(1f + (float)Math.Pow(Utils.GetLerpValue(waitTime + 78, waitTime + 63, Time, true), 2) * 0.6f, 1f - (float)Math.Pow(Utils.GetLerpValue(waitTime + 78, waitTime + 63, Time, true), 4) * 0.8f);
                    }

                    if (Time == waitTime + 60)
                    {
                        int count = (int)DifficultyBasedValue(12, death: 16);
                        int time = (int)DifficultyBasedValue(6, 5, 4, 3);

                        for (int i = 0; i < count; i++)
                        {
                            Projectile leftProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.FindSmashSpot(NPC.Center + new Vector2(i * 130 - 130 * count - 60, 0)), Vector2.Zero, ModContent.ProjectileType<CrimulanSmasher>(), GetDamage(1), 0);
                            leftProj.ai[0] = -30 - time * i;
                            leftProj.ai[1] = -1;
                            leftProj.localAI[0] = 1;                            
                            Projectile rightProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.FindSmashSpot(NPC.Center + new Vector2(i * -130 + 130 * count + 60, 0)), Vector2.Zero, ModContent.ProjectileType<CrimulanSmasher>(), GetDamage(1), 0);
                            rightProj.ai[0] = -30 - time * i;
                            rightProj.ai[1] = -1;
                            rightProj.localAI[0] = 1;
                        }

                        //Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom, Vector2.Zero, ModContent.ProjectileType<CrimulanShockwave>(), 0, 0, ai1: 2000);

                        SoundStyle slam = AssetDirectory.Sounds.Slime.SlimeSlam;
                        SoundEngine.PlaySound(slam, NPC.Center);

                        for (int i = 0; i < Main.rand.Next(30, 40); i++)
                        {
                            Vector2 velocity = Main.rand.NextVector2Circular(8, 1) - Vector2.UnitY * Main.rand.NextFloat(7f, 15f);
                            Vector2 position = NPC.Center + Main.rand.NextVector2Circular(1, 50) + new Vector2(velocity.X * 15f, 32f);
                            ParticleBehavior.NewParticle(ModContent.GetInstance<CrimBombChunkParticleBehavior>(), position, velocity, Color.White, 0.1f + Main.rand.NextFloat(2f));
                        }
                    }

                    if (Time >= waitTime + 55 && Time % 2 == 0)
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(saveTarget, Main.rand.NextVector2CircularEdge(3, 3), 5f, 10, 12));
                }
            }

            int doubleWaitTime = waitTime + 20;
            if (Time > doubleWaitTime + 80 && Time < doubleWaitTime + 150)
            {
                if (Time < doubleWaitTime + 100)
                {
                    NPC.velocity *= 0.1f;
                    squishFactor = new Vector2(1f + (float)Math.Cbrt(Utils.GetLerpValue(doubleWaitTime + 80, doubleWaitTime + 100, Time, true)) * 0.4f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(doubleWaitTime + 80, doubleWaitTime + 100, Time, true)) * 0.5f);

                    if (Time == doubleWaitTime + 95)
                    {
                        SoundStyle hop = AssetDirectory.Sounds.Goozma.SlimeJump;
                        SoundEngine.PlaySound(hop, NPC.Center);
                    }
                }
                else if (Time < doubleWaitTime + 130)
                {
                    NPC.velocity.Y = -20 * Utils.GetLerpValue(doubleWaitTime + 160, doubleWaitTime + 80, Time, true);
                    squishFactor = new Vector2(1f - Utils.GetLerpValue(doubleWaitTime + 150, doubleWaitTime + 110, Time, true) * 0.5f, 1f + Utils.GetLerpValue(doubleWaitTime + 150, doubleWaitTime + 110, Time, true) * 0.5f);
                }
            }

            if (Time == doubleWaitTime + 125)
            {
                SoundStyle createSound = AssetDirectory.Sounds.Slime.CrimslimeTelegraph;
                SoundEngine.PlaySound(createSound.WithVolumeScale(1.5f), NPC.Center);
            }
            if (Time > doubleWaitTime + 100 && Time < doubleWaitTime + 170)
            {
                useNinjaSlamFrame = true;

                Vector2 airTarget = Target.Center - Vector2.UnitY * 320;

                squishFactor = Vector2.Lerp(squishFactor, Vector2.One, 0.1f);
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(airTarget).SafeNormalize(Vector2.Zero) * Math.Max(2, NPC.Distance(airTarget)) * 0.1f, 0.06f) * Utils.GetLerpValue(doubleWaitTime + 150, doubleWaitTime + 135, Time, true);
                saveTarget = Target.Center;
                NPC.rotation = NPC.rotation.AngleLerp(NPC.Top.AngleTo(NPC.FindSmashSpot(saveTarget)) - MathHelper.PiOver2, 0.5f) * 0.4f;
            }

            if (Time > doubleWaitTime + 170 && Time <= doubleWaitTime + 180)
            {
                useNinjaSlamFrame = true;

                squishFactor = new Vector2(0.5f, 1.5f);
                NPC.Center = Vector2.Lerp(NPC.Center, NPC.FindSmashSpot(saveTarget) + Vector2.UnitY, Utils.GetLerpValue(doubleWaitTime + 173, doubleWaitTime + 180, Time, true));
                useNinjaSlamFrame = true;
            }

            if (Time == doubleWaitTime + 180)
            {
                NPC.rotation = 0;

                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom, Vector2.Zero, ModContent.ProjectileType<CrimulanShockwave>(), 0, 0, ai1: 2500);

                SoundStyle slam = AssetDirectory.Sounds.Slime.SlimeSlam;
                SoundEngine.PlaySound(slam, NPC.Center);

                for (int i = 0; i < Main.rand.Next(30, 40); i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(8, 1) - Vector2.UnitY * Main.rand.NextFloat(7f, 15f);
                    Vector2 position = NPC.Center + Main.rand.NextVector2Circular(1, 50) + new Vector2(velocity.X * 15f, 32f);
                    ParticleBehavior.NewParticle(ModContent.GetInstance<CrimBombChunkParticleBehavior>(), position, velocity, Color.White, 0.1f + Main.rand.NextFloat(2f));
                }

                squishFactor = new Vector2(1.5f, 0.5f);

                int count = (int)DifficultyBasedValue(12, death: 16);
                int time = (int)DifficultyBasedValue(6, 5, 4, 3);

                for (int i = 0; i < count; i++)
                {
                    Projectile leftProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.FindSmashSpot(NPC.Center + new Vector2(i * 130 - 130 * count - 60, 0)), Vector2.Zero, ModContent.ProjectileType<CrimulanSmasher>(), GetDamage(1), 0);
                    leftProj.ai[0] = -30 - time * i;
                    leftProj.ai[1] = -1;
                    leftProj.localAI[0] = 1;                            
                    Projectile rightProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.FindSmashSpot(NPC.Center + new Vector2(i * -130 + 130 * count + 60, 0)), Vector2.Zero, ModContent.ProjectileType<CrimulanSmasher>(), GetDamage(1), 0);
                    rightProj.ai[0] = -30 - time * i;
                    rightProj.ai[1] = -1;
                    rightProj.localAI[0] = 1;
                }
            }

            if (Time > doubleWaitTime + 180)
                squishFactor = Vector2.Lerp(squishFactor, Vector2.One, 0.1f);

            if (Time > doubleWaitTime + 260)
                Reset();
        }

        private void EndlessChase()
        {
            int jumpCount = (int)DifficultyBasedValue(8, 9, 10, 11, 12);
            int jumpTime = (int)DifficultyBasedValue(70, 65, 60, 55);
            float jumpHeight = NPC.height * 6f;

            if (Time < 35)
                squishFactor = new Vector2(1f + (float)Math.Pow(Utils.GetLerpValue(5, 35, Time, true), 2) * 0.5f, 1f - (float)Math.Pow(Utils.GetLerpValue(5, 35, Time, true), 2) * 0.5f);

            else if (Time < 35 + jumpCount * jumpTime)
            {
                float localTime = (Time - 35) % jumpTime;

                if (localTime < 1)
                {
                    NPC.velocity.Y -= 30;
                    saveTarget = Target.Center + new Vector2((Main.rand.Next(0, 50) + Math.Abs(Target.Velocity.X) * 25 + 360) * (Target.Center.X > NPC.Center.X ? 1 : -1), NPC.height);
                    
                    SoundStyle hop = AssetDirectory.Sounds.Goozma.SlimeJump;
                    SoundEngine.PlaySound(hop, NPC.Center);
                }
                else if (localTime < (int)(jumpTime * 0.72f))
                {
                    Vector2 midPoint = new Vector2((NPC.Center.X + saveTarget.X) / 2f, NPC.Center.Y - jumpHeight);
                    Vector2 jumpTarget = Vector2.Lerp(Vector2.Lerp(NPC.Center, midPoint, Utils.GetLerpValue(0, jumpTime * 0.3f, localTime, true)), Vector2.Lerp(midPoint, NPC.FindSmashSpot(saveTarget), Utils.GetLerpValue(jumpTime * 0.1f, jumpTime * 0.6f, localTime, true)), Utils.GetLerpValue(0, jumpTime * 0.6f, localTime, true));
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(NPC.FindSmashSpot(jumpTarget)).SafeNormalize(Vector2.Zero) * NPC.Distance(NPC.FindSmashSpot(jumpTarget)) * 0.3f * Utils.GetLerpValue(0, jumpTime * 0.7f, localTime, true), Utils.GetLerpValue(0, jumpTime * 0.7f, localTime, true));
                    NPC.rotation = -NPC.velocity.Y * 0.008f * Math.Sign(NPC.velocity.X);

                    float resquish = Utils.GetLerpValue(jumpTime * 0.4f, 0, localTime, true) + Utils.GetLerpValue(jumpTime * 0.4f, jumpTime * 0.7f, localTime, true);
                    squishFactor = new Vector2(1f - (float)Math.Pow(resquish, 2) * 0.5f, 1f + (float)Math.Pow(resquish, 2) * 0.5f);
                }

                if (localTime > (int)(jumpTime * 0.74f))
                {
                    NPC.frameCounter++;

                    NPC.velocity *= 0.2f;
                    NPC.rotation = 0;

                    if (localTime % 2 == 0)
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(saveTarget, Main.rand.NextVector2CircularEdge(3, 3), 5f, 10, 12));

                    squishFactor = new Vector2(1f + (float)Math.Pow(Utils.GetLerpValue(jumpTime, jumpTime * 0.74f, localTime, true), 2) * 0.8f, 1f - (float)Math.Pow(Utils.GetLerpValue(jumpTime, jumpTime * 0.74f, localTime, true), 2) * 0.7f);
                }
                if (localTime == (int)(jumpTime * 0.74f))
                {
                    foreach (Player player in Main.player.Where(n => n.active && !n.dead && n.Distance(NPC.Center) < 600))
                        player.velocity += player.DirectionFrom(NPC.Bottom + Vector2.UnitY * 10) * 3;

                    SoundStyle slam = AssetDirectory.Sounds.Slime.SlimeSlam;
                    SoundEngine.PlaySound(slam, NPC.Center);

                    for (int i = 0; i < Main.rand.Next(20, 30); i++)
                    {
                        Vector2 velocity = Main.rand.NextVector2Circular(8, 1) - Vector2.UnitY * Main.rand.NextFloat(7f, 15f);
                        Vector2 position = NPC.Center + Main.rand.NextVector2Circular(1, 50) + new Vector2(velocity.X * 15f, 32f);
                        ParticleBehavior.NewParticle(ModContent.GetInstance<CrimBombChunkParticleBehavior>(), position, velocity, Color.White, 0.1f + Main.rand.NextFloat(2f));
                    }
                }
            }

            if (Time > 36 + jumpCount * jumpTime)
                Reset();
        }

        private void SlamDown()
        {
            NPC.damage = 0;

            if (Time < 20)
            {
                squishFactor = Vector2.Lerp(squishFactor, new Vector2(1.4f, 0.7f), Time / 40f);
                if (Time == 18)
                {
                    NPC.velocity.Y = -20;
                    SoundStyle hop = AssetDirectory.Sounds.Goozma.SlimeJump with { Pitch = -0.3f };
                    SoundEngine.PlaySound(hop, NPC.Center);
                }
            }
            else if (Time < 70)
            {
                squishFactor = Vector2.Lerp(new Vector2(0.6f, 1.5f), Vector2.One, Utils.GetLerpValue(40, 60, Time, true));
                Vector2 airTarget = Target.Center - Vector2.UnitY * 120;
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(airTarget).SafeNormalize(Vector2.Zero) * Math.Max(2, NPC.Distance(airTarget)) * 0.3f, 0.4f * (float)Math.Pow(Utils.GetLerpValue(20, 60, Time, true), 1.5f));
            }
            else
            {
                useNinjaSlamFrame = true;
                float distance = 0;
                for (int i = 0; i < Main.tile.Height; i++)
                {
                    Point playerTile = Target.Center.ToTileCoordinates();
                    if (WorldGen.InWorld(playerTile.X, playerTile.Y + i) && WorldGen.InWorld(playerTile.X, playerTile.Y + i + 30))
                    {
                        if (WorldGen.SolidTile3(playerTile.X, playerTile.Y + i))
                        {
                            distance = i;
                            break;
                        }
                    }
                    else
                    {
                        distance = i - 20;
                        break;
                    }
                }

                if (distance > 4 && Time < 90)
                {
                    squishFactor = new Vector2(0.7f, 1.4f);
                    SetTime(80);
                    NPC.velocity.X *= 0.7f;
                    NPC.velocity.Y = 40;
                    NPC.noTileCollide = true;
                    if (Target.Type == Terraria.Enums.NPCTargetType.Player)
                    {
                        Main.player[NPC.target].Bottom = NPC.Bottom;
                        Main.player[NPC.target].velocity.X *= 0.3f;
                        Main.player[NPC.target].AddBuff(BuffID.Slow, 30, true);
                    }
                }
                else if (Time < 90)
                    SetTime(90);

                if (Time == 90)
                {
                    squishFactor = new Vector2(1.5f, 0.6f);
                    NPC.velocity.Y = -16;
                    for (int i = 0; i < Main.rand.Next(40, 60); i++)
                    {
                        Vector2 velocity = Main.rand.NextVector2Circular(8, 1) - Vector2.UnitY * Main.rand.NextFloat(15f, 20f);
                        Vector2 position = NPC.Center + Main.rand.NextVector2Circular(1, 50) + new Vector2(velocity.X * 15f, 32f);
                        ParticleBehavior.NewParticle(ModContent.GetInstance<CrimBombChunkParticleBehavior>(), position, velocity, Color.White, 0.1f + Main.rand.NextFloat(2f));
                    }
                }
                if (Time > 90)
                {
                    NPC.velocity *= 0.9f;
                    squishFactor = Vector2.Lerp(squishFactor * new Vector2(0.98f, 1.02f), Vector2.One, 0.13f);
                }
            }

            if (Time > 140)
                SetAttack((int)RememberAttack, true);
        }

        private int GetDamage(int attack, float modifier = 1f)
        {
            int damage = attack switch
            {
                0 => Main.expertMode ? 600 : 300,//contact
                1 => 100,//smasher
                2 => 150,//shockwave
                _ => damage = 0
            };

            return (int)(damage * modifier);
        }

        public int npcFrame;

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 9)
            {
                NPC.frameCounter = 0;
                npcFrame = (npcFrame + 1) % Main.npcFrameCount[Type];
            }
        }

        public bool useNinjaSlamFrame;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> ninja = AssetDirectory.Textures.Goozma.Ninja;
            Rectangle frame = texture.Frame(1, 4, 0, npcFrame);
            Asset<Texture2D> rayTell = TextureAssets.Extra[60];
            Asset<Texture2D> tell = TextureAssets.Extra[178];
            Color color = Color.White;

            switch (Attack)
            {
                case (int)AttackList.SlamRain:
                    float tellFade = Utils.GetLerpValue(30, 85, Time % 170, true);
                    spriteBatch.Draw(tell.Value, NPC.Bottom - new Vector2(0, NPC.height / 2f * squishFactor.Y).RotatedBy(NPC.rotation) - screenPos, null, new Color(200, 10, 20, 0) * tellFade, NPC.rotation + MathHelper.PiOver2, tell.Size() * new Vector2(0f, 0.5f), new Vector2(1f - tellFade, 110) * new Vector2(squishFactor.Y, squishFactor.X), 0, 0);
                    spriteBatch.Draw(tell.Value, NPC.Bottom - new Vector2(0, NPC.height / 2f * squishFactor.Y).RotatedBy(NPC.rotation) - screenPos, null, new Color(200, 10, 20, 0) * tellFade, NPC.rotation - MathHelper.PiOver2, tell.Size() * new Vector2(0f, 0.5f), new Vector2((1f - tellFade) * 0.2f, 110) * new Vector2(squishFactor.Y, squishFactor.X), 0, 0);

                    break;

                case (int)AttackList.CollidingCrush:
                    if (Time > 50 && Time < 130)
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            Rectangle tellframe = texture.Frame(1, 4, 0, (int)((npcFrame + i * 0.5f) % 4));

                            Vector2 offset = new Vector2(90 * i * (float)Math.Pow(Utils.GetLerpValue(70, 100, Time, true), 2) * (float)Math.Sqrt(Utils.GetLerpValue(70, 100, Time, true)), 0);
                            spriteBatch.Draw(texture.Value, NPC.Bottom - offset - new Vector2(0, (float)Math.Pow(i, 2f)).RotatedBy(NPC.rotation) - screenPos, tellframe, new Color(180, 20, 20, 20) * (0.5f - i / 14f) * Utils.GetLerpValue(70, 80, Time, true), NPC.rotation, frame.Size() * new Vector2(0.5f, 1f), NPC.scale * squishFactor, 0, 0);
                            spriteBatch.Draw(texture.Value, NPC.Bottom + offset - new Vector2(0, (float)Math.Pow(i, 2f)).RotatedBy(NPC.rotation) - screenPos, tellframe, new Color(180, 20, 20, 20) * (0.5f - i / 14f) * Utils.GetLerpValue(70, 80, Time, true), NPC.rotation, frame.Size() * new Vector2(0.5f, 1f), NPC.scale * squishFactor, 0, 0);
                        }
                    }
                    if (Time > 180)
                    {
                        tellFade = Utils.GetLerpValue(180, 195, Time, true) * Utils.GetLerpValue(240, 200, Time, true);
                        spriteBatch.Draw(texture.Value, NPC.Bottom - screenPos + Vector2.UnitY * 40 * Utils.GetLerpValue(190, 240, Time, true), frame, new Color(180, 20, 20, 20) * tellFade, NPC.rotation, frame.Size() * new Vector2(0.5f, 1f), (1f + Utils.GetLerpValue(190, 240, Time, true)) * NPC.scale * squishFactor, 0, 0);
                    }

                    break;

                case (int)AttackList.TooFar:
                    color = Color.Lerp(new Color(100, 100, 100, 0), Color.White, Math.Clamp(NPC.Distance(Target.Center), 100, 300) / 200f);
                    break;
            }

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[Type]; i++)
            {
                Vector2 oldPos = NPC.oldPos[i] + NPC.Size * new Vector2(0.5f, 0f);
                Color trailColor = Color.Lerp(new Color(40, 0, 0, 255), new Color(70, 10, 10, 0), i / (float)NPCID.Sets.TrailCacheLength[Type]) * Math.Clamp(NPC.velocity.Length() * 0.01f, 0, 1) * 0.5f;
                spriteBatch.Draw(texture.Value, oldPos - screenPos, frame, trailColor, NPC.rotation, frame.Size() * 0.5f, NPC.scale * squishFactor, 0, 0);
            }

            Vector2 ninjaPos = NPC.Bottom + new Vector2(0, -60 - (float)Math.Cos(npcFrame * MathHelper.PiOver2) * 4) * squishFactor;
            if (NPC.IsABestiaryIconDummy)
                ninjaPos.Y += 16;

            Rectangle ninjaFrame = ninja.Frame(2, 1, useNinjaSlamFrame ? 1 : 0, 0);
            float ninjaRotation = 0;
            if (Attack == (int)AttackList.EndlessChase)
                ninjaRotation = NPC.velocity.X;
            else
                ninjaRotation = NPC.rotation - NPC.velocity.X * 0.01f;

            spriteBatch.Draw(ninja.Value, ninjaPos + NPC.velocity * 0.2f - screenPos, ninjaFrame, color, ninjaRotation, ninjaFrame.Size() * 0.5f, NPC.scale * (new Vector2(0.5f) + squishFactor * 0.5f), 0, 0);
            spriteBatch.Draw(texture.Value, NPC.Bottom - screenPos, frame, color, NPC.rotation, frame.Size() * new Vector2(0.5f, 1f), NPC.scale * squishFactor, 0, 0);

            return false;
        }

        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            GoozmaResistances.GoozmaItemResistances(item, ref modifiers);
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            GoozmaResistances.GoozmaProjectileResistances(projectile, ref modifiers);
        }

        public void SetAttack(AttackList attack, bool zeroTime = false) => SetAttack((int)attack, zeroTime);

        public void SetAttack(int attack, bool zeroTime = false)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Attack = attack;
                if (zeroTime)
                    Time = 0;
                NPC.netUpdate = true;
            }
        }        
        
        public void StoreAttack()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                RememberAttack = Attack;
                NPC.netUpdate = true;
            }
        }

        public void SetTime(int time)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Time = time;
                NPC.netUpdate = true;
            }
        }
    }
}
