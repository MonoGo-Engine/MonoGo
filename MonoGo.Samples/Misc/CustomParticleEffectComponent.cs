using Iguina.Defs;
using Iguina.Entities;
using MonoGo.Engine;
using MonoGo.Iguina;
using MonoGo.MercuryParticleEngine;

namespace MonoGo.Samples.Misc
{
    public class CustomParticleEffectComponent : GUIComponent
    {
        public ParticleEffectComponent ParticleEffectComponent;
        public ParticleEffect ParticleEffect;

        private Paragraph _activeParticlesParagraph;

        public CustomParticleEffectComponent(ParticleEffect particleEffect)
        {
            ParticleEffectComponent = new ParticleEffectComponent(particleEffect, GameMgr.WindowManager.CanvasCenter);
            ParticleEffect = particleEffect;
        }

        public override void Update()
        {
            base.Update();

            if (_activeParticlesParagraph != null)
            {
                _activeParticlesParagraph.Text =
                    "Active Particles:${FC:FFDB5F} " + ParticleEffect.ActiveParticles + "${RESET}";
            }
        }

        public override void CreateUI()
        {
            base.CreateUI();

            var topPanel = new Panel(GUIMgr.System, null!)
            {
                Anchor = Anchor.TopCenter,
                IgnoreInteractions = true
            };
            topPanel.Size.Y.SetPixels(60);
            AddGUIEntity(topPanel);

            _activeParticlesParagraph = new Paragraph(GUIMgr.System, "")
            {
                Anchor = Anchor.Center
            };
            topPanel.AddChild(_activeParticlesParagraph);
        }
    }
}
