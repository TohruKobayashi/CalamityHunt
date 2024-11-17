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

namespace CalamityHunt.Content.Items.Misc.AltarOfferings
{
    public class OfferingBetterSlimes : ModItem
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
            if (ModLoader.TryGetMod(HUtils.CalamityMod, out Mod calamity)) {
                CreateRecipe()
                .AddIngredient(calamity.Find<ModItem>("AscendantSpiritEssence").Type, 10)
                .AddIngredient(ItemID.Gel, 250)
                .AddIngredient<GelatinousCatalyst>()
                .AddTile<SlimeNinjaStatueTile>()
                .Register();
            }
            else {
                CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 5)
                .AddIngredient(ItemID.Gel, 250)
                .AddIngredient<GelatinousCatalyst>()
                .AddTile<SlimeNinjaStatueTile>()
                .Register();
            }
        }
    }
}
