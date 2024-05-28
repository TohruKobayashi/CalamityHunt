using Arch.Core;
using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;

namespace CalamityHunt.Content.Particles.FlyingSlimes;

public class FlyingFirstEncounterParticleBehavior : FlyingSlimeParticleBehavior
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
        ref var rotation = ref entity.Get<ParticleRotation>();
        rotation.Value += 0.1f;
    }
}
