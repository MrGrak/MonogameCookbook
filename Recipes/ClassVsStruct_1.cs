using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Game1
{
    //define a class and a struct for a basic 2d particle
    public class CParticle
    {
        public Vector2 position;
        public CParticle(Vector2 pos)
        { position = pos; }
    }

    public struct SParticle
    {
        public Vector2 position;
        public SParticle(Vector2 pos)
        { position = pos; }
    }

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //define arrays of structs vs classes
        static int size = 1000000;
        SParticle[] structArray = new SParticle[size];
        CParticle[] classArray = new CParticle[size];
        float[] floatArray = new float[size];
        float gravity = 0.9f;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
            //initialize arrays to size
            for(int i = 0; i < size; i++)
            {
                structArray[i] = new SParticle(new Vector2(100, 100));
                classArray[i] = new CParticle(new Vector2(100, 100));
                floatArray[i] = 100f;
            }
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }
        
        protected override void UnloadContent() { }

        int frames = 0;
        Stopwatch stopwatch = new Stopwatch();
        long classTime = 0;
        long structTime = 0;
        long floatTime = 0;

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //periodically time gravity being applied to particles
            frames++;
            if(frames == 60 * 2)
            {
                frames = 0;

                stopwatch.Restart();
                //loop class array, updating positions
                for (int i = 0; i < size; i++)
                {
                    classArray[i].position.Y += gravity;
                    if (classArray[i].position.Y > 1024)
                    { classArray[i].position.Y = 0; }
                }
                stopwatch.Stop();
                classTime = stopwatch.ElapsedTicks;

                stopwatch.Restart();
                //loop struct array, updating positions
                for(int i = 0; i < size; i++)
                {
                    structArray[i].position.Y += gravity;
                    if (structArray[i].position.Y > 1024)
                    { structArray[i].position.Y = 0; }
                }
                stopwatch.Stop();
                structTime = stopwatch.ElapsedTicks;

                stopwatch.Restart();
                //loop float array, updating positions
                for (int i = 0; i < size; i++)
                {
                    floatArray[i] += gravity;
                    if (floatArray[i] > 1024)
                    { floatArray[i] = 0; }
                }
                stopwatch.Stop();
                floatTime = stopwatch.ElapsedTicks;

                //what took the longest? class, struct, float
                Debug.WriteLine("\nclass array time: " + classTime);
                Debug.WriteLine("struct array time: " + structTime);
                Debug.WriteLine("float array time: " + floatTime);
            }
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}