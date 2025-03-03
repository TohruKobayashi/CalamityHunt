using CalamityHunt.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc
{
    [LegacyName("CancelSlimeRain")]
    public class SludgeSpongeEmpty : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
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
        // this is so the sponge doesnt hover awkwardly on the ground
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            spriteBatch.Draw(TextureAssets.Item[Type].Value, Item.Center - Main.screenPosition, TextureAssets.Item[Type].Frame(), lightColor, rotation, TextureAssets.Item[Type].Size() * 0.5f, scale, 0, 0);
            return false;
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
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddCustomShimmerResult(ModContent.ItemType<SludgeSpongeEmpty>())
                .Register();
        }
    }
}
