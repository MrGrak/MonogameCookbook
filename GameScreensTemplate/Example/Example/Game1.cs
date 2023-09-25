using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Example
{
    public class Game1 : Game
    {
        public Game1()
        {
            ScreenManager.Graphics = new GraphicsDeviceManager(this);
            ScreenManager.CM = Content;
            ScreenManager.CM.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            ScreenManager.SB = new SpriteBatch(GraphicsDevice);
            ScreenManager.Window = Window;
            ScreenManager.Init();
        }

        protected override void Update(GameTime gameTime)
        {
            ScreenManager.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            ScreenManager.Draw();
            base.Draw(gameTime);
        }
    }
}