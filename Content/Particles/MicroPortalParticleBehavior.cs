using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Arch.Core.Extensions;
using CalamityHunt.Common.Utilities;
using ReLogic.Content;
using Terraria;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles;

public struct ParticleMicroPortal
{
    public Color SecondColor { get; set; }

    public int Direction { get; set; }

    public float Time { get; set; }
}
    
public class MicroPortalParticleBehavior : ParticleBehavior
{
    public override void OnSpawn(in Entity entity)
    {
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        rotation.Value = velocity.Value.ToRotation();
        velocity.Value = Vector2.Zero;

        var portal = new ParticleMicroPortal
        {
            Direction = Main.rand.NextBool().ToDirectionInt(),
        };
        entity.Add(portal);
    }

    public override void Update(in Entity entity)
    {
        ref var portal = ref entity.Get<ParticleMicroPortal>();
        ref var scale = ref entity.Get<ParticleScale>();
        ref var active = ref entity.Get<ParticleActive>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        
        portal.Time += 0.05f / scale.Value;
        if (portal.Time > 1f)
            active.Value = false;
        
        if (entity.TryGet<ParticleData<Color>>(out var data))
            portal.SecondColor = data.Value;
        else
            portal.SecondColor = Color.White;

        rotation.Value += (1f - portal.Time * 0.5f) * 0.2f * portal.Direction;
    }

    private static Asset<Texture2D> texture;

    public override void Load()
    {
        texture = AssetUtilities.RequestImmediate<Texture2D>(Texture);
    }

    public override void Draw(in Entity entity, SpriteBatch spriteBatch)
    {
        ref var portal = ref entity.Get<ParticleMicroPortal>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var color = ref entity.Get<ParticleColor>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var scale = ref entity.Get<ParticleScale>();
        
        Rectangle solidFrame = texture.Value.Frame(1, 3, 0, 0);
        Rectangle colorFrame = texture.Value.Frame(1, 3, 0, 1);
        Rectangle glowFrame = texture.Value.Frame(1, 3, 0, 2);
        float curScale = MathF.Sqrt(Utils.GetLerpValue(0, 0.1f, portal.Time, true) * Utils.GetLerpValue(1f, 0.5f, portal.Time, true));
        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, solidFrame, Color.Black * 0.5f, -rotation.Value * 2f, solidFrame.Size() * 0.5f, scale.Value * 0.9f * curScale * (1f + MathF.Sin(portal.Time * 5f) * 0.15f), 0, 0);
        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, colorFrame, color.Value * 0.5f, -rotation.Value * 0.7f, colorFrame.Size() * 0.5f, scale.Value * 1.1f * curScale * (1f + MathF.Sin(portal.Time * 5f) * 0.1f), 0, 0);
        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, solidFrame, Color.Black * 0.5f, rotation.Value * 1.3f, solidFrame.Size() * 0.5f, scale.Value * 0.6f * curScale, 0, 0);

        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, colorFrame, color.Value, rotation.Value, colorFrame.Size() * 0.5f, scale.Value * curScale * (1f + MathF.Sin(portal.Time * 10f) * 0.05f), 0, 0);
        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, glowFrame, portal.SecondColor, rotation.Value, glowFrame.Size() * 0.5f, scale.Value * 1.05f * curScale * (1f + MathF.Sin(portal.Time * 10f) * 0.05f), 0, 0);
    }
}
