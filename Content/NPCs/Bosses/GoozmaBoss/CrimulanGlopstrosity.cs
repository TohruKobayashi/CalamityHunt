using System;
using System.Collections.Generic;
using System.Linq;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss.Projectiles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using XPT.Core.Audio.MP3Sharp.IO;
using static CalamityHunt.Common.Systems.ConditionalValue;

namespace CalamityHunt.Content.NPCs.Bosses.GoozmaBoss
{
    [AutoloadBossHead]
    public class CrimulanGlopstrosity : ModNPC
    {
        /// <summary>
        /// When the telegraph is played
        /// </summary>
        public const int Crim2Telegraph = 65;
        /// <summary>
        /// When one slam ends
        /// </summary>
        public const int Crim2SlamTimeMax = 140;
        /// <summary>
        /// When the clones are spawned
        /// </summary>
        public const int Crim2ImpactMoment = 120;
        /// <summary>
        /// How long one attack cycle lasts
        /// </summary>
        public const int Crim2SlamCycleTime = 200;

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
            if (Common.ModCompatibility.Calamity.IsLoaded) {
                NPCID.Sets.SpecificDebuffImmunity[Type][Common.ModCompatibility.Calamity.Mod.Find<ModBuff>("VulnerabilityHex").Type] = true;
            }

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new()
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
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
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

        // time is updated every frame. 60 frames = 1 sec, scuuuuuuuuule!
        public ref float Time => ref NPC.ai[0];
        public ref float Attack => ref NPC.ai[1];
        public ref NPC Host => ref Main.npc[(int)NPC.ai[2]];
        public ref float RememberAttack => ref NPC.ai[3];

        public NPCAimedTarget Target => NPC.GetTargetData();
        public Vector2 squishFactor = Vector2.One;

        public override void AI()
        {
            useNinjaSlamFrame = false;

            // despawn if Goozma isn't active
            if (!Main.npc.Any(n => n.type == ModContent.NPCType<Goozma>() && n.active)) {
                NPC.active = false;
            }
            // otherwise set its host to Goozma
            else {
                NPC.ai[2] = Main.npc.First(n => n.type == ModContent.NPCType<Goozma>() && n.active).whoAmI;
            }

            NPC.realLife = Host.whoAmI;

            if (!NPC.HasPlayerTarget) {
                NPC.TargetClosestUpgraded();
            }

            if (!NPC.HasPlayerTarget) {
                NPC.active = false;
            }

            NPC.damage = GetDamage(0);

            // spawn animation
            if (Time < 0) {
                NPC.velocity *= 0.9f;
                NPC.damage = 0;
                squishFactor = new Vector2(1f - (float)Math.Pow(Utils.GetLerpValue(-10, -45, Time, true), 2) * 0.5f, 1f + (float)Math.Pow(Utils.GetLerpValue(-10, -45, Time, true), 2) * 0.4f);
                if (Time == -2) {
                    StoreAttack();

                    // i dont think this code even matters since going too far automatically despawns the guy
                    if (NPC.Distance(Target.Center) > 1000) {
                        SetAttack(AttackList.TooFar);
                    }

                    // if the player is more than 36 tiles off the ground, grab them and pull them down
                    float distance = 0;
                    for (int i = 0; i < Main.tile.Height; i++) {
                        Point playerTile = Target.Center.ToTileCoordinates();
                        if (WorldGen.InWorld(playerTile.X, playerTile.Y + i)) {
                            if (WorldGen.SolidTile3(playerTile.X, playerTile.Y + i)) {
                                distance = i;
                                break;
                            }
                        }
                        else {
                            distance = i;
                            break;
                        }
                    }
                    if (distance > 36) {
                        SetAttack(AttackList.SlamDown);
                    }
                }

                NPC.frameCounter++;
            }
            else {
                switch (Attack) {
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

                    // chill for a bit then despawn
                    case (int)AttackList.Interrupt:
                        NPC.noTileCollide = true;
                        NPC.damage = 0;
                        NPC.velocity *= 0.5f;

                        if (Time < 15) {
                            Host.ai[0] = 0;
                        }

                        break;

                    // functionally unused
                    case (int)AttackList.TooFar:

                        if (Time < 5) {
                            saveTarget = Target.Center;
                        }

                        Vector2 midPoint = new Vector2((NPC.Center.X + saveTarget.X) / 2f, NPC.Center.Y);
                        Vector2 jumpTarget = Vector2.Lerp(Vector2.Lerp(NPC.Center, midPoint, Utils.GetLerpValue(0, 15, Time, true)), Vector2.Lerp(midPoint, saveTarget, Utils.GetLerpValue(5, 30, Time, true)), Utils.GetLerpValue(0, 30, Time, true));
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(jumpTarget).SafeNormalize(Vector2.Zero) * NPC.Distance(jumpTarget) * 0.5f * Utils.GetLerpValue(0, 35, Time, true), (float)Math.Pow(Utils.GetLerpValue(0, 40, Time, true), 2f));
                        Host.ai[0] = 0;

                        if (Time < 40) {
                            squishFactor = Vector2.Lerp(Vector2.One, new Vector2(0.6f, 1.3f), Time / 40f);
                        }
                        else {
                            squishFactor = Vector2.Lerp(new Vector2(1.4f, 0.5f), Vector2.One, Utils.GetLerpValue(40, 54, Time, true));
                        }

                        if (Time > 55) {
                            NPC.velocity *= 0f;
                            SetAttack((int)RememberAttack, true);
                        }
                        break;

                }
            }

