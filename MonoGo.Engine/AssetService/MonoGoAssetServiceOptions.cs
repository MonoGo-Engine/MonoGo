using MonoGame.AssetService;
using System;
using System.Collections.Generic;

namespace MonoGo.Engine.AssetService;

/// <summary>
/// Configures a <see cref="MonoGoAssetService" /> instance.
/// </summary>
public sealed class MonoGoAssetServiceOptions
{
    /// <summary>
    /// Gets or sets the content root path used for copied assets.
    /// </summary>
    public required string ContentRootPath { get; init; }

    /// <summary>
    /// Gets or sets additional built asset resolvers.
    /// </summary>
    public IReadOnlyList<IAssetResolver> AssetResolvers { get; init; } = Array.Empty<IAssetResolver>();

    /// <summary>
    /// Gets or sets additional container resolvers.
    /// </summary>
    public IReadOnlyList<IContainerAssetResolver> ContainerResolvers { get; init; } = Array.Empty<IContainerAssetResolver>();
}
