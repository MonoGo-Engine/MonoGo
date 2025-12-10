using Iguina.Defs;
using Iguina.Entities;
using MonoGo.Engine;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.PostProcessing;
using MonoGo.Engine.Resources;
using MonoGo.Engine.SceneSystem;
using MonoGo.Engine.Utils;
using MonoGo.Iguina;
using MonoGo.Samples.Demos;
using System;
using System.Collections.Generic;
using Color = Iguina.Defs.Color;
using Entity = Iguina.Entities.Entity;
using Point = Iguina.Defs.Point;

namespace MonoGo.Samples
{
    public class SceneSwitcher : GUIEntity
    {
        public static readonly string Description =
            "Camera > ${FC:96FF5F}Move${RESET}: ${FC:FFDB5F}" + CameraController.UpButton + "${RESET} / ${FC:FFDB5F}" + CameraController.DownButton + "${RESET} / ${FC:FFDB5F}" + CameraController.LeftButton + "${RESET} / ${FC:FFDB5F}" + CameraController.RightButton + "${RESET}" + Environment.NewLine +
            "Camera > ${FC:96FF5F}Rotate${RESET}: ${FC:FFDB5F}" + CameraController.RotateLeftButton + "${RESET} / ${FC:FFDB5F}" + CameraController.RotateRightButton + "${RESET}" + Environment.NewLine +
            "Camera > ${FC:96FF5F}Zoom${RESET}: ${FC:FFDB5F}" + CameraController.ZoomInButton + "${RESET} / ${FC:FFDB5F}" + CameraController.ZoomOutButton + "${RESET}" + Environment.NewLine +
            "Demo > Restart: ${FC:FFDB5F}" + _restartButton + "${RESET} GUI: ${FC:FFDB5F}" + _toggleUIButton + "${RESET} Fullscreen: ${FC:FFDB5F}" + _toggleFullscreenButton + "${RESET} Exit: ${FC:FFDB5F}" + _exitButton;

        public static Panel DescriptionPanel { get; private set; } = CreateDescriptionPanel();

        const Buttons _prevSceneButton = Buttons.Q;
        const Buttons _nextSceneButton = Buttons.E;
        const Buttons _restartButton = Buttons.F1;
        const Buttons _toggleUIButton = Buttons.F2;
        const Buttons _toggleFullscreenButton = Buttons.F3;
        const Buttons _exitButton = Buttons.Escape;

        Panel _postFXPanel;
        Button _postFXButton;
        Animation _postFXPanelAnimation;
        bool _postFXPanelVisible = false;
        readonly int _postFXPanelOffsetX = -302;

        bool _isUIDemo = false;
        bool _initialSceneCreated = false;

        Button _nextExampleButton;
        Button _previousExampleButton;
        static Title _title;
        static Paragraph _descriptionParagraph;
        static Paragraph _FPS_Paragraph;

        public List<SceneFactory> Factories = new()
        {
            //new SceneFactory(typeof(SpriterDemo)),
            new SceneFactory(typeof(UIDemo)),
            new SceneFactory(typeof(PrimitiveDemo), PrimitiveDemo.Description),
            new SceneFactory(typeof(ShapeDemo)),
            new SceneFactory(typeof(SpriteDemo)),
            new SceneFactory(typeof(ParticlesDemo), ParticlesDemo.Description),
            new SceneFactory(typeof(InputDemo), InputDemo.Description),
            new SceneFactory(typeof(ECDemo), ECDemo.Description),
            new SceneFactory(typeof(SceneSystemDemo), SceneSystemDemo.Description),
            new SceneFactory(typeof(UtilsDemo)),
            new SceneFactory(typeof(TiledDemo), TiledDemo.Description),
            new SceneFactory(typeof(VertexBatchDemo)),
            new SceneFactory(typeof(CoroutinesDemo)),
			new SceneFactory(typeof(CollisionsDemo)),
        };

		public int CurrentSceneID { get; private set; } = 0;
		public SceneFactory CurrentFactory => Factories[CurrentSceneID];

        CameraController _cameraController;

