using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles.FlyingSlimes;

public class FlyingShimmerSlimeParticleBehavior : FlyingSlimeParticleBehavior
{
    public override bool ShouldDraw => false;

    public override void PostUpdate(in Entity entity)
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

    public override void DrawSlime(in Entity entity, SpriteBatch spriteBatch)
    {
        ref var flyingSlime = ref entity.Get<ParticleFlyingSlime>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var color = ref entity.Get<ParticleColor>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var scale = ref entity.Get<ParticleScale>();
            
        Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);

        float fadeIn = Utils.GetLerpValue(0, 30, flyingSlime.Time, true) * flyingSlime.DistanceFade;
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);

        for (int i = 0; i < 10; i++)
        {
            DrawData trailData = new DrawData(texture.Value, position.Value - velocity.Value * i * 0.5f - Main.screenPosition, texture.Frame(), color.Value.MultiplyRGBA(Lighting.GetColor(position.Value.ToTileCoordinates())) * fadeIn * ((10f - i) / 50f), rotation.Value + MathHelper.PiOver2, texture.Size() * 0.5f, scale.Value * flyingSlime.DistanceFade * 1.05f, 0, 0);
            GameShaders.Misc["RainbowTownSlime"].Apply(trailData);
            trailData.Draw(spriteBatch);
        }

        DrawData data = new DrawData(texture.Value, position.Value - Main.screenPosition, texture.Frame(), color.Value.MultiplyRGBA(Lighting.GetColor(position.Value.ToTileCoordinates())) * fadeIn, rotation.Value + MathHelper.PiOver2, texture.Size() * 0.5f, scale.Value * flyingSlime.DistanceFade, 0, 0);
        GameShaders.Misc["RainbowTownSlime"].Apply(data);
        data.Draw(spriteBatch);
        Main.pixelShader.CurrentTechnique.Passes[0].Apply();

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
    }
}
