using CalamityHunt.Content.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Placeable
{
    public class GoozmaMusicBoxP2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot($"{nameof(CalamityHunt)}/Assets/Music/ViscousDesperation"), ModContent.ItemType<GoozmaMusicBoxP2>(), ModContent.TileType<GoozmaMusicBoxP2Tile>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<GoozmaMusicBoxP2Tile>(), 0);
        }
    }
}