        public SceneSwitcher(CameraController cameraController) : base(SceneMgr.DefaultLayer)
		{
			_cameraController = cameraController;

            GUIMgr.OnThemeChanged = () => RestartScene();
        }

        public override void CreateUI()
        {
            base.CreateUI();

            if (!_initialSceneCreated)
            {
                _initialSceneCreated = true;
                CurrentFactory.CreateScene();
            }

            #region PostFX Panel

            if (CurrentFactory?.Type != typeof(UIDemo))
            {
                _postFXPanel = new(GUIMgr.System)
                {
                    Identifier = "PostFXPanel",
                    Anchor = Anchor.TopRight
                };
                _postFXPanel.Size.SetPixels(-_postFXPanelOffsetX, (int)GameMgr.WindowManager.CanvasSize.Y);
                _postFXPanel.Offset.X.SetPixels(_postFXPanelOffsetX);
                _postFXPanel.OverrideStyles.Padding = new Sides(0, 0, 20, 20);
                AddGUIEntity(_postFXPanel);

                _postFXPanelAnimation = new Animation()
                {
                    Easing = Easing.EaseInBounce,
                    Looping = false,
                    Speed = 1,
                    Invert = true
                };
                _postFXPanelAnimation.AnimationEndEvent += (e) =>
                {
                    _postFXPanelAnimation.Invert = !_postFXPanelAnimation.Invert;
                    if (_postFXPanelAnimation.Invert)
                    {
                        _postFXPanelAnimation.Easing = Easing.EaseInBounce;
                    }
                    else
                    {
                        _postFXPanelAnimation.Easing = Easing.EaseOutBounce;
                    }
                };

                _postFXButton = new Button(GUIMgr.System, "FX")
                {
                    Anchor = Anchor.TopRight
                };
                _postFXButton.OverrideStyles.TintColor = new Color(85, 85, 85, 255);
                _postFXButton.Size.SetPixels(100, 50);
                _postFXButton.Events.OnClick = (Entity control) =>
                {
                    _postFXPanelVisible = !_postFXPanelVisible;
                    if (!_postFXPanelAnimation.Running) _postFXPanelAnimation.Start(false);
                };
                AddGUIEntity(_postFXButton);

                _postFXPanel.AddChild(new Title(GUIMgr.System, "Post FX") { Anchor = Anchor.AutoCenter });
                _postFXPanel.AddChild(new HorizontalLine(GUIMgr.System));
                var postFXEnableButton = new Button(GUIMgr.System, "Post Processing")
                {
                    Anchor = Anchor.AutoCenter,
                    ToggleCheckOnClick = true,
                    Checked = RenderMgr.PostProcessing
                };
                postFXEnableButton.Size.SetPixels(300, 50);
                postFXEnableButton.Events.OnClick = (Entity control) =>
                {
                    RenderMgr.PostProcessing = !RenderMgr.PostProcessing;
                };
                _postFXPanel.AddChild(postFXEnableButton);

                #region Color Grading

                var colorGradingEnableButton = new Button(GUIMgr.System, "Color Grading")
                {
                    Anchor = Anchor.AutoCenter,
                    ToggleCheckOnClick = true,
                    Checked = RenderMgr.ColorGradingFX
                };
                colorGradingEnableButton.Size.SetPixels(300, 50);
                colorGradingEnableButton.OverrideStyles.MarginAfter = new Point(0, 5);
                colorGradingEnableButton.Events.OnClick = (Entity control) =>
                {
                    RenderMgr.ColorGradingFX = !RenderMgr.ColorGradingFX;
                };
                _postFXPanel.AddChild(colorGradingEnableButton);

                {
                    Panel panel = new(GUIMgr.System, null!)
                    {
                        Anchor = Anchor.AutoInlineLTR
                    };
                    panel.OverrideStyles.Padding = new Sides(1, 1, 1, 1);
                    panel.Size.SetPixels((int)_postFXPanel.Size.X.Value, 64);
                    _postFXPanel.AddChild(panel);

                    var logo = new Panel(GUIMgr.System, null!)
                    {
                        Anchor = Anchor.AutoInlineLTR
                    };
                    var logoTexture = ColorGrading.CurrentLUT[0].Texture;
                    GUIMgr.RegisterTexture(logoTexture, "Lut");

                    logo.OverrideStyles.Icon = new IconTexture
                    {
                        TextureId = "Lut",
                        TextureScale = 0.5f,
                        SourceRect = new Rectangle(0, 0, logoTexture.Width, logoTexture.Height)
                    };
                    logo.Size.SetPixels(logoTexture.Width, logoTexture.Height);
                    logo.Offset.X.SetPixels(10);

                    var leftButton = new Button(GUIMgr.System, "<")
                    {
                        Anchor = Anchor.AutoInlineLTR
                    };
                    leftButton.Size.SetPixels(64, 64);
                    leftButton.Offset.X.SetPixels(44);
                    leftButton.Events.OnClick = (Entity control) =>
                    {
                        ColorGrading.PreviousLUT();
                        GUIMgr.RegisterTexture(ColorGrading.CurrentLUT[0].Texture, "Lut");
                    };

                    var rightButton = new Button(GUIMgr.System, ">")
                    {
                        Anchor = Anchor.AutoInlineLTR
                    };
                    rightButton.Size.SetPixels(64, 64);
                    rightButton.Offset.X.SetPixels(10);
                    rightButton.Events.OnClick = (Entity control) =>
                    {
                        ColorGrading.NextLUT();
                        GUIMgr.RegisterTexture(ColorGrading.CurrentLUT[0].Texture, "Lut");
                    };

                    panel.AddChild(leftButton);
                    panel.AddChild(logo);
                    panel.AddChild(rightButton);
                }

                #endregion Color Grading Panel
                #region Bloom

                var bloomEnableButton = new Button(GUIMgr.System, "Bloom")
                {
                    Anchor = Anchor.AutoCenter,
                    ToggleCheckOnClick = true,
                    Checked = RenderMgr.BloomFX
                };
                bloomEnableButton.Size.SetPixels(300, 50);
                bloomEnableButton.OverrideStyles.MarginBefore = new Point(0, 5);
                bloomEnableButton.OverrideStyles.MarginAfter = new Point(0, 5);
                bloomEnableButton.Events.OnClick = (Entity control) =>
                {
                    RenderMgr.BloomFX = !RenderMgr.BloomFX;
                };
                _postFXPanel.AddChild(bloomEnableButton);

                {
                    Panel panel = new(GUIMgr.System, null!)
                    {
                        Anchor = Anchor.AutoInlineLTR
                    };
                    panel.OverrideStyles.Padding = new Sides(1, 1, 1, 1);
                    panel.Size.SetPixels((int)_postFXPanel.Size.X.Value, 64);
                    _postFXPanel.AddChild(panel);

                    var logo = new Panel(GUIMgr.System, null!)
                    {
                        Anchor = Anchor.AutoInlineLTR
                    };
                    var logoTexture = ResourceHub.GetResource<Sprite>("ParticleSprites", "Pixel")[0].Texture;
                    GUIMgr.RegisterTexture(logoTexture, "Bloom");
                    logo.OverrideStyles.Icon = new IconTexture
                    {
                        TextureId = "Bloom",
                        TextureScale = 0.5f,
                        SourceRect = new Rectangle(0, 0, 64, 64)
                    };
                    logo.Size.SetPixels(64, 64);
                    logo.Offset.X.SetPixels(10);

                    var leftButton = new Button(GUIMgr.System, "<")
                    {
                        Anchor = Anchor.AutoInlineLTR
                    };
                    leftButton.Size.SetPixels(64, 64);
                    leftButton.Offset.X.SetPixels(44);
                    leftButton.Events.OnClick = (Entity control) =>
                    {
                        Bloom.PreviousPreset();
                    };

                    var rightButton = new Button(GUIMgr.System, ">")
                    {
                        Anchor = Anchor.AutoInlineLTR
                    };
                    rightButton.Size.SetPixels(64, 64);
                    rightButton.Offset.X.SetPixels(10);
                    rightButton.Events.OnClick = (Entity control) =>
                    {
                        Bloom.NextPreset();
                    };

                    panel.AddChild(leftButton);
                    panel.AddChild(logo);
                    panel.AddChild(rightButton);

                    panel.AddChild(new Title(GUIMgr.System, "Threshold") { Anchor = Anchor.AutoCenter });
                    {
                        var slider = new Slider(GUIMgr.System)
                        {
                            MinValue = 0,
                            MaxValue = 100,
                            Value = (int)(100 * Bloom.Threshold)
                        };
                        slider.Events.OnValueChanged = (Entity control) =>
                        {
                            Bloom.Threshold = MathF.Min(((Slider)control).Value / 100f, 0.99f);
                        };
                        panel.AddChild(slider);
                    }
                    {
                        panel.AddChild(new Title(GUIMgr.System, "Streak") { Anchor = Anchor.AutoCenter });
                        var slider = new Slider(GUIMgr.System)
                        {
                            MinValue = 0,
                            MaxValue = 30,
                            Value = (int)((10 * Bloom.StreakLength) / Bloom.StreakLength)
                        };
                        slider.Events.OnValueChanged = (Entity control) =>
                        {
                            Bloom.StreakLength = MathF.Min((((Slider)control).Value / 100f) * 10f, 3f);
                        };
                        panel.AddChild(slider);
                    }
                }

                #endregion Bloom
            }
            #endregion PostFX Panel
            #region Bottom Panel

            var sceneDescription = Description;
            var hasDescription = CurrentFactory.Description != string.Empty;
            if (hasDescription) sceneDescription = CurrentFactory.Description;

            int panelHeight = 256;
            _isUIDemo = false;
            if (CurrentFactory?.Type == typeof(UIDemo))
            {
                _isUIDemo = true;
                panelHeight = 64;
            }

            Panel bottomPanel = new(GUIMgr.System, _isUIDemo ? null! : GUIMgr.System.DefaultStylesheets.Panels)
            {
                Anchor = Anchor.BottomCenter,
                Identifier = "BottomPanel"
            };
            if (!_isUIDemo) bottomPanel.OverrideStyles.Padding = Sides.Zero;
            bottomPanel.Size.SetPixels((int)GameMgr.WindowManager.CanvasSize.X, panelHeight);
            AddGUIEntity(bottomPanel);

            _previousExampleButton = new Button(GUIMgr.System, $"<- ({_prevSceneButton}) Back")
            {
                Anchor = Anchor.CenterLeft
            };
            _previousExampleButton.Size.SetPixels(250, (int)bottomPanel.Size.Y.Value);
            _previousExampleButton.Events.OnClick = (Entity btn) => { PreviousScene(); };
            bottomPanel.AddChild(_previousExampleButton);

            if (!_isUIDemo)
            {
                //Scene Name
                {
                    DescriptionPanel.Anchor = Anchor.AutoInlineLTR;
                    DescriptionPanel.Size.X.SetPixels((int)bottomPanel.Size.X.Value - 500);

                    if (_FPS_Paragraph == null)
                    {
                        _FPS_Paragraph = new Paragraph(GUIMgr.System, "")
                        {
                            Anchor = Anchor.TopRight
                        };
                        _FPS_Paragraph.Offset.Y.SetPixels(30);
                        DescriptionPanel.AddChild(_FPS_Paragraph, 0);
                    }

                    if (_title == null)
                    {
                        _title = new Title(GUIMgr.System, CurrentFactory.Type.Name) { Anchor = Anchor.AutoCenter };
                        _title.Offset.Y.SetPixels(-40);

                        _descriptionParagraph = new Paragraph(GUIMgr.System, sceneDescription);

                        DescriptionPanel.AddChild(_title, 1);
                        DescriptionPanel.AddChild(new HorizontalLine(GUIMgr.System), 2);
                        DescriptionPanel.AddChild(_descriptionParagraph, 3);
                    }
                    else
                    {
                        _title.Text = CurrentFactory.Type.Name;
                        _descriptionParagraph.Text = sceneDescription;
                    }
                    DescriptionPanel.RemoveSelf();
                    bottomPanel.AddChild(DescriptionPanel);
                }
            }

            _nextExampleButton = new Button(GUIMgr.System, $"({_nextSceneButton}) Next ->")
            {
                Anchor = Anchor.CenterRight
            };
            _nextExampleButton.Size.SetPixels(250, (int)bottomPanel.Size.Y.Value);
            _nextExampleButton.Events.OnClick = (Entity btn) => { NextScene(); };
            bottomPanel.AddChild(_nextExampleButton);

            #endregion Bottom Panel
        }

