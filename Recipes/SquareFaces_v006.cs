using System;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    //squarefaces
    //a simple monogame/fna/xna playground
    //update: squarefaces now have velocity + screen bounds check

    //todo: apply gravity to squarefaces based on mouse pos


    public static class Data
    {
        public static GraphicsDeviceManager GDM;
        public static ContentManager CM;
        public static SpriteBatch SB;
        public static Game1 GAME;
        public static Random Rand;
        public static float Version = 0.06f;
    }



    //create a simple game entity, a face with two eyes
    public struct SquareFace
    {
        public Rectangle[] Face;
        public Color Color_Face;
        public Color Color_Eyes;
        public Color Color_Hat;
        public Vector2 Position;
        public Vector2 Velocity;
        public Point FaceSize;
        public Point EyeSize;
        public short Timeline;
        public float Layer;
        public byte BlinkCounter;
    }

    //manage a large size of these squarefaces 
    public static class GameManager
    {
        public static Texture2D Texture;
        public static int Size = 128;
        public static List<SquareFace> SquareFaces;
        
        public static void Constructor()
        {
            if (Texture == null)
            {
                Texture = new Texture2D(Data.GDM.GraphicsDevice, 1, 1);
                Texture.SetData<Color>(new Color[] { Color.White });
                //initialize list to size we need
                SquareFaces = new List<SquareFace>();
                for (int i = 0; i < Size; i++)
                {
                    SquareFaces.Add(new SquareFace());
                    SquareFaces[i] = GetRandomSF();
                }
                Randomize();
            }
        }

        public static SquareFace GetRandomSF()
        {
            SquareFace SF = new SquareFace();

            //create face, then two eyes, one ground shadow, one hat
            SF.Face = new Rectangle[5];

            SF.FaceSize = new Point(Data.Rand.Next(10, 17), Data.Rand.Next(16, 22));
            SF.EyeSize = new Point(Data.Rand.Next(2, 6), Data.Rand.Next(2, 6));

            //choose width and height
            Rectangle skin = new Rectangle(0, 0, SF.FaceSize.X, SF.FaceSize.Y);
            SF.Face[0] = skin;
            //choose width and height
            Rectangle eye = new Rectangle(0, 0, SF.EyeSize.X, SF.EyeSize.Y);
            SF.Face[1] = eye;
            SF.Face[2] = eye;
            //create ground shadow
            Rectangle shadow = new Rectangle(0, 0, SF.FaceSize.X + 6, 1);
            SF.Face[3] = shadow;
            //create hat
            Rectangle hat = new Rectangle(0, 0, SF.FaceSize.X + 6, 1);
            SF.Face[4] = hat;


            //set spawn position
            SF.Position = new Vector2(100, 100);
            //choose face and eye color
            SF.Color_Face = new Color(
                Data.Rand.Next(0, 80),
                Data.Rand.Next(60, 150),
                Data.Rand.Next(60, 150), 255);

            SF.Color_Eyes = Color.White;

            //randomize color of hat
            int choice = Data.Rand.Next(0, 2);
            if (choice == 0) { SF.Color_Hat = Color.Red; }
            else if (choice == 1) { SF.Color_Hat = Color.Yellow; }



            //randomize initial timeline
            SF.Timeline = (short)Data.Rand.Next(0, 30);
            SF.Layer = 0.0f;

            //randomize blink animation
            SF.BlinkCounter = (byte)Data.Rand.Next(0, 255);

            return SF;
        }

        public static void Randomize()
        {   //randomize placement of square faces
            for (int i = 0; i < Size; i++)
            {
                //spread wide across screen
                //SquareFaces[i].Position.X = Data.Rand.Next(10, Data.GDM.PreferredBackBufferWidth - 20);
                //SquareFaces[i].Position.Y = Data.Rand.Next(10, Data.GDM.PreferredBackBufferHeight - 20);

                //concentrate in center
                SquareFace SF = SquareFaces[i];
                SF.Position.X = Data.GDM.PreferredBackBufferWidth / 2 + Data.Rand.Next(-5, 6);
                SF.Position.Y = Data.GDM.PreferredBackBufferHeight / 2 + Data.Rand.Next(-5, 6);
                SquareFaces[i] = SF;
            }
        }

        public static void Update()
        {
            for(int i = 0; i < Size; i++)
            {
                SquareFace SF = SquareFaces[i];
                Animate(ref SF);
                CollisionCheck(ref SF, i);
                SquareFaces[i] = SF;
            }
        }

        public static void Animate(ref SquareFace SF)
        {
            //apply physics
            SF.Velocity.X *= 0.90f;
            SF.Velocity.Y *= 0.90f;

            //apply velocity to position
            SF.Position.X += SF.Velocity.X;
            SF.Position.Y += SF.Velocity.Y;

            //animate position
            SF.Timeline++;
            if (SF.Timeline == 10) { SF.Position.Y--; }
            else if (SF.Timeline == 20) { SF.Position.Y++; }
            else if (SF.Timeline >= 30)
            {
                SF.Timeline = 0;
                SF.Velocity.X += Data.Rand.Next(-2, 3);
            }

            //place skin and eyes at location
            SF.Face[0] = new Rectangle((int)SF.Position.X + 0, (int)SF.Position.Y + 0, SF.FaceSize.X, SF.FaceSize.Y);
            SF.Face[1] = new Rectangle((int)SF.Position.X + 2, (int)SF.Position.Y + 4, SF.EyeSize.X, SF.EyeSize.Y);
            SF.Face[2] = new Rectangle((int)SF.Position.X + 8, (int)SF.Position.Y + 4, SF.EyeSize.X, SF.EyeSize.Y);
            SF.Face[3] = new Rectangle((int)SF.Position.X - 3, (int)SF.Position.Y + SF.FaceSize.Y, SF.FaceSize.X + 6, 2);
            SF.Face[4] = new Rectangle((int)SF.Position.X + 2, (int)SF.Position.Y - 2, 8, 3); //hat

            //sort back to front
            SF.Layer = 1.00000f - ((SF.Position.Y + SF.FaceSize.Y) * 0.00001f);

            //blink on timer
            SF.BlinkCounter++;
            if(SF.BlinkCounter >= 230)
            {
                SF.Face[1] = new Rectangle((int)SF.Position.X + 2, (int)SF.Position.Y + 4, SF.EyeSize.X, 1);
                SF.Face[2] = new Rectangle((int)SF.Position.X + 8, (int)SF.Position.Y + 4, SF.EyeSize.X, 1);
                SF.Color_Eyes = Color.Black;
            }
            else
            {
                SF.Color_Eyes = Color.White;
            }
        }

        public static void CollisionCheck(ref SquareFace SF, int index)
        {
            float push = 0.1f;
            //check collisions with all other squarefaces - expensive!
            for (int g = 0; g < Size; g++)
            {
                if (index == g) { continue; } //skip self intersection check

                if (SF.Face[0].Intersects(SquareFaces[g].Face[0]))
                {
                    //push left/right
                    if (SF.Position.X < SquareFaces[g].Position.X)
                    { SF.Velocity.X -= push; }
                    else { SF.Velocity.X += push; }
                    //push up/down
                    if (SF.Position.Y < SquareFaces[g].Position.Y)
                    { SF.Velocity.Y -= push; }
                    else { SF.Velocity.Y += push; }
                }
            }

            //check against screen bounds
            if (SF.Position.X + SF.Face[0].Width >= Data.GDM.PreferredBackBufferWidth) { SF.Velocity.X = -1f; }
            else if(SF.Position.X <= 0) { SF.Velocity.X = 1f; }
            if (SF.Position.Y + SF.Face[0].Height >= Data.GDM.PreferredBackBufferHeight) { SF.Velocity.Y = -1f; }
            else if (SF.Position.Y <= 0) { SF.Velocity.Y = 1f; }
        }

        public static void Draw()
        {
            Data.SB.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp);

            for (int i = 0; i < Size; i++)
            {
                DrawRectangle(SquareFaces[i].Face[0], SquareFaces[i].Color_Face, SquareFaces[i].Layer);
                DrawRectangle(SquareFaces[i].Face[1], SquareFaces[i].Color_Eyes, SquareFaces[i].Layer - 0.0000001f);
                DrawRectangle(SquareFaces[i].Face[2], SquareFaces[i].Color_Eyes, SquareFaces[i].Layer - 0.0000001f);
                DrawRectangle(SquareFaces[i].Face[3], Color.Black, SquareFaces[i].Layer);
                DrawRectangle(SquareFaces[i].Face[4], SquareFaces[i].Color_Hat, SquareFaces[i].Layer - 0.0000001f);
            }
            
            Data.SB.End();
        }
        
        public static void DrawRectangle(Rectangle Rec, Color color, float Layer)
        {
            Vector2 pos = new Vector2(Rec.X, Rec.Y);
            Data.SB.Draw(Texture, pos, Rec,
                color * 1.0f,
                0.0f, 
                Vector2.Zero, 1.0f,
                SpriteEffects.None, Layer);
        }
    }

    



    public class Game1 : Game
    {
        public static Stopwatch timer = new Stopwatch();

        public KeyboardState currentKeyboardState = new KeyboardState();
        public KeyboardState lastKeyboardState = new KeyboardState();




        public Game1()
        {
            Data.GDM = new GraphicsDeviceManager(this);
            Data.GDM.GraphicsProfile = GraphicsProfile.HiDef;
            Data.CM = Content;
            Data.GAME = this;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Data.GDM.PreferredBackBufferWidth = 400;
            Data.GDM.PreferredBackBufferHeight = 300;
            Data.Rand = new Random(1024);
        }

        protected override void Initialize() { base.Initialize(); }

        protected override void LoadContent()
        {
            Data.SB = new SpriteBatch(GraphicsDevice);
            GameManager.Constructor();
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            timer.Restart();
            GameManager.Update();
            base.Update(gameTime);

            //reset game upon spacebar press

            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            if(IsNewKeyPress(Keys.Space))
            { GameManager.Randomize(); }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(40, 40, 40));
            GameManager.Draw();
            base.Draw(gameTime);

            timer.Stop();
            Data.GAME.Window.Title =
                "Squarefaces " + Data.Version +
                "- ms: " + timer.ElapsedMilliseconds +
                " - total: " + GameManager.Size;
        }

        //check for keyboard key press, hold, and release
        public bool IsNewKeyPress(Keys key)
        {
            return (currentKeyboardState.IsKeyDown(key) &&
                lastKeyboardState.IsKeyUp(key));
        }
    }

}