using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Projectiles;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles.FlyingSlimes;

public struct ParticleFlyingSlime
{
    public int Time { get; set; }

    public float DistanceFade { get; set; }
}

public abstract class FlyingSlimeParticleBehavior : ParticleBehavior
{
    /// <summary>
    /// The acceleration of the slime, defaults to 0.1
    /// </summary>
    public virtual float SlimeSpeed => 22f;

    /// <summary>
    /// The speed of the slime, defaults to 22
    /// </summary>
    public virtual float SlimeAcceleration => 0.1f;

    /// <summary>
    ///  Whether or not normal slime drawing applies. While false, the slime won't draw on its own.
    /// </summary>
    public virtual bool ShouldDraw => true;

    /// <summary>
    ///  Whether or not normal slime rotation applies. While false, the slime won't rotate on its own.
    /// </summary>
    public virtual bool ShouldRotate => true;

    /// <summary>
    /// The update function for the slime. Runs after the rest of Update()
    /// </summary>
    public virtual void PostUpdate(in Entity entity) { }

    /// <summary>
    /// The draw function for the slime, return false when making custom drawing
    /// </summary>
    public virtual void DrawSlime(in Entity entity, SpriteBatch spriteBatch) { }

    /// <summary>
    /// Effects for when the slime DIES
    /// </summary>
    public virtual void KillEffect(in Entity entity) { }

    public override void OnSpawn(in Entity entity)
    {
        ref var scale = ref entity.Get<ParticleScale>();
        scale.Value *= Main.rand.NextFloat(0.9f, 1.1f);
        
        entity.Add(new ParticleFlyingSlime());
    }

    public override void Update(in Entity entity)
    {
        ref var slime = ref entity.Get<ParticleFlyingSlime>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var active = ref entity.Get<ParticleActive>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        
        if (entity.TryGet<ParticleData<Vector2>>(out var data))
        {
            velocity.Value = Vector2.Lerp(velocity.Value, position.Value.DirectionTo(data.Value).SafeNormalize(Vector2.Zero) * SlimeSpeed, SlimeAcceleration);

            if (position.Value.Distance(data.Value) < 10)
            {
                active.Value = false;
                SoundEngine.PlaySound(GoozmaSpawn.slimeabsorb, position.Value);
                KillEffect(in entity);
            }

            slime.DistanceFade = Utils.GetLerpValue(20, 80, position.Value.Distance(data.Value), true);
        }
        else
            slime.DistanceFade = 1f;

        slime.Time++;

        if (slime.Time > 500)
            active.Value = false;

        velocity.Value *= 0.98f;
        rotation.Value = velocity.Value.ToRotation();

        PostUpdate(in entity);
    }

    public override void Draw(in Entity entity, SpriteBatch spriteBatch)
    {
        ref var slime = ref entity.Get<ParticleFlyingSlime>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var color = ref entity.Get<ParticleColor>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var scale = ref entity.Get<ParticleScale>();
        
        if (ShouldDraw)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);

            float fadeIn = Utils.GetLerpValue(0, 30, slime.Time, true) * slime.DistanceFade;

            for (int i = 0; i < 10; i++)
                spriteBatch.Draw(texture.Value, position.Value - velocity.Value * i * 0.5f - Main.screenPosition, null, color.Value.MultiplyRGBA(Lighting.GetColor(position.Value.ToTileCoordinates())) * fadeIn * ((10f - i) / 80f), rotation.Value + MathHelper.PiOver2, texture.Size() * 0.5f, scale.Value * slime.DistanceFade * 1.05f, 0, 0);

            spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, null, color.Value.MultiplyRGBA(Lighting.GetColor(position.Value.ToTileCoordinates())) * fadeIn, rotation.Value + MathHelper.PiOver2, texture.Size() * 0.5f, scale.Value * slime.DistanceFade, 0, 0);
        }
        else
        {
            DrawSlime(in entity, spriteBatch);
        }
    }
}
