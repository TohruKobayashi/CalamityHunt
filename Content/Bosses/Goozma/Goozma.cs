using CalamityHunt.Common;
using CalamityHunt.Common.DropRules;
using CalamityHunt.Common.Graphics.SlimeMonsoon;
using CalamityHunt.Common.Systems;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma.Projectiles;
using CalamityHunt.Content.Items.Accessories;
using CalamityHunt.Content.Items.Armor.Shogun;
using CalamityHunt.Content.Items.BossBags;
using CalamityHunt.Content.Items.Consumable;
using CalamityHunt.Content.Items.Lore;
using CalamityHunt.Content.Items.Masks;
using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Items.Misc;
using CalamityHunt.Content.Items.Misc.AuricSouls;
using CalamityHunt.Content.Items.Mounts;
using CalamityHunt.Content.Items.Weapons.Magic;
using CalamityHunt.Content.Items.Weapons.Melee;
using CalamityHunt.Content.Items.Weapons.Ranged;
using CalamityHunt.Content.Items.Weapons.Rogue;
using CalamityHunt.Content.Items.Weapons.Summoner;
using CalamityHunt.Content.Particles;
using CalamityHunt.Content.Pets.BloatBabyPet;
using CalamityHunt.Content.Projectiles;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arch.Core.Extensions;
using CalamityHunt.Core;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using static CalamityHunt.Common.Systems.DifficultySystem;

namespace CalamityHunt.Content.Bosses.Goozma
{
    [AutoloadBossHead]
    public partial class Goozma : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Goozma");
            NPCID.Sets.TrailCacheLength[Type] = 10;
            NPCID.Sets.TrailingMode[Type] = -1;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.ImmuneToAllBuffs[Type] = true;
            NPCID.Sets.ShouldBeCountedAsBoss[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Rotation = 0.01f,
                Velocity = 1f,
                Position = Vector2.One * 20f,
                PortraitScale = 0.66f
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement($"Mods.{nameof(CalamityHunt)}.Bestiary.Goozma"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.SlimeRain,
                new SlimeMonsoonPortraitBackground()
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 150;
            NPC.height = 150;
            NPC.damage = 0;
            NPC.defense = 110;
            NPC.lifeMax = 2750000;
            NPC.HitSound = null;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 35);
            NPC.SpawnWithHigherTime(30);
            NPC.direction = 1;
            NPC.boss = true;
            NPC.npcSlots = 10f;
            NPC.aiStyle = -1;
            NPC.dontTakeDamage = true;

            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot($"{nameof(CalamityHunt)}/Assets/Music/GlutinousArbitration");
                Music2 = MusicLoader.GetMusicSlot($"{nameof(CalamityHunt)}/Assets/Music/ViscousDesperation");
            }
            if (Main.expertMode)
            {
                NPC.lifeMax = 4500000;
            }
            if (ModLoader.HasMod("CalamityMod"))
            {
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.Call("SetDebuffVulnerabilities", "poison", false);
                calamity.Call("SetDebuffVulnerabilities", "heat", true);
                calamity.Call("SetDefenseDamageNPC", NPC, true);
                if ((bool)calamity.Call("GetDifficultyActive", "revengeance"))
                {
                    NPC.lifeMax = 6500000;
                }
            }

            if (Main.drunkWorld)
                SlimeUtils.GoozmaColorType = Main.rand.Next(10);

            //if (NPC.IsABestiaryIconDummy)
            //{
            //    if (nPCsToDrawCordOn.Count > 10)
            //        nPCsToDrawCordOn.RemoveAt(5);//preserve 5 of them

            //    nPCsToDrawCordOn.Add(NPC);
            //}

