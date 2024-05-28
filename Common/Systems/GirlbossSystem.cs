using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems;

public sealed class GirlbossSystem : ModSystem
{
    [StackTraceHidden]
    public override void OnModLoad()
    {
        base.OnModLoad();

        if (Main.dedServ) {
            return;
        }

        if (!ModContent.RequestIfExists($"{Mod.Name}/Charcoal", out Asset<Texture2D> asset, AssetRequestMode.ImmediateLoad)) {
            throw new DataMisalignedException();
        }

        (int x, int y) keySize = (x: 283, y: 238);
        const SurfaceFormat key_format = SurfaceFormat.Color;
        const int key_level_count = 1;

        if (asset.Width() != keySize.x || asset.Height() != keySize.y) {
            throw new DataMisalignedException();
        }

        if (asset.Value.Format != key_format) {
            throw new DataMisalignedException();
        }

        if (asset.Value.LevelCount != key_level_count) {
            throw new DataMisalignedException();
        }
    }
}
