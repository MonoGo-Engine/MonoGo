
namespace MonoGo.Tiled.MapStructure
{
    /// <summary>
    /// Specifies the stagger index for staggered or hexagonal Tiled maps.
    /// </summary>
    public enum StaggerIndex : byte
    {
        /// <summary>
        /// No stagger index.
        /// </summary>
        None = 0,

        /// <summary>
        /// Stagger on odd rows or columns.
        /// </summary>
        Odd = 1,

        /// <summary>
        /// Stagger on even rows or columns.
        /// </summary>
        Even = 2
    }
}
