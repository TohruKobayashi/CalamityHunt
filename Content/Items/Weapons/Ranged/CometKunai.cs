using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Weapons.Ranged
{
    public class CometKunai : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 100;
            Item.height = 100;
            Item.damage = 185;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 9;
            Item.useTime = 3;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 10f;
            Item.UseSound = SoundID.DD2_JavelinThrowersAttack with { MaxInstances = 0, PitchVariance = 0.1f, Pitch = 0.5f, Volume = 0.8f };
            Item.autoReuse = true;
            Item.shootSpeed = 5f;
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(gold: 20);

            if (ModLoader.TryGetMod(HUtils.CalamityMod, out Mod calamity)) {
                calamity.TryFind("RogueDamageClass", out DamageClass d);
                calamity.TryFind("Violet", out ModRarity r);
                Item.DamageType = d;
                Item.rare = r.Type;
            }
            Item.shoot = ModContent.ProjectileType<CometKunaiProjectile>();
            Item.channel = true;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 200);

        public override bool AltFunctionUse(Player player) => !ModLoader.HasMod(HUtils.CalamityMod);

        public override bool CanUseItem(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.whoAmI == Main.myPlayer) {

                bool CalamityStealth = false;
                if (ModLoader.HasMod(HUtils.CalamityMod)) {
                    CalamityStealth = (bool)ModLoader.GetMod(HUtils.CalamityMod).Call("CanStealthStrike", player);
                }
                // if calamity is on, then do when stealthed up. if not, do when rclick
                if (ModLoader.HasMod(HUtils.CalamityMod) ? CalamityStealth : player.altFunctionUse == 2) {
                    if (player.itemAnimation == player.itemAnimationMax) {
                        Projectile stealthProj = Projectile.NewProjectileDirect(source, position, velocity * 3, ModContent.ProjectileType<CometKunaiSuperProjectile>(), damage * 2, knockback, player.whoAmI);
                        stealthProj.ai[1] = -1;
                        stealthProj.rotation += Main.rand.NextFloat(-1f, 1f);
                        if (ModLoader.HasMod(HUtils.CalamityMod)) {
                            ModLoader.GetMod(HUtils.CalamityMod).Call("SetStealthProjectile", stealthProj, true);
                        }
                    }
                    return false;
                }

                float ai = 0;
                if (Main.rand.NextBool(30)) {
                    type = ModContent.ProjectileType<CometKunaiCritProjectile>();
                    velocity *= 1.5f;
                    damage *= 3;
                    ai = Main.rand.NextFloat(2);
                }
                else {
                    Vector2 newVelocity = velocity * Main.rand.NextFloat(0.7f, 1f);
                    float randRot = Main.rand.NextFloat(-1f, 1f);
                    position += newVelocity.RotatedBy(randRot * 0.6f) * 2;
                    velocity = newVelocity.RotatedBy(randRot * 0.05f);
                }

                Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
                proj.rotation = ai;
                proj.ai[1] = ai;
            }

            return false;
        }
    }
}
