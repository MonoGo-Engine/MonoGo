using Iguina.Defs;
using Iguina.Entities;
using MonoGo.Engine;
using MonoGo.Engine.SceneSystem;

namespace MonoGo.Iguina
{
    public abstract class GUIEntity : Engine.EC.Entity, IHaveGUI
    {
        /// <summary>
        /// This panel Contains all the added UI entities.
        /// </summary>
        public Panel GUIRootPanel { get; internal set; }

        public GUIEntity(Layer layer) : base(layer) { }
        
        /// <summary>
        /// Adds a control to the specified owner, a different owner or the root itself if there is no other UI owner.
        /// </summary>
        /// <param name="control">Control to add.</param>
        public void AddGUIEntity(Entity control)
        {
            GUIRootPanel.AddChild(control);
        }

        /// <summary>
        /// Override and call <see cref="AddGUIEntity"/> to add controls to this GUI root.
        /// </summary>
        public virtual void CreateUI()
        {
            Clear();

            GUIRootPanel = GUIMgr.System.Root.AddChild(new Panel(GUIMgr.System, null!) { Identifier = $"Owner:{GetType().Name}", Anchor = Anchor.Center, UserData = this });
            GUIRootPanel.Size.SetPercents(100, 100);
            GUIRootPanel.IgnoreInteractions = true;
        }

        /// <summary>
        /// Enable or disable the GUI root owner panel.
        /// </summary>
        void IHaveGUI.Enable(bool enable)
        {
            if (GUIRootPanel != null) GUIRootPanel.Enabled = enable;
        }

        /// <summary>
        /// Make the GUI root owner panel visible or invisible.
        /// </summary>
        void IHaveGUI.Visible(bool visible)
        {
            if (GUIRootPanel != null) GUIRootPanel.Visible = visible;
        }

        void IHaveGUI.Clear()
        {
            Clear();
        }
        void Clear()
        {
            GUIRootPanel?.RemoveSelf();
        }
    }
}
