using CalamityHunt.Common.Systems;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Projectiles.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Weapons.Melee
{
    public class Parasanguine : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.HasAProjectileThatHasAUsabilityCheck[Type] = true;
            ItemID.Sets.gunProj[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 78;
            Item.height = 72;
            Item.damage = 2722;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.sellPrice(gold: 20);
            Item.shoot = ModContent.ProjectileType<ParasanguineHeld>();
            Item.shootSpeed = 8f;
            Item.autoReuse = true;

            if (ModLoader.HasMod("CalamityMod"))
            {
                ModRarity r;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Main.LocalPlayer.HeldItem == Item || Main.mouseItem == Item)
            {
                Texture2D bar = AssetDirectory.Textures.Bars.Bar.Value;
                Texture2D barCharge = AssetDirectory.Textures.Bars.BarCharge.Value;

                Rectangle chargeFrame = new Rectangle(0, 0, (int)(barCharge.Width * Main.LocalPlayer.GetModPlayer<GoozmaWeaponsPlayer>().ParasolBloodPercent), barCharge.Height);

                Color barColor = Color.Lerp(Color.DarkRed * 0.5f, Color.Red, Utils.GetLerpValue(0.5f, 1f, Main.LocalPlayer.GetModPlayer<GoozmaWeaponsPlayer>().ParasolBloodPercent, true));
                spriteBatch.Draw(bar, position + new Vector2(0, 50) * scale, bar.Frame(), Color.DarkRed, 0, bar.Size() * 0.5f, scale * 1.75f, 0, 0);
                spriteBatch.Draw(barCharge, position + new Vector2(0, 50) * scale, chargeFrame, barColor, 0, barCharge.Size() * 0.5f, scale * 1.75f, 0, 0);
            }
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<ParasanguineHeld>()] <= 0;

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ParasanguineHeld>()] <= 0)
            {
                int blood = 0;
                if (player.altFunctionUse > 0 && player.GetModPlayer<GoozmaWeaponsPlayer>().ParasolBloodPercent > 0.5f)
                    blood = 2;

                Projectile.NewProjectileDirect(source, position, velocity, type, damage, 0, player.whoAmI, ai1: blood);
            }

            return false;
        }

        public override bool MeleePrefix() => true;
    }
}
