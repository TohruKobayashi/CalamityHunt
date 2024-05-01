#nullable enable

using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;

namespace CalamityHunt.Common.Systems.Particles;

public struct Particle
{
    public ParticleBehavior Behavior { get; set; }
}

public struct ParticlePosition
{
    public Vector2 Value { get; set; }
}

public struct ParticleVelocity
{
    public Vector2 Value { get; set; }
}

public struct ParticleRotation
{
    public float Value { get; set; }
}

public struct ParticleScale
{
    public float Value { get; set; }
}

public struct ParticleColor
{
    public Color Value { get; set; }
}

// Specifically for extra data like in regular Terraria entities.
public struct ParticleData<T>
{
    public T Value { get; set; }
}

public struct ParticleDrawBehindEntities
{ }

public struct ParticleShader
{
    public ArmorShaderData Value { get; set; }
}

public struct ParticleActive
{
    public bool Value { get; set; }
}
