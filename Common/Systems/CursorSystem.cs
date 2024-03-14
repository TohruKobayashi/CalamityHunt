using System;
using CalamityHunt.Common.Graphics.RenderTargets;
using CalamityHunt.Common.Players;
using CalamityHunt.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems;

public class CursorSystem : ModSystem
{
    public static Vector2 OldMouseScreen { get; private set; }

    public override void Load()
    {
        if (!Main.dedServ) {
            On_Main.DrawCursor += DrawCursorAdditions;
            Main.ContentThatNeedsRenderTargets.Add(rainbowTendrilContent = new RainbowTendrilContent());
        }
    }

    public override void PostUpdateEverything()
    {
        Player player = Main.LocalPlayer;
        ropes ??= new Rope[1];

        if (player != null) {
            int ropeCount = player.GetModPlayer<VanityPlayer>().tendrilCount;

            Vector2 mouseCenter = Vector2.Lerp(Main.MouseScreen, OldMouseScreen, 0.66f) / 2f + new Vector2(3f);

            mouseCenter *= Main.GameZoomTarget;
            mouseCenter -= Main.ScreenSize.ToVector2() / 4f * (Main.GameZoomTarget - 1f);

            mouseCenter /= Main.UIScale;

            if (player.GetModPlayer<VanityPlayer>().tendrilCursor) {

                if (ropes.Length != ropeCount) {
                    ropes = new Rope[ropeCount];
                }

                for (int i = 1; i < ropeCount; i++) {

                    ropes[i] ??= new Rope(mouseCenter, (int)(25 + Main.rand.NextFloat(20f)), 1.5f, Vector2.UnitY * 0.1f, 0.05f, 10);
                    ropes[i].StartPos = mouseCenter;
                    float spread = ropeCount - i * 2;
                    ropes[i].gravity = Vector2.UnitY.RotatedBy(spread * 0.1f + MathF.Sin((Main.GlobalTimeWrappedHourly * 3f + i) % MathHelper.TwoPi) * 0.4f) * 0.15f + (Main.MouseScreen - OldMouseScreen).RotatedBy(spread * 0.1f) * 0.01f;
                    ropes[i].Update();
                }
            }
            else {
                ropes = new Rope[1];
            }
        }

        OldMouseScreen = Main.MouseScreen;
    }

    private void DrawCursorAdditions(On_Main.orig_DrawCursor orig, Vector2 bonus, bool smart)
    {
        if (!Main.gameMenu && !Main.gamePaused) {
            DrawRainbowTendrils();
        }

        orig(bonus, smart);
    }

    private static RainbowTendrilContent rainbowTendrilContent;

    public static Rope[] ropes;

    public void DrawRainbowTendrils()
    {
        Player player = Main.LocalPlayer;
        if (player != null) {

            int dye = player.GetModPlayer<VanityPlayer>().cTendril;
            if (player.GetModPlayer<VanityPlayer>().tendrilCursor) {

                rainbowTendrilContent.width = Main.screenWidth;
                rainbowTendrilContent.height = Main.screenHeight;
                rainbowTendrilContent.Request();
                if (rainbowTendrilContent.IsReady) {
                    Texture2D screen = rainbowTendrilContent.GetTarget();

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);

                    DrawData drawData = new DrawData(screen, Vector2.Zero, screen.Frame(), Color.White, 0, Vector2.Zero, 2f, 0, 0);
                    GameShaders.Armor.Apply(dye, null, drawData);
                    drawData.Draw(Main.spriteBatch);
                    Main.pixelShader.CurrentTechnique.Passes[0].Apply();

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);

                }
            }
        }
    }
}
