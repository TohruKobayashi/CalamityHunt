using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles.FlyingSlimes;

public struct ParticleAureusSpawnFlyingSlime
{
    public int CurrentFrame { get; set; }

    public float FrameCounter { get; set; }
}

public class FlyingAureusSpawnSlimeParticleBehavior : FlyingSlimeParticleBehavior
{
    public override float SlimeSpeed => 16;

    public override bool ShouldDraw => false;

    public override void OnSpawn(in Entity entity)
    {
        base.OnSpawn(in entity);
        
        entity.Add(new ParticleAureusSpawnFlyingSlime());
    }

    public override void PostUpdate(in Entity entity)
    {
        ref var flyingSlime = ref entity.Get<ParticleFlyingSlime>();
        ref var aureusSlime = ref entity.Get<ParticleAureusSpawnFlyingSlime>();
        ref var position = ref entity.Get<ParticlePosition>();
        
        Lighting.AddLight(position.Value, 0.6f * flyingSlime.DistanceFade, 0.25f * flyingSlime.DistanceFade, 0f);

        aureusSlime.FrameCounter += 0.22f;
        aureusSlime.FrameCounter %= 4;
        int frame = (int)aureusSlime.FrameCounter;
        aureusSlime.CurrentFrame = frame * 62;
    }

    public override void DrawSlime(in Entity entity, SpriteBatch spriteBatch)
    { 
        ref var flyingSlime = ref entity.Get<ParticleFlyingSlime>();
        ref var aureusSlime = ref entity.Get<ParticleAureusSpawnFlyingSlime>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var color = ref entity.Get<ParticleColor>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var scale = ref entity.Get<ParticleScale>();
        
        // 92 by 62
        Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
        Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Glow");
        Rectangle frame = new(0, aureusSlime.CurrentFrame, 92, 62);
        float fadeIn = Utils.GetLerpValue(0, 30, flyingSlime.Time, true) * flyingSlime.DistanceFade;

        for (int i = 0; i < 10; i++)
            spriteBatch.Draw(texture.Value, position.Value - velocity.Value * i * 0.5f - Main.screenPosition, frame, color.Value.MultiplyRGBA(Lighting.GetColor(position.Value.ToTileCoordinates())) * fadeIn * ((10f - i) / 80f), rotation.Value + MathHelper.PiOver2, frame.Size() * 0.5f, scale.Value * flyingSlime.DistanceFade * 1.05f, 0, 0);

        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, frame, color.Value.MultiplyRGBA(Lighting.GetColor(position.Value.ToTileCoordinates())) * fadeIn, rotation.Value + MathHelper.PiOver2, frame.Size() * 0.5f, scale.Value * flyingSlime.DistanceFade, 0, 0);
        spriteBatch.Draw(glow.Value, position.Value - Main.screenPosition, frame, Color.White * fadeIn, rotation.Value + MathHelper.PiOver2, frame.Size() * 0.5f, scale.Value * flyingSlime.DistanceFade, 0, 0);
    }
}
