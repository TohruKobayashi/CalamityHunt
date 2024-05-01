using CalamityHunt.Common.Players;
using CalamityHunt.Common.Systems;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace CalamityHunt.Common.DropRules;

public class ChromaticDropRule : IItemDropRuleCondition
{
    bool IItemDropRuleCondition.CanDrop(DropAttemptInfo info)
    {
        return !Main.LocalPlayer.GetModPlayer<AuricSoulPlayer>().goozmaSoul;
    }

    bool IItemDropRuleCondition.CanShowItemDropInUI() => true;

    string IProvideItemConditionDescription.GetConditionDescription() => "Drops if not absorbed";
}
