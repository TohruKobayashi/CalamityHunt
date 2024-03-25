using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalamityHunt.Common.DropRules;
using CalamityHunt.Common.Graphics;
using CalamityHunt.Common.Graphics.Skies;
using CalamityHunt.Common.Systems;
using CalamityHunt.Common.Systems.Camera;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Common.Utilities.Interfaces;
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
using CalamityHunt.Content.Items.Placeable;
using CalamityHunt.Content.Items.Weapons.Magic;
using CalamityHunt.Content.Items.Weapons.Melee;
using CalamityHunt.Content.Items.Weapons.Ranged;
using CalamityHunt.Content.Items.Weapons.Rogue;
using CalamityHunt.Content.Items.Weapons.Summoner;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss.Projectiles;
using CalamityHunt.Content.Particles;
using CalamityHunt.Content.Pets.BloatBabyPet;
using CalamityHunt.Content.Projectiles;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using static CalamityHunt.Common.Systems.ConditionalValue;

namespace CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;

[AutoloadBossHead]
public partial class Goozma : ModNPC, ISubjectOfNPC<Goozma>
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

        NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
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

    private int P2LifeMax = 1787500 / 2;

    public override void SetDefaults()
    {
        NPC.width = 150;
        NPC.height = 150;
        NPC.damage = 0;
        NPC.defense = 110;
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

        if (!Main.dedServ) {
            Music = AssetDirectory.Music.GoozmaSoul;
            Music1 = AssetDirectory.Music.GoozmaPhase1;
            Music2 = AssetDirectory.Music.GoozmaPhase2;
        }

        if (ModLoader.TryGetMod(HUtils.CalamityMod, out Mod calamity)) {
            calamity.Call("SetDebuffVulnerabilities", "poison", false);
            calamity.Call("SetDebuffVulnerabilities", "heat", true);
            calamity.Call("SetDefenseDamageNPC", NPC, true);

            NPC.lifeMax = 1787500;
            if (Main.expertMode) {
                NPC.lifeMax = 2925000;
            }

            if ((bool)calamity.Call("GetDifficultyActive", "revengeance")) {
                NPC.lifeMax = 4225000;
            }
        }
        else {
            NPC.lifeMax = 1787500;
            if (Main.expertMode) {
                NPC.lifeMax = 2925000;
            }
        }

        P2LifeMax = NPC.lifeMax / 2;

        SlimeUtils.GoozmaColorType = Main.rand.Next(54);

        if (Main.drunkWorld) {
            SlimeUtils.GoozmaColorType = Main.rand.Next(10);
        }
    }

    public override void BossHeadRotation(ref float rotation) => rotation = NPC.velocity.X * 0.01f;

    public static int Music1;
    public static int Music2;

    public override void Load()
    {
        On_Main.UpdateAudio += FadeMusicOut;
        //On_Main.CheckMonoliths += DrawCordShapes;
        //nPCsToDrawCordOn = new List<NPC>();

        LoadAssets();
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GoozmaSoul>()));
        npcLoot.Add(ItemDropRule.ByCondition(new GoozmaDownedDropRule(), ModContent.ItemType<GoozmaLore>()));
        npcLoot.Add(ItemDropRule.ByCondition(new ZenithWorldDropRule(), ModContent.ItemType<Goozmaga>()));

        if (Main.rand.NextBool(20)) {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<TreasureBucket>()));
        }
        else {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<TreasureTrunk>()));
        }

        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GoozmaTrophy>(), 10));
        npcLoot.Add(ItemDropRule.FewFromOptions(1, 10, ModContent.ItemType<EbonianBehemuckTrophy>(), ModContent.ItemType<DivineGargooptuarTrophy>(), ModContent.ItemType<CrimulanGlopstrosityTrophy>(), ModContent.ItemType<StellarGeliathTrophy>()));

        npcLoot.Add(ItemDropRule.ByCondition(new MasterRevDropRule(), ModContent.ItemType<GoozmaRelic>()));

        npcLoot.Add(ItemDropRule.ByCondition(new MasterRevDropRule(), ModContent.ItemType<ImperialGelato>(), 4));

        LeadingConditionRule classic = new LeadingConditionRule(new Conditions.NotExpert());

        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<SplendorJam>()));
        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ShogunHelm>(), 3));
        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ShogunChestplate>(), 3));
        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ShogunPants>(), 3));
        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<StickyHand>(), 3));
        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ChromaticMass>(), 1, 20, 30));
        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<PaladinPalanquin>(), 4));
        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<TendrilCursorAttachment>(), 20));

        classic.OnSuccess(ItemDropRule.FewFromOptions(1, 7, ModContent.ItemType<EbonianMask>(), ModContent.ItemType<DivineMask>(), ModContent.ItemType<CrimulanMask>(), ModContent.ItemType<StellarMask>()));
        classic.OnSuccess(ItemDropRule.FewFromOptions(2, 1, ModContent.ItemType<Parasanguine>(), ModContent.ItemType<SludgeShaker>(), ModContent.ItemType<CrystalGauntlets>(), ModContent.ItemType<SlimeCane>(), ModContent.ItemType<CometKunai>()));
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

        // i'm sorry but the sendextraai is called before ai so it make the nextAttack null and remove goozma rip
        // and i have to make it array or it errors ripp - jester
        nextAttack ??= new byte[][] {
            new byte[] { 0, 1, 2 },
            new byte[] { 0, 1, 2 },
            new byte[] { 0, 1, 2 },
            new byte[] { 0, 1, 2 }
        };

        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 3; j++) {
                writer.Write(nextAttack[i][j]);
            }
        }
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        currentSlime = reader.ReadInt16();

        nextAttack ??= new byte[][] {
            new byte[] { 0, 1, 2 },
            new byte[] { 0, 1, 2 },
            new byte[] { 0, 1, 2 },
            new byte[] { 0, 1, 2 }
        };

        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 3; j++) {
                nextAttack[i][j] = reader.ReadByte();
            }
        }
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
    {
        if (Main.getGoodWorld && Main.masterMode && RevengeanceMode) {
            target.AddBuff(BuffID.VortexDebuff, 480);
        }
    }

    private void ChangeWeather()
    {
        //if (!Main.slimeRain) {
        //    Main.StartSlimeRain(false);
        //}

        //Main.slimeRainTime += 1.0;

        Main.LocalPlayer.GetModPlayer<SceneEffectPlayer>().effectActive[(ushort)SceneEffectPlayer.EffectorType.SlimeMonsoon] = 30;

        if (Phase >= 2) {
            SlimeMonsoonSky.additionalLightningChance = -53;
        }
    }

    private Vector2 saveTarget;
    private Vector2 eyePower;
    private bool eyeOpen;
    private float soulScale;

    public override bool CheckDead()
    {
        //if (NPC.life < 1) {
        if (Main.netMode != NetmodeID.MultiplayerClient) {

            NPC.netUpdate = true;
            switch (Phase) {
                case 0:
                    SetPhase(1);
                    NPC.lifeMax = P2LifeMax;
                    if (!Main.expertMode && !Main.masterMode) {
                        SetPhase(3);
                    }
                    break;

                case -2:
                case 2:
                    SetPhase(3);


                    break;
            }
        }
        else {
            if (Phase == 0) { // fix display
                NPC.lifeMax = P2LifeMax;
            }
        }

        NPC.life = 1;
        NPC.active = true;
        NPC.dontTakeDamage = true;
        
        //return false;
        //}


        //return false;
        // i have to do this or goozma just randomly disappears when phase change and makes my brain melt in gel - jester
        return Phase > 3 || (Phase == 3 && Time >= 300);
    }

    private short currentSlime;
    private byte[][] nextAttack;

    private static readonly float SpecialAttackWeight = 0.0f; //unable to choose, therefore saved for last

    public int GetSlimeAttack()
    {
        int gotten = -1;

        if (Main.netMode != NetmodeID.MultiplayerClient) {
            if (nextAttack[currentSlime] == null || nextAttack[currentSlime].Length == 0) {
                nextAttack[currentSlime] = new byte[] { 0, 1, 2 };
            }

            const byte skipIt = 255;

            WeightedRandom<int> weightedRandom = new WeightedRandom<int>(Main.rand);
            foreach (int j in nextAttack[currentSlime]) {
                if (j == skipIt) continue;
                weightedRandom.Add(j, j == 2 ? SpecialAttackWeight : 1f);
            }

            gotten = weightedRandom.Get();
            nextAttack[currentSlime][gotten] = skipIt;

            NPC.netUpdate = true;
        }

        return gotten;
    }

    public override void OnSpawn(IEntitySource source)
    {
        eyeOpen = false;
        SetPhase(-1);
        for (int i = 0; i < NPC.oldPos.Length; i++) {
            NPC.oldPos[i] = NPC.Center;
        }
    }

    // the time is non-linear in multiplayer.... goozma has learnt the quantum mechanics....
    private bool initializedLocal = false;
    private bool initializedLocal2 = false;

    private bool initializedLocalP2Camera = false;
    private bool initializedLocalP2Sparkle = false;
    //private bool initializedLocalP2Reform = false;
    private bool initializedLocalP2Roar = false;

    private bool initializedDeathRoar = false;

    public override void AI()
    {

        if (Phase == -1 && Time <= 0) {

            NPC.ai[3] = -1;
            currentSlime = -1;
        }

        if (Phase == -1 && !initializedLocal2) {
            headScale = 1f;

            SoundEngine.PlaySound(AssetDirectory.Sounds.Goozma.Hurt, NPC.Center);
            initializedLocal2 = true;
        }

        nextAttack ??= new byte[][] {
            new byte[] { 0, 1, 2 },
            new byte[] { 0, 1, 2 },
            new byte[] { 0, 1, 2 },
            new byte[] { 0, 1, 2 }
        };

        ChangeWeather();
        GoozmaResistances.DisablePointBlank();

        if (ModLoader.HasMod(HUtils.CalamityMod)) {
            Main.LocalPlayer.AddBuff(ModLoader.GetMod(HUtils.CalamityMod).Find<ModBuff>("BossEffects").Type, 2);
        }

        bool noSlime = NPC.ai[3] < 0 || NPC.ai[3] >= Main.maxNPCs || ActiveSlime.ai[1] > 3 || !ActiveSlime.active;

        if (Phase == 0 && noSlime) {
            SetAttack(AttackList.SpawnSlime);
        }

        if (NPC.velocity.HasNaNs()) {
            NPC.velocity = Vector2.Zero;
        }

        if (Target.Invalid) {
            NPC.TargetClosestUpgraded();
        }

        if (NPC.GetTargetData().Invalid && Phase != -5) {
            SetPhase(-5);
            NPC.dontTakeDamage = true;
            NPC.velocity = Vector2.Zero;
            return;
        }

        if (NPC.Distance(Target.Center) > 800 && Phase == 0 || Phase == 2) {
            NPC.Center = Vector2.Lerp(NPC.Center, NPC.Center + NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * Math.Max(0, NPC.Distance(Target.Center) - 800), 0.01f);
        }

        if (Phase != 3) {
            if (NPC.velocity.Length() < 50f) {
                if (Math.Abs(NPC.Center.X - Target.Center.X) > 20) {
                    NPC.direction = NPC.Center.X > Target.Center.X ? -1 : 1;
                }
            }
            else {
                NPC.direction = NPC.velocity.X > 0 ? 1 : -1;
            }
        }

        NPC.damage = 0;

        if (!NPC.dontTakeDamage || Phase == 0) {
            NPC.damage = GetDamage(0);
            NPC.takenDamageMultiplier = 1f;

            if (!noSlime) {
                if (NPC.Distance(ActiveSlime.Center) < 400) {
                    NPC.damage = 0;
                }
            }

            if (Phase <= 1 && Time < 60) {
                NPC.damage = 0;
            }
        }

        //good luck religious passage
        //Matthew 25:31 When the Son of man shall come in his glory, and all the holy angels with him, then shall he sit upon the throne of his glory

        //number 2
        //1 Chronicles 16:11 Seek the Lord and His strength; Seek His face evermore!

        switch (Phase) {
            case -1:

                const float spawnDelay = 120;
                SetAttack(AttackList.SpawnSelf);
                NPC.direction = -1;
                //NPC.velocity.X = Utils.GetLerpValue(spawnDelay + 3, spawnDelay - 5, Time, true) * (MathF.Cos(Time * MathHelper.Pi / spawnDelay * 2f + 0.33f)) * -20f;
                //NPC.velocity.Y = Utils.GetLerpValue(spawnDelay + 3, spawnDelay / 2f, Time, true) * (MathF.Cos(Time * MathHelper.Pi / spawnDelay * 4f + 0.1f) - Utils.GetLerpValue(spawnDelay / 2.2f, spawnDelay / 1.8f, Time, true)) * 17f;
                eyePower = new Vector2(1.3f * Utils.GetLerpValue(spawnDelay - 1, spawnDelay + 10, Time, true) * (1f + Utils.GetLerpValue(spawnDelay + 80, spawnDelay + 50, Time, true) * MathF.Sin(Time) * 0.3f));
                //NPC.scale = 0.6f + MathF.Sqrt(Utils.GetLerpValue(spawnDelay * 0.2f, spawnDelay, Time, true)) * 0.4f;

                if (Time < spawnDelay) {

                    NPC.velocity.Y = -0.5f + Utils.GetLerpValue(spawnDelay * 0.2f, spawnDelay, Time, true) * 4f;
                    NPC.direction = NPC.velocity.X > 0 ? 1 : -1;
                    //rotate = true;
                    //NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation() + MathHelper.PiOver2, 0.6f * Utils.GetLerpValue(spawnDelay + 3, spawnDelay - 10, Time, true));

                    for (int i = 0; i < Main.musicFade.Length; i++) {
                        Main.musicFade[i] = 0f;
                    }

                    Main.musicFade[Main.curMusic] = 0f;
                    Main.musicFade[Main.newMusic] = 0f;
                }

                if (Time > spawnDelay - 5) {
                    NPC.velocity *= 0.9f;
                    eyeOpen = true;
                }

                if (Time == spawnDelay) {

                    NPC.netUpdate = true;
                    eyeOpen = true;

                    Music = Music1;

                    if (Main.netMode == NetmodeID.SinglePlayer) {
                        Main.NewText(Language.GetTextValue("Announcement.HasAwoken", NPC.TypeName), HUtils.BossTextColor);
                    }
                    else if (Main.netMode == NetmodeID.Server) {
                        ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", NPC.GetTypeNetName()), HUtils.BossTextColor);
                    }
                }

                if (Time >= spawnDelay && !initializedLocal) { // i must fix the music AAAAAAAAAAAAAAAAAAAAAAAAAAamf
                    Music = Music1;
                    if (Main.netMode != NetmodeID.Server) {
                        Main.newMusic = Music1;
                        Main.musicFade[Main.curMusic] = 0f;
                        Main.musicFade[Main.newMusic] = 1f;
                    }

                    SoundEngine.PlaySound(AssetDirectory.Sounds.Goozma.Awaken.WithVolumeScale(1.8f), NPC.Center);

                    initializedLocal = true;
                }
                if (Time > spawnDelay && Time % 4 == 0) {
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2CircularEdge(1, 1), 20f, 6f, 10, 8000, "Goozma"));
                }

                if (Time > spawnDelay + 70) {
                    SetPhase(0);
                    NPC.dontTakeDamage = false;
                }

                if (Main.netMode != NetmodeID.Server) Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(10, 10), DustID.TintableDust, Main.rand.NextVector2CircularEdge(10, 10), 200, Color.Black, Main.rand.NextFloat(2f, 4f)).noGravity = true;

                if (Main.netMode != NetmodeID.Server && Main.rand.NextBool((int)(Time * 0.1f + 2))) {
                    Vector2 particleVelocity = Vector2.UnitY.RotatedByRandom(1f);
                    particleVelocity.Y -= Main.rand.NextFloat(3f);

                    CalamityHunt.particles.Add(Particle.Create<ChromaticGooBurst>(particle => {
                        particle.position = Main.rand.NextVector2FromRectangle(NPC.Hitbox);
                        particle.velocity = particleVelocity + particle.position.DirectionFrom(NPC.Center);
                        particle.scale = Main.rand.NextFloat(0.5f, 1.5f);
                        particle.color = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).Value;
                    }));
                }

                break;

            case 0:

                if (Attack == (int)AttackList.SpawnSlime) {

                    if (Main.netMode != NetmodeID.MultiplayerClient) {
                        NPC.defense = 1000;
                        NPC.takenDamageMultiplier = 0.1f;

                        if (Time > 5 && Time < 45) {
                            NPC.TargetClosestUpgraded();
                            NPC.direction = NPC.DirectionTo(NPC.GetTargetData().Center).X > 0 ? 1 : -1;
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(NPC.GetTargetData().Center) * Math.Max(NPC.Distance(NPC.GetTargetData().Center) - 150, 0) * 0.12f, 0.1f);

                            NPC.position += Main.rand.NextVector2Circular(6, 6);
                            NPC.netUpdate = true;
                        }

                        if (Time > 42 && Time <= 50 && !(NPC.ai[3] < 0 || NPC.ai[3] >= Main.maxNPCs)) {
                            KillSlime(currentSlime);
                            NPC.netUpdate = true;
                        }
                    }

                    if (Main.netMode != NetmodeID.Server && Time > 5 && Time < 45) {
                        for (int i = 0; i < 3; i++) {
                            Vector2 inward = NPC.Center + Main.rand.NextVector2Circular(70, 70) + Main.rand.NextVector2CircularEdge(100 - Time, 100 - Time);

                            CalamityHunt.particles.Add(Particle.Create<ChromaticEnergyDust>(particle => {
                                particle.position = inward;
                                particle.velocity = inward.DirectionTo(NPC.Center) * Main.rand.NextFloat(3f);
                                particle.scale = 1f;
                                particle.color = Color.White;
                                particle.colorData = new ColorOffsetData(true, NPC.localAI[0]);
                            }));
                        }
                    }

                    if (Time == 50) {
                        NPC.velocity *= -1f;

                        if (Main.netMode != NetmodeID.Server) {
                            for (int i = 0; i < 45; i++) {
                                Vector2 outward = NPC.Center + Main.rand.NextVector2Circular(10, 10);

                                CalamityHunt.particles.Add(Particle.Create<ChromaticEnergyDust>(particle => {
                                    particle.position = outward;
                                    particle.velocity = outward.DirectionFrom(NPC.Center) * Main.rand.NextFloat(3f, 10f);
                                    particle.scale = Main.rand.NextFloat(1f, 2f);
                                    particle.color = Color.White;
                                    particle.colorData = new ColorOffsetData(true, NPC.localAI[0]);
                                }));
                            }
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

                        if (Main.getGoodWorld && Main.netMode != NetmodeID.MultiplayerClient) {
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X - 100, (int)NPC.Center.Y, ModContent.NPCType<Goozmite>());
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + 100, (int)NPC.Center.Y, ModContent.NPCType<Goozmite>());
                        }

                        currentSlime = (short)((currentSlime + 1) % 4);
                        int slimeAttack = GetSlimeAttack();

                        //Test slimes and attacks
                        //currentSlime = 3;
                        //slimeAttack = 1;

                        if (Main.zenithWorld) {
                            currentSlime = (short)Main.rand.Next(0, 4);
                        }

                        int[] slimeTypes = new int[]
                        {
                            ModContent.NPCType<EbonianBehemuck>(),
                            ModContent.NPCType<DivineGargooptuar>(),
                            ModContent.NPCType<CrimulanGlopstrosity>(),
                            ModContent.NPCType<StellarGeliath>()
                        };

                        if (Main.netMode != NetmodeID.MultiplayerClient) {
                            if (NPC.ai[3] < 0 || NPC.ai[3] >= Main.maxNPCs || !ActiveSlime.active) {
                                NPC.ai[3] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y - 50, slimeTypes[currentSlime], ai0: -50, ai1: slimeAttack, ai2: NPC.whoAmI);
                                ActiveSlime.velocity.Y -= 16;
                            }
                            else {
                                Vector2 pos = ActiveSlime.Bottom;
                                ActiveSlime.active = false;
                                NPC.ai[3] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)pos.X, (int)pos.Y, slimeTypes[currentSlime], ai0: -50, ai1: slimeAttack, ai2: NPC.whoAmI);
                            }

                            SoundStyle spawn = AssetDirectory.Sounds.Goozma.SpawnSlime;
                            SoundEngine.PlaySound(spawn, NPC.Center);

                            NPC.netUpdate = true;
                        }
                    }

                    NPC.velocity *= 0.9f;

                    if (Time > 70) {
                        SetAttack((int)Attack + 1, true);
                    }
                }
                else {
                    NPC.defense = 100;

                    //attack depends on slime's attack, which depends on slime
                    switch (currentSlime) {
                        //ebonian
                        case 0:
                            switch (ActiveSlime.ai[1]) {
                                case 0:

                                    AresLockTo(Target.Center - new Vector2(0, 400) + Target.Velocity * new Vector2(5f, 2f));

                                    if (Time > 50) {
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
                            switch (ActiveSlime.ai[1]) {
                                case 0:

                                    Vector2 outerRing = Target.Center.DirectionTo(ActiveSlime.Center) * 800;
                                    FlyTo(ActiveSlime.Center - outerRing);
                                    NPC.velocity *= 0.6f;

                                    break;

                                case 1:

                                    AresLockTo(Target.Center + new Vector2(0, 360) + Target.Velocity * new Vector2(4f, 3f));

                                    if (Time > 50) {
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
                            switch (ActiveSlime.ai[1]) {
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

                                    if (Time > 40) {
                                        SortedProjectileAttack(Target.Center, SortedProjectileAttackTypes.CrimulanHop);
                                    }

                                    NPC.velocity *= 0.9f;

                                    break;
                            }
                            break;

                        //stellar
                        case 3:
                            switch (ActiveSlime.ai[1]) {
                                case 0:

                                    if (Time < 5) {
                                        saveTarget = Target.Center;
                                    }
                                    else {
                                        saveTarget = Vector2.Lerp(saveTarget, Target.Center, 0.05f);
                                    }

                                    if (Time >= 70 && Time < 550) {
                                        SortedProjectileAttack(saveTarget, SortedProjectileAttackTypes.StellarDisruption);
                                    }

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
                                    if (Time < 590) {
                                        SortedProjectileAttack(ActiveSlime.Center, SortedProjectileAttackTypes.StellarTaunt);
                                    }

                                    break;
                            }
                            break;
                    }
                }

                //if (Main.expertMode || Main.masterMode) {
                //    if (NPC.life <= NPC.lifeMax * 0.33f) {
                //        SetPhase((int)Phase + 1);
                //        NPC.dontTakeDamage = true;
                //        NPC.life = (int)(NPC.lifeMax * 0.33f);
                //    }
                //}

                eyePower = Vector2.One * 1.2f;

                break;

            case 1:

                NPC.velocity = Vector2.Zero;

                if (NPC.ai[3] > -1 && NPC.ai[3] <= Main.maxNPCs) {
                    if (ActiveSlime.active) {
                        ActiveSlime.active = false;
                    }
                }

                NPC.dontTakeDamage = true;
                //if (NPC.life < NPC.lifeMax * 0.33f) {
                //    NPC.life = (int)(NPC.lifeMax * 0.33f);
                //}
                NPC.life = 1 + (int)((float)Math.Pow(Utils.GetLerpValue(300, 530, Time, true), 3) * (NPC.lifeMax - 1));
                eyePower = Vector2.SmoothStep(Vector2.One * 1.2f, new Vector2(1.5f, 1.45f), Utils.GetLerpValue(300, 500, Time, true));

                if (Time < 15) {
                    KillSlime(currentSlime);
                }

                if (Main.netMode != NetmodeID.Server && Time > 45 && Time < 53) {
                    for (int i = 0; i < 5; i++) {
                        Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(10, 10), DustID.TintableDust, Main.rand.NextVector2CircularEdge(20, 20), 200, Color.Black, Main.rand.NextFloat(2, 4)).noGravity = true;
                        CalamityHunt.particles.Add(Particle.Create<ChromaticGelChunk>(particle => {
                            particle.position = NPC.Center + Main.rand.NextVector2Circular(10, 10);
                            particle.velocity = Main.rand.NextVector2Circular(20, 20) - Vector2.UnitY * 8f;
                            particle.scale = Main.rand.NextFloat(0.5f, 2f);
                            particle.color = Color.White;
                            particle.colorData = new ColorOffsetData(true, NPC.localAI[0]);
                        }));
                    }
                }

                if (Time < 50) {
                    NPC.scale = (float)Math.Sqrt(Utils.GetLerpValue(50, 10, Time, true));

                    if (Main.netMode != NetmodeID.MultiplayerClient) {
                        NPC.Center += Main.rand.NextVector2Circular(5, 5);

                        NPC.netUpdate = true;
                    }


                    if (Main.netMode != NetmodeID.Server) {
                        for (int i = 0; i < Main.rand.Next(1, 4); i++) {
                            CalamityHunt.particles.Add(Particle.Create<GoozmaGelBit>(particle => {
                                particle.position = NPC.Center + Main.rand.NextVector2Circular(30, 40);
                                particle.velocity = Main.rand.NextVector2CircularEdge(10, 10) + Main.rand.NextVector2Circular(20, 20);
                                particle.scale = Main.rand.NextFloat(1f, 2f);
                                particle.color = Color.White;
                                particle.colorData = new ColorOffsetData(true, (int)(300 - Time + Main.rand.Next(55)));
                                particle.holdTime = 270;
                                particle.anchor = () => NPC.Center;
                            }));
                        }
                    }

                }
                else {
                    NPC.scale = MathHelper.Lerp(NPC.scale, MathHelper.SmoothStep(0f, 1f, Utils.GetLerpValue(400, 500, Time, true)), 0.2f);
                    soulScale = MathHelper.Lerp(soulScale, MathHelper.SmoothStep(0f, 1f, Utils.GetLerpValue(400, 500, Time, true)), 0.2f);
                }

                //if (Time > 400 && Time % 12 == 0)
                //{
                //    Particle crack = Particle.NewParticle(ModContent.GetInstance<CrackSpot>(), NPC.Center, Vector2.Zero, Color.Black, 40f);
                //    crack.data = "GoozmaBlack";
                //    crack.behindEntities = true;
                //}

                if (Main.netMode != NetmodeID.Server && Time < 70 || Time > 400) {
                    CalamityHunt.particles.Add(Particle.Create<ChromaticGooBurst>(particle => {
                        particle.velocity = Main.rand.NextVector2Circular(2, 3);
                        particle.position = NPC.Center + particle.velocity * 27f * NPC.scale;
                        particle.scale = Main.rand.NextFloat(0.1f, 1.6f);
                        particle.color = Color.White;
                        particle.colorData = new ColorOffsetData(true, NPC.localAI[0] + Main.rand.NextFloat(0.2f, 0.5f));
                    }));
                }

                if (Time < 570) {
                    FocusConditional.focusTarget = NPC.Center;
                }

                if (Time >= 1 && !initializedLocalP2Camera) {

                    if (Main.netMode != NetmodeID.Server) {
                        int panOut = Main.masterMode && Main.getGoodWorld ? 600 : 30;
                        Main.instance.CameraModifiers.Add(new FocusConditional(30, panOut, () => Time < 500, "Goozma"));

                        SoundStyle boomSound = AssetDirectory.Sounds.Goozma.Explode;
                        SoundEngine.PlaySound(boomSound, NPC.Center);
                    }

                    initializedLocalP2Camera = true;
                }

                if (Time >= 290 && !initializedLocalP2Sparkle) {
                    if (Main.netMode != NetmodeID.Server) {
                        CalamityHunt.particles.Add(Particle.Create<CrossSparkle>(particle => {
                            particle.velocity = MathHelper.PiOver4.ToRotationVector2();
                            particle.position = NPC.Center;
                            particle.scale = 4f;
                            particle.color = Color.White;
                        }));
                        CalamityHunt.particles.Add(Particle.Create<CrossSparkle>(particle => {
                            particle.velocity = Vector2.Zero;
                            particle.position = NPC.Center;
                            particle.scale = 4f;
                            particle.color = Color.White;
                        }));

                        SoundEngine.PlaySound(AssetDirectory.Sounds.Goozma.EyeAppear, NPC.Center);

                        SoundStyle reform = AssetDirectory.Sounds.Goozma.Reform;
                        SoundEngine.PlaySound(reform, NPC.Center);
                    }
                    initializedLocalP2Sparkle = true;
                }

                //if (Time >= 290 && !initializedLocalP2Reform) {
                //    SoundStyle reform = AssetDirectory.Sounds.Goozma.Reform;
                //    SoundEngine.PlaySound(reform, NPC.Center);

                //    initializedLocalP2Reform = true;
                //}

                if (Time >= 570 && !initializedLocalP2Roar) {
                    SoundStyle roar = AssetDirectory.Sounds.Goozma.Reawaken;
                    SoundEngine.PlaySound(roar, NPC.Center);

                    Music = Music2;
                    Main.newMusic = Music1;
                    Main.musicFade[Main.curMusic] = 0f;
                    Main.musicFade[Main.newMusic] = 1f;

                    initializedLocalP2Roar = true;
                }

                if (Time > 571) {
                    if (Main.netMode != NetmodeID.MultiplayerClient) {
                        NPC.dontTakeDamage = false;
                        NPC.defense += 20;
                        NPC.netUpdate = true;
                    }

                    Music = Music2;


                    SetPhase((int)Phase + 1);

                }

                break;

            case 2:
                switch (Attack) {
                    case (int)AttackList.BurstLightning:

                        Orbit((int)(600 + ((float)NPC.life / NPC.lifeMax * 100)), new Vector2(0, -450));
                        SortedProjectileAttack(Target.Center + Target.Velocity * 1.5f, SortedProjectileAttackTypes.BurstLightning);

                        if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(30)) {
                            CalamityHunt.particles.Add(Particle.Create<ChromaticGelChunk>(particle => {
                                particle.position = NPC.Bottom + Main.rand.NextVector2Circular(10, 10);
                                particle.velocity = Main.rand.NextVector2Circular(5, 2) - Vector2.UnitY * 5f + NPC.velocity * 0.1f;
                                particle.scale = Main.rand.NextFloat(0.1f, 1.6f);
                                particle.color = Color.White;
                                particle.colorData = new ColorOffsetData(true, NPC.localAI[0]);
                            }));
                        }

                        if (Time > 640) {
                            SetTime(-60);
                            SetAttack(AttackList.DrillDash);
                        }

                        break;

                    case (int)AttackList.DrillDash:

                        int dashCount = (int)DifficultyBasedValue(4, 5, 6, 7);
                        int dashTime = (int)DifficultyBasedValue(110, 100, 90, 80);

                        if (Time >= 0) {
                            if (Time <= dashTime * dashCount) {
                                if (Time % dashTime == 21) {
                                    SoundStyle roar = AssetDirectory.Sounds.Goozma.Dash;
                                    SoundEngine.PlaySound(roar, NPC.Center);
                                }

                                if (Main.netMode != NetmodeID.MultiplayerClient) {
                                    if (Time % dashTime <= 13) {
                                        NPC.Center = Vector2.Lerp(NPC.Center, NPC.Center + NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * (NPC.Distance(Target.Center) - 300) * 0.2f, 0.6f);
                                        NPC.velocity = NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero);

                                        if (Time % dashTime == 13) {
                                            NPC.velocity -= NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * 20f;
                                        }
                                    }

                                    if (Time % dashTime > 25 && Time % dashTime < 74) {
                                        rotate = true;
                                        NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.ToRotation() + MathHelper.PiOver2, 0.33f * Utils.GetLerpValue(20, 70, Time % dashTime, true));

                                        if (Main.netMode != NetmodeID.Server) {
                                            for (int i = 0; i < 8; i++) {
                                                Vector2 position = Vector2.Lerp(NPC.position, NPC.oldPos[0], i / 24f) + NPC.Size * 0.5f;

                                                //top
                                                CalamityHunt.particlesBehindEntities.Add(Particle.Create<ChromaticEnergyDust>(particle => {
                                                    particle.position = position + Main.rand.NextVector2Circular(8, 8) + NPC.velocity;
                                                    particle.velocity = -NPC.velocity.RotatedBy((float)Math.Sin((Time - (i / 8f)) * 0.23f) * 0.8f);
                                                    particle.scale = 1.5f;
                                                    particle.color = Color.White;
                                                    particle.colorData = new ColorOffsetData(true, NPC.localAI[0]);
                                                }));

                                                //bottom
                                                CalamityHunt.particlesBehindEntities.Add(Particle.Create<ChromaticEnergyDust>(particle => {
                                                    particle.position = position + Main.rand.NextVector2Circular(8, 8) + NPC.velocity;
                                                    particle.velocity = -NPC.velocity.RotatedBy(-(float)Math.Sin((Time - (i / 8f)) * 0.23f) * 0.8f);
                                                    particle.scale = 1.5f;
                                                    particle.color = Color.White;
                                                    particle.colorData = new ColorOffsetData(true, NPC.localAI[0]);
                                                }));
                                            }
                                        }
                                        NPC.velocity += NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * 0.2f;
                                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * 60f, 0.02f);

                                        NPC.netUpdate = true;
                                    }
                                    else {
                                        NPC.velocity *= 0.7f;
                                    }
                                }

                                SortedProjectileAttack(Target.Center, SortedProjectileAttackTypes.DrillDash);
                            }
                        }
                        else {
                            Fly();
                        }

                        if (Time > dashTime * dashCount + 20 && Main.netMode != NetmodeID.MultiplayerClient) {
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

                        if (Time < 10) {
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(NPC.GetTargetData().Center) * Math.Max(NPC.Distance(NPC.GetTargetData().Center) - 150, 0) * 0.12f, 0.1f);
                            NPC.velocity *= 0.9f;

                            if (Main.netMode != NetmodeID.MultiplayerClient) {
                                NPC.position += Main.rand.NextVector2Circular(3, 3);
                                NPC.netUpdate = true;
                            }
                        }

                        else {
                            Fly();
                            NPC.velocity *= 0.5f;
                        }

                        if (Time >= 0 && Time < goozmiteCount * 5f) {
                            if (Main.netMode != NetmodeID.MultiplayerClient) {
                                NPC.position += Main.rand.NextVector2Circular(8, 8);
                                NPC.netUpdate = true;
                            }

                            if (Time % 5 == 1) {
                                Vector2 velocity = new Vector2(1f, 0).RotatedBy(MathHelper.TwoPi * Utils.GetLerpValue(0, goozmiteCount * 5f, Time, true));
                                if (Main.netMode != NetmodeID.MultiplayerClient) {
                                    SoundStyle spawn = AssetDirectory.Sounds.Goozma.SpawnSlime;
                                    SoundEngine.PlaySound(spawn, NPC.Center);

                                    float radius = 100;
                                    NPC goozmite = NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)(NPC.Center.X + velocity.X * radius), (int)(NPC.Center.Y + velocity.Y * radius) + 40, ModContent.NPCType<Goozmite>(), ai1: killTime, ai2: NPC.whoAmI);
                                    goozmite.velocity = velocity * 60f;
                                    goozmite.ai[1] = killTime - Main.npc.Count(n => n.active && n.type == ModContent.NPCType<Goozmite>()) * waitTimeMultiplier;
                                    goozmite.localAI[0] = NPC.localAI[0] + 60f * Utils.GetLerpValue(0, goozmiteCount * 5f, Time, true);
                                }
                                else if (Main.netMode == NetmodeID.MultiplayerClient) {
                                    NPC.netUpdate = true;
                                }
                            }
                        }

                        if (Time > waitTime && Time < waitTime + killTime) {
                            SortedProjectileAttack(Target.Center + Target.Velocity * 10f, SortedProjectileAttackTypes.Absorption);
                        }

                        if (Time > waitTime + 20) {
                            if (!Main.npc.Any(n => n.active && n.type == ModContent.NPCType<Goozmite>()) && Time < waitTime + killTime) {
                                SetTime(waitTime + killTime);
                            }
                        }

                        if (Time > waitTime + killTime + 30) {
                            NPC.dontTakeDamage = false;

                            if (NPC.life > (NPC.lifeMax * 0.15f)) {
                                SetAttack(AttackList.FusionRay, true);
                            }
                            else {
                                SetAttack(AttackList.BurstLightning, true);
                            }
                        }

                        break;

                    case (int)AttackList.FusionRay:

                        SortedProjectileAttack(Target.Center, SortedProjectileAttackTypes.FusionRay);

                        Fly();

                        NPC.damage = 0;

                        if (Time > 1400) {
                            SetAttack(AttackList.BurstLightning, true);
                        }

                        break;

                    default:

                        SetAttack(AttackList.BurstLightning, true);

                        break;
                }

                if (NPC.life < NPC.lifeMax * 0.1f) {
                    NPC.defense = 200;
                    NPC.takenDamageMultiplier = 0.9f;

                    if (Attack != (int)AttackList.FusionRay) {
                        SetPhase(-2);
                    }
                }

                break;

            case -2:

                NPC.defense = 175;
                NPC.takenDamageMultiplier = 0.75f;

                Fly();
                NPC.velocity *= 1.0002f;
                int randCount = Main.rand.Next(60, 80);
                if (Time % randCount > 5 && Time % randCount < 10) {
                    SetTime(10);

                    SoundStyle spawn = AssetDirectory.Sounds.Goozma.SpawnSlime;
                    SoundEngine.PlaySound(spawn, NPC.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient) {
                        int killTime = 180;
                        Vector2 velocity = NPC.DirectionFrom(Target.Center).SafeNormalize(Vector2.Zero).RotatedByRandom(0.1f * NPC.direction);
                        NPC goozmite = NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 40, ModContent.NPCType<Goozmite>(), ai1: killTime, ai2: NPC.whoAmI);
                        goozmite.velocity = velocity * Main.rand.Next(20, 70);
                        goozmite.ai[1] = killTime;
                        goozmite.localAI[0] = NPC.localAI[0] + 20f;
                        goozmite.lifeMax = (int)(goozmite.lifeMax * 0.33f);
                        goozmite.life = goozmite.lifeMax;
                    }
                    
                }

                SortedProjectileAttack(Target.Center, SortedProjectileAttackTypes.FusionRay);

                break;

            case 3:

                NPC.velocity.X = 0;
                NPC.velocity.Y = Utils.GetLerpValue(10, 300, Time, true) * -1f;

                if (Main.netMode != NetmodeID.MultiplayerClient) {
                    NPC.Center += Main.rand.NextVector2Circular(4, 4) * Utils.GetLerpValue(10, 300, Time, true);
                    NPC.netUpdate = true;
                }


                NPC.dontTakeDamage = true;
                NPC.life = 1;

                if (Main.netMode != NetmodeID.MultiplayerClient && Time % 3 == 0) {
                    foreach (Projectile projectile in Main.projectile.Where(n => n.active && n.ModProjectile is ISubjectOfNPC<Goozma>)) {
                        projectile.Kill();
                    }
                }

                FocusConditional.focusTarget = NPC.Center;

                if (Time >= 1 && !initializedDeathRoar) {
                    Main.instance.CameraModifiers.Add(new FocusConditional(50, 50, () => NPC.active, "Goozma"));

                    if (Main.netMode != NetmodeID.MultiplayerClient) {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GoozmaDeathGodrays>(), 0, 0, ai1: NPC.whoAmI);
                    }

                    SoundStyle deathRoar = AssetDirectory.Sounds.Goozma.DeathBuildup;
                    SoundEngine.PlaySound(deathRoar, NPC.Center);

                    initializedDeathRoar = true;
                }

                if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(2 + (int)(60 - Time / 5f))) {
                    for (int i = 0; i < Main.rand.Next(1, 4); i++) {
                        CalamityHunt.particles.Add(Particle.Create<ChromaticGelChunk>(particle => {
                            particle.position = NPC.Center + Main.rand.NextVector2Circular(10, 10);
                            particle.velocity = Main.rand.NextVector2Circular(20, 20) - Vector2.UnitY * 10f;
                            particle.scale = Main.rand.NextFloat(0.5f, 2f);
                            particle.color = Color.White;
                            particle.colorData = new ColorOffsetData(true, NPC.localAI[0]);
                        }));
                    }
                }

                if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(1 + (int)(65 - Time / 4.66f)) || Time > 250) {
                    CalamityHunt.particles.Add(Particle.Create<ChromaticGooBurst>(particle => {
                        particle.velocity = Main.rand.NextVector2CircularEdge(1, 2);
                        particle.position = NPC.Center + particle.velocity * Main.rand.NextFloat(4, 16) * NPC.scale;
                        particle.scale = Main.rand.NextFloat(0.5f, 1.5f);
                        particle.color = Color.White;
                        particle.colorData = new ColorOffsetData(true, NPC.localAI[0] + Main.rand.NextFloat(0.2f, 0.5f));
                    }));
                }

                //if (Time % 3 == 0) {
                //    Color glowColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(NPC.localAI[0] + Main.rand.NextFloat(0.1f));
                //    Vector2 off = Main.rand.NextVector2Circular(150, 220);
                //    float sparkleScale = Utils.GetLerpValue(200, 0, off.Length() + Main.rand.Next(-10, 10));
                //    Particle.NewParticle(ModContent.GetInstance<CrossSparkle>(), NPC.Center + off, Vector2.Zero, glowColor, sparkleScale * 2f);
                //}

                if (Time > 250) {
                    ScreenDarkness.frontColor = Color.White;
                    ScreenDarkness.screenObstruction = Utils.GetLerpValue(255, 290, Time, true);
                }

                if (Time > 290) {
                    headScale += 0.11f;
                }

                if (Time > 300) {

                    Main.musicFade[Main.curMusic] = 0f;
                    Main.curMusic = -1;

                    NPC.life = 0;
                    NPC.HitEffect();
                    NPC.NPCLoot();
                    NPC.active = false;
                    NPC.netUpdate = true;
                    NPC.justHit = true;

                    if (Main.netMode != NetmodeID.MultiplayerClient) {

                        //NPC.checkDead();
                    }
                    else OnKillClientSide();


                }

                if (!Main.expertMode && !Main.masterMode) {
                    if (NPC.ai[3] > -1 && NPC.ai[3] <= Main.maxNPCs) {
                        if (ActiveSlime.active) {
                            ActiveSlime.active = false;
                        }
                    }

                    if (Time < 15) {
                        KillSlime(currentSlime);
                    }
                }

                break;

            case -5:

                NPC.velocity *= 0.8f;
                NPC.velocity.Y -= (-31 - Time) * 0.025f;
                NPC.scale *= 0.9999f;
                NPC.dontTakeDamage = true;

                if (Time > 0) {
                    SetTime(0);
                }

                if (Time == -1) {
                    if (NPC.ai[3] > -1 && NPC.ai[3] <= Main.maxNPCs) {
                        if (ActiveSlime.active) {
                            ActiveSlime.active = false;
                        }
                    }
                }

                if (Time > -15) {
                    KillSlime(currentSlime);
                }

                if (Time < -30) {
                    NPC.EncourageDespawn(30);
                }

                if (Time < -300) {
                    NPC.active = false;
                }

                if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(3)) {

                    CalamityHunt.particles.Add(Particle.Create<ChromaticGooBurst>(particle => {
                        particle.velocity = Main.rand.NextVector2CircularEdge(2, 3);
                        particle.position = NPC.Center + particle.velocity * Main.rand.NextFloat(4, 16) * NPC.scale;
                        particle.scale = Main.rand.NextFloat(0.75f, 1.75f);
                        particle.color = Color.White;
                        particle.colorData = new ColorOffsetData(true, NPC.localAI[0] + Main.rand.NextFloat(0.2f, 0.5f));
                    }));
                }

                break;

            case -21:

                Attack = (int)AttackList.Shimmering;

                NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.UnitY * -8, 0.2f) * Utils.GetLerpValue(60, 20, Time, true);

                NPC.Center += Main.rand.NextVector2Circular(15, 15) * Utils.GetLerpValue(0, 100, Time, true);
                NPC.netUpdate = true;

                if (Time < 15) {
                    KillSlime(currentSlime);
                }

                if (Time > 80) {
                    NPC.active = false;
                    foreach (NPC left in Main.npc.Where(n => !n.active)) {
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

                //Test phase

                NPC.dontTakeDamage = false;
                NPC.velocity *= 0.5f;

                if (Main.mouseRight) {
                    NPC.velocity += NPC.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero) * NPC.Distance(Main.MouseWorld) * 0.1f;
                }

                if (NPC.velocity.Length() > 41) {
                    rotate = true;
                    NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.ToRotation() + MathHelper.PiOver2, 0.4f);
                }

                if (Time % 120 == 0) {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero), ModContent.ProjectileType<GooLightning>(), GetDamage(3), 0, -1, -50, 1500, 0);
                }

                break;

            default:

                SetPhase(-5);

                break;
        };

        HandleLoopedSounds();

        if (Phase == -5) {
            Time--;
        }
        else {
            Time++;
        }

        if (!Main.dedServ) {

            oldVel ??= new Vector2[NPCID.Sets.TrailCacheLength[Type]];
            NPC.localAI[1] -= NPC.velocity.X * 0.001f + NPC.direction * 0.02f;

            for (int i = NPCID.Sets.TrailCacheLength[Type] - 1; i > 0; i--) {
                NPC.oldPos[i] = Vector2.Lerp(NPC.oldPos[i], NPC.oldPos[i - 1], 0.5f + (float)Math.Sin(NPC.localAI[0] * 0.5f - i * 2f) * 0.4f);
                NPC.oldRot[i] = MathHelper.Lerp(NPC.oldRot[i], NPC.oldRot[i - 1], 0.5f + (float)Math.Sin(NPC.localAI[0] * 0.5f - i * 2f) * 0.4f);
                oldVel[i] = Vector2.Lerp(oldVel[i], oldVel[i - 1], 0.5f + (float)Math.Sin(NPC.localAI[0] * 0.5f - i * 2f) * 0.4f);
            }
            NPC.oldPos[0] = Vector2.Lerp(NPC.oldPos[0], NPC.position, 0.5f + (float)Math.Sin(NPC.localAI[0] * 0.5f) * 0.4f);
            NPC.oldRot[0] = MathHelper.Lerp(NPC.oldRot[0], NPC.rotation, 0.5f + (float)Math.Sin(NPC.localAI[0] * 0.5f) * 0.4f);
            oldVel[0] = Vector2.Lerp(oldVel[0], NPC.velocity, 0.5f + (float)Math.Sin(NPC.localAI[0] * 0.5f) * 0.4f);

            //for (int i = NPCID.Sets.TrailCacheLength[Type] - 1; i > 0; i--)
            //{
            //    NPC.oldPos[i] = Vector2.Lerp(NPC.oldPos[i], NPC.oldPos[i - 1], 0.3f);
            //    NPC.oldRot[i] = MathHelper.Lerp(NPC.oldRot[i], NPC.oldRot[i - 1], 0.3f);
            //    oldVelocity[i] = Vector2.Lerp(oldVelocity[i], oldVelocity[i - 1], 0.3f);
            //}
            //NPC.oldPos[0] = Vector2.Lerp(NPC.oldPos[0], NPC.position, 0.3f);
            //NPC.oldRot[0] = MathHelper.Lerp(NPC.oldRot[0], NPC.rotation, 0.3f);
            //oldVelocity[0] = Vector2.Lerp(oldVelocity[0], NPC.velocity, 0.3f);

            if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(3)) {
                Dust dust = Dust.NewDustDirect(NPC.Center - new Vector2(50), 100, 160, DustID.TintableDust, Main.rand.NextFloat(-1f, 1f), -4f, 230, Color.Black, Main.rand.NextFloat(2f));
                dust.noGravity = true;
            }

            if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(6) || (Phase == 2 && Main.rand.NextBool()) || (Phase == -22 && Main.rand.NextBool())) {
                CalamityHunt.particles.Add(Particle.Create<LightningParticle>(particle => {
                    particle.position = NPC.Center + Main.rand.NextVector2Circular(80, 130).RotatedBy(NPC.rotation) * NPC.scale;
                    particle.velocity = particle.position.DirectionFrom(NPC.Center) * Main.rand.NextFloat(0.5f, 2f);
                    particle.rotation = particle.velocity.ToRotation() + Main.rand.NextFloat(-0.1f, 0.1f);
                    particle.scale = Main.rand.NextFloat(0.6f, 1.5f);
                    particle.color = (new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(NPC.localAI[0]) * 1.5f) with { A = 128 };
                    particle.maxTime = Main.rand.Next(4, 15);
                    particle.anchor = () => NPC.velocity * 0.9f;
                }));
            }
        }

        //shimmer
        if (Phase != -21 && Phase != -1 && Main.tile[NPC.Center.ToTileCoordinates()].LiquidType == LiquidID.Shimmer) {
            Phase = -21;
            Time = 0;
            for (int i = 0; i < 4; i++) {
                KillSlime(currentSlime);
            }

            ActiveSlime.active = false;
        }

        if (hitTimer > 0) {
            hitTimer--;
        }
        hitDirection = Vector2.Lerp(hitDirection, Vector2.Zero, 0.05f + Utils.GetLerpValue(10, 0, hitTimer, true) * 0.1f);
        if (hitDirection.HasNaNs() || hitDirection.Length() > 10) {
            hitDirection = Vector2.Zero;
        }

        //if (Phase != -1) {
        //    Phase = -22;
        //}
    }

    private void KillSlime(int index)
    {
        Vector2 velocity = Main.rand.NextVector2Circular(8, 1) - Vector2.UnitY * Main.rand.NextFloat(7f, 16f);
        Vector2 position = ActiveSlime.Center + Main.rand.NextVector2Circular(1, 50) + new Vector2(velocity.X * 12f, 32f);

        for (int i = 0; i < Main.rand.Next(4, 9); i++) {
            switch (index) {
                case 0:
                    if (Main.netMode != NetmodeID.Server) {
                        CalamityHunt.particles.Add(Particle.Create<EbonGelChunk>(particle => {
                            particle.position = position;
                            particle.velocity = velocity;
                            particle.scale = Main.rand.NextFloat(0.1f, 2.1f);
                            particle.color = Color.White;
                        }));
                    }


                    break;

                case 1:
                    if (Main.netMode != NetmodeID.Server) {
                        CalamityHunt.particles.Add(Particle.Create<DivineGelChunk>(particle => {
                            particle.position = position;
                            particle.velocity = velocity;
                            particle.scale = Main.rand.NextFloat(0.1f, 2.1f);
                            particle.color = Color.White;
                        }));
                    }


                    break;

                case 2:

                    velocity = Main.rand.NextVector2Circular(8, 1) - Vector2.UnitY * Main.rand.NextFloat(7f, 18f);
                    position = ActiveSlime.Center + Main.rand.NextVector2Circular(1, 50) + new Vector2(velocity.X * 16f, 32f);

                    if (Main.netMode != NetmodeID.Server) {
                        CalamityHunt.particles.Add(Particle.Create<CrimGelChunk>(particle => {
                            particle.position = position;
                            particle.velocity = velocity;
                            particle.scale = Main.rand.NextFloat(0.1f, 2.1f);
                            particle.color = Color.White;
                        }));
                    }


                    break;

                case 3:

                    if (Main.netMode != NetmodeID.Server) {
                        CalamityHunt.particles.Add(Particle.Create<StellarGelChunk>(particle => {
                            particle.position = position;
                            particle.velocity = velocity;
                            particle.scale = Main.rand.NextFloat(0.1f, 2.1f);
                            particle.color = Color.White;
                        }));
                    }


                    break;
            }
        }
    }

    private int hitTimer;
    private Vector2 hitDirection;

    public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
    {
        if (hitTimer <= 0) {
            hitTimer += Main.rand.Next(10, 18);
            hitDirection = player.DirectionTo(NPC.Center) * 10;
            SoundEngine.PlaySound(AssetDirectory.Sounds.Goozma.Hurt, NPC.Center);

            if (Main.netMode != NetmodeID.Server) {
                for (int i = 0; i < Main.rand.Next(1, 6); i++) {
                    CalamityHunt.particles.Add(Particle.Create<ChromaticGelChunk>(particle => {
                        particle.position = NPC.Center + Main.rand.NextVector2Circular(40, 50);
                        particle.velocity = NPC.DirectionFrom(player.Center).RotatedByRandom(0.4f) * Main.rand.Next(4, 10);
                        particle.scale = Main.rand.NextFloat(0.5f, 1.5f);
                        particle.color = Color.White;
                        particle.colorData = new ColorOffsetData(true, NPC.localAI[0]);
                    }));
                }
            }

        }
    }

    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        if (hitTimer <= 0) {
            hitTimer += Main.rand.Next(10, 18);
            hitDirection = projectile.DirectionTo(NPC.Center) * 10;
            SoundEngine.PlaySound(AssetDirectory.Sounds.Goozma.Hurt, NPC.Center);

            if (Main.netMode != NetmodeID.Server) {
                for (int i = 0; i < Main.rand.Next(1, 6); i++) {
                    CalamityHunt.particles.Add(Particle.Create<ChromaticGelChunk>(particle => {
                        particle.position = NPC.Center + Main.rand.NextVector2Circular(40, 50);
                        particle.velocity = NPC.DirectionFrom(projectile.Center).RotatedByRandom(0.4f) * Main.rand.Next(4, 10);
                        particle.scale = Main.rand.NextFloat(0.5f, 1.5f);
                        particle.color = Color.White;
                        particle.colorData = new ColorOffsetData(true, NPC.localAI[0]);
                    }));
                }
            }
        }
    }

    public void OnKillClientSide()
    {
        for (int i = 0; i < Main.musicFade.Length; i++) {
            Main.musicFade[i] = 0.2f;
        }

        goozmaWarble.StopSound();
        goozmaShoot.StopSound();
        goozmaSimmer.StopSound();

        SoundEngine.PlaySound(AssetDirectory.Sounds.Goozma.Pop, NPC.Center);
        SoundEngine.PlaySound(AssetDirectory.Sounds.Goozma.Explode.WithPitchOffset(0.2f).WithVolumeScale(0.9f), NPC.Center);

        Main.windSpeedTarget = 0f;
        Main.windSpeedCurrent = MathHelper.Lerp(Main.windSpeedCurrent, 0, 0.7f);

        if (Main.netMode != NetmodeID.Server) {
            for (int i = 0; i < 200; i++) {
                CalamityHunt.particles.Add(Particle.Create<ChromaticGelChunk>(particle => {
                    particle.position = NPC.Center + Main.rand.NextVector2Circular(20, 20);
                    particle.velocity = Main.rand.NextVector2Circular(30, 30) - Vector2.UnitY * 15f;
                    particle.scale = Main.rand.NextFloat(0.1f, 2.1f);
                    particle.color = Color.White;
                    particle.colorData = new ColorOffsetData(true, NPC.localAI[0]);
                }));
            }
        }
    }

    public override void OnKill()
    {
        if (ModLoader.HasMod(HUtils.CalamityMod)) {
            ModLoader.GetMod(HUtils.CalamityMod).Call("SendNPCAlert", new int[] { NPCID.Stylist }, BossDownedSystem.Instance.GoozmaDowned);
        }
        BossDownedSystem.Instance.GoozmaDowned = true;

        //Main.StopSlimeRain();

        Main.windSpeedTarget = 0f;
        Main.windSpeedCurrent = MathHelper.Lerp(Main.windSpeedCurrent, 0, 0.7f);

        if (Main.netMode == NetmodeID.Server) {
            NetMessage.SendData(MessageID.WorldData);
        }

        //for (int i = 0; i < Main.musicFade.Length; i++) {
        //    Main.musicFade[i] = 0.2f;
        //}

        //goozmaWarble.StopSound();
        //goozmaShoot.StopSound();
        //goozmaSimmer.StopSound();



    }

    private void Fly()
    {
        if (NPC.Distance(Target.Center) < 300) {
            NPC.velocity *= 0.93f;
        }

        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * Math.Max(0, NPC.Distance(Target.Center) - 300) * 0.04f, 0.02f);
    }

    private void FlyTo(Vector2 target)
    {
        if (NPC.Distance(target) < 20) {
            NPC.velocity *= 0.9f;
        }

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
        if (Time > time - 6) {
            power = 0.2f;
        }
        else if (Time > time - 15) {
            NPC.velocity *= 0.9f;
        }

        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero) * Math.Clamp(NPC.Distance(targetPos) * power * 1.5f, 20, 100), power > 0.05f ? 0.1f : 0.03f);

        if (Time > time) {
            SetTime(0);
        }
    }

    private int randDirection;

    private void Orbit(int frequency, Vector2 startingVector)
    {
        Vector2 target = Target.Center + startingVector.RotatedBy(Time % frequency / (float)frequency * MathHelper.TwoPi);

        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(target).SafeNormalize(Vector2.Zero) * NPC.Distance(target) * 0.1f, 0.1f);
    }

    private enum SortedProjectileAttackTypes
    {
        None,
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
        if (Phase == 3) {
            volumeScale *= Utils.GetLerpValue(140, 0, Time, true);
        }

        goozmaWarbleVolume = MathHelper.Lerp(goozmaWarbleVolume, Math.Clamp(1f - Main.LocalPlayer.Distance(NPC.Center) * 0.0001f, 0, 1) * NPC.scale * volumeScale + NPC.velocity.Length() * 0.2f, 0.1f);
        goozmaWarblePitch = MathHelper.Lerp(goozmaWarblePitch, Math.Clamp(NPC.velocity.Length() * 0.02f - Main.LocalPlayer.Distance(NPC.Center) * 0.0001f, -0.8f, 0.8f), 0.1f);

        goozmaWarble ??= new LoopingSound(AssetDirectory.Sounds.Goozma.WarbleLoop, new HUtils.NPCAudioTracker(NPC).IsActiveAndInGame);
        goozmaWarble.PlaySound(() => NPC.Center, () => goozmaWarbleVolume, () => goozmaWarblePitch);
    }

    public static LoopingSound goozmaShoot;
    public static float goozmaShootPowerCurrent;
    public static float goozmaShootPowerTarget;

    public void HandleGoozmaShootSound()
    {
        goozmaShootPowerCurrent = MathHelper.Lerp(goozmaShootPowerCurrent, goozmaShootPowerTarget, 0.2f);
        if (goozmaShootPowerCurrent < 0.05f) {
            goozmaShootPowerCurrent = 0f;
        }

        if (Phase == 1) {
            goozmaShootPowerCurrent = Utils.GetLerpValue(340, 510, Time, true) * 1.2f;
        }

        if (goozmaShoot == null) {
            goozmaShoot = new LoopingSound(AssetDirectory.Sounds.Goozma.ShootLoop, () => new HUtils.NPCAudioTracker(NPC).IsActiveAndInGame() && goozmaShootPowerCurrent > 0.05f);
        }

        goozmaShoot.PlaySound(() => NPC.Center, () => goozmaShootPowerCurrent * 1.5f, () => 0f);

        goozmaShootPowerTarget -= 0.05f;
        if (goozmaShootPowerTarget < 0.1f) {
            goozmaShootPowerTarget = 0f;
        }
    }

    public static LoopingSound goozmaSimmer;
    public static float goozmaSimmerVolume;
    public static float goozmaSimmerPitch;

    public void HandleGoozmaSimmerSound()
    {
        float pitch = 0f;
        float volume = 0f;
        if (Phase == 1) {
            pitch = Utils.GetLerpValue(330, 550, Time, true);
            volume = 0.5f + Utils.GetLerpValue(330, 550, Time, true) * 0.6f;
        }

        goozmaSimmerVolume = Math.Clamp(1f - Main.LocalPlayer.Distance(NPC.Center) * 0.00001f, 0, 1) * volume;
        goozmaSimmerPitch = pitch;

        if (goozmaSimmer == null) {
            goozmaSimmer = new LoopingSound(AssetDirectory.Sounds.Goozma.SimmerLoop, new HUtils.NPCAudioTracker(NPC).IsActiveAndInGame);
        }

        goozmaSimmer.PlaySound(() => NPC.Center, () => goozmaSimmerVolume, () => goozmaSimmerPitch);
    }

    private void SortedProjectileAttack(Vector2 targetPos, SortedProjectileAttackTypes type)
    {
        SoundStyle fizzSound = AssetDirectory.Sounds.Goozma.SlimeShoot;
        SoundStyle bloatSound = AssetDirectory.Sounds.Goozma.BloatedBlastShoot with { PitchVariance = 0.15f };
        SoundStyle dartSound = AssetDirectory.Sounds.Goozma.Dart with { Volume = 0.66f };
        SoundStyle fireballSound = AssetDirectory.Sounds.Goozma.RainbowShoot;
        SoundStyle fusionSound = AssetDirectory.Sounds.Goozma.Shot;

        switch (type) {
            case SortedProjectileAttackTypes.None:

                break;

            case SortedProjectileAttackTypes.EbonianBubbles:

                if (NPC.Distance(Target.Center) > 300) {
                    if (Time % 15 == 0) {
                        SoundEngine.PlaySound(fizzSound, NPC.Center);
                        goozmaShootPowerTarget = 1f;
                    }

                    float angle = MathHelper.SmoothStep(1.5f, 0.9f, Time / 350f);
                    if (Time % 6 == 0) {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitY.RotatedBy(angle).RotatedByRandom(0.3f) * Main.rand.Next(4, 7), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitY.RotatedBy(-angle).RotatedByRandom(0.3f) * Main.rand.Next(4, 7), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                    }
                    if ((Time + 6) % 6 == 0) {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitY.RotatedBy(angle + 0.3f).RotatedByRandom(0.3f) * Main.rand.Next(2, 4), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitY.RotatedBy(-angle - 0.3f).RotatedByRandom(0.3f) * Main.rand.Next(2, 4), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                    }
                    if (Target.Center.Y < NPC.Top.Y - 300) {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * Main.rand.Next(2, 8), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                    }
                }

                break;

            case SortedProjectileAttackTypes.EbonianTrifecta:

                if (Time >= 180) {
                    if (Time - 180 % 60 == 0) {
                        SoundEngine.PlaySound(fizzSound, NPC.Center);
                        goozmaShootPowerTarget = 1f;

                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(0.3f) * 3f, ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                    }
                }


                break;

            case SortedProjectileAttackTypes.CrystalStorm:

                if (NPC.Distance(Target.Center) > 300) {
                    if (Time % 18 == 0) {
                        SoundEngine.PlaySound(fizzSound, NPC.Center);
                        goozmaShootPowerTarget = 1f;
                    }

                    float angle = MathHelper.SmoothStep(-1.5f, -1.0f, Time / 350f);
                    if (Time % 15 == 0) {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, -Vector2.UnitY.RotatedBy(angle).RotatedByRandom(0.2f) * Main.rand.Next(4, 7), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, -Vector2.UnitY.RotatedBy(-angle).RotatedByRandom(0.2f) * Main.rand.Next(4, 7), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                    }
                    if ((Time + 8) % 8 == 0) {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, -Vector2.UnitY.RotatedBy(angle - 0.3f).RotatedByRandom(0.2f) * Main.rand.Next(2, 4), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, -Vector2.UnitY.RotatedBy(-angle + 0.3f).RotatedByRandom(0.2f) * Main.rand.Next(2, 4), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                    }
                    if (Target.Center.Y > NPC.Top.Y + 300) {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero) * Main.rand.Next(2, 8), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                    }
                }

                break;

            case SortedProjectileAttackTypes.PixieBallDisruption:

                if (Time % 80 == 75) {
                    int count = Main.rand.Next(10, 18);
                    for (int i = 0; i < count; i++) {
                        Projectile dart = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.Next(10, 20), 0).RotatedBy(MathHelper.TwoPi / count * i), ModContent.ProjectileType<BloatedBlast>(), GetDamage(1), 0);
                        dart.ai[1] = 1;
                        dart.localAI[0] = NPC.localAI[0] + i / (float)count * 90f;
                    }
                    SoundEngine.PlaySound(dartSound, NPC.Center);
                }

                break;

            case SortedProjectileAttackTypes.CrimulanSlam:

                if (NPC.Distance(Target.Center) > 300) {
                    if (Time % 25 == 0) {
                        SoundEngine.PlaySound(fizzSound, NPC.Center);
                        goozmaShootPowerTarget = 1f;
                    }

                    if ((Time + Main.rand.Next(0, 2)) % 8 == 0) {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(0.15f).RotatedBy(0.8f) * 20f, ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                    }

                    if ((Time + Main.rand.Next(0, 2)) % 8 == 0) {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(0.15f).RotatedBy(-0.8f) * 20f, ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                    }
                }

                break;

            case SortedProjectileAttackTypes.CrimulanHop:

                if (Time % 80 < 20 && Time % 5 == 1) {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(1f), ModContent.ProjectileType<RainbowLaser>(), GetDamage(1), 0, ai0: -Main.rand.Next(30, 35), ai1: NPC.whoAmI);
                }

                if (Time % 80 < 50 && Time % 15 == 5) {
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
                if ((Time - 75) % 160 > 80 && (Time - 75) % 15 == 10) {
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

                if (Time % 130 == 10) {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(1.3f) * NPC.Distance(targetPos) * 0.0016f * Main.rand.NextFloat(0.9f, 1.2f), ModContent.ProjectileType<SlimeBomb>(), GetDamage(2), 0);
                }

                break;

            case SortedProjectileAttackTypes.BurstLightning:

                if (Time % 170 > 130) {
                    int freq = (int)DifficultyBasedValue(10, 9, 7, 6);
                    if ((Time % 170) % freq == 0) {
                        SoundEngine.PlaySound(fireballSound, NPC.Center);

                        Projectile fireball = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero).RotatedByRandom(0.3f).RotatedBy(1f * NPC.direction) * 16f, ModContent.ProjectileType<RainbowBall>(), GetDamage(4), 0);
                        fireball.localAI[0] = NPC.localAI[0];
                    }
                }
                else if (Time % 20 == 0) {
                    SoundEngine.PlaySound(fizzSound, NPC.Center);
                    goozmaShootPowerTarget = 1f;

                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(1f), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                }

                if (Time % 270 == 0) {
                    SoundEngine.PlaySound(bloatSound, NPC.Center);

                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero) * 10, ModContent.ProjectileType<BloatedBlast>(), GetDamage(5), 0);
                }

                if (Time % 140 == 0) {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionFrom(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(2.5f), ModContent.ProjectileType<GooLightning>(), GetDamage(4), 0, -1, -50, 1500, 1);
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero), ModContent.ProjectileType<GooLightning>(), GetDamage(3), 0, -1, -50, 1500, 0);
                }

                if (Time % 55 == 0 && !Main.rand.NextBool(20)) {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionFrom(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(2.5f), ModContent.ProjectileType<GooLightning>(), GetDamage(4), 0, -1, -50, 1500, 1);
                }

                break;

            case SortedProjectileAttackTypes.DrillDash:

                int dashTime = (int)DifficultyBasedValue(110, 100, 90, 80);

                if (Time % dashTime > 24) {
                    if (Time % dashTime == 25) {
                        SoundEngine.PlaySound(fizzSound, NPC.Center);
                        goozmaShootPowerTarget = 1f;
                    }
                    if (Time % dashTime < 30) {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(1f) * 2.5f, ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                    }
                }

                if (Time % 30 == 5) {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(0.1f) * 10f, ModContent.ProjectileType<SlimeBomb>(), GetDamage(2), 0);
                }

                if (Time % dashTime == 70) {
                    SoundEngine.PlaySound(dartSound, NPC.Center);

                    int count = Main.rand.Next(18, 25);
                    for (int i = 0; i < count; i++) {
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

                if (Phase == -2) {
                    if (Time == 2) {
                        Projectile fusionRay = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero), ModContent.ProjectileType<FusionRay>(), GetDamage(7), 0);
                        fusionRay.ai[0] = 200;
                        fusionRay.ai[1] = 1;
                        fusionRay.ai[2] = NPC.whoAmI;
                    }
                }
                else {
                    if (Time < 350 && Time % 18 < 3) {
                        if (Time % 18 == 1) {
                            SoundEngine.PlaySound(fizzSound, NPC.Center);
                            goozmaShootPowerTarget = 1f;
                        }

                        for (int i = 0; i < Main.rand.Next(2, 4); i++) {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero).RotatedByRandom(0.5f) * Main.rand.NextFloat(1.8f, 2.5f), ModContent.ProjectileType<SlimeShot>(), GetDamage(1), 0);
                        }

                        if (Time % 30 == 0) {
                            SoundEngine.PlaySound(fireballSound, NPC.Center);

                            Projectile fireball = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero).RotatedByRandom(0.3f).RotatedBy(1f * NPC.direction) * 16f, ModContent.ProjectileType<RainbowBall>(), GetDamage(4), 0);
                            fireball.localAI[0] = NPC.localAI[0];
                        }
                    }

                    if (Time > 150 && Time <= 450 && Time % 90 == 0) {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero) * 5, ModContent.ProjectileType<BloatedBlast>(), GetDamage(5), 0);
                        SoundEngine.PlaySound(bloatSound, NPC.Center);
                    }

                    if (Time > 250 && Time < 530 && Time % 40 == 0) {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero).RotatedByRandom(1f), ModContent.ProjectileType<GooLightning>(), GetDamage(4), 0, -1, -50, 1500, 1);
                    }

                    if (Time == 250) {
                        Projectile fusionRay = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Target.Center).SafeNormalize(Vector2.Zero), ModContent.ProjectileType<FusionRay>(), GetDamage(7), 0);
                        fusionRay.ai[2] = NPC.whoAmI;
                    }

                    if (Time > 500) {
                        if (Time % 70 == 0) {
                            SoundEngine.PlaySound(bloatSound, NPC.Center);

                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(targetPos).SafeNormalize(Vector2.Zero) * 5, ModContent.ProjectileType<BloatedBlast>(), GetDamage(5), 0);
                        }

                        if (Time % 46 == 30) {
                            Vector2 bombVelocity = HUtils.GetDesiredVelocityForDistance(NPC.Center, targetPos, 0.955f, 40).RotatedByRandom(0.5f) * Main.rand.NextFloat(0.8f, 1.2f);
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, bombVelocity, ModContent.ProjectileType<SlimeBomb>(), GetDamage(5), 0);
                        }
                    }
                }

                break;
        }
    }

    private int GetDamage(int attack, float modifier = 1f)
    {
        int damage = 0;

        if (ModLoader.HasMod(HUtils.CalamityMod)) {
            damage = attack switch
            {
                0 => Main.expertMode ? 600 : 300,//contact
                1 => 75,//slime balls
                2 => 105,//pure gel
                3 => 125, //lightning
                4 => 60, //static
                5 => 100, //bloat
                6 => Main.expertMode ? 400 : 200, //drill dash
                7 => 300, //fusion ray
                _ => 0
            };
        }
        else {
            damage = attack switch
            {
                0 => Main.expertMode ? 100 : 50,//contact
                1 => 20,//slime balls
                2 => 50,//pure gel
                3 => 66, //lightning
                4 => 30, //static
                5 => 50, //bloat
                6 => Main.expertMode ? 180 : 100, //drill dash
                7 => 150, //fusion ray
                _ => 0
            };
        }

        return (int)(damage * modifier);
    }

    private float extraTilt;
    private Vector2 drawOffset;
    private float headScale;
    public Vector2 drawVelocity;
    private Vector2 tentacleVelocity;
    private Vector2 tentacleAcceleration;
    private Vector2[] oldVel;
    public bool rotate;

    public override void FindFrame(int frameHeight)
    {
        NPC.localAI[0]++;

        if (oldVel == null) {
            oldVel = new Vector2[NPCID.Sets.TrailCacheLength[Type]];
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[Type]; i++) {
                oldVel[i] = NPC.velocity;
            }
        }

        drawOffset = new Vector2((float)Math.Sin(NPC.localAI[0] * 0.05f % MathHelper.TwoPi) * 2, (float)Math.Cos(NPC.localAI[0] * 0.025f % MathHelper.TwoPi) * 3);
        float offsetWobble = (float)Math.Cos(NPC.localAI[0] * 0.05f % MathHelper.TwoPi) * 0.07f;

        if (!rotate) {
            NPC.rotation = NPC.rotation.AngleLerp(Math.Clamp(drawVelocity.X * 0.012f, -1f, 1f) - (offsetWobble - 0.1f) * NPC.direction, 0.1f);
        }

        extraTilt = MathHelper.Lerp(extraTilt, Math.Clamp(-drawVelocity.X * 0.025f, -1f, 1f) - 0.01f * NPC.direction, 0.15f);
        //headScale = MathHelper.Lerp(headScale, 1f, 0.05f);

        tentacleVelocity *= 0.93f;
        tentacleAcceleration = (NPC.velocity - oldVel[(int)(oldVel.Length / 3f)]) * 0.1f;
        tentacleVelocity += tentacleAcceleration.RotatedBy(-NPC.rotation) * 0.7f;

        rotate = false;
    }

    private void FadeMusicOut(On_Main.orig_UpdateAudio orig, Main self)
    {
        orig(self);

        if (Main.npc.Any(n => n.active && n.type == ModContent.NPCType<Goozma>())) {
            NPC goozma = Main.npc.FirstOrDefault(n => n.active && n.type == Type);

            if (goozma.ai[2] == 1) {
                for (int i = 0; i < Main.musicFade.Length; i++) {
                    float volume = Main.musicFade[i] * Main.musicVolume * Utils.GetLerpValue(320, 20, goozma.ai[0], true);
                    float tempFade = Main.musicFade[i];
                    Main.audioSystem.UpdateCommonTrackTowardStopping(i, volume, ref tempFade, Main.musicFade[i] > 0.15f && goozma.ai[0] < 320);
                    Main.musicFade[i] = tempFade;
                }
            }
            if (goozma.ai[2] == 3) {
                for (int i = 0; i < Main.musicFade.Length; i++) {
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
        NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
    }

    //helper methods to do syncing
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

    public void SetPhase(int phase, bool zeroTime = true)
    {
        if (Main.netMode != NetmodeID.MultiplayerClient) {
            Phase = phase;
            if (zeroTime) {
                Time = 0;
            }

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
