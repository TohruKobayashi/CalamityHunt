using System;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityHunt.Content.Particles;

public sealed class PrettySparkle : Particle<PrettySparkle>
{
    private int time;
    
    public override void FetchFromPool()
    {
        base.FetchFromPool();

        time = 0;
    }

    public override void OnSpawn()
    {
        Scale *= Main.rand.NextFloat(0.9f, 1.1f);
        Velocity *= Main.rand.NextFloat(0.9f, 1.1f);
        Rotation *= 0.05f;
    }

    protected override void Update()
    {
        base.Update();
        
        Velocity *= 0.95f;
        time++;

        if (time > 40 + Scale.X) {
            Scale *= 0.8f + Math.Min(Scale.X * 0.2f, 0.18f);
        }

        if (Scale.X < 0.1f) {
            ShouldBeRemovedFromRenderer = true;
        }
    }

    protected override void Draw(SpriteBatch spriteBatch)
    {
        Texture2D texture = TextureAsset.Value;
        Vector2 drawScale = Scale * (float)Math.Sqrt(Utils.GetLerpValue(-5, 10, time, true));
        Color drawColor = Color.Lerp(Color, Color.White, 0.6f) with { A = 0 };
        spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color with { A = (byte)(Color.A / 2) }, Rotation, texture.Size() * 0.5f, drawScale * 0.6f, 0, 0);
        spriteBatch.Draw(texture, Position - Main.screenPosition, null, drawColor, Rotation, texture.Size() * 0.5f, drawScale * 0.3f, 0, 0);
    }
    
    protected override PrettySparkle NewInstance()
    {
        return new PrettySparkle();
    }
}
