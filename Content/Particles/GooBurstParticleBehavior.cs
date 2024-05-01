using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Arch.Core.Extensions;
using CalamityHunt.Core;
using Terraria;
using Terraria.ModLoader;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles;

public struct ParticleGooBurst
{
    public int Variant { get; set; }

    public float ColOffset { get; set; }

    public int FrameCounter { get; set; }

    public int Frame { get; set; }
}
    
public class GooBurstParticleBehavior : ParticleBehavior
{
    public override void OnSpawn(in Entity entity)
    {
        ref var scale = ref entity.Get<ParticleScale>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var rotation = ref entity.Get<ParticleRotation>();
            
        scale.Value *= Main.rand.NextFloat(0.9f, 1.1f);
        var burst = new ParticleGooBurst
        {
            Variant = Main.rand.Next(0, 2),
        };
        rotation.Value = velocity.Value.ToRotation() + MathHelper.PiOver2;
        velocity.Value = Vector2.Zero;
            
        entity.Add(burst);
    }

    public override void Update(in Entity entity)
    {
        ref var burst = ref entity.Get<ParticleGooBurst>();
        ref var active = ref entity.Get<ParticleActive>();
        ref var color = ref entity.Get<ParticleColor>();
        
        burst.FrameCounter++;
        if (burst.Frame < 3)
            burst.FrameCounter++;
        if (burst.FrameCounter % 4 == 0)
            burst.Frame++;
        if (burst.Frame > 7)
            active.Value = false;
        
        if (entity.TryGet<ParticleData<float>>(out var data))
            color.Value = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(data.Value + Math.Max(0, burst.FrameCounter - 18) * 3f - 2f);
    }

    public override void Draw(in Entity entity, SpriteBatch spriteBatch)
    { 
        ref var burst = ref entity.Get<ParticleGooBurst>();
        ref var color = ref entity.Get<ParticleColor>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var scale = ref entity.Get<ParticleScale>();
        
        Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
        Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Glow");
        Rectangle drawFrame = texture.Frame(8, 2, burst.Frame, burst.Variant);

        Color glowColor = color.Value;
        glowColor.A = 0;

        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, drawFrame, Color.Gray, rotation.Value, drawFrame.Size() * new Vector2(0.5f, 1f), scale.Value, 0, 0);
        spriteBatch.Draw(glow.Value, position.Value - Main.screenPosition, drawFrame, color.Value, rotation.Value, drawFrame.Size() * new Vector2(0.5f, 1f), scale.Value, 0, 0);
        spriteBatch.Draw(glow.Value, position.Value - Main.screenPosition, drawFrame, glowColor * 0.5f, rotation.Value, drawFrame.Size() * new Vector2(0.5f, 1f), scale.Value * 1.01f, 0, 0);
    }
}
