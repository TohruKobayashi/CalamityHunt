using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using CalamityHunt.Content.Bosses.Goozma;
using Microsoft.Xna.Framework;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Terraria.Graphics;

namespace CalamityHunt.Content.Projectiles.Weapons.Summoner;

public class GoozmoemTextureContent : ARenderTargetContentByRequest
{
    public int width;
    public int height;
    public Action drawFunction;
    public Action drawNonGlowFunction;

    protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
    {
        Texture2D asset = AssetDirectory.Textures.Extras.PaladinPalanquinBall.Value;
        PrepareARenderTarget_AndListenToEvents(ref _target, device, width, height, RenderTargetUsage.PreserveContents);
        device.SetRenderTarget(_target);
        device.Clear(Color.Transparent);

        GetGradientMapValues(out float[] brightnesses, out Vector3[] colors);
        Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/HolographEffect", AssetRequestMode.ImmediateLoad).Value;
        effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly % 1f);
        effect.Parameters["colors"].SetValue(colors);
        effect.Parameters["brightnesses"].SetValue(brightnesses);
        effect.Parameters["baseToScreenPercent"].SetValue(1.05f);
        effect.Parameters["baseToMapPercent"].SetValue(-0.05f);

        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect);
        drawFunction.Invoke();
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);
        drawNonGlowFunction.Invoke();
        spriteBatch.End();

        device.SetRenderTarget(null);
        _wasPrepared = true;
    }
    private void GetGradientMapValues(out float[] brightnesses, out Vector3[] colors)
    {
        brightnesses = new float[10];
        colors = new Vector3[10];

        float maxBright = 0.667f;
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
        for (int i = 1; i < 10; i++)
            brightnesses[i] = HuntOfTheOldGodUtils.Modulo(brightnesses[i], maxBright) * maxBright;

        //Store the first element's value so we can find it again later
        float firstBrightnessValue = brightnesses[1];

        //Sort the values from lowest to highest
        Array.Sort(brightnesses);

        //Find the new index of the original first element after the list being sorted
        int rainbowStartIndex = Array.IndexOf(brightnesses, firstBrightnessValue);
        //Substract 1 from the index, because we are ignoring the currently negative first array slot.
        rainbowStartIndex--;

        //9 loop, filling a list of colors in a array of 10 elements (ignoring the first one)
        for (int i = 0; i < 9; i++)
        {
            colors[1 + (rainbowStartIndex + i) % 9] = GoozmaColorUtils.Oil[i];
        }

        //We always want a brightness at index 0 to be the lower bound
        brightnesses[0] = 0;
        //Make the color at index 0 be a mix between the first and last colors in the list, based on the distance between the 2.
        float interpolant = (1 - brightnesses[9]) / (brightnesses[1] + (1 - brightnesses[9]));
        colors[0] = Vector3.Lerp(colors[9], colors[0], interpolant);
    }
}

public class GoozmoemCordTextureContent : ARenderTargetContentByRequest
{
    public int width;
    public int height;
    public Vector2[] positions;

    protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
    {
        Texture2D asset = AssetDirectory.Textures.Extras.PaladinPalanquinBall.Value;
        PrepareARenderTarget_AndListenToEvents(ref _target, device, width, height, RenderTargetUsage.PreserveContents);
        device.SetRenderTarget(_target);
        device.Clear(Color.Transparent);

        float[] rotations = new float[positions.Length];
        for (int i = 1; i < positions.Length; i++)
            rotations[i] = positions[i - 1].AngleTo(positions[i]);
        rotations[positions.Length - 1] = rotations[positions.Length - 2];

        Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/GoozmaCordMap", AssetRequestMode.ImmediateLoad).Value;
        effect.Parameters["uTransformMatrix"].SetValue(Matrix.CreateOrthographicOffCenter(0, width, height, 0, -1, 1));
        if (!Main.gameInactive)
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.5f);
        effect.Parameters["uTexture"].SetValue(AssetDirectory.Textures.Goozma.LiquidTrail.Value);
        effect.Parameters["uMap"].SetValue(AssetDirectory.Textures.Goozma.ColorMap.Value);

        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect);
        effect.CurrentTechnique.Passes[0].Apply();

        VertexStrip strip = new VertexStrip();

        Color ColorFunc(float progress) => Color.White;
        float WidthFunc(float progress) => MathF.Pow(Utils.GetLerpValue(0f, 0.3f, progress, true) * Utils.GetLerpValue(1f, 0.8f, progress, true), 0.7f) * 6f;
        strip.PrepareStrip(positions, rotations, ColorFunc, WidthFunc, Vector2.Zero, positions.Length, true);
        strip.DrawTrail();

        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        spriteBatch.End();

        device.SetRenderTarget(null);
        _wasPrepared = true;
    }
}
