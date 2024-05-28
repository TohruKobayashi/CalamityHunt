using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;

namespace CalamityHunt.Content.Particles;

public class MicroShockwave : Particle
{
    private float scaleLife;

    public Color secondColor;

    public ArmorShaderData shader;

    public override void OnSpawn()
    {
        rotation = velocity.ToRotation();
        velocity = Vector2.Zero;
    }

    public override void Update()
    {
        scaleLife += (scale - scaleLife * 0.8f) * 0.09f;
        if (scaleLife > scale) {
            ShouldRemove = true;
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        shader?.Shader?.CurrentTechnique.Passes[0].Apply();

        Texture2D texture = AssetDirectory.Textures.Particle[Type].Value;
        Rectangle solidFrame = texture.Frame(1, 3, 0, 0);
        Rectangle colorFrame = texture.Frame(1, 3, 0, 1);
        Rectangle glowFrame = texture.Frame(1, 3, 0, 2);
        float drawScale = Utils.GetLerpValue(scale, scale * 0.7f, scaleLife, true);
        spriteBatch.Draw(texture, position - Main.screenPosition, solidFrame, Color.Black * 0.1f * drawScale, rotation, solidFrame.Size() * 0.5f, new Vector2(scaleLife, scaleLife * 0.5f), 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, colorFrame, color * drawScale, rotation, colorFrame.Size() * 0.5f, new Vector2(scaleLife, scaleLife * 0.5f), 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, glowFrame, secondColor * drawScale, rotation, glowFrame.Size() * 0.5f, new Vector2(scaleLife, scaleLife * 0.5f), 0, 0);

        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }
}
