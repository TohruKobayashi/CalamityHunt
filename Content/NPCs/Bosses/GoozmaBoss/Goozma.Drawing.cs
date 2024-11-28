using System;
using System.Collections.Generic;
using CalamityHunt.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;

//IbanPlay shader code
public partial class Goozma : ModNPC
{
    private static float maxBright;
    public static bool IsXmas => DateTime.Now.Month == 12 && DateTime.Now.Day >= 15;

    public static void GetGradientMapValues(out float[] brightnesses, out Vector3[] colors) => GetGradientMapValues(SlimeUtils.GoozColorsVector3, out brightnesses, out colors);

    public static void GetGradientMapValues(Vector3[] gradient, out float[] brightnesses, out Vector3[] colors)
    {
        maxBright = 0.667f;
        brightnesses = new float[10];
        colors = new Vector3[10];

        float rainbowStartOffset = 0.35f + Main.GlobalTimeWrappedHourly * 0.5f % (maxBright * 2f);
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
            colors[1 + (rainbowStartIndex + i) % 9] = gradient[i];
        }

        //We always want a brightness at index 0 to be the lower bound
        brightnesses[0] = 0;
        //Make the color at index 0 be a mix between the first and last colors in the list, based on the distance between the 2.
        float interpolant = (1 - brightnesses[9]) / (brightnesses[1] + (1 - brightnesses[9]));
        colors[0] = Vector3.Lerp(colors[9], colors[0], interpolant);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D sclera = AssetDirectory.Textures.Goozma.Sclera.Value;
        Texture2D godEye = AssetDirectory.Textures.ChromaticSoulEye.Value;

        Texture2D glow = AssetDirectory.Textures.Glow[0].Value;
        Texture2D sparkle = AssetDirectory.Textures.Sparkle.Value;

        SpriteEffects direction = NPC.direction < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        Color glowColor = (new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(NPC.localAI[0])) with { A = 0 };

        //Texture2D stick = TextureAssets.FishingLine.Value;
        //spriteBatch.Draw(stick, NPC.Center - Main.screenPosition, null, Color.DimGray, 0, TextureAssets.FishingLine.Size() * new Vector2(0.5f, 0f), new Vector2(6f, 150f / stick.Height + 10 * NPC.scale), 0, 0);
        //spriteBatch.Draw(stick, NPC.Center - Main.screenPosition, null, Color.LightGray, 0, TextureAssets.FishingLine.Size() * new Vector2(0.5f, 0f), new Vector2(4f, 148f / stick.Height + 10 * NPC.scale), 0, 0);

        oldVel ??= new Vector2[NPCID.Sets.TrailCacheLength[Type]];

        float trailStrength = Phase > 1 ? 1.3f : 0.5f;

        if (NPC.IsABestiaryIconDummy) {
            headScale = 1f;
        }

        float eyeScale = 1.1f + (float)Math.Sin(NPC.localAI[0] * 0.025f % MathHelper.TwoPi) * 0.15f;
        float eyeRot = -MathHelper.PiOver4;

        spriteBatch.Draw(glow, NPC.Center - screenPos, glow.Frame(), glowColor * 0.2f, extraTilt + NPC.rotation, glow.Size() * 0.5f, 5f * NPC.scale, 0, 0);

        Vector2 realEyeOffset = drawOffset * 0.5f + new Vector2(15 * NPC.direction, -22).RotatedBy(extraTilt * 0.9f + NPC.rotation) * headScale * NPC.scale;

