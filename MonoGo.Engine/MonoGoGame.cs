using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGo.Engine.Drawing;
using System;
using System.Diagnostics;

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
            Content.RootDirectory = "Content";

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

        protected void LoadEngineContent()
        {
            GraphicsMgr.Init(GraphicsDevice);

            try
            {
                Text.CurrentFont = GameMgr.AssetService.Load<IFont>("Engine/Fonts/Default");
            }
            catch (Exception e) { Debug.WriteLine($"--> Engine Content loading skipped: {e.Message}"); }
        }

        protected override void UnloadContent()
        {
            GameMgr.UnloadAssets();
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
