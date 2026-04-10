using Microsoft.Xna.Framework.Content;
using MonoGame.AssetService;
using System;
using System.Collections.Generic;

namespace MonoGo.Engine.AssetService;

/// <summary>
/// Provides MonoGo-specific manifest-driven asset loading on top of <see cref="AssetServiceBase" />.
/// </summary>
public sealed class MonoGoAssetService : AssetServiceBase
{
    private readonly SpriteGroupContainerResolver _spriteGroupContainerResolver;

    /// <summary>
    /// Initializes a new <see cref="MonoGoAssetService" /> instance.
    /// </summary>
    public MonoGoAssetService(ContentManager contentManager, IAssetManifestProvider manifestProvider, MonoGoAssetServiceOptions options)
        : base(contentManager, manifestProvider, PrepareOptions(options, out SpriteGroupContainerResolver spriteGroupContainerResolver))
    {
        _spriteGroupContainerResolver = spriteGroupContainerResolver;
    }

    /// <summary>
    /// Clears any cached MonoGo-specific container state.
    /// </summary>
    public void ClearCaches()
    {
        _spriteGroupContainerResolver.Clear();
    }

    private static AssetServiceOptions PrepareOptions(MonoGoAssetServiceOptions options, out SpriteGroupContainerResolver spriteGroupContainerResolver)
    {
        ArgumentNullException.ThrowIfNull(options);

        spriteGroupContainerResolver = new SpriteGroupContainerResolver();
        FontAssetResolver fontAssetResolver = new();

        List<IAssetResolver> assetResolvers =
        [
            fontAssetResolver,
        ];

        if (options.AssetResolvers.Count > 0)
        {
            assetResolvers.AddRange(options.AssetResolvers);
        }

        List<IContainerAssetResolver> containerResolvers =
        [
            spriteGroupContainerResolver,
        ];

        if (options.ContainerResolvers.Count > 0)
        {
            containerResolvers.AddRange(options.ContainerResolvers);
        }

        return new AssetServiceOptions
        {
            ContentRootPath = options.ContentRootPath,
            AssetResolvers = assetResolvers,
            ContainerResolvers = containerResolvers,
        };
    }
}
