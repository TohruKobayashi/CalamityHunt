using System.Collections.Generic;
using ReLogic.Content;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Utilities;

/// <summary>
///     Various asset-loading- and asset-manipulation-related utilities.
/// </summary>
public static class AssetUtilities
{
    // These are essentially just calls to ModContent.Request, but they were
    // originally made for a more sophisticated asset loading system. I'd like
    // to keep these methods in use in the event a use arises for them again.
    // - Tomat

    /// <summary>
    ///     Makes a cacheable request for an asset.
    /// </summary>
    /// <param name="path">The path to the asset.</param>
    /// <typeparam name="T">The asset type.</typeparam>
    /// <returns>The cached asset pointer.</returns>
    public static Asset<T> RequestImmediate<T>(string path) where T : class => Request<T>(path, AssetRequestMode.ImmediateLoad);

    public static Asset<T>[] RequestArrayImmediate<T>(string path, int count, int start = 0) where T : class
    {
        Asset<T>[] assets = new Asset<T>[count];
        for (int i = 0; i < assets.Length; i++) {
            assets[i] = RequestImmediate<T>(path + (i + start));
        }

        return assets;
    }

    public static Asset<T>[] RequestArrayTotalImmediate<T>(string name) where T : class
    {
        List<Asset<T>> assets = new List<Asset<T>>();

        int i = 0;
        while (ModContent.RequestIfExists(name + i, out Asset<T> asset, AssetRequestMode.ImmediateLoad)) {
            assets.Add(asset);
            i++;
        }

        return assets.ToArray();
    }

    private static Asset<T> Request<T>(string path, AssetRequestMode requestMode) where T : class
    {
        /*// In the event that this system for some reason has not yet loaded, we
        // can simply handle it like this.
        if (ModContent.GetInstance<AssetUtilitiesSystem>() is { } system)
            return system.GetAssetManager<T>().GetOrRequestAsset(path, requestMode);*/

        return ModContent.Request<T>(path, requestMode);
    }
}
