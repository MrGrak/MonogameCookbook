using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    //two cheap blackholes push particles around in interesting patterns
    //press left mouse click to start sim

    //mit license mrgrak2021

    public static class Data
    {
        public static GraphicsDeviceManager GDM;
        public static ContentManager CM;
        public static SpriteBatch SB;
        public static Game1 GAME;
    }

    public enum ParticleID : byte { Inactive, Active }

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
        public const int size = 100000;
        public static Particle[] data = new Particle[size];

        static Random Rand = new Random();
        public static Texture2D texture;
        public static Rectangle drawRec = new Rectangle(0, 0, 3, 3);
        public static float gravity = 0.1f;
        public static float windCounter = 0;
        public static int lastActive = 0;
        public static MouseState MS;
        public static byte SimulationState = 0;


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


            
            //spawn all particles in random locations on screen
            for (int i = 0; i < size; i++)
            {
                Spawn(ParticleID.Active,
                    Rand.Next(0, 1280),
                    Rand.Next(0, 720),
                    0, 0);
            }
        }

        public static void Spawn(
            ParticleID ID,          //type of particle to spawn
            float X, float Y,       //spawn x, y position 
            float accX, float accY  //initial x, y acceleration
            )
        {
            Particle P = new Particle();
            P.X = X; P.preX = X;
            P.Y = Y; P.preY = Y;
            P.accX = accX;
            P.accY = accY;
            P.color = Color.Black;

            //change color based on spawn pos
            //if (X < 1280 / 3) { P.color.R = 255; }
            //else if (X < (1280 / 3) * 2) { P.color.G = 255; }
            //else { P.color.B = 255; }

            //P.color.R = (byte)(X * 3);
            //P.color.G = (byte)(255 - Y);
            //P.color.B = (byte)(255 - X);

            //P.color.B = (byte)((X * 255) / 255);

            //add some randomness to blue channel
            P.color.B += (byte)Rand.Next(0, 100);
            P.color.R += (byte)Rand.Next(0, 100);
            P.color.G += (byte)Rand.Next(150, 255);

            P.Id = ID;

            //save P to heap
            data[lastActive] = P;
            lastActive++;
            //bound dead index to array size
            if (lastActive >= size)
            { lastActive = size - 1; }
        }

        public static void Update()
        {
            //get mouse state
            MS = Mouse.GetState();

            /*
            if (MS.LeftButton == ButtonState.Pressed)
            {
                Spawn(ParticleID.Active, MS.X, MS.Y, 0, 0);
                for (int i = 0; i < 10; i++)
                {
                    Spawn(ParticleID.Active,
                    MS.X + Rand.Next(-4, 5), MS.Y + Rand.Next(-4, 5), 0, 0);
                }

            }
            */

            //player must press left mouse btn to start sim
            if (MS.LeftButton == ButtonState.Pressed) { SimulationState = 1; }
            if(SimulationState == 0) { return; }

            //reset with r click
            if (MS.RightButton == ButtonState.Pressed)
            {
                if(SimulationState == 1)
                {
                    SimulationState = 0;
                    Reset();
                }
            }


            //shorten these for later use
            int width = Data.GDM.PreferredBackBufferWidth;
            int height = Data.GDM.PreferredBackBufferHeight;
            


            #region Step 1 - update particle system
            
            for (int i = lastActive - 1; i >= 0; i--)
            {

                //here we're hinting to compiler that we'd
                //like a stack copy of the data, might be ignored
                Particle P = data[i];

                bool isAlive = true;


                
                //bound to screen
                if (P.X < 0) { P.X = 0; }
                else if (P.X > width) { P.X = width; P.accX = 0; }

                if (P.Y < 0) { P.Y = 0; }
                else if (P.Y > height) { P.Y = height; P.accY = 0; }

                //cull particle if it goes off screen vertically
                //if (P.Y < 0 || P.Y > height) { isAlive = false; }


                if (isAlive)
                {

                    #region Pull Towards Mouse
                    /*
                    {
                        float distX = P.X - MS.X;
                        float distY = P.Y - MS.Y;
                        float distance = (float)Math.Sqrt((distX * distX) + (distY * distY));

                        //very cheap radial gravity
                        P.accX -= distX / distance * 0.03f;
                        P.accY -= distY / distance * 0.03f;

                        if (distance <= 100) //radius of influence
                        {
                            //push away from center
                            if (MS.X < P.X)
                            { P.accX += 1.1f; }
                            else { P.accX -= 1.1f; }
                            if (MS.Y < P.Y)
                            { P.accY += 1.1f; }
                            else { P.accY -= 1.1f; }
                        }
                    }
                    */
                    #endregion



                    #region Pull Towards Specific Positions

                    {
                        int X = 0;
                        int Y = height / 2;
                        float distX = P.X - X;
                        float distY = P.Y - Y;
                        float distance = (float)Math.Sqrt((distX * distX) + (distY * distY));

                        //very cheap radial gravity
                        P.accX -= distX / distance * 0.03f;
                        P.accY -= distY / distance * 0.03f;

                        if (distance <= 100) //radius of influence
                        {
                            //push away from center
                            if (X < P.X)
                            { P.accX += 1.1f; }
                            else { P.accX -= 1.1f; }
                            if (Y < P.Y)
                            { P.accY += 1.1f; }
                            else { P.accY -= 1.1f; }
                        }
                    }

                    {
                        int X = width;
                        int Y = height / 2;
                        float distX = P.X - X;
                        float distY = P.Y - Y;
                        float distance = (float)Math.Sqrt((distX * distX) + (distY * distY));

                        //very cheap radial gravity
                        P.accX -= distX / distance * 0.03f;
                        P.accY -= distY / distance * 0.03f;

                        if (distance <= 100) //radius of influence
                        {
                            //push away from center
                            if (X < P.X)
                            { P.accX += 1.1f; }
                            else { P.accX -= 1.1f; }
                            if (Y < P.Y)
                            { P.accY += 1.1f; }
                            else { P.accY -= 1.1f; }
                        }
                    }

                    #endregion




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

            #endregion


            /*
            //step 2 - create new particles
            int numberOfParticlesToSpawn = 100;
            if (lastActive + numberOfParticlesToSpawn < size)
            {
                for (int i = 0; i < numberOfParticlesToSpawn; i++)
                {   
                    Spawn(ParticleID.Active,
                        Rand.Next(0, width),
                        Rand.Next(0, height), 0, 0);
                    
                }
            }
            */

        }

        public static void Draw()
        {   //open spritebatch and draw active particles
            Data.SB.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp);

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

            Data.GDM.PreferredBackBufferWidth = 1280;
            Data.GDM.PreferredBackBufferHeight = 720;

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
            GraphicsDevice.Clear(Color.Black);
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