        if (Phase == 1 && Time >= 50) {
            eyeScale *= (float)Math.Cbrt(Utils.GetLerpValue(290, 330, Time, true));
            float eyeFlash = Utils.GetLerpValue(300, 360, Time, true);
            spriteBatch.Draw(godEye, NPC.Center - screenPos + realEyeOffset, godEye.Frame(), glowColor * (1f - eyeFlash) * 0.5f, eyeRot, godEye.Size() * 0.5f, eyeScale * (1f + eyePower.Length() * 0.06f + eyeFlash * 3f), 0, 0);
            spriteBatch.Draw(godEye, NPC.Center - screenPos + realEyeOffset, godEye.Frame(), glowColor * (1f - eyeFlash) * 0.2f, eyeRot, godEye.Size() * 0.5f, eyeScale * (1f + eyePower.Length() * 0.06f + eyeFlash * 9f), 0, 0);

            spriteBatch.Draw(sclera, NPC.Center - screenPos + realEyeOffset, sclera.Frame(), Color.White, 0, sclera.Size() * 0.5f, 1.05f, 0, 0);
            spriteBatch.Draw(glow, NPC.Center - screenPos + realEyeOffset, glow.Frame(), glowColor * 0.5f, extraTilt + NPC.rotation, glow.Size() * 0.5f, 1.2f, 0, 0);
        }

        RequestTarget(spriteBatch);

        if (goozmaContent.IsTargetReady(NPC.whoAmI)) {
            Texture2D goozmaTexture = goozmaContent.GetTarget(NPC.whoAmI);

            RestartSpriteBatch(spriteBatch, null, true);

            //Main.pixelShader.CurrentTechnique.Passes["ColorOnly"].Apply();

            // Afterimages
            if (!NPC.IsABestiaryIconDummy) {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[Type]; i++) {
                    Vector2 oldPos = NPC.oldPos[i] + NPC.Size * 0.5f;
                    Color trailColor = (new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(NPC.localAI[0] - i * 5f)) with { A = 170 };
                    trailColor = Color.Lerp(trailColor, Color.Transparent, MathF.Pow((float)i / NPCID.Sets.TrailCacheLength[Type], 0.3f)) * 0.5f;
                    spriteBatch.Draw(goozmaTexture, oldPos + drawOffset - screenPos, goozmaTexture.Frame(), trailColor, NPC.oldRot[i], goozmaTexture.Size() * 0.5f, NPC.scale, direction, 0);
                }
            }

            // My solid color blast
            Main.pixelShader.CurrentTechnique.Passes["ColorOnly"].Apply();

