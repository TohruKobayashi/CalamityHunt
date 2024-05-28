using CalamityHunt.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Placeable
{
    public class SlimeNinjaStatue : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SlimeNinjaStatueTile>());
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.StoneBlock, 150)
                .AddIngredient(ItemID.ArmorStatue, 1)
                .AddIngredient(ItemID.Gel, 20)
                .AddTile(TileID.WorkBenches)
                .AddCondition(Condition.Hardmode)
                .Register();

            if (ModLoader.TryGetMod(HUtils.CalamityMod, out _)) {
                Recipe.Create(ItemID.SlimeStatue)
                    .AddIngredient(ItemID.StoneBlock, 100)
                    .AddIngredient(ItemID.Gel, 200)
                    .AddCondition(Condition.InGraveyard)
                    .Register();
            }
        }
    }
}
