using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGo.Tiled.MapStructure
{
    /// <summary>
    /// Represents a generic layer in a Tiled map.
    /// </summary>
    public abstract class TiledMapLayer
    {
        /// <summary>
        /// Gets or sets the name of the layer.
        /// </summary>
        public string Name;

        /// <summary>
        /// Gets or sets the unique ID of the layer.
        /// </summary>
        public int ID;

        /// <summary>
        /// Gets or sets a value indicating whether the layer is visible.
        /// </summary>
        public bool Visible;

        /// <summary>
        /// Gets or sets the opacity of the layer.
        /// </summary>
        public float Opacity;

        /// <summary>
        /// Gets or sets the offset of the layer in pixels.
        /// </summary>
        public Vector2 Offset;

        /// <summary>
        /// Gets or sets the custom properties of the layer.
        /// </summary>
        public Dictionary<string, string> Properties;
    }
}
