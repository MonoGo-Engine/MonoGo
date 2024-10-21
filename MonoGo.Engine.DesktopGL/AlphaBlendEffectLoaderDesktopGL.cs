using System.Reflection;

namespace MonoGo.Engine.DesktopGL
{
	internal class AlphaBlendEffectLoaderDesktopGl: AlphaBlendEffectLoader
	{
		protected override string _effectName => "MonoGo.Engine.DesktopGL.AlphaBlend_gl.mgfxo";

		protected override Assembly _assembly => Assembly.GetAssembly(typeof(AlphaBlendEffectLoaderDesktopGl));

	}
}
