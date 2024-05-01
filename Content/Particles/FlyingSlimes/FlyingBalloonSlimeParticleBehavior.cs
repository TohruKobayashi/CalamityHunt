using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Arch.Core.Extensions;
using CalamityHunt.Common.Systems.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles.FlyingSlimes;

public struct ParticleFlyingBalloonSlime
{
    public int BalloonWobbleTime { get; set; }

    public int BalloonVariant { get; set; }
}
    
public class FlyingBalloonSlimeParticleBehavior : FlyingSlimeParticleBehavior
{
    public override bool ShouldDraw => false;
    public override float SlimeSpeed => 13f;
    public override float SlimeAcceleration => 0.2f;

    public override void OnSpawn(in Entity entity)
    {
        base.OnSpawn(in entity);

        var balloonSlime = new ParticleFlyingBalloonSlime
        {
            BalloonVariant = Main.rand.Next(7),
        };
        entity.Add(balloonSlime);
    }

    public override void PostUpdate(in Entity entity)
    {
        ref var color = ref entity.Get<ParticleColor>();
        ref var scale = ref entity.Get<ParticleScale>();

        if (color.Value != Color.White)
            return;

        WeightedRandom<Color> slimeColor = new WeightedRandom<Color>();
        slimeColor.Add(ContentSamples.NpcsByNetId[NPCID.GreenSlime].color, 1f);
        slimeColor.Add(ContentSamples.NpcsByNetId[NPCID.BlueSlime].color, 0.9f);
        color.Value = slimeColor.Get();

        if (Main.rand.NextBool(100))
        {
            color.Value = ContentSamples.NpcsByNetId[NPCID.PurpleSlime].color;
            scale.Value = ContentSamples.NpcsByNetId[NPCID.PurpleSlime].scale;
        }
        if (Main.rand.NextBool(3000))
        {
            color.Value = ContentSamples.NpcsByNetId[NPCID.Pinky].color;
            scale.Value = ContentSamples.NpcsByNetId[NPCID.Pinky].scale;
        }
    }

    public override void DrawSlime(in Entity entity, SpriteBatch spriteBatch)
    {
        ref var flyingSlime = ref entity.Get<ParticleFlyingSlime>();
        ref var balloonSlime = ref entity.Get<ParticleFlyingBalloonSlime>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var color = ref entity.Get<ParticleColor>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var scale = ref entity.Get<ParticleScale>();
        
        balloonSlime.BalloonWobbleTime++;

        Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
        Asset<Texture2D> balloon = ModContent.Request<Texture2D>(Texture + "Balloons");
        Rectangle balloonFrame = balloon.Frame(7, 1, balloonSlime.BalloonVariant, 0);
        float fadeIn = Utils.GetLerpValue(0, 30, flyingSlime.Time, true) * flyingSlime.DistanceFade;

        for (int i = 0; i < 10; i++)
            spriteBatch.Draw(texture.Value, position.Value - velocity.Value * i * 0.5f - Main.screenPosition, null, color.Value.MultiplyRGBA(Lighting.GetColor(position.Value.ToTileCoordinates())) * fadeIn * ((10f - i) / 50f), rotation.Value + MathHelper.PiOver2, texture.Size() * 0.5f, scale.Value * flyingSlime.DistanceFade * 1.05f, 0, 0);

        spriteBatch.Draw(texture.Value, position.Value - Main.screenPosition, null, color.Value.MultiplyRGBA(Lighting.GetColor(position.Value.ToTileCoordinates())) * fadeIn, rotation.Value + MathHelper.PiOver2, texture.Size() * 0.5f, scale.Value * flyingSlime.DistanceFade, 0, 0);
        spriteBatch.Draw(balloon.Value, position.Value + new Vector2(texture.Height() / 2f * scale.Value * flyingSlime.DistanceFade, 0).RotatedBy(rotation.Value) - Main.screenPosition, balloonFrame, Lighting.GetColor(position.Value.ToTileCoordinates()) * fadeIn, rotation.Value - MathHelper.PiOver2 + (float)Math.Sin(balloonSlime.BalloonWobbleTime * 0.5f) * 0.07f, balloonFrame.Size() * new Vector2(0.5f, 1f), flyingSlime.DistanceFade, 0, 0);
    }
}
