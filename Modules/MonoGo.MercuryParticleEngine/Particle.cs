using Microsoft.Xna.Framework;
using MonoGo.Engine;
using System.Runtime.InteropServices;

namespace MonoGo.MercuryParticleEngine
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Particle {
        public float Inception;
        public float Age;
        public Vector2 Position;
        public Vector2 TriggerPos;
        public Vector2 Velocity;
        public HSLColor Colour;
        public float Opacity;
        public Vector2 Scale;
        public float Rotation;
        public float Mass;

        public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Particle));
    }
}