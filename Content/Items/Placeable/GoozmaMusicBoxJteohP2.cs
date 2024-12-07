using CalamityHunt.Content.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Placeable
{
    public class GoozmaMusicBoxJteohP2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            MusicLoader.AddMusicBox(Mod, AssetDirectory.Music.JteohGoozma2, ModContent.ItemType<GoozmaMusicBoxJteohP2>(), ModContent.TileType<GoozmaMusicBoxJteohP2Tile>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<GoozmaMusicBoxJteohP2Tile>(), 0);
        }
    }
}
