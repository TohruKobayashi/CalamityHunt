using System.Collections.Generic;
using CalamityHunt.Common.DropRules;
using CalamityHunt.Common.Systems;
using CalamityHunt.Content.Items.Consumable;
using CalamityHunt.Content.Items.Dyes;
using CalamityHunt.Content.Items.Misc;
using CalamityHunt.Content.Items.Misc.AuricSouls;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Common.GlobalNPCs
{
    public class DropAdditions : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public static Condition goozDown = new("After Goozma has been defeated", () => BossDownedSystem.Instance.GoozmaDowned);

        public override void ModifyShop(NPCShop shop)
        {
            int type = shop.NpcType;
            if (type == NPCID.Stylist) {
                shop.Add(ModContent.ItemType<GoopHairDye>(), goozDown);
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            // calamity and calamityless
            if (npc.type == NPCID.QueenSlimeBoss) {
                npcLoot.Add(ItemDropRule.ByCondition(new RemixWorldDropRule(), ModContent.ItemType<SludgeFocus>()));
            }

            // calamity only
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                Mod cal = ModLoader.GetMod(HUtils.CalamityMod);
                if (npc.type == cal.Find<ModNPC>("Yharon").Type) {
                    npcLoot.Add(ItemDropRule.ByCondition(new YharonSoulDropRule(), ModContent.ItemType<YharonSoul>()));
                }

                List<string> plagueEnemies = new List<string>
                {
                    "Melter",
                    "PestilentSlime",
                    "PlaguebringerMiniboss",
                    "PlagueCharger",
                    "PlagueChargerLarge",
                    "Plagueshell",
                    "Viruling"
                };
                foreach (string i in plagueEnemies) {
                    if (npc.type == cal.Find<ModNPC>(i).Type) {
                        npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<NuclearLemonade>(), 25));
                    }
                }
            }
            // calamityless only
            else {
                if (npc.type == NPCID.AngryTrapper) {
                    npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<NuclearLemonade>(), 25));
                }
            }

            // community remix
            if (ModLoader.HasMod(HUtils.CommunityRemix)) {
                Mod ccr = ModLoader.GetMod(HUtils.CommunityRemix);
                List<string> morePlagueEnemies = new List<string> {
                    "EvilPlagueBee",
                    "Miasmius",
                    "PlaguedSpidrone",
                    //"PlagueTrapper",
                    "PlagueEmperor"
                };
                foreach (string i in morePlagueEnemies) {
                    if (npc.type == ccr.Find<ModNPC>(i).Type) {
                        npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<NuclearLemonade>(), 25));
                    }
                }

                if (npc.type == ccr.Find<ModNPC>("Barocrab").Type) {
                    npcLoot.Add(ItemDropRule.Food(ItemID.Fake_ShadowChest, 72, 3011, 4785));
                    npcLoot.Add(ItemDropRule.Food(ItemID.LargeTopaz, 91, 2012, 3046));
                    npcLoot.Add(ItemDropRule.Food(ItemID.GoblinStatue, 44, 768, 4172));
                }
            }
        }
    }
}
