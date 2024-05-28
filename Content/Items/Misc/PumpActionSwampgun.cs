using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc
{
    public class PumpActionSwampgun : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 90;
            Item.height = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.value = Item.sellPrice(silver: 50);
            Item.shoot = ModContent.ProjectileType<PumpActionSwampgunHeld>();
            Item.shootSpeed = 5f;
            Item.rare = ModContent.RarityType<VioletRarity>();
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                ModRarity r;
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<PumpActionSwampgunHeld>()] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<PumpActionSwampgunHeld>()] <= 0) {
                Projectile.NewProjectile(source, position, velocity, type, 0, 0, player.whoAmI);
            }

            return false;
        }
    }
}
