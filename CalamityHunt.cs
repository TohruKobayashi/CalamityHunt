using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalamityHunt.Common.Graphics.Skies;
using CalamityHunt.Common.Systems;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Buffs;
using CalamityHunt.Content.Items.Misc;
using CalamityHunt.Content.Items.Weapons.Summoner;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using CalamityHunt.Content.Projectiles.Weapons.Summoner;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content.Sources;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityHunt
{
    public class CalamityHunt : Mod
    {
        public static Mod Instance;

        public static ParticleSystem particles;
        public static ParticleSystem particlesBehindEntities;

        public override void Load()
        {
            Instance = this;

            particles = new ParticleSystem();
            particlesBehindEntities = new ParticleSystem();

            On_Main.UpdateParticleSystems += UpdateParticleSystems;
            On_Main.DrawDust += DrawParticleSystems;
            On_Main.DoDraw_DrawNPCsOverTiles += DrawParticleSystemBehindEntities;

            Ref<Effect> stellarblackhole = new Ref<Effect>(AssetDirectory.Effects.BlackHole.Value);
            Filters.Scene["HuntOfTheOldGods:StellarBlackHole"] = new Filter(new ScreenShaderData(stellarblackhole, "BlackHolePass"), EffectPriority.VeryHigh);
            Filters.Scene["HuntOfTheOldGods:StellarBlackHole"].Load();

            Ref<Effect> pluripotentSpawnDistort = new Ref<Effect>(AssetDirectory.Effects.PluripotentDistortion.Value);
            Filters.Scene["HuntOfTheOldGods:PluripotentSpawn"] = new Filter(new ScreenShaderData(pluripotentSpawnDistort, "DistortionPass"), EffectPriority.Medium);
            Filters.Scene["HuntOfTheOldGods:PluripotentSpawn"].Load();

            Ref<Effect> slimeMonsoonDistort = new Ref<Effect>(AssetDirectory.Effects.SlimeMonsoonDistortion.Value);
            Filters.Scene["HuntOfTheOldGods:SlimeMonsoon"] = new Filter(new ScreenShaderData(slimeMonsoonDistort, "DistortionPass"), EffectPriority.Medium);
            Filters.Scene["HuntOfTheOldGods:SlimeMonsoon"].Load();
            SkyManager.Instance["HuntOfTheOldGods:SlimeMonsoon"] = new SlimeMonsoonSky();
            SkyManager.Instance["HuntOfTheOldGods:SlimeMonsoon"].Load();
        }

        private void UpdateParticleSystems(On_Main.orig_UpdateParticleSystems orig, Main self)
        {
            orig(self);
            particlesBehindEntities.Update();
            particles.Update();
        }

        private void DrawParticleSystems(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            particles.Draw(Main.spriteBatch);
        }

        private void DrawParticleSystemBehindEntities(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            particlesBehindEntities.Draw(Main.spriteBatch);
            orig(self);
        }

        public override void PostSetupContent()
        {
            // Kill Old Duke and inject Goozma into boss rush
            if (ModLoader.TryGetMod(HUtils.CalamityMod, out Mod calamity)) {
                BossRushInjection(calamity);
                Predicate<NPC> hasGobbed = (NPC npc) => npc.HasBuff<Gobbed>();
                Predicate<NPC> hasSwamped = (NPC npc) => npc.HasBuff<Swamped>();
                Predicate<NPC> hasBurn = (NPC npc) => npc.GetGlobalNPC<FusionBurnNPC>().active;
                calamity.Call("RegisterDebuff", "CalamityHunt/Assets/Textures/Buffs/Gobbed", hasGobbed);
                calamity.Call("RegisterDebuff", "CalamityHunt/Assets/Textures/Buffs/Swamped", hasSwamped);
                calamity.Call("RegisterDebuff", "CalamityHunt/Assets/Textures/Buffs/FusionBurn", hasBurn);
            }

            if (ModLoader.TryGetMod("BossChecklist", out Mod bossChecklist)) {
                BossChecklist(bossChecklist);
            }

            if (ModLoader.TryGetMod("SummonersAssociation", out Mod sAssociation)) {
                sAssociation.Call("AddMinionInfo", ModContent.ItemType<SlimeCane>(), ModContent.BuffType<SlimeCaneBuff>(), new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>()
                    {
                        ["ProjID"] = ModContent.ProjectileType<EbonianBlinky>()
                    },
                    new Dictionary<string, object>()
                    {
                        ["ProjID"] = ModContent.ProjectileType<CrimulanClyde>()
                    },
                    new Dictionary<string, object>()
                    {
                        ["ProjID"] = ModContent.ProjectileType<DivinePinky>()
                    },
                    new Dictionary<string, object>()
                    {
                        ["ProjID"] = ModContent.ProjectileType<StellarInky>()
                    },
                    new Dictionary<string, object>()
                    {
                        ["ProjID"] = ModContent.ProjectileType<Goozmoem>()
                    }
                });
            }
        }

        public static void BossRushInjection(Mod cal)
        {
            // Goozma
            List<(int, int, Action<int>, int, bool, float, int[], int[])> brEntries = (List<(int, int, Action<int>, int, bool, float, int[], int[])>)cal.Call("GetBossRushEntries");
            int[] slimeIDs = { ModContent.NPCType<EbonianBehemuck>(), ModContent.NPCType<CrimulanGlopstrosity>(), ModContent.NPCType<DivineGargooptuar>(), ModContent.NPCType<StellarGeliath>(), ModContent.NPCType<Goozmite>() };
            int[] goozmaID = { ModContent.NPCType<Goozma>() };
            Action<int> pr = delegate (int npc)
            {
                SoundStyle roar = AssetDirectory.Sounds.Goozma.Awaken;
                int whomst = Player.FindClosest(new Vector2(Main.maxTilesX, Main.maxTilesY) * 16f * 0.5f, 1, 1);
                Player guy = Main.player[whomst];
                SoundEngine.PlaySound(roar, guy.Center);
                NPC.SpawnOnPlayer(whomst, ModContent.NPCType<Goozma>());
            };
            int ODID = cal.Find<ModNPC>("OldDuke").Type;
            int InsertID = cal.Find<ModNPC>("SupremeCalamitas").Type;

            for (int i = 0; i < brEntries.Count(); i++) {
                if (brEntries[i].Item1 == ODID) {
                    brEntries.RemoveAt(i);
                    ODID = i;
                }
                if (brEntries[i].Item1 == InsertID) {
                    InsertID = i;
                }
            }

            brEntries.Insert(InsertID + 1, (ModContent.NPCType<Goozma>(), -1, pr, 180, true, 0f, slimeIDs, goozmaID));
            cal.Call("SetBossRushEntries", brEntries);
        }

        public void BossChecklist(Mod bossChecklist)
        {
            int sludge = ModContent.ItemType<OverloadedSludge>();
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                sludge = ModLoader.GetMod(HUtils.CalamityMod).Find<ModItem>("OverloadedSludge").Type;
            }
            Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                Texture2D texture = AssetDirectory.Textures.Goozma.BossChecklistPortrait.Value;
                Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                sb.Draw(texture, centered, color);
            };
            bossChecklist.Call("LogBoss", this, "Goozma", 23.6, () => BossDownedSystem.Instance.GoozmaDowned, ModContent.NPCType<Goozma>(), new Dictionary<string, object>()
            {
                ["spawnItems"] = sludge,
                ["customPortrait"] = portrait,
                ["despawnMessage"] = DespawnMessage
            });
        }

        Func<NPC, LocalizedText> DespawnMessage = delegate (NPC npc)
        {
            int numberOfAdjectives = 34;
            int numberOfNouns = 29;
            int numberOfSpecial = 16;
            int specialChance = 40;

            string adjective = "Mods.CalamityHunt.NPCs.Goozma.Titles.Adjective" + Main.rand.Next(1, numberOfAdjectives + 1);
            string noun = "Mods.CalamityHunt.NPCs.Goozma.Titles.Noun" + Main.rand.Next(1, numberOfNouns + 1);
            LocalizedText final = Language.GetText("Mods.CalamityHunt.NPCs.Goozma.BossChecklistIntegration.DespawnMessage").WithFormatArgs(Language.GetText(adjective), Language.GetText(noun));
            if (Main.rand.NextBool(specialChance)) {
                string special = "Mods.CalamityHunt.NPCs.Goozma.Titles.Specific" + Main.rand.Next(1, numberOfSpecial + 1);
                final = Language.GetText("Mods.CalamityHunt.NPCs.Goozma.BossChecklistIntegration.DespawnMessage").WithFormatArgs(Language.GetText(special), Language.GetOrRegister(""));
            }
            return final;
        };

        public override IContentSource CreateDefaultContentSource()
        {
            SmartContentSource source = new SmartContentSource(base.CreateDefaultContentSource());
            source.AddDirectoryRedirect("Content", "Assets/Textures");
            source.AddDirectoryRedirect("Common", "Assets/Textures");
            return source;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            PacketType packet = (PacketType)reader.ReadByte();
            switch (packet) {
                case PacketType.TrollPlayer:
                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Got packet to epically troll someone"), Color.White);
                    int playerNum = reader.ReadByte();
                    int projNum = reader.ReadByte();
                    ref Player player = ref Main.player[playerNum];

                    if (player.difficulty != 1 && player.difficulty != 2) {
                        player.DropItems();
                    }

                    player.ghost = true;
                    player.statLife = 0;
                    player.KillMe(PlayerDeathReason.ByProjectile(playerNum, projNum), 1, 0);
                    break;
            }
        }

        public enum PacketType : byte
        {
            TrollPlayer
        }
    }
}
