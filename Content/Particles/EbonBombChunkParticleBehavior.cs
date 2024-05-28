using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Arch.Core.Extensions;
using Terraria;
using Terraria.ModLoader;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles;

public struct ParticleEbonBombChunk
{
    public int Variant { get; set; }

    public int Time { get; set; }

    public bool Stuck { get; set; }
}
    
public class EbonBombChunkParticleBehavior : ParticleBehavior
{
    public override void OnSpawn(in Entity entity)
    {
        ref var scale = ref entity.Get<ParticleScale>();
        scale.Value *= Main.rand.NextFloat(1f, 1.2f);
            
        var chunk = new ParticleEbonBombChunk
        {
            Variant = Main.rand.Next(2),
        };
        entity.Add(chunk);
    }

    public override void Update(in Entity entity)
    {
        ref var chunk = ref entity.Get<ParticleEbonBombChunk>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var scale = ref entity.Get<ParticleScale>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var active = ref entity.Get<ParticleActive>();
        
        chunk.Time++;

        if (!chunk.Stuck)
        {
            if (velocity.Value.Y < 30)
                velocity.Value = new Vector2(velocity.Value.X, velocity.Value.Y + 0.5f);

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

    public override void Draw(in Entity entity, SpriteBatch spriteBatch)
    { 
        ref var chunk = ref entity.Get<ParticleEbonBombChunk>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var color = ref entity.Get<ParticleColor>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var scale = ref entity.Get<ParticleScale>();
        
        Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
        Rectangle frame = texture.Frame(4, 1, chunk.Variant, 0);
        Vector2 squish = new Vector2(1f - velocity.Value.Length() * 0.01f, 1f + velocity.Value.Length() * 0.01f);
        float grow = (float)Math.Sqrt(Utils.GetLerpValue(-20, 40, chunk.Time, true));
        if (chunk.Stuck)
        {
            grow = 1f;
            frame = texture.Frame(4, 1, chunk.Variant + 2, 0);
            squish = new Vector2(1f + (float)Math.Sqrt(Utils.GetLerpValue(20, 0, chunk.Time, true)) * 0.33f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(20, 0, chunk.Time, true)) * 0.33f);
        }

        Color lightColor = Lighting.GetColor(position.Value.ToTileCoordinates());
        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, frame, Color.Lerp(color.Value, color.Value.MultiplyRGBA(lightColor), chunk.Stuck ? 1f : Utils.GetLerpValue(10, 50, chunk.Time, true)), rotation.Value, frame.Size() * new Vector2(0.5f, 0.84f), scale.Value * grow * squish, 0, 0);
    }
}
