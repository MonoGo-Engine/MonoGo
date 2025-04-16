using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGo.Tiled.MapStructure
{
    /// <summary>
    /// Represents an image layer in a Tiled map.
    /// </summary>
    public class TiledMapImageLayer : TiledMapLayer
    {
        /// <summary>
        /// Gets or sets the path to the texture used in the image layer.
        /// </summary>
        public string TexturePath;

        /// <summary>
        /// Gets or sets the texture used in the image layer.
        /// </summary>
        public Texture2D Texture;

        /// <summary>
        /// Gets or sets the transparent color for the image layer.
        /// </summary>
        /// <remarks>
        /// Tiled will treat this color as transparent.
        /// </remarks>
        public Color TransparentColor;
    }
}
