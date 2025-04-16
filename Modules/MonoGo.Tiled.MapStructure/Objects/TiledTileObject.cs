
namespace MonoGo.Tiled.MapStructure.Objects
{
    /// <summary>
    /// Represents a tile object in a Tiled map.
    /// </summary>
    public class TiledTileObject : TiledObject
    {
        /// <summary>
        /// Gets or sets the global ID (GID) of the tile.
        /// </summary>
        public int GID;

        /// <summary>
        /// Gets or sets a value indicating whether the tile is flipped horizontally.
        /// </summary>
        public bool FlipHor;

        /// <summary>
        /// Gets or sets a value indicating whether the tile is flipped vertically.
        /// </summary>
        public bool FlipVer;

        /// <summary>
        /// Gets or sets the tileset to which the tile belongs.
        /// </summary>
        public TiledMapTileset Tileset;

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledTileObject"/> class.
        /// </summary>
        public TiledTileObject() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledTileObject"/> class by copying another object.
        /// </summary>
        /// <param name="obj">The object to copy.</param>
        public TiledTileObject(TiledObject obj) : base(obj) { }
    }
}
