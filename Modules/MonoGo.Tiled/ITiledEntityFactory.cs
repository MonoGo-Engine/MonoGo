using MonoGo.Engine.EC;
using MonoGo.Engine.SceneSystem;
using MonoGo.Tiled.MapStructure.Objects;

namespace MonoGo.Tiled
{
    /// <summary>
    /// Defines a factory interface for creating entities from Tiled object structures.
    /// </summary>
    public interface ITiledEntityFactory
    {
        /// <summary>
        /// Gets the identifying tag for the factory.
        /// </summary>
        /// <remarks>
        /// All factory tags must be unique to ensure proper identification.
        /// </remarks>
        string Tag { get; }

        /// <summary>
        /// Creates an entity from a Tiled object on a specified layer.
        /// </summary>
        /// <param name="obj">The Tiled object to convert into an entity.</param>
        /// <param name="layer">The layer where the entity will be placed.</param>
        /// <param name="map">The map builder instance used for additional context.</param>
        /// <returns>The created entity.</returns>
        Entity Make(TiledObject obj, Layer layer, MapBuilder map);
    }
}
