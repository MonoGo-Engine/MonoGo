﻿using Microsoft.Xna.Framework;

namespace MonoGo.Engine.Utils
{
    /// <summary>
    /// You can use this class to scale the contents of your world in relation to the <c>GameMgr.WindowManager.CanvasSize</c>.
    /// </summary>
    public static class WorldScaler
    {
        /// <summary>
        /// Usually your world map size.
        /// </summary>
        public static Vector2 WorldScaleBase
        {
            get { return _worldScaleBase; }
            private set
            {
                _worldScaleBase = value;
                CalculateScalingFactor();
            }
        }
        private static Vector2 _worldScaleBase;

        /// <summary>
        /// The factor of how much the world scaled based on <c>WorldScaleBase</c>.
        /// </summary>
        /// <remarks>Use this value to scale the contents of your world.</remarks>
        public static Vector2 WorldScaleVector { get; set; } = Vector2.One;

        /// <summary>
        /// The factor of how much the world scaled based on <c>WorldScaleBase</c>.
        /// </summary>
        /// <remarks>Use this value to scale the contents of your world.</remarks>
        public static float WorldScaleFactor { get; } = (WorldScaleVector.X + WorldScaleVector.Y) / 2f;

        /// <summary>
        /// Initialize the WorldScaler for helping scale your world and world-contents.
        /// </summary>
        /// <param name="worldScaleBase">Usually your world map size.</param>
        public static void Init(Vector2 worldScaleBase)
        {
            WorldScaleBase = worldScaleBase;
        }

        private static void CalculateScalingFactor()
        {
            WorldScaleVector = new Vector2(
                GameMgr.WindowManager.CanvasSize.X / WorldScaleBase.X,
                GameMgr.WindowManager.CanvasSize.Y / WorldScaleBase.Y);
        }

        public static float Scale(this float value)
        {
            return value * ((WorldScaleVector.X + WorldScaleVector.Y) / 2f);
        }

        public static Vector2 WorldToScreen(Vector2 value)
        {
            return value * GameMgr.WindowManager.CanvasSize;
        }

        public static Vector2 ScreenToWorld(Vector2 value)
        {
            return value / GameMgr.WindowManager.CanvasSize;
        }

        public static float WorldToScreenX(float value)
        {
            return value * GameMgr.WindowManager.CanvasSize.X;
        }

        public static float WorldToScreenY(float value)
        {
            return value * GameMgr.WindowManager.CanvasSize.Y;
        }

        public static float ScreenToWorldX(float value)
        {
            return value / GameMgr.WindowManager.CanvasSize.X;
        }

        public static float ScreenToWorldY(float value)
        {
            return value / GameMgr.WindowManager.CanvasSize.Y;
        }

        public static float ToScreenX(this float value)
        {
            return value * GameMgr.WindowManager.CanvasSize.X;
        }

        public static float ToScreenY(this float value)
        {
            return value * GameMgr.WindowManager.CanvasSize.Y;
        }

        public static float ToWorld(this float value, bool scale = false)
        {
            if (scale) value = value.Scale();
            return value / ((GameMgr.WindowManager.CanvasSize.X + GameMgr.WindowManager.CanvasSize.Y) / 2f);
        }

        public static float ToWorldX(this float value)
        {
            return value / GameMgr.WindowManager.CanvasSize.X;
        }

        public static float ToWorldY(this float value)
        {
            return value / GameMgr.WindowManager.CanvasSize.Y;
        }

        public static Vector2 ToScreen(this Vector2 value)
        {
            return value * GameMgr.WindowManager.CanvasSize;
        }

        public static Vector2 ToWorld(this Vector2 value)
        {
            return value / GameMgr.WindowManager.CanvasSize;
        }
    }
}
