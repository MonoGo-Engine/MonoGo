using Microsoft.Xna.Framework;
using System;

namespace MonoGo.Engine
{
    /// <summary>
    /// Common extensions used by the MonoGo Engine.
    /// Probably useful for you as well.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Get the next Enum out of an array of enums.
        /// </summary>
        /// <typeparam name="T">T needs to be an Enum.</typeparam>
        /// <param name="currentValue">The current enum value.</param>
        /// <returns>The next enum value of the enum array.</returns>
        public static T Next<T>(this T currentValue) where T : Enum
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            int index = Array.IndexOf(values, currentValue);

            if (index == values.Length - 1)
            {
                return values[0];
            }

            return values[index + 1];
        }

        /// <summary>
        /// Get the previous Enum out of an array of enums.
        /// </summary>
        /// <typeparam name="T">T needs to be an Enum.</typeparam>
        /// <param name="currentValue">The current enum value.</param>
        /// <returns>The previous enum value of the enum array.</returns>
        public static T Previous<T>(this T currentValue) where T : Enum
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            int index = Array.IndexOf(values, currentValue);

            if (index == 0)
            {
                return values[^1];
            }

            return values[index - 1];
        }

        /// <summary>
        /// Gets the Vector2 of the Axis.
        /// </summary>
        public static Vector2 ToVector2(this Axis value, float magnitude = 1.0f)
        {
            return new Vector2(value.X * magnitude, value.Y * magnitude);
        }

        /// <summary>
        /// Gets the Axis of the Vector2.
        /// </summary>
        public static Axis ToAxis(this Vector2 value)
        {
            return new Axis(value.X, value.Y);
        }
    }
}
