﻿using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.Enums;
using MonoGo.Engine.Resources;

namespace MonoGo.Engine.PostProcessing
{
    /// <summary>
    /// Color Grading is a post processing effect that changes colors based on the transformation value in a look-up table (LUT) for this specific color. 
    /// This enables very wide range of processing for specific values, and can be used to shift colors, change contrast, saturation, brightness and much much more.
    /// </summary>
    public static class ColorGrading
    {
        public static Surface Surface { get; private set; }
        
        public static Sprite CurrentLUT { get; set; }
        public static Sprite[] LUTs { get; private set; }

        private static Effect _shaderEffect;

        internal static void Init()
        {
            _shaderEffect = ResourceHub.GetResource<Effect>(nameof(EngineResources.Effects), "ColorGrading");

            var effectSpriteBox = ResourceHub.GetResourceBox(nameof(EngineResources.LUTSprites)) as SpriteGroupResourceBox;
            LUTs = effectSpriteBox.Select(x => x.Value).ToArray();

            CurrentLUT = LUTs.ToList().FirstOrDefault(
                x => x.Name.Contains("Default", System.StringComparison.InvariantCultureIgnoreCase)) ?? NextLUT();
        }

        internal static void Process()
        {
            if (RenderMgr.ColorGradingFX)
            {
                UpdateResolution();

                var renderTarget = RenderMgr.SceneSurface.RenderTarget;

                GraphicsMgr.VertexBatch.Texture = renderTarget;
                GraphicsMgr.VertexBatch.Effect = _shaderEffect;
                _shaderEffect.Parameters["Input"].SetValue(renderTarget);
                _shaderEffect.Parameters["LUT"].SetValue(CurrentLUT[0].Texture);
                _shaderEffect.Parameters["World"].SetValue(GraphicsMgr.VertexBatch.World);
                _shaderEffect.Parameters["View"].SetValue(GraphicsMgr.VertexBatch.View);
                _shaderEffect.Parameters["Projection"].SetValue(GraphicsMgr.VertexBatch.Projection);

                GraphicsMgr.VertexBatch.BlendState = BlendState.Opaque;

                Surface.SetTarget(Surface);
                GraphicsMgr.Device.Clear(Color.Black);
                GraphicsMgr.VertexBatch.AddQuad(Surface.Size);
                Surface.ResetTarget();

                GraphicsMgr.VertexBatch.BlendState = BlendState.AlphaBlend;

                GraphicsMgr.VertexBatch.Effect = null;
                GraphicsMgr.VertexBatch.Texture = null;
            }
        }

        public static Sprite NextLUT()
        {
            var i = LUTs.ToList().IndexOf(CurrentLUT);
            if (i == LUTs.Length - 1) CurrentLUT = LUTs[0];
            else CurrentLUT = LUTs[i + 1];

            return CurrentLUT;
        }

        public static Sprite PreviousLUT()
        {
            var i = LUTs.ToList().IndexOf(CurrentLUT);
            if (i == 0) CurrentLUT = LUTs[^1];
            else CurrentLUT = LUTs[i - 1];

            return CurrentLUT;
        }

        public static void SetLut(string key)
        {
            CurrentLUT = LUTs.First(x => x.Name == key);
        }

        private static void UpdateResolution()
        {
            if (Surface == null || Surface.Size != GameMgr.WindowManager.CanvasSize)
            {
                Surface = new Surface(GameMgr.WindowManager.CanvasSize);
            }
        }

        internal static void Dispose()
        {
            Surface?.Dispose();
            _shaderEffect?.Dispose();
        }
    }
}
