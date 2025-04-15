using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;

using Terraria;

namespace CalamityHunt.Content.Particles;

public sealed class DarkSludgeChunk : Particle<DarkSludgeChunk>
{
    public int  variant;
    public int  time;
    public bool stuck;
    
    public override void FetchFromPool()
    {
        base.FetchFromPool();
    }

    public override void OnSpawn()
    {
        Scale   *= Main.rand.NextFloat(0.7f, 0.9f);
        variant =  Main.rand.Next(2);
    }

    protected override void Update()
    {
        time++;

        if (!stuck)
        {
            if (Velocity.Y < 30)
                Velocity.Y += 0.4f;

            Rotation = Velocity.ToRotation() - MathHelper.PiOver2;

            if (Collision.IsWorldPointSolid(Position + Velocity) && time > 2)
            {
                time       = 0;
                stuck      = true;
                Position.Y = (int)(Position.Y / 16f) * 16 + 16;
                for (int i = 0; i < 8; i++)
                {
                    if (Collision.IsWorldPointSolid(Position + Velocity - new Vector2(0, 8 * i)))
                        Position.Y -= 8;                        
                }
                Position.Y -= 3;
            }
        }
        else
        {
            Rotation = 0;
            Velocity = Vector2.Zero;
            if (time > 10)
                Scale *= 0.95f;

            if (Scale.X < 0.1f)
                ShouldBeRemovedFromRenderer = true; 
        }

    }
        
    protected override DarkSludgeChunk NewInstance()
    {
        return new DarkSludgeChunk();
    }
}
