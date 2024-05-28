using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Weapons.Rogue
{
    public class Goozmaga : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 100;
            Item.damage = 21203;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 10f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shootSpeed = 22f;
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.DamageType = DamageClass.Throwing;
            Item.value = Item.sellPrice(gold: 20);
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                DamageClass d;
                ModRarity r;
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.TryFind<DamageClass>("RogueDamageClass", out d);
                calamity.TryFind<ModRarity>("CalamityRed", out r);
                Item.DamageType = d;
                Item.rare = r.Type;
            }
            Item.shoot = ModContent.ProjectileType<GoozmagaBomb>();
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2) {
                Item.shoot = ProjectileID.None;
                Item.useStyle = ItemUseStyleID.HoldUp;
            }
            else {
                Item.shoot = ModContent.ProjectileType<GoozmagaBomb>();
                Item.useStyle = ItemUseStyleID.Swing;
            }
            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            for (int i = 0; i < Main.maxProjectiles; i++) {
                Projectile proj = Main.projectile[i];
                if (proj.active && (proj.type == ModContent.ProjectileType<GoozmagaBomb>() || proj.type == ModContent.ProjectileType<GoozmagaShrapnel>()) && proj.owner == player.whoAmI) {
                    proj.Kill();
                }
            }
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);

                if ((bool)calamity.Call("CanStealthStrike", player)) //setting the stealth strike
                {
                    int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, 1f);
                    calamity.Call("SetStealthProjectile", Main.projectile[p], true);
                    return false;
                }
            }
            else if (player.vortexStealthActive || player.shroomiteStealth) {
                // stealth strike
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, 1f);
                return false;
            }
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
