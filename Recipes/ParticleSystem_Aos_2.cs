using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;

namespace Game1
{
    //for this version, the particle struct was reduced in size to 32 bytes, 
    //and alignment was fixed (hopefully) - in addition, a horizontal bounce
    //was added so more particles stay on screen, which leads to interesting
    //patterns. a slight randomness was added to spawn starting acceleration
    //which helps to break up particles in an interesting way
    //mit license mrgrak2021

    public static class Data
    {
        public static GraphicsDeviceManager GDM;
        public static ContentManager CM;
        public static SpriteBatch SB;
        public static Game1 GAME;
    }

    public enum ParticleID : byte { Inactive, Fire }

    public struct Particle
    {   //32 bytes, aligned on 4 bytes
        public ParticleID Id;
        public Color color;
        public float X, Y; //current position
        public float accX, accY; //accelerations
        public float preX, preY; //last position
    }

    public static class ParticleSystem
    {
        static Random Rand = new Random();
        public static Texture2D texture;
        public static Rectangle drawRec = new Rectangle(0, 0, 3, 3);
        public static float gravity = 0.1f;
        public static float windCounter = 0;
        public static int lastActive = 0;

        public const int size = 50000;
        public static Particle[] data = new Particle[size];


        public static void Reset()
        {   
            //acts as a constructor as well
            if (texture == null)
            {   //set up a general texture we can draw dots with, if required
                texture = new Texture2D(Data.GDM.GraphicsDevice, 1, 1);
                texture.SetData<Color>(new Color[] { Color.White });
            }
            //reset particles to inactive state
            for (int i = 0; i < size; i++)
            { data[i].Id = ParticleID.Inactive; }
            lastActive = 0; //reset index too
        }

        public static void Spawn(
            ParticleID ID,          //type of particle to spawn
            float X, float Y,       //spawn x, y position 
            float accX, float accY  //initial x, y acceleration
            )
        {   
            Particle P = new Particle();
            //setup P based on parameters
            P.X = X; P.preX = X;
            P.Y = Y; P.preY = Y;
            P.accX = accX;
            P.accY = accY;
            P.color = Color.White;
            P.color.A = 255;
            //pass ID
            P.Id = ID;

            //setup particle based on ID
            if (P.Id == ParticleID.Fire)
            {   //fire starts red
                P.color = Color.White;
            }

            //save P to heap via marker
            data[lastActive] = P;
            lastActive++;
            //bound dead index to array size
            if (lastActive >= size)
            { lastActive = size - 1; }
        }

        public static void Update()
        {   //step 1 - update/animate all the particles
            //step 2 - randomly spawn fire and rain particles

            //increase wind counter
            windCounter += 0.015f;
            //get left or right horizontal value for wind
            float wind = (float)Math.Sin(windCounter) * 0.03f;

            //shorten these for later use
            int width = Data.GDM.GraphicsDevice.Viewport.Width;
            int height = Data.GDM.GraphicsDevice.Viewport.Height;

            for (int i = lastActive - 1; i >= 0; i--)
            {   

                //here we're hinting to compiler that we'd
                //like a stack copy of the data, might be ignored
                Particle P = data[i];

                bool isAlive = true;
                

                //affect particles based on id
                if (P.Id == ParticleID.Fire)
                {   //fire rises, gravity does not affect it
                    P.accY -= gravity;
                    P.accY -= 0.02f;
                    //animate color to be darker
                    P.color.G-=1;
                    P.color.B-=1;
                }
                //else if (P.Id == ParticleID.Rain)
                {   //rain falls (at different speeds)
                    //P.accY = Rand.Next(0, 100) * 0.001f;
                }
                
                //push particles off sides of screen horizontally
                if (P.X < 0) { P.accX += 0.2f; P.color.G -= 10; P.color.B -= 10; }
                else if (P.X > width) { P.accX -= 0.2f; P.color.G -= 10; P.color.B -= 10; }

                //cull particle if it goes off screen vertically
                if (P.Y < 0 || P.Y > height)
                { isAlive = false; }

                if (isAlive)
                {
                    //add gravity to push down
                    P.accY += gravity;
                    //add wind to push left/right
                    P.accX += wind;

                    //calculate velocity using current and previous pos
                    float velocityX = P.X - P.preX;
                    float velocityY = P.Y - P.preY;

                    //store previous positions (the current positions)
                    P.preX = P.X;
                    P.preY = P.Y;

                    //set next positions using current + velocity + acceleration
                    P.X = P.X + velocityX + P.accX;
                    P.Y = P.Y + velocityY + P.accY;

                    //clear accelerations
                    P.accX = 0; P.accY = 0;

                    //write local to heap
                    data[i] = P;
                }
                else
                {   //deactivate particle
                    if (i < lastActive - 1)
                    {
                        data[i] = data[lastActive - 1];
                        lastActive--;
                    }
                    else { lastActive--; }
                }
                
            }

            // create particles and disallow creation from exceeding buffer limit.
            int numberOfParticlesToSpawn = 100;
            if (lastActive + numberOfParticlesToSpawn < size)
            {
                for (int i = 0; i < numberOfParticlesToSpawn; i++)
                {   //step 2 - randomly spawn fire
                    Spawn(ParticleID.Fire, 
                        Rand.Next(0 + 100, width - 101), height, 
                        0, Rand.Next(-100, 0) * 0.01f);
                }
            }
        }

        public static void Draw()
        {   //open spritebatch and draw active particles
            Data.SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            int s = lastActive; //size
            for (int i = 0; i < s; i++)
            {   //if particle is active, draw it
                if (data[i].Id > 0)
                {  
                    Vector2 pos = new Vector2(data[i].X, data[i].Y);
                    Data.SB.Draw(texture,
                        pos,
                        drawRec,
                        data[i].color,
                        0,
                        Vector2.Zero,
                        1.0f, //scale
                        SpriteEffects.None,
                        i * 0.00001f);
                }   //layer particles based on index
            }

            Data.SB.End();
        }
    }

    public class Game1 : Game
    {
        public static Stopwatch timer = new Stopwatch();

        public Game1()
        {
            Data.GDM = new GraphicsDeviceManager(this);
            Data.GDM.GraphicsProfile = GraphicsProfile.HiDef;
            Data.CM = Content;
            Data.GAME = this;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;


        }

        protected override void Initialize() { base.Initialize(); }

        protected override void LoadContent()
        {
            Data.SB = new SpriteBatch(GraphicsDevice);
            ParticleSystem.Reset();
            
            Data.GDM.PreferredBackBufferWidth = 600;
            Data.GDM.PreferredBackBufferHeight = 800;
            IsMouseVisible = true;
            Data.GDM.ApplyChanges();
        }

        protected override void UnloadContent() { } //lol

        protected override void Update(GameTime gameTime)
        {
            timer.Restart();
            base.Update(gameTime);
            ParticleSystem.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
            ParticleSystem.Draw();
            timer.Stop();

            ///* this wont work for all platforms
            Data.GAME.Window.Title =
                "AoS Particle System Example by @_mrgrak" +
                "- ms: " + timer.ElapsedMilliseconds +
                " - total particles: " + ParticleSystem.lastActive +
                " / " + ParticleSystem.size;
            //*/

        }
    }
}