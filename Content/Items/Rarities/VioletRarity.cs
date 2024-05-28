using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Rarities
{
    public class VioletRarity : ModRarity
    {
        public override Color RarityColor => new Color(108, 45, 199);

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset < 0)
                return ItemRarityID.Blue;
            if (offset > 0)
                return ModContent.RarityType<CalamityRedRarity>();
            return Type;
        }
    }
}
