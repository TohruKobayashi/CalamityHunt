using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityHunt.Common.Systems.Particles;

public class ParticleSystem
{
    public ParticleSystem()
    {
        particles = new HashSet<Particle>();
    }

    private HashSet<Particle> particles;

    public void Add(Particle particle)
    {
        if (particles != null) particles.Add(particle);
    }

    public void Clear() => particles.Clear();

    public void Update()
    {
        if (Main.dedServ) {
            if (particles.Count > 0) {
                particles.Clear();
            }
            return;
        }

        foreach (Particle particle in particles.ToHashSet()) {
            if (particle != null) {
                particle.Update();
                particle.position += particle.velocity;
                if (particle.ShouldRemove) {
                    particles.Remove(particle);
                }
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, bool begin = true)
    {
        if (Main.dedServ) {
            return;
        }

        if (!begin) {
            spriteBatch.End();
        }

        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

        Rectangle checkRect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.ScreenSize.X, Main.ScreenSize.Y);
        Rectangle particleRect = new Rectangle(0, 0, 400, 400);

        foreach (Particle particle in particles.ToHashSet()) {

            if (particle != null) {
                float halfSize = 200 * particle.scale;
                particleRect.X = (int)(particle.position.X - halfSize);
                particleRect.Y = (int)(particle.position.Y - halfSize);
                particleRect.Width = (int)(halfSize * 2f);
                particleRect.Height = (int)(halfSize * 2f);

                if (checkRect.Intersects(particleRect)) {
                    particle.Draw(spriteBatch);
                }
            }
        }

        spriteBatch.End();

        if (!begin) {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        }
    }
}
