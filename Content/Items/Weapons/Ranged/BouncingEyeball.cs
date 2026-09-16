using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Weapons.Ranged
{
    public class BouncingEyeball : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 20;

            Item.DamageType = DamageClass.Ranged;
            Item.damage = 16;
            Item.shoot = ModContent.ProjectileType<BouncingEyeball_Proj>();
            Item.useTime = 18;
            Item.useAnimation = Item.useTime;
            Item.autoReuse = true;
            Item.shootSpeed = 5f;
            Item.knockBack = 3f;

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1 with { MaxInstances = 0, PitchVariance = 0.1f, Pitch = 0.5f, Volume = 0.8f };

            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 50);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(Item.GetSource_FromThis(), position, velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.75f, 1.25f), type, damage, knockback, Main.myPlayer);
            return false;
        }
    }
}
