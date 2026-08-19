using CalamityHunt.Common.Players;
using CalamityHunt.Common.World.GenShapes;
using CalamityHunt.Content.Items.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityHunt.Content.Items.Accessories
{
    public class TheCommunity : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 58;
            Item.value = Item.sellPrice(gold: 15);
            Item.accessory = true;
            Item.rare = ItemRarityID.Expert;

            //REMOVE THIS WHEN NOT DEBUGGING
            Item.useStyle = ItemUseStyleID.Swing;
        }

        public override bool? UseItem(Player player)
        {
            Vector2 mouseTilePos = Main.MouseWorld / 16;
            Point point = new Point((int)mouseTilePos.X, (int)mouseTilePos.Y);

            Dust.NewDust(Main.MouseWorld, 5, 5, DustID.AmberBolt);

            //float simulatedDirectionOfCommunity = Vector2.UnitX.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)).ToRotation(); //would be directionfrom
            float simulatedDirectionOfCommunity = Vector2.UnitX.ToRotation();
            Point pointA = point;
            Point pointB = point;
            pointB += (Vector2.UnitY.RotatedBy(simulatedDirectionOfCommunity + -0.2f) * 1000).ToPoint();
            if (pointB.Y > Main.maxTilesY - 2)
                pointB.Y = Main.maxTilesY - 2;
            Point pointC = point;
            pointC += (Vector2.UnitY.RotatedBy(simulatedDirectionOfCommunity + 0.2f) * 1000).ToPoint();
            if (pointC.Y > Main.maxTilesY - 2)
                pointC.Y = Main.maxTilesY - 2;
            
            WorldUtils.Gen(point, new Tri(pointA, pointB, pointC), new Actions.SetTile(TileID.Obsidian, true, true));

            return true;
        }
    }
}
