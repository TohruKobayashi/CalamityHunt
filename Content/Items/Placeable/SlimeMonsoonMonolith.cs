using CalamityHunt.Common.Systems;
using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Placeable
{
    public class SlimeMonsoonMonolith : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SlimeMonsoonMonolithTile>());
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.accessory = true;
            Item.vanity = true;
            if (ModLoader.HasMod("CalamityMod"))
            {
                ModRarity r;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
        }

        public override void UpdateAccessory(Player player, bool visual)
        {
            player.GetModPlayer<EffectTilePlayer>().effectorCount["SlimeMonsoon"] = 5;
        }
        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<EffectTilePlayer>().effectorCount["SlimeMonsoon"] = 5;
        }

        public override void AddRecipes()
        {
            if (ModLoader.HasMod("CalamityMod"))
            {
                Mod calamity = ModLoader.GetMod("CalamityMod");
                CreateRecipe()
                    .AddIngredient<ChromaticMass>(15)
                    .AddTile(calamity.Find<ModTile>("DraedonsForge").Type)
                    .Register();
            }
            else
            {
                CreateRecipe()
                    .AddIngredient<ChromaticMass>(15)
                    .AddTile<SlimeNinjaStatueTile>()
                    .Register();
            }
        }
    }
}
