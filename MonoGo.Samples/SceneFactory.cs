using MonoGo.Engine;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.SceneSystem;
using System;

namespace MonoGo.Samples
{
	public class SceneFactory
	{
		public Scene Scene;
		public Type Type { get; private set; }

		public SceneFactory(Type type, string description = "")
		{
			Type = type;
			Description = description;
		}

		public readonly string Description;

		public void CreateScene()
		{
			Text.CurrentFont = GameMgr.AssetService.Load<IFont>("Engine/Fonts/Default");

            Scene = SceneMgr.CreateScene(Type.Name);
			Scene.CreateLayer("default");
			Activator.CreateInstance(Type, Scene["default"]);
		}

		public void DestroyScene() =>
			SceneMgr.DestroyScene(Scene);
	}
}
