using CalamityHunt.Common.Players;
using CalamityHunt.Content.Items.Masks;
using CalamityHunt.Content.Items.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace CalamityHunt.Content.Items.Armor.Shogun
{
    [AutoloadEquip(EquipType.Head)]
    public class ShogunHelm : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.defense = 48;
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
            Asset<Texture2D> ponytail = ModContent.Request<Texture2D>(Texture + "_Ponytail");
            spriteBatch.Draw(ponytail.Value, position, frame, drawColor.MultiplyRGBA(Main.LocalPlayer.hairColor), 0, origin, scale, 0, 0);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Asset<Texture2D> ponytail = ModContent.Request<Texture2D>(Texture + "_Ponytail");
            spriteBatch.Draw(ponytail.Value, Item.Center - Main.screenPosition, ponytail.Frame(), lightColor.MultiplyRGBA(Main.LocalPlayer.hairColor), 0, ponytail.Size() * 0.5f, scale, 0, 0);
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<ShogunChestplate>() && legs.type == ModContent.ItemType<ShogunPants>();

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.20f;
            player.GetCritChance(DamageClass.Generic) += 0.15f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.15f;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetOrRegister(Mod.GetLocalizationKey("SetBonuses.Shogun")).Value;
            player.jumpSpeedBoost += 2f;
            player.GetModPlayer<ShogunArmorPlayer>().active = true;
            player.GetDamage(DamageClass.Generic) += 0.18f;
            player.maxMinions += 5;
        }
    }

    public class ShogunHelmPonytailLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.HeadBack);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.armor[10].IsAir && drawInfo.drawPlayer.armor[0].ModItem is ShogunHelm || drawInfo.drawPlayer.armor[10].ModItem is ShogunHelm;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Content/Items/Armor/Shogun/ShogunHelm_Head_Ponytail");

            DrawData drawData = new DrawData(texture.Value, drawInfo.HeadPosition(), drawInfo.drawPlayer.legFrame, drawInfo.colorHair, drawInfo.drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0);
            drawData.shader = drawInfo.hairDyePacked;
            drawInfo.DrawDataCache.Add(drawData);
        }
    }
}
