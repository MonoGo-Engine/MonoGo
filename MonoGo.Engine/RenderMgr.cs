﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.PostProcessing;
using MonoGo.Engine.SceneSystem;

namespace MonoGo.Engine
{
    public static class RenderMgr
    {
        public static bool PostProcessing { get; set; } = false;

        public static Surface SceneSurface { get; set; }
        public static Surface GUISurface { get; set; }

        public static Matrix GUITransformMatrix { get; set; } = Matrix.Identity;

        public static void Init()
        {
            ColorGrading.Init();
            Bloom.Init();

            SceneSurface = new Surface(GameMgr.WindowManager.CanvasSize);
            GUISurface = new Surface(GameMgr.WindowManager.CanvasSize);

            SceneMgr.OnPreDraw += SceneMgr_OnPreDraw;
            SceneMgr.OnPostDraw += SceneMgr_OnPostDraw;
            SceneMgr.OnPreDrawGUI += SceneMgr_OnPreDrawGUI;
            SceneMgr.OnPostDrawGUI += SceneMgr_OnPostDrawGUI;
        }

        private static void SceneMgr_OnPreDraw()
        {
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

            if (PostProcessing)
            {
                ColorGrading.Process();
                Bloom.Process(ColorGrading.Surface.RenderTarget);

                GraphicsMgr.VertexBatch.BlendState = BlendState.Additive;
                ColorGrading.Surface.Draw();
                Bloom.Surface.Draw();
                GraphicsMgr.VertexBatch.BlendState = BlendState.AlphaBlend;
            }
            else SceneSurface.Draw();

            if (GUITransformMatrix == Matrix.Identity) GUISurface.Draw();
            else
            {
                GraphicsMgr.VertexBatch.PushViewMatrix(GUITransformMatrix);
                GUISurface.Draw();
                GraphicsMgr.VertexBatch.PopViewMatrix();
            }
        }

        public static void Destroy()
        {
            SceneSurface.Dispose();
            GUISurface.Dispose();
            ColorGrading.Dispose();
            Bloom.Dispose();
        }
    }
}
