using CalamityHunt.Content.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Placeable
{
    [LegacyName("GoozmaMusicBoxP1")]
    public class GoozmaMusicBoxExiledP1 : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            MusicLoader.AddMusicBox(Mod, AssetDirectory.Music.ExiledGoozma1, ModContent.ItemType<GoozmaMusicBoxExiledP1>(), ModContent.TileType<GoozmaMusicBoxExiledP1Tile>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<GoozmaMusicBoxExiledP1Tile>(), 0);
        }
    }
}
