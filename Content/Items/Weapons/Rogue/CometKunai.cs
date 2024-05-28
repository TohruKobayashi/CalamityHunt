using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Rogue;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Weapons.Rogue
{
    public class CometKunai : ModItem
    {
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
            Item.shootSpeed = 11f;
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
            Item.shoot = ModContent.ProjectileType<CometKunaiProjectile>();
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 200);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //if (Item.UseSound.HasValue)
            //    SoundEngine.PlaySound(Item.UseSound.Value, player.Center);

            if (player.whoAmI == Main.myPlayer)
            {
                if (ModLoader.HasMod("CalamityMod"))
                {
                    Mod calamity = ModLoader.GetMod("CalamityMod");

                    if ((bool)calamity.Call("CanStealthStrike", player)) //setting the stealth strike
                    {
                        // stealth strike
                        if (player.itemAnimation == player.itemAnimationMax)
                        {
                            Projectile stealthProj = Projectile.NewProjectileDirect(source, position, velocity * 3, ModContent.ProjectileType<CometKunaiStealthProjectile>(), damage * 2, knockback, player.whoAmI);
                            stealthProj.ai[1] = -1;
                            stealthProj.rotation += Main.rand.NextFloat(-1f, 1f);

                            for (int i = 0; i < 5; i++)
                            {
                                Vector2 offVel = velocity.RotatedBy(0.3f / 4f * i - 0.15f);
                                Projectile.NewProjectileDirect(source, position, offVel, type, damage, knockback, player.whoAmI);
                            }
                            SoundEngine.PlaySound(AssetDirectory.Sounds.Slime.StellarConstellationWave with { Pitch = 1f, Volume = 0.2f }, player.Center);
                        }
                        return false;
                    }
                }
                else if (player.vortexStealthActive || player.shroomiteStealth)
                {
                    // stealth strike
                    if (player.itemAnimation == player.itemAnimationMax)
                    {
                        Projectile stealthProj = Projectile.NewProjectileDirect(source, position, velocity * 3, ModContent.ProjectileType<CometKunaiStealthProjectile>(), damage * 2, knockback, player.whoAmI);
                        stealthProj.ai[1] = -1;
                        stealthProj.rotation += Main.rand.NextFloat(-1f, 1f);

                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 offVel = velocity.RotatedBy(0.3f / 4f * i - 0.15f);
                            Projectile.NewProjectileDirect(source, position, offVel, type, damage, knockback, player.whoAmI);
                        }
                        SoundEngine.PlaySound(AssetDirectory.Sounds.Slime.StellarConstellationWave with { Pitch = 1f, Volume = 0.2f }, player.Center);
                    }
                    return false;
                }

                float ai = 0;
                if (Main.rand.NextBool(30))
                {
                    type = ModContent.ProjectileType<CometKunaiCritProjectile>();
                    velocity *= 1.5f;
                    damage *= 3;
                    ai = Main.rand.NextFloat(2);
                }
                else
                {
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
