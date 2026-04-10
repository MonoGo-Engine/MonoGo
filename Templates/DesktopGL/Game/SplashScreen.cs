using Iguina.Defs;
using Iguina.Entities;
using MonoGo.Engine.SceneSystem;
using MonoGo.Iguina;
using System.Reflection;

namespace MGNamespace
{
    public class SplashScreen : GUIEntity
    {
        public SplashScreen() : base(SceneMgr.DefaultLayer)
        {
        }

        public override void CreateUI()
        {
            base.CreateUI();

            var panel = new Panel(GUIMgr.System, null!)
            {
                Anchor = Anchor.Center,
                AutoHeight = true,
                OverflowMode = OverflowMode.HideOverflow
            };
            AddGUIEntity(panel);

            var welcomeText = new Paragraph(GUIMgr.System, @$"MonoGo Engine ${{FC:FFDB5F}}v.{Assembly.GetAssembly(typeof(MonoGo.Engine.EC.Entity)).GetName().Version}${{RESET}}");
            welcomeText.OverrideStyles.FontSize = 28;
            panel.AddChild(welcomeText);
        }
    }
}
