﻿using System.Reflection;

namespace MonoGo.Engine.WindowsDX
{
    internal class AlphaBlendEffectLoaderWindowsDX : AlphaBlendEffectLoader
	{
		protected override string _effectName => "MonoGo.Engine.WindowsDX.AlphaBlend_dx.mgfxo";

		protected override Assembly _assembly => Assembly.GetAssembly(typeof(AlphaBlendEffectLoaderWindowsDX));
	}
}