            Time++;
        }

        private void Reset()
        {
            SetAttack(AttackList.Interrupt, true);
            squishFactor = Vector2.One;
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                NPC.netUpdate = true;
            }
        }

        public Vector2 saveTarget;

        // attempt to slam onto the player, then create spreads of clones on impact and repeat
        private void SlamRain()
        {
            // the amount of times this attack is performed
            int ceilingCount = (int)DifficultyBasedValue(2, 2, 3, 4, master: 3, masterrev: 4, masterdeath: 6);
            int waveTime = 170;

            bool disableDamage = true;


            // most of the behaviour
            //
            if (Time <= ceilingCount * waveTime) {
                float localTime = Time % waveTime;

                if (localTime == 35) {
                    SoundStyle createSound = AssetDirectory.Sounds.GoozmaMinions.CrimslimeTelegraph;
                    SoundEngine.PlaySound(createSound.WithVolumeScale(1.5f), NPC.Center);
                }

                // slow to a stop then jump
                if (localTime < 32) {
                    NPC.velocity *= 0.1f;
                    squishFactor = new Vector2(1f + (float)Math.Cbrt(Utils.GetLerpValue(10, 26, localTime, true)) * 0.4f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(10, 26, localTime, true)) * 0.5f);
                    if (localTime == 25) {
                        SoundStyle hop = AssetDirectory.Sounds.Goozma.SlimeJump;
                        SoundEngine.PlaySound(hop, NPC.Center);
                        SoundEngine.PlaySound(SoundID.QueenSlime, NPC.Center);
                    }

                }

                // rise to position where the stomp will begin
                else if (localTime < 35) {
                    NPC.velocity.Y = -14;
                    squishFactor = new Vector2(0.5f, 1.4f);
                }

                // everything else
                else if (localTime < 140) {
                    Vector2 airTarget = Target.Center - Vector2.UnitY * 450 + Target.Velocity * 4;

                    // smash towards the player
                    if (localTime < 70) {
                        squishFactor = Vector2.Lerp(Vector2.One, new Vector2(0.5f, 1.4f), (float)Math.Cbrt(Utils.GetLerpValue(58, 40, localTime, true)));
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(airTarget).SafeNormalize(Vector2.Zero) * Math.Max(2, NPC.Distance(airTarget)) * 0.2f, 0.08f) * Utils.GetLerpValue(90, 75, localTime, true);
                        saveTarget = Target.Center + Target.Velocity * 5;
                        NPC.rotation = NPC.rotation.AngleLerp(NPC.Top.AngleTo(NPC.FindSmashSpot(saveTarget)) - MathHelper.PiOver2, 0.5f) * 0.4f; 
                    }
                    else {
                        disableDamage = false;
                        NPC.rotation = NPC.rotation.AngleLerp(NPC.Top.AngleTo(NPC.FindSmashSpot(saveTarget)) - MathHelper.PiOver2, 0.2f);
                        NPC.Center = Vector2.Lerp(NPC.Center, NPC.FindSmashSpot(saveTarget), Utils.GetLerpValue(75, 100, localTime, true));
                        NPC.velocity *= 0.5f;
                        useNinjaSlamFrame = true;

                        // squiche
                        if (localTime < 100) {
                            squishFactor = Vector2.Lerp(Vector2.One, new Vector2(0.5f, 1.5f), (float)Math.Pow(Utils.GetLerpValue(75, 85, localTime, true), 2));
                            NPC.frameCounter++;
                        }

                        // squiche 2
                        if (localTime > 100) {
                            NPC.rotation = 0;
                            squishFactor = Vector2.Lerp(Vector2.One, new Vector2(0.5f, 1.5f), (float)Math.Pow(Utils.GetLerpValue(90, 93, localTime, true), 2));
                            squishFactor = new Vector2(1f + (float)Math.Pow(Utils.GetLerpValue(118, 103, localTime, true), 2) * 0.6f, 1f - (float)Math.Pow(Utils.GetLerpValue(118, 103, localTime, true), 4) * 0.8f);
                        }

                        // spawn clones
                        if (localTime == 100) {
                            // the amount of clones spawned 
                            int extension = (int)DifficultyBasedValue(10, 12, 16, 18, 20, master: 16, masterrev: 18, masterdeath: 20);
                            int telegraphTime = 60;

                            if (Main.netMode != NetmodeID.MultiplayerClient) {
                                // purely visual shockwave
                                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom, Vector2.Zero, ModContent.ProjectileType<CrimulanShockwave>(), 0, 0, ai1: 2500);

                                for (int i = 0; i < extension; i++) {
                                    Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2((Target.Center.X - NPC.Center.X) * 0.02f + i * Main.rand.NextFloat(-15f, 15f), 0), ModContent.ProjectileType<CrimulanSmasher>(), GetDamage(1), 0,
                                        ai0: -telegraphTime - i,
                                        ai1: -1,
                                        ai2: 1);
                                    proj.localAI[0] = 1;
                                }
                            }

                            // you Will stay.
                            foreach (Player player in Main.player.Where(n => n.active && !n.dead && n.Distance(NPC.Center) < 600)) {
                                player.velocity += player.DirectionFrom(NPC.Bottom + Vector2.UnitY * 10) * 5;
                            }

                            SoundStyle slam = AssetDirectory.Sounds.GoozmaMinions.SlimeSlam;
                            SoundEngine.PlaySound(slam, NPC.Center);

                            for (int i = 0; i < Main.rand.Next(30, 40); i++) {
                                Vector2 velocity = Main.rand.NextVector2Circular(8, 1) - Vector2.UnitY * Main.rand.NextFloat(7f, 15f);
                                Vector2 position = NPC.Center + Main.rand.NextVector2Circular(1, 50) + new Vector2(velocity.X * 15f, 32f);
                                CalamityHunt.particles.Add(Particle.Create<CrimGelChunk>(particle => {
                                    particle.position = position;
                                    particle.velocity = velocity;
                                    particle.scale = Main.rand.NextFloat(0.1f, 2.1f);
                                    particle.color = Color.White;
                                }));
                            }
                        }

                        // shake the screen
                        if (Time >= 106 && Time % 2 == 0) {
                            Main.instance.CameraModifiers.Add(new PunchCameraModifier(saveTarget, Main.rand.NextVector2CircularEdge(3, 3), 5f, 10, 12));
                        }
                    }
                }
            }

            if (disableDamage)
                NPC.damage = 0;

            // end attack
            if (Time > ceilingCount * waveTime + 10) {
                Reset();
            }
        }

        // stomp to create evenly spaced clones on both sides, then stomp again
        private void CollidingCrush()
        {
            int waitTime = 60; // how long he waits before jumping
            int firstJumpUp = 32; // when he first begins to jump
            int slowAfterJump = 35; // when he starts slowing down
            int jumpCount = 2; // how many times he jumps
            int waitAfterFinal = 60; // how long he waits after his final jump
            int totalTime = Crim2SlamCycleTime * jumpCount + waitAfterFinal; // time of the entire attack

            int minDist = 500; // minimum distance he jumps away on the second slam
            int maxDist = 700; // maximum distance he jumps away on the second slam

            bool disableDamage = true; // set false to enable contact damage

            float localTime = Time % Crim2SlamCycleTime;
            int jumpNumber = (int)Math.Ceiling(Time / Crim2SlamCycleTime);

            if (Time <= jumpCount * Crim2SlamCycleTime) {
                // produce telegraph 
                if (localTime == Crim2Telegraph) {
                    SoundStyle createSound = AssetDirectory.Sounds.GoozmaMinions.CrimslimeTelegraph;
                    SoundEngine.PlaySound(createSound.WithVolumeScale(1.5f), NPC.Center);
                }

                // big jump
                if (localTime < firstJumpUp) {
                    NPC.velocity *= 0.1f;
                    squishFactor = new Vector2(1f + (float)Math.Cbrt(Utils.GetLerpValue(15, 56, localTime, true)) * 0.4f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(15, 56, localTime, true)) * 0.5f);
                    if (localTime == 58) {
                        SoundStyle hop = AssetDirectory.Sounds.Goozma.SlimeJump;
                        SoundEngine.PlaySound(hop, NPC.Center);
                        SoundEngine.PlaySound(SoundID.QueenSlime, NPC.Center);
                    }
                }

                // slow down jump and prepare to slam down
                else if (localTime < slowAfterJump) {
                    NPC.velocity.Y = -14;
                    squishFactor = new Vector2(0.5f, 1.4f);
                }
                // save the target location
                else if (localTime == slowAfterJump) {
                    saveTarget = Target.Center + Vector2.UnitX * Main.rand.Next(minDist, maxDist) * Main.rand.NextBool().ToDirectionInt();
                    Point tileCords = saveTarget.ToTileCoordinates();
                    for (int i = 0; i < 200; i++) {
                        Tile t = Framing.GetTileSafely(tileCords.X, tileCords.Y + i);
                        if (t.HasTile && (Main.tileSolidTop[t.TileType] || Main.tileSolid[t.TileType])) {
                            saveTarget.Y += i * 16;
                            break;
                        }
                    }
                }

                // SLAM !!
                else if (localTime < Crim2SlamTimeMax) {
                    useNinjaSlamFrame = true;

                    if (localTime < waitTime + 20) {
                        squishFactor = new Vector2(1f - Utils.GetLerpValue(waitTime + 18, waitTime, localTime, true) * 0.5f, 1f + (float)Math.Cbrt(Utils.GetLerpValue(waitTime + 18, waitTime, localTime, true)) * 0.4f);
                    }

                    if (localTime < waitTime + 50) {
                        Vector2 airTarget = new Vector2(saveTarget.X, Target.Center.Y) - Vector2.UnitY * 320;

                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(airTarget).SafeNormalize(Vector2.Zero) * Math.Max(2, NPC.Distance(airTarget)) * 0.1f, 0.06f) * Utils.GetLerpValue(waitTime + 50, waitTime + 35, localTime, true);
                        
                        NPC.rotation = NPC.rotation.AngleLerp(NPC.Top.AngleTo(NPC.FindSmashSpot(saveTarget)) - MathHelper.PiOver2, 0.5f) * 0.4f;
                    }
                    else {
                        disableDamage = false;
                        NPC.rotation = NPC.rotation.AngleLerp(NPC.Top.AngleTo(NPC.FindSmashSpot(saveTarget)) - MathHelper.PiOver2, 0.2f);
                        NPC.Center = Vector2.Lerp(NPC.Center, NPC.FindSmashSpot(saveTarget), Utils.GetLerpValue(waitTime + 53, waitTime + 60, localTime, true));
                        NPC.velocity *= 0.5f;

                        if (localTime < Crim2ImpactMoment) {
                            squishFactor = new Vector2(1f - (float)Math.Pow(Utils.GetLerpValue(waitTime + 50, waitTime + 53, localTime, true), 2) * 0.5f, 1f + (float)Math.Pow(Utils.GetLerpValue(waitTime + 50, waitTime + 52, localTime, true), 2) * 0.5f);
                        }

                        if (localTime > Crim2ImpactMoment) {
                            NPC.rotation = 0;
                            squishFactor = new Vector2(1f + (float)Math.Pow(Utils.GetLerpValue(waitTime + 78, waitTime + 63, localTime, true), 2) * 0.6f, 1f - (float)Math.Pow(Utils.GetLerpValue(waitTime + 78, waitTime + 63, localTime, true), 4) * 0.8f);
                        }

                        if (localTime == Crim2ImpactMoment) {
                            // how many clones are spawned in each direction (multiply by 2 for total clones)
                            int count = (int)DifficultyBasedValue(12, death: 16, master: 16, masterrev: 18, masterdeath: 20);
                            // the delay at which clones slam down
                            int time = (int)DifficultyBasedValue(6, 5, 4, 3, master: 4, masterrev: 3, masterdeath: 2);
                            // how long it takes before the first clone slams down
                            int telegraphlocalTime = 50;

                            if (Main.netMode != NetmodeID.MultiplayerClient) {
                                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom, Vector2.Zero, ModContent.ProjectileType<CrimulanShockwave>(), 0, 0, ai1: 2500);

                                for (int i = 0; i < count; i++) {
                                    Projectile leftProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.FindSmashSpot(NPC.Center + new Vector2(i * 130 - 130 * count - 60, 0)), Vector2.Zero, ModContent.ProjectileType<CrimulanSmasher>(), GetDamage(1), 0,
                                        ai0: -telegraphlocalTime - time * i,
                                        ai1: -1);
                                    leftProj.localAI[0] = 1;
                                    Projectile rightProj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.FindSmashSpot(NPC.Center + new Vector2(i * -130 + 130 * count + 60, 0)), Vector2.Zero, ModContent.ProjectileType<CrimulanSmasher>(), GetDamage(1), 0,
                                        ai0: -telegraphlocalTime - time * i,
                                        ai1: -1);
                                    rightProj.localAI[0] = 1;
                                }
                            }

                            SoundStyle slam = AssetDirectory.Sounds.GoozmaMinions.SlimeSlam;
                            SoundEngine.PlaySound(slam, NPC.Center);

                            for (int i = 0; i < Main.rand.Next(30, 40); i++) {
                                Vector2 velocity = Main.rand.NextVector2Circular(8, 1) - Vector2.UnitY * Main.rand.NextFloat(7f, 15f);
                                Vector2 position = NPC.Center + Main.rand.NextVector2Circular(1, 50) + new Vector2(velocity.X * 15f, 32f);
                                CalamityHunt.particles.Add(Particle.Create<CrimGelChunk>(particle => {
                                    particle.position = position;
                                    particle.velocity = velocity;
                                    particle.scale = Main.rand.NextFloat(0.1f, 2.1f);
                                    particle.color = Color.White;
                                }));
                            }
                        }

                        if (localTime >= waitTime + 55 && localTime % 2 == 0) {
                            Main.instance.CameraModifiers.Add(new PunchCameraModifier(saveTarget, Main.rand.NextVector2CircularEdge(3, 3), 5f, 10, 12));
                        }
                    }
                }
            }

            if (Time > totalTime) {
                Reset();
            }

            if (disableDamage)
                NPC.damage = 0;
        }

        // no.
        private void EndlessChase()
        {
            int jumpCount = (int)DifficultyBasedValue(8, 9, 10, 11, 12, master: 10, masterrev: 11, masterdeath: 12);
            int jumpTime = (int)DifficultyBasedValue(70, 65, 60, 55, master: 60, masterrev: 55, masterdeath: 50);
            float jumpHeight = NPC.height * 6f;

            if (Time < 35) {
                squishFactor = new Vector2(1f + (float)Math.Pow(Utils.GetLerpValue(5, 35, Time, true), 2) * 0.5f, 1f - (float)Math.Pow(Utils.GetLerpValue(5, 35, Time, true), 2) * 0.5f);
            }
            else if (Time < 35 + jumpCount * jumpTime) {
                float localTime = (Time - 35) % jumpTime;

                if (localTime < 1) {
                    NPC.velocity.Y -= 30;
                    saveTarget = Target.Center + new Vector2((Main.rand.Next(0, 50) + Math.Abs(Target.Velocity.X) * 25 + 360) * (Target.Center.X > NPC.Center.X ? 1 : -1), NPC.height);

                    SoundStyle hop = AssetDirectory.Sounds.Goozma.SlimeJump;
                    SoundEngine.PlaySound(hop, NPC.Center);
                }
                else if (localTime < (int)(jumpTime * 0.72f)) {
                    Vector2 midPoint = new Vector2((NPC.Center.X + saveTarget.X) / 2f, NPC.Center.Y - jumpHeight);
                    Vector2 jumpTarget = Vector2.Lerp(Vector2.Lerp(NPC.Center, midPoint, Utils.GetLerpValue(0, jumpTime * 0.3f, localTime, true)), Vector2.Lerp(midPoint, NPC.FindSmashSpot(saveTarget), Utils.GetLerpValue(jumpTime * 0.1f, jumpTime * 0.6f, localTime, true)), Utils.GetLerpValue(0, jumpTime * 0.6f, localTime, true));
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(NPC.FindSmashSpot(jumpTarget)).SafeNormalize(Vector2.Zero) * NPC.Distance(NPC.FindSmashSpot(jumpTarget)) * 0.3f * Utils.GetLerpValue(0, jumpTime * 0.7f, localTime, true), Utils.GetLerpValue(0, jumpTime * 0.7f, localTime, true));
                    NPC.rotation = -NPC.velocity.Y * 0.008f * Math.Sign(NPC.velocity.X);

                    float resquish = Utils.GetLerpValue(jumpTime * 0.4f, 0, localTime, true) + Utils.GetLerpValue(jumpTime * 0.4f, jumpTime * 0.7f, localTime, true);
                    squishFactor = new Vector2(1f - (float)Math.Pow(resquish, 2) * 0.5f, 1f + (float)Math.Pow(resquish, 2) * 0.5f);
                }

                if (localTime > (int)(jumpTime * 0.74f)) {
                    NPC.frameCounter++;

                    NPC.velocity *= 0.2f;
                    NPC.rotation = 0;

                    if (localTime % 2 == 0) {
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(saveTarget, Main.rand.NextVector2CircularEdge(3, 3), 5f, 10, 12));
                    }

                    squishFactor = new Vector2(1f + (float)Math.Pow(Utils.GetLerpValue(jumpTime, jumpTime * 0.74f, localTime, true), 2) * 0.8f, 1f - (float)Math.Pow(Utils.GetLerpValue(jumpTime, jumpTime * 0.74f, localTime, true), 2) * 0.7f);
                }
                if (localTime == (int)(jumpTime * 0.74f)) {
                    foreach (Player player in Main.player.Where(n => n.active && !n.dead && n.Distance(NPC.Center) < 600)) {
                        player.velocity += player.DirectionFrom(NPC.Bottom + Vector2.UnitY * 10) * 3;
                    }

                    SoundStyle slam = AssetDirectory.Sounds.GoozmaMinions.SlimeSlam;
                    SoundEngine.PlaySound(slam, NPC.Center);

                    for (int i = 0; i < Main.rand.Next(20, 30); i++) {
                        Vector2 velocity = Main.rand.NextVector2Circular(8, 1) - Vector2.UnitY * Main.rand.NextFloat(7f, 15f);
                        Vector2 position = NPC.Center + Main.rand.NextVector2Circular(1, 50) + new Vector2(velocity.X * 15f, 32f);
                        CalamityHunt.particles.Add(Particle.Create<CrimGelChunk>(particle => {
                            particle.position = position;
                            particle.velocity = velocity;
                            particle.scale = Main.rand.NextFloat(0.1f, 2.1f);
                            particle.color = Color.White;
                        }));
                    }
                }
            }

            if (Time > 36 + jumpCount * jumpTime) {
                Reset();
            }
        }

        private void SlamDown()
        {
            NPC.damage = 0;

            if (Time < 20) {
                squishFactor = Vector2.Lerp(squishFactor, new Vector2(1.4f, 0.7f), Time / 40f);
                if (Time == 18) {
                    NPC.velocity.Y = -20;
                    SoundStyle hop = AssetDirectory.Sounds.Goozma.SlimeJump with { Pitch = -0.3f };
                    SoundEngine.PlaySound(hop, NPC.Center);
                }
            }
            else if (Time < 70) {
                squishFactor = Vector2.Lerp(new Vector2(0.6f, 1.5f), Vector2.One, Utils.GetLerpValue(40, 60, Time, true));
                Vector2 airTarget = Target.Center - Vector2.UnitY * 120;
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(airTarget).SafeNormalize(Vector2.Zero) * Math.Max(2, NPC.Distance(airTarget)) * 0.3f, 0.4f * (float)Math.Pow(Utils.GetLerpValue(20, 60, Time, true), 1.5f));
            }
            else {
                useNinjaSlamFrame = true;
                float distance = 0;
                for (int i = 0; i < Main.tile.Height; i++) {
                    Point playerTile = Target.Center.ToTileCoordinates();
                    if (WorldGen.InWorld(playerTile.X, playerTile.Y + i) && WorldGen.InWorld(playerTile.X, playerTile.Y + i + 30)) {
                        if (WorldGen.SolidTile3(playerTile.X, playerTile.Y + i)) {
                            distance = i;
                            break;
                        }
                    }
                    else {
                        distance = i - 20;
                        break;
                    }
                }

                if (distance > 4 && Time < 90) {
                    squishFactor = new Vector2(0.7f, 1.4f);
                    SetTime(80);
                    NPC.velocity.X *= 0.7f;
                    NPC.velocity.Y = 40;
                    NPC.noTileCollide = true;
                    if (Target.Type == Terraria.Enums.NPCTargetType.Player) {
                        Main.player[NPC.target].Bottom = NPC.Bottom;
                        Main.player[NPC.target].velocity.X *= 0.3f;
                        Main.player[NPC.target].AddBuff(BuffID.Slow, 30, true);
                    }
                }
                else if (Time < 90) {
                    SetTime(90);
                }

                if (Time == 90) {
                    squishFactor = new Vector2(1.5f, 0.6f);
                    NPC.velocity.Y = -16;
                    for (int i = 0; i < Main.rand.Next(40, 60); i++) {
                        Vector2 velocity = Main.rand.NextVector2Circular(8, 1) - Vector2.UnitY * Main.rand.NextFloat(15f, 20f);
                        Vector2 position = NPC.Center + Main.rand.NextVector2Circular(1, 50) + new Vector2(velocity.X * 15f, 32f);
                        CalamityHunt.particles.Add(Particle.Create<CrimGelChunk>(particle => {
                            particle.position = position;
                            particle.velocity = velocity;
                            particle.scale = Main.rand.NextFloat(0.1f, 2.1f);
                            particle.color = Color.White;
                        }));
                    }
                }
                if (Time > 90) {
                    NPC.velocity *= 0.9f;
                    squishFactor = Vector2.Lerp(squishFactor * new Vector2(0.98f, 1.02f), Vector2.One, 0.13f);
                }
            }

            if (Time > 140) {
                SetAttack((int)RememberAttack, true);
            }
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
            if (NPC.frameCounter > 9) {
                NPC.frameCounter = 0;
                npcFrame = (npcFrame + 1) % Main.npcFrameCount[Type];
            }
        }

        public bool useNinjaSlamFrame;

        public static Texture2D ninjaTexture;

        public override void Load()
        {
            ninjaTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "DeadNinja").Value;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Rectangle frame = texture.Frame(1, 4, 0, npcFrame);
            Texture2D rayTell = TextureAssets.Extra[60].Value;
            Texture2D tell = TextureAssets.Extra[178].Value;
            Color color = Color.White;

            switch (Attack) {
                case (int)AttackList.SlamRain:

                    float tellFade = Utils.GetLerpValue(30, 85, Time % 170, true);
                    spriteBatch.Draw(tell, NPC.Bottom - new Vector2(0, NPC.height / 2f * squishFactor.Y).RotatedBy(NPC.rotation) - screenPos, null, new Color(200, 10, 20, 0) * tellFade, NPC.rotation + MathHelper.PiOver2, tell.Size() * new Vector2(0f, 0.5f), new Vector2(1f - tellFade, 110) * new Vector2(squishFactor.Y, squishFactor.X), 0, 0);
                    spriteBatch.Draw(tell, NPC.Bottom - new Vector2(0, NPC.height / 2f * squishFactor.Y).RotatedBy(NPC.rotation) - screenPos, null, new Color(200, 10, 20, 0) * tellFade, NPC.rotation - MathHelper.PiOver2, tell.Size() * new Vector2(0f, 0.5f), new Vector2((1f - tellFade) * 0.2f, 110) * new Vector2(squishFactor.Y, squishFactor.X), 0, 0);

                    break;

                case (int)AttackList.CollidingCrush:
                    float localTime = Time % 200;
                    int maxTime = 140;

                    if (localTime > 50 && localTime < maxTime && Time < 400) {
                        for (int i = 0; i < 7; i++) {
                            Rectangle tellframe = texture.Frame(1, 4, 0, (int)((npcFrame + i * 0.5f) % 4));

                            Vector2 offset = new Vector2(90 * i * (float)Math.Pow(Utils.GetLerpValue(70, 100, localTime, true), 2) * (float)Math.Sqrt(Utils.GetLerpValue(70, 100, localTime, true)), 0);
                            if (localTime > 110)
                                offset = Vector2.Lerp(offset, Vector2.UnitX * 200 * i * Utils.GetLerpValue(70, 100, localTime, true), Utils.GetLerpValue(110, maxTime, localTime, true));
                            Color afterimageColor = new Color(180, 20, 20, 20) * (0.5f - i / 14f) * MathF.Pow(Utils.GetLerpValue(70, 80, localTime, true), 2) * (float)(Math.Sqrt(Utils.GetLerpValue(maxTime, 110, localTime, true)));
                            spriteBatch.Draw(texture, NPC.Bottom - offset - new Vector2(0, (float)Math.Pow(i, 2f)).RotatedBy(NPC.rotation) - screenPos, tellframe, afterimageColor, NPC.rotation, frame.Size() * new Vector2(0.5f, 1f), NPC.scale * squishFactor, 0, 0);
                            spriteBatch.Draw(texture, NPC.Bottom + offset - new Vector2(0, (float)Math.Pow(i, 2f)).RotatedBy(NPC.rotation) - screenPos, tellframe, afterimageColor, NPC.rotation, frame.Size() * new Vector2(0.5f, 1f), NPC.scale * squishFactor, 0, 0);
                        }
                    }

                    break;

                case (int)AttackList.TooFar:
                    color = Color.Lerp(new Color(100, 100, 100, 0), Color.White, Math.Clamp(NPC.Distance(Target.Center), 100, 300) / 200f);
                    break;
            }

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[Type]; i++) {
                Vector2 oldPos = NPC.oldPos[i] + NPC.Size * new Vector2(0.5f, 0f);
                Color trailColor = Color.Lerp(new Color(40, 0, 0, 255), new Color(70, 10, 10, 0), i / (float)NPCID.Sets.TrailCacheLength[Type]) * Math.Clamp(NPC.velocity.Length() * 0.01f, 0, 1) * 0.5f;
                spriteBatch.Draw(texture, oldPos - screenPos, frame, trailColor, NPC.rotation, frame.Size() * 0.5f, NPC.scale * squishFactor, 0, 0);
            }

            Vector2 ninjaPos = NPC.Bottom + new Vector2(0, -60 - (float)Math.Cos(npcFrame * MathHelper.PiOver2) * 4) * squishFactor;
            if (NPC.IsABestiaryIconDummy) {
                ninjaPos.Y += 16;
            }

            Rectangle ninjaFrame = ninjaTexture.Frame(2, 1, useNinjaSlamFrame ? 1 : 0, 0);
            float ninjaRotation = 0;
            if (Attack == (int)AttackList.EndlessChase) {
                ninjaRotation = NPC.velocity.X;
            }
            else {
                ninjaRotation = NPC.rotation - NPC.velocity.X * 0.01f;
            }

            spriteBatch.Draw(ninjaTexture, ninjaPos + NPC.velocity * 0.2f - screenPos, ninjaFrame, color, ninjaRotation, ninjaFrame.Size() * 0.5f, NPC.scale * (new Vector2(0.5f) + squishFactor * 0.5f), 0, 0);
            spriteBatch.Draw(texture, NPC.Bottom - screenPos, frame, color, NPC.rotation, frame.Size() * new Vector2(0.5f, 1f), NPC.scale * squishFactor, 0, 0);

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
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                Attack = attack;
                if (zeroTime) {
                    Time = 0;
                }

                NPC.netUpdate = true;
            }
        }

        public void StoreAttack()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                RememberAttack = Attack;
                NPC.netUpdate = true;
            }
        }

        public void SetTime(int time)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                Time = time;
                NPC.netUpdate = true;
            }
        }
    }
}
