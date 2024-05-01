using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.Intrinsics;

namespace Project1
{
    public static class Globals
    {
        public static GraphicsDeviceManager GDM;
        public static ContentManager CM;
        public static SpriteBatch SB;
        public static Game1 GAME;
        public static Random RAND = new Random();
        public static Texture2D lineTexture;
        public static KeyboardState currentKeyboardState = new KeyboardState();
        public static KeyboardState lastKeyboardState = new KeyboardState();
    }

    public struct Particle
    {
        public float CurrPos_X, CurrPos_Y;
        public float PrevPos_X, PrevPos_Y;
    }

    public class Game1 : Game
    {
        public const int total = 128 * 100; //needs to be multiple of 128
        public Particle[] particles = new Particle[total];
        public Stopwatch timer = new Stopwatch();
        public long ticks_update = 0;
        public long ticks_draw = 0;
        public bool updateScalar = true;

        public Game1()
        {
            Globals.GAME = this;
            Globals.GDM = new GraphicsDeviceManager(this);
            Globals.GDM.GraphicsProfile = GraphicsProfile.HiDef;
            Globals.CM = Content;
            Content.RootDirectory = "Content";
            IsFixedTimeStep = true;
            Globals.GDM.SynchronizeWithVerticalRetrace = true;
            IsMouseVisible = true;

            Globals.GDM.PreferredBackBufferWidth = 1280;
            Globals.GDM.PreferredBackBufferHeight = 720;
            Globals.GDM.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();
            Globals.lineTexture = new Texture2D(Globals.GDM.GraphicsDevice, 1, 1);
            Globals.lineTexture.SetData<Color>(new Color[] { Color.White });
        }

