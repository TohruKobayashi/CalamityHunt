using System;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityHunt.Content.Particles;

public class SmokeSplatterParticle : Particle
{
    private int time;

    public int maxTime;

    private int style;

    private int direction;

    private float rotationalVelocity;

    public Func<Vector2> anchor;

    public Vector2 gravity;

    public Color fadeColor;

    public override void OnSpawn()
    {
        style = Main.rand.Next(5);
        direction = Main.rand.NextBool().ToDirectionInt();
        scale *= Main.rand.NextFloat(0.9f, 1.1f);
        rotationalVelocity = Main.rand.NextFloat(0.2f);
        maxTime = (int)(maxTime * 0.66f);
    }

    public override void Update()
    {
        float progress = (float)time / maxTime;

        velocity *= 0.97f;
        velocity += gravity * (0.5f + progress);

        if (time++ > maxTime) {
            ShouldRemove = true;
        }

        if (anchor != null) {
            position += anchor.Invoke();
        }

        rotationalVelocity *= 0.96f;
        rotation += (1f - MathF.Cbrt(progress)) * rotationalVelocity * direction;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        float progress = (float)time / maxTime;

        Texture2D texture = AssetDirectory.Textures.Particle[Type].Value;
        Rectangle frame = texture.Frame(1, 5, 0, style);
        SpriteEffects flip = direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
        Color drawColor = Color.Lerp(color, fadeColor, Utils.GetLerpValue(0f, 0.5f, progress, true));
        float drawScale = scale * MathF.Sqrt(Utils.GetLerpValue(0f, 6f, time, true)) * (0.4f + progress * 0.5f);

        Effect dissolveEffect = AssetDirectory.Effects.FlameDissolve.Value;
        dissolveEffect.Parameters["uTexture0"].SetValue(AssetDirectory.Textures.Noise[9].Value);
        dissolveEffect.Parameters["uTextureScale"].SetValue(new Vector2(2f + scale * 0.07f));
        dissolveEffect.Parameters["uFrameCount"].SetValue(5);
        dissolveEffect.Parameters["uProgress"].SetValue(MathF.Pow(progress, 1.3f));
        dissolveEffect.Parameters["uPower"].SetValue(2f + Utils.GetLerpValue(0.15f, 0.8f, progress, true) * 50f);
        dissolveEffect.Parameters["uNoiseStrength"].SetValue(progress);
        dissolveEffect.CurrentTechnique.Passes[0].Apply();

        Vector2 squish = new Vector2(1f - progress * 0.1f, 1f + progress * 0.1f);
        spriteBatch.Draw(texture, position - Main.screenPosition, frame, drawColor, rotation, frame.Size() * 0.5f, squish * drawScale * 0.5f, flip, 0);

        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }
}
