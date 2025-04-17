using System.IO;
using SearchOption = System.IO.SearchOption;

namespace MonoGo.Engine.Resources
{
    public class FileInfoResourceBox : ResourceBox<FileInfo>
    {
        private readonly string _resourcePath;
        private readonly string _searchPattern;
        private readonly SearchOption _searchOption;

        public FileInfoResourceBox(string name, string filePath, string seachPattern = "**", SearchOption searchOption = SearchOption.AllDirectories) : base(name)
        {
            _resourcePath = filePath;
            _searchPattern = seachPattern;
            _searchOption = searchOption;
        }

        public override void Load()
        {
            if (Loaded)
            {
                return;
            }
            Loaded = true;

            var baseFilePath = Path.Combine(ResourceInfoMgr.ContentDir, _resourcePath);

            if (Directory.Exists(baseFilePath))
            {
                foreach (var filePath in Directory.GetFiles(baseFilePath, _searchPattern, _searchOption))
                {
                    try
                    {
                        AddResource(Path.GetFileNameWithoutExtension(filePath), new FileInfo(filePath));
                    }
                    catch { }
                }
            }
            else throw new IOException($"Directory does not exist: {baseFilePath}");
        }

        public override void Unload()
        {
            if (!Loaded)
            {
                return;
            }
            Loaded = false;
        }
    }
}
