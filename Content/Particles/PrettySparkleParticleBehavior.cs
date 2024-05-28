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

public struct ParticlePrettySparkle
{
    public int Time { get; set; }
}
    
public class PrettySparkleParticleBehavior : ParticleBehavior
{
    public override void OnSpawn(in Entity entity)
    {
        ref var scale = ref entity.Get<ParticleScale>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        scale.Value *= Main.rand.NextFloat(0.9f, 1.1f);
        velocity.Value *= Main.rand.NextFloat(0.9f, 1.1f);
        rotation.Value *= 0.05f;

        entity.Add(new ParticlePrettySparkle());
    }

    public override void Update(in Entity entity)
    {
        ref var sparkle = ref entity.Get<ParticlePrettySparkle>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var scale = ref entity.Get<ParticleScale>();
        ref var active = ref entity.Get<ParticleActive>();
        
        velocity.Value *= 0.95f;
        sparkle.Time++;

        if (sparkle.Time > 40 + scale.Value)
            scale.Value *= 0.8f + Math.Min(scale.Value * 0.2f, 0.18f);

        if (scale.Value < 0.1f)
            active.Value = false;
    }

    public override void Draw(in Entity entity, SpriteBatch spriteBatch)
    {
        ref var sparkle = ref entity.Get<ParticlePrettySparkle>();
        ref var scale = ref entity.Get<ParticleScale>();
        ref var color = ref entity.Get<ParticleColor>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        
        Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);

        float drawScale = scale.Value * (float)Math.Sqrt(Utils.GetLerpValue(-5, 10, sparkle.Time, true));
        Color drawColor = Color.Lerp(color.Value, Color.White, 0.6f);
        drawColor.A = 0;
        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, null, new Color(color.Value.R, color.Value.G, color.Value.B, color.Value.A / 2), rotation.Value, texture.Size() * 0.5f, drawScale * 0.6f, 0, 0);
        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, null, drawColor, rotation.Value, texture.Size() * 0.5f, drawScale * 0.3f, 0, 0);
    }
}
