using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityHunt.Content.Particles;

public class StellarGelChunk : BaseGelChunk
{
    public override void Draw(SpriteBatch spriteBatch)
    {
        Texture2D texture = AssetDirectory.Textures.Particle[Type].Value;
        Rectangle frame = texture.Frame(4, 2, style, 0);
        Rectangle glowFrame = texture.Frame(4, 2, style, 1);
        Vector2 squish = new Vector2(1f - velocity.Length() * 0.01f, 1f + velocity.Length() * 0.01f);
        float grow = (float)Math.Sqrt(Utils.GetLerpValue(-20, 40, time, true));
        if (sticking) {
            grow = 1f;
            frame = texture.Frame(4, 2, style + 2, 0);
            glowFrame = texture.Frame(4, 2, style + 2, 1);
            squish = new Vector2(1f + (float)Math.Sqrt(Utils.GetLerpValue(20, 0, time, true)) * 0.33f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(20, 0, time, true)) * 0.33f);
        }
        spriteBatch.Draw(texture, position - Main.screenPosition, glowFrame, new Color(10, 30, 110, 0) * 0.4f, rotation, glowFrame.Size() * new Vector2(0.5f, 0.87f), scale * grow * squish * new Vector2(1.3f, 1.5f), 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, frame, color, rotation, frame.Size() * new Vector2(0.5f, 0.84f), scale * grow * squish, 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, frame, new Color(60, 40, 35, 0).MultiplyRGBA(color), rotation, frame.Size() * new Vector2(0.5f, 0.84f), scale * 1.2f * grow * squish, 0, 0);
    }
}
