using System.Text.Json;
using System.Text.Json.Nodes;

namespace MonoGo.Pipeline.SpriteGroup
{
    public static class SpriteGroupDefinitionReader
    {
        public static SpriteGroupDefinition Read(string filename)
        {
            try
            {
                var json = File.ReadAllText(filename);
                var options = new JsonDocumentOptions()
                {
                    AllowTrailingCommas = true,
                    CommentHandling = JsonCommentHandling.Skip,
                };

                JsonNode configData = JsonNode.Parse(json, documentOptions: options)
                    ?? throw new InvalidOperationException("Sprite group config is empty.");

                string spriteGroupDirectory = Path.GetDirectoryName(filename)
                    ?? throw new InvalidOperationException("Sprite group directory could not be resolved.");

                string rootDir = configData["rootDir"]?.ToString()
                    ?? throw new InvalidOperationException("Sprite group config is missing rootDir.");

                JsonArray textureWildcards = (JsonArray?)configData["singleTexturesWildcards"]
                    ?? throw new InvalidOperationException("Sprite group config is missing singleTexturesWildcards.");

                string[] wildcards = new string[textureWildcards.Count];
                for (int i = 0; i < textureWildcards.Count; i += 1)
                {
                    wildcards[i] = textureWildcards[i]?.ToString() ?? string.Empty;
                }

                string trimmedRootDir = rootDir.TrimStart('/', '\\');
                string resolvedRootDirectory = Path.GetFullPath(Path.Combine(spriteGroupDirectory, trimmedRootDir));

                return new SpriteGroupDefinition
                {
                    AtlasSize = int.Parse(configData["atlasSize"]?.ToString() ?? throw new InvalidOperationException("Sprite group config is missing atlasSize.")),
                    TexturePadding = int.Parse(configData["texturePadding"]?.ToString() ?? throw new InvalidOperationException("Sprite group config is missing texturePadding.")),
                    GroupName = Path.GetFileNameWithoutExtension(filename),
                    RootDirectory = resolvedRootDirectory,
                    SingleTextureWildcards = wildcards,
                };
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Importing spritegroup failed! " + e.Message, e);
            }
        }
    }
}
