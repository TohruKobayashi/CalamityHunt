using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Arch.Core.Extensions;
using Terraria;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles;

public struct ParticleDarkSludgeChunk
{
    public int Variant { get; set; }

    public int Time { get; set; }

    public bool Stuck { get; set; }
}
    
public class DarkSludgeChunkParticleBehavior : ParticleBehavior
{
    public override void OnSpawn(in Entity entity)
    {
        ref var scale = ref entity.Get<ParticleScale>();
        scale.Value *= Main.rand.NextFloat(0.7f, 0.9f);
            
        var chunk = new ParticleDarkSludgeChunk
        {
            Variant = Main.rand.Next(2),
        };
        entity.Add(chunk);
    }

    public override void Update(in Entity entity)
    {
        ref var chunk = ref entity.Get<ParticleDarkSludgeChunk>();
        ref var scale = ref entity.Get<ParticleScale>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var active = ref entity.Get<ParticleActive>();
        
        chunk.Time++;

        if (!chunk.Stuck)
        {
            if (velocity.Value.Y < 30)
                velocity.Value = new Vector2(velocity.Value.X, velocity.Value.Y + 0.4f);

            rotation.Value = velocity.Value.ToRotation() - MathHelper.PiOver2;

            if (Collision.IsWorldPointSolid(position.Value + velocity.Value) && chunk.Time > 2)
            {
                chunk.Time = 0;
                chunk.Stuck = true;
                position.Value = new Vector2(position.Value.X, (int)(position.Value.Y / 16f) * 16 + 16);
                for (int i = 0; i < 8; i++)
                {
                    if (Collision.IsWorldPointSolid(position.Value + velocity.Value - new Vector2(0, 8 * i)))
                        position.Value = new Vector2(position.Value.X, position.Value.Y - 8);
                }
                position.Value = new Vector2(position.Value.X, position.Value.Y - 3);
            }
        }
        else
        {
            rotation.Value = 0;
            velocity.Value = Vector2.Zero;
            if (chunk.Time > 10)
                scale.Value *= 0.95f;

            if (scale.Value < 0.1f)
                active.Value = false;
        }

    }
}
