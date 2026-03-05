using Iguina.Defs;
using Iguina.Entities;
using Microsoft.Xna.Framework;
using MonoGo.Engine.SceneSystem;
using MonoGo.Iguina;
using MonoGo.MercuryParticleEngine;
using MonoGo.MercuryParticleEngine.Modifiers;
using System.Linq;
using Point = Iguina.Defs.Point;

namespace MonoGo.Samples.Misc
{
    public class ParticleEditorEntity : GUIEntity
    {
        public ParticleEffectComponent ParticleEffectComponent { get; set; }

        public ParticleEditorEntity(Layer layer) : base(layer)
        {
            Visible = false;
        }

        public void OffsetX(float value)
        {
            ParticleEffectComponent.ParticleEffect.Modifiers<FollowPositionModifier>().ToList()
                .ForEach(x => x.Offset = new Vector2(value, x.Offset.Y));
        }

        public void OffsetY(float value)
        {
            ParticleEffectComponent.ParticleEffect.Modifiers<FollowPositionModifier>().ToList()
                .ForEach(x => x.Offset = new Vector2(x.Offset.X, value));
        }

        public void Speed(float value)
        {
            ParticleEffectComponent.ParticleEffect.Modifiers<FollowPositionModifier>().ToList()
                .ForEach(x => x.Speed = value);
        }

        public void ToggleInside()
        {
            var player = Layer.FindEntity<Player>();
            player.InsideParticles = !player.InsideParticles;

            ParticleEffectComponent.ParticleEffect.Modifiers<FollowPositionModifier>().ToList()
                .ForEach(x => x.Inside = player.InsideParticles);
        }

        private void ToggleAttract()
        {
            var player = Layer.FindEntity<Player>();
            player.AttractParticles = !player.AttractParticles;

            CheckParticleAttraction(player);
        }

        private void CheckParticleAttraction(Player player)
        {
            if (player.AttractParticles)
            {
                ParticleEffectComponent.AttractParticlesTo(player.GetComponent<PositionComponent>());
            }
            else
            {
                ParticleEffectComponent.AttractParticlesTo(null);
            }
        }

        public override void CreateUI()
        {
            base.CreateUI();

            var player = Layer.FindEntity<Player>();
            ParticleEffectComponent = player.GetComponent<ParticleEffectComponent>();
            CheckParticleAttraction(player);

            // Controls
            var firstPanel = new Panel(GUIMgr.System, null!)
            {
                Identifier = "extra",
                Anchor = Anchor.AutoLTR,
                AutoHeight = false,
                OverflowMode = OverflowMode.AllowOverflow
            };
            firstPanel.Size.X.SetPercents(100);
            firstPanel.Size.Y.SetPixels(64);
            SceneSwitcher.DescriptionPanel.AddChild(firstPanel);
            {
                var checkbox = new Checkbox(GUIMgr.System, "Enabled") { Anchor = Anchor.AutoLTR, Checked = ParticleEffectComponent.Enabled };
                checkbox.Size.X.SetPercents(20);
                checkbox.Events.OnValueChanged = (Entity control) =>
                {
                    ParticleEffectComponent.Enabled = !ParticleEffectComponent.Enabled;
                };
                firstPanel.AddChild(checkbox);
            }
            {
                var checkbox = new Checkbox(GUIMgr.System, "Visible") { Anchor = Anchor.AutoInlineLTR, Checked = ParticleEffectComponent.Visible };
                checkbox.Size.X.SetPercents(20);
                checkbox.Events.OnValueChanged = (Entity control) =>
                {
                    ParticleEffectComponent.Visible = !ParticleEffectComponent.Visible;
                };
                firstPanel.AddChild(checkbox);
            }
            {
                var checkbox = new Checkbox(GUIMgr.System, "Follow") { Anchor = Anchor.AutoInlineLTR, Checked = ParticleEffectComponent.FollowOwner };
                checkbox.Size.X.SetPercents(20);
                checkbox.Events.OnValueChanged = (Entity control) =>
                {
                    ParticleEffectComponent.ToggleFollowOwner();
                };
                firstPanel.AddChild(checkbox);
            }
            {
                var checkbox = new Checkbox(GUIMgr.System, "Attract") { Anchor = Anchor.AutoInlineLTR, Checked = player.AttractParticles };
                checkbox.Size.X.SetPercents(20);
                checkbox.Events.OnValueChanged = (Entity control) =>
                {
                    ToggleAttract();
                };
                firstPanel.AddChild(checkbox);
            }
            {
                var checkbox = new Checkbox(GUIMgr.System, "Inside") { Anchor = Anchor.AutoInlineLTR, Checked = player.InsideParticles };
                checkbox.Size.X.SetPercents(20);
                checkbox.Events.OnValueChanged = (Entity control) =>
                {
                    ToggleInside();
                };
                firstPanel.AddChild(checkbox);
            }

            // OFFSET, SPEED
            // left panel
            {
                var panel = new Panel(GUIMgr.System, null!)
                {
                    Identifier = "extra",
                    Anchor = Anchor.AutoLTR
                };
                panel.OverrideStyles.MarginBefore = Point.Zero;
                panel.Size.Y.SetPixels(72);
                panel.Size.X.SetPercents(33);
                SceneSwitcher.DescriptionPanel.AddChild(panel);

                var numinput = new NumericInput(GUIMgr.System);
                numinput.Size.Y.SetPixels(64);
                numinput.Events.OnValueChanged = (Entity control) =>
                {
                    OffsetX((float)numinput.NumericValue);
                };
                panel.AddChild(numinput);
                panel.AddChild(new Label(GUIMgr.System, "Offset: X"));
            }
            // center panel
            {
                var panel = new Panel(GUIMgr.System, null!)
                {
                    Identifier = "extra",
                    Anchor = Anchor.AutoInlineLTR
                };
                panel.Size.Y.SetPixels(72);
                panel.Size.X.SetPercents(33);
                SceneSwitcher.DescriptionPanel.AddChild(panel);

                var numinput = new NumericInput(GUIMgr.System)
                {
                    Anchor = Anchor.AutoInlineLTR
                };
                numinput.Size.Y.SetPixels(64);
                numinput.Events.OnValueChanged = (Entity control) =>
                {
                    OffsetY((float)numinput.NumericValue);
                };
                panel.AddChild(numinput);
                panel.AddChild(new Label(GUIMgr.System, "Offest:Y"));
            }
            // right panel
            {
                var panel = new Panel(GUIMgr.System, null!)
                {
                    Identifier = "extra",
                    Anchor = Anchor.AutoInlineLTR
                };
                panel.Size.Y.SetPixels(72);
                panel.Size.X.SetPercents(33);
                SceneSwitcher.DescriptionPanel.AddChild(panel);

                var numinput = new NumericInput(GUIMgr.System)
                {
                    Anchor = Anchor.AutoInlineLTR
                };
                numinput.Size.Y.SetPixels(64);
                numinput.Events.OnValueChanged = (Entity control) =>
                {
                    Speed((float)numinput.NumericValue);
                };
                panel.AddChild(numinput);
                panel.AddChild(new Label(GUIMgr.System, "Speed"));
            }
        }
    }
}
