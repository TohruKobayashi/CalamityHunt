using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles.FlyingSlimes;

public class FlyingSlimeBunnyParticleBehavior : FlyingSlimeParticleBehavior
{
    public override bool ShouldDraw => false;

    public override void KillEffect(in Arch.Core.Entity entity)
    {
        ref var position = ref entity.Get<ParticlePosition>();
        
        Gore.NewGore(Entity.GetSource_None(), position.Value, Main.rand.NextVector2Circular(1, 1), 440);
    }

    public override void PostUpdate(in Arch.Core.Entity entity)
    {
        ref var position = ref entity.Get<ParticlePosition>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var color = ref entity.Get<ParticleColor>();
        
        if (Main.rand.NextBool(10))
        {
            Dust slime = Dust.NewDustPerfect(position.Value + Main.rand.NextVector2Circular(20, 20), 306, velocity.Value * 0.2f, 128, color.Value, 0.5f + Main.rand.NextFloat() * 0.3f);
            slime.fadeIn = 0.9f;
            slime.color = Main.hslToRgb(((float)Main.timeForVisualEffects / 300f + Main.rand.NextFloat() * 0.1f) % 1f, 1f, 0.65f, 0);
            slime.noLightEmittence = true;
            slime.noGravity = true;
        }

        Lighting.AddLight(position.Value + velocity.Value, Main.hslToRgb(((float)Main.timeForVisualEffects / 300f + Main.rand.NextFloat() * 0.1f) % 1f, 1f, 0.65f, 0).ToVector3() * 0.1f);
    }

    public override void DrawSlime(in Arch.Core.Entity entity, SpriteBatch spriteBatch)
    {
        ref var flyingSlime = ref entity.Get<ParticleFlyingSlime>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var color = ref entity.Get<ParticleColor>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var scale = ref entity.Get<ParticleScale>();
        
        Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
        float fadeIn = Utils.GetLerpValue(0, 30, flyingSlime.Time, true) * flyingSlime.DistanceFade;

        for (int i = 0; i < 10; i++)
            spriteBatch.Draw(texture.Value, position.Value - velocity.Value * i * 0.6f - Main.screenPosition, null, color.Value.MultiplyRGBA(Lighting.GetColor(position.Value.ToTileCoordinates())) * 0.78f * fadeIn * ((10f - i) / 100f), rotation.Value + MathHelper.PiOver2, texture.Size() * 0.5f, scale.Value * 1.1f * flyingSlime.DistanceFade * 1.05f, 0, 0);

        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, null, color.Value.MultiplyRGBA(Lighting.GetColor(position.Value.ToTileCoordinates())) * 0.78f * fadeIn, rotation.Value + MathHelper.PiOver2, texture.Size() * 0.5f, scale.Value * 1.1f * flyingSlime.DistanceFade, 0, 0);
    }
}
