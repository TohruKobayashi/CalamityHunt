using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.Graphics.Renderers;

namespace CalamityHunt.Common.Systems.Particles;

public abstract class ParticleRenderer
{
    protected List<IGoozParticle> particles = [];
    protected ParticleRendererSettings settings;

    public bool ShouldRestart { get; set; }

    public Effect Effect { get; set; }
    
    public IEnumerable<IGoozParticle> Particles => particles;

    public static ParticleRenderer MakeDefaultRenderer()
    {
        return Main.dedServ ? new NoOpParticleRenderer() : new DefaultParticleRenderer();
    }

    public T SpawnParticle<T>(Action<T> initializer) where T : Particle<T>
    {
        var particle = Particle<T>.RequestParticle();
        initializer(particle);
        particle.OnSpawn();
        
        Add(particle);
        return particle;
    }

    public virtual void Add(IGoozParticle particle)
    {
        particles.Add(particle);
    }

    public virtual void Clear()
    {
        particles.Clear();
    }

    public virtual void Update()
    {
        for (var i = 0; i < particles.Count; i++) {
            var particle = particles[i];

            if (particle.ShouldBeRemovedFromRenderer) {
                particle.RestInPool();
                particles.RemoveAt(i);
                i--;
            }
            else {
                particle.Update(ref settings);
            }
        }
    }

    public abstract void Draw(SpriteBatch sb);
}

public sealed class NoOpParticleRenderer : ParticleRenderer
{
    public override void Add(IGoozParticle particle) { }

    public override void Clear() { }

    public override void Update() { }

    public override void Draw(SpriteBatch sb) { }
}

public sealed class DefaultParticleRenderer : ParticleRenderer
{
    public override void Draw(SpriteBatch sb)
    {
        if (ShouldRestart) {
            sb.End();
        }

        var requiresImmediateMode = Effect is not null || particles.Any(x => x.RequiresImmediateMode);
        sb.Begin(
            requiresImmediateMode ? SpriteSortMode.Immediate : SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            Main.DefaultSamplerState,
            DepthStencilState.None,
            Main.Rasterizer,
            Effect,
            Main.Transform
        );

        foreach (var particle in particles) {
            particle.Draw(ref settings, sb);
        }

        sb.End();

        if (ShouldRestart) {
            sb.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                Main.DefaultSamplerState,
                DepthStencilState.None,
                Main.Rasterizer,
                null,
                Main.Transform
            );
        }
    }
}
