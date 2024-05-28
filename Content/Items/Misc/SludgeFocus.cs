using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc
{
    public class SludgeFocus : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 48;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<VioletRarity>();
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                ModRarity r;
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
            Item.channel = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ChromaticMass>(5)
                .AddIngredient(ItemID.Gel, 100)
                .AddIngredient<GelatinousCatalyst>()
                .AddTile<SlimeNinjaStatueTile>()
                .Register();
        }

        public static Asset<Texture2D> glowTexture;

        public override void Load()
        {
            glowTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "Glow");
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            spriteBatch.Draw(glowTexture.Value, position, glowTexture.Value.Frame(), Color.White, 0, glowTexture.Value.Size() * 0.5f, scale, 0, 0);
            spriteBatch.Draw(glowTexture.Value, position, glowTexture.Value.Frame(), new Color(50, 50, 50, 0), 0, glowTexture.Value.Size() * 0.5f, scale, 0, 0);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            spriteBatch.Draw(glowTexture.Value, Item.Center - Main.screenPosition, glowTexture.Value.Frame(), Color.White, rotation, glowTexture.Value.Size() * 0.5f, scale, 0, 0);
            spriteBatch.Draw(glowTexture.Value, Item.Center - Main.screenPosition, glowTexture.Value.Frame(), new Color(50, 50, 50, 0), rotation, glowTexture.Value.Size() * 0.5f, scale, 0, 0);
        }
    }
}
