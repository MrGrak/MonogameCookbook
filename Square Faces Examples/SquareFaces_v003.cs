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
    //update: squarefaces now check collisions with all other squarefaces

    public static class Data
    {
        public static GraphicsDeviceManager GDM;
        public static ContentManager CM;
        public static SpriteBatch SB;
        public static Game1 GAME;
        public static Random Rand;
        public static float Version = 0.03f;
    }



    //create a simple game entity, a face with two eyes
    public class SquareFace
    {
        public Rectangle[] Face;
        public Color Color_Face;
        public Color Color_Eyes;
        public Point Position;
        public Point FaceSize;
        public Point EyeSize;
        public short Timeline;
        public float Layer;

        public SquareFace()
        {
            //create face, then two eyes, one ground shadow
            Face = new Rectangle[4];
            
            FaceSize = new Point(Data.Rand.Next(10, 17), Data.Rand.Next(16, 22));
            EyeSize = new Point(Data.Rand.Next(2, 6), Data.Rand.Next(2, 6));

            //choose width and height
            Rectangle skin = new Rectangle(0, 0, FaceSize.X, FaceSize.Y);
            Face[0] = skin;
            //choose width and height
            Rectangle eye = new Rectangle(0, 0, EyeSize.X, EyeSize.Y);
            Face[1] = eye;
            Face[2] = eye;
            //create ground shadow
            Rectangle shadow = new Rectangle(0, 0, FaceSize.X + 6, 1);
            Face[3] = eye;

            //set spawn position
            Position = new Point(100, 100);
            //choose face and eye color
            Color_Face = new Color(
                Data.Rand.Next(100, 255), 
                Data.Rand.Next(100, 255), 
                Data.Rand.Next(100, 255), 255);
            Color_Eyes = new Color(
                Data.Rand.Next(0, 80),
                Data.Rand.Next(0, 80),
                Data.Rand.Next(0, 80), 255);

            //randomize initial timeline
            Timeline = (short)Data.Rand.Next(0, 30);
            Layer = 0.0f;
        }

        public void Update()
        {
            //animate position
            Timeline++;
            if (Timeline == 10) { Position.Y--; }
            else if (Timeline == 20) { Position.Y++; }
            else if (Timeline >= 30)
            {
                Timeline = 0;
                Position.X += Data.Rand.Next(-2, 3);
            }

            //place skin and eyes at location
            Face[0] = new Rectangle(Position.X + 0, Position.Y + 0, FaceSize.X, FaceSize.Y);
            Face[1] = new Rectangle(Position.X + 2, Position.Y + 4, EyeSize.X, EyeSize.Y);
            Face[2] = new Rectangle(Position.X + 8, Position.Y + 4, EyeSize.X, EyeSize.Y);
            Face[3] = new Rectangle(Position.X - 3, Position.Y + FaceSize.Y, FaceSize.X + 6, 2);

            //sort back to front
            Layer = 1.00000f - ((Position.Y + FaceSize.Y) * 0.00001f);
        }
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
                { SquareFaces.Add(new SquareFace()); }
                Randomize();
            }
        }

        public static void Randomize()
        {   //randomize placement of square faces
            for (int i = 0; i < Size; i++)
            {
                //spread wide across screen
                //SquareFaces[i].Position.X = Data.Rand.Next(10, Data.GDM.PreferredBackBufferWidth - 20);
                //SquareFaces[i].Position.Y = Data.Rand.Next(10, Data.GDM.PreferredBackBufferHeight - 20);

                //concentrate in center
                SquareFaces[i].Position.X = Data.GDM.PreferredBackBufferWidth / 2 + Data.Rand.Next(-5, 6);
                SquareFaces[i].Position.Y = Data.GDM.PreferredBackBufferHeight / 2 + Data.Rand.Next(-5, 6);
            }
        }

        public static void Update()
        {
            for(int i = 0; i < Size; i++)
            {
                SquareFaces[i].Update();
                CollisionCheck(SquareFaces[i], i);
            }
        }

        public static void CollisionCheck(SquareFace SF, int index)
        {
            //check collisions with all other squarefaces - expensive!
            for (int g = 0; g < Size; g++)
            {
                if (index == g) { continue; } //skip self intersection check

                if (SF.Face[0].Intersects(SquareFaces[g].Face[0]))
                {
                    //push left/right
                    if (SF.Position.X < SquareFaces[g].Position.X)
                    { SF.Position.X--; }
                    else { SF.Position.X++; }
                    //push up/down
                    if (SF.Position.Y < SquareFaces[g].Position.Y)
                    { SF.Position.Y--; }
                    else { SF.Position.Y++; }
                }
            }
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