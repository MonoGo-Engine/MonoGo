using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGo.Tiled.MapStructure
{
    /// <summary>
    /// Represents the data structure for a Tiled map.
    /// </summary>
    public class TiledMap
    {
        /// <summary>
        /// Gets or sets the name of the Tiled map.
        /// </summary>
        public string Name;

        /// <summary>
        /// Gets or sets the background color of the map.
        /// </summary>
        public Color? BackgroundColor;

        /// <summary>
        /// Gets or sets the width of the map in tiles.
        /// </summary>
        public int Width;

        /// <summary>
        /// Gets or sets the height of the map in tiles.
        /// </summary>
        public int Height;

        /// <summary>
        /// Gets or sets the width of a single tile in pixels.
        /// </summary>
        public int TileWidth;

        /// <summary>
        /// Gets or sets the height of a single tile in pixels.
        /// </summary>
        public int TileHeight;

        /// <summary>
        /// Gets or sets the render order of the tiles in the map.
        /// </summary>
        public RenderOrder RenderOrder;

        /// <summary>
        /// Gets or sets the orientation of the map (e.g., orthogonal, isometric).
        /// </summary>
        public Orientation Orientation;

        /// <summary>
        /// Gets or sets the stagger axis for staggered or hexagonal maps.
        /// </summary>
        public StaggerAxis StaggerAxis = StaggerAxis.None;

        /// <summary>
        /// Gets or sets the stagger index for staggered or hexagonal maps.
        /// </summary>
        public StaggerIndex StaggerIndex = StaggerIndex.None;

        /// <summary>
        /// Gets or sets the side length of hexagonal tiles, if applicable.
        /// </summary>
        public int HexSideLength;

        /// <summary>
        /// Gets or sets the tilesets used in the map.
        /// </summary>
        public TiledMapTileset[] Tilesets;

        /// <summary>
        /// Gets or sets the tile layers in the map.
        /// </summary>
        public TiledMapTileLayer[] TileLayers;

        /// <summary>
        /// Gets or sets the object layers in the map.
        /// </summary>
        public TiledMapObjectLayer[] ObjectLayers;

        /// <summary>
        /// Gets or sets the image layers in the map.
        /// </summary>
        public TiledMapImageLayer[] ImageLayers;

        /// <summary>
        /// Gets or sets the custom properties of the map.
        /// </summary>
        public Dictionary<string, string> Properties;

        /// <summary>
        /// Retrieves a tileset tile by its global ID (GID).
        /// </summary>
        /// <param name="gid">The global ID of the tile.</param>
        /// <returns>The corresponding <see cref="TiledMapTilesetTile"/>, or <c>null</c> if not found.</returns>
        public TiledMapTilesetTile? GetTilesetTile(int gid)
        {
            var tileset = GetTileset(gid);
            if (tileset != null)
            {
                return tileset.Tiles[gid - tileset.FirstGID];
            }
            return null;
        }

        /// <summary>
        /// Retrieves the tileset that contains a tile with the specified global ID (GID).
        /// </summary>
        /// <param name="gid">The global ID of the tile.</param>
        /// <returns>The corresponding <see cref="TiledMapTileset"/>, or <c>null</c> if not found.</returns>
        public TiledMapTileset GetTileset(int gid)
        {
            foreach (var tileset in Tilesets)
            {
                if (gid >= tileset.FirstGID && gid < tileset.FirstGID + tileset.Tiles.Length)
                {
                    return tileset;
                }
            }
            return null;
        }
    }
}
