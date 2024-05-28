using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace CalamityHunt.Common.DropRules;

public class ZenithWorldDropRule : IItemDropRuleCondition
{
    bool IItemDropRuleCondition.CanDrop(DropAttemptInfo info) => Main.zenithWorld;

    bool IItemDropRuleCondition.CanShowItemDropInUI() => false;

    string IProvideItemConditionDescription.GetConditionDescription() => "";
}
