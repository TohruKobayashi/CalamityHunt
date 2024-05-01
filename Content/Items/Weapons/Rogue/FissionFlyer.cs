using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Rogue;
using CalamityHunt.Content.Tiles;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;

namespace CalamityHunt.Content.Items.Weapons.Rogue
{
	public class FissionFlyer : ModItem
	{
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
			Item.UseSound = SoundID.DD2_SkyDragonsFuryShot;
			Item.autoReuse = true;
			Item.shootSpeed = 15f;
			Item.rare = ModContent.RarityType<VioletRarity>();
			Item.DamageType = DamageClass.Throwing;
            Item.value = Item.sellPrice(gold: 20);
			if (ModLoader.HasMod("CalamityMod"))
            {
				DamageClass d;
				ModRarity r;
				Mod calamity = ModLoader.GetMod("CalamityMod");
				calamity.TryFind<DamageClass>("RogueDamageClass", out d);
				calamity.TryFind<ModRarity>("Violet", out r);
				Item.DamageType = d;
				Item.rare = r.Type;
            }
			Item.shoot = ModContent.ProjectileType<FissionFlyerProj>();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			bool stealth = false;
			if (ModLoader.HasMod("CalamityMod"))
			{
				Mod calamity = ModLoader.GetMod("CalamityMod");

				if ((bool)calamity.Call("CanStealthStrike", player)) //setting the stealth strike
					stealth = true;
			}
			else if (player.vortexStealthActive || player.shroomiteStealth)
			stealth = true;

			if (Main.myPlayer == player.whoAmI)
			{
				Vector2 mouseWorld = Main.MouseWorld;
				player.LimitPointToPlayerReachableArea(ref mouseWorld);
				velocity = velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.3f) * Main.rand.NextFloat(0.8f, 1.3f) * Math.Max(170, player.Distance(mouseWorld)) * MathF.E * 0.009f;
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, 0, stealth ? 1 : 0);

			}

            		return false;
		}

        public override void AddRecipes()
        {
            if (ModLoader.HasMod("CalamityMod"))
            {
                Mod calamity = ModLoader.GetMod("CalamityMod");
                CreateRecipe()
                    .AddIngredient<ChromaticMass>(15)
                    .AddIngredient(calamity.Find<ModItem>("MangroveChakram").Type)
                    .AddIngredient(calamity.Find<ModItem>("Valediction").Type)
                    .AddIngredient(calamity.Find<ModItem>("ToxicantTwister").Type)
                    .AddIngredient(calamity.Find<ModItem>("SludgeSplotch").Type, 100)
                    .AddTile(calamity.Find<ModTile>("DraedonsForge").Type)
                    .Register();
            }
            else 
            {
                CreateRecipe()
                    .AddIngredient(ItemID.WoodenBoomerang)
                    .AddIngredient<ChromaticMass>(15)
                    .AddTile<SlimeNinjaStatueTile>()
                    .Register();
            }
		}
    }
}