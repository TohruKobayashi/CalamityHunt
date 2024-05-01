using CalamityHunt.Common.Systems;
using Terraria.GameContent.ItemDropRules;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Common.DropRules
{
    public class MasterRevDropRule : IItemDropRuleCondition
    {
        bool IItemDropRuleCondition.CanDrop(DropAttemptInfo info)
        {
            if (ModCompatibility.Calamity.IsLoaded)
                return (bool)ModCompatibility.Calamity.Mod!.Call("GetDifficultyActive", "revengeance") || Main.masterMode;
            else
                return Main.masterMode;
        }

        bool IItemDropRuleCondition.CanShowItemDropInUI() => false;

        string IProvideItemConditionDescription.GetConditionDescription() => ModCompatibility.Calamity.IsLoaded ? "Drops in Revengeance or Master Mode" : "Drops in Master Mode";
    }
}
