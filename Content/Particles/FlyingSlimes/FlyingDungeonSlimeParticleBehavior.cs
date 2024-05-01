using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles.FlyingSlimes;

public class FlyingDungeonSlimeParticleBehavior : FlyingSlimeParticleBehavior
{
    public override float SlimeSpeed => 7f;
    public override float SlimeAcceleration => 0.2f;
    public override bool ShouldDraw => false;

    public override void DrawSlime(in Entity entity, SpriteBatch spriteBatch)
    {
        ref var flyingSlime = ref entity.Get<ParticleFlyingSlime>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var color = ref entity.Get<ParticleColor>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var scale = ref entity.Get<ParticleScale>();
        
        Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
        Asset<Texture2D> keyTexture = ModContent.Request<Texture2D>(Texture + "Key");
        float fadeIn = Utils.GetLerpValue(0, 30, flyingSlime.Time, true) * flyingSlime.DistanceFade;

        for (int i = 0; i < 10; i++)
            spriteBatch.Draw(texture.Value, position.Value - velocity.Value * i * 0.6f - Main.screenPosition, null, color.Value.MultiplyRGBA(Lighting.GetColor(position.Value.ToTileCoordinates())) * 0.765f * fadeIn * ((10f - i) / 100f), rotation.Value + MathHelper.PiOver2, texture.Size() * 0.5f, scale.Value * 1.25f * flyingSlime.DistanceFade * 1.05f, 0, 0);

        spriteBatch.Draw(keyTexture.Value, position.Value + new Vector2(texture.Height() / 4f * scale.Value * flyingSlime.DistanceFade, 0).RotatedBy(rotation.Value) - Main.screenPosition, null, Lighting.GetColor(position.Value.ToTileCoordinates()) * fadeIn, rotation.Value + MathHelper.PiOver2, keyTexture.Size() * 0.5f, scale.Value * 1.25f * flyingSlime.DistanceFade, 0, 0);
        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, null, color.Value.MultiplyRGBA(Lighting.GetColor(position.Value.ToTileCoordinates())) * 0.765f * fadeIn, rotation.Value + MathHelper.PiOver2, texture.Size() * 0.5f, scale.Value * 1.25f * flyingSlime.DistanceFade, 0, 0);
    }
}
