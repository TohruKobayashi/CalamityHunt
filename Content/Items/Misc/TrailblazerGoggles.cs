using CalamityHunt.Common.Players;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Items.Rarities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc;

public class TrailblazerGoggles : ModItem
{
    public static Asset<Texture2D> goggleTexture;
    public override void Load()
    {
        goggleTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "_Goggles");
    }

    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 14; 
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

    public override void HoldItem(Player player)
    {
        Main.NewText("Current particles: " + CalamityHunt.particles.particles.Count);
    }

    public override void UpdateEquip(Player player) => UpdateVanity(player);

    public override void UpdateVanity(Player player) => player.GetModPlayer<VanityPlayer>().trailblazerGoggles = true;
}
