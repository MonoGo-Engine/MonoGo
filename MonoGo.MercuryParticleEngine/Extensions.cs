using Microsoft.Xna.Framework;
using MonoGo.Engine;

namespace MonoGo.MercuryParticleEngine
{
    public static class Extensions
    {
        /// <summary>
        /// Copies the X and Y components of the vector to the specified memory location.
        /// </summary>
        /// <param name="value">The value of the Vector2 coordinate.</param>
        /// <param name="destination">The memory location to copy the coordinate to.</param>
        public static unsafe void CopyTo(this Vector2 value, float* destination)
        {
            destination[0] = value.X;
            destination[1] = value.Y;
        }
    }
}
