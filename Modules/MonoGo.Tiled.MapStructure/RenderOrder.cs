
namespace MonoGo.Tiled.MapStructure
{
    /// <summary>
    /// Specifies the render order of tiles in a Tiled map.
    /// </summary>
    public enum RenderOrder : byte
    {
        /// <summary>
        /// Render tiles from right to left, top to bottom.
        /// </summary>
        RightDown = 0,

        /// <summary>
        /// Render tiles from right to left, bottom to top.
        /// </summary>
        RightUp = 1,

        /// <summary>
        /// Render tiles from left to right, top to bottom.
        /// </summary>
        LeftDown = 2,

        /// <summary>
        /// Render tiles from left to right, bottom to top.
        /// </summary>
        LeftUp = 3
    }
}
