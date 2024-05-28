using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityHunt.Content.Particles;

public abstract class BaseGelChunk : Particle
{
    public int style;

    public int time;

    public bool sticking;

    public override void OnSpawn()
    {
        scale *= Main.rand.NextFloat(1f, 1.2f);
        style = Main.rand.Next(2);
    }

    public override void Update()
    {
        time++;

        if (!sticking) {
            if (velocity.Y < 30) {
                velocity += new Vector2(0, 0.5f);
            }

            rotation = velocity.ToRotation() - MathHelper.PiOver2;

            if (Collision.IsWorldPointSolid(position + velocity) && time > 2) {
                time = 0;
                sticking = true;
                position = new Vector2(position.X, (int)(position.Y / 16f) * 16 + 16);
                for (int i = 0; i < 8; i++) {
                    if (Collision.IsWorldPointSolid(position + velocity - new Vector2(0, 8 * i))) {
                        position -= new Vector2(0, 8);
                    }
                }
                position -= new Vector2(0, 3);
            }
        }
        else {
            rotation = 0;
            velocity = Vector2.Zero;
            if (time > 10) {
                scale *= 0.95f;
            }

            if (scale < 0.1f) {
                ShouldRemove = true;
            }
        }
    }
}
