using CalamityHunt.Content.Items.Placeable;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Tiles.Relics;

public class GoozmaRelicTile : RelicTile
{
    public override int PedestalStyle => 1;

    public override int ItemType => ModContent.ItemType<GoozmaRelic>();
}
