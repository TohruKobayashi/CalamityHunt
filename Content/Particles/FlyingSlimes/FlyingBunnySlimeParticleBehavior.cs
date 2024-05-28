using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using Terraria;

namespace CalamityHunt.Content.Particles.FlyingSlimes;

public class FlyingBunnySlimeParticleBehavior : FlyingSlimeParticleBehavior
{
    public override float SlimeSpeed => 25f;

    public override void KillEffect(in Arch.Core.Entity entity)
    {
        ref var position = ref entity.Get<ParticlePosition>();

        Gore.NewGore(Entity.GetSource_None(), position.Value, Main.rand.NextVector2Circular(1, 1), 76);
        Gore.NewGore(Entity.GetSource_None(), position.Value, Main.rand.NextVector2Circular(1, 1), 77);
    }
}
