using CalamityHunt.Common.Systems;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace CalamityHunt.Common.DropRules;

public class GoozmaDownedDropRule : IItemDropRuleCondition
{
    bool IItemDropRuleCondition.CanDrop(DropAttemptInfo info) => !ModContent.GetInstance<BossDownedSystem>().GoozmaDowned;

    bool IItemDropRuleCondition.CanShowItemDropInUI() => true;

    string IProvideItemConditionDescription.GetConditionDescription() => "Drops only on the first kill";
}
