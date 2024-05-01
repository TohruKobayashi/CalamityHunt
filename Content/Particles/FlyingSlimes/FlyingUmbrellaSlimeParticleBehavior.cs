using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using Terraria;

namespace CalamityHunt.Content.Particles.FlyingSlimes;

public class FlyingUmbrellaSlimeParticleBehavior : FlyingSlimeParticleBehavior
{
    public override float SlimeSpeed => 7f;

    public override float SlimeAcceleration => 0.2f;

    public override void KillEffect(in Arch.Core.Entity entity)
    {
        ref var position = ref entity.Get<ParticlePosition>();

        Gore.NewGore(Entity.GetSource_None(), position.Value, Main.rand.NextVector2Circular(1, 1), 314);
    }
}
