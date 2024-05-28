using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc
{
    public class OverloadedSludge : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.LightRed;
            Item.channel = true;
            Item.maxStack = Item.CommonMaxStack;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tooltip = new(Mod, "CalamityHunt:SludgeWarning", Language.GetOrRegister($"Mods.{nameof(CalamityHunt)}.SludgeWarning").Value);
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                int check = tooltips.IndexOf(tooltips.Find(t => t.Text.Equals("Summons the Slime God")));
                if (check == -1) {
                    check = tooltips.IndexOf(tooltips.Find(t => t.Name == "Tooltip0"));
                }
                tooltips.RemoveAt(check);
                tooltips.Insert(check, tooltip);
            }
        }
        public override void AddRecipes()
        {
            if (!ModLoader.HasMod(HUtils.CalamityMod)) {
                CreateRecipe()
                    .AddIngredient(ItemID.PinkGel, 40)
                    .AddRecipeGroup("CalamityHunt:AnyEvilBlock", 40)
                    .AddTile(TileID.DemonAltar)
                    .Register();
            }
        }
    }
}
