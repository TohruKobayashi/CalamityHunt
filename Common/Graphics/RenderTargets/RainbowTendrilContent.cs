using CalamityHunt.Common.Players;
using CalamityHunt.Common.Systems;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace CalamityHunt.Common.Graphics.RenderTargets;

public class RainbowTendrilContent : ARenderTargetContentByRequest
{
    public int width;
    public int height;

    protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
    {
        PrepareARenderTarget_AndListenToEvents(ref _target, device, width, height, RenderTargetUsage.PreserveContents);
        device.SetRenderTarget(_target);
        device.Clear(Color.Transparent);

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);

        Texture2D tentacleTexture = AssetDirectory.Textures.Goozma.MicroTentacle.Value;

        if (CursorSystem.ropes != null) {
            foreach (Rope rope in CursorSystem.ropes) {
                if (rope != null) {
                    Vector2[] points = rope.GetPoints().ToArray();
                    for (int j = 1; j < points.Length - 1; j++) {
                        Vector2 scale = new Vector2(0.66f, points[j].Distance(points[j - 1]) / 13.98f);

                        Rectangle frame = tentacleTexture.Frame(1, 2, 0, 0);
                        if (j == points.Length - 2) {
                            frame = tentacleTexture.Frame(1, 2, 0, 1);
                            scale.Y = 1f;
                        }
                        frame.Height -= 2;
                        Color color = (new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Main.GlobalTimeWrappedHourly * 40 - j)) with { A = 255 };
                        if (Main.LocalPlayer.GetModPlayer<VanityPlayer>().cTendril > 0) {
                            color = Color.Lerp(color, Color.White, 0.7f);
                        }
                        float rotation = points[j].AngleTo(points[j + 1]);
                        spriteBatch.Draw(tentacleTexture, points[j], frame, color, rotation - MathHelper.PiOver2, frame.Size() * new Vector2(0.5f, 0f), scale, 0, 0);
                    }
                }
            }
        }

        spriteBatch.End();

        device.SetRenderTarget(null);
        _wasPrepared = true;
    }
}
