using Microsoft.Xna.Framework;

namespace MonoGo.Engine.ViewportAdapters
{
    public static class Extensions
    {
        public static Vector2 Scale(this Vector2 value)
        {
            return new Vector2(
                value.X * RenderMgr.ViewportAdapter.VirtualWidth, 
                value.Y * RenderMgr.ViewportAdapter.VirtualHeight);
        }

        public static float ScaleX(this float value)
        {
            return value * RenderMgr.ViewportAdapter.VirtualWidth;
        }

        public static float ScaleY(this float value)
        {
            return value * RenderMgr.ViewportAdapter.VirtualHeight;
        }

        public static Vector2 UnScale(this Vector2 value)
        {
            return new Vector2(
                value.X / RenderMgr.ViewportAdapter.VirtualWidth,
                value.Y / RenderMgr.ViewportAdapter.VirtualHeight);
        }

        public static float UnScaleX(this float value)
        {
            return value / RenderMgr.ViewportAdapter.VirtualWidth;
        }

        public static float UnScaleY(this float value)
        {
            return value / RenderMgr.ViewportAdapter.VirtualHeight;
        }

        public static Vector2 ToWorld(this Vector2 value)
        {
            return RenderMgr.ViewportAdapter.PointToScreen((int)value.X, (int)value.Y).ToVector2();
        }
    }
}
