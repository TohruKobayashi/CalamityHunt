using CalamityHunt.Common.Systems;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace CalamityHunt.Content.EmoteBubbles
{
	public class GoozmaEmote : ModEmoteBubble
	{
		public override void SetStaticDefaults() {
			AddToCategory(EmoteID.Category.Dangers);
		}

		public override bool IsUnlocked() {
            return BossDownedSystem.Instance.GoozmaDowned;
        }
	}
}
