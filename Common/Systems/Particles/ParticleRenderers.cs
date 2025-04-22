using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.Graphics.Renderers;

namespace CalamityHunt.Common.Systems.Particles;

public abstract class ParticleRenderer
{
    public bool ShouldRestart { get; set; }

    public Effect Effect { get; set; }

    public abstract IEnumerable<IGoozParticle> Particles { get; }

    protected ParticleRendererSettings Settings;

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

    protected abstract void Add(IGoozParticle particle);

    protected abstract void Clear();

    public abstract void Update();

    public abstract void Draw(SpriteBatch sb);
}

public sealed class NoOpParticleRenderer : ParticleRenderer
{
    public override IEnumerable<IGoozParticle> Particles
    {
        get
        {
            yield break;
        }
    }

    protected override void Add(IGoozParticle particle) { }

    protected override void Clear() { }

    public override void Update() { }

    public override void Draw(SpriteBatch sb) { }
}

public sealed class DefaultParticleRenderer : ParticleRenderer
{
    private readonly List<IGoozParticle> deferredParticles = [];
    private readonly List<IGoozParticle> immediateParticles = [];

    public override IEnumerable<IGoozParticle> Particles =>
        deferredParticles.Concat(immediateParticles);

    protected override void Add(IGoozParticle particle)
    {
        if (particle.RequiresImmediateMode) {
            immediateParticles.Add(particle);
        }
        else {
            deferredParticles.Add(particle);
        }
    }

    protected override void Clear()
    {
        deferredParticles.Clear();
        immediateParticles.Clear();
    }

    public override void Update()
    {
        for (var i = 0; i < deferredParticles.Count; i++) {
            var particle = deferredParticles[i];

            if (particle.ShouldBeRemovedFromRenderer) {
                particle.RestInPool();
                deferredParticles.RemoveAt(i);
                i--;
            }
            else {
                particle.Update(ref Settings);
            }
        }

        for (var i = 0; i < immediateParticles.Count; i++) {
            var particle = immediateParticles[i];

            if (particle.ShouldBeRemovedFromRenderer) {
                particle.RestInPool();
                immediateParticles.RemoveAt(i);
                i--;
            }
            else {
                particle.Update(ref Settings);
            }
        }
    }

    public override void Draw(SpriteBatch sb)
    {
        if (ShouldRestart) {
            sb.End();
        }

        if (Effect is not null) {
            DrawParticles(Particles, immediate: true);
        }
        else {
            if (deferredParticles.Count > 0) {
                DrawParticles(deferredParticles, immediate: true);
            }

            if (immediateParticles.Count > 0) {
                DrawParticles(immediateParticles, immediate: true);
            }
        }

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

        return;

        void DrawParticles(IEnumerable<IGoozParticle> theParticles, bool immediate)
        {
            sb.Begin(
                immediate ? SpriteSortMode.Immediate : SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                Main.DefaultSamplerState,
                DepthStencilState.None,
                Main.Rasterizer,
                Effect,
                Main.Transform
            );

            foreach (var particle in theParticles) {
                particle.Draw(ref Settings, sb);
            }

            sb.End();
        }
    }
}
