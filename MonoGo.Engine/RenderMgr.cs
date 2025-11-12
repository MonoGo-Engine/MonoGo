using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.PostProcessing;
using MonoGo.Engine.SceneSystem;
using MonoGo.Engine.ViewportAdapters;
using System;
using System.Diagnostics;

namespace MonoGo.Engine
{
    /// <summary>
    /// The render manager draws the (post-processed) scene- and gui surface in the correct order.
    /// Set the properties here to enable or disable certain PostFX features or all at once via the "PostProcessing" property.
    /// </summary>
    public static class RenderMgr
    {
        public static bool Initialized { get; internal set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the post-processed scene should be manually rendered to a render target.
        /// </summary>
        /// <remarks>
        /// When set to <see langword="true"/>, the post-processed scene will not be automatically rendered and must be explicitly handled by the user.
        /// 
        /// <para>
        /// <strong>Important Notes:</strong>
        /// <list type="bullet">
        /// <item><description>When enabled, post-processing effects (Color Grading, Bloom) are rendered to <see cref="PostFXSceneSurface"/> instead of directly to the screen.</description></item>
        /// <item><description>You must manually call <c>PostFXSceneSurface.Draw();</c> to render it to the screen.</description></item>
        /// <item><description>This feature is useful for custom rendering pipelines, advanced post-processing chains, or integration with external rendering systems.</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public static bool DrawPostFXSceneToRenderTarget { get; set; } = false;

        public static bool PostProcessing { get; set; } = false;
        public static bool ColorGradingFX { get; set; } = true;
        public static bool BloomFX { get; set; } = true;

        public static ViewportAdapter ViewportAdapter { get; set; } = new DefaultViewportAdapter();
        public static Surface PostFXSceneSurface { get; private set; }
        public static Surface SceneSurface { get; private set; }
        public static Surface GUISurface { get; private set; }

        internal static void Init()
        {
            try
            {
                ColorGrading.Init();
                Bloom.Init();

                Initialized = true;
            }
            catch (Exception e) { Debug.WriteLine($"--> Shader Effects not initialized: {e.Message}"); }

            SceneMgr.OnPreDraw += SceneMgr_OnPreDraw;
            SceneMgr.OnPostDraw += SceneMgr_OnPostDraw;
            SceneMgr.OnPreDrawGUI += SceneMgr_OnPreDrawGUI;
            SceneMgr.OnPostDrawGUI += SceneMgr_OnPostDrawGUI;
        }

        private static void SceneMgr_OnPreDraw()
        {
            UpdateResolution();

            Surface.SetTarget(SceneSurface, GraphicsMgr.VertexBatch.View);
            GraphicsMgr.Device.Clear(GraphicsMgr.CurrentCamera.BackgroundColor);
        }

        private static void SceneMgr_OnPreDrawGUI()
        {
            Surface.SetTarget(GUISurface);
            GraphicsMgr.Device.Clear(Color.Transparent);
        }

        private static void SceneMgr_OnPostDraw()
        {
            if (!Surface.SurfaceStackEmpty)
            {
                Surface.ResetTarget();
            }
        }

        private static void SceneMgr_OnPostDrawGUI()
        {
            if (!Surface.SurfaceStackEmpty)
            {
                Surface.ResetTarget();
            }

            if (Initialized && PostProcessing && (ColorGradingFX || BloomFX))
            {
                ColorGrading.Process();
                Bloom.Process();
                DrawPostFXScene();
            }
            else SceneSurface.Draw();

            GUISurface.Draw();
        }

        private static void DrawPostFXScene()
        {
            if (DrawPostFXSceneToRenderTarget)
            {
                Surface.SetTarget(PostFXSceneSurface);
                GraphicsMgr.Device.Clear(Color.Black);
            }

            GraphicsMgr.VertexBatch.BlendState = BlendState.Additive;

            if (ColorGradingFX) ColorGrading.Surface.Draw(Vector2.Zero, ViewportAdapter.GetScale(), Angle.Right);
            else SceneSurface.Draw(Vector2.Zero, ViewportAdapter.GetScale(), Angle.Right);

            if (BloomFX) Bloom.Surface.Draw(Vector2.Zero, ViewportAdapter.GetScale(), Angle.Right);

            GraphicsMgr.VertexBatch.BlendState = BlendState.AlphaBlend;

            if (DrawPostFXSceneToRenderTarget)
            {
                Surface.ResetTarget();
            }
        }

        private static void UpdateResolution()
        {
            if (PostFXSceneSurface == null || PostFXSceneSurface.Size != GameMgr.WindowManager.CanvasSize)
            {
                PostFXSceneSurface = new Surface(GameMgr.WindowManager.CanvasSize);
            }

            if (SceneSurface == null || SceneSurface.Size != GameMgr.WindowManager.CanvasSize)
            {
                SceneSurface = new Surface(GameMgr.WindowManager.CanvasSize);
            }

            if (GUISurface == null || GUISurface.Size != GameMgr.WindowManager.CanvasSize)
            {
                GUISurface = new Surface(GameMgr.WindowManager.CanvasSize);
            }
        }

        public static void Dispose()
        {
            PostFXSceneSurface?.Dispose();
            SceneSurface?.Dispose();
            GUISurface?.Dispose();
            ColorGrading.Dispose();
            Bloom.Dispose();
        }
    }
}
