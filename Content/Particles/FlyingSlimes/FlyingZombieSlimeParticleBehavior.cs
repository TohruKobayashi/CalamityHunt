using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityHunt.Content.Particles.FlyingSlimes;

public class FlyingZombieSlimeParticleBehavior : FlyingSlimeParticleBehavior
{
    public override float SlimeSpeed => 30f;

    public override bool ShouldRotate => false;

    public override void KillEffect(in Arch.Core.Entity entity)
    {
        ref var position = ref entity.Get<ParticlePosition>();

        Gore.NewGore(Entity.GetSource_None(), position.Value, Main.rand.NextVector2Circular(1, 1), 4);
        Gore.NewGore(Entity.GetSource_None(), position.Value, Main.rand.NextVector2Circular(1, 1), 5);
    }

    public override void PostUpdate(in Arch.Core.Entity entity)
    {
        ref var position = ref entity.Get<ParticlePosition>();
        ref var velocity = ref entity.Get<ParticleVelocity>();

        Lighting.AddLight(position.Value + velocity.Value, Color.Pink.ToVector3());
    }
}