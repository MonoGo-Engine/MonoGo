﻿using MonoGo.Engine.Cameras;
using MonoGo.Engine.SceneSystem;
using MonoGo.Engine.Utils.Coroutines;
using MonoGo.Engine.Utils.CustomCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MonoGo.Engine.EC
{
    /// <summary>
    /// Parent class of every in-game object.
    /// Can hold components, or implement its own logic.
    /// </summary>
    public class Entity
	{
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
					Layer._depthListEntitiesOutdated = true;
				}
			}
		}
		private int _depth;
		
		internal bool _depthListComponentsOutdated = false;

        /// <summary>
        /// Tells f object was destroyed.
        /// </summary>
        public bool Destroyed {get; private set;} = false;

		/// <summary>
		/// Enables and makes Entity Visible in one go.
		/// </summary>
        public virtual void Activate()
        {
            Enabled = true;
            Visible = true;
        }

        /// <summary>
        /// Disables and makes Entity Invisible in one go.
        /// </summary>
        public virtual void DeActivate()
        {
            Enabled = false;
            Visible = false;
        }

        /// <summary>
        /// If false, Update and Destroy events won't be executed.
        /// NOTE: This also applies to entity's components.
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
        /// Enable or Disable all GUI controls of this entity.
        /// NOTE: This also applies to entity's components!
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
        /// Hide or Show all GUI controls of this entity.
        /// NOTE: This also applies to entity's components!
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

        /// <summary>
        /// If false, Draw events won't be executed.
        /// NOTE: This also applies to entity's components.
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

		internal void _GUIEnable(bool enable)
		{
            if (this is IHaveGUI GUI) GUI.Enable(enable);

            GetAllComponents().ToList().ForEach(component => component._GUIEnable(enable));
        }

        internal void _GUIVisible(bool visible)
        {
            if (this is IHaveGUI GUI) GUI.Visible(visible);

            GetAllComponents().ToList().ForEach(component => component._GUIVisible(visible));
        }

        /// <summary>
        /// Layer that entity is currently on.
        /// </summary>
        public Layer Layer
		{
			get => _layer;
			set
			{
				if (_layer != null)
				{
					_layer.RemoveEntity(this);
				}
				_layer = value;
				_layer.AddEntity(this);
			}
		}
		private Layer _layer;

		/// <summary>
		/// Scene that entity is currently on.
		/// </summary>
		public Scene Scene => _layer.Scene;

		// We need two collections to eliminate collection modification 
		// during foreach while keeping GetComponent fast.
		private Dictionary<Type, Component> _componentDictionary;
        internal SafeList<Component> _componentList;
        internal SafeList<Component> _depthSortedComponents;
        private ComponentDepthComparer _depthComparer = new ComponentDepthComparer();

        /// <summary>
        /// If camera's RenderMask does not have any bits in common with entity's RenderMask, 
        /// the entity will not be rendered for that camera.
        /// </summary>
        public RenderMask RenderMask = RenderMask.Default;

		public Entity(Layer layer)
		{
			_componentDictionary = new Dictionary<Type, Component>();
			_componentList = new SafeList<Component>();
			Layer = layer;
		}		

		#region Events.

		/*
		 * Event order:
		 * - FixedUpdate
		 * - Update
		 * - Draw
		 */

		/// <summary>
		/// Updates at a fixed rate, if entity is enabled.
		/// </summary>
		public virtual void FixedUpdate() 
		{
			foreach(var component in _componentList)
			{
				if (component.Enabled)
				{
					component.FixedUpdate();
				}
			}
		}
		
		/// <summary>
		/// Updates every frame, if entity is enabled.
		/// </summary>
		public virtual void Update() 
		{
			foreach (var component in _componentList)
			{
				if (component.Enabled)
				{
					component.Update();
				}
			}
		}

		/// <summary>
		/// Draw updates. Triggers only if entity is visible.
		/// 
		/// NOTE: DO NOT put any significant logic into Draw.
		/// It may skip frames.
		/// </summary>
		public virtual void Draw() 
		{
			foreach (var component in _depthSortedComponents)
			{
				if (component.Visible)
				{
					component.Draw();
				}
			}
		}

		/// <summary>
		///	Triggers right before destruction.
		/// </summary>
		public virtual void Destroy() 
		{
			foreach (var component in _componentDictionary.Values)
			{
				component.Destroy();
			}

            if (this is IHaveGUI GUI) GUI.Clear();
        }

		#endregion Events.

		#region Components.

		/// <summary>
		/// Adds component to the entity.
		/// </summary>
		public Component AddComponent(Type type, Component component)
		{
			if (component.Owner != null)
			{
				throw new Exception("Component " + component + "already has an owner!");
			}
			_componentDictionary.Add(type, component);
			_componentList.Add(component);
			component.Owner = this;
			component.Initialize();
			component.Initialized = true;
            _depthListComponentsOutdated = true;

            if (component is IHaveGUI GUI) GUI.CreateUI();

            return component; // Doing a passthrough for nicer syntax.	
		}

		/// <summary>
		/// Adds component to the entity.
		/// </summary>
		public T AddComponent<T>(T component) where T : Component =>
			(T)AddComponent(typeof(T), component);

        /// <summary>
        /// Adds component to the entity and place it at the top of the component list.
        /// </summary>
        public T AddComponentToTop<T>(T component) where T : Component
        {
            AddComponent(typeof(T), component);
            ReorderComponentToTop(component);
            return component;
        }

        /// <summary>
        /// Adds component to the entity the the specific index of the component list.
        /// </summary>
        public T AddComponent<T>(T component, int index) where T : Component
		{
			AddComponent(typeof(T), component);
			ReorderComponent(component, index);
			return component;
        }

        /// <summary>
        /// Returns component of given class.
        /// </summary>
        public T GetComponent<T>() where T : Component =>
			(T)_componentDictionary[typeof(T)];

        /// <summary>
        /// Returns component of given class.
        /// </summary>
        public Component GetComponent(Type type) =>
			_componentDictionary[type];

        /// <summary>
        /// Returns component of given type.
		/// Useful for components with interfaces for example.
        /// </summary>
        public T FindComponent<T>()
        {
            return (dynamic)_componentDictionary.First(x => x.Value is T).Value;
        }

        /// <summary>
        /// Retrieves component of given class, if it exists, and returns true. If it doesn't, returns false.
        /// </summary>
        public bool TryGetComponent<T>(out T component) where T : Component
		{
			var result = _componentDictionary.TryGetValue(typeof(T), out Component c);
			component = (T)c; // Needs a manual cast.
			return result;
		}
		
		/// <summary>
		/// Retrieves component of given class, if it exists, and returns true. If it doesn't, returns false.
		/// </summary>
		public bool TryGetComponent(out Component component, Type type) =>
			_componentDictionary.TryGetValue(type, out component);
		
		/// <summary>
		/// Returns all the components. All of them.
		/// </summary>
		public Component[] GetAllComponents()
		{
			var array = new Component[_componentDictionary.Count];
			var id = 0;

			foreach(var componentPair in _componentDictionary)
			{
				array[id] = componentPair.Value;
				id += 1;
			}
			return array;
		}

		/// <summary>
		/// Checks if an entity has the component of given type.
		/// </summary>
		public bool HasComponent<T>() where T : Component =>
			_componentDictionary.ContainsKey(typeof(T));
		
		/// <summary>
		/// Checks if an entity has the component of given type.
		/// </summary>
		public bool HasComponent(Type type) =>
			_componentDictionary.ContainsKey(type);
				
		/// <summary>
		/// Removes component from an entity and returns it.
		/// </summary>
		public Component RemoveComponent<T>() where T : Component =>
			RemoveComponent(typeof(T));
		
		/// <summary>
		/// Removes component from an entity and returns it.
		/// </summary>
		public Component RemoveComponent(Type type)
		{
			if (_componentDictionary.TryGetValue(type, out Component component))
			{
				component.Destroy();
				_componentDictionary.Remove(type);
				_componentList.Remove(component);
				component.Owner = null;
                _depthListComponentsOutdated = true;

                if (component is IHaveGUI GUI) GUI.Clear();

                return component;
			}
			return null;
		}

        #region Ordering.

        /// <summary>
        /// Changes the update order of a component and places it 
        /// at the specified position of the component list.
        /// </summary>
        public void ReorderComponent(Component component, int index)
        {
            if (!_componentList.Contains(component))
            {
                throw new Exception("Cannot reorder component - it doesn't belong to this entity.");
            }
            _componentList.Remove(component);
            _componentList.Insert(index, component);
        }

        /// <summary>
        /// Changes the update order of an component and places it 
        /// at the top of the component list.
        /// </summary>
        public void ReorderComponentToTop(Component component) =>
            ReorderComponent(component, 0);

        /// <summary>
        /// Changes the update order of a component and places it 
        /// at the bottom of the component list.
        /// </summary>
        public void ReorderComponentToBottom(Component component)
        {
            if (!_componentList.Contains(component))
            {
                throw new Exception("Cannot reorder component - it doesn't belong to this entity.");
            }
            _componentList.Remove(component);
            _componentList.Add(component);
        }

        #endregion Ordering.

        #endregion Components.

        /// <summary>
        /// Sorts the components of this entity by depth, if depth sorting is enabled.
        /// </summary>
        internal void SortByDepth()
        {
			if (Layer.DepthSorting)
			{
				if (_depthListComponentsOutdated)
				{
					_depthSortedComponents = new SafeList<Component>(_componentList.ToList());
					_depthSortedComponents.Sort(_depthComparer);
					_depthListComponentsOutdated = false;
				}
			}
			else
			{
                _depthSortedComponents = _componentList;
            }
        }

        /// <summary>
        /// Starts a new coroutine.
        /// </summary>
        public Coroutine StartCoroutine(IEnumerator routine)
		{
			if (!HasComponent<CCoroutineRunner>())
			{
				AddComponent(new CCoroutineRunner());
			}
			var c = GetComponent<CCoroutineRunner>();
			return c.StartCoroutine(routine);
		}

		public void StopCoroutine(Coroutine coroutine)
		{
			if (!HasComponent<CCoroutineRunner>())
			{
				return;
			}
			var c = GetComponent<CCoroutineRunner>();
			c.StopCoroutine(coroutine);
		}

		/// <summary>
		/// Starts a new job with a certain time budget.
		/// Jobs are similar to coroutines, but instead of scheduling execution
		/// to the next frame every time, they have a time budget. 
		/// If there is enough time left, yield return null will immediately return
		/// back the the job in the same frame.
		/// Jobs are well-suited for large tasks that should happen in the background, like pathfinding, map loading, etc.
		/// NOTE: DO NOT use Wait methods, they are incompatible with jobs!!!
		/// </summary>
		public Job StartJob(IEnumerator routine, float millisecondBudget = 0.1f)
		{
			if (!HasComponent<CCoroutineRunner>())
			{
				AddComponent(new CCoroutineRunner());
			}
			var c = GetComponent<CCoroutineRunner>();
			return c.StartJob(routine, millisecondBudget);
		}

		public void StopJob(Job job) =>
			StopCoroutine(job);

		/// <summary>
		/// Destroys entity and all of its components.
		/// </summary>
		public void DestroyEntity()
		{
			if (!Destroyed)
			{
				Destroyed = true;
				Destroy();
			}
		}
	}
}
