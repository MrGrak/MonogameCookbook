using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace BodySim
{
    //mrgrak2025

    public static class Data
    {
        public static GraphicsDeviceManager GDM;
        public static ContentManager CM;
        public static SpriteBatch SB;
        public static Game1 GAME;
    }

    public struct Body
    {
        public Vector2 pos;
        public Vector2 posPrev;
        public float mass;
    }

    public static class BodySim
    {
        public const int size = 300;

        public static Body[] bodies = new Body[size];
        public static Vector2[] accelerations = new Vector2[size];

        public static MouseState MS;
        public static MouseState MSprev;
        static Random Rand = new Random();
        public static Texture2D texture;
        public static Rectangle bodyRec = new Rectangle(0, 0, 5, 5);

        public static void Reset()
        {
            //acts as a constructor as well
            if (texture == null)
            {   //set up a general texture we can draw dots with, if required
                texture = new Texture2D(Data.GDM.GraphicsDevice, 1, 1);
                texture.SetData<Color>(new Color[] { Color.White });
            }

            //reset bodies to inactive state
            for (int i = 0; i < size; i++) { bodies[i].mass = -1.0f; }

            //spawn bodies randomly all over screen
            for (int i = 0; i < size; i++)
            {
                //Spawn(Rand.Next(0, 1280), Rand.Next(0, 720));
            }
        }

        public static void Spawn(float X, float Y)
        {
            for (int i = 0; i < size; i++) 
            { 
                if (bodies[i].mass <= 0.0f) //find inactive
                {
                    bodies[i].pos.X = X;
                    bodies[i].pos.Y = Y;
                    bodies[i].posPrev.X = X;
                    bodies[i].posPrev.Y = Y;
                    bodies[i].mass = Rand.Next(1, 20) * 1.0f;
                    return;
                }
            }
        }

        public static void Update()
        {
            //set last mouse state
            MSprev = MS;

            //get new mouse state
            MS = Mouse.GetState();
            
            //reset with rmb
            if (MS.RightButton == ButtonState.Pressed && MSprev.RightButton == ButtonState.Released) 
            { 
                Reset(); 
                return; 
            }

            //spawn new body with lmb
            if (MS.LeftButton == ButtonState.Pressed && MSprev.LeftButton == ButtonState.Released) 
            {
                Spawn(MS.Position.X, MS.Position.Y);
            }




            //shorten these for later use
            int width = Data.GDM.PreferredBackBufferWidth;
            int height = Data.GDM.PreferredBackBufferHeight;

            //newton's gravitational constant
            //what should this value be at this pixel scale? lmao who knows
            float G = 1.0f; //tuned so this isn't needed
            float dampening = 0.01f; //reduces accelerations
            float c = 0.01f; //max acceleration / light speed

            //update each body
            for (int i = 0; i < size; i++)
            {
                if (bodies[i].mass > 0) //body must be active
                {

                    #region calculate pull from all bodies

                    for (int j = 0; j < size; j++)
                    {
                        if (i != j) //no self check
                        {
                            if (bodies[j].mass > 0) //body must be active
                            {
                                Vector2 dir = Vector2.Subtract(bodies[j].pos, bodies[i].pos);
                                float radius = dir.Length();

                                //this kinda controls how much swirling happens before bodies merge
                                float bodyRadius = 0.2f;

                                //check for body collision
                                if (radius < bodyRadius)
                                {
                                    //just give it to i over j by default for speed
                                    bodies[i].mass += bodies[j].mass;
                                    bodies[j].mass = 0.0f;
                                }
                                else
                                {
                                    //apply newtons law of gravity
                                    float force = G * ((bodies[i].mass * bodies[j].mass) / ((float)Math.Pow(radius, 2)));

                                    //we could swap these
                                    float acceleration = force / bodies[i].mass;
                                    //float acceleration = bodies[i].mass / force;

                                    acceleration *= dampening;

                                    //limit acceleration
                                    if (acceleration > c) { acceleration = c; }
                                    else if (acceleration < -c) { acceleration = -c; }

                                    dir.Normalize();
                                    dir = Vector2.Multiply(dir, acceleration);

                                    accelerations[i] += dir;
                                }
                            }
                        }
                    }

                    #endregion


                    #region bound bodies

                    if (bodies[i].pos.X < 5)
                    {
                        bodies[i].pos.X = 5;
                        bodies[i].posPrev.X = bodies[i].pos.X;
                    }
                    else if (bodies[i].pos.X > width - 5)
                    {
                        bodies[i].pos.X = width - 5;
                        bodies[i].posPrev.X = bodies[i].pos.X;
                    }

                    if (bodies[i].pos.Y < 5)
                    {
                        bodies[i].pos.Y = 5;
                        bodies[i].posPrev.Y = bodies[i].pos.Y;
                    }
                    else if (bodies[i].pos.Y > height - 5)
                    {
                        bodies[i].pos.Y = height - 5;
                        bodies[i].posPrev.Y = bodies[i].pos.Y;
                    }

                    #endregion


                    //calculate velocity using current and previous pos
                    float velocityX = bodies[i].pos.X - bodies[i].posPrev.X;
                    float velocityY = bodies[i].pos.Y - bodies[i].posPrev.Y;

                    //store previous positions (the current positions)
                    bodies[i].posPrev.X = bodies[i].pos.X;
                    bodies[i].posPrev.Y = bodies[i].pos.Y;

                    //set next positions with velocity + acceleration
                    bodies[i].pos.X += velocityX + accelerations[i].X;
                    bodies[i].pos.Y += velocityY + accelerations[i].Y;

                    //clear accelerations
                    accelerations[i].X = 0; accelerations[i].Y = 0;


                }
            }
        }

        public static void Draw()
        {   
            Data.SB.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp);

            Color drawCol = new Color(0, 255, 0);
            bodyRec.Width = 3;
            bodyRec.Height = 3;

            int s = size;
            for (int i = 0; i < s; i++)
            {   
                if (bodies[i].mass > 0.0f) //if particle is active, draw it
                {
                    Data.SB.Draw(texture,
                        bodies[i].pos + new Vector2(-bodyRec.Width/2, -bodyRec.Height/2), //center rec to pos
                        bodyRec,
                        drawCol,
                        0,
                        Vector2.Zero,
                        1.0f, //scale
                        SpriteEffects.None,
                        i * 0.00001f);
                }   
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
            BodySim.Reset();

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
            BodySim.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
            BodySim.Draw();
            timer.Stop();

            try
            {
                //this wont work for all platforms
                Data.GAME.Window.Title = "ms: " + timer.ElapsedMilliseconds;
            }
            catch(Exception e){ }
        }
    }
}