using Terraria.GameContent.ItemDropRules;
using Terraria;

namespace CalamityHunt.Common.DropRules;

public class ZenithDropRule : IItemDropRuleCondition
{
    bool IItemDropRuleCondition.CanDrop(DropAttemptInfo info) => Main.zenithWorld;

    bool IItemDropRuleCondition.CanShowItemDropInUI() => false;

    string IProvideItemConditionDescription.GetConditionDescription() => "";
}
