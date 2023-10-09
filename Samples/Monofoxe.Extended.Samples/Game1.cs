﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monofoxe.Extended.Engine;
using Monofoxe.Extended.Engine.Drawing;
using Monofoxe.Extended.Engine.Resources;
using Monofoxe.Extended.Resources;
using Monofoxe.Extended.Samples.Resources;
using Monofoxe.Extended.Tiled;
using Monofoxe.Extended.Tiled.MapStructure;
using Monofoxe.Extended.GUI;
using System;

namespace Monofoxe.Extended.Samples
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
    {
        public Game1()
		{
			Content.RootDirectory = ResourceInfoMgr.ContentDir;
            GameMgr.Init(this);

            if (GameMgr.CurrentPlatform == Platform.Android)
			{
				GameMgr.WindowManager.SetFullScreen(true); // Has to be exactly here, apparently.
			}
        }

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
        {
			base.Initialize();
			TiledEntityFactoryPool.InitFactoryPool();

            var depth = new DepthStencilState();
			depth.DepthBufferEnable = true;
			depth.DepthBufferWriteEnable = true;
			GraphicsMgr.Device.PresentationParameters.DepthStencilFormat = DepthFormat.Depth24Stencil8;

			GraphicsMgr.VertexBatch.DepthStencilState = depth;
            new GameController();

			RenderMgr.Init();
        }

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			GraphicsMgr.Init(GraphicsDevice);

            new SpriteGroupResourceBox("DefaultSprites", "Graphics/Default");
            new SpriteGroupResourceBox("GUISprites", "Graphics/GUI");
            new DirectoryResourceBox<Effect>("Effects", "Effects");
			new DirectoryResourceBox<TiledMap>("Maps", "Maps");

			new Fonts();

            UserInterface.Init("Graphics/GUI/Styles");
            UserInterface.Active.BlendState = BlendState.AlphaBlend;
            UserInterface.Active.SamplerState = SamplerState.PointWrap;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
		{
			ResourceHub.UnloadAll();
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			GameMgr.Update(gameTime);
			
			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
        {
            GameMgr.Draw(gameTime);

			UserInterface.Active.DrawCursor();

			base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
			RenderMgr.Destroy();

            base.OnExiting(sender, args);
        }
    }
}
