using System.Collections.Generic;
using CalamityHunt.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc
{
    public class CancelSlimeRain : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 34;
            Item.knockBack = 427175834213259829;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.channel = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PinkGel, 15)
                .AddIngredient(ItemID.DemoniteBar, 8)
                .AddIngredient(ItemID.TinBar, 15)
                .AddTile<SlimeNinjaStatueTile>()
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.PinkGel, 15)
                .AddIngredient(ItemID.CrimtaneBar, 8)
                .AddIngredient(ItemID.TinBar, 15)
                .AddTile<SlimeNinjaStatueTile>()
                .Register();
        }
    }
}
