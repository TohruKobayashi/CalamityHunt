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

public struct ParticleCosmicSmoke
{
    public int Time { get; set; }

    public int MaxTime { get; set; }

    public int Variant { get; set; }

    public float RotationalVelocity { get; set; }
}
    
public class CosmicSmokeParticleBehavior : ParticleBehavior
{
    public override void OnSpawn(in Entity entity)
    {
        ref var scale = ref entity.Get<ParticleScale>();
        ref var rotation = ref entity.Get<ParticleRotation>();

        scale.Value *= 0.8f + Main.rand.NextFloat(0.9f, 1.1f);
        rotation.Value += Main.rand.NextFloat(-3f, 3f);

        var smoke = new ParticleCosmicSmoke
        {
            Variant = Main.rand.Next(8),
            RotationalVelocity = Main.rand.NextFloat(-0.15f, 0.15f),
            MaxTime = (int)(15 * (scale.Value * 0.2f + 0.8f))
        };
        entity.Add(smoke);
    }

    public override void Update(in Entity entity)
    {
        ref var scale = ref entity.Get<ParticleScale>();
        ref var smoke = ref entity.Get<ParticleCosmicSmoke>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var active = ref entity.Get<ParticleActive>();
        
        scale.Value *= 0.99f;
        smoke.Time++;

        smoke.RotationalVelocity *= 0.96f * Math.Clamp(1 - smoke.Time * 0.001f, 0.7f, 1f);
        rotation.Value += smoke.RotationalVelocity;

        if (smoke.Time > smoke.MaxTime * 0.5f)
        {
            scale.Value *= 0.93f - Math.Clamp(scale.Value * 0.0001f, 0f, 0.5f);
            velocity.Value *= 0.83f;
        }
        else
            velocity.Value *= 1.01f;

        if (smoke.Time > smoke.MaxTime * 0.8f)
            active.Value = false;
    }
    public override void Draw(in Entity entity, SpriteBatch spriteBatch)
    {
        if (entity.TryGet<ParticleData<string>>(out var data) && data.Value == "Cosmos")
        {
            _ = entity.AddOrGet(new CosmicSmokeParticleBehavior());
            return;
        }
        
        ref var smoke = ref entity.Get<ParticleCosmicSmoke>();
        ref var scale = ref entity.Get<ParticleScale>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var color = ref entity.Get<ParticleColor>();
        ref var position = ref entity.Get<ParticlePosition>();

        Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
        Rectangle frame = texture.Frame(4, 2, smoke.Variant % 4, (int)(smoke.Variant / 4f));
        float grow = (float)Math.Sqrt(Utils.GetLerpValue(0, smoke.MaxTime * 0.2f, smoke.Time, true));
        float opacity = Utils.GetLerpValue(smoke.MaxTime * 0.8f, 0, smoke.Time, true) * Math.Clamp(scale.Value, 0, 1);
        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, frame, color.Value * opacity, rotation.Value, frame.Size() * 0.5f, scale.Value * grow * 0.5f, 0, 0);
    }
}
