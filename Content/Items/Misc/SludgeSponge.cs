using CalamityHunt.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc
{
    [LegacyName("CancelSlimeRain")]
    public class SludgeSpongeEmpty : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 44;
            Item.knockBack = 427175834213259829;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Gel, 50)
                .AddIngredient(ItemID.Coral, 8)
                .AddTile<SlimeNinjaStatueTile>()
                .Register();
        }
    }

    [LegacyName("GelatinousCatalyst")]
    public class SludgeSpongeFull : ModItem
    {
        public override void Load()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SludgeSpongeEmpty>();
        }
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 44;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
        }
    }
}
