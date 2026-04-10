namespace MonoGo.Pipeline.SpriteGroup
{
    public sealed class SpriteGroupDefinition
    {
        public required int AtlasSize { get; init; }
        public required int TexturePadding { get; init; }
        public required string GroupName { get; init; }
        public required string RootDirectory { get; init; }
        public required string[] SingleTextureWildcards { get; init; } = Array.Empty<string>();
    }
}
