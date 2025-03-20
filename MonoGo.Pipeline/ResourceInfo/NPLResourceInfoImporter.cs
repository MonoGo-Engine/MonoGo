using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;

// Inspired by:
// https://github.com/Ragath/MonoGame.AssetInfo

namespace MonoGo.Pipeline.ResourceInfo
{
    /// <summary>
    /// Asset info importer. Reads .npl file and extracts all asset paths.
    /// </summary>
    [ContentImporter(".npl", DisplayName = "NPL Resource Info Importer - MonoGo", DefaultProcessor = "PassThroughProcessor")]
    public class NPLResourceInfoImporter : ContentImporter<string[]>
    {
        public override string[] Import(string filename, ContentImporterContext context)
        {
            string jsonString;
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(fs))
            {
                jsonString = reader.ReadToEnd();
            }
            var jsonObject = JsonNode.Parse(jsonString);

            // Settings
            var contentRoot = jsonObject["root"]?.ToString() ?? "";

            // Content Items
            Dictionary<string, ContentItem> ContentList = new();
            var content = jsonObject["content"].AsObject().AsEnumerable();
            for (int i = 0; i < content.Count(); i++)
            {
                var data = content.ToArray()[i];
                var categoryObject = jsonObject["content"][data.Key];

                var nplItem = new ContentItem(data.Key);
                foreach (var categoryItem in categoryObject.AsObject())
                {
                    var itemKey = categoryItem.Key; //e.g. path
                    var itemValue = categoryItem.Value; //e.g. "C:\\"

                    nplItem.SetParameter(itemKey, itemValue);
                }
                ContentList.Add(data.Key, nplItem);
            }

            contentRoot = contentRoot.Replace("\\", "/");
            if (contentRoot.StartsWith("./"))
            {
                contentRoot = contentRoot.Substring(2, contentRoot.Length - 2);
            }

            var resources = new List<string>();
            foreach (var nplItem in ContentList)
            {
                string path = nplItem.Value.Path.ToString().Replace("\\", "/");
                if (path.Contains("../"))
                {
                    throw new Exception("'path' is not allowed to contain '../'! Use 'root' property to specify a different root instead.");
                }

                if (path.StartsWith('$'))
                {
                    // $ means that the path will not have the root appended to it.
                    path = path.TrimStart('$');
                }
                else if (contentRoot != "")
                {
                    // Appending root now so that we would work with full paths.
                    path = Path.Combine(contentRoot, path);
                }

                GetFilePath(path, out var fileName, out var filePath);

                var searchOpt = SearchOption.TopDirectoryOnly;
                if (nplItem.Value.Recursive)
                {
                    searchOpt = SearchOption.AllDirectories;
                }
                else searchOpt = SearchOption.TopDirectoryOnly;

                if (nplItem.Value.Action == ContentItem.BuildAction.Copy)
                {
                    var files = Directory.GetFiles(filePath, fileName, searchOpt);
                    foreach (var file in files)
                    {
                        resources.Add(file.Replace('\\', '/'));
                    }
                }
                else if (nplItem.Value.Action == ContentItem.BuildAction.Build)
                {
                    var files = Directory.GetFiles(filePath, fileName, searchOpt);
                    foreach (var file in files)
                    {
                        var resourcePath = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
                        resources.Add(resourcePath.Replace('\\', '/'));
                    }
                }
            }
            return resources.ToArray();
        }

        private static void GetFilePath(string path, out string fileName, out string filePath)
        {
            fileName = Path.GetFileName(path);
            filePath = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Directory.GetCurrentDirectory();
            }
        }

        public class ContentItem
        {
            public enum BuildAction
            {
                Build,
                Copy,
            }

            public string Path
            {
                get => _path;
                set
                {
                    _path = value.Replace("\\", "/");
                    if (_path.StartsWith("/"))
                    {
                        _path = Path.Substring(1);
                    }
                }
            }
            private string _path;
            public string Category;
            public bool Recursive;
            public List<string> Dependencies = [];
            public string[] Parameters;
            public BuildAction Action;

            public ContentItem(string category)
            {
                Category = category;
            }

            public void SetParameter(string param, object value)
            {
                switch (param)
                {
                    case "path":
                        Path = value.ToString();
                        break;
                    case "recursive":
                        Recursive = string.Compare(value.ToString(), "true", true) == 0;
                        break;
                    case "action":
                        Action = (BuildAction)Enum.Parse(typeof(BuildAction), value.ToString(), true);
                        break;
                    case "dependencies":
                        {
                            var itemArray = (JsonArray)value;
                            foreach (var item in itemArray)
                            {
                                Dependencies.Add(item.ToString());
                            }
                        }
                        break;
                    default:
                        {
                            if (Parameters == null) Parameters = new string[1];
                            else Array.Resize(ref Parameters, Parameters.Length + 1);

                            Parameters[^1] = GetParameterString(param, value);
                        }
                        break;
                }
            }

            public string GetParameterString(string param, object value)
            {
                return $"{param}:{value}";
            }
        }
    }
}
