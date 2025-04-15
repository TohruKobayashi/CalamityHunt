using System;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityHunt.Content.Particles;

public sealed class SmokeSplatterParticle : Particle<SmokeSplatterParticle>
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
        
        time = 0;
        maxTime = 0;
        style = 0;
        direction = 0;
        rotationalVelocity = 0f;
        anchor = null;
        gravity = Vector2.Zero;
        fadeColor = default(Color);
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
        Color drawColor = Color.Lerp(Color, fadeColor, Utils.GetLerpValue(0f, 0.5f, progress, true));
        Vector2 drawScale = Scale * MathF.Sqrt(Utils.GetLerpValue(0f, 6f, time, true)) * (0.4f + progress * 0.5f);

        Effect dissolveEffect = AssetDirectory.Effects.FlameDissolve.Value;
        dissolveEffect.Parameters["uTexture0"].SetValue(AssetDirectory.Textures.Noise[9].Value);
        dissolveEffect.Parameters["uTextureScale"].SetValue(new Vector2(2f) + Scale * 0.07f);
        dissolveEffect.Parameters["uFrameCount"].SetValue(5);
        dissolveEffect.Parameters["uProgress"].SetValue(MathF.Pow(progress, 1.3f));
        dissolveEffect.Parameters["uPower"].SetValue(2f + Utils.GetLerpValue(0.15f, 0.8f, progress, true) * 50f);
        dissolveEffect.Parameters["uNoiseStrength"].SetValue(progress);
        dissolveEffect.CurrentTechnique.Passes[0].Apply();

        Vector2 squish = new Vector2(1f - progress * 0.1f, 1f + progress * 0.1f);
        spriteBatch.Draw(texture, Position - Main.screenPosition, frame, drawColor, Rotation, frame.Size() * 0.5f, squish * drawScale * 0.5f, flip, 0);

        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }
    
    protected override SmokeSplatterParticle NewInstance()
    {
        return new SmokeSplatterParticle();
    }
}
