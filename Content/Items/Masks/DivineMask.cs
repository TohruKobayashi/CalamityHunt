using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Masks
{
    [AutoloadEquip(EquipType.Head)]
    public class DivineMask : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 24;
            Item.vanity = true;
            Item.rare = ItemRarityID.Blue;
        }
    }
}
