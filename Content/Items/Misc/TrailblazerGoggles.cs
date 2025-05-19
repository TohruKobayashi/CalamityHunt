using System.Linq;

using CalamityHunt.Common.Graphics.RenderTargets;
using CalamityHunt.Common.Players;
using CalamityHunt.Common.Systems;
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

    public override void UpdateEquip(Player player)
    {
        UpdateVanity(player);

        if (Config.Instance.debugMode) {
            Main.NewText("");
            Main.NewText("");
            Main.NewText("");
            Main.NewText("");
            Main.NewText("");
            Main.NewText("");
            Main.NewText("Current particles: " + CalamityHunt.Particles.Particles.Count());
            Main.NewText("Current bg particles: " + CalamityHunt.ParticlesBehindEntities.Particles.Count());
            Main.NewText("Current metaballs: " + CosmosMetaball.Particles.Particles.Count());
            int dustc = 0;
            foreach (Dust d in Main.dust) {
                if (d.active)
                    dustc++;
            }
            Main.NewText("Current dust: " + dustc);
        }
    }

    public override void UpdateVanity(Player player) => player.GetModPlayer<VanityPlayer>().trailblazerGoggles = true;
}
