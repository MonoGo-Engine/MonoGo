using Microsoft.Xna.Framework.Graphics;
using MonoGame.AssetService;
using MonoGo.Engine.Drawing;
using System;

namespace MonoGo.Engine.AssetService;

/// <summary>
/// Resolves MonoGo <see cref="IFont" /> assets from built <see cref="SpriteFont" /> content.
/// </summary>
public sealed class FontAssetResolver : IAssetResolver
{
    /// <inheritdoc />
    public bool CanLoad<T>(AssetManifestEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        return typeof(T) == typeof(IFont)
            && string.Equals(entry.Kind, AssetManifestKinds.BuiltContent, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public T Load<T>(AssetManifestEntry entry, AssetServiceBase assetService) where T : class
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(assetService);

        SpriteFont spriteFont = assetService.LoadBuiltAsset<SpriteFont>(entry);
        IFont font = new Font(spriteFont);
        return (T)font;
    }
}
