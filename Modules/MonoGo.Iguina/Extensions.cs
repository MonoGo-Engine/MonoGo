using Iguina.Defs;

namespace MonoGo.Iguina
{
    /// <summary>
    /// Provides extension methods to simplify working with the Iguina UI-System.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Sets the value of a <see cref="Measurement"/> in pixels.
        /// </summary>
        /// <param name="measurement">The measurement to modify (e.g. Offset).</param>
        /// <param name="value">The new value in pixels (could be a fraction of Offset).</param>
        public static void SetPixels(this ref Measurement measurement, float value)
        {
            measurement.Value = value;
            measurement.Units = MeasureUnit.Pixels;
        }
    }
}
