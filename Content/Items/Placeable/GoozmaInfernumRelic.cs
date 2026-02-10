using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Placeable
{
    public class GoozmaInfernumRelic : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModLoader.HasMod(HUtils.InfernumMode);
        }
        public override void SetDefaults()
        {
            // Vanilla has many useful methods like these, use them! This substitutes setting Item.createTile and Item.placeStyle as well as setting a few values that are common across all placeable items
            // The place style (here by default 0) is important if you decide to have more than one relic share the same tile type (more on that in the tiles' code)
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Relics.GoozmaInfernumRelicTile>(), 0);

            Item.width = 30;
            Item.height = 40;
            Item.rare = ItemRarityID.Master;
            if (ModLoader.TryGetMod("InfernumMode", out Mod infernumMod)) {
                Item.rare = infernumMod.Find<ModRarity>("InfernumRedRarity").Type;
            }
            Item.value = Item.buyPrice(0, 5);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine obj = tooltips.FirstOrDefault((x) => x.Name == "Tooltip0" && x.Mod == "Terraria");
            obj.Text = Language.GetOrRegister($"Mods.{nameof(CalamityHunt)}.InfernalRelicText").Value;
            obj.OverrideColor = Color.DarkRed;
        }
    }
}
