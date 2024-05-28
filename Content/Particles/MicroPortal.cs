using System;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;

namespace CalamityHunt.Content.Particles;

public class MicroPortal : Particle
{
    public Color secondColor;

    public ArmorShaderData shader;

    private float life;

    public int direction;

    public override void OnSpawn()
    {
        direction = Main.rand.NextBool().ToDirectionInt();
    }

    public override void Update()
    {
        life += 0.05f / scale;

        if (life > 1f) {
            ShouldRemove = true;
        }

        rotation += (1f - life * 0.5f) * 0.2f * direction;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        shader?.Shader?.CurrentTechnique.Passes[0].Apply();

        Texture2D texture = AssetDirectory.Textures.Particle[Type].Value;
        Rectangle solidFrame = texture.Frame(1, 3, 0, 0);
        Rectangle colorFrame = texture.Frame(1, 3, 0, 1);
        Rectangle glowFrame = texture.Frame(1, 3, 0, 2);
        float curScale = MathF.Sqrt(Utils.GetLerpValue(0, 0.1f, life, true) * Utils.GetLerpValue(1f, 0.5f, life, true));
        spriteBatch.Draw(texture, position - Main.screenPosition, solidFrame, Color.Black * 0.5f, -rotation * 2f, solidFrame.Size() * 0.5f, scale * 0.9f * curScale * (1f + MathF.Sin(life * 5f) * 0.15f), 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, colorFrame, color * 0.5f, -rotation * 0.7f, colorFrame.Size() * 0.5f, scale * 1.1f * curScale * (1f + MathF.Sin(life * 5f) * 0.1f), 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, solidFrame, Color.Black * 0.5f, rotation * 1.3f, solidFrame.Size() * 0.5f, scale * 0.6f * curScale, 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, colorFrame, color, rotation, colorFrame.Size() * 0.5f, scale * curScale * (1f + MathF.Sin(life * 10f) * 0.05f), 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, glowFrame, secondColor, rotation, glowFrame.Size() * 0.5f, scale * 1.05f * curScale * (1f + MathF.Sin(life * 10f) * 0.05f), 0, 0);

        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }
}
