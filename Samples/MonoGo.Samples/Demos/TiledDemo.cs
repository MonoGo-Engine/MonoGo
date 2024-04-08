﻿using MonoGo.Engine;
using MonoGo.Engine.EC;
using MonoGo.Engine.Resources;
using MonoGo.Engine.SceneSystem;
using MonoGo.Samples.Misc.Tiled;
using MonoGo.Tiled;
using MonoGo.Tiled.MapStructure;
using System;

namespace MonoGo.Samples.Demos
{
	/// <summary>
	/// Tiled is a free map editor which can be downloaded at
	/// https://mapeditor.org
	/// 
	/// Though, note that not all the Tiled features are
	/// currently supported (like infinite tilemaps or animated tiles.)
	/// </summary>
	public class TiledDemo : Entity
	{
		public static readonly string Description =
			"Build > {{L_GREEN}}Collision{{DEFAULT}}: {{YELLOW}}" + BuildCustomMapBuilderButton + "{{L_GREEN}} Default{{DEFAULT}}: {{YELLOW}}" + BuildDefaultMapBuilderButton + "{{DEFAULT}}" + " Delete > {{YELLOW}}" + DestroyMapButton + "{{DEFAULT}}" + Environment.NewLine +
			"Move > {{YELLOW}}WASD{{DEFAULT}}" + Environment.NewLine +
			"Camera > {{L_GREEN}}Move{{DEFAULT}}: {{YELLOW}}" + CameraController.UpButton + " / " + CameraController.DownButton + " / " + CameraController.LeftButton + " / " + CameraController.RightButton + "{{DEFAULT}}";

		MapBuilder _builder;

		public const Buttons BuildCustomMapBuilderButton = Buttons.B;
		public const Buttons BuildDefaultMapBuilderButton = Buttons.N;
		public const Buttons DestroyMapButton = Buttons.U;

		private TiledMap _testMap; 

		public TiledDemo(Layer layer) : base(layer)
		{
			// TiledMap which is loaded from Content, is just a data structure
			// describing the map. We need to make an actual Scene object with entities on it.
			// You can write your own map builder, or use the default one.
			// Default map builder can also be expanded.

			_testMap = ResourceHub.GetResource<TiledMap>("Maps", "Test");

			_builder = new SolidMapBuilder(_testMap);
			_builder.Build();
		}

		public override void Update()
		{
			base.Update();

			if (Input.CheckButtonPress(BuildCustomMapBuilderButton))
			{
				if (_builder != null)
				{
					_builder.Destroy();
					_builder = null;
				}
				_builder = new SolidMapBuilder(_testMap);
				_builder.Build();
			}

			if (Input.CheckButtonPress(BuildDefaultMapBuilderButton))
			{
				if (_builder != null)
				{
					_builder.Destroy();
					_builder = null;
				}
				_builder = new MapBuilder(_testMap);
				_builder.Build();
			}


			if (_builder != null && Input.CheckButtonPress(DestroyMapButton))
			{
				_builder.Destroy();
				_builder = null;
			}
		}


		public override void Destroy()
		{
			base.Destroy();

			if (_builder != null)
			{
				_builder.Destroy();
			}
		}
	}
}