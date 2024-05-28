using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;

namespace CalamityHunt.Common.Systems.FlyingSlimes;

public class FlyingSlime
{
    public float progress;

    public Vector2 targetPosition;

    public Vector2 startPosition;

    public Vector2 currentPosition;

    public float rotation;

    public float scale;

    public FlyingSlimeData data;

    public bool ShouldRemove { get; private set; }

    public float curvature;

    public void Update(float curveModifier = 0f)
    {
        Vector2 distance = Vector2.Lerp(startPosition, targetPosition, progress);
        Vector2 lastPos = new Vector2(currentPosition.X, currentPosition.Y);
        startPosition += (startPosition.AngleTo(targetPosition) - MathHelper.PiOver2).ToRotationVector2() * (curvature + curveModifier) * MathHelper.TwoPi * (1f - progress * 0.7f);
        currentPosition = distance;
        rotation = (lastPos - currentPosition).ToRotation() - MathHelper.PiOver2;
        progress += data.Speed / 80f;

        if (currentPosition.Distance(targetPosition) < 20 || progress > Main.rand.NextFloat(0.98f, 1f)) {
            ShouldRemove = true;
        }

        data.DustMethod?.Invoke(data, currentPosition);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        Color drawColor = Lighting.GetColor(currentPosition.ToTileCoordinates()) * Utils.GetLerpValue(0, 0.1f, progress, true);

        if (data.SpecialDraw != null) {
            data.SpecialDraw(data, spriteBatch, currentPosition - screenPos, rotation, scale, progress, drawColor);
            return;
        }

        Texture2D texture = AssetDirectory.Textures.FlyingSlime[data.Type].Value;
        Color secondColor = data.DrawColor;
        if (ContentSamples.NpcsByNetId.ContainsKey(data.NPCType)) {
            secondColor = ContentSamples.NpcsByNetId[data.NPCType].GetAlpha(drawColor);
        }

        spriteBatch.Draw(texture, currentPosition - screenPos, texture.Frame(), secondColor, rotation, texture.Size() * new Vector2(0.5f, 0.33f), scale, 0, 0);
        if (data.DrawColor != default) {
            spriteBatch.Draw(texture, currentPosition - screenPos, texture.Frame(), drawColor.MultiplyRGBA(data.DrawColor), rotation, texture.Size() * new Vector2(0.5f, 0.33f), scale, 0, 0);
        }
    }

    public FlyingSlime(int type, float curvature)
    {
        data = FlyingSlimeLoader.flyingSlimeDataInstances[type];
        scale = Main.rand.NextFloat(0.9f, 1.1f);
        data.RandomValue = Main.rand.Next(100);
        this.curvature = curvature;
    }

    public static FlyingSlime CreateRandom()
    {
        WeightedRandom<int> random = new WeightedRandom<int>();

        foreach (FlyingSlimeData type in FlyingSlimeLoader.flyingSlimeDataInstances.ToList()) {
            if (type.SpecificCondition()) {
                random.Add(type.Type, 1f / type.Rarity);
            }
        }

        return new FlyingSlime(random.Get(), Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
    }
}
