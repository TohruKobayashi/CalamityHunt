using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Consumable
{
    public class NuclearLemonade : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;

            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

            ItemID.Sets.DrinkParticleColors[Item.type] = new Color[3] {
                new Color(255, 223, 145),// highlight
                new Color(255, 211, 102),// midlight
                new Color(229, 186, 43) // lowlight
            };

            ItemID.Sets.IsFood[Type] = true; 
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(22, 22, BuffID.WellFed3, 79200, true); // 22 minutes
            Item.value = Item.buyPrice(0, 1);
            Item.rare = ItemRarityID.Red;
        }

        public override void OnConsumeItem(Player player)
        {
            // killa you
            if (Main.rand.NextBool(500))
                player.KillMe(PlayerDeathReason.ByCustomReason($"{player.name} must be drinking Lord Vertice Lemonade."), Main.rand.Next(220000000, 229999999), -1);
        }
    }
}
