using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles.FlyingSlimes;

public struct ParticleFlyingMetanovaSlime
{
    public bool Armored { get; set; }
}

public class FlyingMetanovaSlimeParticleBehavior : FlyingSlimeParticleBehavior
{
    public override bool ShouldDraw => false;

    public override void OnSpawn(in Entity entity)
    {
        base.OnSpawn(in entity);
        
        entity.Add(new ParticleFlyingMetanovaSlime { Armored = Main.rand.NextBool() });
    }

    public override void PostUpdate(in Entity entity)
    {
        ref var position = ref entity.Get<ParticlePosition>();
        ref var velocity = ref entity.Get<ParticleVelocity>();

        Lighting.AddLight(position.Value + velocity.Value, Color.Purple.ToVector3());
    }

    public override void DrawSlime(in Entity entity, SpriteBatch spriteBatch)
    {
        ref var flyingSlime = ref entity.Get<ParticleFlyingSlime>();
        ref var metanovaSlime = ref entity.Get<ParticleFlyingMetanovaSlime>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var scale = ref entity.Get<ParticleScale>();
        ref var color = ref entity.Get<ParticleColor>();
        
        Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
        if (metanovaSlime.Armored)
            texture = ModContent.Request<Texture2D>(Texture + "Armored");

        float fadeIn = Utils.GetLerpValue(0, 30, flyingSlime.Time, true) * flyingSlime.DistanceFade;

        for (int i = 0; i < 10; i++)
            spriteBatch.Draw(texture.Value, position.Value - velocity.Value * i * 0.5f - Main.screenPosition, null, color.Value.MultiplyRGBA(Lighting.GetColor(position.Value.ToTileCoordinates())) * fadeIn * ((10f - i) / 80f), rotation.Value + MathHelper.PiOver2, texture.Size() * 0.5f, scale.Value * flyingSlime.DistanceFade * 1.05f, 0, 0);

        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, null, color.Value.MultiplyRGBA(Lighting.GetColor(position.Value.ToTileCoordinates())) * fadeIn, rotation.Value + MathHelper.PiOver2, texture.Size() * 0.5f, scale.Value * flyingSlime.DistanceFade, 0, 0);
    }
}
