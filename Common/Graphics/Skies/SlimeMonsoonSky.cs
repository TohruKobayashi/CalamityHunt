using System;
using CalamityHunt.Common.Systems;
using CalamityHunt.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Utilities;

namespace CalamityHunt.Common.Graphics.Skies;

public class SlimeMonsoonSky : CustomSky
{
    public override void Activate(Vector2 position, params object[] args)
    {
        _active = true;
        radialDistortPos = position;
        strengthTarget = 1f;
        additionalLightningChance = 0;
        lightningEnabled = true;
    }

    public override void Deactivate(params object[] args)
    {
        _active = false;
        strengthTarget = 0;
        additionalLightningChance = 0;
    }

    public override bool IsActive() => _strength > 0.01f && !Main.gameMenu;

    public override void Reset()
    {
        _active = false;
        strengthTarget = 0;
        additionalLightningChance = 0;
    }

    private static UnifiedRandom _random = new UnifiedRandom();
    private bool _active;
    private float _strength;
    private float _windSpeed;
    private float _brightness;
    public static Vector2 radialDistortPos;
    public static float strengthTarget;
    public static int additionalLightningChance;
    public static bool lightningEnabled;
    public static Color lightColor;
    public static float DistortionStrength => Config.Instance.monsoonDistortion;

    public static float? forceStrength;

    public override Color OnTileColor(Color inColor)
    {
        if (inColor.R + inColor.G + inColor.B > 20) {
            float fastStrength = Math.Clamp(_strength * 3f, 0, 1f);
            Main.ColorOfTheSkies = Color.Lerp(Main.ColorOfTheSkies, (lightColor * 0.4f) with { A = 255 }, fastStrength);
            return inColor.MultiplyRGBA(Color.Lerp(Color.White, lightColor, fastStrength));
        }
        return inColor;
    }

    public override float GetCloudAlpha() => 1f - _strength;

    public override void Update(GameTime gameTime)
    {
        if (_active) {
            _strength = Math.Min(strengthTarget, _strength + 0.005f);
        }
        else {
            _strength = Math.Max(0f, _strength - 0.005f);
        }

        if (forceStrength.HasValue) {
            _strength = forceStrength.Value;
            forceStrength = null;
        }

        radialDistortPos = Vector2.Lerp(radialDistortPos, Main.screenPosition + Main.ScreenSize.ToVector2() / 2f, 0.5f);

        _brightness = MathHelper.Lerp(_brightness, 0.15f, 0.08f);
        _windSpeed -= Main.WindForVisuals * 0.0025f;
        _windSpeed = _windSpeed % 1f;
    }

    public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
    {
        SkyManager.Instance["Ambience"].Deactivate();

        float yOffPower = (float)Utils.GetLerpValue(200, Main.rockLayer - 100, Main.LocalPlayer.Center.Y / 16f, true);
        int yOffset = (int)(yOffPower * 1600f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.275f % MathHelper.TwoPi) * 100f);

        //gold: new Color(40, 22, 15);
        Color brightColor = new GradientColor(new Color[]
        {
            new Color(40, 36, 62),
            new Color(45, 40, 62),
            new Color(50, 35, 40)

        }, 2f, 2f).Value;
        Color darkColor = Color.Lerp(brightColor, Color.Black, 0.7f);
        lightColor = Color.Lerp(Color.Purple, brightColor, 0.8f);

        if (Filters.Scene["HuntOfTheOldGods:SlimeMonsoon"].Active) {
            Filters.Scene["HuntOfTheOldGods:SlimeMonsoon"].GetShader()
                .UseColor(Color.White)
                .UseTargetPosition(radialDistortPos)
                .UseProgress(Main.GlobalTimeWrappedHourly * 0.01f % 1f)
                .UseOpacity(0);//_strength * (Main.zenithWorld ? 2f : 0.1f) * Config.Instance.monsoonDistortion);
            Effect filterEffect = Filters.Scene["HuntOfTheOldGods:SlimeMonsoon"].GetShader().Shader;
            filterEffect.Parameters["uNoiseTexture0"].SetValue(AssetDirectory.Textures.Noise[11].Value);
            filterEffect.Parameters["uNoiseTexture1"].SetValue(AssetDirectory.Textures.Noise[0].Value);
            filterEffect.Parameters["distortSize"].SetValue(Vector2.One);

            if (_strength < 0.2f) {
                Filters.Scene["HuntOfTheOldGods:SlimeMonsoon"].Deactivate();
            }
        }

        if (maxDepth >= float.MaxValue && minDepth < float.MaxValue) {
            spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * (float)Math.Sqrt(_strength));
            spriteBatch.Draw(AssetDirectory.Textures.Noise[4].Value, new Rectangle(0, -yOffset - 100, Main.screenWidth, Main.screenHeight * 3), darkColor * 0.1f * _strength);
        }

        Effect skyEffect = AssetDirectory.Effects.SlimeMonsoonSkyLayer.Value;
        skyEffect.Parameters["uMap"].SetValue(AssetDirectory.Textures.ColorMap[4].Value);

        if (maxDepth >= 4 && minDepth < 5) {

            skyEffect.Parameters["uWorldPos"].SetValue((Main.screenPosition + Main.ScreenSize.ToVector2() / 2f) * 0.0001f);
            skyEffect.Parameters["uTime"].SetValue(_windSpeed * 3f);
            skyEffect.Parameters["uStrength"].SetValue(_strength);
            skyEffect.Parameters["uTexture0"].SetValue(AssetDirectory.Textures.Noise[5].Value);
            skyEffect.Parameters["uTexture1"].SetValue(AssetDirectory.Textures.Noise[0].Value);
            skyEffect.Parameters["uHeight"].SetValue(0f);
            skyEffect.Parameters["uBrightness"].SetValue(0.5f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, skyEffect, Main.BackgroundViewMatrix.TransformationMatrix);

            spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), darkColor * (float)Math.Sqrt(_strength));

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.BackgroundViewMatrix.TransformationMatrix);
        }

        if (maxDepth >= 2 && minDepth < 3) {

            skyEffect.Parameters["uWorldPos"].SetValue((Main.screenPosition + Main.ScreenSize.ToVector2() / 2f) * 0.0001f);
            skyEffect.Parameters["uTime"].SetValue(_windSpeed * 3f);
            skyEffect.Parameters["uStrength"].SetValue(_strength);
            skyEffect.Parameters["uTexture0"].SetValue(AssetDirectory.Textures.Noise[5].Value);
            skyEffect.Parameters["uTexture1"].SetValue(AssetDirectory.Textures.Noise[0].Value);
            skyEffect.Parameters["uHeight"].SetValue(0f);
            skyEffect.Parameters["uBrightness"].SetValue(0.5f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, skyEffect, Main.BackgroundViewMatrix.TransformationMatrix);

            spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), darkColor * (float)Math.Sqrt(_strength));

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.BackgroundViewMatrix.TransformationMatrix);
        }

        if (maxDepth >= 1 && minDepth < 2) {

        }

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.BackgroundViewMatrix.TransformationMatrix);
    }

    public class MonsoonStrike
    {
        public MonsoonStrike(int layer)
        {

        }
    }
}
