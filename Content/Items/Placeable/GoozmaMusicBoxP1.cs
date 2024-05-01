using CalamityHunt.Content.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Placeable
{
    public class GoozmaMusicBoxP1 : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot($"{nameof(CalamityHunt)}/Assets/Music/GlutinousArbitration"), ModContent.ItemType<GoozmaMusicBoxP1>(), ModContent.TileType<GoozmaMusicBoxP1Tile>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<GoozmaMusicBoxP1Tile>(), 0);
        }
    }
}