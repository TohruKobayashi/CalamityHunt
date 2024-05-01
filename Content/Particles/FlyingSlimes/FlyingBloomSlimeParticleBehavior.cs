using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using Terraria;
using Terraria.ID;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles.FlyingSlimes;

public class FlyingBloomSlimeParticleBehavior : FlyingSlimeParticleBehavior
{
    public override void PostUpdate(in Entity entity)
    {
        ref var position = ref entity.Get<ParticlePosition>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var color = ref entity.Get<ParticleColor>();

        if (!Main.rand.NextBool(3))
            return;

        Dust slime = Dust.NewDustPerfect(position.Value + Main.rand.NextVector2Circular(20, 20), DustID.CursedTorch, velocity.Value * 0.5f, 200, color.Value, 0.5f + Main.rand.NextFloat());
        slime.noGravity = true;
    }
}
