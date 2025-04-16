
namespace MonoGo.Tiled.MapStructure
{
    /// <summary>
    /// Specifies the drawing order for objects in a Tiled object layer.
    /// </summary>
    public enum TiledMapObjectDrawingOrder : byte
    {
        /// <summary>
        /// Objects are drawn from top to bottom.
        /// </summary>
        TopDown = 0,

        /// <summary>
        /// Objects are drawn in a manual order.
        /// </summary>
        Manual = 1
    }
}
