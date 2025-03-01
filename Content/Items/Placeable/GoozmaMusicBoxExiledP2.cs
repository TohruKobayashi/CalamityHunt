using CalamityHunt.Content.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Placeable
{
    [LegacyName("GoozmaMusicBoxP2")]
    public class GoozmaMusicBoxExiledP2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            MusicLoader.AddMusicBox(Mod, AssetDirectory.Music.ExiledGoozma2, ModContent.ItemType<GoozmaMusicBoxExiledP2>(), ModContent.TileType<GoozmaMusicBoxExiledP2Tile>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<GoozmaMusicBoxExiledP2Tile>(), 0);
        }
    }
}
