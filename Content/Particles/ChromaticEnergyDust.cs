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

public sealed class ChromaticEnergyDust : Particle<ChromaticEnergyDust>
{
    public float life;

    public ColorOffsetData colorData;

    private bool frogicle;

    private Vector2[] oldPos;

    private float[] oldRot;
    
    public override void FetchFromPool()
    {
        base.FetchFromPool();

        life = 0;
        colorData = default(ColorOffsetData);
        frogicle = false;
        oldPos = null;
        oldRot = null;
    }

    public override void OnSpawn()
    {
        Rotation += Main.rand.NextFloat(-3f, 3f);
        oldPos = Enumerable.Repeat(Position, 8).ToArray();
        oldRot = Enumerable.Repeat(Rotation, 8).ToArray();
        if (Main.zenithWorld && BossDownedSystem.Instance.GoozmaDowned) {
            frogicle = Main.rand.NextBool(100);
        }
    }

    protected override void Update()
    {
        life += 0.1f;
        Scale *= 0.99f;
        Rotation += Velocity.X * 0.2f;

        if (life > 4f) {
            Scale *= 0.95f;
        }

        Velocity *= 0.97f;
        Velocity = Vector2.Lerp(Velocity, Main.rand.NextVector2Circular(5, 5), 0.02f + life * 0.02f);

        if (colorData.active) {
            Color = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(colorData.offset + life * 0.05f);
        }

        for (int i = oldPos.Length - 1; i > 0; i--) {
            oldPos[i] = Vector2.Lerp(oldPos[i - 1], Position, 0.1f);
            oldRot[i] = oldRot[i - 1];
        }
        oldRot[0] = Position.AngleFrom(oldPos[0]);
        oldPos[0] = Position;

        if (!Collision.SolidTiles(Position, 2, 2)) {
            Scale *= 1.0005f;
            //Lighting.AddLight(position, color.ToVector3() * 0.2f * scale);
        }

        if (Main.rand.NextBool(250) && Scale.X > 0.25f) {
            CalamityHunt.Particles.Add(Create<ChromaticEnergyDust>(newParticle => {
                newParticle.position = Position;
                newParticle.color = Color * 0.99f;
                newParticle.scale = MathHelper.Clamp(Scale.X * 2f, 0.1f, 1.5f);
                newParticle.colorData = colorData;
            }));
        }

        if (Scale.X < 0.1f) {
            ShouldBeRemovedFromRenderer = true;
        }
    }

    protected override void Draw(SpriteBatch spriteBatch)
    {
        Texture2D texture = TextureAsset.Value;
        Texture2D glow = AssetDirectory.Textures.Glow[0].Value;

        if (frogicle) {
            texture = AssetDirectory.Textures.FrogParticle.Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, texture.Frame(), Color.Lerp(Color with { A = 128 }, Color.White, 0.5f + MathF.Sin(life * 0.1f) * 0.5f), Rotation, texture.Size() * 0.5f, Scale * 0.3f, 0, 0);
            return;
        }

        for (int i = 1; i < oldPos.Length; i++) {
            Color trailColor = Color with { A = 40 } * (float)Math.Pow(1f - ((float)i / oldPos.Length), 2f) * 0.1f;
            Vector2 trailStretch = new Vector2(oldPos[i].Distance(oldPos[i - 1]) * 0.5f, Scale.X * 0.4f);
            spriteBatch.Draw(texture, oldPos[i] - Main.screenPosition, null, trailColor, oldRot[i], texture.Size() * 0.5f, trailStretch, 0, 0);
        }

        spriteBatch.Draw(texture, Position - Main.screenPosition, texture.Frame(), (Color * 0.9f) with { A = (byte)(Color.A / 2f + 20) }, Rotation, texture.Size() * 0.5f, Scale, 0, 0);

        float innerGlowScale = 0.7f * Utils.GetLerpValue(5f, 1.5f, life, true);
        spriteBatch.Draw(texture, Position - Main.screenPosition, texture.Frame(), Color.White with { A = 0 }, Rotation, texture.Size() * 0.5f, Scale * innerGlowScale, 0, 0);
    }

    protected override ChromaticEnergyDust NewInstance()
    {
        return new ChromaticEnergyDust();
    }
}
