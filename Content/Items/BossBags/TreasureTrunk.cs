using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Items.Accessories;
using CalamityHunt.Content.Items.Armor.Shogun;
using CalamityHunt.Content.Items.Masks;
using CalamityHunt.Content.Items.Materials;
using CalamityHunt.Content.Items.Misc;
using CalamityHunt.Content.Items.Mounts;
using CalamityHunt.Content.Items.Weapons.Magic;
using CalamityHunt.Content.Items.Weapons.Melee;
using CalamityHunt.Content.Items.Weapons.Ranged;
using CalamityHunt.Content.Items.Weapons.Summoner;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.BossBags
{
    public class TreasureTrunk : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.BossBag[Type] = true;
            ItemID.Sets.OpenableBag[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 40;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
            Item.consumable = true;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ChromaticMass>(), 1, 20, 30));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<SplendorJam>()));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShogunHelm>(), 3));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShogunChestplate>(), 3));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShogunPants>(), 3));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<StickyHand>(), 3));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<PaladinPalanquin>(), 2));
            itemLoot.Add(ItemDropRule.FewFromOptions(1, 7, ModContent.ItemType<EbonianMask>(), ModContent.ItemType<DivineMask>(), ModContent.ItemType<CrimulanMask>(), ModContent.ItemType<StellarMask>()));
            itemLoot.Add(ItemDropRule.FewFromOptions(2, 1, ModContent.ItemType<Parasanguine>(), ModContent.ItemType<SludgeShaker>(), ModContent.ItemType<CrystalGauntlets>(), ModContent.ItemType<SlimeCane>(), ModContent.ItemType<CometKunai>()));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<TendrilCursorAttachment>(), 20));
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Glow");
            Color color = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Main.GlobalTimeWrappedHourly * 20f);
            spriteBatch.Draw(glow.Value, position, frame, color * 0.8f, 0, origin, scale, 0, 0);
            spriteBatch.Draw(glow.Value, position, frame, new Color(color.R, color.G, color.B, 0), 0, origin, scale, 0, 0);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Glow");
            Color color = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Main.GlobalTimeWrappedHourly * 20f);

            spriteBatch.Draw(glow.Value, Item.Center - Main.screenPosition, null, color * 0.5f, rotation, Item.Size * 0.5f, scale, 0, 0);
            spriteBatch.Draw(glow.Value, Item.Center - Main.screenPosition, null, new Color(color.R, color.G, color.B, 0), rotation, Item.Size * 0.5f, scale, 0, 0);
        }
    }
}
