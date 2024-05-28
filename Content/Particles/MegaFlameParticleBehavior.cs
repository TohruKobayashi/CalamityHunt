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

public struct ParticleMegaFlame
{
    public int Time { get; set; }

    public int MaxTime { get; set; }

    public int Variant { get; set; }
        
    public float RotationalVelocity { get; set; }
}
    
public class MegaFlameParticleBehavior : ParticleBehavior
{
    public override void OnSpawn(in Entity entity)
    {
        ref var scale = ref entity.Get<ParticleScale>();
        var flame = new ParticleMegaFlame
        {
            Variant = Main.rand.Next(8),
        };
        scale.Value *= 0.8f + Main.rand.NextFloat(0.9f, 1.1f);
        flame.MaxTime = (int)(25 * (scale.Value * 0.2f + 0.8f));
        entity.Add(flame);
    }

    public override void Update(in Entity entity)
    {
        ref var flame = ref entity.Get<ParticleMegaFlame>();
        ref var scale = ref entity.Get<ParticleScale>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var active = ref entity.Get<ParticleActive>();
        
        scale.Value *= 0.99f;
        flame.Time++;

        rotation.Value = velocity.Value.ToRotation();

        if (flame.Time > flame.MaxTime * 0.3f)
        {
            scale.Value *= 0.93f - Math.Clamp(scale.Value * 0.0001f, 0f, 0.5f);
            velocity.Value *= 0.95f;
            velocity.Value -= new Vector2(0f, 0.1f);
        }
        else
            velocity.Value *= 1.01f;

        if (flame.Time > flame.MaxTime * 0.8f)
            active.Value = false;
    }
    public override void Draw(in Entity entity, SpriteBatch spriteBatch)
    {
        ref var flame = ref entity.Get<ParticleMegaFlame>();
        ref var scale = ref entity.Get<ParticleScale>();
        ref var color = ref entity.Get<ParticleColor>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var rotation = ref entity.Get<ParticleRotation>();

        if (entity.TryGet<ParticleData<string>>(out var data) && data.Value == "Sludge")
            return;

        Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
        Rectangle frame = texture.Frame(4, 2, flame.Variant % 4, (int)(flame.Variant / 4f));
        float grow = (float)Math.Sqrt(Utils.GetLerpValue(0, flame.MaxTime * 0.3f, flame.Time, true));
        float opacity = Utils.GetLerpValue(flame.MaxTime * 0.8f, 0, flame.Time, true) * Math.Clamp(scale.Value, 0, 1);
        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, frame, color.Value * opacity, rotation.Value - MathHelper.PiOver2, frame.Size() * 0.5f, scale.Value * grow * 0.5f, 0, 0);
    }
}
