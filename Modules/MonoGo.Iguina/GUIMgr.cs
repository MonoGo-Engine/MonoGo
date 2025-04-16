using Iguina;
using Iguina.Entities;
using Microsoft.Xna.Framework.Graphics;
using MonoGo.Engine;
using MonoGo.Engine.SceneSystem;

namespace MonoGo.Iguina
{
    /// <summary>
    /// Provides management functionality for the GUI system, including theme handling, 
    /// control searching, and rendering.
    /// </summary>
    public static class GUIMgr
    {
        /// <summary>
        /// Gets the instance of the UI system.
        /// </summary>
        public static UISystem System { get; private set; } = null!;

        /// <summary>
        /// Gets or sets the path to the base theme folder of the UI system.
        /// </summary>
        public static string ThemeBaseFolder { get; private set; }

        /// <summary>
        /// Gets all available theme folders from the compiled content directory.
        /// </summary>
        public static string[] ThemeFolders => Directory.GetDirectories(ThemeBaseFolder)
            .Select(x => Path.GetFileNameWithoutExtension(x)!)
            .ToArray();

        /// <summary>
        /// Gets the path to the currently active theme folder.
        /// </summary>
        public static string ThemeActiveFolder { get; private set; }

        /// <summary>
        /// Gets the name of the currently active theme folder.
        /// </summary>
        public static string ThemeActiveName => Path.GetFileNameWithoutExtension(ThemeActiveFolder)!;

