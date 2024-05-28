using System;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityHunt.Content.Particles;

public class CrossSparkle : Particle
{
    private int time;
    public Func<Vector2> anchor;

    public override void OnSpawn()
    {
        rotation = velocity.ToRotation() + Main.rand.NextFloat(-0.05f, 0.05f);
        velocity = Vector2.Zero;
    }

    public override void Update()
    {
        time++;
        if (time > 15) {
            ShouldRemove = true;
        }

        if (anchor != null) {
            position += anchor.Invoke();
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Texture2D texture = AssetDirectory.Textures.Particle[Type].Value;

        float drawScale = scale * MathF.Pow(Utils.GetLerpValue(15, 6, time, true), 2.5f) * Utils.GetLerpValue(0, 5, time, true);
        Color drawColor = color with { A = 0 };
        spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), color * 0.2f, rotation - MathHelper.PiOver4, texture.Size() * 0.5f, drawScale * new Vector2(0.3f, 0.5f), 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), color * 0.2f, rotation + MathHelper.PiOver4, texture.Size() * 0.5f, drawScale * new Vector2(0.3f, 0.5f), 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), drawColor, rotation - MathHelper.PiOver4, texture.Size() * 0.5f, drawScale * new Vector2(0.6f, 1f), 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), drawColor, rotation + MathHelper.PiOver4, texture.Size() * 0.5f, drawScale * new Vector2(0.6f, 1f), 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), drawColor * 0.2f, rotation - MathHelper.PiOver4, texture.Size() * 0.5f, drawScale * new Vector2(0.4f, 2f), 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), drawColor * 0.2f, rotation + MathHelper.PiOver4, texture.Size() * 0.5f, drawScale * new Vector2(0.4f, 2f), 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), new Color(255, 255, 255, 0), rotation - MathHelper.PiOver4, texture.Size() * 0.5f, drawScale * 0.5f, 0, 0);
        spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), new Color(255, 255, 255, 0), rotation + MathHelper.PiOver4, texture.Size() * 0.5f, drawScale * 0.5f, 0, 0);
    }
}
