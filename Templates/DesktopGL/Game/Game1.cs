using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGo.Engine;
using MonoGo.Iguina;
using System.IO;

namespace MGNamespace
{
    // Inherit from MonoGoGame to start using the MonoGo Engine!
    public class Game1 : MonoGoGame
    {
        protected override void Initialize()
        {
            base.Initialize();

            // This is just a sample.
            // Remove or modify the GameController and SplashScreen as you wish!
            new GameController(new Vector2(1280, 720), "MonoGo");
            new SplashScreen();
        }

        protected override void LoadContent()
        {
            // IMPORTANT: Don't delete! It loads engine specific stuff.
            LoadEngineContent();

            GUIMgr.Init(Path.Combine(GameMgr.ContentDirectory, "Game/GUI"), "MonoGoTheme");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            GameMgr.WindowManager.WindowTitle =
               $"FPS: {GameMgr.FPS} | UPS: {GameMgr.UPS} | Update: {GameMgr.LastUpdateMs:0.00} ms | Draw: {GameMgr.LastDrawMs:0.00} ms";

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
