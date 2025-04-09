using CalamityHunt.Common.Players;
using CalamityHunt.Content.Items.Misc;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems
{
    public class TreeShake : GlobalTile
    {
        public override void PreShakeTree(int x, int y, TreeTypes treeType)
        {
            Player player = Main.LocalPlayer;
            if (player.GetModPlayer<SplendorJamPlayer>().active && player.GetModPlayer<ShogunArmorPlayer>().active && GoozmaSystem.GoozmaActive) {
                if (Main.netMode != NetmodeID.MultiplayerClient && WorldGen.genRand.NextBool(22)) {
                    Item.NewItem(new EntitySource_TileBreak(x, y), x * 16, y * 16, 16, 16, ModContent.ItemType<BadApple>());
                }
            }
        }
    }
}
