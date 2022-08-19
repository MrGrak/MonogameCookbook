using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Game1
{
    //reading from l1 cache vs stack allocation test on vector2s
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Stopwatch St;

        //8 bytes * 1024 = 8192 bytes (should all fit into l1 cache)
        static int size = 1024;
        Vector2[] vectors = new Vector2[size];
        //values to store times
        long justLoop, stackAllocTime, l1ReadTime = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            St = new Stopwatch();
        }

        protected override void Initialize() { base.Initialize(); }
        protected override void LoadContent() { spriteBatch = new SpriteBatch(GraphicsDevice); }
        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            St.Restart();
            //first, try to place all vectors into l1 cache
            for (int i = 0; i < size; i++)
            {   //do some work that forces compiler not to optimize this code away
                vectors[i].X = i;
            }
            St.Stop();
            justLoop = St.ElapsedTicks;

            //second, test clamp on all vectors
            St.Restart();
            for (int i = 0; i < size; i++)
            {   //this involves stack allocation via return value
                vectors[i] = Clamp(vectors[i], Vector2.Zero, Vector2.One);
            }
            St.Stop();
            stackAllocTime = St.ElapsedTicks;

            St.Restart();
            for (int i = 0; i < size; i++)
            {   //this involves mutating value in l1/register
                Clamp(ref vectors[i], Vector2.Zero, Vector2.One, out vectors[i]);
            }
            St.Stop();
            l1ReadTime = St.ElapsedTicks;

            //write times to window title, formatted nicely
            Window.Title = "JUST LOOP: " + justLoop.ToString("D4") + 
                "  STACK ALLOC: " + stackAllocTime.ToString("D4") + 
                "  L1READ: " + l1ReadTime.ToString("D4");

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }

        public static Vector2 Clamp(Vector2 value1, Vector2 min, Vector2 max)
        {
            return new Vector2(
                MathHelper.Clamp(value1.X, min.X, max.X),
                MathHelper.Clamp(value1.Y, min.Y, max.Y));
        }

        public static void Clamp(ref Vector2 value1, Vector2 min, Vector2 max, out Vector2 result)
        {   //note this method slightly differs from mg's implementation since min and max aren't refs
            result.X = MathHelper.Clamp(value1.X, min.X, max.X);
            result.Y = MathHelper.Clamp(value1.Y, min.Y, max.Y);
        }

    }
}