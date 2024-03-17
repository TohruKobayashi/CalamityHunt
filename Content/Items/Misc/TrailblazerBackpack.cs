using CalamityHunt.Common.Players;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Items.Rarities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc;

public class TrailblazerBackpack : ModItem
{
    public static Asset<Texture2D> backTexture;
    public static Asset<Texture2D> backSwirlTexture;
    public static Asset<Texture2D> backAntennaTexture;
    public static Asset<Texture2D> strapTexture;
    public override void Load()
    {
        backTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "_Back");
        backSwirlTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "_BackSwirl");
        backAntennaTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "_BackAntenna");
        strapTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "_Strap");
    }

    public override void SetDefaults()
    {
        Item.width = 32; //update
        Item.height = 32; //update
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

    public override void UpdateEquip(Player player) => UpdateVanity(player);

    public override void UpdateVanity(Player player) => player.GetModPlayer<VanityPlayer>().trailblazerBackpack = true;
}
