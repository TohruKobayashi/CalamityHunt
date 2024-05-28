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
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Player player = Main.LocalPlayer;
            if (player.GetModPlayer<SplendorJamPlayer>().active && player.GetModPlayer<ShogunArmorPlayer>().active && GoozmaSystem.GoozmaActive) {
                if (!effectOnly && !fail && Main.netMode != NetmodeID.MultiplayerClient && TileID.Sets.IsShakeable[type] && WorldGen.genRand.NextBool(22)) {
                    GetTreeBottom(i, j, out int treeX, out int treeY);
                    TreeTypes treeType = WorldGen.GetTreeType(Main.tile[treeX, treeY].TileType);
                    if (treeType != TreeTypes.None) {
                        treeY--;
                        while (treeY > 10 && Main.tile[treeX, treeY].HasTile && TileID.Sets.IsShakeable[Main.tile[treeX, treeY].TileType]) {
                            treeY--;
                        }

                        treeY++;

                        if (WorldGen.IsTileALeafyTreeTop(treeX, treeY) && !Collision.SolidTiles(treeX - 2, treeX + 2, treeY - 2, treeY + 2)) {
                            Item.NewItem(new EntitySource_TileBreak(treeX, treeY), treeX * 16, treeY * 16, 16, 16, ModContent.ItemType<BadApple>());
                        }
                    }
                }
            }
        }
        public static void GetTreeBottom(int i, int j, out int x, out int y)
        {
            x = i;
            y = j;
            Tile tileSafely = Framing.GetTileSafely(x, y);
            if (tileSafely.TileType == TileID.PalmTree) {
                while (y < Main.maxTilesY - 50 && (!tileSafely.HasTile || tileSafely.TileType == TileID.PalmTree)) {
                    y++;
                    tileSafely = Framing.GetTileSafely(x, y);
                }

                return;
            }

            int num = tileSafely.TileFrameX / 22;
            int num2 = tileSafely.TileFrameY / 22;
            if (num == 3 && num2 <= 2) {
                x++;
            }
            else if (num == 4 && num2 >= 3 && num2 <= 5) {
                x--;
            }
            else if (num == 1 && num2 >= 6 && num2 <= 8) {
                x--;
            }
            else if (num == 2 && num2 >= 6 && num2 <= 8) {
                x++;
            }
            else if (num == 2 && num2 >= 9) {
                x++;
            }
            else if (num == 3 && num2 >= 9) {
                x--;
            }

            tileSafely = Framing.GetTileSafely(x, y);
            while (y < Main.maxTilesY - 50 && (!tileSafely.HasTile || TileID.Sets.IsATreeTrunk[tileSafely.TileType] || tileSafely.TileType == TileID.MushroomTrees)) {
                y++;
                tileSafely = Framing.GetTileSafely(x, y);
            }
        }
    }
}