        protected override void LoadContent()
        {
            Globals.SB = new SpriteBatch(GraphicsDevice);

            for(int i = 0; i < total; i++)
            {
                particles[i].CurrPos_X = Globals.RAND.Next(0, Globals.GDM.PreferredBackBufferWidth);
                particles[i].CurrPos_Y = Globals.RAND.Next(0, Globals.GDM.PreferredBackBufferHeight);
                particles[i].PrevPos_X = particles[i].CurrPos_X;
                particles[i].PrevPos_Y = particles[i].CurrPos_Y;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            timer.Restart();

            //toggle scalar/vector update via space bar
            Globals.lastKeyboardState = Globals.currentKeyboardState;
            Globals.currentKeyboardState = Keyboard.GetState();
            if (Globals.currentKeyboardState.IsKeyDown(Keys.Space) && Globals.lastKeyboardState.IsKeyUp(Keys.Space))
            { if (updateScalar) { updateScalar = false; } else { updateScalar = true; } }


            #region Scalar physics update

            if (updateScalar)
            {
                for (int i = 0; i < total; i++)
                {
                    //calc velocity
                    float velocity = particles[i].CurrPos_Y - particles[i].PrevPos_Y;

                    //apply gravity
                    velocity += 0.98f;

                    //update last to current
                    particles[i].PrevPos_Y = particles[i].CurrPos_Y;

                    //project current position to next position
                    particles[i].CurrPos_Y += velocity;

                    //bounce off floor
                    if (particles[i].CurrPos_Y > Globals.GDM.PreferredBackBufferHeight)
                    {
                        particles[i].CurrPos_Y = Globals.GDM.PreferredBackBufferHeight;
                        particles[i].PrevPos_Y = Globals.GDM.PreferredBackBufferHeight + Globals.RAND.Next(100, 3500) * 0.01f;
                    }
                }
            }

            #endregion

            else
            {
                unsafe
                {
                    if (Vector128.IsHardwareAccelerated)
                    {
                        //Vector128<byte>.Count
                        int simdSize = Vector<float>.Count;
                        int numIterations = particles.Length / simdSize;

                        //create arrays for vector data
                        float[] curr_yPos = new float[simdSize];
                        float[] prev_yPos = new float[simdSize];
                        float[] velocity_y = new float[simdSize];
                        float[] gravity_y = new float[simdSize];

                        for (int i = 0; i < numIterations; i++)
                        {
                            //load particle data into vectors
                            for (int j = 0; j < simdSize; j++)
                            {
                                curr_yPos[j] = particles[i * simdSize + j].CurrPos_Y;
                                prev_yPos[j] = particles[i * simdSize + j].PrevPos_Y;
                                gravity_y[j] = 0.98f; //set gravity
                                velocity_y[j] = 0; //reset velocity
                            }

                            //calc velocities (new way)
                            var v1 = new Vector<float>(curr_yPos);
                            var v2 = new Vector<float>(prev_yPos);
                            (v1 - v2).CopyTo(velocity_y);

                            //apply gravity (new way)
                            var v3 = new Vector<float>(velocity_y);
                            var v4 = new Vector<float>(gravity_y);
                            (v3 + v4).CopyTo(velocity_y);
                            
                            //update last to current (new way)
                            var v5 = new Vector<float>(curr_yPos);
                            (v5).CopyTo(prev_yPos);

                            //project current
                            var v6 = new Vector<float>(velocity_y);
                            (v5 + v6).CopyTo(curr_yPos);
                            
                            //bounce off floor
                            for (int j = 0; j < simdSize; j++)
                            {
                                if (curr_yPos[j] > Globals.GDM.PreferredBackBufferHeight) 
                                {
                                    curr_yPos[j] = Globals.GDM.PreferredBackBufferHeight;
                                    prev_yPos[j] = Globals.GDM.PreferredBackBufferHeight + Globals.RAND.Next(100, 3500) * 0.01f;
                                }
                            }

                            //store updated data back into particles
                            for (int j = 0; j < simdSize; j++)
                            {
                                particles[i * simdSize + j].CurrPos_Y = curr_yPos[j];
                                particles[i * simdSize + j].PrevPos_Y = prev_yPos[j];
                            }
                        }

                        //handle remaining particles if any (we dont cause length is multiple of 128)
                        //for (int i = numIterations * simdSize; i < particles.Length; i++) { }
                    }
                }
            }

            timer.Stop();
            ticks_update = timer.ElapsedTicks;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Globals.SB.Begin(SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null, null, null, null);

            timer.Restart();

            //render particle system
            Rectangle R = new Rectangle(0, 0, 3, 3);
            for (int i = 0; i < total; i++)
            {
                R.X = (int)particles[i].CurrPos_X;
                R.Y = (int)particles[i].CurrPos_Y;
                //faster to manually inline this fn
                Globals.SB.Draw(
                    Globals.lineTexture,
                    new Microsoft.Xna.Framework.Vector2(particles[i].CurrPos_X, particles[i].CurrPos_Y),
                    new Rectangle(0, 0, 3, 3),
                    Color.White, 0.0f, new Microsoft.Xna.Framework.Vector2(0, 0),
                    1.0f, SpriteEffects.None, 0.1f);
            }

            timer.Stop();
            ticks_draw = timer.ElapsedTicks;

            {   //render update time
                Rectangle r = new Rectangle(10, 10, (int)(ticks_update * 0.1f), 10);
                if (updateScalar)
                { Draw(r, Color.Yellow, 1.0f, 0.9f); }
                else { Draw(r, Color.MonoGameOrange, 1.0f, 0.9f); }
            }

            {   //render draw time
                Rectangle r = new Rectangle(10, 20, (int)(ticks_draw * 0.1f), 10);
                Draw(r, Color.Green, 1.0f, 0.9f);
            }

            Globals.SB.End();
        }

        //

        public static void Draw(Rectangle rec, Color color, float alpha, float layer)
        {
            Globals.SB.Draw(
                Globals.lineTexture,
                new Microsoft.Xna.Framework.Vector2(rec.X, rec.Y),
                new Rectangle(0, 0, rec.Width, rec.Height),
                color * alpha,
                0.0f,
                new Microsoft.Xna.Framework.Vector2(0, 0),
                1.0f,
                SpriteEffects.None,
                layer);
        }
    }
}