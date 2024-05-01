using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles.FlyingSlimes;

public class FlyingGoldSlimeParticleBehavior : FlyingSlimeParticleBehavior
{
    public override float SlimeSpeed => 30f;

    public override void PostUpdate(in Entity entity)
    {
        ref var position = ref entity.Get<ParticlePosition>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var color = ref entity.Get<ParticleColor>();
        
        if (Main.rand.NextBool(2))
        {
            Dust slime = Dust.NewDustPerfect(position.Value + Main.rand.NextVector2Circular(20, 20), 246, velocity.Value * 0.2f, 200, color.Value, 1.5f);
            slime.noLightEmittence = true;
            slime.noGravity = true;
        }

        Lighting.AddLight(position.Value + velocity.Value, new Color(204, 181, 72, 255).ToVector3());
    }
}
