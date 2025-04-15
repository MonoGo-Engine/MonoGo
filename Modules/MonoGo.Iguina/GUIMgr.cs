using Iguina;
using Iguina.Entities;
using Microsoft.Xna.Framework.Graphics;
using MonoGo.Engine;
using MonoGo.Engine.SceneSystem;

namespace MonoGo.Iguina
{
    public static class GUIMgr
    {
        // ui system instance
        public static UISystem System { get; private set; } = null!;

        /// <summary>
        /// Path to the base theme folder of the UI system.
        /// </summary>
        public static string ThemeBaseFolder { get; private set; }

        /// <summary>
        /// All the available theme folders from the compiled content directory.
        /// </summary>
        public static string[] ThemeFolders => Directory.GetDirectories(ThemeBaseFolder).Select(x => Path.GetFileNameWithoutExtension(x)!).ToArray();

        /// <summary>
        /// Path to the current active theme folder.
        /// </summary>
        public static string ThemeActiveFolder { get; private set; }

        /// <summary>
        /// Name of the current active theme folder.
        /// </summary>
        public static string ThemeActiveName => Path.GetFileNameWithoutExtension(ThemeActiveFolder)!;

        /// <summary>
        /// Triggers after a UI theme was loaded.
        /// </summary>
        public static Action OnThemeChanged { get; set; }

        public static bool DebugDraw
        {
            get { return _debugDraw; }
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
        /// Find the root owner by its type name.
        /// </summary>
        /// <param name="owner">The type name of the owner.</param>
        /// <returns>The root control of the owner.</returns>
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
        /// Find the root owner of a <see cref="IHaveGUI"/> object.
        /// </summary>
        /// <param name="owner">The owner object.</param>
        /// <returns>The root control of the owner.</returns>
        public static Panel? FindRootOwner(IHaveGUI? owner)
        {
            return FindRootOwner(owner?.GetType().Name);
        }

        /// <summary>
        /// Find the root owner of a <see cref="Type"/> object.
        /// </summary>
        /// <param name="owner">The owner object.</param>
        /// <returns>The root control of the owner.</returns>
        public static Panel? FindRootOwner<T>(T? owner)
        {
            if (owner != null) return FindRootOwner(((dynamic)owner).Name);
            return null;
        }

        /// <summary>
        /// Get all root owners created so far.
        /// </summary>
        /// <returns>The root owner controls.</returns>
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
        /// Find and return first occurance of a control with a given identifier and specific type.
        /// </summary>
        /// <typeparam name="T">Control type to get.</typeparam>
        /// <param name="identifier">Identifier to find.</param>
        /// <returns>First found control with given identifier and type, or null if nothing found.</returns>
        public static T? Find<T>(string identifier) where T : Entity
        {
            // should we return any control type?
            bool anyType = typeof(T) == typeof(Entity);
            T? ret = default;

            // iterate children
            System.Root.Walk(
                x =>
                {
                    // check if identifier and type matches - if so, return it
                    if (x.Identifier == identifier && (anyType || (x.GetType() == typeof(T))))
                    {
                        ret = (T)x;
                        return false;
                    }
                    return true;
                });

            // not found?
            return ret;
        }

        /// <summary>
        /// Find and return first occurance of a control with a given identifier.
        /// </summary>
        /// <param name="identifier">Identifier to find.</param>
        /// <returns>First found control with given identifier, or null if nothing found.</returns>
        public static Entity? Find(string identifier)
        {
            return Find<Entity>(identifier);
        }

        /// <summary>
        /// Set the font size for the GUI according to your <c>SpriteFont</c> size.
        /// </summary>
        public static void SetFontSize(float size)
        {
            _renderer.FontSize = size;
        }

        /// <summary>
        /// Set the global text scale for the GUI.
        /// </summary>
        public static void SetTextScale(float scale)
        {
            _renderer.GlobalTextScale = scale;
        }

        /// <summary>
        /// Register textures for the UISystem.
        /// </summary>
        /// <param name="texture">Texture to register.</param>
        /// <param name="textureId">ID of the texture to register.</param>
        public static void RegisterTexture(Texture2D texture, string textureId)
        {
            _renderer.RegisterTexture(texture, textureId);
        }

        /// <summary>
        /// Get a registered texture by its ID.
        /// </summary>
        public static Texture2D GetTextureByID(string textureId)
        {
            return _renderer.GetTextureByID(textureId);
        }

        /// <param name="themeName">UI System theme name.</param>
        public static void LoadTheme(string themeName)
        {
            System.Root.ClearChildren();
            Init(ThemeBaseFolder, themeName);
            OnThemeChanged?.Invoke();
        }

        public static void Init(string themeFolder, string themeName)
        {
            if (!SceneMgr.GUILayer.EntityExists<UIController>())
            {
                new UIController(SceneMgr.GUILayer);
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

        public static void Update()
        {
            _input.StartFrame();
            System.Update((float)GameMgr.ElapsedTime);
            _input.EndFrame();
        }

        public static void Draw()
        {
            _renderer.StartFrame();
            System.Draw();
            _renderer.EndFrame();
        }
    }
}
