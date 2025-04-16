using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGo.Tiled.MapStructure.Objects
{
    /// <summary>
    /// Represents a base Tiled object.
    /// </summary>
    public class TiledObject
    {
        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        public string Name;

        /// <summary>
        /// Gets or sets the type of the object.
        /// </summary>
        public string Type;

        /// <summary>
        /// Gets or sets the unique ID of the object.
        /// </summary>
        public int ID;

        /// <summary>
        /// Gets or sets the position of the object in pixels.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// Gets or sets the size of the object in pixels.
        /// </summary>
        public Vector2 Size;

        /// <summary>
        /// Gets or sets the rotation of the object in degrees.
        /// </summary>
        public float Rotation;

        /// <summary>
        /// Gets or sets a value indicating whether the object is visible.
        /// </summary>
        public bool Visible;

        /// <summary>
        /// Gets or sets the custom properties of the object.
        /// </summary>
        public Dictionary<string, string> Properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledObject"/> class.
        /// </summary>
        public TiledObject() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledObject"/> class by copying another object.
        /// </summary>
        /// <param name="obj">The object to copy.</param>
        public TiledObject(TiledObject obj)
        {
            Name = obj.Name;
            Type = obj.Type;
            ID = obj.ID;
            Position = obj.Position;
            Size = obj.Size;
            Rotation = obj.Rotation;
            Visible = obj.Visible;
            Properties = new Dictionary<string, string>(obj.Properties);
        }
    }
}
