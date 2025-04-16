using Microsoft.Xna.Framework;

namespace MonoGo.Tiled.MapStructure.Objects
{
    /// <summary>
    /// Represents an ellipse object in a Tiled map.
    /// </summary>
    public class TiledEllipseObject : TiledObject
	{
		public Vector2 Center => Position + Size / 2f;

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledEllipseObject"/> class.
        /// </summary>
        public TiledEllipseObject() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledEllipseObject"/> class by copying another object.
        /// </summary>
        /// <param name="obj">The object to copy.</param>
        public TiledEllipseObject(TiledObject obj) : base(obj) { }
    }
}
