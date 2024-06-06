﻿using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGo.Engine.Particles.Profiles;
using MonoGo.Engine.Particles.Modifiers;
using System.Text.Json.Serialization;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.Utils;

namespace MonoGo.Engine.Particles
{
    public unsafe class Emitter : IDisposable
    {
        public Emitter(int capacity, TimeSpan term, Profile profile) {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            _term = (float)term.TotalSeconds;
            _MaxTime = _CurrentTime = (float)term.TotalSeconds * 60f;
            GetTerm = term;

            _capacity = capacity;
            GetCapacity = capacity;

            Buffer = new ParticleBuffer(capacity);
            Offset = new Vector();
            Profile = profile;
            Modifiers = new IModifier[0];
            ModifierExecutionStrategy = ModifierExecutionStrategy.Serial;
            Parameters = new ReleaseParameters();
        }

        [JsonPropertyName("Capacity")]
        public int GetCapacity { get; set; }
        private readonly int _capacity;

        [JsonPropertyName("Term")]
        public TimeSpan GetTerm { get; set; }
        private readonly float _term;
        internal float _MaxTime, _CurrentTime;

        public bool Loop 
        {
            get { return _Loop; }
            set
            {
                _Loop = value;
                if (_Loop)
                {
                    if (_CurrentTime == 0)
                    {
                        _CurrentTime = _MaxTime;
                        StopEmitting = false;
                    }
                }
            }
        }
        private bool _Loop;

        [JsonIgnore]
        public bool Draw_b { get; set; } = true;

        [JsonIgnore]
        public bool StopEmitting { get; set; } = false;

        private float _totalSeconds;
        internal readonly ParticleBuffer Buffer;

        [JsonIgnore]
        public int ActiveParticles => Buffer.Count;

        public string Name { get; set; }
        public Vector Offset { get; set; }
        public IModifier[] Modifiers { get; set; }
        public Profile Profile { get; }
        public ReleaseParameters Parameters { get; set; }
        public BlendMode BlendMode { get; set; }
        public float LayerDepth { get; set; }
        public string TextureKey { get; set; }

        [JsonIgnore]
        public Sprite Sprite { get; set; }

        //[JsonIgnore]
        //TODO: Implement SpriteSheet Animations
        //public Animation Animation { get; set; }

        public ModifierExecutionStrategy ModifierExecutionStrategy { get; set; }

        public void UpdateModifierReferences(ref object _object)
        {
            foreach (IModifier modifier in Modifiers) modifier.UpdateReferences(ref _object);
        }

        public override string ToString()
        {
            return Name;
        }

        public Emitter Clone()
        {
            Emitter emitter = new Emitter(Buffer.Size, GetTerm, Profile);
            emitter.BlendMode = BlendMode;
            emitter.ModifierExecutionStrategy = ModifierExecutionStrategy;
            emitter.Modifiers = Modifiers;
            emitter.Name = Name;
            emitter.Offset = Offset;
            emitter.Parameters = Parameters;
            emitter.StopEmitting = StopEmitting;
            emitter.Loop = Loop;
            emitter.Sprite = Sprite;
            emitter.TextureKey = TextureKey;

            return emitter;
        }

        private void ReclaimExpiredParticles()
        {
            var iterator = Buffer.Iterator;

            var expired = 0;

            while (iterator.HasNext)
            {
                var particle = iterator.Next();

                if (_totalSeconds - particle->Inception < _term)
                    break;

                expired++;
            }
            if (expired != 0)
                Buffer.Reclaim(expired);
        }

        public void Update(float elapsedSeconds) 
        {
            if (!Loop)
            {
                if (_CurrentTime <= 0) StopEmitting = true;
                else _CurrentTime--;
            }

            _totalSeconds += elapsedSeconds;

            if (Buffer.Count == 0)
            {
                return;
            }

            //Uncomment the next line to make the particles stop getting reset
            ReclaimExpiredParticles();

            var iterator = Buffer.Iterator;

            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                particle->Age = (_totalSeconds - particle->Inception) / _term;
                particle->Position = particle->Position + particle->Velocity * elapsedSeconds;
            }

            ModifierExecutionStrategy.ExecuteModifiers(Modifiers, elapsedSeconds, iterator);
        }

        public void Trigger(Vector position)
        {
            if (Parameters != null && !StopEmitting)
            {
                var numToRelease = FastRand.NextInteger(Parameters.Quantity);
                Release(position + Offset, numToRelease);
            }
        }

        public void Trigger(LineSegment line)
        {
            if (!StopEmitting)
            {
                var numToRelease = FastRand.NextInteger(Parameters.Quantity);
                var lineVector = line.ToVector();

                for (var i = 0; i < numToRelease; i++)
                {
                    var offset = lineVector * FastRand.NextSingle();
                    Release(line.Origin + offset, 1);
                }
            }
        }

        private void Release(Vector position, int numToRelease)
        {
            var iterator = Buffer.Release(numToRelease);

            while (iterator.HasNext)
            {
                var particle = iterator.Next();

                Axis heading;
                Profile.GetOffsetAndHeading(out particle->Position, out heading);

                particle->Age = 0f;
                particle->Inception = _totalSeconds;

                particle->Position += position;

                particle->TriggerPos = position;

                var speed = FastRand.NextSingle(Parameters.Speed);

                particle->Velocity = heading * speed;

                FastRand.NextColour(out particle->Colour, Parameters.Colour);

                particle->Opacity  = FastRand.NextSingle(Parameters.Opacity);
                var scale = FastRand.NextSingle(Parameters.Scale);
                particle->Scale    = new Vector(scale, scale);
                particle->Rotation = FastRand.NextSingle(Parameters.Rotation);
                particle->Mass     = FastRand.NextSingle(Parameters.Mass);
            }
        }

        public unsafe void Draw(BasicEffect effect = null)
        {
            if (Draw_b)
            {
                var blendState = BlendMode == BlendMode.Add
                    ? BlendState.Additive
                    : BlendState.AlphaBlend;

                //TODO var sortMode = emitter.RenderingOrder == RenderingOrder.BackToFront ?

                var iterator = Buffer.Iterator;

                while (iterator.HasNext)
                {
                    var particle = iterator.Next();

                    var color = particle->Colour.ToColor();
                    if (blendState == BlendState.AlphaBlend)
                        color *= particle->Opacity;
                    else
                        color.A = (byte)(particle->Opacity * 255);
                                        
                    Sprite.Draw(
                        new Vector2(particle->Position.X, particle->Position.Y),
                        0,
                        new Vector2(particle->Scale.X, particle->Scale.Y),
                        Angle.FromRadians(particle->Rotation),
                        color);
                }
            }
        }

        public void Dispose()
        {
            Buffer?.Dispose();
            GC.SuppressFinalize(this);
        }

        ~Emitter() {
            Dispose();
        }
    }
}
