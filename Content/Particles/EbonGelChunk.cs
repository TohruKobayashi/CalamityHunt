using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityHunt.Content.Particles;

public sealed class EbonGelChunk : BaseGelChunk<EbonGelChunk>
{
    protected override void Draw(SpriteBatch spriteBatch)
    {
        Texture2D texture = TextureAsset.Value;
        Rectangle frame = texture.Frame(4, 1, style, 0);
        Vector2 squish = new Vector2(1f - Velocity.Length() * 0.01f, 1f + Velocity.Length() * 0.01f);
        float grow = (float)Math.Sqrt(Utils.GetLerpValue(-20, 40, time, true));
        if (sticking) {
            grow = 1f;
            frame = texture.Frame(4, 1, style + 2, 0);
            squish = new Vector2(1f + (float)Math.Sqrt(Utils.GetLerpValue(20, 0, time, true)) * 0.33f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(20, 0, time, true)) * 0.33f);
        }

        Color lightColor = Lighting.GetColor(Position.ToTileCoordinates());
        spriteBatch.Draw(texture, Position - Main.screenPosition, frame, Color.Lerp(Color, Color.MultiplyRGBA(lightColor), sticking ? 1f : Utils.GetLerpValue(10, 50, time, true)), Rotation, frame.Size() * new Vector2(0.5f, 0.84f), Scale * grow * squish, 0, 0);
    }
    
    protected override EbonGelChunk NewInstance()
    {
        return new EbonGelChunk();
    }
}
