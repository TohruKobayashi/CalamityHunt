using CalamityHunt.Common.DropRules;
using CalamityHunt.Common.Systems;
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
            if (npc.type == NPCID.QueenSlimeBoss) {
                npcLoot.Add(ItemDropRule.ByCondition(new RemixWorldDropRule(), ModContent.ItemType<SludgeFocus>()));
            }
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                Mod cal = ModLoader.GetMod(HUtils.CalamityMod);
                if (npc.type == cal.Find<ModNPC>("Yharon").Type) {
                    npcLoot.Add(ItemDropRule.ByCondition(new YharonSoulDropRule(), ModContent.ItemType<YharonSoul>()));
                }
            }
        }
    }
}
