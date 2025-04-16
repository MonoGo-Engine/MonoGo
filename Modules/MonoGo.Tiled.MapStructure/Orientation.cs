
namespace MonoGo.Tiled.MapStructure
{
    /// <summary>
    /// Specifies the orientation of a Tiled map.
    /// </summary>
    public enum Orientation : byte
    {
        /// <summary>
        /// Orthogonal orientation (grid-based).
        /// </summary>
        Orthogonal = 0,

        /// <summary>
        /// Isometric orientation (diamond-shaped tiles).
        /// </summary>
        Isometric = 1,

        /// <summary>
        /// Staggered orientation (hexagonal or staggered isometric tiles).
        /// </summary>
        Staggered = 2,

        /// <summary>
        /// Hexagonal orientation (hexagonal tiles).
        /// </summary>
        Hexagonal = 3
    }
}