            // Outline 
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[Type]; i++) {
                Color outlineColor = (new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(NPC.localAI[0] * 5f)) with { A = 170 };
                outlineColor = Color.Lerp(outlineColor, Color.Transparent, MathF.Pow((float)i / NPCID.Sets.TrailCacheLength[Type], 0.3f)) * 0.5f;
                spriteBatch.Draw(goozmaTexture, (NPC.position + NPC.Size * 0.5f) + drawOffset - screenPos, goozmaTexture.Frame(), outlineColor, NPC.rotation, goozmaTexture.Size() * 0.5f, NPC.scale, direction, 0);
            }

            for (int i = 0; i < 4; i++) {
                Vector2 off = new Vector2(2, 0).RotatedBy(MathHelper.TwoPi / 4f * i + NPC.rotation);
                spriteBatch.Draw(goozmaTexture, NPC.Center + drawOffset + off - screenPos, goozmaTexture.Frame(), glowColor with { A = 170 }, NPC.rotation, goozmaTexture.Size() * 0.5f, NPC.scale, direction, 0);
            }

            // No more solid color blast
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            DrawSoulStuff();

            spriteBatch.Draw(goozmaTexture, NPC.Center + drawOffset - screenPos, goozmaTexture.Frame(), Color.White, NPC.rotation, goozmaTexture.Size() * 0.5f, NPC.scale, direction, 0);

            RestartSpriteBatch(spriteBatch, null, false);
        }

        if (eyeOpen) {
            spriteBatch.Draw(godEye, NPC.Center - screenPos + realEyeOffset, godEye.Frame(), new Color(200, 200, 200, 150), eyeRot, godEye.Size() * 0.5f, eyeScale * 1.3f * eyePower, 0, 0);
            spriteBatch.Draw(godEye, NPC.Center - screenPos + realEyeOffset, godEye.Frame(), glowColor, eyeRot, godEye.Size() * 0.5f, (eyeScale * 1.3f + 0.1f) * eyePower, 0, 0);

            spriteBatch.Draw(sparkle, NPC.Center - screenPos + realEyeOffset, sparkle.Frame(), glowColor * 0.15f, eyeRot + MathHelper.PiOver2, sparkle.Size() * 0.5f, eyeScale * new Vector2(0.33f, 3f) + new Vector2(0, eyePower.Y), 0, 0);
            spriteBatch.Draw(sparkle, NPC.Center - screenPos + realEyeOffset, sparkle.Frame(), glowColor * 0.15f, eyeRot, sparkle.Size() * 0.5f, eyeScale * new Vector2(0.33f, 4f) + new Vector2(0, eyePower.X), 0, 0);
            //spriteBatch.Draw(sparkle, NPC.Center - screenPos + realEyeOffset, sparkle.Frame(), glowColor * 0.15f, eyeRot + MathHelper.PiOver4, sparkle.Size() * 0.5f, eyeScale * new Vector2(0.33f, 3f) + new Vector2(0, eyePower.X), 0, 0);
            //spriteBatch.Draw(sparkle, NPC.Center - screenPos + realEyeOffset, sparkle.Frame(), glowColor * 0.15f, eyeRot - MathHelper.PiOver4, sparkle.Size() * 0.5f, eyeScale * new Vector2(0.33f, 3f) + new Vector2(0, eyePower.Y), 0, 0);

            spriteBatch.Draw(glow, NPC.Center - screenPos + realEyeOffset, null, glowColor * 0.3f, extraTilt + NPC.rotation, glow.Size() * 0.5f, 2f * eyeScale, 0, 0);
        }

        return false;
    }

    public void DrawSoulStuff()
    {
        soulStrip ??= new VertexStrip();


    }

    public void RestartSpriteBatch(SpriteBatch spriteBatch, Effect effect, bool immediate)
    {
        if (NPC.IsABestiaryIconDummy) {
            RasterizerState priorRasterizer = spriteBatch.GraphicsDevice.RasterizerState;
            Rectangle priorScissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            spriteBatch.End();
            spriteBatch.GraphicsDevice.RasterizerState = priorRasterizer;
            spriteBatch.GraphicsDevice.ScissorRectangle = priorScissorRectangle;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, priorRasterizer, effect, Main.UIScaleMatrix);
        }
        else {
            spriteBatch.End();
            SpriteSortMode sortMode = SpriteSortMode.Deferred;
            if (immediate) {
                sortMode = SpriteSortMode.Immediate;
            }

            spriteBatch.Begin(sortMode, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.Transform);
        }
    }

    public static Texture2D dressTexture;
    public static Texture2D crownTexture;
    public static Texture2D crownMaskTexture;
    public static Texture2D tentacleTexture;
    public static RenderTargetDrawContent goozmaContent;
    public static RenderTargetDrawContent cordContent;
    public VertexStrip soulStrip;
    public VertexStrip cordStrip;
    //move all this to asset dir 
    private void LoadAssets()
    {
        dressTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "Dress").Value;
        crownTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "Crown").Value;
        crownMaskTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "CrownMask").Value;
        tentacleTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "Tentacle").Value;
        Main.ContentThatNeedsRenderTargets.Add(goozmaContent = new RenderTargetDrawContent());
        Main.ContentThatNeedsRenderTargets.Add(cordContent = new RenderTargetDrawContent());
    }

    private void RequestTarget(SpriteBatch spriteBatch)
    {
        goozmaContent.Request(2000, 2000, NPC.whoAmI, sb => {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.EffectMatrix);

            GetGradientMapValues(out float[] brightnesses, out Vector3[] colors);

            Effect effect = AssetDirectory.Effects.HolographicGel.Value;
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly % 1f);
            effect.Parameters["colors"].SetValue(colors);
            effect.Parameters["brightnesses"].SetValue(brightnesses);
            effect.Parameters["baseToScreenPercent"].SetValue(1.05f);
            effect.Parameters["baseToMapPercent"].SetValue(-0.05f);
            if (Phase == 3) {
                effect.Parameters["baseToScreenPercent"].SetValue(1.05f - Utils.GetLerpValue(80, 180, Time, true) * 1.05f);
                effect.Parameters["baseToMapPercent"].SetValue(-0.05f + Utils.GetLerpValue(20, 150, Time, true) * 0.1f);//-1f + Utils.GetLerpValue(20, 180, Time, true)
            }
            effect.CurrentTechnique.Passes[0].Apply();

            Texture2D texture = TextureAssets.Npc[Type].Value;

            Vector2 position = new Vector2(1000);
            Color color = Color.White;
            Vector2 velocity = new Vector2(NPC.velocity.X * NPC.direction, -NPC.velocity.Y);
            Vector2 tentacleVel = new Vector2(-tentacleVelocity.X * NPC.direction, tentacleVelocity.Y);
            float tilt = -extraTilt * NPC.direction;

            Vector2 crownPos = position - new Vector2(-4, 44).RotatedBy(tilt * 0.8f) * headScale;
            Vector2 dressPos = position + new Vector2(4, 16).RotatedBy(-tilt * 0.4f) * headScale;

            Vector2 basePos = position + new Vector2(0, 10).RotatedBy(-tilt * 0.4f);

            float tentaCount = 5;
            int segmentCount = 12;
            float tentacleRotationSwerve = NPC.velocity.Length() * 0.001f;
            float tentacleWobbleFactor = 1f + tentacleRotationSwerve * 24f;

            for (int j = 0; j < tentaCount; j++) {
                float rot = 0.2f - (j / tentaCount) * 0.5f + MathHelper.PiOver2 - tilt * 0.2f;
                Vector2 pos = basePos + new Vector2(0, 20).RotatedBy(MathHelper.Lerp(0.5f, -1.3f, j / tentaCount));
                Vector2 stick = (rot.ToRotationVector2() * 12 - tentacleVel * 0.01f) * (0.5f + headScale * 0.5f);
                int segments = segmentCount - Math.Clamp(Math.Abs(j - (int)(tentaCount / 2f)), 1, 2);
                Vector2 lastPos = pos;
                float freq = 5f;
                for (int i = 0; i < segments; i++) {
                    float prog = i / (float)segments;
                    Rectangle frame = tentacleTexture.Frame(1, 10, 0, Math.Clamp((int)(prog * 8f) + 1, 2, 9));
                    if (i == 0) {
                        frame = tentacleTexture.Frame(1, 5, 0, 0);
                    }

                    if (i >= segments - 1) {
                        frame = tentacleTexture.Frame(1, 5, 0, 4);
                    }

                    float tentacleLerp = Utils.GetLerpValue(tentaCount * 0.5f, tentaCount, j - Math.Abs(tentacleVel.X) * 0.001f * (1f - prog)) * (i / (float)segments);
                    float newRot = Math.Clamp(tentacleVel.X * 0.01f, -1, 1) * prog - Math.Clamp(tentacleVel.Y * 0.015f, -0.6f, 1f) * tentacleLerp - i * (extraTentacleSwerve + tentacleRotationSwerve);
                    float tentacleWobble = (float)Math.Sin((NPC.localAI[0] * 0.06f - prog * freq - (j / tentaCount) * 0.5f) % MathHelper.TwoPi) * (0.66f - tentacleVel.Length() * 0.1f) * tentacleWobbleFactor;
                    Vector2 nextStick = stick.RotatedBy(newRot + tentacleWobble * tentacleLerp);
                    Vector2 stretch = new Vector2(0.6f + prog * 0.7f, lastPos.Distance(lastPos + nextStick) / (frame.Height - 4)) * MathHelper.Lerp(headScale, 1f, i / (float)segments);
                    if (i == 0 || i >= segments - 1) {
                        stretch = Vector2.One;
                    }

                    float stickRot = lastPos.AngleTo(lastPos + nextStick);
                    lastPos += nextStick;
                    Color tentaColor = Color.Lerp(Color.Black * (color.A / 255), color, (float)Math.Sqrt(prog));
                    spriteBatch.Draw(tentacleTexture, lastPos, frame, tentaColor, stickRot - MathHelper.PiOver2, frame.Size() * new Vector2(0.5f, 0f), stretch, 0, 0);
                }
            }

            spriteBatch.Draw(dressTexture, dressPos, null, color, tilt * 0.1f + (float)Math.Sin(NPC.localAI[0] * 0.35f % MathHelper.TwoPi) * 0.02f, dressTexture.Size() * new Vector2(0.5f, 0f), headScale, 0, 0);

            int identifier = NPC.IsABestiaryIconDummy ? -1 : NPC.whoAmI;
            cordContent.Request(1024, 1024, identifier, spriteBatch => {
                List<Vector2> positions = new List<Vector2>();
                List<float> rotations = new List<float>();

                float partitions = 50;
                for (int i = 0; i < partitions; i++) {
                    Vector2 offset = new Vector2(20 + Utils.GetLerpValue(0, partitions / 1.8f, i, true) * Utils.GetLerpValue(partitions * 1.4f, partitions / 3f, i, true) * (150 + (float)Math.Sin((NPC.localAI[0] * 0.1f - i / (partitions * 0.036f)) % MathHelper.TwoPi) * 20 * (i / partitions)), 0).RotatedBy(MathHelper.SmoothStep(0.15f, -3.2f, i / partitions));
                    offset.X *= -1;
                    offset += velocity.RotatedBy(-NPC.rotation) * Utils.GetLerpValue(partitions / 3f, partitions, i, true);
                    offset *= 0.5f;
                    positions.Add(new Vector2(512, 520) + offset);
                    rotations.Add(offset.ToRotation() - MathHelper.PiOver2 * (i / partitions) * -1);
                }

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);

                Effect effect = AssetDirectory.Effects.GoozmaCordMap.Value;
                effect.Parameters["uTransformMatrix"].SetValue(Matrix.Invert(Main.GameViewMatrix.EffectMatrix) * Matrix.CreateOrthographicOffCenter(0, 1024, 1024, 0, -1, 1));
                effect.Parameters["uTime"].SetValue((NPC.IsABestiaryIconDummy ? Main.GlobalTimeWrappedHourly * 0.3f : NPC.localAI[0] * 0.005f) % 1f);
                effect.Parameters["uTexture"].SetValue(AssetDirectory.Textures.Goozma.LiquidTrail.Value);
                effect.Parameters["uMap"].SetValue(AssetDirectory.Textures.ColorMap[0].Value);
                effect.CurrentTechnique.Passes[0].Apply();

                cordStrip ??= new VertexStrip();

                Color ColorFunc(float progress) => Color.White;
                float WidthFunc(float progress) => MathF.Pow(Utils.GetLerpValue(1.1f, 0.3f, progress, true), 0.6f) * Utils.GetLerpValue(-0.1f, 0.1f, progress, true) * 18f;

                cordStrip.PrepareStrip(positions.ToArray(), rotations.ToArray(), ColorFunc, WidthFunc, Vector2.Zero, positions.Count, true);
                cordStrip.DrawTrail();
                Main.pixelShader.CurrentTechnique.Passes[0].Apply();

                spriteBatch.End();
            });

            if (cordContent.IsTargetReady(identifier)) {
                Texture2D cordTexture = cordContent.GetTarget(identifier);
                spriteBatch.Draw(cordTexture, position, null, color, 0, cordTexture.Size() * 0.5f, 2f, 0, 0);
            }

            spriteBatch.Draw(texture, position, null, color, tilt * 0.9f, texture.Size() * 0.5f, headScale, 0, 0);

            if (!IsXmas) {
                spriteBatch.Draw(crownTexture, crownPos, null, color, tilt, crownTexture.Size() * new Vector2(0.5f, 1f), 1f, 0, 0);
            }

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            Vector2 eyeOffset = new Vector2(15, -22).RotatedBy(tilt * 0.9) * headScale;

            if (IsXmas) {
                Texture2D santaHat = AssetDirectory.Textures.SantaHat.Value;
                spriteBatch.Draw(santaHat, crownPos, null, Color.White, tilt - 0.66f, santaHat.Size() * new Vector2(0.25f, 0.46f), 0.66f, SpriteEffects.FlipHorizontally, 0);
            }
            else {
                spriteBatch.Draw(crownMaskTexture, crownPos, crownMaskTexture.Frame(), Color.White, tilt, crownMaskTexture.Size() * new Vector2(0.5f, 1f), 1f, 0, 0);
            }

            sb.End();
        });
    }
}
