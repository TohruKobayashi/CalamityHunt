using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using Terraria;
using Terraria.ID;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles.FlyingSlimes;

public class FlyingHoppinJackParticleBehavior : FlyingSlimeParticleBehavior
{
    public override float SlimeSpeed => 16f;

    public override float SlimeAcceleration => 0.2f;

    public override void PostUpdate(in Entity entity)
    {
        ref var position = ref entity.Get<ParticlePosition>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var color = ref entity.Get<ParticleColor>();
        
        if (!Main.rand.NextBool(8))
            return;

        Dust slime = Dust.NewDustPerfect(position.Value + Main.rand.NextVector2Circular(30, 30), DustID.Torch, velocity.Value * 0.2f, 200, color.Value, 0.5f + Main.rand.NextFloat());
        slime.noGravity = true;
    }
}
