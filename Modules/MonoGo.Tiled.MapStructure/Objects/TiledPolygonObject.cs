using Microsoft.Xna.Framework;

namespace MonoGo.Tiled.MapStructure.Objects
{
    /// <summary>
    /// Represents a polygon object in a Tiled map.
    /// </summary>
    public class TiledPolygonObject : TiledObject
    {
        /// <summary>
        /// Gets or sets a value indicating whether the polygon is closed.
        /// </summary>
        public bool Closed;

        /// <summary>
        /// Gets or sets the points that define the polygon.
        /// </summary>
        public Vector2[] Points;

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledPolygonObject"/> class.
        /// </summary>
        public TiledPolygonObject() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledPolygonObject"/> class by copying another object.
        /// </summary>
        /// <param name="obj">The object to copy.</param>
        public TiledPolygonObject(TiledObject obj) : base(obj) { }
    }
}
