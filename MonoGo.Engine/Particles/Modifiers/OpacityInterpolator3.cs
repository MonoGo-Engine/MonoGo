namespace MonoGo.Engine.Particles.Modifiers 
{
    /// <summary>
    /// Defines a modifier which interpolates the opacity of a particle over the course of its lifetime,
    /// with a middle opacity value for smoother transitions.
    /// </summary>
    public class OpacityInterpolator3 : IModifier
    {
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the initial opacity of particles when they are released.
        /// </summary>
        public float InitialOpacity { get; set; }

        /// <summary>
        /// Gets or sets the middle opacity of particles at the halfway point of their lifetime.
        /// </summary>
        public float MiddleOpacity { get; set; }

        /// <summary>
        /// Gets or sets the final opacity of particles when they are retired.
        /// </summary>
        public float FinalOpacity { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                
                // Calculate opacity based on particle age
                if (particle->Age < 0.5f)
                {
                    // First half of lifetime: interpolate from Initial to Middle
                    float normalizedAge = particle->Age * 2.0f; // Scale age to 0-1 for first half
                    float delta = MiddleOpacity - InitialOpacity;
                    particle->Opacity = delta * normalizedAge + InitialOpacity;
                }
                else
                {
                    // Second half of lifetime: interpolate from Middle to Final
                    float normalizedAge = (particle->Age - 0.5f) * 2.0f; // Scale age to 0-1 for second half
                    float delta = FinalOpacity - MiddleOpacity;
                    particle->Opacity = delta * normalizedAge + MiddleOpacity;
                }
            }
        }
    }
}