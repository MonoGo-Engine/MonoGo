using Iguina.Defs;
using Iguina.Entities;
using MonoGo.Engine;
using MonoGo.Engine.SceneSystem;

namespace MonoGo.Iguina
{
    /// <summary>
    /// Represents a base class for GUI entities that manage UI elements within a <see cref="Engine.EC.Entity"/>.
    /// </summary>
    public abstract class GUIEntity : Engine.EC.Entity, IHaveGUI
    {
        /// <summary>
        /// Gets or sets the root panel that contains all the added UI entities.
        /// </summary>
        public Panel GUIRootPanel { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIEntity"/> class.
        /// </summary>
        /// <param name="layer">The layer to which this GUI entity belongs.</param>
        public GUIEntity(Layer layer) : base(layer) { }

        /// <summary>
        /// Adds a control to the GUIRootPanel of this <see cref="GUIEntity"/> class.
        /// </summary>
        /// <param name="control">The control to add.</param>
        public void AddGUIEntity(Entity control)
        {
            GUIRootPanel.AddChild(control);
        }

        /// <summary>
        /// Creates the UI for this GUI entity. Override this method to add custom controls.
        /// </summary>
        /// <remarks>Add UI-Controls via <see cref="AddGUIEntity"/>.</remarks>
        public virtual void CreateUI()
        {
            Clear();

            GUIRootPanel = GUIMgr.System.Root.AddChild(new Panel(GUIMgr.System, null!)
            {
                Identifier = $"Owner:{GetType().Name}",
                Anchor = Anchor.Center,
                UserData = this
            });
            GUIRootPanel.Size.SetPercents(100, 100);
            GUIRootPanel.IgnoreInteractions = true;
        }

        /// <summary>
        /// Enables or disables the GUI root owner panel.
        /// </summary>
        /// <param name="enable">A value indicating whether to enable the panel.</param>
        void IHaveGUI.Enable(bool enable)
        {
            if (GUIRootPanel != null) GUIRootPanel.Enabled = enable;
        }

        /// <summary>
        /// Makes the GUI root owner panel visible or invisible.
        /// </summary>
        /// <param name="visible">A value indicating whether to make the panel visible.</param>
        void IHaveGUI.Visible(bool visible)
        {
            if (GUIRootPanel != null) GUIRootPanel.Visible = visible;
        }

        /// <summary>
        /// Clears the GUI root and removes all its contents.
        /// </summary>
        void IHaveGUI.Clear()
        {
            Clear();
        }

        /// <summary>
        /// Removes the GUI root panel and all its children.
        /// </summary>
        private void Clear()
        {
            GUIRootPanel?.RemoveSelf();
        }
    }
}
