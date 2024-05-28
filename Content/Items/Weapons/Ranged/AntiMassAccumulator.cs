using CalamityHunt.Content.Projectiles.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Weapons.Ranged;

public class AntiMassAccumulator : ModItem
{
    public override void SetDefaults()
    {
        if (HUtils.NonVanillaBalance) {
            Item.damage = 98;
            Item.rare = ItemRarityID.Red;
        }
        else {
            Item.damage = 545;
            Item.rare = ItemRarityID.Red;
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                ModRarity r;
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.TryFind<ModRarity>("Turquoise", out r);
                Item.rare = r.Type;
            }
        }

        Item.DamageType = DamageClass.Ranged;
        Item.width = 100;
        Item.height = 42;
        Item.useTime = 90;
        Item.useAnimation = Item.useTime;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 6;
        Item.value = 10000;
        Item.autoReuse = true;
        Item.shootSpeed = 5;
        Item.shoot = ModContent.ProjectileType<AntiMassAccumulatorProj>();
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.channel = true;
    }

    public override void AddRecipes()
    {
        if (ModLoader.TryGetMod(HUtils.CalamityMod, out Mod calamity)) {
            if (ModLoader.TryGetMod(HUtils.CatalystMod, out Mod catalyst)) {
                CreateRecipe() //calamity + catalyst recipe
                    .AddIngredient(calamity.Find<ModItem>("AdamantiteParticleAccelerator").Type)
                    .AddIngredient(catalyst.Find<ModItem>("MetanovaBar").Type, 6)
                    .AddIngredient(calamity.Find<ModItem>("ExodiumCluster").Type, 30)
                    .AddIngredient(calamity.Find<ModItem>("MysteriousCircuitry").Type, 18)
                    .AddIngredient(calamity.Find<ModItem>("SuspiciousScrap").Type, 3)
                    .AddTile(TileID.LunarCraftingStation)
                    .Register();
            }
            else {
                CreateRecipe() //calamity recipe
                    .AddIngredient(calamity.Find<ModItem>("AdamantiteParticleAccelerator").Type)
                    .AddIngredient(calamity.Find<ModItem>("UelibloomBar").Type, 12)
                    .AddIngredient(calamity.Find<ModItem>("ExodiumCluster").Type, 30)
                    .AddIngredient(calamity.Find<ModItem>("MysteriousCircuitry").Type, 18)
                    .AddIngredient(calamity.Find<ModItem>("SuspiciousScrap").Type, 3) //this is the illegal gun parts of calamity
                    .AddTile(TileID.LunarCraftingStation)
                    .Register();
            }
        }
        else {
            CreateRecipe() //vanilla recipe
                .AddIngredient(ItemID.AdamantiteBar, 10)
                .AddIngredient(ItemID.SoulofFlight, 6)
                .AddIngredient(ItemID.LunarBar, 90) //value of uelibloom bar and exodium cluster (108 gold) / value of liminite bar (1.2 gold) = 90 luminite bars
                .AddIngredient(ItemID.IllegalGunParts, 3) //this is the illegal gun parts of vanilla
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectileDirect(source, position, velocity, type, damage, 0, player.whoAmI);

        return false;
    }

    public static Asset<Texture2D> glowmask;

    public override void Load()
    {
        glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow", AssetRequestMode.ImmediateLoad);
    }

    public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        Texture2D glow = glowmask.Value;
        spriteBatch.Draw(glow, position, frame, Color.Gold with { A = 40 }, 0, origin, scale, 0, 0);
    }

    public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
    {
        Texture2D glow = glowmask.Value;
        spriteBatch.Draw(glow, Item.Center - Main.screenPosition, glow.Frame(), Color.Gold with { A = 40 }, rotation, glow.Size() * 0.5f, scale, 0, 0);
    }
}
