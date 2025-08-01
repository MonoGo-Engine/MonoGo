using Microsoft.Xna.Framework;

namespace MonoGo.Engine.ViewportAdapters
{
    /// <summary>
    /// Provides extension methods for coordinate transformation and scaling with the ViewportAdapter.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Scales a Vector2 according to the virtual resolution of the ViewportAdapter.
        /// Useful for converting normalized values (0-1) to virtual pixel coordinates.
        /// </summary>
        /// <param name="value">The vector to be scaled.</param>
        /// <returns>A scaled vector according to the virtual resolution.</returns>
        public static Vector2 Scale(this Vector2 value)
        {
            return new Vector2(
                value.X * RenderMgr.ViewportAdapter.VirtualWidth, 
                value.Y * RenderMgr.ViewportAdapter.VirtualHeight);
        }

        /// <summary>
        /// Scales a float value according to the virtual width of the ViewportAdapter.
        /// Useful for horizontal distances or positions.
        /// </summary>
        /// <param name="value">The value to be scaled.</param>
        /// <returns>A scaled value according to the virtual width.</returns>
        public static float ScaleX(this float value)
        {
            return value * RenderMgr.ViewportAdapter.VirtualWidth;
        }

        /// <summary>
        /// Scales a float value according to the virtual height of the ViewportAdapter.
        /// Useful for vertical distances or positions.
        /// </summary>
        /// <param name="value">The value to be scaled.</param>
        /// <returns>A scaled value according to the virtual height.</returns>
        public static float ScaleY(this float value)
        {
            return value * RenderMgr.ViewportAdapter.VirtualHeight;
        }

        /// <summary>
        /// Scales a Vector2 back to normalized values (0-1) based on the virtual resolution.
        /// The reverse operation of Scale().
        /// </summary>
        /// <param name="value">The vector to be normalized in virtual coordinates.</param>
        /// <returns>A normalized vector with values between 0 and 1.</returns>
        public static Vector2 UnScale(this Vector2 value)
        {
            return new Vector2(
                value.X / RenderMgr.ViewportAdapter.VirtualWidth,
                value.Y / RenderMgr.ViewportAdapter.VirtualHeight);
        }

        /// <summary>
        /// Scales a float value back to a normalized value (0-1) based on the virtual width.
        /// The reverse operation of ScaleX().
        /// </summary>
        /// <param name="value">The value to be normalized in virtual X-coordinates.</param>
        /// <returns>A normalized value between 0 and 1.</returns>
        public static float UnScaleX(this float value)
        {
            return value / RenderMgr.ViewportAdapter.VirtualWidth;
        }

        /// <summary>
        /// Scales a float value back to a normalized value (0-1) based on the virtual height.
        /// The reverse operation of ScaleY().
        /// </summary>
        /// <param name="value">The value to be normalized in virtual Y-coordinates.</param>
        /// <returns>A normalized value between 0 and 1.</returns>
        public static float UnScaleY(this float value)
        {
            return value / RenderMgr.ViewportAdapter.VirtualHeight;
        }

        /// <summary>
        /// Transforms world coordinates to screen coordinates using the ViewportAdapter.
        /// Useful when drawing objects on screen or for user interaction.
        /// </summary>
        /// <param name="worldPosition">The position in world coordinates.</param>
        /// <returns>The corresponding position in screen coordinates.</returns>
        public static Vector2 ToScreen(this Vector2 worldPosition)
        {
            return RenderMgr.ViewportAdapter.WorldToViewport(worldPosition);
        }

        /// <summary>
        /// Transforms screen coordinates to world coordinates using the ViewportAdapter.
        /// Useful for mouse interactions or touch input to determine the position in the game world.
        /// </summary>
        /// <param name="viewportPosition">The position in screen coordinates.</param>
        /// <returns>The corresponding position in world coordinates.</returns>
        public static Vector2 ToWorld(this Vector2 viewportPosition)
        {
            return RenderMgr.ViewportAdapter.ViewportToWorld(viewportPosition);
        }
    }
}
