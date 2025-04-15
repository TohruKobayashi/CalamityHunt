using System;

using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityHunt.Content.Particles;

public sealed class SmokeSplatterMetaball : Particle<SmokeSplatterMetaball>
{
    private int time;

    public int maxTime;

    private int style;

    private int direction;

    private float rotationalVelocity;

    public Func<Vector2> anchor;

    public Vector2 gravity;

    public Color fadeColor;
    
    public override void FetchFromPool()
    {
        base.FetchFromPool();
    }

    public override void OnSpawn()
    {
        style = Main.rand.Next(5);
        direction = Main.rand.NextBool().ToDirectionInt();
        Scale *= Main.rand.NextFloat(0.9f, 1.1f);
        rotationalVelocity = Main.rand.NextFloat(0.2f);
        maxTime = (int)(maxTime * 0.66f);
    }

    protected override void Update()
    {
        float progress = (float)time / maxTime;

        Velocity *= 0.97f;
        Velocity += gravity * (0.5f + progress);

        if (time++ > maxTime) {
            ShouldBeRemovedFromRenderer = true;
        }

        if (anchor != null) {
            Position += anchor.Invoke();
        }

        rotationalVelocity *= 0.96f;
        Rotation += (1f - MathF.Cbrt(progress)) * rotationalVelocity * direction;
    }

    protected override void Draw(SpriteBatch spriteBatch)
    {
        float progress = (float)time / maxTime;

        Texture2D texture = TextureAsset.Value;
        Rectangle frame = texture.Frame(1, 5, 0, style);
        SpriteEffects flip = direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
        Color scaleProgress = new Color(Scale.X, progress, 0, 1);
        Vector2 drawScale = Scale * MathF.Sqrt(Utils.GetLerpValue(0f, 6f, time, true)) * (0.4f + progress * 0.5f);

        Vector2 squish = new Vector2(1f - progress * 0.1f, 1f + progress * 0.1f);
        spriteBatch.Draw(texture, Position - Main.screenPosition, frame, scaleProgress, Rotation, frame.Size() * 0.5f, squish * drawScale * 0.5f, flip, 0);
    }
    
    protected override SmokeSplatterMetaball NewInstance()
    {
        return new SmokeSplatterMetaball();
    }
}