        public override void Update()
		{
			base.Update();

            if (_postFXPanel != null)
            {
                _postFXPanelAnimation.Update();
                if (_postFXPanelAnimation.Running)
                {
                    _postFXPanel.Offset.X.SetPixels(_postFXPanelOffsetX * (float)_postFXPanelAnimation.Progress);
                    _postFXButton.Offset.X.SetPixels(_postFXPanel.Offset.X.Value - _postFXPanelOffsetX);
                }
            }

            if (Input.CheckButtonPress(_toggleUIButton))
			{
                GUIMgr.System.Root.Visible = !GUIMgr.System.Root.Visible;
            }

            if (Input.CheckButtonPress(_restartButton))
			{
				RestartScene();
			}

			if (Input.CheckButtonPress(_nextSceneButton))
			{
                if (_isUIDemo && UIDemo.HasTextInput) return;

				NextScene();
			}

			if (Input.CheckButtonPress(_prevSceneButton))
			{
                if (_isUIDemo && UIDemo.HasTextInput) return;

                PreviousScene();
			}

			if (Input.CheckButtonPress(_toggleFullscreenButton))
			{
				GameMgr.WindowManager.ToggleFullScreen();
			}

            if (Input.CheckButtonPress(_exitButton))
            {
                GameMgr.ExitGame();
            }

            if (_FPS_Paragraph != null) _FPS_Paragraph.Text = "FPS: ${FC:FFFF00}" + GameMgr.FPS + "${RESET}";
        }

