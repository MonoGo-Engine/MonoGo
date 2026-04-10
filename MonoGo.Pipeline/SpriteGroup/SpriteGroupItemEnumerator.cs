namespace MonoGo.Pipeline.SpriteGroup
{
    public static class SpriteGroupItemEnumerator
    {
        public static IReadOnlyList<string> Enumerate(string filename)
        {
            SpriteGroupDefinition definition = SpriteGroupDefinitionReader.Read(filename);

            var itemKeys = new List<string>();
            EnumerateRecursive(definition.RootDirectory, string.Empty, itemKeys);
            itemKeys.Sort(StringComparer.OrdinalIgnoreCase);
            return itemKeys;
        }

        private static void EnumerateRecursive(string dirPath, string dirName, List<string> itemKeys)
        {
            var dirInfo = new DirectoryInfo(dirPath);
            if (!dirInfo.Exists)
            {
                return;
            }

            foreach (FileInfo file in dirInfo.GetFiles("*.png"))
            {
                itemKeys.Add(dirName + Path.GetFileNameWithoutExtension(file.Name));
            }

            foreach (DirectoryInfo dir in dirInfo.GetDirectories())
            {
                EnumerateRecursive(dir.FullName, dirName + dir.Name + '/', itemKeys);
            }
        }
    }
}
