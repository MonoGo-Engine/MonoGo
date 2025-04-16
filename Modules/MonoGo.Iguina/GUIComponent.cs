using Iguina.Defs;
using Iguina.Entities;
using MonoGo.Engine;
using MonoGo.Engine.EC;
using Entity = Iguina.Entities.Entity;

namespace MonoGo.Iguina
{
    /// <summary>
    /// Represents a base class for GUI components that manage UI elements within an <see cref="Engine.EC.Component"/>.
    /// </summary>
    public abstract class GUIComponent : Component, IHaveGUI
    {
        /// <summary>
        /// Gets or sets the root panel that contains all the added UI entities.
        /// </summary>
        public Panel GUIRootPanel { get; internal set; }

        /// <summary>
        /// Adds a control to the specified owner, a different owner, or the root itself if there is no other UI owner.
        /// </summary>
        /// <param name="control">The control to add.</param>
        public void AddGUIEntity(Entity control)
        {
            GUIRootPanel.AddChild(control);
        }

        /// <summary>
        /// Creates the UI for this GUI component. Override this method to add custom controls.
        /// </summary>
        /// <remarks>Add UI controls via <see cref="AddGUIEntity"/>.</remarks>
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
