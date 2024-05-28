using System;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityHunt.Content.Particles;

public class ChromaticGelChunk : BaseGelChunk
{
    public ColorOffsetData colorData;

    public override void Draw(SpriteBatch spriteBatch)
    {
        Texture2D texture = AssetDirectory.Textures.Particle[Type].Value;
        Rectangle frame = texture.Frame(4, 1, style, 0);
        Vector2 squish = new Vector2(1f - velocity.Length() * 0.01f, 1f + velocity.Length() * 0.01f);
        float grow = (float)Math.Sqrt(Utils.GetLerpValue(-20, 40, time, true));
        if (sticking) {
            grow = 1f;
            frame = texture.Frame(4, 1, style + 2, 0);
            squish = new Vector2(1f + (float)Math.Sqrt(Utils.GetLerpValue(20, 0, time, true)) * 0.33f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(20, 0, time, true)) * 0.33f);
        }

        Goozma.GetGradientMapValues(out float[] brightnesses, out Vector3[] colors);
        Effect effect = AssetDirectory.Effects.HolographicGel.Value;
        effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly % 1f);
        effect.Parameters["colors"].SetValue(colors);
        effect.Parameters["brightnesses"].SetValue(brightnesses);
        effect.Parameters["baseToScreenPercent"].SetValue(sticking ? 0f : Utils.GetLerpValue(70, 30, time, true));
        effect.Parameters["baseToMapPercent"].SetValue(0f);
        effect.CurrentTechnique.Passes[0].Apply();

        spriteBatch.Draw(texture, position - Main.screenPosition, frame, color, rotation, frame.Size() * new Vector2(0.5f, 0.84f), scale * grow * squish, 0, 0);

        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }
}
