
namespace MonoGo.Tiled.MapStructure.Objects
{
    /// <summary>
    /// Represents a rectangle object in a Tiled map.
    /// </summary>
    public class TiledRectangleObject : TiledObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TiledRectangleObject"/> class.
        /// </summary>
        public TiledRectangleObject() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledRectangleObject"/> class by copying another object.
        /// </summary>
        /// <param name="obj">The object to copy.</param>
        public TiledRectangleObject(TiledObject obj) : base(obj) { }
    }
}

