using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Game1
{
    //flappy bird demo
    //mrgrak2025

    public static class Data
    {
        public static GraphicsDeviceManager GDM;
        public static ContentManager CM;
        public static SpriteBatch SB;
        public static Game1 GAME;
    }

    public static class BodySim
    {
        public const int size = 7;
        public static Rectangle[] pipes = new Rectangle[size];

        public static Rectangle bird = new Rectangle(0, 0, 50, 25);
        public static float bird_Ypos;
        public static float bird_Ypos_prev;
        public static float birdAcceleration;

        public static MouseState MS;
        public static MouseState MSprev;
        static Random Rand = new Random();
        public static Texture2D texture;
        



        public static void Reset()
        {
            if (texture == null)
            {   //set up a general texture we can draw dots with, if required
                texture = new Texture2D(Data.GDM.GraphicsDevice, 1, 1);
                texture.SetData<Color>(new Color[] { Color.White });
            }

            bird.X = 100;
            bird_Ypos = 720 / 2;
            bird_Ypos_prev = bird_Ypos;

            //place pipes
            for (int i = 0; i < size; i++)
            {
                pipes[i].X = 200 + (i * 186);
                pipes[i].Y = 0;
                pipes[i].Width = 50;
                pipes[i].Height = Rand.Next(50, 300);
            }
        }

        public static void Update()
        {
            //set last mouse state
            MSprev = MS;
            //get new mouse state
            MS = Mouse.GetState();

            //increase bird Y position with mouse btn press
            if (MS.LeftButton == ButtonState.Pressed 
                && MSprev.LeftButton == ButtonState.Released)
            {
                birdAcceleration -= 5;
            }
            
            //apply gravity
            birdAcceleration += 0.3f;

            //calc velocity using positions
            float velocityY = bird_Ypos - bird_Ypos_prev;

            //limit velocity
            if (velocityY > 2.0f) { velocityY = 2.0f; }

            //store prev pos
            bird_Ypos_prev = bird_Ypos;

            //set next pos
            bird_Ypos += (velocityY + birdAcceleration);

            //clear accelerations
            birdAcceleration = 0;

            //reset game if bird reaches bottom of screen
            if (bird_Ypos > Data.GDM.PreferredBackBufferHeight)
            {
                Reset();
            }

            //place bird rec at proper location
            bird.Y = (int)bird_Ypos;



            //move pipes left
            int s = size;
            for (int i = 0; i < s; i++)
            {
                pipes[i].X--;

                //if pipe goes off screen, place on right
                if (pipes[i].X < -pipes[i].Width)
                {
                    //place off screen right
                    pipes[i].X = Data.GDM.PreferredBackBufferWidth;
                    //randomize height
                    pipes[i].Height = Rand.Next(50, 300);
                }

                //check top pipe collisions with bird
                if (pipes[i].Intersects(bird))
                {
                    Reset();
                }
                
                //check bottom pipe collisions with bird
                int yPos = pipes[i].Y + pipes[i].Height + 300;
                Rectangle botPipe = new Rectangle(pipes[i].X, yPos, pipes[i].Width, 512);
                if (botPipe.Intersects(bird))
                {
                    Reset();
                }
                
            }



        }

        public static void Draw()
        {   
            Data.SB.Begin(SpriteSortMode.Deferred, 
                BlendState.AlphaBlend, SamplerState.PointClamp);

            //draw bird
            Data.SB.Draw(texture,
                        new Vector2(bird.X, bird_Ypos),
                        bird,
                        Color.Yellow,
                        0,
                        Vector2.Zero,
                        1.0f, //scale
                        SpriteEffects.None,
                        0.01f);

            //draw pipes
            int s = size;
            for (int i = 0; i < s; i++)
            {   
                //draw top pipe
                Data.SB.Draw(texture,
                    new Vector2(pipes[i].X, pipes[i].Y),
                    pipes[i],
                    Color.Green,
                    0,
                    Vector2.Zero,
                    1.0f, //scale
                    SpriteEffects.None,
                    0.02f);

                //draw bottom connecting pipe
                int yPos = pipes[i].Y + pipes[i].Height + 300;
                Data.SB.Draw(texture,
                    new Vector2(pipes[i].X, yPos),
                    new Rectangle(pipes[i].X, yPos, pipes[i].Width, 512),
                    Color.Green,
                    0,
                    Vector2.Zero,
                    1.0f, //scale
                    SpriteEffects.None,
                    0.02f);
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
            GraphicsDevice.Clear(Color.CornflowerBlue);
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