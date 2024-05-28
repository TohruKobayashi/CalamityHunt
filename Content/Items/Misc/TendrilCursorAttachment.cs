using CalamityHunt.Common.Players;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc;

public class TendrilCursorAttachment : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 36;
        Item.height = 32;
        Item.rare = ModContent.RarityType<VioletRarity>();
        Item.accessory = true;
        Item.vanity = true;
        if (ModLoader.HasMod(HUtils.CalamityMod)) {
            ModRarity r;
            Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
            calamity.TryFind("Violet", out r);
            Item.rare = r.Type;
        }
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.6f, 0.6f).Value;
        glowColor.A = 150;
        spriteBatch.Draw(TextureAssets.Item[Type].Value, Item.Center - Main.screenPosition, TextureAssets.Item[Type].Frame(), glowColor, rotation, TextureAssets.Item[Type].Size() * 0.5f, scale, 0, 0);
        return false;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.6f, 0.6f).Value;
        glowColor.A = 150;
        spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, glowColor, 0, origin, scale, 0, 0);
        return false;
    }

    public override void UpdateEquip(Player player) => UpdateVanity(player);

    public override void UpdateVanity(Player player) => player.GetModPlayer<VanityPlayer>().tendrilCursor = true;
}
