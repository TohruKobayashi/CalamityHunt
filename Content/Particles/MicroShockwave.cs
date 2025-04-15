using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;

namespace CalamityHunt.Content.Particles;

public sealed class MicroShockwave : Particle<MicroShockwave>
{
    private float scaleLife;

    public Color secondColor;

    public ArmorShaderData shader;
    
    public override void FetchFromPool()
    {
        base.FetchFromPool();

        scaleLife = 0f;
        secondColor = default(Color);
        shader = null;
    }

    public override void OnSpawn()
    {
        Rotation = Velocity.ToRotation();
        Velocity = Vector2.Zero;
    }

    protected override void Update()
    {
        base.Update();
        
        scaleLife += (Scale.X - scaleLife * 0.8f) * 0.09f;
        if (scaleLife > Scale.X) {
            ShouldBeRemovedFromRenderer = true;
        }
    }

    protected override void Draw(SpriteBatch spriteBatch)
    {
        shader?.Shader?.CurrentTechnique.Passes[0].Apply();

        Texture2D texture = TextureAsset.Value;
        Rectangle solidFrame = texture.Frame(1, 3, 0, 0);
        Rectangle colorFrame = texture.Frame(1, 3, 0, 1);
        Rectangle glowFrame = texture.Frame(1, 3, 0, 2);
        float drawScale = Utils.GetLerpValue(Scale.X, Scale.X * 0.7f, scaleLife, true);
        spriteBatch.Draw(texture, Position - Main.screenPosition, solidFrame, Color.Black * 0.1f * drawScale, Rotation, solidFrame.Size() * 0.5f, new Vector2(scaleLife, scaleLife * 0.5f), 0, 0);
        spriteBatch.Draw(texture, Position - Main.screenPosition, colorFrame, Color * drawScale, Rotation, colorFrame.Size() * 0.5f, new Vector2(scaleLife, scaleLife * 0.5f), 0, 0);
        spriteBatch.Draw(texture, Position - Main.screenPosition, glowFrame, secondColor * drawScale, Rotation, glowFrame.Size() * 0.5f, new Vector2(scaleLife, scaleLife * 0.5f), 0, 0);

        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }
    
    protected override MicroShockwave NewInstance()
    {
        return new MicroShockwave();
    }
}
