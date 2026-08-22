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

            // hell spike
            /*
            float simulatedDirectionOfCommunity = Vector2.UnitX.RotatedBy(Main.rand.NextFloat(-1.2f, 1.2f)).ToRotation();
            Point pointA = point;
            Point pointB = point;
            pointB += (Vector2.UnitY.RotatedBy(simulatedDirectionOfCommunity + -0.2f).SafeNormalize(Vector2.Zero) * 100).ToPoint();
            pointB = GetPointBetweenTwoPointsBasedOnIntersectionValue(pointA, pointB, Main.maxTilesY - 2);
            Point pointC = point;
            pointC += (Vector2.UnitY.RotatedBy(simulatedDirectionOfCommunity + 0.2f).SafeNormalize(Vector2.Zero) * 100).ToPoint();
            pointC = GetPointBetweenTwoPointsBasedOnIntersectionValue(pointA, pointC, Main.maxTilesY - 2);

            WorldUtils.Gen(point, new Tri(pointA, pointB, pointC), new Actions.SetTile(TileID.SapphireGemspark, true, true));
            */

            // sky spike
            float simulatedDirectionOfCommunity = -Vector2.UnitX.RotatedBy(Main.rand.NextFloat(-1.2f, 1.2f)).ToRotation();

            ShapeData entirePlanetoid = new ShapeData();
            WorldUtils.Gen(point, new Shapes.Circle(16), Actions.Chain(
                new Actions.SetTile(TileID.PoopBlock, true, true), 
                new Actions.Blank().Output(entirePlanetoid)
                ));

            Point pointA = point;
            pointA += (Vector2.UnitY.RotatedBy(simulatedDirectionOfCommunity).SafeNormalize(Vector2.Zero) * 40).ToPoint();
            Point pointB = point;
            pointB += (Vector2.UnitY.RotatedBy(simulatedDirectionOfCommunity + -1.6f).SafeNormalize(Vector2.Zero) * 6).ToPoint();
            Point pointC = point;
            pointC += (Vector2.UnitY.RotatedBy(simulatedDirectionOfCommunity + 1.6f).SafeNormalize(Vector2.Zero) * 6).ToPoint();

            WorldUtils.Gen(point, new Tri(pointA, pointB, pointC), Actions.Chain(
                new Actions.SetTile(TileID.RubyGemspark, true, true),
                new Actions.Blank().Output(entirePlanetoid)
                ));

            WorldUtils.Gen(point, new Shapes.Circle(10), Actions.Chain(
                new Modifiers.Blotches(2, 0.5), 
                new Actions.ClearTile()
                ));

            WorldUtils.Gen(point, new Shapes.Circle(14), new Actions.PlaceWall(WallID.PoopWall));

            WorldUtils.Gen(point, new ModShapes.All(entirePlanetoid), new Actions.SetFrames());

            return true;
        }

        public Point GetPointBetweenTwoPointsBasedOnIntersectionValue(Point a, Point b, int i)
        {
            return new Point(a.X + ((i - a.Y) / (b.Y - a.Y)) * (b.X - a.X), i);
        }
    }
}
