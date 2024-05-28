using CalamityHunt.Common.Players;
using CalamityHunt.Content.Items.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Accessories
{
    public class SplendorJam : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 30;
            Item.value = Item.sellPrice(gold: 35);
            Item.accessory = true;
            Item.rare = ModContent.RarityType<VioletRarity>();
        }
        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<SplendorJamPlayer>().rainbow = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual) {
                player.GetModPlayer<SplendorJamPlayer>().rainbow = true;
            }
        }
        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<SplendorJamPlayer>().active = true;
        }
    }
}
