using CalamityHunt.Content.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Placeable
{
    public class YharonSoulMusicBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            MusicLoader.AddMusicBox(Mod, AssetDirectory.Music.YharonSoul, ModContent.ItemType<YharonSoulMusicBox>(), ModContent.TileType<YharonSoulMusicBoxTile>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<YharonSoulMusicBoxTile>(), 0);
        }

        public override void AddRecipes()
        {
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                CreateRecipe()
                    .AddIngredient(ItemID.MusicBox)
                    .AddIngredient(calamity.Find<ModItem>("YharonSoulFragment").Type, 15)
                    .AddTile(calamity.Find<ModTile>("CosmicAnvil").Type)
                    .Register();
            }
            else {
                CreateRecipe()
                    .AddIngredient(ItemID.MusicBox)
                    .AddIngredient(ItemID.FragmentSolar, 15)
                    .AddTile<SlimeNinjaStatueTile>()
                    .Register();
            }
        }
    }
}
