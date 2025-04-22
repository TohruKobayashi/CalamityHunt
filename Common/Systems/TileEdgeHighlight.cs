using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems;

public class TileEdgeHighlight : ModSystem
{
    private float _fade;
    public bool Active { get; set; }

    public RenderTarget2D tileTarget;
    public RenderTarget2D subtractionTarget;

    public override void Load()
    {
        Main.OnRenderTargetsInitialized += InitTarget;
        Main.OnRenderTargetsReleased += ClearTarget;
        Main.targetSet = false;

        On_Main.UpdateAtmosphereTransparencyToSkyColor += CombineTileTargets;
        On_Main.DrawPlayers_AfterProjectiles += DrawHighlight;
    }

    private void DrawHighlight(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
    {
        orig(self);

        if (tileTarget == null || _fade <= 0.01f)
            return;

        Effect edgeEffect = AssetDirectory.Effects.TileEdgeHighlight.Value;
        Vector2 size = tileTarget.Size();
        edgeEffect.Parameters["uImageSize"].SetValue(size);
        edgeEffect.Parameters["uColor"].SetValue(new Vector4(1f, 0.23f, 0f, 1f) * _fade);
        edgeEffect.Parameters["uSecondaryColor"].SetValue(new Vector4(0.1f, 0.5f, 0.8f, 0.5f) * _fade);

        edgeEffect.Parameters["uNoise"].SetValue(AssetDirectory.Textures.Noise[11].Value);
        edgeEffect.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly / 30f % 1);
        edgeEffect.Parameters["uDistortionStrength"].SetValue(30f);
        edgeEffect.Parameters["uCenter"].SetValue((Main.LocalPlayer.Center - Main.screenPosition) / size);
        edgeEffect.Parameters["uScreenPosition"].SetValue(Main.screenPosition / size);
        edgeEffect.Parameters["uDistance"].SetValue(0.2f);

        //Main.spriteBatch.End();
        //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);        
        //Main.spriteBatch.Draw(TextureAssets.BlackTile.Value, Vector2.Zero, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), (Color.MidnightBlue * 0.15f) with { A = 255 });
        
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, edgeEffect, Main.Transform);
        Main.spriteBatch.Draw(tileTarget, Vector2.Zero, Color.White);
        Main.spriteBatch.End();
        //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
    }

    private void CombineTileTargets(On_Main.orig_UpdateAtmosphereTransparencyToSkyColor orig)
    {
        orig();

        if (tileTarget == null)
            return;

        Main.instance.GraphicsDevice.SetRenderTarget(tileTarget);
        Main.instance.GraphicsDevice.Clear(Color.Transparent);

        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);
        Main.spriteBatch.Draw(Main.instance.tileTarget, Main.sceneTilePos - Main.screenPosition, Color.White);
        Main.spriteBatch.End();

        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);
        foreach (Player p in Main.ActivePlayers)
            Main.PlayerRenderer.DrawPlayer(Main.Camera, p, p.VisualPosition, p.fullRotation, p.fullRotationOrigin, 0f, 1f);

        Main.spriteBatch.End();

        // Pseudo DrawBlack
        Main.tileBatch.Begin();

        int X1 = (int)(Main.screenPosition.X / 16) - 2;
        int X2 = (int)((Main.screenPosition.X + Main.screenWidth) / 16) + 2;
        int Y1 = (int)(Main.screenPosition.Y / 16) - 2;
        int Y2 = (int)((Main.screenPosition.Y + Main.screenHeight) / 16) + 2;

        int avgColor = (Main.tileColor.R + Main.tileColor.G + Main.tileColor.B) / 3;
        float brightness = (float)(avgColor * 0.38) / 255f;
 
        // TODO: If it's still a little laggy, we can try swapping the X and Y
        //       loops for better cache locality.
        for (int i = X1; i < X2; i++)
        {
            int rectHeight = 0;
            for (int j = Y1; j < Y2; j++)
            {
                if (!WorldGen.InWorld(i, j)) {
                    continue;
                }

                if (Main.tileSolid[Main.tile[i, j].TileType] && j < Y2 - 1 && Lighting.Brightness(i, j) < brightness && WorldGen.SolidTile(i, j)) {
                    rectHeight++;
                }
                else
                {
                    Main.tileBatch.Draw(TextureAssets.BlackTile.Value, new Vector4(i * 16 - Main.screenPosition.X, (j - rectHeight) * 16 - Main.screenPosition.Y, 16, rectHeight * 16), new VertexColors(Color.White));
                    rectHeight = 0;
                }
            }
        }

        Main.tileBatch.End();
         
        Main.instance.GraphicsDevice.SetRenderTarget(null);
        Main.instance.GraphicsDevice.Clear(Color.Transparent);
    }

    public override void PostUpdateDusts()
    {
        int geliath = NPC.FindFirstNPC(ModContent.NPCType<StellarGeliath>());

        if (geliath != -1) {

            StellarGeliath geliathMod = Main.npc[geliath].ModNPC as StellarGeliath;

            if (geliathMod.Attack == 2 && geliathMod.Time < 570 && geliathMod.Time > 50) {
                Active = true;
            }
            else
                Active = false;
        }
        else
            Active = false;

        if (Active)
            _fade = Math.Min(_fade + 0.05f, 1f);
        else
            _fade = Math.Max(_fade - 0.1f, 0f);
    }

    private void InitTarget(int width, int height)
    {
        tileTarget = new RenderTarget2D(Main.instance.GraphicsDevice, width, height);
        subtractionTarget = new RenderTarget2D(Main.instance.GraphicsDevice, width, height);
    }

    private void ClearTarget()
    {
        try
        {
            tileTarget?.Dispose();
            subtractionTarget?.Dispose();
        }
        catch
        {
            Utils.LogAndChatAndConsoleInfoMessage("Tile highlighter failed to dispose.");
        }

        tileTarget = null;
        subtractionTarget = null;
    }
}
