using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityHunt.Content.Particles;

public class DivineGelChunk : BaseGelChunk
{
    public override void Draw(SpriteBatch spriteBatch)
    {
        Texture2D texture = AssetDirectory.Textures.Particle[Type].Value;
        Rectangle frame = texture.Frame(4, 2, style, 0);
        Rectangle shineFrame = texture.Frame(4, 2, style, 1);
        Vector2 squish = new Vector2(1f - velocity.Length() * 0.01f, 1f + velocity.Length() * 0.01f);
        float grow = (float)Math.Sqrt(Utils.GetLerpValue(-20, 40, time, true));
        if (sticking) {
            grow = 1f;
            frame = texture.Frame(4, 2, style + 2, 0);
            shineFrame = texture.Frame(4, 2, style + 2, 1);
            squish = new Vector2(1f + (float)Math.Sqrt(Utils.GetLerpValue(20, 0, time, true)) * 0.33f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(20, 0, time, true)) * 0.33f);
        }
        Effect gelEffect = AssetDirectory.Effects.RainbowGel.Value;
        gelEffect.Parameters["uImageSize"].SetValue(texture.Size());
        gelEffect.Parameters["uSourceRect"].SetValue(new Vector4(frame.Left, frame.Top, frame.Width, frame.Height));
        gelEffect.Parameters["uMap"].SetValue(AssetDirectory.Textures.ColorMap[2].Value);
        gelEffect.Parameters["uRbThresholdLower"].SetValue(0.45f);
        gelEffect.Parameters["uRbThresholdUpper"].SetValue(0.55f);
        gelEffect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.5f % 1f);
        gelEffect.Parameters["uRbTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.8f % 1f);
        gelEffect.Parameters["uFrequency"].SetValue(1.1f);
        gelEffect.CurrentTechnique.Passes[0].Apply();

        spriteBatch.Draw(texture, position - Main.screenPosition, frame, color, rotation, frame.Size() * new Vector2(0.5f, 0.84f), scale * grow * squish, 0, 0);

        Main.pixelShader.CurrentTechnique.Passes[0].Apply();

        spriteBatch.Draw(texture, position - Main.screenPosition, shineFrame, new Color(255, 255, 255, 0), rotation, frame.Size() * new Vector2(0.5f, 0.84f), scale * grow * squish, 0, 0);

    }
}
