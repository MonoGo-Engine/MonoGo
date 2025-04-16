using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGo.Tiled.MapStructure.Objects;

namespace MonoGo.Tiled.MapStructure
{
    /// <summary>
    /// Represents a single tile in a Tiled tileset.
    /// </summary>
    public struct TiledMapTilesetTile
    {
        /// <summary>
        /// Gets or sets the global ID (GID) of the tile.
        /// </summary>
        public int GID;

        /// <summary>
        /// Gets or sets the texture ID of the tile within the tileset.
        /// </summary>
        public int TextureID;

        /// <summary>
        /// Gets the texture of the tile.
        /// </summary>
        public Texture2D Texture
        {
            get
            {
                if (Tileset == null)
                {
                    return null;
                }
                else
                {
                    return Tileset.Textures[TextureID];
                }
            }
        }

        /// <summary>
        /// Gets or sets the tileset to which this tile belongs.
        /// </summary>
        public TiledMapTileset Tileset;

        /// <summary>
        /// Gets or sets the position of the tile within the texture.
        /// </summary>
        public Rectangle TexturePosition;

        /// <summary>
        /// Gets or sets the drawing order of objects associated with this tile.
        /// </summary>
        public TiledMapObjectDrawingOrder ObjectsDrawingOrder;

        /// <summary>
        /// Gets or sets the objects associated with this tile.
        /// </summary>
        public TiledObject[] Objects;

        /// <summary>
        /// Gets or sets the custom properties of the tile.
        /// </summary>
        public Dictionary<string, string> Properties;
    }
}
