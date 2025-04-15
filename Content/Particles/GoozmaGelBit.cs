using System;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace CalamityHunt.Content.Particles;

public sealed class GoozmaGelBit : Particle<GoozmaGelBit>
{
    public int style;

    public int direction;

    public int time;

    public int holdTime;

    public Func<Vector2> anchor;

    public ColorOffsetData colorData;
    
    public override void FetchFromPool()
    {
        base.FetchFromPool();

        style = 0;
        direction = 0;
        time = 0;
        holdTime = 0;
        anchor = null;
        colorData = default(ColorOffsetData);
    }

    public override void OnSpawn()
    {
        holdTime = Math.Max(20, holdTime);

        Scale *= Main.rand.NextFloat(1f, 1.1f);
        style = Main.rand.Next(5); // first 5 gooballs

        if (Main.rand.NextBool(750)) {
            style = Main.rand.Next(5, 8); // gooballs 6-8
        }

        direction = Main.rand.NextBool().ToDirectionInt();
        colorData = new ColorOffsetData(Main.rand.NextBool(2), Main.rand.NextFloat());
    }

    protected override void Update()
    {
        Vector2 home = anchor.Invoke();
        float distanceFromHome = Position.Distance(home);

        time++;
        Rotation += Velocity.X * 0.02f;

        if (time > 20 && time < holdTime) {
            Velocity *= 0.99f;
            Velocity = Vector2.Lerp(Velocity, Main.rand.NextVector2Circular(78, 67), 0.025f);
        }
        else if (time > holdTime + 20) {
            Velocity = Vector2.Lerp(Velocity, Position.DirectionTo(home).SafeNormalize(Vector2.Zero) * (distanceFromHome + 10) * 0.02f, 0.1f * Utils.GetLerpValue(holdTime + 20, holdTime + 40, time, true));
            Velocity = Velocity.RotatedBy(0.03f * direction);
            if (distanceFromHome < 28) {
                Scale *= 0.89f;
            }

        }
        else {
            Velocity *= 0.98f;
        }
        if (Main.netMode != NetmodeID.Server) {
            if (Main.rand.NextBool(50)) {
                CalamityHunt.ParticlesBehindEntities.SpawnParticle<ChromaticEnergyDust>(
                    particle => {
                        particle.Position = Position + Main.rand.NextVector2Circular(30, 30);
                        particle.Velocity = Main.rand.NextVector2Circular(2, 2) - Vector2.UnitY * 2f;
                        particle.Scale = new Vector2(Main.rand.NextFloat(0.1f, 1.6f));
                        particle.Color = Color.White;
                        particle.colorData = new ColorOffsetData(true, time * 2f + colorData.offset);
                    }
                );
            }

            if (Main.rand.NextBool(120)) {
                CalamityHunt.ParticlesBehindEntities.SpawnParticle<ChromaticGooBurst>(particle => {
                    particle.Position = Position + Main.rand.NextVector2Circular(30, 30);
                    particle.Velocity = -Velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.2f) - Vector2.UnitY * Main.rand.NextFloat();
                    particle.Scale = new Vector2(Main.rand.NextFloat(0.1f, 1.6f));
                    particle.Color = Color.White;
                    particle.colorData = new ColorOffsetData(true, time * 2f + colorData.offset);
                });
            }

            if (Main.rand.NextBool(70)) {
                Dust.NewDustPerfect(Position + Main.rand.NextVector2Circular(10, 10), DustID.TintableDust, Main.rand.NextVector2CircularEdge(3, 3), 100, Color.Black, Main.rand.NextFloat(2, 4)).noGravity = true;
            }
        }

        if (Scale.X < 0.5f) {
            ShouldBeRemovedFromRenderer = true;
        }

        
    }


    protected override void Draw(SpriteBatch spriteBatch)
    {
        Texture2D texture = TextureAsset.Value;
        Texture2D glow = AssetDirectory.Textures.Glow[0].Value;
        Rectangle frame = texture.Frame(8, 1, style, 0);

        Color glowColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(time * 2f + colorData.offset);
        glowColor.A = 0;

        for (int i = 0; i < 4; i++) {
            Vector2 off = new Vector2(2).RotatedBy(MathHelper.TwoPi / 4f * i + Rotation);
            spriteBatch.Draw(texture, Position + off - Main.screenPosition, frame, glowColor, Rotation, frame.Size() * 0.5f, Scale, 0, 0);
        }

        if (colorData.active) {
            GetGradientMapValues(out float[] brightnesses, time, out Vector3[] colors);
            Effect effect = AssetDirectory.Effects.HolographicGel.Value;
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly % 1f);
            effect.Parameters["colors"].SetValue(colors);
            effect.Parameters["brightnesses"].SetValue(brightnesses);
            effect.Parameters["baseToScreenPercent"].SetValue(1f);
            effect.Parameters["baseToMapPercent"].SetValue(0f);
            effect.CurrentTechnique.Passes[0].Apply();
        }

        spriteBatch.Draw(texture, Position - Main.screenPosition, frame, Color.Lerp(Color, Color.Black, 0.6f), Rotation, frame.Size() * 0.5f, Scale, 0, 0);

        if (colorData.active) {
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }
    }

    public void GetGradientMapValues(out float[] brightnesses, int time, out Vector3[] colors)
    {
        float maxBright = 0.667f;
        brightnesses = new float[10];
        colors = new Vector3[10];

        float rainbowStartOffset = 0.35f + time * 0.01f % (maxBright * 2f);
        //Calculate and store every non-modulo brightness, with the shifting offset. 
        //The first brightness is ignored for the moment, it will be relevant later. Setting it to -1 temporarily
        brightnesses[0] = -1;
        brightnesses[1] = rainbowStartOffset + 0.35f;
        brightnesses[2] = rainbowStartOffset + 0.42f;
        brightnesses[3] = rainbowStartOffset + 0.47f;
        brightnesses[4] = rainbowStartOffset + 0.51f;
        brightnesses[5] = rainbowStartOffset + 0.56f;
        brightnesses[6] = rainbowStartOffset + 0.61f;
        brightnesses[7] = rainbowStartOffset + 0.64f;
        brightnesses[8] = rainbowStartOffset + 0.72f;
        brightnesses[9] = rainbowStartOffset + 0.75f;

        //Pass the entire rainbow through modulo 1
        for (int i = 1; i < 10; i++) {
            brightnesses[i] = HUtils.Modulo(brightnesses[i], maxBright) * maxBright;
        }

        //Store the first element's value so we can find it again later
        float firstBrightnessValue = brightnesses[1];

        //Sort the values from lowest to highest
        Array.Sort(brightnesses);

        //Find the new index of the original first element after the list being sorted
        int rainbowStartIndex = Array.IndexOf(brightnesses, firstBrightnessValue);
        //Substract 1 from the index, because we are ignoring the currently negative first array slot.
        rainbowStartIndex--;

        //9 loop, filling a list of colors in a array of 10 elements (ignoring the first one)
        for (int i = 0; i < 9; i++) {
            colors[1 + (rainbowStartIndex + i) % 9] = SlimeUtils.GoozColorsVector3[i];
        }

        //We always want a brightness at index 0 to be the lower bound
        brightnesses[0] = 0;
        //Make the color at index 0 be a mix between the first and last colors in the list, based on the distance between the 2.
        float interpolant = (1 - brightnesses[9]) / (brightnesses[1] + (1 - brightnesses[9]));
        colors[0] = Vector3.Lerp(colors[9], colors[0], interpolant);
    }
    
    protected override GoozmaGelBit NewInstance()
    {
        return new GoozmaGelBit();
    }
}
