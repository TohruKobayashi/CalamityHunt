using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Arch.Core.Extensions;
using Terraria;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles;

public struct ParticleCrossSparkle
{
    public int Time { get; set; }
}
    
public class CrossSparkleParticleBehavior : ParticleBehavior
{
    public override void OnSpawn(in Entity entity)
    {
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var rotation = ref entity.Get<ParticleRotation>();
            
        rotation.Value = velocity.Value.ToRotation() + Main.rand.NextFloat(-0.05f, 0.05f);
        velocity.Value = Vector2.Zero;
            
        entity.Add(new ParticleCrossSparkle());
    }

    public override void Update(in Entity entity)
    {
        ref var sparkle = ref entity.Get<ParticleCrossSparkle>();
        ref var active = ref entity.Get<ParticleActive>();

        sparkle.Time++;
        if (sparkle.Time > 15)
            active.Value = false;

        // if (emit)
        //     Main.NewText("default is on");
    }

    public override void Draw(in Entity entity, SpriteBatch spriteBatch)
    {
        ref var sparkle = ref entity.Get<ParticleCrossSparkle>();
        ref var scale = ref entity.Get<ParticleScale>();
        ref var color = ref entity.Get<ParticleColor>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var position = ref entity.Get<ParticlePosition>();
        
        Texture2D texture = AssetDirectory.Textures.Sparkle.Value;

        float power = scale.Value * MathF.Pow(Utils.GetLerpValue(15, 6, sparkle.Time, true), 2.5f) * Utils.GetLerpValue(0, 5, sparkle.Time, true);
        Color drawColor = color.Value;
        drawColor.A = 0;
        spriteBatch.Draw(texture, position.Value - Main.screenPosition, texture.Frame(), color.Value * 0.2f, rotation.Value - MathHelper.PiOver4, texture.Size() * 0.5f, power * new Vector2(0.3f, 0.5f), 0, 0);
        spriteBatch.Draw(texture, position.Value - Main.screenPosition, texture.Frame(), color.Value * 0.2f, rotation.Value + MathHelper.PiOver4, texture.Size() * 0.5f, power * new Vector2(0.3f, 0.5f), 0, 0);
        spriteBatch.Draw(texture, position.Value - Main.screenPosition, texture.Frame(), drawColor, rotation.Value - MathHelper.PiOver4, texture.Size() * 0.5f, power * new Vector2(0.6f, 1f), 0, 0);
        spriteBatch.Draw(texture, position.Value - Main.screenPosition, texture.Frame(), drawColor, rotation.Value + MathHelper.PiOver4, texture.Size() * 0.5f, power * new Vector2(0.6f, 1f), 0, 0);
        spriteBatch.Draw(texture, position.Value - Main.screenPosition, texture.Frame(), drawColor * 0.2f, rotation.Value - MathHelper.PiOver4, texture.Size() * 0.5f, power * new Vector2(0.4f, 2f), 0, 0);
        spriteBatch.Draw(texture, position.Value - Main.screenPosition, texture.Frame(), drawColor * 0.2f, rotation.Value + MathHelper.PiOver4, texture.Size() * 0.5f, power * new Vector2(0.4f, 2f), 0, 0);
        spriteBatch.Draw(texture, position.Value - Main.screenPosition, texture.Frame(), new Color(255, 255, 255, 0), rotation.Value - MathHelper.PiOver4, texture.Size() * 0.5f, power * 0.5f, 0, 0);
        spriteBatch.Draw(texture, position.Value - Main.screenPosition, texture.Frame(), new Color(255, 255, 255, 0), rotation.Value + MathHelper.PiOver4, texture.Size() * 0.5f, power * 0.5f, 0, 0);
    }
}
