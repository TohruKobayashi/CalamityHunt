using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using Terraria;
using Microsoft.Xna.Framework;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles.FlyingSlimes;

public class FlyingNovaSlimerParticleBehavior : FlyingSlimeParticleBehavior
{
    public override bool ShouldRotate => false;

    public override void OnSpawn(in Entity entity)
    {
        base.OnSpawn(in entity);

        ref var scale = ref entity.Get<ParticleScale>();
        scale.Value = 1;
    }

    public override void PostUpdate(in Entity entity)
    {
        ref var position = ref entity.Get<ParticlePosition>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var rotation = ref entity.Get<ParticleRotation>();

        Lighting.AddLight(position.Value + velocity.Value, Color.Purple.ToVector3());
        rotation.Value += 0.1f;
    }
}