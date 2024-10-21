using MonoGo.Engine.UI;
using MonoGo.Engine.UI.Controls;

namespace MonoGo.Engine
{
    /// <summary>
    /// Giving your <see cref="Engine.EC.Entity"/>'s or <see cref="Engine.EC.Component"/>'s managed GUI capabilities.
    /// </summary>
    /// <remarks>
    /// An object bound GUI will be automatically created, deleted, enabled, disabled or become visible, invisible on certain events.
    /// E.g.: Making an <c>Entity</c> invisible will also make its connected GUI invisible.
    /// It's also possible to manually set visibility or delete the GUI.
    /// Theoretically you can use this interface on everything to create object bound GUI, but then you can only manage the above mentioned states manually.
    /// This is why it's recommended to inherit from <c>Entity</c> or <c>Component</c> in your custom objects before implementing this interface.
    /// </remarks>
    public interface IHaveGUI
    {
        /// <summary>
        /// Adds all the GUI elements you are defining here to the GUI root owner panel.
        /// </summary>
        void CreateUI();

        /// <summary>
        /// Creates the GUI-"RootOwner"-Panel which contains all GUI elements created by "CreateUI()".
        /// </summary>
        void Init()
        {
            Clear();

            UISystem._ownerStack.Push(this);
            var rootOwner = UISystem.Root.AddChild(new Panel(stylesheet: null!) { Identifier = $"Owner:{GetType().Name}", Anchor = UI.Defs.Anchor.Center, UserData = this }, true);
            rootOwner.Size.SetPercents(100, 100);
            rootOwner.IgnoreInteractions = true;
            CreateUI();
            UISystem._ownerStack.Pop();
        }

        /// <summary>
        /// Enable or disable the GUI root owner panel.
        /// </summary>
        void Enable(bool enable)
        {
            var rootOwner = UISystem.FindRootOwner(this);
            if (rootOwner != null) rootOwner.Enabled = enable;
        }

        /// <summary>
        /// Make the GUI root owner panel visible or invisible.
        /// </summary>
        void Visible(bool visible)
        {
            var rootOwner = UISystem.FindRootOwner(this);
            if (rootOwner != null) rootOwner.Visible = visible;
        }

        /// <summary>
        /// Removes the GUI root owner panel and all of its contents.
        /// </summary>
        void Clear()
        {
            UISystem.FindRootOwner(this)?.RemoveSelf(true);
        }
    }
}
