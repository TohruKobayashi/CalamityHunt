using System;
using CalamityHunt.Common.Players;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Items.Misc;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Ranged;
using CalamityHunt.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Weapons.Ranged
{
    public class Trailblazer : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SkipsInitialUseSound[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 90;
            Item.height = 38;
            Item.damage = 750;
            Item.noMelee = true;
            Item.useAnimation = 15;
            Item.useTime = 3;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 10f;
            Item.UseSound = AssetDirectory.Sounds.Weapons.TrailBlazerFireStart;
            Item.channel = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TrailblazerFlame>();
            Item.shootSpeed = 9.5f;
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.useAmmo = AmmoID.Gel;
            Item.value = Item.sellPrice(gold: 20);
            Item.ArmorPenetration = 15;
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                ModRarity r;
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
            Item.DamageType = DamageClass.Ranged;
            Item.consumeAmmoOnFirstShotOnly = true;
        }

        public override void HoldItem(Player player)
        {
            player.GetModPlayer<VanityPlayer>().trailblazerGoggles = true;
            player.GetModPlayer<VanityPlayer>().trailblazerBackpack = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-16f, 0f);

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (Main.rand.Next(0, 100) < 80) {
                return false;
            }

            return true;
        }

        public override void AddRecipes()
        {
            Mod calamityHunt = ModLoader.GetMod("CalamityHunt");
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                CreateRecipe()
                    .AddIngredient<ChromaticMass>(15)
                    .AddIngredient(calamity.Find<ModItem>("CleansingBlaze").Type)
                    .AddIngredient(calamity.Find<ModItem>("PristineFury").Type)
                    .AddIngredient(calamity.Find<ModItem>("OverloadedBlaster").Type)
                    .AddIngredient(calamity.Find<ModItem>("AuroraBlazer").Type)
                    .AddTile(calamity.Find<ModTile>("DraedonsForge").Type)
                    .AddCustomShimmerResult(calamityHunt.Find<ModItem>("TrailblazerGoggles").Type)
                    .AddCustomShimmerResult(calamityHunt.Find<ModItem>("TrailblazerBackpack").Type)
                    .Register();
            }
            else {
                CreateRecipe()
                    .AddIngredient(ItemID.ElfMelter)
                    .AddIngredient<ChromaticMass>(15)
                    .AddTile<SlimeNinjaStatueTile>()
                    .AddCustomShimmerResult(calamityHunt.Find<ModItem>("TrailblazerGoggles").Type)
                    .AddCustomShimmerResult(calamityHunt.Find<ModItem>("TrailblazerBackpack").Type)
                    .Register();
            }
        }
    }
}
