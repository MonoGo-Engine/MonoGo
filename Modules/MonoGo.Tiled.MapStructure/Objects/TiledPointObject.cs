using Microsoft.Xna.Framework;

namespace MonoGo.Tiled.MapStructure.Objects
{
    /// <summary>
    /// Represents a point object in a Tiled map.
    /// </summary>
    public class TiledPointObject : TiledObject
    {
        /// <summary>
        /// Gets the size of the point, which is always zero.
        /// </summary>
        public new Vector2 Size => Vector2.Zero; // Points cannot have size.

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledPointObject"/> class.
        /// </summary>
        public TiledPointObject() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledPointObject"/> class by copying another object.
        /// </summary>
        /// <param name="obj">The object to copy.</param>
        public TiledPointObject(TiledObject obj) : base(obj) { }
    }
}
