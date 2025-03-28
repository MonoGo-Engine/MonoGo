﻿using Microsoft.Xna.Framework;
using MonoGo.Engine.Cameras;
using MonoGo.Engine.SceneSystem;
using MonoGo.Engine.UI.Controls;
using MonoGo.Engine.UI.Defs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace MonoGo.Engine.UI
{
    /// <summary>
    /// A static GUI system class to manage GUI elements everywhere and on everything.
    /// </summary>
    public static class UISystem
    {
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

        /// <summary>
        /// Total elapsed time this system is running, in seconds.
        /// </summary>
        public static double ElapsedTime { get; private set; }

        /// <summary>
        /// Last update frame delta time, in seconds.
        /// </summary>
        public static float LastDeltaTime { get; private set; }

        /// <summary>
        /// If true, will render UI cursor (if defined in stylesheet).
        /// </summary>
        public static bool ShowCursor = true;

        /// <summary>
        /// Default stylesheets to use for different control types when no stylesheet is provided.
        /// </summary>
        public static class DefaultStylesheets
        {
            public static StyleSheet? Panels;
            public static StyleSheet? MessageBoxPanels;
            public static StyleSheet? MessageBoxParagraphs;
            public static StyleSheet? MessageBoxTitles;
            public static StyleSheet? MessageBoxButtons;
            public static StyleSheet? MessageBoxBackdrop;
            public static StyleSheet? Paragraphs;
            public static StyleSheet? Titles;
			public static StyleSheet? Labels;
            public static StyleSheet? Buttons;
            public static StyleSheet? HorizontalLines;
			public static StyleSheet? VerticalLines;
            public static StyleSheet? CheckBoxes;
            public static StyleSheet? RadioButtons;
            public static StyleSheet? HorizontalSliders;
            public static StyleSheet? HorizontalSlidersHandle;
            public static StyleSheet? VerticalSliders;
            public static StyleSheet? VerticalSlidersHandle;
			public static StyleSheet? HorizontalColorSliders;
            public static StyleSheet? HorizontalColorSlidersHandle;
            public static StyleSheet? VerticalColorSliders;
            public static StyleSheet? VerticalColorSlidersHandle;
            public static StyleSheet? ListPanels;
            public static StyleSheet? ListItems;
            public static StyleSheet? DropDownPanels;
            public static StyleSheet? DropDownItems;
			public static StyleSheet? DropDownIcon;
            public static StyleSheet? VerticalScrollbars;
            public static StyleSheet? VerticalScrollbarsHandle;
            public static StyleSheet? TextInput;
			public static StyleSheet? NumericTextInput;
            public static StyleSheet? NumericTextInputButton;
            public static StyleSheet? HorizontalProgressBars;
            public static StyleSheet? HorizontalProgressBarsFill;
            public static StyleSheet? VerticalProgressBars;
            public static StyleSheet? VerticalProgressBarsFill;
			public static StyleSheet? ColorPickers;
            public static StyleSheet? ColorPickersHandle;
        }

        /// Currently-targeted control (control we point on with the cursor).
        /// </summary>
        public static Control? TargetedControl { get; private set; }

        /// <summary>
        /// System-level stylesheet.
        /// Define properties like cursor graphics and general stuff.
        /// </summary>
        public static SystemStyleSheet SystemStyleSheet = new SystemStyleSheet();

        /// <summary>
        /// If true, will debug-render controls.
        /// </summary>
        public static bool DebugDraw = false;

        /// <summary>
        /// Control events you can register to.
        /// These events will trigger for any control in the system.
        /// </summary>
        public static ControlEvents Events;

        /// <summary>
        /// Scale all the texts in this UI system.
        /// </summary>
        public static float TextsScale => SystemStyleSheet.TextScale;

        /// <summary>
        /// Root control.
        /// All child controls should be added to this object.
        /// </summary>
        public static Panel Root { get; private set; }

        // queue of scissor regions to cut-off rendering when overflow mode is hidden.
        internal static Queue<Rectangle> _scissorRegionQueue = new();

        /// <summary>
        /// When controls turn into interactive state (for example a button is clicked on), it will be locked in this state for at least this time, in seconds.
        /// This property is useful to make sure the interactive state is properly shown, even if user perform very rapid short clicks.
        /// </summary>
        /// <remarks>This property is especially important when there's interpolation on texture change, and switching to interactive state is not immediate.</remarks>
        internal static float TimeToLockInteractiveState => SystemStyleSheet.TimeToLockInteractiveState;

        /// <summary>
        /// The owner stack of newly created IHaveGUI objects.
        /// </summary>
        internal static Stack<IHaveGUI> _ownerStack = new();

        /// <summary>
        /// Adds a control to the current owner or the root itself if there is no other UI owner.
        /// </summary>
        /// <param name="control">Control to add.</param>
        public static void Add(Control control)
        {
            var rootOwner = FindRootOwner(_ownerStack.Peek()) ?? Root;
            rootOwner.AddChild(control);
        }

        /// <summary>
        /// Adds a control to the current owner, a different owner or the root itself if there is no other UI owner.
        /// </summary>
        /// <param name="control">Control to add.</param>
        /// <param name="otherOwner">Add this control to a different owner.</param>
        public static void Add(Control control, string? otherOwner = null)
        {
            var rootOwner = FindRootOwner(otherOwner) ?? FindRootOwner(_ownerStack.Peek()) ?? Root;            
            rootOwner.AddChild(control);
        }

        /// <summary>
        /// Adds a control to the current owner, a different owner or the root itself if there is no other UI owner.
        /// </summary>
        /// <param name="control">Control to add.</param>
        /// <param name="otherOwner">Add this control to a different owner.</param>
        public static void Add(Control control, IHaveGUI? otherOwner = null)
        {
            var rootOwner = FindRootOwner(otherOwner) ?? FindRootOwner(_ownerStack.Peek()) ?? Root;
            rootOwner.AddChild(control);
        }

        /// <summary>
        /// Adds a control to the current owner, a different owner or the root itself if there is no other UI owner.
        /// </summary>
        /// <param name="control">Control to add.</param>
        /// <param name="otherOwner">Add this control to a different owner.</param>
        public static void Add<T>(Control control, T? otherOwner = default)
        {
            var rootOwner = FindRootOwner(otherOwner) ?? FindRootOwner(_ownerStack.Peek()) ?? Root;

            rootOwner.AddChild(control);
        }

        /// <summary>
        /// Find the root owner by its type name.
        /// </summary>
        /// <param name="owner">The type name of the owner.</param>
        /// <returns>The root control of the owner.</returns>
        public static Panel? FindRootOwner(string? owner)
        {
            if (string.IsNullOrEmpty(owner)) return null;

            Panel? rootOwner = null;
            Root.IterateChildren(
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
        /// Find the root owner of an <see cref="IHaveGUI"/> object.
        /// </summary>
        /// <returns>The root owner controls.</returns>
        public static List<IHaveGUI> GetRootOwners()
        {
            var owners = new List<IHaveGUI>();
            Root.IterateChildren(
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
        public static T? Find<T>(string identifier) where T : Control
        {
            // should we return any control type?
            bool anyType = typeof(T) == typeof(Control);
            T? ret = default;

            // iterate children
            Root.Walk(
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
        public static Control? Find(string identifier)
        {
            return Find<Control>(identifier);
        }

        internal static void Init(string themeFolder, string themeName)
        {
            Root = new Panel(null!) {Identifier = "Root" };

            if (!SceneMgr.GUILayer.EntityExists<UIController>())
            {
                new UIController(SceneMgr.GUILayer);
            }
            ThemeBaseFolder = themeFolder;
            ThemeActiveFolder = Path.Combine(themeFolder, themeName);
            var defaultStyleSheetFilePath = Path.Combine(ThemeActiveFolder, "system_style.json");

            // create renderer and input
            Renderer.Init(ThemeActiveFolder);

            try
            {
                SystemStyleSheet = JsonSerializer.Deserialize<SystemStyleSheet>(
                    File.ReadAllText(defaultStyleSheetFilePath), Serialization.SerializerOptions)!;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to read or deserialize UI system stylesheet!", e);
            }

            if (SystemStyleSheet.LoadDefaultStylesheets != null)
            {
                LoadDefaultStylesheets(SystemStyleSheet.LoadDefaultStylesheets, Path.GetDirectoryName(defaultStyleSheetFilePath) ?? string.Empty);
            }
        }

        /// <param name="themeName">UI System theme name.</param>
        public static void LoadTheme(string themeName)
        {
            Clear();
            Init(ThemeBaseFolder, themeName);
            OnThemeChanged?.Invoke();
        }

        /// <summary>
        /// Load all default stylesheets from dictionary.
        /// </summary>
        /// <param name="stylesheetsToLoad">Stylesheets to load. Key = stylesheet control name, Value = path to load from.</param>
        /// <param name="parentFolder">Folder to load stylesheet files from.</param>
        public static void LoadDefaultStylesheets(Dictionary<string, string> stylesheetsToLoad, string parentFolder)
        {
            foreach (var pair in stylesheetsToLoad)
            {
                var controlStyleName = pair.Key;
                var path = pair.Value;

                var field = typeof(DefaultStylesheets).GetField(controlStyleName, BindingFlags.Static | BindingFlags.Public);
                if (field == null)
                {
                    throw new FormatException($"Error loading stylesheet for control style id '{controlStyleName}': control key not found under 'DefaultStylesheets'.");
                }

                var fullPath = Path.Combine(parentFolder, path);
                try
                {
                    var stylesheet = StyleSheet.LoadFromJsonFile(fullPath);
                    field.SetValue(null, stylesheet);
                }
                catch (FileNotFoundException)
                {
                    throw new FormatException($"Error loading stylesheet for control style id '{controlStyleName}': stylesheet file '{fullPath}' not found!");
                }
            }
        }

        public static void Clear()
        {
            foreach (var child in Root._children.ToList())
            {
                child.RemoveSelf();
            }
        }

        public static Vector2 TransformMousePosition()
        {
            Matrix cam = CameraMgr.Cameras[0].View;
            var mousePos = Input.ScreenMousePosition;
            return Vector2.Transform(mousePos, Matrix.Identity) - new Vector2(cam.Translation.X, cam.Translation.Y);
        }

        /// <summary>
        /// Perform UI updates.
        /// Must be called every update frame in your game main loop.
        /// </summary>
        public static void Update()
        {
            var deltaTime = (float)GameMgr.ElapsedTime;

            // update elapsed time
            LastDeltaTime = deltaTime;
            ElapsedTime += deltaTime;

            // set root to cover entire screen
            var screenBounds = Renderer.GetScreenBounds();
            Root.Size.SetPixels(screenBounds.Width, screenBounds.Height);

            // update all controls
            Root._DoUpdate(deltaTime);

            // check if should lock target control
            bool keepTargetControl = (TargetedControl != null) ? 
                (TargetedControl.LockFocusOnSelf && TargetedControl.IsCurrentlyVisible() && !TargetedControl.IsCurrentlyLocked() && !TargetedControl.IsCurrentlyDisabled()) 
                : false;

            // if dragging an control, we can't lose the target
            if (Input.CheckButton(Buttons.MouseLeft) && (TargetedControl != null) && TargetedControl.LockFocusWhileMouseDown)
            {
                keepTargetControl = true;
            }

            // current mouse position
            var cp = TransformMousePosition().ToPoint();

            // find new control we target
            if (!keepTargetControl)
            {
                // reset target control
                TargetedControl = null;

                // iterate all controls to see which control we point on
                List<Control> controlsToPostProcess = new List<Control>();
                Root.Walk((Control control) =>
                {
                    // skip controls that are without interactions
                    // note: we don't want to skip locked or disabled controls because they can still 'block' other controls.
                    if (control.IgnoreInteractions)
                    {
                        return true;
                    }

                    // check if control can't get focused while mouse is down
                    if (!control.CanGetFocusWhileMouseIsDown && Input.CheckButton(Buttons.MouseLeft))
                    {
                        return true;
                    }

                    // check if top most interactions
                    if (control.TopMostInteractions)
                    {
                        controlsToPostProcess.Add(control);
                        return true;
                    }

                    // check if we point on this control
                    if (control.IsCurrentlyVisible() && control.IsPointedOn(cp))
                    {
                        TargetedControl = control;
                    }

                    // continue iteration
                    return true;
                });

                // do top-most interactions
                foreach (var control in controlsToPostProcess)
                {
                    if (control.IsCurrentlyVisible() && control.IsPointedOn(cp))
                    {
                        TargetedControl = control;
                    }
                }
            }

            // calculate current input state
            var currInputState = new CurrentInputState()
            {
                MousePosition = cp,
                LeftMouseButton = Input.CheckButton(Buttons.MouseLeft),
                RightMouseButton = Input.CheckButton(Buttons.MouseRight),
                WheelMouseButton = Input.CheckButton(Buttons.MouseMiddle),
                MouseWheelChange = Input.MouseWheelValue,
                TextInputCommands = TextInputCmd.GetTextInputCommands()
            };
            var inputState = new InputState()
            {
                _Previous = _lastInputState,
                _Current = currInputState,
                ScreenBounds = Renderer.GetScreenBounds()
            };
            _lastInputState = currInputState;
            CurrentInputState = inputState;

            // do interactions with targeted control
            // unless its locked or disabled
            if (TargetedControl != null)
            {
                if (TargetedControl.TransferInteractionsTo != null)
                {
                    TargetedControl = TargetedControl.TransferInteractionsTo;
                }

                if (!TargetedControl.IsCurrentlyLocked() && !TargetedControl.IsCurrentlyDisabled())
                {
                    TargetedControl.DoInteractions(inputState);
                }
            }

            // do post interactions
            Root.Walk((Control control) =>
            {
                if (control.Interactable)
                {
                    control.PostUpdate(inputState);
                }
                return true;
            });
        }

        // last frame input state
        private static CurrentInputState _lastInputState;

        /// <summary>
        /// Get current input state.
        /// </summary>
        public static InputState CurrentInputState { get; private set; }

        /// <summary>
        /// Add action to call after rendering controls.
        /// </summary>
        internal static void RunAfterDrawingControls(Action action)
        {
            _postDrawActions.Add(action);
        }
        private static List<Action> _postDrawActions = new List<Action>();

        /// <summary>
        /// Render the UI.
        /// Must be called every draw frame in your game rendering loop.
        /// </summary>
        /// <remarks>
        /// Clearing screen, setting render target, or using a 'camera' like object for zooming, is up to the host application.
        /// </remarks>
        public static void Draw()
        {
            Renderer.StartFrame();

            // reset scissors queue
            _scissorRegionQueue.Clear();
            Renderer.ClearScissorRegion();

            // draw all controls
            var screenRect = Renderer.GetScreenBounds();
            var rootDrawResult = new Control.DrawMethodResult()
            {
                BoundingRect = screenRect,
                InternalBoundingRect = screenRect
            };
            Root._DoDraw(rootDrawResult, null, false);

            // call post-draw actions
            if (_postDrawActions.Count > 0)
            {
                foreach (var action in _postDrawActions)
                {
                    action();
                }
                _postDrawActions.Clear();
            }

            // debug draw stuff
            if (DebugDraw)
            {
                Root.DebugDraw(true);
            }

            Renderer.EndFrame();
        }

        public static void DrawCursor()
        {
            var pos = Input.ScreenMousePosition.ToPoint();

            // get which cursor to render
            CursorProperties? cursor = SystemStyleSheet.CursorDefault;
            if (TargetedControl?.IsPointedOn(pos, true) ?? false)
            {
                if (TargetedControl.CursorStyle != null)
                {
                    cursor = TargetedControl.CursorStyle;
                }
                else if (TargetedControl.IsCurrentlyDisabled())
                {
                    cursor = SystemStyleSheet.CursorDisabled ?? SystemStyleSheet.CursorDefault;
                }
                else if (TargetedControl.IsCurrentlyLocked())
                {
                    cursor = SystemStyleSheet.CursorLocked ?? SystemStyleSheet.CursorDefault;
                }
                else if (TargetedControl.Interactable)
                {
                    cursor = SystemStyleSheet.CursorInteractable ?? SystemStyleSheet.CursorDefault;
                }
            }

            // render cursor
            if (ShowCursor && cursor != null)
            {
                var mousePos = pos;
				var scale = cursor.Scale * SystemStyleSheet.CursorScale;
                var destRect = cursor.SourceRect;
                destRect.X = mousePos.X + (int)(cursor.Offset.X * scale);
                destRect.Y = mousePos.Y + (int)(cursor.Offset.Y * scale);
                destRect.Width = (int)(destRect.Width * scale);
                destRect.Height = (int)(destRect.Height * scale);
                Renderer.DrawSprite(cursor.TextureId, destRect, cursor.SourceRect, cursor.FillColor);
            }
        }
    }
}
