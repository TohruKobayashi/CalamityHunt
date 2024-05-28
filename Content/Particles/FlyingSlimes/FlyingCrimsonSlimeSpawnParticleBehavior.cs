using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles.FlyingSlimes;

public struct ParticleFlyingCrimsonSlime
{
    public bool Spiked { get; set; }
}

public class FlyingCrimsonSlimeSpawnParticleBehavior : FlyingSlimeParticleBehavior
{
    public override bool ShouldDraw => false;

    public override void OnSpawn(in Entity entity)
    {
        base.OnSpawn(in entity);
        
        entity.Add(new ParticleFlyingCrimsonSlime { Spiked = Main.rand.NextBool() });
    }

    public override void PostUpdate(in Entity entity)
    {
        ref var position = ref entity.Get<ParticlePosition>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var color = ref entity.Get<ParticleColor>();
        
        if (!Main.rand.NextBool(10))
            return;

        Dust slime = Dust.NewDustPerfect(position.Value + Main.rand.NextVector2Circular(20, 20), 306, velocity.Value * 0.2f, DustID.Blood, color.Value, 0.5f + Main.rand.NextFloat() * 0.3f);
        slime.noGravity = true;
    }

    public override void DrawSlime(in Entity entity, SpriteBatch spriteBatch)
    {
        ref var flyingSlime = ref entity.Get<ParticleFlyingSlime>();
        ref var crimsonSlime = ref entity.Get<ParticleFlyingCrimsonSlime>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var color = ref entity.Get<ParticleColor>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var scale = ref entity.Get<ParticleScale>();
        
        Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
        if (crimsonSlime.Spiked)
            texture = ModContent.Request<Texture2D>(Texture + "Spiked");

        float fadeIn = Utils.GetLerpValue(0, 30, flyingSlime.Time, true) * flyingSlime.DistanceFade;

        for (int i = 0; i < 10; i++)
            spriteBatch.Draw(texture.Value, position.Value - velocity.Value * i * 0.5f - Main.screenPosition, null, color.Value.MultiplyRGBA(Lighting.GetColor(position.Value.ToTileCoordinates())) * fadeIn * ((10f - i) / 80f), rotation.Value + MathHelper.PiOver2, texture.Size() * 0.5f, scale.Value * flyingSlime.DistanceFade * 1.05f, 0, 0);

        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, null, color.Value.MultiplyRGBA(Lighting.GetColor(position.Value.ToTileCoordinates())) * fadeIn, rotation.Value + MathHelper.PiOver2, texture.Size() * 0.5f, scale.Value * flyingSlime.DistanceFade, 0, 0);
    }
}
