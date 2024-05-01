using CalamityHunt.Common.Systems;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Particles;
using CalamityHunt.Content.Projectiles.Weapons.Magic;
using CalamityHunt.Content.Projectiles.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityHunt.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Weapons.Magic
{
    public class CrystalGauntlets : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.HasAProjectileThatHasAUsabilityCheck[Type] = true;
            ItemID.Sets.gunProj[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 54;
            Item.damage = 1000;
            Item.DamageType = DamageClass.Magic;
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.mana = 15;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.sellPrice(gold: 20);
            Item.shoot = ModContent.ProjectileType<CrystalGauntletBall>();
            Item.shootSpeed = 4f;
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

                Rectangle chargeFrame = new Rectangle(0, 0, (int)(barCharge.Width * Main.LocalPlayer.GetModPlayer<GoozmaWeaponsPlayer>().CrystalGauntletsCharge), barCharge.Height);

                Color barColor = Color.Lerp(Color.MediumOrchid, Color.Turquoise, Utils.GetLerpValue(0.3f, 0.8f, Main.LocalPlayer.GetModPlayer<GoozmaWeaponsPlayer>().CrystalGauntletsCharge, true));
                barColor.A = 128;
                spriteBatch.Draw(bar, position + new Vector2(0, 35) * scale, bar.Frame(), Color.DarkSlateBlue, 0, bar.Size() * 0.5f, scale * 1.2f, 0, 0);
                spriteBatch.Draw(barCharge, position + new Vector2(0, 35) * scale, chargeFrame, barColor, 0, barCharge.Size() * 0.5f, scale * 1.2f, 0, 0);
            }
        }

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse > 0)
                reduce *= 0.25f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<CrystalGauntletBall>()] <= 0;

        public override bool AltFunctionUse(Player player) => true;

        public static Color[] SpectralColor => new Color[]
        {
            new Color(232, 140, 167),
            new Color(184, 130, 207),
            new Color(82, 172, 158),
            new Color(114, 189, 201)
        };

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.manaCost = 0f;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<CrystalGauntletBall>()] <= 0)
            {
                if (player.altFunctionUse == 0)
                    Projectile.NewProjectileDirect(source, position, velocity, type, damage, 0, player.whoAmI);
            }

            if (player.altFunctionUse > 0)
            {
                player.GetModPlayer<GoozmaWeaponsPlayer>().crystalGauntletsClapTime = 30;
                player.GetModPlayer<GoozmaWeaponsPlayer>().crystalGauntletsClapDir = player.DirectionTo(Main.MouseWorld);

                //Projectile piercer = Projectile.NewProjectileDirect(source, position, velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.05f) * 18f, ModContent.ProjectileType<CrystalPiercer>(), damage, 0, player.whoAmI);
                //piercer.localAI[0] = Main.GlobalTimeWrappedHourly * 7f;
            }

            return false;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            gravity *= 0.8f;
            Color glowColor = new GradientColor(SpectralColor, 0.1f, 0.1f).ValueAt(Main.GlobalTimeWrappedHourly * 7f);
            Lighting.AddLight(Item.Center, glowColor.ToVector3() * 0.4f);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 200);

        public override void EquipFrameEffects(Player player, EquipType type)
        {
            player.handon = -1;
            player.handoff = -1;
        }

        public override void HoldItem(Player player)
        {
            Color glowColor = new GradientColor(SpectralColor, 0.1f, 0.1f).ValueAt(Main.GlobalTimeWrappedHourly * 7f);
            Lighting.AddLight(player.MountedCenter, glowColor.ToVector3() * 0.4f);
            player.GetModPlayer<GoozmaWeaponsPlayer>().crystalGauntletsHeld = true;
        }
    }
}
