using System.Linq;
using CalamityHunt.Common.Systems.Metaballs;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Graphics.RenderTargets;

public class CosmosMetaball : MetaballDrawer
{
    public static ParticleRenderer Particles { get; } = ParticleRenderer.MakeDefaultRenderer();

    public static Effect MetaballShader;

    public override void Initialize()
    {
        MetaballShader = AssetDirectory.Effects.FlameDissolveMetaball.Value;
        Particles.Effect = MetaballShader;

        content.width = Main.screenWidth;
        content.height = Main.screenHeight;
        content.draw = spriteBatch => {

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.EffectMatrix);
            foreach (Projectile projectile in Main.projectile.Where(n => n.active && n.type == ModContent.ProjectileType<StellarBlackHole>())) {

                float width = MathHelper.SmoothStep(0f, 1f, Utils.GetLerpValue(40, 110, projectile.ai[0], true) * Utils.GetLerpValue(projectile.ai[1] * 0.95f, projectile.ai[1] * 0.8f, projectile.ai[0], true));
                Vector2 position = Vector2.Lerp(projectile.Center - Main.screenPosition, new Vector2(Main.screenWidth / 2f, 0), width);
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, position, new Rectangle(0, 0, 2, 2), Color.White, 0, Vector2.One, new Vector2(width * (Main.screenWidth + 50), Main.screenHeight + 50), 0, 0);
            }
            spriteBatch.End();

            MetaballShader.Parameters["uTexture0"].SetValue(AssetDirectory.Textures.Noise[9].Value);
            MetaballShader.Parameters["uFrameCount"].SetValue(5);
            Particles.Draw(spriteBatch);
        };

        On_Main.UpdateParticleSystems += UpdateCosmosParticleSystem;
        On_Main.DoDraw_DrawNPCsOverTiles += DrawTarget;
    }

    private void UpdateCosmosParticleSystem(On_Main.orig_UpdateParticleSystems orig, Main self)
    {
        orig(self);
        Particles.Update();
    }

    private void DrawTarget(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
    {
        Effect effect = AssetDirectory.Effects.Cosmos.Value;
        effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.05f);
        effect.Parameters["uZoom"].SetValue(0.5f + Main.GameZoomTarget * 0.5f);
        effect.Parameters["uTextureNoise"].SetValue(AssetDirectory.Textures.Noise[10].Value);
        effect.Parameters["uTextureClose"].SetValue(AssetDirectory.Textures.Space[0].Value);
        effect.Parameters["uTextureFar"].SetValue(AssetDirectory.Textures.Space[1].Value);
        effect.Parameters["uPosition"].SetValue(Main.screenPosition / 256f);
        effect.Parameters["uScrollClose"].SetValue(new Vector2(-Main.GlobalTimeWrappedHourly * 0.027f % 2f, -(Main.GlobalTimeWrappedHourly * 0.017f % 2f)));
        effect.Parameters["uScrollFar"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * 0.008f % 2f, -(Main.GlobalTimeWrappedHourly * 0.0004f % 2f)));
        effect.Parameters["uCloseColor"].SetValue(Color.Lerp(Color.CornflowerBlue, Color.RoyalBlue, 0.1f).ToVector3() * 1.2f);
        effect.Parameters["uFarColor"].SetValue((Color.Blue * 0.2f).ToVector3());
        effect.Parameters["uOutlineColor"].SetValue(new Color(0, 10, 100, 20).ToVector4());
        effect.Parameters["uImageSize"].SetValue(Main.ScreenSize.ToVector2());
        effect.Parameters["uNoiseRepeats"].SetValue(0.1f);

        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.GameViewMatrix.EffectMatrix);

        content.width = Main.screenWidth;
        content.height = Main.screenHeight;
        content.Request();

        if (content.IsReady) {
            Texture2D texture = content.GetTarget();
            Main.spriteBatch.Draw(texture, Vector2.Zero, texture.Frame(), Color.White, 0, Vector2.Zero, 1f, 0, 0);
        }

        Main.spriteBatch.End();

        orig(self);
    }
}
