
namespace MonoGo.Tiled.MapStructure
{
    /// <summary>
    /// Represents a single tile in a Tiled map.
    /// </summary>
    public struct TiledMapTile
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
        /// Gets or sets a value indicating whether the tile is flipped diagonally.
        /// </summary>
        /// <remarks>
        /// Diagonal flipping is a combination of rotation and mirroring.
        /// </remarks>
        /// <example>
        /// <code>
        /// 0 0 ' 0 0     0 0 ' 0 0
        /// 0 ' ' 0 0 ==> 0 ' 0 0 '
        /// ' 0 ' 0 0 ==> ' ' ' ' '
        /// 0 0 ' 0 0 ==> 0 0 0 0 '
        /// 0 ' ' ' 0     0 0 0 0 0
        /// 
        /// Vertices:
        /// 0 1 => 0 2
        /// 2 3 => 1 3
        /// 
        /// (90 deg rotation) + (-1 xscale)
        /// </code>
        /// </example>
        public bool FlipDiag;

        /// <summary>
        /// Gets a value indicating whether the tile is blank (no GID assigned).
        /// </summary>
        public bool IsBlank => GID == 0;
    }
}
