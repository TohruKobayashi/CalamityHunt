using System;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityHunt.Content.Particles;

public sealed class FlameParticle : Particle<FlameParticle>
{
    public int time;

    public int maxTime;

    public int style;

    private int direction;

    private float rotationalVelocity;

    public Func<Vector2> anchor;

    public Vector2 gravity;

    public Color fadeColor;

    public bool emitLight;
    
    public override bool RequiresImmediateMode => true;

    public override void FetchFromPool()
    {
        base.FetchFromPool();

        time = 0;
        maxTime = 0;
        style = 0;
        direction = 0;
        rotationalVelocity = 0f;
        anchor = null;
        gravity = Vector2.Zero;
        fadeColor = default(Color);
        emitLight = false;
    }

    public override void OnSpawn()
    {
        style = Main.rand.Next(15);
        direction = Main.rand.NextBool().ToDirectionInt();
        Scale *= Main.rand.NextFloat(0.9f, 1.1f);
        maxTime = (maxTime <= 0) ? Main.rand.Next(50, 80) : maxTime;
        rotationalVelocity = Main.rand.NextFloat(-0.1f, 0.2f);
    }

    protected override void Update()
    {
        base.Update();
        
        float progress = (float)time / maxTime;

        Velocity *= 0.97f - progress * 0.2f;
        Velocity += gravity;

        if (time++ > maxTime) {
            ShouldBeRemovedFromRenderer = true;
        }

        if (anchor != null) {
            Position += anchor.Invoke();
        }

        Rotation += (1f - MathF.Cbrt(progress)) * rotationalVelocity * direction;

        if (emitLight) {
            Lighting.AddLight(Position, fadeColor.ToVector3() * Utils.GetLerpValue(0.5f, 0, progress, true));
        }
    }

    protected override void Draw(SpriteBatch spriteBatch)
    {
        float progress = (float)time / maxTime;

        Texture2D texture = TextureAsset.Value;
        Texture2D glow = AssetDirectory.Textures.Glow[1].Value;
        Rectangle frame = texture.Frame(1, 15, 0, style);
        SpriteEffects flip = direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
        Color drawColor = Color.Lerp(Color, fadeColor, Utils.GetLerpValue(0f, 0.4f, progress, true));
        Vector2 drawScale = Scale * MathF.Sqrt(Utils.GetLerpValue(0f, 2f, time, true)) * (0.7f + progress * 0.6f);

        spriteBatch.Draw(glow, Position - Main.screenPosition, glow.Frame(), fadeColor * 0.2f * Utils.GetLerpValue(0.6f, 0.1f, progress, true), Rotation, glow.Size() * 0.5f, drawScale * 0.25f, 0, 0);

        Effect dissolveEffect = AssetDirectory.Effects.FlameDissolve.Value;
        dissolveEffect.Parameters["uTexture0"].SetValue(AssetDirectory.Textures.Noise[9].Value);
        dissolveEffect.Parameters["uTextureScale"].SetValue(new Vector2(1f) + Scale * 0.06f);
        dissolveEffect.Parameters["uFrameCount"].SetValue(15);
        dissolveEffect.Parameters["uProgress"].SetValue(MathF.Pow(progress, 0.6f));
        dissolveEffect.Parameters["uPower"].SetValue(10f + progress * 70f);
        dissolveEffect.Parameters["uNoiseStrength"].SetValue(1.1f);
        dissolveEffect.CurrentTechnique.Passes[0].Apply();

        Vector2 squish = new Vector2(1f + MathF.Sin(progress * 4f) * 0.1f, 1f + MathF.Cos(progress * 4f) * 0.1f);
        spriteBatch.Draw(texture, Position - Main.screenPosition, frame, drawColor, Rotation, frame.Size() * 0.5f, squish * drawScale * 0.45f, flip, 0);

        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }
    
    protected override FlameParticle NewInstance()
    {
        return new FlameParticle();
    }
}
