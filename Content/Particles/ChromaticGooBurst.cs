using System;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityHunt.Content.Particles;

public class ChromaticGooBurst : Particle
{
    public ColorOffsetData colorData;

    public int style;

    private int frame;

    private int frameCounter;

    public override void OnSpawn()
    {
        scale *= Main.rand.NextFloat(0.9f, 1.1f);
        style = Main.rand.Next(2);
        rotation = velocity.ToRotation() + MathHelper.PiOver2;
        velocity = Vector2.Zero;
    }

    public override void Update()
    {
        frameCounter++;

        if (frame < 3) {
            frameCounter++;
        }

        if (frameCounter % 4 == 0) {
            frame++;
        }

        if (frame > 7) {
            ShouldRemove = true;
        }

        if (colorData.active) {
            color = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(colorData.offset + Math.Max(0, frameCounter - 18) * 3f - 2f);
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Texture2D texture = AssetDirectory.Textures.Particle[Type].Value;
        Rectangle drawFrame = texture.Frame(8, 4, frame, style);
        Rectangle glowFrame = texture.Frame(8, 4, frame, style + 2);

        spriteBatch.Draw(texture, position - Main.screenPosition, drawFrame, Color.LightGray, rotation, drawFrame.Size() * new Vector2(0.5f, 1f), scale, 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, glowFrame, color, rotation, glowFrame.Size() * new Vector2(0.5f, 1f), scale, 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, glowFrame, (color * 0.5f) with { A = 0 }, rotation, glowFrame.Size() * new Vector2(0.5f, 1f), scale * 1.01f, 0, 0);
    }
}
