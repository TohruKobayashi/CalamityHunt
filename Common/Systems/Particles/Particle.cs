using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems.Particles;

public abstract class Particle<T> : ILoadable, IPooledParticle where T : Particle<T>
{
    private static ParticlePool<T> pool;

    protected virtual int MaxParticles => 200;

    public virtual bool ShouldBeRemovedFromRenderer { get; protected set; }

    public virtual bool IsRestingInPool { get; protected set; }

    public Vector2 Position { get; set; }

    public Vector2 Velocity { get; set; }

    public Vector2 Acceleration { get; set; }

    public float Rotation { get; set; }

    public float RotationVelocity { get; set; }

    public Vector2 Scale { get; set; } = Vector2.One;

    public Vector2 ScaleVelocity { get; set; }

    public Color Color { get; set; } = Color.White;

    void ILoadable.Load(Mod mod)
    {
        pool = new ParticlePool<T>(MaxParticles, NewInstance);
    }

    void ILoadable.Unload() { }

    void IParticle.Update(ref ParticleRendererSettings settings)
    {
        Update();
    }

    void IParticle.Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
    {
        Draw(spritebatch);
    }

    protected virtual void Update()
    {
        Velocity         += Acceleration;
        Position         += Velocity;
        RotationVelocity += RotationVelocity;
        Scale            += ScaleVelocity;
    }

    protected virtual void Draw(SpriteBatch sb) { }

    public virtual void RestInPool()
    {
        IsRestingInPool = true;
    }

    public virtual void FetchFromPool()
    {
        IsRestingInPool             = false;
        ShouldBeRemovedFromRenderer = false;

        Position         = Vector2.Zero;
        Velocity         = Vector2.Zero;
        Acceleration     = Vector2.Zero;
        Rotation         = 0f;
        RotationVelocity = 0f;
        Scale            = Vector2.One;
        Color            = Color.White;
    }

    protected abstract T NewInstance();

    public static T RequestParticle()
    {
        return pool.RequestParticle();
    }
}
