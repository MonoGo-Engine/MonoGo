using MonoGo.Engine.EC;
using MonoGo.Engine.SceneSystem;

namespace MonoGo.Iguina
{
    /// <summary>
    /// The UIController class is responsible for managing the user interface (UI) system.
    /// It updates and renders UI elements by interacting with the GUI manager.
    /// </summary>
    internal class UIController : Entity
    {
        /// <summary>
        /// Initializes a new instance of the UIController class and assigns it to the GUI layer.
        /// </summary>
        internal UIController() : base(SceneMgr.GUILayer) { }

        /// <summary>
        /// Updates the state of the UI system. This method is called once per frame.
        /// </summary>
        public override void Update()
        {
            base.Update();

            // Updates the GUI manager.
            GUIMgr.Update();
        }

        /// <summary>
        /// Draws the UI elements managed by the GUI manager. This method is called once per frame.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            // Draws the GUI elements.
            GUIMgr.Draw();
        }
    }
}
