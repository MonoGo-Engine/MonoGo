using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGo.Engine;
using MonoGo.Engine.Cameras;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.EC;
using MonoGo.Engine.SceneSystem;
using MonoGo.Engine.Utils;
using MonoGo.Engine.ViewportAdapters;

namespace MGNamespace
{
    public class GameController : Entity
    {
        public static Camera2D MainCamera;

        public static RandomExt Random = new();
        public static RasterizerState DefaultRasterizer;

        public GameController(Vector2 screenSize, string title) : base(SceneMgr.DefaultLayer)
        {
            GameMgr.MaxGameSpeed = 60;
            GameMgr.MinGameSpeed = 60; // Fixing framerate on 60.

            MainCamera = new Camera2D(screenSize)
            {
                BackgroundColor = new Color(38, 38, 38)
            };

            GameMgr.WindowManager.PreferMultiSampling = true;
            GameMgr.WindowManager.CanvasSize = screenSize;
            GameMgr.WindowManager.Window.AllowUserResizing = false;
            GameMgr.WindowManager.ApplyChanges();
            GameMgr.WindowManager.CenterWindow();
            GameMgr.WindowManager.CanvasMode = CanvasMode.Fill;
            GameMgr.WindowManager.WindowTitle = title;
            //GameMgr.Game.Window.IsBorderless = true;
            //GameMgr.WindowManager.ToggleFullScreen();

            RenderMgr.ViewportAdapter = new ScalingViewportAdapter(1280, 720);

            DefaultRasterizer = new RasterizerState
            {
                CullMode = CullMode.CullCounterClockwiseFace,
                FillMode = FillMode.Solid,
                MultiSampleAntiAlias = true
            };
            GraphicsMgr.VertexBatch.RasterizerState = DefaultRasterizer;

            // Enabling applying postprocessing effects to separate layers.
            MainCamera.PostprocessingMode = PostprocessingMode.CameraAndLayers;
        }

        public override void Destroy()
        {
            base.Destroy();
        }
    }
}
