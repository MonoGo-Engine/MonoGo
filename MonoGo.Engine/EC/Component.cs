﻿namespace MonoGo.Engine.EC
{
	/// <summary>
	/// Stores data, which will be processed by corresponding systems.
	/// </summary>
	public abstract class Component
	{
		/// <summary>
		/// Owner of a component.
		/// 
		/// NOTE: Component should ALWAYS have an owner. 
		/// </summary>
		public Entity Owner {get; internal set;}

		/// <summary>
		/// The key of this component.
		/// </summary>
        public string Key { get; internal set; }

        /// <summary>
        /// Tells if this component was initialized.
        /// </summary>
        public bool Initialized {get; internal set;} = false;

        /// <summary>
		/// Enables and makes Component Visible in one go.
		/// </summary>
        public void Activate()
        {
            Enabled = true;
            Visible = true;
        }

        /// <summary>
        /// Disables and makes Component Invisible in one go.
        /// </summary>
        public void DeActivate()
        {
            Enabled = false;
            Visible = false;
        }

        /// <summary>
        /// If component is enabled, it will be processed by Create and Update methods.
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                _GUIEnable(value);
            }
        }
        private bool _enabled = true;

        /// <summary>
        /// Enable or Disable all GUI controls of this component.
        /// </summary>
        public bool GUIEnabled
        {
            get { return _guiEnabled; }
            set
            {
                _guiEnabled = value;
                _GUIEnable(value);
            }
        }
        private bool _guiEnabled = true;

        /// <summary>
        /// If component is visible, it will be processed by Draw method.
        /// 
        /// NOTE: components are NOT visible by default!
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                _GUIVisible(value);
            }
        }
        private bool _visible = true;

        /// <summary>
        /// Hide or Show all GUI controls of this component.
        /// </summary>
        public bool GUIVisible
        {
            get { return _guiVisible; }
            set
            {
                _guiVisible = value;
                _GUIVisible(value);
            }
        }
        private bool _guiVisible = true;

        internal void _GUIEnable(bool enable)
        {
            if (this is IHaveGUI GUI) GUI.Enable(enable);
        }

        internal void _GUIVisible(bool visible)
        {
            if (this is IHaveGUI GUI) GUI.Visible(visible);
        }

        public Component(string key = default) 
		{ 
			Key = key;
		}
		
		/// <summary>
        /// Depth of Draw event. Objects with the lowest depth draw the last.
        /// </summary>
        public int Depth
        {
            get => _depth;
            set
            {

                if (value != _depth)
                {
                    _depth = value;
                    if (Owner != null) Owner._depthListComponentsOutdated = true;
                }
            }
        }
        private int _depth;

        #region Events.

        /*
		 * Event order:
		 * - FixedUpdate
		 * - Update
		 * - Draw
		 */

        /// <summary>
        /// Gets called when component is added to the entity. 
        /// If removed and added several times, the event will still be called.
        /// </summary>
        public virtual void Initialize() { }
		
		/// <summary>
		/// Updates at a fixed rate, if entity is enabled.
		/// </summary>
		public virtual void FixedUpdate() { }

		/// <summary>
		/// Updates every frame, if entity is enabled.
		/// </summary>
		public virtual void Update() { }

		/// <summary>
		/// Draw updates. Triggers only if entity is visible.
		/// 
		/// NOTE: DO NOT put any significant logic into Draw.
		/// It may skip frames.
		/// </summary>
		public virtual void Draw() { }

        /// <summary>
        ///	Triggers right before destruction.
        /// </summary>
        public virtual void Destroy()
        {
            if (this is IHaveGUI GUI) GUI.Clear();
        }

		#endregion Events.

	}
}
