using MonoGame.AssetService;
using MonoGo.Engine.Drawing;
using System;
using System.Collections.Generic;

namespace MonoGo.Engine.AssetService;

/// <summary>
/// Resolves MonoGo sprite group containers and item sprites.
/// </summary>
public sealed class SpriteGroupContainerResolver : IContainerAssetResolver
{
    private readonly Dictionary<string, IReadOnlyDictionary<string, Sprite>> _containerCache = new(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public bool CanLoadContainer(AssetManifestEntry containerEntry)
    {
        ArgumentNullException.ThrowIfNull(containerEntry);

        return string.Equals(containerEntry.Kind, AssetManifestKinds.Container, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public bool CanLoadItem<T>(AssetManifestEntry itemEntry, AssetManifestEntry containerEntry) where T : class
    {
        ArgumentNullException.ThrowIfNull(itemEntry);
        ArgumentNullException.ThrowIfNull(containerEntry);

        return typeof(T) == typeof(Sprite)
            && string.Equals(itemEntry.Kind, AssetManifestKinds.Item, StringComparison.OrdinalIgnoreCase)
            && string.Equals(containerEntry.Kind, AssetManifestKinds.Container, StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrWhiteSpace(itemEntry.ItemKey);
    }

    /// <inheritdoc />
    public object LoadContainer(AssetManifestEntry containerEntry, AssetServiceBase assetService)
    {
        ArgumentNullException.ThrowIfNull(containerEntry);
        ArgumentNullException.ThrowIfNull(assetService);

        string containerPath = GetContainerContentPath(containerEntry);
        if (_containerCache.TryGetValue(containerPath, out IReadOnlyDictionary<string, Sprite>? cachedSprites))
        {
            return cachedSprites;
        }

        IReadOnlyDictionary<string, Sprite> sprites = assetService.LoadBuiltAsset<Dictionary<string, Sprite>>(containerEntry);
        _containerCache[containerPath] = sprites;
        return sprites;
    }

    /// <inheritdoc />
    public T LoadItem<T>(AssetManifestEntry itemEntry, AssetManifestEntry containerEntry, object container, AssetServiceBase assetService) where T : class
    {
        ArgumentNullException.ThrowIfNull(itemEntry);
        ArgumentNullException.ThrowIfNull(containerEntry);
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(assetService);

        if (container is not IReadOnlyDictionary<string, Sprite> sprites)
        {
            throw new InvalidCastException($"Container asset '{containerEntry.AssetPath}' is not a readable sprite dictionary.");
        }

        if (string.IsNullOrWhiteSpace(itemEntry.ItemKey))
        {
            throw new InvalidOperationException($"Item asset '{itemEntry.AssetPath}' does not define an item key.");
        }

        if (!sprites.TryGetValue(itemEntry.ItemKey, out Sprite? sprite) || sprite == null)
        {
            throw new KeyNotFoundException($"Sprite group item '{itemEntry.ItemKey}' was not found in container '{containerEntry.AssetPath}'.");
        }

        sprite.Name = itemEntry.AssetPath;
        return (T)(object)sprite;
    }

    /// <summary>
    /// Clears cached sprite group containers.
    /// </summary>
    public void Clear()
    {
        _containerCache.Clear();
    }

    private static string GetContainerContentPath(AssetManifestEntry containerEntry)
    {
        return !string.IsNullOrWhiteSpace(containerEntry.OutputPath)
            ? containerEntry.OutputPath
            : containerEntry.AssetPath;
    }
}
