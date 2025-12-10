using Microsoft.Xna.Framework;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.EC;
using MonoGo.Engine.Resources;
using MonoGo.Engine.SceneSystem;
using MonoGo.MercuryParticleEngine;
using MonoGo.MercuryParticleEngine.Modifiers;
using MonoGo.MercuryParticleEngine.Profiles;
using MonoGo.Samples.Misc;
using System;

namespace MonoGo.Samples.Demos
{
    public class ParticlesDemo : Entity
    {
        public static readonly string Description = "Move > ${FC:FFDB5F}WASD${RESET}";

        public ParticlesDemo(Layer layer) : base(layer)
        {
            layer.DepthSorting = true;

            var particleEffect = new ParticleEffect
            {
                Name = "Potpourri",
                Emitters = new[]
                {
                    new Emitter(1000, TimeSpan.FromSeconds(4), new PointProfile())
                    {
                        Sprite = ResourceHub.GetResource<Sprite>("ParticleSprites", "Pixel"),
                        Loop = true,
                        Name = "Splash",
                        Modifiers =
                        [
                            new FollowPositionModifier()
                            {
                                Inside = true,
                                Speed = 1f,
                                Offset = Vector2.Zero
                            }
                        ]
                    }
                }
            };

            var cCustomParticleEffect = new CustomParticleEffectComponent(particleEffect)
            {
                Depth = 1
            };

            var player = new Player(layer, new Vector2(400, 300));
            player.AddComponentToTop(cCustomParticleEffect.ParticleEffectComponent);
            player.AddComponent(cCustomParticleEffect);
            cCustomParticleEffect.ParticleEffectComponent.AttractParticlesTo(player.GetComponent<PositionComponent>());

            new ParticleEditorEntity(layer);
        }
    }
}
