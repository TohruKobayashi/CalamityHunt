using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Magic;
using CalamityHunt.Content.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Weapons.Magic
{
    public class Goomoire : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 48;
            Item.damage = 1750;
            Item.DamageType = DamageClass.Magic;
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.mana = 15;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.sellPrice(gold: 20);
            Item.shoot = ModContent.ProjectileType<GoomoireSuck>();
            Item.shootSpeed = 4f;
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                ModRarity r;
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<GoomoireSuck>()] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.manaCost = 0f;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<GoomoireSuck>()] <= 0) {
                if (player.altFunctionUse == 0) {
                    Projectile.NewProjectileDirect(source, position, velocity, type, damage, 0, player.whoAmI);
                }
            }

            return false;
        }

        public override void AddRecipes()
        {
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                CreateRecipe()
                    .AddIngredient<ChromaticMass>(15)
                    .AddIngredient(calamity.Find<ModItem>("PrimordialAncient").Type)
                    .AddIngredient(calamity.Find<ModItem>("Biofusillade").Type)
                    .AddIngredient(calamity.Find<ModItem>("AbyssalTome").Type)
                    .AddIngredient(calamity.Find<ModItem>("WintersFury").Type)
                    .AddTile(calamity.Find<ModTile>("DraedonsForge").Type)
                    .Register();
            }
            else {
                CreateRecipe()
                    .AddIngredient(ItemID.SpellTome)
                    .AddIngredient<ChromaticMass>(15)
                    .AddTile<SlimeNinjaStatueTile>()
                    .Register();
            }
        }
    }
}
