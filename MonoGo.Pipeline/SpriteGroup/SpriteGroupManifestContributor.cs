using MonoGame.AssetService;
using MonoGame.AssetService.Builder;
using MonoGame.Framework.Content.Pipeline.Builder;

namespace MonoGo.Pipeline.SpriteGroup
{
    public sealed class SpriteGroupManifestContributor : IAssetManifestContributor
    {
        public bool CanHandle(ContentInfo contentInfo, string relativePath)
        {
            return contentInfo.Importer is SpriteGroupImporter
                && contentInfo.Processor is SpriteGroupProcessor;
        }

        public IEnumerable<AssetManifestEntry> CreateEntries(
            AssetManifestBuildContext context,
            string relativePath,
            string fullSourcePath,
            ContentInfo contentInfo)
        {
            string sourceWithoutExtension = AssetManifestPath.RemoveExtension(relativePath);
            string containerPath = AssetManifestPath.RemoveExtension(contentInfo.GetOutputPath(sourceWithoutExtension));
            string containerQualifiedAlias = AssetManifestAliases.CreateContainerQualifiedAlias(containerPath);

            yield return new AssetManifestEntry
            {
                AssetPath = containerPath,
                Kind = AssetManifestKinds.Container,
                Alias = containerQualifiedAlias,
                QualifiedAlias = containerQualifiedAlias,
                SourcePath = relativePath,
                OutputPath = containerPath,
            };

            IReadOnlyList<string> itemKeys = SpriteGroupItemEnumerator.Enumerate(fullSourcePath);
            var itemAliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string itemKey in itemKeys)
            {
                string itemAlias = Path.GetFileName(itemKey);
                if (!itemAliases.Add(itemAlias))
                {
                    throw new InvalidOperationException($"Sprite group '{relativePath}' contains duplicate logical item alias '{itemAlias}'. Item aliases are derived from file names and must be unique within one container.");
                }

                yield return new AssetManifestEntry
                {
                    AssetPath = containerPath + "/" + itemKey,
                    Kind = AssetManifestKinds.Item,
                    Alias = itemAlias,
                    QualifiedAlias = AssetManifestAliases.CreateItemQualifiedAlias(containerPath, itemAlias),
                    ContainerPath = containerPath,
                    ItemKey = itemKey,
                    SourcePath = relativePath,
                    OutputPath = containerPath,
                };
            }
        }
    }
}
