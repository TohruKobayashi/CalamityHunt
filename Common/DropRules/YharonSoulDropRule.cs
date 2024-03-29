using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityHunt.Common.Players;
using CalamityHunt.Common.Systems;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace CalamityHunt.Common.DropRules;
public class YharonSoulDropRule : IItemDropRuleCondition
{
    bool IItemDropRuleCondition.CanDrop(DropAttemptInfo info) => Main.player.Any(p => p.active && !p.GetModPlayer<AuricSoulPlayer>().yharonSoul);

    bool IItemDropRuleCondition.CanShowItemDropInUI() => true;

    string IProvideItemConditionDescription.GetConditionDescription() => Language.GetOrRegister($"{nameof(CalamityHunt)}.Common.DropWhenAnyPlayerHasntEaten").Value;
}
