using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Arch.Core.Extensions;
using CalamityHunt.Common.Utilities;
using ReLogic.Content;
using Terraria;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles;

public struct ParticleMicroShockwave
{
    public float CurScale { get; set; }

    public Color SecondColor { get; set; }
}
    
public class MicroShockwaveParticleBehavior : ParticleBehavior
{
    public override void OnSpawn(in Entity entity)
    {
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        rotation.Value = velocity.Value.ToRotation();
        velocity.Value = Vector2.Zero;

        entity.Add(new ParticleMicroShockwave());
    }

    public override void Update(in Entity entity)
    {
        ref var shockwave = ref entity.Get<ParticleMicroShockwave>();
        ref var scale = ref entity.Get<ParticleScale>();
        ref var active = ref entity.Get<ParticleActive>();

        shockwave.CurScale += (scale.Value - shockwave.CurScale * 0.8f) * 0.09f;
        if (shockwave.CurScale > scale.Value)
            active.Value = false;

        if (entity.TryGet<ParticleData<Color>>(out var data))
            shockwave.SecondColor = data.Value;
        else
            shockwave.SecondColor = Color.White;
    }

    private static Asset<Texture2D> texture;

    public override void Load()
    {
        texture = AssetUtilities.RequestImmediate<Texture2D>(Texture);
    }

    public override void Draw(in Entity entity, SpriteBatch spriteBatch)
    {
        ref var shockwave = ref entity.Get<ParticleMicroShockwave>();
        ref var scale = ref entity.Get<ParticleScale>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var color = ref entity.Get<ParticleColor>();
        
        Rectangle solidFrame = texture.Value.Frame(1, 3, 0, 0);
        Rectangle colorFrame = texture.Value.Frame(1, 3, 0, 1);
        Rectangle glowFrame = texture.Value.Frame(1, 3, 0, 2);
        float power = Utils.GetLerpValue(scale.Value, scale.Value * 0.7f, shockwave.CurScale, true);
        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, solidFrame, Color.Black * 0.1f * power, rotation.Value, solidFrame.Size() * 0.5f, new Vector2(shockwave.CurScale, shockwave.CurScale * 0.5f), 0, 0);
        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, colorFrame, color.Value * power, rotation.Value, colorFrame.Size() * 0.5f, new Vector2(shockwave.CurScale, shockwave.CurScale * 0.5f), 0, 0);
        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, glowFrame, shockwave.SecondColor * power, rotation.Value, glowFrame.Size() * 0.5f, new Vector2(shockwave.CurScale, shockwave.CurScale * 0.5f), 0, 0);
    }
}
