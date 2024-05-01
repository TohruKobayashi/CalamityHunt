using CalamityHunt.Content.Items.Materials;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems
{
    public class CraftingSystem : ModSystem
    {
        public static int AnyEvilBlock;
        public override void AddRecipeGroups()
        {
            RecipeGroup evil = new RecipeGroup(() => Language.GetOrRegister($"Mods.{nameof(CalamityHunt)}.AnyEvilBlock").Value, 61, 836, 833, 835, 370, 1246, 3274, 3275, 3276, 3277);
            AnyEvilBlock = RecipeGroup.RegisterGroup("CalamityHunt:AnyEvilBlock", evil);
        }
        public override void PostAddRecipes()
        {
            Mod cal;
            ModLoader.TryGetMod("CalamityMod", out cal);
            if (cal != null)
            {
                for (int i = 0; i < Recipe.numRecipes; i++)
                {
                    Recipe recipe = Main.recipe[i];
                    if (recipe.HasResult(cal.Find<ModItem>("ShadowspecBar").Type))
                        recipe.AddIngredient(ModContent.ItemType<ChromaticMass>());

                }
            }
        }
    }
}
