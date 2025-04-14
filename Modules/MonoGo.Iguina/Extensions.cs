using Iguina.Defs;

namespace MonoGo.Iguina
{
    /// <summary>
    /// Extensions for making it easier to work with Iguina.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Enables smooth values for animations (tween, lerp).
        /// </summary>
        /// <param name="measurement">E.g.: Offset.</param>
        /// <param name="value">New value (E.g.: possibly a fraction of Offset).</param>
        public static void SetPixels(this ref Measurement measurement, float value)
        {
            measurement.Value = value;
            measurement.Units = MeasureUnit.Pixels;
        }
    }
}
