
namespace MonoGo.Tiled.MapStructure
{
    /// <summary>
    /// Specifies the stagger axis for staggered or hexagonal Tiled maps.
    /// </summary>
    public enum StaggerAxis : byte
    {
        /// <summary>
        /// No stagger axis.
        /// </summary>
        None = 0,

        /// <summary>
        /// Stagger along the X-axis.
        /// </summary>
        X = 1,

        /// <summary>
        /// Stagger along the Y-axis.
        /// </summary>
        Y = 2
    }
}
