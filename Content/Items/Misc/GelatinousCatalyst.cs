using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc
{
    public class GelatinousCatalyst : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 34;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.channel = true;
            Item.maxStack = Item.CommonMaxStack;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Gel, 200)
                .AddIngredient(ItemID.SoulofMight, 3)
                .AddIngredient(ItemID.SoulofSight, 3)
                .AddIngredient(ItemID.SoulofFright, 3)
                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }
}