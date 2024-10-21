using Microsoft.Xna.Framework;

namespace MonoGo.Engine
{
    /// <summary>
    /// Make your object movable by adding a <see cref="Vector2"/> property.
    /// </summary>
    public interface IAmMovable
    {
        public Vector2 Position { get; set; }
    }
}
