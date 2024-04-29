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

                List<ModNPC> plagueEnemies = new List<ModNPC>
                {
                    cal.Find<ModNPC>("Melter"),
                    cal.Find<ModNPC>("PestilentSlime"),
                    cal.Find<ModNPC>("PlaguebringerMiniboss"),
                    cal.Find<ModNPC>("PlagueCharger"),
                    cal.Find<ModNPC>("PlagueChargerLarge"),
                    cal.Find<ModNPC>("Plagueshell"),
                    cal.Find<ModNPC>("Viruling")
                };
                foreach (ModNPC i in plagueEnemies) {
                    if (npc.type == cal.Find<ModNPC>(i.Name).Type) {
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
        }
    }
}
