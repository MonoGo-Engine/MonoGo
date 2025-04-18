﻿using System;
using System.Collections.Generic;

namespace MonoGo.Engine.Resources
{
	/// <summary>
	/// Central place to access all game resources.
	/// Hub holds resource boxes and gives access to them.
	/// </summary>
	public static class ResourceHub
	{
		private static Dictionary<string, IResourceBox> _boxes =
			new Dictionary<string, IResourceBox>(StringComparer.OrdinalIgnoreCase);

		public static IReadOnlyDictionary<string, IResourceBox> Boxes => _boxes;

		public static void UnloadAll()
		{
			foreach (var boxPair in _boxes)
			{
				boxPair.Value.Unload();
			}
		}

		public static void AddResourceBox(string key, IResourceBox box) =>
			_boxes.Add(key, box);

		public static void RemoveResourceBox(string key) =>
			_boxes.Remove(key);

		public static IResourceBox GetResourceBox(string key)
		{
			if (_boxes.TryGetValue(key, out IResourceBox box))
			{
				if (!box.Loaded)
				{
					box.Load();
				}

				return box;
			}

			return null;
		}

		public static IEnumerable<IResourceBox> GetResourceBoxes<T>()
		{
			var type = typeof(T);

			// Could be done faster with an additional <Type, ResourceBox> dictionary, but I don't think it's that necessary.
			foreach (var box in _boxes)
			{
				if (box.Value.Type == type)
				{
					if (!box.Value.Loaded)
					{
						box.Value.Load();
					}
					yield return box.Value;
				}
			}
		}

		public static bool ContainsResourceBox(string key) =>
			_boxes.ContainsKey(key);

		/// <summary>
		/// Returns all resources from the ResourceHub.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <returns></returns>
        public static List<TValue>? GetResources<TValue>()
        {
            foreach (var box in GetResourceBoxes<TValue>())
            {
				return ((ResourceBox<TValue>)box).GetResources();
            }
            return default;
        }

        /// <summary>
        /// Returns all resources from a specific ResourceBox.
        /// </summary>
        public static List<TValue>? GetResources<TValue>(string boxKey)
        {
			var box = GetResourceBox(boxKey) as ResourceBox<TValue>;
			if (box != null) return box.GetResources();

			return default;
        }

        /// <summary>
        /// Returns resource from the hub with the given name.
        /// This method doesn't use the resource box name so if you have several resource boxes 
        /// with the same type, it'll search all of them and pick the first match. 
        /// </summary>
        public static TValue? GetResource<TValue>(string resourceKey)
		{
			foreach (var box in GetResourceBoxes<TValue>())
			{
				if (((ResourceBox<TValue>)box).TryGetResource(resourceKey, out var resource))
				{
					return resource;
				}
			}
			return default;
		}

		/// <summary>
		/// Returns resource from the hub with the given name.
		/// </summary>
		public static TValue? GetResource<TValue>(string boxKey, string resourceKey)
		{
			if (_boxes.TryGetValue(boxKey, out IResourceBox box))
			{
				if (!box.Loaded)
				{
					box.Load();
				}

				return ((ResourceBox<TValue>)box).GetResource(resourceKey);
			}
			return default;
		}

		/// <summary>
		/// Returns resource from the hub with the given name.
		/// </summary>
		public static bool ContainsResource<TValue>(string boxKey, string resourceKey)
		{
			if (_boxes.TryGetValue(boxKey, out IResourceBox box))
			{
				return ((ResourceBox<TValue>)box).ContainsResource(resourceKey);
			}
			return false;
		}

		/// <summary>
		/// Returns resource from the hub with the given name.
		/// </summary>
		public static bool ContainsResource<TValue>(string resourceKey)
		{
			foreach (var box in GetResourceBoxes<TValue>())
			{
				if (((ResourceBox<TValue>)box).ContainsResource(resourceKey))
				{
					return true;
				}
			}

			return false;
		}
	}
}