            SlimeUtils.GoozmaColorType = Main.rand.Next(54);
        }

        public int Music2;

        private static int relicType;
        public static int[] trophyTypes;

        public override void Load()
        {
            relicType = BossDropAutoloader.AddBossRelic("Goozma");
            trophyTypes = new int[5];
            trophyTypes[0] = BossDropAutoloader.AddBossTrophy("Goozma");          
            trophyTypes[1] = BossDropAutoloader.AddBossTrophy("EbonianBehemuck"); 
            trophyTypes[2] = BossDropAutoloader.AddBossTrophy("DivineGargooptuar");          
            trophyTypes[3] = BossDropAutoloader.AddBossTrophy("CrimulanGlopstrosity");          
            trophyTypes[4] = BossDropAutoloader.AddBossTrophy("StellarGeliath");          
            On_Main.UpdateAudio += FadeMusicOut;
            //On_Main.CheckMonoliths += DrawCordShapes;
            //nPCsToDrawCordOn = new List<NPC>();

            LoadAssets();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new ChromaticDropRule(), ModContent.ItemType<GoozmaAuricSoul>()));
            npcLoot.Add(ItemDropRule.ByCondition(new GoozmaDownedDropRule(), ModContent.ItemType<GoozmaLore>()));
            npcLoot.Add(ItemDropRule.ByCondition(new ZenithDropRule(), ModContent.ItemType<Goozmaga>()));

            if (Main.rand.NextBool(20))
                npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<TreasureBucket>()));
            else
                npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<TreasureTrunk>()));

            npcLoot.Add(ItemDropRule.Common(trophyTypes[0], 10));
            npcLoot.Add(ItemDropRule.FewFromOptions(1, 10, trophyTypes[1], trophyTypes[2], trophyTypes[3], trophyTypes[4]));

            npcLoot.Add(ItemDropRule.ByCondition(new MasterRevDropRule(), (relicType)));

            npcLoot.Add(ItemDropRule.ByCondition(new MasterRevDropRule(), ModContent.ItemType<ImperialGelato>(), 4));

            LeadingConditionRule classic = new LeadingConditionRule(new Conditions.NotExpert());

            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<SplendorJam>()));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ShogunHelm>(), 3));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ShogunChestplate>(), 3));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ShogunPants>(), 3));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<StickyHand>(), 3));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ChromaticMass>(), 1, 20, 30));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<PaladinPalanquin>(), 4));

            classic.OnSuccess(ItemDropRule.FewFromOptions(1, 7, ModContent.ItemType<EbonianMask>(), ModContent.ItemType<DivineMask>(), ModContent.ItemType<CrimulanMask>(), ModContent.ItemType<StellarMask>()));
            classic.OnSuccess(ItemDropRule.FewFromOptions(1, 1, ModContent.ItemType<Parasanguine>(), ModContent.ItemType<SludgeShaker>(), ModContent.ItemType<CrystalGauntlets>(), ModContent.ItemType<SlimeCane>(), ModContent.ItemType<CometKunai>()));
            npcLoot.Add(classic);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<SupremeRestorationPotion>();
        }

        public ref float Time => ref NPC.ai[0];
        public ref float Attack => ref NPC.ai[1];
        public ref float Phase => ref NPC.ai[2];
        public ref NPC ActiveSlime => ref Main.npc[(int)NPC.ai[3]];

        public NPCAimedTarget Target => NPC.ai[3] < 0 ? NPC.GetTargetData() : (ActiveSlime.GetTargetData().Invalid ? NPC.GetTargetData() : ActiveSlime.GetTargetData());

        public enum AttackList
        {
            Shimmering,
            SpawnSelf,
            SpawnSlime,
            BurstLightning,
            Absorption,
            DrillDash,
            FusionRay
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(currentSlime);

            for (int i = 0; i < nextAttack.Length; i++) 
                for (int j = 0; j < nextAttack[i].Count; j++)
                    writer.Write(nextAttack[i][j]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            currentSlime = reader.Read();

            for (int i = 0; i < nextAttack.Length; i++)
                for (int j = 0; j < nextAttack[i].Count; j++)
                    nextAttack[i][j] = reader.Read();
        }

        public override void OnSpawn(IEntitySource source)
        {
            NPC.ai[3] = -1;
            currentSlime = -1;

            //this sucks
            nextAttack = new List<int>[]
            {
                new List<int> { 0, 1, 2},
                new List<int> { 0, 1, 2},
                new List<int> { 0, 1, 2},
                new List<int> { 0, 1, 2}
            };

            SetPhase(-1);
            headScale = 0.9f;

            oldVel = new Vector2[NPCID.Sets.TrailCacheLength[Type]];
            oldTentacleVel = new Vector2[NPCID.Sets.TrailCacheLength[Type]];
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[Type]; i++)
            {
                NPC.oldPos[i] = NPC.position;
                NPC.oldRot[i] = NPC.rotation;
                oldVel[i] = NPC.velocity;
                oldTentacleVel[i] = NPC.velocity;
            }

            Main.newMusic = Music;
            for (int i = 0; i < Main.musicFade.Length; i++)
                Main.musicFade[i] = 0.1f;
            Main.musicFade[Main.newMusic] = 1f;

            //Particle crack = Particle.NewParticle(ModContent.GetInstance<CrackSpot>(), NPC.Center, Vector2.Zero, Color.Black, 36f);
            //crack.data = "GoozmaColor";

            SoundStyle roar = AssetDirectory.Sounds.Goozma.Awaken;
            SoundEngine.PlaySound(roar, NPC.Center);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (Main.getGoodWorld && Main.masterMode && RevengeanceMode)
                target.AddBuff(BuffID.VortexDebuff, 480);
        }

        private void ChangeWeather()
        {
            if (!Main.slimeRain)
                Main.StartSlimeRain(false);

            //Main.LocalPlayer.ManageSpecialBiomeVisuals("HuntOfTheOldGods:SlimeMonsoon", true, Main.LocalPlayer.Center);
        }

        private Vector2 saveTarget;
        private Vector2 eyePower;
        private Vector2[] oldVel;

        public override bool CheckDead()
        {
            if (NPC.life < 1)
            {
                switch (Phase)
                {
                    case 0:

                        SetPhase(1);
                        NPC.life = 1;
                        NPC.lifeMax = (int)(NPC.lifeMax * 0.3f);
                        NPC.dontTakeDamage = true;
                        if (!Main.expertMode && !Main.masterMode)
                            SetPhase(3);

                        break;

                    case -2:
                    case 2:

                        SetPhase(3);
                        NPC.life = 1;
                        NPC.dontTakeDamage = true;

                        break;
                }
            }
            return Phase > 3 || (Phase == 3 && Time >= 300);
        }


        private int currentSlime;
        private List<int>[] nextAttack;

        private static readonly float SpecialAttackWeight = 0.0f; //unable to choose, therefore saved for last
        public int GetSlimeAttack()
        {
            int gotten = -1;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (nextAttack[currentSlime] == null || nextAttack[currentSlime].Count == 0)
                    nextAttack[currentSlime] = new List<int> { 0, 1, 2 };

                WeightedRandom<int> weightedRandom = new WeightedRandom<int>(Main.rand);
                foreach (int j in nextAttack[currentSlime])
                    weightedRandom.Add(j, j == 2 ? SpecialAttackWeight : 1f);

                gotten = weightedRandom.Get();
                nextAttack[currentSlime].Remove(gotten);

                NPC.netUpdate = true;
            }

            return gotten;
        }

        public override void AI()
        {
            ChangeWeather();
            GoozmaResistances.DisablePointBlank();
            if (ModLoader.HasMod("CalamityMod"))
            {
                Main.player[Main.myPlayer].AddBuff(ModLoader.GetMod("CalamityMod").Find<ModBuff>("BossEffects").Type, 2);
            }
            bool noSlime = NPC.ai[3] < 0 || NPC.ai[3] >= Main.maxNPCs || ActiveSlime.ai[1] > 3 || !ActiveSlime.active;
            if (Phase == 0 && noSlime)
                SetAttack(AttackList.SpawnSlime);

            if (NPC.velocity.HasNaNs())
                NPC.velocity = Vector2.Zero;

            if (Target.Invalid)
                NPC.TargetClosestUpgraded();

            if (NPC.GetTargetData().Invalid && Phase != -5)
            {
                SetPhase(-5);
                NPC.dontTakeDamage = true;
                NPC.velocity = Vector2.Zero;
                return;
            }

            if (NPC.Distance(Target.Center) > 800 && Phase == 0 || Phase == 2)
                NPC.Center = Vector2.Lerp(NPC.Center, NPC.Center + NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * Math.Max(0, NPC.Distance(Target.Center) - 800), 0.01f);

            if (NPC.velocity.Length() < 50f)
            {
                if (Math.Abs(NPC.Center.X - Target.Center.X) > 20)
                    NPC.direction = NPC.Center.X > Target.Center.X ? -1 : 1;
            }
            else
                NPC.direction = NPC.velocity.X > 0 ? 1 : -1;

            NPC.damage = 0;

            if (!NPC.dontTakeDamage || Phase == 0)
            {
                NPC.damage = GetDamage(0);
                NPC.takenDamageMultiplier = 1f;

                if (!noSlime)
                    if (NPC.Distance(ActiveSlime.Center) < 400)
                        NPC.damage = 0;

                if (Phase <= 1 && Time < 60)
                    NPC.damage = 0;
            }

            //good luck religious passage
            //Matthew 25:31 When the Son of man shall come in his glory, and all the holy angels with him, then shall he sit upon the throne of his glory
            switch (Phase)
            {
                case -1:

                    SetAttack(AttackList.SpawnSelf);
                    NPC.direction = -1;
                    NPC.velocity.Y = -Utils.GetLerpValue(20, 40, Time, true);
                    if (Time > 60)
                    {
                        SetPhase(0);
                        NPC.dontTakeDamage = false;
                        headScale = 1f;
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        if (Main.rand.NextBool((int)(Time + 1)))
                        {
                            Vector2 velocity = Vector2.UnitY.RotatedBy(MathHelper.TwoPi / 3f * i).RotatedByRandom(1f);
                            velocity.Y -= 1f + Main.rand.NextFloat();
                            ParticleBehavior.NewParticle(ModContent.GetInstance<GooBurstParticleBehavior>(), Main.rand.NextVector2FromRectangle(NPC.Hitbox), velocity, new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).Value, 1f + Main.rand.NextFloat(1f));
                        }
                    }
                    if (Time % 3 == 0)
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2CircularEdge(1, 1), 10f, 6f, 10));

                    Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(10, 10), DustID.TintableDust, Main.rand.NextVector2CircularEdge(10, 10), 200, Color.Black, Main.rand.NextFloat(2f, 4f)).noGravity = true;

                    break;

                case 0:

                    if (Attack == (int)AttackList.SpawnSlime)
                    {
                        NPC.TargetClosestUpgraded();
                        NPC.direction = NPC.DirectionTo(NPC.GetTargetData().Center).X > 0 ? 1 : -1;
                        NPC.defense = 1000;
                        NPC.takenDamageMultiplier = 0.1f;

                        if (Time > 5 && Time < 45)
                        {
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(NPC.GetTargetData().Center) * Math.Max(NPC.Distance(NPC.GetTargetData().Center) - 150, 0) * 0.12f, 0.1f);
                            NPC.position += Main.rand.NextVector2Circular(6, 6);

                            for (int i = 0; i < 3; i++)
                            {
                                Vector2 inward = NPC.Center + Main.rand.NextVector2Circular(70, 70) + Main.rand.NextVector2CircularEdge(100 - Time, 100 - Time);
                                var hue = ParticleBehavior.NewParticle(ModContent.GetInstance<HueLightDustParticleBehavior>(), inward, inward.DirectionTo(NPC.Center) * Main.rand.NextFloat(3f), Color.White, 1f);
                                hue.Add(new ParticleData<float> { Value = NPC.localAI[0] });
                            }
                        }

                        if (Time > 42 && Time <= 50 && !(NPC.ai[3] < 0 || NPC.ai[3] >= Main.maxNPCs))
                            KillSlime(currentSlime);

                        if (Time == 50)
                        {
                            NPC.velocity *= -1f;
                            for (int i = 0; i < 45; i++)
                            {
                                Vector2 outward = NPC.Center + Main.rand.NextVector2Circular(10, 10);
                                var hue = ParticleBehavior.NewParticle(ModContent.GetInstance<HueLightDustParticleBehavior>(), outward, outward.DirectionFrom(NPC.Center) * Main.rand.NextFloat(3f, 10f), Color.White, 2f);
                                hue.Add(new ParticleData<float> { Value = NPC.localAI[0] });
                            }

                            //for (int i = 0; i < Main.rand.Next(3, 8); i++)
                            //{
                            //    NPC slime = NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y - 50, Main.rand.Next(SlimeUtils.SlimeIDs));
                            //    slime.velocity = new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(-7, -5));
                            //    if (Main.rand.NextBool(10))
                            //    {
                            //        NPC jellyfish = NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y - 50, Main.rand.Next(SlimeUtils.JellyfishIDs));
                            //        jellyfish.velocity = new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(-7, -5));
                            //    }
                            //}

                            if (Main.getGoodWorld)
                            {
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X - 100, (int)NPC.Center.Y, ModContent.NPCType<Goozmite>());
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + 100, (int)NPC.Center.Y, ModContent.NPCType<Goozmite>());
                            }

                            currentSlime = (currentSlime + 1) % 4;
                            int slimeAttack = GetSlimeAttack();

                            //for (int i = 0; i < nextAttack.Length; i++)
                            //    nextAttack[i] = nextAttack[i] % 3;

                            if (Main.zenithWorld)
                            {
                                currentSlime = Main.rand.Next(0, 4);
                                //nextAttack[currentSlime] = ;
                            }

                            //currentSlime = 2;
                            //slimeAttack = 2;

                            int[] slimeTypes = new int[]
                            {
                                ModContent.NPCType<EbonianBehemuck>(),
                                ModContent.NPCType<DivineGargooptuar>(),
                                ModContent.NPCType<CrimulanGlopstrosity>(),
                                ModContent.NPCType<StellarGeliath>()
                            };

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (NPC.ai[3] < 0 || NPC.ai[3] >= Main.maxNPCs || !ActiveSlime.active)
                                {
                                    NPC.ai[3] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y - 50, slimeTypes[currentSlime], ai0: -50, ai1: slimeAttack, ai2: NPC.whoAmI);
                                    ActiveSlime.velocity.Y -= 16;
                                }
                                else
                                {
                                    Vector2 pos = ActiveSlime.Bottom;
                                    ActiveSlime.active = false;
                                    NPC.ai[3] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)pos.X, (int)pos.Y, slimeTypes[currentSlime], ai0: -50, ai1: slimeAttack, ai2: NPC.whoAmI);
                                }

                                SoundStyle spawn = AssetDirectory.Sounds.Goozma.SpawnSlime;
                                SoundEngine.PlaySound(spawn, NPC.Center);
                            }
                            else if (Main.netMode == NetmodeID.MultiplayerClient)
                                NPC.netUpdate = true;
                        }

                        NPC.velocity *= 0.9f;

                        if (Time > 70)
                            SetAttack((int)Attack + 1, true);
                    }
                    else
                    {
                        NPC.defense = 100;

                        //attack depends on slime's attack, which depends on slime
                        switch (currentSlime)
                        {
                            //ebonian
                            case 0:
                                switch (ActiveSlime.ai[1])
                                {
                                    case 0:

                                        AresLockTo(Target.Center - new Vector2(0, 400) + Target.Velocity * new Vector2(5f, 2f));

                                        if (Time > 50)
                                        {
                                            SortedProjectileAttack(Target.Center - Target.Velocity * 10, SortedProjectileAttackTypes.EbonianBubbles);
                                            NPC.velocity.X *= 0.8f;
                                        }

                                        break;
                                    case 1:

                                        Fly();
                                        NPC.velocity *= 1.01f;
                                        //if (Time > 80)
                                        //{
                                        //    if (Time % 70 == 0)
                                        //        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center + Target.Velocity * 10).SafeNormalize(Vector2.Zero) * 10, ModContent.ProjectileType<PureSlimeball>(), GetDamage(2), 0);
                                           
                                        //    SortedProjectileAttack(Target.Center + Target.Velocity * 20 + Main.rand.NextVector2Circular(10, 10), SortedProjectileAttackTypes.EbonianTrifecta);
                                        //}

                                        break;

                                    case 2:

                                        AresLockTo(Target.Center - Vector2.UnitY * 270);

                                        break;
                                }
                                break;

                            //divine
                            case 1:
                                switch (ActiveSlime.ai[1])
                                {
                                    case 0:

                                        Vector2 outerRing = Target.Center.DirectionTo(ActiveSlime.Center) * 800;
                                        FlyTo(ActiveSlime.Center - outerRing);
                                        NPC.velocity *= 0.6f;

                                        break;

                                    case 1:

                                        AresLockTo(Target.Center + new Vector2(0, 360) + Target.Velocity * new Vector2(4f, 3f));

                                        if (Time > 50)
                                        {
                                            SortedProjectileAttack(Target.Center - Target.Velocity * 10, SortedProjectileAttackTypes.CrystalStorm);
                                            NPC.velocity.X *= 0.8f;
                                        }

                                        break;

                                    case 2:

                                        Orbit(500, new Vector2(600, 100));
                                        SortedProjectileAttack(Target.Center, SortedProjectileAttackTypes.PixieBallDisruption);

                                        break;
                                }
                                break;

                            //crimulan
                            case 2:
                                switch (ActiveSlime.ai[1])
                                {
                                    case 0:

                                        //if (Time > 70 && Time < 230)
                                        //{
                                        //    FlyTo(Target.Center + new Vector2(700 * (ActiveSlime.Center.X > Target.Center.X ? -1 : 1) * Utils.GetLerpValue(0, 100, Time, true), (float)Math.Sin(Time % 110 * MathHelper.TwoPi / 110f) * 130));

                                        //    if ((Main.rand.NextBool(15) || Time % 30 == 0) && Time > 90)
                                        //        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Main.rand.NextVector2Circular(15, 15), ModContent.ProjectileType<PureSlimeball>(), GetDamage(2), 0);

                                        //}
                                        //else
                                        //    Fly();

                                        Orbit(1000, new Vector2(500, 0));

                                        break;
                                    case 1:

                                        Fly();
                                        NPC.velocity *= 0.9f;

                                        break;
                                    case 2:

                                        Orbit(400, new Vector2(700, 0));

                                        if (Time > 40)
                                            SortedProjectileAttack(Target.Center, SortedProjectileAttackTypes.CrimulanHop);


                                        NPC.velocity *= 0.9f;

                                        break;
                                }
                                break;

                            //stellar
                            case 3:
                                switch (ActiveSlime.ai[1])
                                {
                                    case 0:

                                        if (Time < 5)
                                            saveTarget = Target.Center;
                                        else
                                            saveTarget = Vector2.Lerp(saveTarget, Target.Center, 0.05f);

                                        if (Time >= 70 && Time < 550)
                                            SortedProjectileAttack(saveTarget, SortedProjectileAttackTypes.StellarDisruption);

                                        FlyTo(saveTarget);
                                        NPC.velocity *= 0.75f * Utils.GetLerpValue(-50, 60, Time, true);
                                        NPC.damage = 0;

                                        break;

                                    case 1:
                                        Fly();

                                        break;

                                    case 2:

                                        Vector2 outerRing = Target.Center.DirectionTo(ActiveSlime.Center) * 1500;
                                        FlyTo(ActiveSlime.Center - outerRing);
                                        NPC.velocity *= 0.6f;
                                        if (Time < 590)
                                            SortedProjectileAttack(ActiveSlime.Center, SortedProjectileAttackTypes.StellarTaunt);

                                        break;
                                }
                                break;
                        }
                    }

                    if (Main.expertMode || Main.masterMode)
                    {
                        if (NPC.life <= NPC.lifeMax * 0.33f)
                        {
                            SetPhase((int)Phase + 1);
                            NPC.dontTakeDamage = true;
                            NPC.life = (int)(NPC.lifeMax * 0.33f);
                        }    
                    }    

                    eyePower = Vector2.One * 1.5f;

                    break;

                case 1:

                    NPC.velocity = Vector2.Zero;
                    if (NPC.ai[3] > -1 && NPC.ai[3] <= Main.maxNPCs)
                        if (ActiveSlime.active)
                            ActiveSlime.active = false;

                    drawOffset += Main.rand.NextVector2Circular(10, 10) * Utils.GetLerpValue(0, 300, Time, true) * Utils.GetLerpValue(302, 300, Time, true);
                    NPC.dontTakeDamage = true;
                    if (NPC.life < NPC.lifeMax * 0.33f)
                        NPC.life = (int)(NPC.lifeMax * 0.33f);
                    //NPC.life = 1 + (int)((float)Math.Pow(Utils.GetLerpValue(300, 530, Time, true), 3) * (NPC.lifeMax - 1));
                    eyePower = Vector2.SmoothStep(Vector2.One * 1.5f, new Vector2(5f, 3.6f), Utils.GetLerpValue(300, 500, Time, true));
                    
                    if (Time < 15)
                        KillSlime(currentSlime);

                    if (Time > 45 && Time < 53)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(10, 10), DustID.TintableDust, Main.rand.NextVector2CircularEdge(20, 20), 200, Color.Black, Main.rand.NextFloat(2, 4)).noGravity = true;
                            ParticleBehavior.NewParticle(ModContent.GetInstance<GoozBombChunkParticleBehavior>(), NPC.Center + Main.rand.NextVector2Circular(10, 10), Main.rand.NextVector2Circular(20, 20) - Vector2.UnitY * 8f, Color.White, 0.5f + Main.rand.NextFloat(1.5f));
                        }
                    }

                    if (Time < 50)
                    {
                        NPC.scale = (float)Math.Sqrt(Utils.GetLerpValue(50, 10, Time, true));
                        NPC.Center += Main.rand.NextVector2Circular(5, 5);
                        for (int i = 0; i < Main.rand.Next(1, 4); i++)
                        {
                            var gelBit = ParticleBehavior.NewParticle(ModContent.GetInstance<GoozGelBitParticleBehavior>(), NPC.Center + Main.rand.NextVector2Circular(30, 40), Main.rand.NextVector2CircularEdge(10, 10) + Main.rand.NextVector2Circular(20, 20), Color.White, 1f + Main.rand.NextFloat());
                            gelBit.Add(new ParticleData<int> { Value = (int)(300 - Time + Main.rand.Next(55)) });
                        }
                    }
                    else
                        NPC.scale = MathHelper.Lerp(NPC.scale, MathHelper.SmoothStep(0f, 1f, Utils.GetLerpValue(400, 500, Time, true)), 0.2f);

                    //if (Time > 400 && Time % 12 == 0)
                    //{
                    //    Particle crack = Particle.NewParticle(ModContent.GetInstance<CrackSpot>(), NPC.Center, Vector2.Zero, Color.Black, 40f);
                    //    crack.data = "GoozmaBlack";
                    //    crack.behindEntities = true;
                    //}

                    if (Time < 70 || Time > 400)
                    {
                        Vector2 gooVelocity = Main.rand.NextVector2Circular(2, 3);
                        var goo = ParticleBehavior.NewParticle(ModContent.GetInstance<GooBurstParticleBehavior>(), NPC.Center + Main.rand.NextVector2Circular(50, 80) * NPC.scale, gooVelocity, Color.White, 0.1f + Main.rand.NextFloat(1.5f));
                        goo.Add(new ParticleData<float> { Value = NPC.localAI[0] + Main.rand.NextFloat(0.2f, 0.5f) });
                    }

                    if (Time == 1)
                    {
                        //Main.instance.CameraModifiers.Add(new SlowPan(NPC.Bottom, 300, 200, 30, "GoozmaEnrage"));
                        SoundStyle boomSound = AssetDirectory.Sounds.Goozma.Explode;
                        SoundEngine.PlaySound(boomSound, NPC.Center);
                    }
                                        
                    if (Time == 290)
                    {
                        SoundStyle eyeSound = AssetDirectory.Sounds.Goozma.EyeAppear;
                        SoundEngine.PlaySound(eyeSound, NPC.Center);
                    }

                    if (Time == 290)
                    {
                        SoundStyle roar = AssetDirectory.Sounds.Goozma.Reform;
                        SoundEngine.PlaySound(roar, NPC.Center);
                    }

                    if (Time > 570)
                    {
                        SoundStyle roar = AssetDirectory.Sounds.Goozma.Reawaken;
                        SoundEngine.PlaySound(roar, NPC.Center);

                        Music = Music2;
                        Main.newMusic = Music;
                        Main.musicFade[Main.curMusic] = 0f;
                        Main.musicFade[Main.newMusic] = 1f;

                            SetPhase((int)Phase + 1);
                        NPC.dontTakeDamage = false;
                        NPC.defense += 20;
                    }

                    break;

                case 2:

                    switch (Attack)
                    {
                        case (int)AttackList.BurstLightning:

                            Orbit((int)(600 + ((float)NPC.life / NPC.lifeMax * 100)), new Vector2(0, -450));
                            SortedProjectileAttack(Target.Center + Target.Velocity * 1.5f, SortedProjectileAttackTypes.BurstLightning);

                            if (Main.rand.NextBool(30))
                                ParticleBehavior.NewParticle(ModContent.GetInstance<GoozBombChunkParticleBehavior>(), NPC.Bottom + Main.rand.NextVector2Circular(10, 10), Main.rand.NextVector2Circular(5, 2) - Vector2.UnitY * 5f + NPC.velocity * 0.1f, Color.White, 0.1f + Main.rand.NextFloat(1.5f));
                           
                            if (Time > 640)
                            {
                                SetTime(-60);
                                SetAttack(AttackList.DrillDash);
                            }

                            break;

                        case (int)AttackList.DrillDash:

                            int dashCount = (int)DifficultyBasedValue(4, 5, 6, 7);
                            int dashTime = (int)DifficultyBasedValue(110, 100, 90, 80);

                            if (Time >= 0)
                            {
                                if (Time <= dashTime * dashCount)
                                {
                                    if (Time % dashTime == 21)
                                    {
                                        SoundStyle roar = AssetDirectory.Sounds.Goozma.Dash;
                                        SoundEngine.PlaySound(roar, NPC.Center);
                                    }

                                    if (Time % dashTime <= 13)
                                    {
                                        NPC.Center = Vector2.Lerp(NPC.Center, NPC.Center + NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * (NPC.Distance(Target.Center) - 300) * 0.2f, 0.6f);
                                        NPC.velocity = NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero);

                                        if (Time % dashTime == 13)
                                            NPC.velocity -= NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * 20f;

                                    }

                                    if (Time % dashTime > 25 && Time % dashTime < 74)
                                    {
                                        rotate = true;
                                        NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.ToRotation() + MathHelper.PiOver2, 0.33f * Utils.GetLerpValue(20, 70, Time % dashTime, true));

                                        for (int i = 0; i < 8; i++)
                                        {
                                            Vector2 position = Vector2.Lerp(NPC.position, NPC.oldPos[0], i / 24f) + NPC.Size * 0.5f;

                                            var hueTop = ParticleBehavior.NewParticle(ModContent.GetInstance<HueLightDustParticleBehavior>(), position + Main.rand.NextVector2Circular(8, 8) + NPC.velocity, -NPC.velocity.RotatedBy((float)Math.Sin((Time - (i / 8f)) * 0.23f) * 0.8f), Color.White, 2f);
                                            hueTop.Add(new ParticleData<float> { Value = NPC.localAI[0] });

                                            var hueBot = ParticleBehavior.NewParticle(ModContent.GetInstance<HueLightDustParticleBehavior>(), position + Main.rand.NextVector2Circular(8, 8) + NPC.velocity, -NPC.velocity.RotatedBy(-(float)Math.Sin((Time - (i / 8f)) * 0.23f) * 0.8f), Color.White, 2f);
                                            hueBot.Add(new ParticleData<float> { Value = NPC.localAI[0] });
                                        }

                                        NPC.velocity += NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * 0.5f;
                                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * 60f, 0.03f);
                                    }
                                    else
                                        NPC.velocity *= 0.7f;

                                    SortedProjectileAttack(Target.Center, SortedProjectileAttackTypes.DrillDash);
                                }
                            }
                            else
                                Fly();

                            if (Time > dashTime * dashCount + 20 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                SetTime(-30);
                                SetAttack(RevengeanceMode ? (int)AttackList.Absorption : (int)AttackList.FusionRay);
                            }

                            NPC.damage = GetDamage(6, 0.9f + Time % dashTime / dashTime * 0.2f);

                            break;

                        case (int)AttackList.Absorption:

                            NPC.dontTakeDamage = true;
                            NPC.damage = 0;

                            int goozmiteCount = 6;
                            int killTime = 850;
                            int waitTimeMultiplier = 7;
                            int waitTime = goozmiteCount * waitTimeMultiplier;

                            goozmiteCount += (int)DifficultyBasedValue(0, 3, 6, 9);
                            killTime -= (int)DifficultyBasedValue(0, 50, 100, 150);

                            if (Time < 10)
                            {
                                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(NPC.GetTargetData().Center) * Math.Max(NPC.Distance(NPC.GetTargetData().Center) - 150, 0) * 0.12f, 0.1f);
                                NPC.velocity *= 0.9f;
                                NPC.position += Main.rand.NextVector2Circular(3, 3);
                            }

                            else
                            {
                                Fly();
                                NPC.velocity *= 0.5f;
                            }

                            if (Time >= 0 && Time < goozmiteCount * 5f)
                            {
                                NPC.position += Main.rand.NextVector2Circular(8, 8);

                                if (Time % 5 == 1)
                                {
                                    Vector2 velocity = new Vector2(1f, 0).RotatedBy(MathHelper.TwoPi * Utils.GetLerpValue(0, goozmiteCount * 5f, Time, true));
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        SoundStyle spawn = AssetDirectory.Sounds.Goozma.SpawnSlime;
                                        SoundEngine.PlaySound(spawn, NPC.Center);

                                        float radius = 100;
                                        NPC goozmite = NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)(NPC.Center.X + velocity.X * radius), (int)(NPC.Center.Y + velocity.Y * radius) + 40, ModContent.NPCType<Goozmite>(), ai1: killTime, ai2: NPC.whoAmI);
                                        goozmite.velocity = velocity * 60f;   
                                        goozmite.ai[1] = killTime - Main.npc.Count(n => n.active && n.type == ModContent.NPCType<Goozmite>()) * waitTimeMultiplier;
                                        goozmite.localAI[0] = NPC.localAI[0] + 60f * Utils.GetLerpValue(0, goozmiteCount * 5f, Time, true);
                                    }
                                    else if (Main.netMode == NetmodeID.MultiplayerClient)
                                        NPC.netUpdate = true;

                                }
                            }

                            if (Time > waitTime && Time < waitTime + killTime)
                                SortedProjectileAttack(Target.Center + Target.Velocity * 10f, SortedProjectileAttackTypes.Absorption);

                            if (Time > waitTime + 20)
                            {
                                if (!Main.npc.Any(n => n.active && n.type == ModContent.NPCType<Goozmite>()) && Time < waitTime + killTime)
                                    SetTime(waitTime + killTime);
                            }

                            if (Time > waitTime + killTime + 30)
                            {
                                NPC.dontTakeDamage = false;

                                if (NPC.life > (NPC.lifeMax * 0.15f))
                                    SetAttack(AttackList.FusionRay, true);
                                else
                                    SetAttack(AttackList.BurstLightning, true);
                            }

                            break;

                        case (int)AttackList.FusionRay:

                            SortedProjectileAttack(Target.Center, SortedProjectileAttackTypes.FusionRay);

                            Fly();

                            NPC.damage = 0;

                            if (Time > 1400)
                                SetAttack(AttackList.BurstLightning, true);

                            break;

                        default:

                            SetAttack(AttackList.BurstLightning, true);

                            break;
                    }

                    if (NPC.life < NPC.lifeMax * 0.1f)
                    {
                        NPC.defense = 200;
                        NPC.takenDamageMultiplier = 0.9f;

                        if (Attack != (int)AttackList.FusionRay)
                            SetPhase(-2);
                    }

                    break;

                case -2:

                    NPC.defense = 175;

                    Fly();
                    NPC.velocity *= 1.0002f;
                    int randCount = Main.rand.Next(60, 80);
                    if (Time % randCount > 5 && Time % randCount < 10)
                    {
                        SetTime(10);

                        SoundStyle spawn = AssetDirectory.Sounds.Goozma.SpawnSlime;
                        SoundEngine.PlaySound(spawn, NPC.Center);

                        int killTime = 180;
                        Vector2 velocity = NPC.DirectionFrom(Target.Center).SafeNormalize(Vector2.Zero).RotatedByRandom(0.1f * NPC.direction);
                        NPC goozmite = NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 40, ModContent.NPCType<Goozmite>(), ai1: killTime, ai2: NPC.whoAmI);
                        goozmite.velocity = velocity * Main.rand.Next(20, 70);
                        goozmite.ai[1] = killTime;
                        goozmite.localAI[0] = NPC.localAI[0] + 20f;
                        goozmite.lifeMax = (int)(goozmite.lifeMax * 0.33f);
                        goozmite.life = goozmite.lifeMax;
                    }

                    SortedProjectileAttack(Target.Center, SortedProjectileAttackTypes.FusionRay);

                    break;

                case 3:

                    NPC.velocity.X = 0;
                    NPC.velocity.Y = Utils.GetLerpValue(10, 300, Time, true) * -1f;
                    NPC.Center += Main.rand.NextVector2Circular(4, 4) * Utils.GetLerpValue(10, 300, Time, true);

                    NPC.dontTakeDamage = true;
                    NPC.life = 1;

                    if (Time % 3 == 0)
                    {
                        foreach (Projectile projectile in Main.projectile.Where(n => n.active && n.ModProjectile is IDieWithGoozma))
                            projectile.Kill();
                    }

                    if (Time == 2)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GoozmaDeathGodrays>(), 0, 0, ai1: NPC.whoAmI);

                        SoundStyle deathRoar = AssetDirectory.Sounds.Goozma.DeathBuildup;
                        SoundEngine.PlaySound(deathRoar, NPC.Center);
                    }

                    if (Main.rand.NextBool(2 + (int)(60 - Time / 5f)))
                        for (int i =0; i < Main.rand.Next(1, 4); i++)
                            ParticleBehavior.NewParticle(ModContent.GetInstance<GoozBombChunkParticleBehavior>(), NPC.Center + Main.rand.NextVector2Circular(10, 10), Main.rand.NextVector2Circular(20, 20) - Vector2.UnitY * 10f, Color.White, 0.5f + Main.rand.NextFloat(1.5f));

                    if (Main.rand.NextBool(1 + (int)(65 - Time / 4.66f)) || Time > 250)
                    {
                        Vector2 deathGooVelocity = Main.rand.NextVector2CircularEdge(1, 2);
                        var deathGoo = ParticleBehavior.NewParticle(ModContent.GetInstance<GooBurstParticleBehavior>(), NPC.Center + deathGooVelocity * Main.rand.NextFloat(4, 16) * NPC.scale, deathGooVelocity, Color.White, 0.5f + Main.rand.NextFloat());
                        deathGoo.Add(new ParticleData<float> { Value = NPC.localAI[0] + Main.rand.NextFloat(0.2f, 0.5f) });
                    }

                    if (Time % 3 == 0)
                    {
                        Color glowColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(NPC.localAI[0] + Main.rand.NextFloat(0.1f));
                        Vector2 off = Main.rand.NextVector2Circular(150, 220);
                        float sparkleScale = Utils.GetLerpValue(200, 0, off.Length() + Main.rand.Next(-10, 10));
                        ParticleBehavior.NewParticle(ModContent.GetInstance<CrossSparkleParticleBehavior>(), NPC.Center + off, Vector2.Zero, glowColor, sparkleScale * 2f);
                    }

                    if (Time > 250)
                        MoonlordDeathDrama.RequestLight(Utils.GetLerpValue(250, 280, Time, true), NPC.Center);

                    if (Time > 290)
                        headScale += 0.15f;

                    if (Time > 300)
                    {
                        //Particle kill = Particle.NewParticle(ModContent.GetInstance<CrackSpot>(), NPC.Center, Vector2.Zero, Color.Black, 36f);
                        //kill.data = "GoozmaBlack";
                        Main.musicFade[Main.curMusic] = 0f;
                        Main.curMusic = -1;

                        SlimeMonsoonBackground.forceStrength = 0f;
                        NPC.justHit = true;
                        NPC.life = 0;
                        NPC.checkDead();
                    }

                    if (!Main.expertMode && !Main.masterMode)
                    {
                        if (NPC.ai[3] > -1 && NPC.ai[3] <= Main.maxNPCs)
                            if (ActiveSlime.active)
                                ActiveSlime.active = false;

                        if (Time < 15)
                            KillSlime(currentSlime);
                    }

                    break;

                case -5:

                    NPC.velocity *= 0.8f;
                    NPC.velocity.Y -= (-31 - Time) * 0.025f;
                    NPC.scale *= 0.9999f;
                    NPC.dontTakeDamage = true;

                    if (Time > 0)
                        SetTime(0);

                    if (Time == -1)
                    {
                        if (NPC.ai[3] > -1 && NPC.ai[3] <= Main.maxNPCs)
                            if (ActiveSlime.active)
                                ActiveSlime.active = false;
                    }

                    if (Time > -15)
                        KillSlime(currentSlime);

                    if (Time < -30)
                        NPC.EncourageDespawn(30);

                    if (Time < -300)
                        NPC.active = false;

                    if (Main.rand.NextBool(3))
                    {
                        Vector2 deathGooVelocity = Main.rand.NextVector2CircularEdge(2, 3);
                        var deathGoo = ParticleBehavior.NewParticle(ModContent.GetInstance<GooBurstParticleBehavior>(), NPC.Center + deathGooVelocity * Main.rand.NextFloat(4, 16) * NPC.scale, deathGooVelocity, Color.White, 0.75f + Main.rand.NextFloat());
                        deathGoo.Add(new ParticleData<float> { Value = NPC.localAI[0] + Main.rand.NextFloat(0.2f, 0.5f) });
                    }

                    break;

                case -21:

                    Attack = (int)AttackList.Shimmering;

                    NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.UnitY * -8, 0.2f) * Utils.GetLerpValue(60, 20, Time, true);

                    NPC.Center += Main.rand.NextVector2Circular(15, 15) * Utils.GetLerpValue(0, 100, Time, true);
                    
                    if (Time < 15)
                        KillSlime(currentSlime);

                    if (Time > 80)
                    {
                        NPC.active = false;
                        foreach (NPC left in Main.npc.Where(n => !n.active))
                        {
                            int type = Utils.SelectRandom(Main.rand,
                                NPCID.BlueSlime,
                                NPCID.BlueSlime,
                                NPCID.BlueSlime,
                                NPCID.BlueSlime,
                                NPCID.GreenSlime,
                                NPCID.GreenSlime,
                                NPCID.GreenSlime,
                                NPCID.GreenSlime,
                                NPCID.PurpleSlime,
                                NPCID.PurpleSlime,
                                NPCID.PurpleSlime,
                                NPCID.RedSlime,
                                NPCID.RedSlime,
                                NPCID.YellowSlime,
                                NPCID.BlackSlime);
                            NPC npc = NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, type);
                            npc.velocity = Main.rand.NextVector2Circular(20, 20);
                        }
                    }

                    break;

                case -22:

                    NPC.dontTakeDamage = false;
                    NPC.velocity *= 0.5f;

                    if (Main.mouseRight)
                        NPC.velocity += NPC.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero) * NPC.Distance(Main.MouseWorld) * 0.1f;

                    if (NPC.velocity.Length() > 4)
                    {
                        rotate = true;
                        NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.ToRotation() + MathHelper.PiOver2, 0.4f);

                        //for (int i = 0; i < 5; i++)
                        //{
                        //    Color glowColor = new GradientColor(SlimeUtils.GoozColorArray, 0.2f, 0.2f).ValueAt(NPC.localAI[0]);
                        //    glowColor.A /= 3;
                        //    Particle smoke = Particle.NewParticle(ModContent.GetInstance<CosmicSmoke>(), NPC.Center + Main.rand.NextVector2Circular(100, 100), -NPC.velocity * Main.rand.NextFloat(), glowColor, 1f + Main.rand.NextFloat());
                        //}
                    }
                    else if (Main.rand.NextBool(20))
                    {
                        Vector2 deathGooVelocity = Main.rand.NextVector2CircularEdge(1, 2);
                        var deathGoo = ParticleBehavior.NewParticle(ModContent.GetInstance<GooBurstParticleBehavior>(), NPC.Center + deathGooVelocity * Main.rand.NextFloat(4, 16) * NPC.scale, deathGooVelocity, Color.White, 0.75f + Main.rand.NextFloat());
                        deathGoo.Add(new ParticleData<float> { Value = NPC.localAI[0] + Main.rand.NextFloat(0.2f, 0.5f) });
                    }

                    break;

                default:

                    SetPhase(-5);

                    break;
            };

            if (Phase >= 2)
                SlimeMonsoonBackground.additionalLightningChance = -53;

            HandleLoopedSounds();

            if (Phase != -5)
                Time++;
            else
                Time--;

            if (!Main.dedServ)
            {
                NPC.localAI[0]++;
                NPC.localAI[1] -= NPC.velocity.X * 0.001f + NPC.direction * 0.02f;

                for (int i = NPCID.Sets.TrailCacheLength[Type] - 1; i > 0; i--)
                {
                    NPC.oldPos[i] = Vector2.Lerp(NPC.oldPos[i], NPC.oldPos[i - 1], 0.37f + (float)Math.Sin(NPC.localAI[0] * 0.33f - i * 2f) * 0.25f);
                    NPC.oldRot[i] = MathHelper.Lerp(NPC.oldRot[i], NPC.oldRot[i - 1], 0.37f + (float)Math.Sin(NPC.localAI[0] * 0.33f - i * 2f) * 0.25f);
                    oldVel[i] = Vector2.Lerp(oldVel[i], oldVel[i - 1], 0.25f);
                }
                NPC.oldPos[0] = Vector2.Lerp(NPC.oldPos[0], NPC.position, 0.37f + (float)Math.Sin(NPC.localAI[0] * 0.33f) * 0.25f);
                NPC.oldRot[0] = MathHelper.Lerp(NPC.oldRot[0], NPC.rotation, 0.37f + (float)Math.Sin(NPC.localAI[0] * 0.33f) * 0.25f);
                oldVel[0] = Vector2.Lerp(oldVel[0], drawVelocity, 0.25f);

                //for (int i = NPCID.Sets.TrailCacheLength[Type] - 1; i > 0; i--)
                //{
                //    NPC.oldPos[i] = Vector2.Lerp(NPC.oldPos[i], NPC.oldPos[i - 1], 0.3f);
                //    NPC.oldRot[i] = MathHelper.Lerp(NPC.oldRot[i], NPC.oldRot[i - 1], 0.3f);
                //    oldVel[i] = Vector2.Lerp(oldVel[i], oldVel[i - 1], 0.3f);
                //}
                //NPC.oldPos[0] = Vector2.Lerp(NPC.oldPos[0], NPC.position, 0.3f);
                //NPC.oldRot[0] = MathHelper.Lerp(NPC.oldRot[0], NPC.rotation, 0.3f);
                //oldVel[0] = Vector2.Lerp(oldVel[0], NPC.velocity, 0.3f);

                if (Main.rand.NextBool())
                {
                    Dust dust = Dust.NewDustDirect(NPC.Center - new Vector2(50), 100, 160, DustID.TintableDust, Main.rand.NextFloat(-1f, 1f), -4f, 230, Color.Black, 2f + Main.rand.NextFloat());
                    dust.noGravity = true;
                }
                if (Main.rand.NextBool(8))
                {
                    var hue = ParticleBehavior.NewParticle(ModContent.GetInstance<HueLightDustParticleBehavior>(), NPC.Center + Main.rand.NextVector2Circular(60, 80), Main.rand.NextVector2Circular(1, 1) - Vector2.UnitY * 3f, Color.White, 1f);
                    hue.Add(new ParticleData<float> { Value = NPC.localAI[0] });
                }
            }

            //shimmer
            if (Phase != -21 && Phase != -1 && Main.tile[NPC.Center.ToTileCoordinates()].LiquidType == LiquidID.Shimmer)
            {
                Phase = -21;
                Time = 0;
                for (int i = 0; i < 4; i++)
                    KillSlime(currentSlime);
                ActiveSlime.active = false;
            }

            if (hitTimer > 0)
                hitTimer--;
        }

        private void KillSlime(int index)
        {
            switch (index)
            {
                case 0:

                    for (int i = 0; i < Main.rand.Next(4, 9); i++)
                    {
                        Vector2 velocity = Main.rand.NextVector2Circular(8, 1) - Vector2.UnitY * Main.rand.NextFloat(7f, 16f);
                        Vector2 position = ActiveSlime.Center + Main.rand.NextVector2Circular(1, 50) + new Vector2(velocity.X * 12f, 32f);
                        ParticleBehavior.NewParticle(ModContent.GetInstance<EbonBombChunkParticleBehavior>(), position, velocity + NPC.velocity, Color.White, 0.1f + Main.rand.NextFloat(2f));
                    }

                    break;

                case 1:

                    for (int i = 0; i < Main.rand.Next(4, 9); i++)
                    {
                        Vector2 velocity = Main.rand.NextVector2Circular(8, 1) - Vector2.UnitY * Main.rand.NextFloat(7f, 16f);
                        Vector2 position = ActiveSlime.Center + Main.rand.NextVector2Circular(1, 50) + new Vector2(velocity.X * 12f, 32f);
                        ParticleBehavior.NewParticle(ModContent.GetInstance<HolyBombChunkParticleBehavior>(), position, velocity, Color.White, 0.1f + Main.rand.NextFloat(2f));
                    }

                    break;

                case 2:

                    for (int i = 0; i < Main.rand.Next(4, 9); i++)
                    {
                        Vector2 velocity = Main.rand.NextVector2Circular(8, 1) - Vector2.UnitY * Main.rand.NextFloat(7f, 18f);
                        Vector2 position = ActiveSlime.Center + Main.rand.NextVector2Circular(1, 50) + new Vector2(velocity.X * 16f, 32f);
                        ParticleBehavior.NewParticle(ModContent.GetInstance<CrimBombChunkParticleBehavior>(), position, velocity, Color.White, 0.1f + Main.rand.NextFloat(2f));
                    }

                    break;

                case 3:

                    for (int i = 0; i < Main.rand.Next(4, 9); i++)
                    {
                        Vector2 velocity = Main.rand.NextVector2Circular(8, 1) - Vector2.UnitY * Main.rand.NextFloat(7f, 16f);
                        Vector2 position = ActiveSlime.Center + Main.rand.NextVector2Circular(1, 50) + new Vector2(velocity.X * 12f, 32f);
                        ParticleBehavior.NewParticle(ModContent.GetInstance<StarBombChunkParticleBehavior>(), position, velocity, Color.White, 0.1f + Main.rand.NextFloat(2f));
                    }

                    break;
            }
        }

        private int hitTimer;

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (hitTimer <= 0)
            {
                hitTimer += Main.rand.Next(10, 18);
                SoundStyle hurt = AssetDirectory.Sounds.Goozma.Hurt;
                SoundEngine.PlaySound(hurt, NPC.Center);
            }

            if (Main.rand.NextBool(3))
                for (int i = 0; i < Main.rand.Next(6); i++)
                    ParticleBehavior.NewParticle(ModContent.GetInstance<GoozBombChunkParticleBehavior>(), NPC.Center + Main.rand.NextVector2Circular(30, 30), NPC.DirectionFrom(player.Center).RotatedByRandom(0.2f) * Main.rand.Next(4, 10), Color.White, 0.5f + Main.rand.NextFloat());
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (hitTimer <= 0)
            {
                hitTimer += Main.rand.Next(10, 18);
                SoundStyle hurt = AssetDirectory.Sounds.Goozma.Hurt;
                SoundEngine.PlaySound(hurt, NPC.Center);
            }

            if (Main.rand.NextBool(3))
                for (int i = 0; i < Main.rand.Next(6); i++)
                    ParticleBehavior.NewParticle(ModContent.GetInstance<GoozBombChunkParticleBehavior>(), NPC.Center + Main.rand.NextVector2Circular(30, 30), NPC.DirectionFrom(projectile.Center).RotatedByRandom(0.2f) * Main.rand.Next(4, 10), Color.White, 0.5f + Main.rand.NextFloat());
        }

        public override void OnKill()
        {
            ModContent.GetInstance<BossDownedSystem>().GoozmaDowned = true;

            Main.StopSlimeRain();

            Main.windSpeedTarget = 0f;
            Main.windSpeedCurrent = MathHelper.Lerp(Main.windSpeedCurrent, 0, 0.7f);
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);

            for (int i = 0; i < Main.musicFade.Length; i++)
                Main.musicFade[i] = 0.2f;

            SoundEngine.PlaySound(AssetDirectory.Sounds.Goozma.Pop, NPC.Center);
            SoundEngine.PlaySound(AssetDirectory.Sounds.Goozma.Explode.WithPitchOffset(0.2f).WithVolumeScale(0.9f), NPC.Center);

            for (int i = 0; i < 200; i++)
                ParticleBehavior.NewParticle(ModContent.GetInstance<GoozBombChunkParticleBehavior>(), NPC.Center + Main.rand.NextVector2Circular(20, 20), Main.rand.NextVector2Circular(30, 30) - Vector2.UnitY * 15f, Color.White, 0.1f + Main.rand.NextFloat(2f));
        }

        private void Fly()
        {
            if (NPC.Distance(Target.Center) < 300)
                NPC.velocity *= 0.93f;

            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * Math.Max(0, NPC.Distance(Target.Center) - 300) * 0.04f, 0.02f);
        }        
        
        private void FlyTo(Vector2 target)
        {
            if (NPC.Distance(target) < 20)
                NPC.velocity *= 0.9f;

            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(target).SafeNormalize(Vector2.Zero) * Math.Max(1, NPC.Distance(target) * 0.2f), 0.1f);
        }        
        
        private void AresLockTo(Vector2 target)
        {
            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(target).SafeNormalize(Vector2.Zero) * Math.Max(1, NPC.Distance(target) * 0.4f), 0.27f);
        }

        //private void Dash(int dashCD)
        //{
        //    if (NPC.Center.Y < Target.Center.Y + 90 && NPC.Center.Y > Target.Center.Y - 90)
        //    {
        //        Time = 0;
        //        NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, (NPC.Center.Y - Target.Center.Y) * 0.2f, 0.1f);
        //        Fly();
        //    }

        //        float power = 0.05f;
        //    if (Time > dashCD - 6)
        //        power = 0.2f;
        //    else if (Time > dashCD - 15)
        //        NPC.velocity *= 0.9f;

        //    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * Math.Clamp(NPC.Distance(Target.Center) * power * 1.5f, 20, 100), power > 0.05f ? 0.1f : 0.03f);

        //    if (Time > dashCD)
        //        Time = 0;
        //}
        
        private void DashTo(Vector2 targetPos, int time)
        {
            float power = 0.05f;
            if (Time > time - 6)
                power = 0.2f;
            else if (Time > time - 15)
                NPC.velocity *= 0.9f;

            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero) * Math.Clamp(NPC.Distance(targetPos) * power * 1.5f, 20, 100), power > 0.05f ? 0.1f : 0.03f);

            if (Time > time)
                SetTime(0);
        }

        private int randDirection;

        private void Orbit(int frequency, Vector2 startingVector)
        {
            Vector2 target = Target.Center + startingVector.RotatedBy(Time % frequency / (float)frequency * MathHelper.TwoPi);

            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(target).SafeNormalize(Vector2.Zero) * NPC.Distance(target) * 0.1f, 0.1f);
        }

        private enum SortedProjectileAttackTypes
        {
            Default,
            EbonianBubbles,
            EbonianTrifecta,
            CrystalStorm,
            PixieBallDisruption,
            CrimulanSlam,
            CrimulanHop,
            StellarDisruption,
            StellarTaunt,
            BurstLightning,
            Absorption,
            DrillDash,
            FusionRay
        }

        private void HandleLoopedSounds()
        {
            HandleGoozmaShootSound();
            HandleGoozmaWarbleSound();
            HandleGoozmaSimmerSound();
        }

        public LoopingSound goozmaWarble;
        public float goozmaWarbleVolume;
        public float goozmaWarblePitch;

        public void HandleGoozmaWarbleSound()
        {
            float volumeScale = (Phase > 1 ? 0.9f : 0.4f);
            if (Phase == 3)
                volumeScale *= Utils.GetLerpValue(140, 0, Time, true);
            goozmaWarbleVolume = MathHelper.Lerp(goozmaWarbleVolume, Math.Clamp(1f - Main.LocalPlayer.Distance(NPC.Center) * 0.0001f, 0, 1) * NPC.scale * volumeScale + NPC.velocity.Length() * 0.2f, 0.1f);
            goozmaWarblePitch = MathHelper.Lerp(goozmaWarblePitch, Math.Clamp(NPC.velocity.Length() * 0.02f - Main.LocalPlayer.Distance(NPC.Center) * 0.0001f, -0.8f, 0.8f), 0.1f);

            if (goozmaWarble == null)
                goozmaWarble = new LoopingSound(AssetDirectory.Sounds.Goozma.WarbleLoop, new HuntOfTheOldGodUtils.NPCAudioTracker(NPC).IsActiveAndInGame);
            goozmaWarble.Update(() => NPC.Center, () => goozmaWarbleVolume, () => goozmaWarblePitch);
        }

        public static LoopingSound goozmaShoot;
        public static float goozmaShootPowerCurrent;
        public static float goozmaShootPowerTarget;

        public void HandleGoozmaShootSound()
        {
            goozmaShootPowerCurrent = MathHelper.Lerp(goozmaShootPowerCurrent, goozmaShootPowerTarget, 0.2f);
            if (goozmaShootPowerCurrent < 0.05f)
                goozmaShootPowerCurrent = 0f;
            if (Phase == 1)
                goozmaShootPowerCurrent = Utils.GetLerpValue(340, 510, Time, true) * 1.2f;

            if (goozmaShoot == null)
                goozmaShoot = new LoopingSound(AssetDirectory.Sounds.Goozma.ShootLoop, () => new HuntOfTheOldGodUtils.NPCAudioTracker(NPC).IsActiveAndInGame() && goozmaShootPowerCurrent > 0.05f);
            goozmaShoot.Update(() => NPC.Center, () => goozmaShootPowerCurrent * 1.5f, () => 0f);

            goozmaShootPowerTarget -= 0.05f;
            if (goozmaShootPowerTarget < 0.1f)
                goozmaShootPowerTarget = 0f;
        }

        public static LoopingSound goozmaSimmer;
        public static float goozmaSimmerVolume;
        public static float goozmaSimmerPitch;

        public void HandleGoozmaSimmerSound()
        {
            float pitch = 0f;
            float volume = 0f;
            if (Phase == 1)
            {
                pitch = Utils.GetLerpValue(330, 550, Time, true);
                volume = 0.5f + Utils.GetLerpValue(330, 550, Time, true) * 0.6f;
            }

            goozmaSimmerVolume = Math.Clamp(1f - Main.LocalPlayer.Distance(NPC.Center) * 0.00001f, 0, 1) * volume;
            goozmaSimmerPitch = pitch;

            if (goozmaSimmer == null)
                goozmaSimmer = new LoopingSound(AssetDirectory.Sounds.Goozma.SimmerLoop, new HuntOfTheOldGodUtils.NPCAudioTracker(NPC).IsActiveAndInGame);
            goozmaSimmer.Update(() => NPC.Center, () => goozmaSimmerVolume, () => goozmaSimmerPitch);
        }

        private void SortedProjectileAttack(Vector2 targetPos, SortedProjectileAttackTypes type)
        {
            SoundStyle fizzSound = AssetDirectory.Sounds.Goozma.SlimeShoot;
            SoundStyle bloatSound = AssetDirectory.Sounds.Goozma.BloatedBlastShoot with { PitchVariance = 0.15f };
            SoundStyle dartSound = AssetDirectory.Sounds.Goozma.Dart with { Volume = 0.66f };
            SoundStyle fireballSound = AssetDirectory.Sounds.Goozma.RainbowShoot;
            SoundStyle fusionSound = AssetDirectory.Sounds.Goozma.Shot;

            switch (type)
            {
                case SortedProjectileAttackTypes.Default:

                    break;

                case SortedProjectileAttackTypes.EbonianBubbles:

                    if (NPC.Distance(Target.Center) > 300)
                    {
                        if (Time % 15 == 0)
                        {
                            SoundEngine.PlaySound(fizzSound, NPC.Center);
                            goozmaShootPowerTarget = 1f;
                        }

                        float angle = MathHelper.SmoothStep(1.5f, 0.9f, Time / 350f);
                        if (Time % 6 == 0)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitY.RotatedBy(angle).RotatedByRandom(0.3f) * Main.rand.Next(4, 7), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitY.RotatedBy(-angle).RotatedByRandom(0.3f) * Main.rand.Next(4, 7), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                        }
                        if ((Time + 6) % 6 == 0)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitY.RotatedBy(angle + 0.3f).RotatedByRandom(0.3f) * Main.rand.Next(2, 4), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitY.RotatedBy(-angle - 0.3f).RotatedByRandom(0.3f) * Main.rand.Next(2, 4), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                        }
                        if (Target.Center.Y < NPC.Top.Y - 300)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * Main.rand.Next(2, 8), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                    }

                    break;

                case SortedProjectileAttackTypes.EbonianTrifecta:

                    if (Time >= 180)
                    {
                        if (Time - 180 % 60 == 0)
                        {
                            SoundEngine.PlaySound(fizzSound, NPC.Center);
                            goozmaShootPowerTarget = 1f;

                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(0.3f) * 3f, ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                        }
                    }


                    break;

                case SortedProjectileAttackTypes.CrystalStorm:

                    if (NPC.Distance(Target.Center) > 300)
                    {
                        if (Time % 18 == 0)
                        {
                            SoundEngine.PlaySound(fizzSound, NPC.Center);
                            goozmaShootPowerTarget = 1f;
                        }

                        float angle = MathHelper.SmoothStep(-1.5f, -1.0f, Time / 350f);
                        if (Time % 15 == 0)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, -Vector2.UnitY.RotatedBy(angle).RotatedByRandom(0.2f) * Main.rand.Next(4, 7), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, -Vector2.UnitY.RotatedBy(-angle).RotatedByRandom(0.2f) * Main.rand.Next(4, 7), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                        }
                        if ((Time + 8) % 8 == 0)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, -Vector2.UnitY.RotatedBy(angle - 0.3f).RotatedByRandom(0.2f) * Main.rand.Next(2, 4), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, -Vector2.UnitY.RotatedBy(-angle + 0.3f).RotatedByRandom(0.2f) * Main.rand.Next(2, 4), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                        }
                        if (Target.Center.Y > NPC.Top.Y + 300)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * Main.rand.Next(2, 8), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                    }

                    break;

                case SortedProjectileAttackTypes.PixieBallDisruption:

                    if (Time % 80 == 75)
                    {
                        int count = Main.rand.Next(10, 18);
                        for (int i = 0; i < count; i++)
                        {
                            Projectile dart = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.Next(10, 20), 0).RotatedBy(MathHelper.TwoPi / count * i), ModContent.ProjectileType<BloatedBlast>(), GetDamage(1), 0);
                            dart.ai[1] = 1;
                            dart.localAI[0] = NPC.localAI[0] + i / (float)count * 90f;
                        }
                        SoundEngine.PlaySound(dartSound, NPC.Center);
                    }

                    break;
                
                case SortedProjectileAttackTypes.CrimulanSlam:

                    if (NPC.Distance(Target.Center) > 300)
                    {
                        if (Time % 25 == 0)
                        {
                            SoundEngine.PlaySound(fizzSound, NPC.Center);
                            goozmaShootPowerTarget = 1f;
                        }

                        if ((Time + Main.rand.Next(0, 2)) % 8 == 0)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(0.15f).RotatedBy(0.8f) * 20f, ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                        if ((Time + Main.rand.Next(0, 2)) % 8 == 0)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(0.15f).RotatedBy(-0.8f) * 20f, ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);

                    }

                    break;

                case SortedProjectileAttackTypes.CrimulanHop:

                    if (Time % 80 < 20 && Time % 5 == 1)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(1f), ModContent.ProjectileType<RainbowLaser>(), GetDamage(1), 0, ai0: -Main.rand.Next(30, 35), ai1: NPC.whoAmI);
                    
                    if (Time % 80 < 50 && Time % 15 == 5)
                    {
                        SoundEngine.PlaySound(fizzSound, NPC.Center);
                        goozmaShootPowerTarget = 1f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(4f), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                    }

                    break;

                case SortedProjectileAttackTypes.StellarDisruption:

                    //if ((Time - 70) % 160 > 100)
                    //{
                    //    if ((Time - 70) % 5 == 0)
                    //    {
                    //        SoundEngine.PlaySound(fizzSound, NPC.Center);
                    //        goozmaShootPowerTarget = 1f;
                    //    }
                    //    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(0.3f) * 8f, ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                    //}
                    if ((Time - 75) % 160 > 80 && (Time - 75) % 15 == 10)
                    {
                        Projectile pure = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(1.3f) * (NPC.Distance(targetPos) + 220) * 0.02f * Main.rand.NextFloat(0.9f, 1.2f), ModContent.ProjectileType<SlimeBomb>(), GetDamage(2), 0);
                        pure.ai[0] = (int)((Time - 70) % 160 / 1.5f) - 30;
                    }

                    break;

                case SortedProjectileAttackTypes.StellarTaunt:

                    //if (Time % 100 == 40 || Main.rand.NextBool(180))
                    //{
                    //        SoundEngine.PlaySound(fizzSound, NPC.Center);
                    //        goozmaShootPowerTarget = 1f;
                    //    }

                    //    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(1f) * 2f, ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                    //}

                    if (Time % 130 == 10)
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(1.3f) * NPC.Distance(targetPos) * 0.0016f * Main.rand.NextFloat(0.9f, 1.2f), ModContent.ProjectileType<SlimeBomb>(), GetDamage(2), 0);

                    break;

                case SortedProjectileAttackTypes.BurstLightning:

                    if (Time % 170 > 130)
                    {
                        int freq = (int)DifficultyBasedValue(10, 9, 7, 6);
                        if ((Time % 170) % freq == 0)
                        {
                            SoundEngine.PlaySound(fireballSound, NPC.Center);

                            Projectile fireball = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero).RotatedByRandom(0.3f).RotatedBy(1f * NPC.direction) * 16f, ModContent.ProjectileType<RainbowBall>(), GetDamage(4), 0);
                            fireball.localAI[0] = NPC.localAI[0];
                        }
                    }
                    else if (Time % 20 == 0)
                    {
                        SoundEngine.PlaySound(fizzSound, NPC.Center);
                        goozmaShootPowerTarget = 1f;

                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(1f), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                    }

                    if (Time % 270 == 0)
                    {
                        SoundEngine.PlaySound(bloatSound, NPC.Center);

                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero) * 10, ModContent.ProjectileType<BloatedBlast>(), GetDamage(5), 0);                  
                    }

                    if (Time % 140 == 0)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionFrom(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(2.5f), ModContent.ProjectileType<GooLightning>(), GetDamage(4), 0, -1, -50, 1500, 1);
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero), ModContent.ProjectileType<GooLightning>(), GetDamage(3), 0, -1, -50, 1500, 0);
                    }

                    if (Time % 55 == 0 && !Main.rand.NextBool(20))
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionFrom(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(2.5f), ModContent.ProjectileType<GooLightning>(), GetDamage(4), 0, -1, -50, 1500, 1);

                    break;

                case SortedProjectileAttackTypes.DrillDash:

                    int dashTime = (int)DifficultyBasedValue(110, 100, 90, 80);

                    if (Time % dashTime > 24)
                    {
                        if (Time % dashTime == 25)
                        {
                            SoundEngine.PlaySound(fizzSound, NPC.Center);
                            goozmaShootPowerTarget = 1f;
                        }
                        if (Time % dashTime < 30)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(1f) * 2.5f, ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);

                    }

                    if (Time % 30 == 5)
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(0.1f) * 10f, ModContent.ProjectileType<SlimeBomb>(), GetDamage(2), 0);
                    
                    if (Time % dashTime == 70)
                    {
                        SoundEngine.PlaySound(dartSound, NPC.Center);

                        int count = Main.rand.Next(18, 25);
                        for (int i = 0; i < count; i++)
                        {
                            Projectile dart = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.Next(12, 15), 0).RotatedBy(MathHelper.TwoPi / count * i), ModContent.ProjectileType<BloatedBlast>(), GetDamage(5), 0);
                            dart.ai[1] = 1;
                            dart.localAI[0] = NPC.localAI[0] + i / (float)count * 90f;
                        }
                    }

                    break;

                case SortedProjectileAttackTypes.Absorption:

                    //if (Time % 80 >= 60 && Time % 5 == 0)
                    //{
                    //        SoundEngine.PlaySound(dartSound, NPC.Center);

                    //    Projectile dart = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(1f) * Main.rand.Next(20, 25), ModContent.ProjectileType<BloatedBlast>(), GetDamage(5), 0);
                    //    dart.ai[1] = 1;
                    //    dart.localAI[0] = NPC.localAI[0] + Time % 80;
                    //}

                    break;
                
                case SortedProjectileAttackTypes.FusionRay:

                    if (Phase == -2)
                    {
                        if (Time == 2)
                        {
                            Projectile fusionRay = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero), ModContent.ProjectileType<FusionRay>(), GetDamage(7), 0);
                            fusionRay.ai[0] = 200;
                            fusionRay.ai[1] = 1;
                            fusionRay.ai[2] = NPC.whoAmI;
                        }
                    }
                    else
                    {
                        if (Time < 350 && Time % 18 < 3)
                        {
                            if (Time % 18 == 1)
                            {
                                SoundEngine.PlaySound(fizzSound, NPC.Center);
                                goozmaShootPowerTarget = 1f;
                            }

                            for (int i = 0; i < Main.rand.Next(2, 4); i++)
                                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(0.5f) * Main.rand.NextFloat(1.8f, 2.5f), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);

                            if (Time % 30 == 0)
                            {
                                SoundEngine.PlaySound(fireballSound, NPC.Center);

                                Projectile fireball = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero).RotatedByRandom(0.3f).RotatedBy(1f * NPC.direction) * 16f, ModContent.ProjectileType<RainbowBall>(), GetDamage(4), 0);
                                fireball.localAI[0] = NPC.localAI[0];
                            }
                        }

                        if (Time > 150 && Time <= 450 && Time % 90 == 0)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero) * 5, ModContent.ProjectileType<BloatedBlast>(), GetDamage(5), 0);
                            SoundEngine.PlaySound(bloatSound, NPC.Center);
                        }

                        if (Time > 250 && Time < 530 && Time % 40 == 0)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero).RotatedByRandom(1f), ModContent.ProjectileType<GooLightning>(), GetDamage(4), 0, -1, -50, 1500, 1);

                        if (Time == 250)
                        {
                            Projectile fusionRay = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero), ModContent.ProjectileType<FusionRay>(), GetDamage(7), 0);
                            fusionRay.ai[2] = NPC.whoAmI;
                        }

                        if (Time > 500)
                        {
                            if (Time % 70 == 0)
                            {
                                SoundEngine.PlaySound(bloatSound, NPC.Center);

                                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero) * 5, ModContent.ProjectileType<BloatedBlast>(), GetDamage(5), 0);
                            }

                            if (Time % 46 == 30)
                            {
                                Vector2 bombVelocity = HuntOfTheOldGodUtils.GetDesiredVelocityForDistance(NPC.Center, targetPos, 0.955f, 40).RotatedByRandom(0.5f) * Main.rand.NextFloat(0.8f, 1.2f);
                                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, bombVelocity, ModContent.ProjectileType<SlimeBomb>(), GetDamage(5), 0);
                            }
                        }
                    }
                    
                    break;
            }
        }

        private int GetDamage(int attack, float modifier = 1f)
        {
            int damage = attack switch
            {
                0 => Main.expertMode ? 600 : 300,//contact
                1 => 75,//slime balls
                2 => 105,//pure gel
                3 => 125, //lightning
                4 => 60, //static
                5 => 100, //bloat
                6 => Main.expertMode ? 400 : 200, //drill dash
                7 => 200, //fusion ray
                _ => 0
            };

            return (int)(damage * modifier);
        }

        private float extraTilt;
        private Vector2 drawOffset;
        private float headScale;
        public Vector2 drawVelocity;
        private Vector2 tentacleVelocity;
        private Vector2 tentacleAcceleration;
        private Vector2[] oldTentacleVel;
        public bool rotate;

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.localAI[0]++;

            if (oldVel == null)
            {
                oldVel = new Vector2[NPCID.Sets.TrailCacheLength[Type]];
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[Type]; i++)
                    oldVel[i] = NPC.velocity;
            }

            drawOffset = new Vector2((float)Math.Sin(NPC.localAI[0] * 0.05f % MathHelper.TwoPi) * 2, (float)Math.Cos(NPC.localAI[0] * 0.025f % MathHelper.TwoPi) * 3);
            float offsetWobble = (float)Math.Cos(NPC.localAI[0] * 0.05f % MathHelper.TwoPi) * 0.07f;

            if (!rotate)
                NPC.rotation = NPC.rotation.AngleLerp(Math.Clamp(drawVelocity.X * 0.012f, -1f, 1f) - (offsetWobble - 0.1f) * NPC.direction, 0.2f);

            extraTilt = MathHelper.Lerp(extraTilt, Math.Clamp(-drawVelocity.X * 0.025f, -1f, 1f) - 0.01f * NPC.direction, 0.15f);
            headScale = MathHelper.Lerp(headScale, 1f, 0.05f);

            tentacleVelocity *= 0.8f;
            tentacleAcceleration = NPC.velocity - oldVel[(int)(oldVel.Length / 2f)];
            tentacleVelocity += tentacleAcceleration.RotatedBy(-NPC.rotation) * 0.5f;

            if (oldTentacleVel != null)
            {
                for (int i = NPCID.Sets.TrailCacheLength[Type] - 1; i > 0; i--)
                    oldTentacleVel[i] = Vector2.Lerp(oldTentacleVel[i], oldTentacleVel[i - 1], 0.3f);
                oldTentacleVel[0] = Vector2.Lerp(oldTentacleVel[0], tentacleVelocity, 0.3f);
            }

            rotate = false;
        }

        private void FadeMusicOut(On_Main.orig_UpdateAudio orig, Main self)
        {
            orig(self);

            if (Main.npc.Any(n => n.active && n.type == ModContent.NPCType<Goozma>()))
            {
                NPC goozma = Main.npc.FirstOrDefault(n => n.active && n.type == Type);
                if (goozma.ai[2] == 1)
                {
                    for (int i = 0; i < Main.musicFade.Length; i++)
                    {
                        float volume = Main.musicFade[i] * Main.musicVolume * Utils.GetLerpValue(320, 20, goozma.ai[0], true);
                        float tempFade = Main.musicFade[i];
                        Main.audioSystem.UpdateCommonTrackTowardStopping(i, volume, ref tempFade, Main.musicFade[i] > 0.15f && goozma.ai[0] < 320);
                        Main.musicFade[i] = tempFade;
                    }
                }                
                if (goozma.ai[2] == 3)
                {
                    for (int i = 0; i < Main.musicFade.Length; i++)
                    {
                        float volume = Main.musicFade[i] * Main.musicVolume * Utils.GetLerpValue(170, 20, goozma.ai[0], true);
                        float tempFade = Main.musicFade[i];
                        Main.audioSystem.UpdateCommonTrackTowardStopping(i, volume, ref tempFade, Main.musicFade[i] > 0.15f && goozma.ai[0] < 320);
                        Main.musicFade[i] = tempFade;
                    }
                }
            }
        }

        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            GoozmaResistances.GoozmaItemResistances(item, ref modifiers);
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            GoozmaResistances.GoozmaProjectileResistances(projectile, ref modifiers);
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
        }

        //helper methods to do syncing
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

        public void SetPhase(int phase, bool zeroTime = true)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Phase = phase; 
                if (zeroTime)
                    Time = 0;
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
