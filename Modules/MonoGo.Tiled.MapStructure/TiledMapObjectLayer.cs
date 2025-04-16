using Microsoft.Xna.Framework;
using MonoGo.Tiled.MapStructure.Objects;

namespace MonoGo.Tiled.MapStructure
{
    /// <summary>
    /// Represents an object layer in a Tiled map.
    /// </summary>
    public class TiledMapObjectLayer : TiledMapLayer
    {
        /// <summary>
        /// Gets or sets the array of objects in the layer.
        /// </summary>
        public TiledObject[] Objects;

        /// <summary>
        /// Gets or sets the drawing order of objects in the layer.
        /// </summary>
        public TiledMapObjectDrawingOrder DrawingOrder;

        /// <summary>
        /// Gets or sets the color of the object layer.
        /// </summary>
        public Color Color;
    }
}
