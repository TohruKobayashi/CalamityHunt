﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityHunt.Core;

public struct LightningData
{
    public LightningData(Vector2 origin, Vector2 end, float strength = 1f, int overridePoints = 0)
    {
        if (overridePoints > 0)
            pointCount = overridePoints + 2;
        else
            pointCount = (int)(origin.Distance(end) / 90f) + Main.rand.Next(2, 5);

        this.origin = origin;
        this.mid = Vector2.Lerp(origin, end, 0.5f);
        this.end = end;
        this.strength = strength;
    }    
    
    public LightningData(Vector2 origin, Vector2 mid, Vector2 end, float strength = 1f, int overridePoints = 0)
    {
        if (overridePoints > 0)
            pointCount = overridePoints + 2;
        else
            pointCount = (int)((origin.Distance(mid) + mid.Distance(end)) / 180f) + Main.rand.Next(2, 5);

        this.origin = origin;
        this.mid = mid;
        this.end = end;
        this.strength = strength;
    }

    public Vector2 origin, mid, end;
    public float strength;
    private int pointCount;

    private List<Vector2> Calculate()
    {
        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i < pointCount; i++)
        {
            Vector2 a = Vector2.Lerp(origin, mid, (float)i / (pointCount - 1));
            Vector2 b = Vector2.Lerp(mid, end, (float)i / (pointCount - 1));
            points.Add(Vector2.Lerp(a, b, (float)i / (pointCount - 1)));
        }

        for (int i = 1; i < pointCount - 1; i++)
            points[i] += Main.rand.NextVector2Circular(8, 16).RotatedBy(points[i].AngleTo(points[i + 1])) * strength;

        return points;
    }

    public List<Vector2> Value => Calculate();
}
