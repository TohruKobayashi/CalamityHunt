using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityHunt.Common.DropRules
{
    public class InfernumDropRule : IItemDropRuleCondition
    {
        private static bool CheckInfernum()
        {
            return ModLoader.HasMod(HUtils.InfernumMode) && (bool)ModLoader.GetMod(HUtils.InfernumMode).Call("GetInfernumActive");
        }

        bool IItemDropRuleCondition.CanDrop(DropAttemptInfo info) => CheckInfernum();

        bool IItemDropRuleCondition.CanShowItemDropInUI() => CheckInfernum();

        string IProvideItemConditionDescription.GetConditionDescription()
        {
            return Language.GetTextValue("Mods.InfernumMode.UI.InfernumDrop");
        }
    }
}
