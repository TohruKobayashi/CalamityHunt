using System;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityHunt.Content.Particles;

public sealed class LightningParticle : Particle<LightningParticle>
{
    private int time;

    public int maxTime;

    private int style;

    private int direction;

    public Func<Vector2> anchor;

    public float flickerSpeed;
    
    public override void FetchFromPool()
    {
        base.FetchFromPool();
    }

    public override void OnSpawn()
    {
        style = Main.rand.Next(10);
        direction = Main.rand.NextBool().ToDirectionInt();
        maxTime = maxTime <= 0 ? Main.rand.Next(3, 6) : maxTime;
    }

    protected override void Update()
    {
        Velocity *= 0.98f;

        if (time++ > maxTime) {
            ShouldBeRemovedFromRenderer = true;
        }

        if (anchor != null) {
            Position += anchor.Invoke();
        }
    }

    protected override void Draw(SpriteBatch spriteBatch)
    {
        Texture2D texture = TextureAsset.Value;
        Rectangle frame = texture.Frame(1, 10, 0, style);
        SpriteEffects flip = direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
        Color drawColor = Color * (0.8f + MathF.Sin(time * flickerSpeed) * 0.2f);

        Effect dissolveEffect = AssetDirectory.Effects.FlameDissolve.Value;
        dissolveEffect.Parameters["uTexture0"].SetValue(AssetDirectory.Textures.Noise[9].Value);
        dissolveEffect.Parameters["uTextureScale"].SetValue(new Vector2(0.7f) + Scale * 0.05f);
        dissolveEffect.Parameters["uFrameCount"].SetValue(10);
        dissolveEffect.Parameters["uProgress"].SetValue(Utils.GetLerpValue(maxTime / 3f, maxTime, time, true));
        dissolveEffect.Parameters["uPower"].SetValue(4f + Utils.GetLerpValue(maxTime / 4f, maxTime / 3f, time, true) * 40f);
        dissolveEffect.Parameters["uNoiseStrength"].SetValue(1f);
        dissolveEffect.CurrentTechnique.Passes[0].Apply();

        spriteBatch.Draw(texture, Position - Main.screenPosition, frame, drawColor, Rotation + MathHelper.Pi / 3f * direction, frame.Size() * 0.5f, Scale * new Vector2(1f, 1f + time * 0.05f) * 0.5f, flip, 0);

        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }
    
    protected override LightningParticle NewInstance()
    {
        return new LightningParticle();
    }
}
