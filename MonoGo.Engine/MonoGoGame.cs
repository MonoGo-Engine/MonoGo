﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.Resources;
using MonoGo.Engine.UI;
using MonoGo.Resources;
using System;
using System.IO;

namespace MonoGo.Engine
{
    /// <summary>
    /// Loads engine specific content, which is why you **need** to inherit from this class to use the engine.
    /// 
    /// <inheritdoc/>
    /// </summary>
    public abstract class MonoGoGame : Game
    {
        public MonoGoGame()
        {
            Content.RootDirectory = ResourceInfoMgr.ContentDir;
            GameMgr.Init(this);

            if (GameMgr.CurrentPlatform == Platform.Android)
            {
                GameMgr.WindowManager.SetFullScreen(true); // Has to be exactly here, apparently.
            }
        }

        protected override void Initialize()
        {
            base.Initialize();

            RenderMgr.Init();

            var depth = new DepthStencilState
            {
                DepthBufferEnable = true,
                DepthBufferWriteEnable = true
            };
            GraphicsMgr.Device.PresentationParameters.DepthStencilFormat = DepthFormat.Depth24Stencil8;
            GraphicsMgr.VertexBatch.DepthStencilState = depth;
        }

        protected override void LoadContent()
        {            
            GraphicsMgr.Init(GraphicsDevice);

            //var resourcePaths = ResourceInfoMgr.GetResourcePaths("**");

            new SpriteGroupResourceBox("ParticleSprites", "Engine/Particles");
            new SpriteGroupResourceBox("LUTSprites", "Engine/LUT");
            new DirectoryResourceBox<Effect>("Effects", "Engine/Effects");
            new FontResourceBox("Fonts", "Engine/Fonts");

            Text.CurrentFont = ResourceHub.GetResource<IFont>("Fonts", "Default");

            UISystem.Init(Path.Combine(ResourceInfoMgr.ContentDir, "Engine/GUI"), "MonoGoTheme");
        }

        protected override void UnloadContent()
        {
            ResourceHub.UnloadAll();
        }

        protected override void Update(GameTime gameTime)
        {
            GameMgr.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GameMgr.Draw(gameTime);

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, ExitingEventArgs args)
        {
            RenderMgr.Dispose();

            base.OnExiting(sender, args);
        }
    }
}
