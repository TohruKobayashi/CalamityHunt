using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.WorldBuilding;

namespace CalamityHunt.Common.World.GenShapes;
public class Tri : GenShape
{
    private Point pointA;
    private Point pointB;
    private Point pointC;

    public Tri(Point pointA, Point pointB, Point pointC)
    {
        this.pointA = pointA;
        this.pointB = pointB;
        this.pointC = pointC;
    }

    public override bool Perform(Point origin, GenAction action)
    {
        Point topLeft = pointA;
        if (topLeft.X > pointB.X)
            topLeft.X = pointB.X;
        if (topLeft.X > pointC.X)
            topLeft.X = pointC.X;
        if (topLeft.Y > pointB.Y)
            topLeft.Y = pointB.Y;
        if (topLeft.Y > pointC.Y)
            topLeft.Y = pointC.Y;
        Point bottomRight = pointA;
        if (bottomRight.X < pointB.X)
            bottomRight.X = pointB.X;
        if (bottomRight.X < pointC.X)
            bottomRight.X = pointC.X;
        if (bottomRight.Y < pointB.Y)
            bottomRight.Y = pointB.Y;
        if (bottomRight.Y < pointC.Y)
            bottomRight.Y = pointC.Y;

        for (int x = topLeft.X; x < bottomRight.X; x++)
        {
            for (int y = topLeft.Y; y < bottomRight.Y; y++)
            {
                if (HUtils.WithinTriangle(pointA, pointB, pointC, new Point(x, y)))
                {
                    if (!UnitApply(action, origin, x, y) && _quitOnFail)
                        return false;
                }
            }
        }

        return true;
    }
}
