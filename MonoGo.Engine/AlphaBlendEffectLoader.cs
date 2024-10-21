using Microsoft.Xna.Framework.Graphics;
using MonoGo.Engine.Drawing;
using System.IO;
using System.Reflection;

namespace MonoGo.Engine
{
    internal interface IAlphaBlendEffectLoader
	{ 
		Effect Load();
	}

    internal abstract class AlphaBlendEffectLoader : IAlphaBlendEffectLoader
	{
		protected abstract string _effectName { get; }
		protected abstract Assembly _assembly { get; }

		private readonly object _lock = new object();

        internal byte[] Bytecode
		{
			get
			{
				if (_bytecode == null)
				{
					lock (_lock)
					{
						if (_bytecode != null)
						{
							return _bytecode;
						}

						var stream = _assembly.GetManifestResourceStream(_effectName);
						using (var ms = new MemoryStream())
						{
							stream.CopyTo(ms);
							_bytecode = ms.ToArray();
						}
					}
				}

				return _bytecode;
			}
		}
		private volatile byte[] _bytecode;


        public Effect Load() =>
			new Effect(GraphicsMgr.Device, Bytecode);
	}
}
