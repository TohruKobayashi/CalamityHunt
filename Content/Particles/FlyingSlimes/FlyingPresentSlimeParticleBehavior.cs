using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles.FlyingSlimes;

public struct ParticleFlyingPresentSlime
{
    public int Variant { get; set; }
}
    
public class FlyingPresentSlimeParticleBehavior : FlyingSlimeParticleBehavior
{
    public override float SlimeSpeed => 25f;

    public override bool ShouldDraw => false;

    public override void OnSpawn(in Arch.Core.Entity entity)
    {
        base.OnSpawn(in entity);

        entity.Add(new ParticleFlyingPresentSlime { Variant = Main.rand.Next(4) });
    }

    public override void KillEffect(in Arch.Core.Entity entity)
    {
        ref var position = ref entity.Get<ParticlePosition>();
        
        Gore.NewGore(Entity.GetSource_None(), position.Value, Main.rand.NextVector2Circular(1, 1), 76);
        Gore.NewGore(Entity.GetSource_None(), position.Value, Main.rand.NextVector2Circular(1, 1), 77);
    }

    public override void DrawSlime(in Arch.Core.Entity entity, SpriteBatch spriteBatch)
    {
        ref var flyingSlime = ref entity.Get<ParticleFlyingSlime>();
        ref var presentSlime = ref entity.Get<ParticleFlyingPresentSlime>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var scale = ref entity.Get<ParticleScale>();
        ref var color = ref entity.Get<ParticleColor>();
        
        Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
        Rectangle frame = texture.Frame(4, 1, presentSlime.Variant, 0);
        float fadeIn = Utils.GetLerpValue(0, 30, flyingSlime.Time, true) * flyingSlime.DistanceFade;

        for (int i = 0; i < 10; i++)
            spriteBatch.Draw(texture.Value, position.Value - velocity.Value * i * 0.6f - Main.screenPosition, frame, color.Value.MultiplyRGBA(Lighting.GetColor(position.Value.ToTileCoordinates())) * 0.78f * fadeIn * ((10f - i) / 100f), rotation.Value + MathHelper.PiOver2, frame.Size() * 0.5f, scale.Value * 1.1f * flyingSlime.DistanceFade * 1.05f, 0, 0);

        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, frame, color.Value.MultiplyRGBA(Lighting.GetColor(position.Value.ToTileCoordinates())) * 0.78f * fadeIn, rotation.Value + MathHelper.PiOver2, frame.Size() * 0.5f, scale.Value * 1.1f * flyingSlime.DistanceFade, 0, 0);
    }
}
