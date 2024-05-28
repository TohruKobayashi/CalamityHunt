using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.Xna.Framework;

namespace CalamityHunt.Core;

public readonly struct BezierCurve
{
    private readonly List<Vector2> controlPoints;

    public BezierCurve(List<Vector2> controlPoints) => this.controlPoints = controlPoints;

    private Vector2 EvaluateRecursive(List<Vector2> controls, float distance)
    {
        if (controls.Count <= 2)
            return Vector2.Lerp(controls[0], controls[1], distance);

        else
        {
            List<Vector2> nextPoints = new List<Vector2>();

            for (int i = 0; i < controls.Count - 1; ++i)
                nextPoints.Add(Vector2.Lerp(controls[i], controls[i + 1], distance));

            return EvaluateRecursive(nextPoints, distance);
        }
    }

    [Pure]
    public Vector2 GetSinglePoint(float proportionalDistance) => EvaluateRecursive(controlPoints, Math.Clamp(proportionalDistance, 0f, 1f));

    [Pure]
    public List<Vector2> GetPoints(int pointCount)
    {
        float interval = 1f / pointCount;

        List<Vector2> points = new List<Vector2>();

        for (float i = 0f; i <= 1f; i += interval)
        {
            Vector2 point = GetSinglePoint(i);
            points.Add(point);
        }

        return points;
    }

    [Pure]
    public float Distance(int pointCount)
    {
        float interval = 1f / pointCount;

        float totalDistance = 0f;

        for (float i = interval; i <= 1f; i += interval)
            totalDistance += (GetSinglePoint(i) - GetSinglePoint(i - interval)).Length();

        return totalDistance;
    }

    [Pure]
    public Vector2 this[int x] => controlPoints[x];
}
