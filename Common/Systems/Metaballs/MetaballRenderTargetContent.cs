using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace CalamityHunt.Common.Systems.Metaballs;

public class MetaballRenderTargetContent : ARenderTargetContentByRequest
{
    public Action<SpriteBatch> draw;
    public int width;
    public int height;

    protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
    {
        device.SetRenderTarget(_target);
        device.Clear(Color.Transparent);

        PrepareARenderTarget_AndListenToEvents(ref _target, device, width, height, RenderTargetUsage.PreserveContents);

        draw?.Invoke(spriteBatch);

        device.SetRenderTarget(null);
        _wasPrepared = true;
    }
}
