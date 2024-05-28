using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles.FlyingSlimes;

public class FlyingYuHParticleBehavior : FlyingSlimeParticleBehavior
{
    public override float SlimeSpeed => 35f;

    public override void PostUpdate(in Entity entity)
    {
        ref var position = ref entity.Get<ParticlePosition>();
        ref var velocity = ref entity.Get<ParticleVelocity>();

        Lighting.AddLight(position.Value + velocity.Value, Color.DarkCyan.ToVector3() * 0.2f);
    }
}
