using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles;

public struct ParticleHueLightDust
{
    public int Frame { get; set; }

    public float Life { get; set; }
}
    
public class HueLightDustParticleBehavior : ParticleBehavior
{
    public override void OnSpawn(in Entity entity)
    {
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var scale = ref entity.Get<ParticleScale>();
        rotation.Value += Main.rand.NextFloat(-3f, 3f);
        scale.Value *= 1.5f;

        var dust = new ParticleHueLightDust
        {
            Frame = Main.rand.Next(3),
        };
        entity.Add(dust);
    }

    public override void Update(in Entity entity)
    {
        ref var dust = ref entity.Get<ParticleHueLightDust>();
        ref var scale = ref entity.Get<ParticleScale>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var color = ref entity.Get<ParticleColor>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var active = ref entity.Get<ParticleActive>();
        
        dust.Life += 0.1f;
        scale.Value *= 0.94f;
        rotation.Value += velocity.Value.X * 0.2f;

        scale.Value -= 0.02f;
        velocity.Value *= 0.97f;
        velocity.Value += new Vector2(Main.rand.Next(-3, 3) * 0.005f, Main.rand.Next(-3, 3) * 0.008f);
        
        if (entity.TryGet<ParticleData<float>>(out var data))
            color.Value = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(data.Value + dust.Life * 0.05f);

        if (!Collision.SolidTiles(position.Value, 2, 2))
        {
            scale.Value *= 1.0004f;
            Lighting.AddLight(position.Value, color.Value.ToVector3() * 0.2f * scale.Value);
        }

        if (Main.rand.NextBool(150) && scale.Value > 0.25f)
        {
            var hue = NewParticle(this, position.Value, Main.rand.NextVector2Circular(1, 1), color.Value * 0.99f, MathHelper.Clamp(scale.Value * 2f, 0.1f, 1.5f));
            // TODO: This is passed as `in`, don't modify ever...
            hue.Add(data);
        }

        if (scale.Value < 0.1f)
            active.Value = false;
    }

    public static Asset<Texture2D> texture;

    public override void Load()
    {
        texture = AssetUtilities.RequestImmediate<Texture2D>(Texture);
    }

    public override void Draw(in Entity entity, SpriteBatch spriteBatch)
    {
        ref var dust = ref entity.Get<ParticleHueLightDust>();
        ref var color = ref entity.Get<ParticleColor>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var scale = ref entity.Get<ParticleScale>();
        
        Texture2D glow = AssetDirectory.Textures.Glow.Value;
        Rectangle rect = texture.Value.Frame(1, 3, 0, dust.Frame);
        Color drawColor = color.Value;
        drawColor.A /= 3;
        Color glowColor = color.Value * 0.2f;
        glowColor.A = 0;
        Color whiteColor = Color.White;
        whiteColor.A = 0;

        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, rect, drawColor, rotation.Value, rect.Size() * 0.5f, scale.Value, 0, 0);
        spriteBatch.Draw(glow, position.Value - Main.screenPosition, null, glowColor * 0.5f, rotation.Value, glow.Size() * 0.5f, scale.Value * 0.5f, 0, 0);

        float innerGlowScale = 0.7f - Utils.GetLerpValue(0f, 1f, dust.Life, true) * 0.2f;
        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, rect, whiteColor, rotation.Value, rect.Size() * 0.5f, scale.Value * innerGlowScale * 0.7f, 0, 0);
    }
}
