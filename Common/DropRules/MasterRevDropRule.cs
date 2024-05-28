using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace CalamityHunt.Common.DropRules
{
    public class MasterRevDropRule : IItemDropRuleCondition
    {
        private static bool CheckRevenge()
        {
            return ModCompatibility.Calamity.IsLoaded && (bool)ModCompatibility.Calamity.Mod!.Call("GetDifficultyActive", "revengeance");
        }

        bool IItemDropRuleCondition.CanDrop(DropAttemptInfo info) => CheckRevenge() || Main.masterMode;

        bool IItemDropRuleCondition.CanShowItemDropInUI() => CheckRevenge() || Main.masterMode;

        string IProvideItemConditionDescription.GetConditionDescription()
        {
            // Replicates the behavior of Calamity's "Master or Rev" description
            // The "Master" drop line takes priority if the world is in Master mode
            // The "Revengeance" drop line takes priority otherwise
            // (Unless calamity isn't enabled, then only use the Master drop line)
            return ModCompatibility.Calamity.IsLoaded && !Main.masterMode
                ? Language.GetTextValue("Mods.CalamityMod.Condition.Drops.IsRev")
                : Language.GetTextValue("Bestiary_ItemDropConditions.IsMasterMode");
        }
    }
}
