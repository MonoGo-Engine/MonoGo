
namespace MonoGo.Tiled.MapStructure.Objects
{
    /// <summary>
    /// Specifies the type of a Tiled object.
    /// </summary>
    internal enum TiledObjectType : byte
    {
        /// <summary>
        /// No specific type.
        /// </summary>
        None = 0,

        /// <summary>
        /// A tile object.
        /// </summary>
        Tile = 1,

        /// <summary>
        /// A point object.
        /// </summary>
        Point = 2,

        /// <summary>
        /// A rectangle object.
        /// </summary>
        Rectangle = 3,

        /// <summary>
        /// An ellipse object.
        /// </summary>
        Ellipse = 4,

        /// <summary>
        /// A polygon object.
        /// </summary>
        Polygon = 5,

        /// <summary>
        /// A text object.
        /// </summary>
        Text = 6,
    }
}
