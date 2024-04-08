﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGo.Engine;
using MonoGo.Engine.Cameras;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.EC;
using MonoGo.Engine.Resources;
using MonoGo.Engine.SceneSystem;
using MonoGo.Engine.Utils;
using System.Diagnostics;

namespace MonoGo.Samples
{
	public class GameController : Entity
	{
		public Camera2D MainCamera = new Camera2D(new Vector2(1280, 720));

		Layer _guiLayer;

		public static RasterizerState DefaultRasterizer;
		public static RasterizerState WireframeRasterizer;

		private Stopwatch _stopwatch = new Stopwatch();

		public static RandomExt Random = new RandomExt();

		public GameController() : base(SceneMgr.GetScene("default")["default"])
		{
			GameMgr.MaxGameSpeed = 60;
			GameMgr.MinGameSpeed = 60; // Fixing framerate on 60.

			MainCamera.BackgroundColor = new Color(38, 38, 38);

			GameMgr.WindowManager.CanvasSize = MainCamera.Size;
			GameMgr.WindowManager.Window.AllowUserResizing = false;
			GameMgr.WindowManager.ApplyChanges();
			GameMgr.WindowManager.CenterWindow();
			GameMgr.WindowManager.CanvasMode = CanvasMode.Fill;

			GraphicsMgr.VertexBatch.SamplerState = SamplerState.PointWrap; // Will make textures repeat without interpolation.

			DefaultRasterizer = new RasterizerState();
			DefaultRasterizer.CullMode = CullMode.CullCounterClockwiseFace;
			DefaultRasterizer.FillMode = FillMode.Solid;
			DefaultRasterizer.MultiSampleAntiAlias = false;

			WireframeRasterizer = new RasterizerState();
			WireframeRasterizer.CullMode = CullMode.CullCounterClockwiseFace;
			WireframeRasterizer.FillMode = FillMode.WireFrame;
			WireframeRasterizer.MultiSampleAntiAlias = false;

			GraphicsMgr.VertexBatch.RasterizerState = DefaultRasterizer;

			_guiLayer = Scene.CreateLayer("gui");
			_guiLayer.IsGUI = true;

			var cameraController = new CameraController(_guiLayer, MainCamera);

			var switcher = new SceneSwitcher(_guiLayer, cameraController);
			switcher.CurrentFactory.CreateScene();
			switcher.CreateUI();

			// Enabling applying postprocessing effects to separate layers.
			// Note that this will create an additional surface.
			MainCamera.PostprocessingMode = PostprocessingMode.CameraAndLayers;

			// Setting a default Font to avoid crashes when skipping the samples backwards.
            Text.CurrentFont = ResourceHub.GetResource<IFont>("Fonts", "Arial");

            SceneMgr.OnPreDraw += OnPreDraw; // You can do the same for individual layers or scenes.
			SceneMgr.OnPostDraw += OnPostDraw;
		}

		private void OnPreDraw() =>
			_stopwatch.Start();

		private void OnPostDraw()
		{
			_stopwatch.Stop();
			GameMgr.WindowManager.WindowTitle = "Rendering time: " + _stopwatch.Elapsed;
			_stopwatch.Reset();
		}

		public override void Destroy()
		{
			base.Destroy();

			SceneMgr.OnPreDraw -= OnPreDraw;
			SceneMgr.OnPostDraw -= OnPostDraw;
		}
	}
}