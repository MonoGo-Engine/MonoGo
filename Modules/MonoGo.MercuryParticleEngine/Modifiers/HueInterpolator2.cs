using MonoGo.Engine;

namespace MonoGo.MercuryParticleEngine.Modifiers
{
    public class HueInterpolator2 : IModifier
    {
        public string Key { get; set; }

        public float InitialHue { get; set; }
        public float FinalHue { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            var delta = FinalHue - InitialHue;

            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                particle->Colour = new HSLColor(
                    delta * particle->Age + InitialHue,
                    particle->Colour.S,
                    particle->Colour.L);
            }
        }
    }
}