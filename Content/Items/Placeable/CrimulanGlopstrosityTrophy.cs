using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Placeable;

public class CrimulanGlopstrosityTrophy : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Trophies.CrimulanGlopstrosityTrophyTile>());

        Item.width = 32;
        Item.height = 32;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.buyPrice(0, 1);
    }
}
