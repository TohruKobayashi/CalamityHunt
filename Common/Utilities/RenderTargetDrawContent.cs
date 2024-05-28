using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace CalamityHunt.Common.Utilities;

public class RenderTargetDrawContent : INeedRenderTargetContent
{
    private class DrawContent
    {
        public bool requested;
        public int identifier;
        public int width;
        public int height;
        public Action<SpriteBatch> draw;
        public RenderTarget2D target;
    }

    private List<DrawContent> _draws;

    private bool _ready;
    private bool _requestedAny;

    public bool IsReady => _ready;

    public void Request(int width, int height, int identifier, Action<SpriteBatch> draw)
    {
        _draws ??= new List<DrawContent>();
        DrawContent newDraw = new DrawContent { requested = true, identifier = identifier, width = width, height = height, draw = draw };
        if (!_draws.Any(n => n.identifier == identifier)) {
            _draws.Add(newDraw);
        }
        else {
            DrawContent currentDraw = _draws.First(n => n.identifier == identifier);
            currentDraw.requested = true;
            currentDraw.width = width;
            currentDraw.height = height;
            currentDraw.draw = draw;
        }

        _requestedAny = true;
    }

    public bool IsTargetReady(int identifier) => _draws.Any(n => n?.identifier == identifier && n?.target != null) && IsReady;

    public RenderTarget2D GetTarget(int identifier) => _draws.First(n => n.identifier == identifier && n.target != null).target;

    public void PrepareRenderTarget(GraphicsDevice device, SpriteBatch spriteBatch)
    {
        _ready = false;

        if (_requestedAny) {
            _requestedAny = false;

            if (_draws != null) {

                for (int i = 0; i < _draws.ToList().Count; i++) {
                    if (!_draws[i].requested) {
                        _draws.Remove(_draws[i]);
                    }
                    else {
                        _draws[i].requested = false;
                    }
                }

                for (int i = 0; i < _draws.ToList().Count; i++) {
                    InitTarget(ref _draws[i].target, device, _draws[i].width, _draws[i].height, RenderTargetUsage.PreserveContents);

                    device.SetRenderTarget(_draws[i].target);
                    device.Clear(Color.Transparent);

                    _draws[i].draw.Invoke(spriteBatch);

                    device.SetRenderTarget(null);
                }
            }

            _ready = true;
        }
    }

    public void Reset()
    {
        _ready = false;
        _requestedAny = false;
        _draws = new List<DrawContent>();
    }

    protected void InitTarget(ref RenderTarget2D target, GraphicsDevice device, int width, int height, RenderTargetUsage usage)
    {
        if (target == null || target.IsDisposed || target.Width != width || target.Height != height) {
            if (target != null) {
                target.ContentLost -= target_ContentLost;
                target.Disposing -= target_Disposing;
            }

            target = new RenderTarget2D(device, width, height, mipMap: false, device.PresentationParameters.BackBufferFormat, DepthFormat.None, 0, usage);
            target.ContentLost += target_ContentLost;
            target.Disposing += target_Disposing;
        }
    }

    private void target_Disposing(object sender, EventArgs e)
    {
        _ready = false;
    }

    private void target_ContentLost(object sender, EventArgs e)
    {
        _ready = false;
    }
}
