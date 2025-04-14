namespace MonoGo.Engine
{
    /// <summary>
    /// General interface for working with GUI libs.
    /// </summary>
    public interface IHaveGUI
    {
        /// <summary>
        /// Add UI entities to this GUI root.
        /// </summary>
        void CreateUI();

        /// <summary>
        /// Enable or disable this GUI root.
        /// </summary>
        void Enable(bool enable);

        /// <summary>
        /// Make this GUI root visible or invisible.
        /// </summary>
        void Visible(bool visible);

        /// <summary>
        /// Removes this GUI root and all of its contents from his parent.
        /// </summary>
        void Clear();
    }
}
