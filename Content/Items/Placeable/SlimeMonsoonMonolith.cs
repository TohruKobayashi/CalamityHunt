using CalamityHunt.Common.Systems;
using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Placeable
{
    public class SlimeMonsoonMonolith : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SlimeMonsoonMonolithTile>());
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.accessory = true;
            Item.vanity = true;
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                ModRarity r;
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
        }


        public override void UpdateAccessory(Player player, bool visual)
        {
            UpdateVanity(player);
        }

        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<SceneEffectPlayer>().effectActive[(ushort)SceneEffectPlayer.EffectorType.SlimeMonsoon] = 30;
        }


        public override void AddRecipes()
        {
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                CreateRecipe()
                    .AddIngredient<ChromaticMass>(15)
                    .AddTile(calamity.Find<ModTile>("DraedonsForge").Type)
                    .DisableDecraft()
                    .Register();
            }
            else {
                CreateRecipe()
                    .AddIngredient<ChromaticMass>(15)
                    .AddTile<SlimeNinjaStatueTile>()
                    .DisableDecraft()
                    .Register();
            }
        }
    }
}
