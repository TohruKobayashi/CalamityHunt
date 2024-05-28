using System;
using System.Diagnostics.Contracts;
using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityHunt.Common.Utilities;

/// <summary>
///     A utility that can hold an arbitrary array of <see cref="Color"/>s with
///     the ability to interpolate between them.
/// </summary>
public readonly struct GradientColor
{
    private readonly Color[] colors;
    private readonly float timePerColor;
    private readonly float fadeSpeed;

    public GradientColor(Color[] colors, float timePerColor = 1, float fadeSpeed = 1)
    {
        this.colors = colors;
        this.timePerColor = timePerColor * 60;
        this.fadeSpeed = fadeSpeed * 60;
        if (this.fadeSpeed > this.timePerColor) {
            this.fadeSpeed = this.timePerColor;
        }
    }

    /// <summary>
    ///     Calculates the <see cref="Color"/> at the given time.
    /// </summary>
    /// <param name="time">The time to calculate at.</param>
    /// <returns>The calculated color.</returns>
    [Pure]
    public Color ValueAt(float time)
    {
        float t = time % timePerColor / fadeSpeed;
        int index = (int)(Math.Abs(time) / timePerColor) % colors.Length;
        return Color.Lerp(colors[index], colors[(index + 1) % colors.Length], t);
    }

    /// <summary>
    ///     Gets the <see cref="ValueAt"/> the current game time.
    /// </summary>
    [Pure]
    public Color Value => ValueAt(Main.GlobalTimeWrappedHourly * 60);
}
