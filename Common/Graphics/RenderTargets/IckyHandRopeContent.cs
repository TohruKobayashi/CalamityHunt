using System;
using System.Collections.Generic;

using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using CalamityHunt.Content.Projectiles;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace CalamityHunt.Common.Graphics.RenderTargets;

public class IckyHandRopeContent : ARenderTargetContentByRequest
{
    public int width;
    public int height;

    protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
    {
        PrepareARenderTarget_AndListenToEvents(ref _target, device, width, height, RenderTargetUsage.PreserveContents);
        device.SetRenderTarget(_target);
        device.Clear(Color.Transparent);

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
        foreach (var projectile in Main.ActiveProjectiles) {
            if (projectile.ModProjectile is not StickyHandProj stickyHandProj) {
                continue;
            }

            Rope rope = stickyHandProj.rope;
            Player player = Main.player[projectile.owner];

            if (rope != null) {
                List<Vector2> points = rope.GetPoints();
                points.Add(projectile.Center);
                BezierCurve curve = new BezierCurve(points);
                int pointCount = 50;
                points = curve.GetPoints(pointCount);
                points.Add(projectile.Center);

                Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
                Rectangle frame = texture.Frame(1, 2, 0, projectile.frame);
                SpriteEffects effects = projectile.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                Rectangle chainFrame = StickyHandProj.chainTexture.Value.Frame();

                Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.5f, 0.5f).ValueAt(Main.GlobalTimeWrappedHourly * 120);

                for (int i = 0; i < points.Count - 1; i++) {
                    float rotation = points[i].AngleTo(points[i + 1]);
                    float thinning = 1f - MathF.Sin((float)i / points.Count * MathHelper.Pi) * 0.6f * Utils.GetLerpValue(0, 400, projectile.Distance(player.MountedCenter) * 0.9f, true);
                    Vector2 stretch = new Vector2(projectile.scale * thinning, points[i].Distance(points[i + 1]) / (StickyHandProj.chainTexture.Height() - 4));
                    Color chainGlowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.5f, 0.5f).ValueAt((1f - (float)(i + 3) / points.Count) * 70 + Main.GlobalTimeWrappedHourly * 120);

                    DrawData drawData = new DrawData(StickyHandProj.chainTexture.Value, points[i] - Main.screenPosition, chainFrame, chainGlowColor, rotation + MathHelper.PiOver2, chainFrame.Size() * new Vector2(0.5f, 1f), stretch, 0, 0);
                    drawData.shader = player.cGrapple;
                    Main.EntitySpriteDraw(drawData);
                }

                DrawData drawData2 = new DrawData(texture, projectile.Center - Main.screenPosition, frame, glowColor, points[pointCount - 1].AngleTo(projectile.Center) + MathHelper.PiOver2, frame.Size() * 0.5f, projectile.scale, effects, 0);
                drawData2.shader = player.cGrapple;

                Main.EntitySpriteDraw(drawData2);
            }
        }
        spriteBatch.End();

        device.SetRenderTarget(null);
        _wasPrepared = true;
    }
}
