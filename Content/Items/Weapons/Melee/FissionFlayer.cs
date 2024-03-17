using System;
using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Melee;
using CalamityHunt.Content.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Weapons.Melee
{
    public class FissionFlayer : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SkipsInitialUseSound[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.damage = 950;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.reuseDelay = 20;
            Item.useLimitPerAnimation = 3;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5f;
            Item.autoReuse = true;
            Item.shootSpeed = 2f;
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.DamageType = DamageClass.Melee;
            Item.value = Item.sellPrice(gold: 20);
            //if (ModLoader.HasMod(HUtils.CalamityMod)) {
            //    DamageClass d;
            //    ModRarity r;
            //    Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
            //    calamity.TryFind("RogueDamageClass", out d);
            //    calamity.TryFind("Violet", out r);
            //    Item.DamageType = d;
            //    Item.rare = r.Type;
            //}
            Item.shoot = ModContent.ProjectileType<FissionFlayerProjectile>();
        }

        public override bool MeleePrefix() => ModLoader.HasMod(HUtils.CalamityMod);

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<FissionFlayerProjectile>()] <= 0;

        public override bool AltFunctionUse(Player player) => true;

        public int swingStyle;
        public float spin;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<FissionFlayerProjectile>()] <= 0) {
                int nextSwingStyle = swingStyle;
                if (player.altFunctionUse == 2) {
                    nextSwingStyle = -1;
                }
                else {
                    swingStyle = (swingStyle + 1) %2;
                }

                Projectile.NewProjectileDirect(source, position, velocity, type, damage, 0, player.whoAmI, ai1: nextSwingStyle);
            }
            //Main.StartSlimeRain(true); 
            return false;
        }

        public override void AddRecipes()
        {
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                CreateRecipe()
                    .AddIngredient<ChromaticMass>(15)
                    .AddIngredient(calamity.Find<ModItem>("MangroveChakram").Type)
                    .AddIngredient(calamity.Find<ModItem>("Valediction").Type)
                    .AddIngredient(calamity.Find<ModItem>("ToxicantTwister").Type)
                    .AddIngredient(calamity.Find<ModItem>("SludgeSplotch").Type, 100)
                    .AddTile(calamity.Find<ModTile>("DraedonsForge").Type)
                    .Register();
            }
            else {
                CreateRecipe()
                    .AddIngredient(ItemID.StaffofEarth)
                    .AddIngredient(ItemID.Trimarang)
                    .AddIngredient<ChromaticMass>(15)
                    .AddTile<SlimeNinjaStatueTile>()
                    .Register();
            }
        }
    }
}
