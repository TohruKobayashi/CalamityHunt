using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Consumables
{
    public class NuclearLemonade : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;

            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

            ItemID.Sets.DrinkParticleColors[Item.type] = new Color[3] {
                new Color(48, 42, 81),// highlight
                new Color(48, 63, 70),// midlight
                new Color(41, 69, 71) // lowlight
            };

            ItemID.Sets.IsFood[Type] = true; //This allows it to be placed on a plate and held correctly
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(22, 22, BuffID.WellFed3, 79200, true); // 22 minutes
            Item.value = Item.buyPrice(0, 1);
            Item.rare = ItemRarityID.Red;
        }

        // If you want multiple buffs, you can apply the remainder of buffs with this method.
        // Make sure the primary buff is set in SetDefaults so that the QuickBuff hotkey can work properly.
        public override void OnConsumeItem(Player player)
        {
            player.AddBuff(BuffID.SugarRush, 3600);
        }
    }
}
