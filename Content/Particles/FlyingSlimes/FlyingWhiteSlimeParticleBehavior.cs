using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles.FlyingSlimes;

public class FlyingWhiteSlimeParticleBehavior : FlyingSlimeParticleBehavior // White Slime from the 'Aequus' mod
{
    public override float SlimeSpeed => 30f;
    public override bool ShouldDraw => false;

    public override void PostUpdate(in Entity entity)
    {
        ref var position = ref entity.Get<ParticlePosition>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
            
        Lighting.AddLight(position.Value + velocity.Value, Color.White.ToVector3());
    }

    public override void DrawSlime(in Entity entity, SpriteBatch spriteBatch)
    {
        ref var flyingSlime = ref entity.Get<ParticleFlyingSlime>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var scale = ref entity.Get<ParticleScale>();
        ref var color = ref entity.Get<ParticleColor>();
            
        Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
        float fadeIn = Utils.GetLerpValue(0, 30, flyingSlime.DistanceFade, true) * flyingSlime.DistanceFade;
        Color drawColor = color.Value;
        drawColor.A /= 2;
        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, null, drawColor * fadeIn, rotation.Value + MathHelper.PiOver2, texture.Size() * 0.5f, scale.Value * flyingSlime.DistanceFade, 0, 0);
        for (int i = 0; i < 10; i++)
            spriteBatch.Draw(texture.Value, position.Value - velocity.Value * i * 0.5f - Main.screenPosition, null, drawColor * fadeIn * ((10f - i) / 30f), rotation.Value + MathHelper.PiOver2, texture.Size() * 0.5f, scale.Value * flyingSlime.DistanceFade, 0, 0);
    }
}
