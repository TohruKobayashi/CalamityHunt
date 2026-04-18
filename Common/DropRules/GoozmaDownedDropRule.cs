using CalamityHunt.Common.Systems;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace CalamityHunt.Common.DropRules;

public class GoozmaDownedDropRule : IItemDropRuleCondition
{
    bool IItemDropRuleCondition.CanDrop(DropAttemptInfo info) => !BossDownedSystem.Instance.GoozmaDowned;

    bool IItemDropRuleCondition.CanShowItemDropInUI() => true;

    string IProvideItemConditionDescription.GetConditionDescription() => Language.GetOrRegister($"Mods.CalamityMod.Condition.Drops.FirstKill").Value;
}
