using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Placeable
{
    public class GoozmaBoulderItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GoozmaBoulderTile>());
            Item.width = 46;
            Item.height = 46;
            Item.rare = ModContent.RarityType<VioletRarity>();
        }

        public override void AddRecipes()
        {
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                CreateRecipe()
                    .AddIngredient(ItemID.Boulder)
                    .AddIngredient<ChromaticMass>(5)
                    .AddTile(calamity.Find<ModTile>("DraedonsForge").Type)
                    .AddCondition(Condition.InGraveyard)
                    .Register();
            }
            else {
                CreateRecipe()
                    .AddIngredient(ItemID.Boulder)
                    .AddIngredient<ChromaticMass>(5)
                    .AddTile(TileID.HeavyWorkBench)
                    .AddCondition(Condition.InGraveyard)
                    .Register();
            }
        }
    }
}
