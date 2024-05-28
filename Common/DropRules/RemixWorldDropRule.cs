using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace CalamityHunt.Common.DropRules;

public class RemixWorldDropRule : IItemDropRuleCondition
{
    bool IItemDropRuleCondition.CanDrop(DropAttemptInfo info) => Main.remixWorld;

    bool IItemDropRuleCondition.CanShowItemDropInUI() => false;

    string IProvideItemConditionDescription.GetConditionDescription() => "";
}
