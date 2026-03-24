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
            // There is currently no text in Infernum about drop conditions for its mode
            // I've reported this, and once it's implemented, the custom key should be replaced with Infernum's key
            return Language.GetTextValue("Bestiary_ItemDropConditions.SimpleCondition", Language.GetOrRegister($"Mods.{nameof(CalamityHunt)}.InfernumOnly").Value);
        }
    }
}
