using System;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;

namespace CalamityHunt.Content.Particles;

public sealed class MicroPortal : Particle<MicroPortal>
{
    public Color secondColor;

    public ArmorShaderData shader;

    private float life;

    public int direction;
    
    public override void FetchFromPool()
    {
        base.FetchFromPool();

        secondColor = default(Color);
        shader = null;
        life = 0f;
        direction = 0;
    }

    public override void OnSpawn()
    {
        direction = Main.rand.NextBool().ToDirectionInt();
    }

    protected override void Update()
    {
        life += 0.05f / Scale.X;

        if (life > 1f) {
            ShouldBeRemovedFromRenderer = true;
        }

        Rotation += (1f - life * 0.5f) * 0.2f * direction;
    }

    protected override void Draw(SpriteBatch spriteBatch)
    {
        shader?.Shader?.CurrentTechnique.Passes[0].Apply();

        Texture2D texture = TextureAsset.Value;
        Rectangle solidFrame = texture.Frame(1, 3, 0, 0);
        Rectangle colorFrame = texture.Frame(1, 3, 0, 1);
        Rectangle glowFrame = texture.Frame(1, 3, 0, 2);
        float curScale = MathF.Sqrt(Utils.GetLerpValue(0, 0.1f, life, true) * Utils.GetLerpValue(1f, 0.5f, life, true));
        spriteBatch.Draw(texture, Position - Main.screenPosition, solidFrame, Color.Black * 0.5f, -Rotation * 2f, solidFrame.Size() * 0.5f, Scale * 0.9f * curScale * (1f + MathF.Sin(life * 5f) * 0.15f), 0, 0);
        spriteBatch.Draw(texture, Position - Main.screenPosition, colorFrame, Color * 0.5f, -Rotation * 0.7f, colorFrame.Size() * 0.5f, Scale * 1.1f * curScale * (1f + MathF.Sin(life * 5f) * 0.1f), 0, 0);
        spriteBatch.Draw(texture, Position - Main.screenPosition, solidFrame, Color.Black * 0.5f, Rotation * 1.3f, solidFrame.Size() * 0.5f, Scale * 0.6f * curScale, 0, 0);
        spriteBatch.Draw(texture, Position - Main.screenPosition, colorFrame, Color, Rotation, colorFrame.Size() * 0.5f, Scale * curScale * (1f + MathF.Sin(life * 10f) * 0.05f), 0, 0);
        spriteBatch.Draw(texture, Position - Main.screenPosition, glowFrame, secondColor, Rotation, glowFrame.Size() * 0.5f, Scale * 1.05f * curScale * (1f + MathF.Sin(life * 10f) * 0.05f), 0, 0);

        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }
    
    protected override MicroPortal NewInstance()
    {
        return new MicroPortal();
    }
}
