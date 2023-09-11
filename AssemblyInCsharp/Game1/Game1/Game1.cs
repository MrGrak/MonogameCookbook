using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    public class Game1 : Game
    {
        public Game1()
        {
            Prototype.Game = this;
            Prototype.Constructor();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Prototype.LoadContent();
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Prototype.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            Prototype.Draw();
            base.Draw(gameTime);
        }
    }
}