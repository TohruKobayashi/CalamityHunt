using System;
using System.Linq;
using CalamityHunt.Common.Systems;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityHunt.Content.Particles;

public class ChromaticEnergyDust : Particle
{
    public float life;

    public ColorOffsetData colorData;

    private bool frogicle;

    private Vector2[] oldPos;

    private float[] oldRot;

    public override void OnSpawn()
    {
        rotation += Main.rand.NextFloat(-3f, 3f);
        oldPos = Enumerable.Repeat(position, 8).ToArray();
        oldRot = Enumerable.Repeat(rotation, 8).ToArray();
        if (Main.zenithWorld && BossDownedSystem.Instance.GoozmaDowned) {
            frogicle = Main.rand.NextBool(100);
        }
    }

    public override void Update()
    {
        life += 0.1f;
        scale *= 0.99f;
        rotation += velocity.X * 0.2f;

        if (life > 4f) {
            scale *= 0.95f;
        }

        velocity *= 0.97f;
        velocity = Vector2.Lerp(velocity, Main.rand.NextVector2Circular(5, 5), 0.02f + life * 0.02f);

        if (colorData.active) {
            color = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(colorData.offset + life * 0.05f);
        }

        for (int i = oldPos.Length - 1; i > 0; i--) {
            oldPos[i] = Vector2.Lerp(oldPos[i - 1], position, 0.1f);
            oldRot[i] = oldRot[i - 1];
        }
        oldRot[0] = position.AngleFrom(oldPos[0]);
        oldPos[0] = position;

        if (!Collision.SolidTiles(position, 2, 2)) {
            scale *= 1.0005f;
            //Lighting.AddLight(position, color.ToVector3() * 0.2f * scale);
        }

        if (Main.rand.NextBool(250) && scale > 0.25f) {
            CalamityHunt.particles.Add(Create<ChromaticEnergyDust>(newParticle => {
                newParticle.position = position;
                newParticle.color = color * 0.99f;
                newParticle.scale = MathHelper.Clamp(scale * 2f, 0.1f, 1.5f);
                newParticle.colorData = colorData;
            }));
        }

        if (scale < 0.1f) {
            ShouldRemove = true;
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Texture2D texture = AssetDirectory.Textures.Particle[Type].Value;
        Texture2D glow = AssetDirectory.Textures.Glow[0].Value;

        if (frogicle) {
            texture = AssetDirectory.Textures.FrogParticle.Value;
            spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), Color.Lerp(color with { A = 128 }, Color.White, 0.5f + MathF.Sin(life * 0.1f) * 0.5f), rotation, texture.Size() * 0.5f, scale * 0.3f, 0, 0);
            return;
        }

        for (int i = 1; i < oldPos.Length; i++) {
            Color trailColor = color with { A = 40 } * (float)Math.Pow(1f - ((float)i / oldPos.Length), 2f) * 0.1f;
            Vector2 trailStretch = new Vector2(oldPos[i].Distance(oldPos[i - 1]) * 0.5f, scale * 0.4f);
            spriteBatch.Draw(texture, oldPos[i] - Main.screenPosition, null, trailColor, oldRot[i], texture.Size() * 0.5f, trailStretch, 0, 0);
        }

        spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), (color * 0.9f) with { A = (byte)(color.A / 2f + 20) }, rotation, texture.Size() * 0.5f, scale, 0, 0);

        float innerGlowScale = 0.7f * Utils.GetLerpValue(5f, 1.5f, life, true);
        spriteBatch.Draw(texture, position - Main.screenPosition, texture.Frame(), Color.White with { A = 0 }, rotation, texture.Size() * 0.5f, scale * innerGlowScale, 0, 0);
    }
}
