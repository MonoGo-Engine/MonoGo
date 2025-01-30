using Microsoft.Xna.Framework;
using System;

namespace MonoGo.Engine.Utils
{
    public static class WorldScaler
    {
        public static Vector2 WorldScaleBase
        {
            get { return _worldScaleBase; }
            set
            {
                _worldScaleBase = value;
                CalculateScalingFactor();
            }
        }
        private static Vector2 _worldScaleBase = new(1280, 720);
        public static float WorldScaleFactor { get; set; } = 1f;

        public static void Init(Vector2 worldScaleBase = default)
        {
            WorldScaleBase = worldScaleBase == default ? _worldScaleBase : worldScaleBase;
        }

        private static void CalculateScalingFactor()
        {
            WorldScaleFactor = Math.Min(
                GameMgr.WindowManager.CanvasSize.X / WorldScaleBase.X,
                GameMgr.WindowManager.CanvasSize.Y / WorldScaleBase.Y);
        }

        public static Vector2 WorldToScreen(Vector2 value)
        {
            return value * WorldScaleFactor;
        }

        public static Vector2 ScreenToWorld(Vector2 value)
        {
            return value / WorldScaleFactor;
        }

        public static float WorldToScreen(float value)
        {
            return value * WorldScaleFactor;
        }

        public static float ScreenToWorld(float value)
        {
            return value / WorldScaleFactor;
        }

        public static float ToScreen(this float value)
        {
            return value * WorldScaleFactor;
        }

        public static float ToWorld(this float value)
        {
            return value / WorldScaleFactor;
        }

        public static Vector2 ToScreen(this Vector2 value)
        {
            return value * WorldScaleFactor;
        }

        public static Vector2 ToWorld(this Vector2 value)
        {
            return value / WorldScaleFactor;
        }
    }
}
