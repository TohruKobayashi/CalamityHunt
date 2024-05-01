using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Arch.Core.Extensions;
using Terraria;
using Terraria.ModLoader;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles;

public struct ParticleHolyBombChunk
{
    public int Variant { get; set; }

    public int Time { get; set; }

    public bool Stuck { get; set; }
}
    
public class HolyBombChunkParticleBehavior : ParticleBehavior
{
    public override void OnSpawn(in Entity entity)
    {
        ref var scale = ref entity.Get<ParticleScale>();
        scale.Value *= Main.rand.NextFloat(1f, 1.2f);
            
        var chunk = new ParticleHolyBombChunk
        {
            Variant = Main.rand.Next(2),
        };
        entity.Add(chunk);
    }

    public override void Update(in Entity entity)
    {
        ref var chunk = ref entity.Get<ParticleHolyBombChunk>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var scale = ref entity.Get<ParticleScale>();
        ref var active = ref entity.Get<ParticleActive>();
        
        chunk.Time++;

        if (!chunk.Stuck)
        {
            if (velocity.Value.Y < 30)
                velocity.Value += new Vector2(0, 0.5f);

            rotation.Value = velocity.Value.ToRotation() - MathHelper.PiOver2;

            if (Collision.IsWorldPointSolid(position.Value + velocity.Value) && chunk.Time > 2)
            {
                chunk.Time = 0;
                chunk.Stuck = true;
                position.Value = new Vector2(position.Value.X, (int)(position.Value.Y / 16f) * 16 + 16);
                for (int i = 0; i < 8; i++)
                {
                    if (Collision.IsWorldPointSolid(position.Value + velocity.Value - new Vector2(0, 8 * i)))
                        position.Value -= new Vector2(0, 8);
                }
                position.Value -= new Vector2(0, 3);
            }
        }
        else
        {
            rotation.Value = 0;
            velocity.Value = Vector2.Zero;
            if (chunk.Time > 10)
                scale.Value *= 0.95f;

            if (scale.Value < 0.1f)
                active.Value = false;
        }

    }

    public override void Draw(in Entity entity, SpriteBatch spriteBatch)
    {
        ref var chunk = ref entity.Get<ParticleHolyBombChunk>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var color = ref entity.Get<ParticleColor>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var scale = ref entity.Get<ParticleScale>();
        
        Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
        Rectangle frame = texture.Frame(4, 2, chunk.Variant, 0);
        Rectangle shineFrame = texture.Frame(4, 2, chunk.Variant, 1);
        Vector2 squish = new Vector2(1f - velocity.Value.Length() * 0.01f, 1f + velocity.Value.Length() * 0.01f);
        float grow = (float)Math.Sqrt(Utils.GetLerpValue(-20, 40, chunk.Time, true));
        if (chunk.Stuck)
        {
            grow = 1f;
            frame = texture.Frame(4, 2, chunk.Variant + 2, 0);
            shineFrame = texture.Frame(4, 2, chunk.Variant + 2, 1);
            squish = new Vector2(1f + (float)Math.Sqrt(Utils.GetLerpValue(20, 0, chunk.Time, true)) * 0.33f, 1f - (float)Math.Sqrt(Utils.GetLerpValue(20, 0, chunk.Time, true)) * 0.33f);
        }
        Asset<Texture2D> colorMap = AssetDirectory.Textures.Extras.RainbowGelMap;
        Effect gelEffect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/RainbowGel", AssetRequestMode.ImmediateLoad).Value;
        gelEffect.Parameters["uImageSize"].SetValue(texture.Size());
        gelEffect.Parameters["uSourceRect"].SetValue(new Vector4(frame.Left, frame.Top, frame.Width, frame.Height));
        gelEffect.Parameters["uMap"].SetValue(colorMap.Value);
        gelEffect.Parameters["uRbThresholdLower"].SetValue(0.45f);
        gelEffect.Parameters["uRbThresholdUpper"].SetValue(0.55f);
        gelEffect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.5f % 1f);
        gelEffect.Parameters["uRbTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.8f % 1f);
        gelEffect.Parameters["uFrequency"].SetValue(1.1f);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, gelEffect, Main.Transform);

        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, frame, color.Value, rotation.Value, frame.Size() * new Vector2(0.5f, 0.84f), scale.Value * grow * squish, 0, 0);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, shineFrame, new Color(255, 255, 255, 0) * 0.4f, rotation.Value, frame.Size() * new Vector2(0.5f, 0.84f), scale.Value * grow * squish, 0, 0);

    }
}
