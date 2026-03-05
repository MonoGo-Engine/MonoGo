using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MonoGo.Engine
{
    /// <summary>
    /// Provides convenient resource paths and lists all content assets.
    /// </summary>
    public static class ResourceInfoMgr
	{
		/// <summary>
		/// Root directory of the game content.
		/// </summary>
		public static string ContentDir = "Content";


		/// <summary>
		/// List of all game ssets.
		/// </summary>
		private static string[] _assetPaths;



		internal static void Init()
        {
            var assets = new List<string>();
            var contentPath = Path.GetFullPath(ContentDir);

			// Alle Dateien rekursiv aus dem ContentDir laden
			if (Directory.Exists(contentPath))
			{
				var files = Directory.GetFiles(contentPath, "*", SearchOption.AllDirectories);
				foreach (var file in files)
				{
					// Relativen Pfad vom ContentDir aus ermitteln
					var relativePath = Path.GetRelativePath(contentPath, file);
					
					// .xnb Endung entfernen, falls vorhanden
					if (relativePath.EndsWith(".xnb"))
					{
						relativePath = relativePath[..^4]; // Letzte 4 Zeichen (.xnb) entfernen
					}

                    // Backslashes durch Slashes ersetzen für Konsistenz
                    var normalizedPath = relativePath.Replace("\\", "/");
					assets.Add(normalizedPath);
				}
			}

            Array.Resize(ref _assetPaths, assets.Count);
			assets.CopyTo(_assetPaths);
        }


		/// <summary>
		/// Returns list of resource paths matching the given wildcard pattern. * and ? are supported.
		/// </summary>
		public static string[] GetResourcePaths(string pattern)
		{
			if (_assetPaths == null)
			{
				return null;
			}

			if (pattern == "")
			{
				var assetPathsCopy = new string[_assetPaths.Length];
				_assetPaths.CopyTo(assetPathsCopy, 0);
				return assetPathsCopy;
			}

			var pattenRegex = WildCardToRegular(pattern.Replace("\\", "/"));

			var list = new List<string>();
			foreach (var info in _assetPaths)
			{
				if (Regex.IsMatch(info, pattenRegex))
				{
					list.Add(info);
				}
			}

			return list.ToArray();
		}

		private static string WildCardToRegular(string value) =>
			"^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
	}
}
