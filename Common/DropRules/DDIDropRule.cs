using Terraria.GameContent.ItemDropRules;
using Terraria;

namespace CalamityHunt.Common.DropRules;

public class DDIDropRule : IItemDropRuleCondition
{
    bool IItemDropRuleCondition.CanDrop(DropAttemptInfo info) => Main.remixWorld;

    bool IItemDropRuleCondition.CanShowItemDropInUI() => false;

    string IProvideItemConditionDescription.GetConditionDescription() => "";
}
