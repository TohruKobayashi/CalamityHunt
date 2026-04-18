using CalamityHunt.Common.DropRules;
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
    public class TreasureBucket : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.BossBag[Type] = true;
            ItemID.Sets.OpenableBag[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 46;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
            Item.consumable = true;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot) => ItemLoader.GetItem(ModContent.ItemType<TreasureTrunk>()).ModifyItemLoot(itemLoot);

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
