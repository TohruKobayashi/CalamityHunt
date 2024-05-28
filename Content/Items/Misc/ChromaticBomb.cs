using CalamityHunt.Content.Tiles;
using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc
{
    public class ChromaticBomb : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod) => false;

        public const int Damage = 1500;
        public const float Knockback = 10f;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
            ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[Item.type] = true;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.useTime = Item.useAnimation = 40;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<ChromaticBombProjectile>();
            Item.autoReuse = true;
            Item.width = 28;
            Item.height = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.rare = ModContent.RarityType<VioletRarity>();
            if (ModLoader.HasMod("CalamityMod"))
            {
                ModRarity r;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.Bombs;
        }

        public override void AddRecipes()
        {
            if (ModLoader.HasMod("CalamityMod"))
            {
                Mod calamity = ModLoader.GetMod("CalamityMod");
                CreateRecipe(100)
                    .AddIngredient(ItemID.Bomb, 100)
                    .AddIngredient<ChromaticMass>()
                    .AddTile(calamity.Find<ModTile>("DraedonsForge").Type)
                    .Register();
            }
            else
            {
                CreateRecipe(100)
                    .AddIngredient(ItemID.Bomb, 100)
                    .AddIngredient<ChromaticMass>()
                    .AddTile<SlimeNinjaStatueTile>()
                    .Register();
            }
        }
    }
}
