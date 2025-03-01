using System;
using System.Linq;
using Arch.Core.Extensions;
using CalamityHunt.Common.Systems;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Systems.ParticlesOld;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Entity = Arch.Core.Entity;

namespace CalamityHunt.Content.Particles;

public struct ChromaticEnergyDust2
{
    public int Frame { get; set; }

    public float Life { get; set; }

    public float[] oldRot { get; set; }

    public Vector2[] oldPos {  get; set; }

    public bool frog {  get; set; }
}

public class ChromaticEnergyDust2Behavior : ParticleBehavior
{
    public override void OnSpawn(in Entity entity)
    {
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var scale = ref entity.Get<ParticleScale>();
        ref var position = ref entity.Get<ParticlePosition>();
        rotation.Value += Main.rand.NextFloat(-3f, 3f);
        scale.Value *= 1.5f;

        var dust = new ChromaticEnergyDust2
        {
            Frame = Main.rand.Next(3),
        };
        dust.oldPos = Enumerable.Repeat(position.Value, 8).ToArray();
        dust.oldRot = Enumerable.Repeat(rotation.Value, 8).ToArray();
        if (Main.zenithWorld && BossDownedSystem.Instance.GoozmaDowned) {
            dust.frog = Main.rand.NextBool(100);
        }
        entity.Add(dust);
    }

    public override void Update(in Entity entity)
    {
        ref var dust = ref entity.Get<ChromaticEnergyDust2>();
        ref var scale = ref entity.Get<ParticleScale>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var velocity = ref entity.Get<ParticleVelocity>();
        ref var color = ref entity.Get<ParticleColor>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var active = ref entity.Get<ParticleActive>();

        dust.Life += 0.1f;
        scale.Value *= 0.99f;
        rotation.Value += velocity.Value.X * 0.2f;

        if (dust.Life > 4f) {
            scale.Value *= 0.95f;
        }

        for (int i = dust.oldPos.Length - 1; i > 0; i--) {
            dust.oldPos[i] = Vector2.Lerp(dust.oldPos[i - 1], position.Value, 0.1f);
            dust.oldRot[i] = dust.oldRot[i - 1];
        }
        dust.oldRot[0] = position.Value.AngleFrom(dust.oldPos[0]);
        dust.oldPos[0] = position.Value;

        velocity.Value *= 0.97f;
        velocity.Value = Vector2.Lerp(velocity.Value, Main.rand.NextVector2Circular(5, 5), 0.02f + dust.Life * 0.02f);

        if (entity.TryGet<ParticleData<float>>(out var data))
            color.Value = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(data.Value + dust.Life * 0.05f);

        if (!Collision.SolidTiles(position.Value, 2, 2)) {
            scale.Value *= 1.0005f;
        }

        if (Main.rand.NextBool(250) && scale.Value > 0.25f) {
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
        Texture2D texture = AssetDirectory.Textures.Particle[Type].Value;
        Texture2D glow = AssetDirectory.Textures.Glow[0].Value;

        ref var dust = ref entity.Get<ChromaticEnergyDust2>();
        ref var color = ref entity.Get<ParticleColor>();
        ref var position = ref entity.Get<ParticlePosition>();
        ref var rotation = ref entity.Get<ParticleRotation>();
        ref var scale = ref entity.Get<ParticleScale>();

        if (dust.frog) {
            texture = AssetDirectory.Textures.FrogParticle.Value;
            spriteBatch.Draw(texture, position.Value - Main.screenPosition, texture.Frame(), Color.Lerp(color.Value with { A = 128 }, Color.White, 0.5f + MathF.Sin(dust.Life * 0.1f) * 0.5f), rotation.Value, texture.Size() * 0.5f, scale.Value * 0.3f, 0, 0);
            return;
        }

        for (int i = 1; i < dust.oldPos.Length; i++) {
            Color trailColor = color.Value with { A = 40 } * (float)Math.Pow(1f - ((float)i / dust.oldPos.Length), 2f) * 0.1f;
            Vector2 trailStretch = new Vector2(dust.oldPos[i].Distance(dust.oldPos[i - 1]) * 0.5f, scale.Value * 0.4f);
            spriteBatch.Draw(texture, dust.oldPos[i] - Main.screenPosition, null, trailColor, dust.oldRot[i], texture.Size() * 0.5f, trailStretch, 0, 0);
        }

        spriteBatch.Draw(texture, position.Value - Main.screenPosition, texture.Frame(), (color.Value * 0.9f) with { A = (byte)(color.Value.A / 2f + 20) }, rotation.Value, texture.Size() * 0.5f, scale.Value, 0, 0);

        float innerGlowScale = 0.7f * Utils.GetLerpValue(5f, 1.5f, dust.Life, true);
        spriteBatch.Draw(texture, position.Value - Main.screenPosition, texture.Frame(), Color.White with { A = 0 }, rotation.Value, texture.Size() * 0.5f, scale.Value * innerGlowScale, 0, 0);
    }
}
