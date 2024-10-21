using System;
using System.Collections.Generic;

namespace MonoGo.Engine
{
	/// <summary>
	/// Contains (usually platform-specific) stuff.
	/// </summary>
	internal static class StuffResolver
	{
		private static Dictionary<Type, object> _stuff = new Dictionary<Type, object>();

        internal static void AddStuffAs<T>(object stuff)
		{
			var type = typeof(T);
			if (!type.IsInterface)
			{
				throw new Exception("Provided type " + type + " has to be an interface!");
			}
			foreach (var i in stuff.GetType().GetInterfaces())
			{
				if (type == i)
				{
					_stuff.Add(type, stuff);
					return;
				}
			}
			throw new Exception("Object should implement interface " + type + "!");
		}

        internal static T GetStuff<T>() =>
			(T)_stuff[typeof(T)];

        internal static bool StuffExists<T>() =>
			_stuff.ContainsKey(typeof(T));
	}
}
