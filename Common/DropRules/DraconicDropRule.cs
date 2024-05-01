using CalamityHunt.Common.Players;
using CalamityHunt.Common.Systems;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace CalamityHunt.Common.DropRules;

public class DraconicDropRule : IItemDropRuleCondition
{
    bool IItemDropRuleCondition.CanDrop(DropAttemptInfo info)
    {
        if (!ModCompatibility.Calamity.IsLoaded)
            return false;

        return !Main.LocalPlayer.GetModPlayer<AuricSoulPlayer>().yharonSoul;
    }

    bool IItemDropRuleCondition.CanShowItemDropInUI() => true;

    string IProvideItemConditionDescription.GetConditionDescription() => "Drops if not absorbed";
}
