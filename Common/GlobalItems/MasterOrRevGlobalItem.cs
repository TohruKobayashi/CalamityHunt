using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityHunt.Common.GlobalItems;

/// <summary>
/// Applies the "Master or Revengeance" tooltip to Hunt items when Calamity is enabled.
/// </summary>
public class MasterOrRevGlobalItem : GlobalItem
{
    public override bool IsLoadingEnabled(Mod mod)
    {
        return ModCompatibility.Calamity.IsLoaded;
    }

    public override bool AppliesToEntity(Item item, bool lateInstantiation)
    {
        return item.master && item.ModItem?.Mod == CalamityHunt.Instance;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        TooltipLine masterLine = tooltips.Find((t) => t.Name.Equals("Master"));
        if (masterLine != null) {
            masterLine.Text += Language.GetTextValue("Mods.CalamityHunt.OrRev");
        }
    }
}