        /// <summary>
        /// Occurs after a UI theme is loaded.
        /// </summary>
        public static Action OnThemeChanged { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether debug rendering is enabled.
        /// </summary>
        public static bool DebugDraw
        {
            get => _debugDraw;
            set
            {
                _debugDraw = value;
                System.DebugRenderEntities = value;
            }
        }
        private static bool _debugDraw = false;

        private static UIRenderer _renderer = null!;
        private static UIInput _input = null!;

        /// <summary>
        /// Finds the root owner by its type name.
        /// </summary>
        /// <param name="owner">The type name of the owner.</param>
        /// <returns>The root control of the owner, or <c>null</c> if not found.</returns>
        public static Panel? FindRootOwner(string? owner)
        {
            if (string.IsNullOrEmpty(owner)) return null;

            Panel? rootOwner = null;
            System.Root.IterateChildren(
                control =>
                {
                    var identifier = control.Identifier?.Split(':').Last();
                    if (identifier != null && identifier == owner)
                    {
                        rootOwner = control as Panel;
                        return false;
                    }
                    return true;
                });
            return rootOwner;
        }

        /// <summary>
        /// Finds the root owner of a <see cref="IHaveGUI"/> object.
        /// </summary>
        /// <param name="owner">The owner object.</param>
        /// <returns>The root control of the owner, or <c>null</c> if not found.</returns>
        public static Panel? FindRootOwner(IHaveGUI? owner)
        {
            return FindRootOwner(owner?.GetType().Name);
        }

        /// <summary>
        /// Finds the root owner of a specified object type.
        /// </summary>
        /// <typeparam name="T">The type of the owner.</typeparam>
        /// <param name="owner">The owner object.</param>
        /// <returns>The root control of the owner, or <c>null</c> if not found.</returns>
        public static Panel? FindRootOwner<T>(T? owner)
        {
            if (owner != null) return FindRootOwner(((dynamic)owner).Name);
            return null;
        }

        /// <summary>
        /// Gets all root owners created so far.
        /// </summary>
        /// <returns>A list of root owner controls.</returns>
        public static List<IHaveGUI> GetRootOwners()
        {
            var owners = new List<IHaveGUI>();
            System.Root.IterateChildren(
                control =>
                {
                    var identifier = control.Identifier?.Split(':').First();
                    if (identifier != null && identifier == "Owner")
                    {
                        if (control.UserData != null && control.UserData is IHaveGUI owner)
                        {
                            owners.Add(owner);
                        }
                    }
                    return true;
                });
            return owners;
        }

        /// <summary>
        /// Finds and returns the first occurrence of a control with a given identifier and specific type.
        /// </summary>
        /// <typeparam name="T">The type of the control to find.</typeparam>
        /// <param name="identifier">The identifier of the control.</param>
        /// <returns>The first found control with the given identifier and type, or <c>null</c> if not found.</returns>
        public static T? Find<T>(string identifier) where T : Entity
        {
            bool anyType = typeof(T) == typeof(Entity);
            T? ret = default;

            System.Root.Walk(
                x =>
                {
                    if (x.Identifier == identifier && (anyType || (x.GetType() == typeof(T))))
                    {
                        ret = (T)x;
                        return false;
                    }
                    return true;
                });

            return ret;
        }

        /// <summary>
        /// Finds and returns the first occurrence of a control with a given identifier.
        /// </summary>
        /// <param name="identifier">The identifier of the control.</param>
        /// <returns>The first found control with the given identifier, or <c>null</c> if not found.</returns>
        public static Entity? Find(string identifier)
        {
            return Find<Entity>(identifier);
        }

        /// <summary>
        /// Sets the font size for the GUI according to the <c>SpriteFont</c> size.
        /// </summary>
        /// <param name="size">The font size to set.</param>
        public static void SetFontSize(float size)
        {
            _renderer.FontSize = size;
        }

        /// <summary>
        /// Sets the global text scale for the GUI.
        /// </summary>
        /// <param name="scale">The text scale to set.</param>
        public static void SetTextScale(float scale)
        {
            _renderer.GlobalTextScale = scale;
        }

        /// <summary>
        /// Registers a texture for the UI system.
        /// </summary>
        /// <param name="texture">The texture to register.</param>
        /// <param name="textureId">The ID of the texture.</param>
        public static void RegisterTexture(Texture2D texture, string textureId)
        {
            _renderer.RegisterTexture(texture, textureId);
        }

        /// <summary>
        /// Gets a registered texture by its ID.
        /// </summary>
        /// <param name="textureId">The ID of the texture.</param>
        /// <returns>The texture associated with the given ID.</returns>
        public static Texture2D GetTextureByID(string textureId)
        {
            return _renderer.GetTextureByID(textureId);
        }

        /// <summary>
        /// Loads a UI theme by its name.
        /// </summary>
        /// <param name="themeName">The name of the theme to load.</param>
        public static void LoadTheme(string themeName)
        {
            System.Root.ClearChildren();
            Init(ThemeBaseFolder, themeName);
            OnThemeChanged?.Invoke();
        }

        /// <summary>
        /// Initializes the GUI system with the specified theme folder and theme name.
        /// </summary>
        /// <param name="themeFolder">The path to the theme folder.</param>
        /// <param name="themeName">The name of the theme to initialize.</param>
        public static void Init(string themeFolder, string themeName)
        {
            if (!SceneMgr.GUILayer.EntityExists<UIController>())
            {
                new UIController();
            }
            ThemeBaseFolder = themeFolder;
            ThemeActiveFolder = Path.Combine(themeFolder, themeName);
            var defaultStyleSheetFilePath = Path.Combine(ThemeActiveFolder, "system_style.json");

            System = new UISystem(
                defaultStyleSheetFilePath,
                _renderer = new UIRenderer(ThemeActiveFolder),
                _input = new UIInput())
            {
                DebugRenderEntities = DebugDraw,
                AutoFocusEntities = false
            };
        }

        /// <summary>
        /// Updates the GUI system. This method is called once per frame.
        /// </summary>
        public static void Update()
        {
            _input.StartFrame();
            System.Update((float)GameMgr.ElapsedTime);
            _input.EndFrame();
        }

        /// <summary>
        /// Draws the GUI system. This method is called once per frame.
        /// </summary>
        public static void Draw()
        {
            _renderer.StartFrame();
            System.Draw();
            _renderer.EndFrame();
        }
    }
}