		public void NextScene()
        {
            CurrentFactory.DestroyScene();

            CurrentSceneID += 1;
			if (CurrentSceneID >= Factories.Count)
			{
				CurrentSceneID = 0;
            }

            CurrentFactory.CreateScene();

            CreateUI();

            if (CurrentFactory?.Type == typeof(UIDemo))
            {
                UIDemo.ResetCurrentExample();
            }
            else if (CurrentFactory?.Type != typeof(ParticlesDemo))
            {
                CleanDescriptionPanel();
            }

             _cameraController.Reset();
        }

		public void PreviousScene()
		{
            CurrentFactory.DestroyScene();

            CurrentSceneID -= 1;
			if (CurrentSceneID < 0)
			{
				CurrentSceneID = Factories.Count - 1;
            }

            CurrentFactory.CreateScene();

            CreateUI();

            if (CurrentFactory?.Type == typeof(UIDemo))
            {
                UIDemo.ResetCurrentExample();
            }
            else if (CurrentFactory?.Type != typeof(ParticlesDemo))
            {
                CleanDescriptionPanel();
            }

            _cameraController.Reset();
        }

		public void RestartScene()
		{
            CreateDescriptionPanel();

            CurrentFactory.DestroyScene();

            CurrentFactory.CreateScene();

            CreateUI();

            if (CurrentFactory?.Type == typeof(UIDemo))
            {
                UIDemo.ResetCurrentExample();
            }
            else if (CurrentFactory?.Type != typeof(ParticlesDemo))
            {
                CleanDescriptionPanel();
            }

            _cameraController.Reset();
        }

        private void CleanDescriptionPanel()
        {
            DescriptionPanel.IterateChildren(
                x =>
                {
                    if (!string.IsNullOrEmpty(x.Identifier) && x.Identifier.Equals("extra"))
                    {
                        x.RemoveSelf();
                    }
                    return true;
                });
        }

        private static Panel CreateDescriptionPanel()
        {
            if (DescriptionPanel != null)
            {
                _title = null;
                _descriptionParagraph = null;
                _FPS_Paragraph = null;
                DescriptionPanel.ClearChildren();
                DescriptionPanel.RemoveSelf();
            }
            DescriptionPanel = new(GUIMgr.System, null!) { Identifier = "DescriptionPanel" };
            return DescriptionPanel;
        }
    }
}
