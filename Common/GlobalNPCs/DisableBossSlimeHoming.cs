using CalamityHunt.Content.Items.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Common.GlobalNPCs
{
    public class DisableBossSlimeHoming : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void SetDefaults(NPC npc)
        {
            if (npc.type == NPCID.KingSlime || npc.type == NPCID.QueenSlimeBoss) {
                foreach (Player p in Main.player) {
                    if (p == null) continue;
                    if (!p.active) continue;
                    if (!p.HasItem(ModContent.ItemType<GelatinousCatalyst>())) continue;
                    npc.chaseable = false;
                    break;
                }
            }
        }
    }
}
