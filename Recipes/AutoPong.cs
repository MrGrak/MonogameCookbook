using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Game1
{
    public static class AutoPong
    {
        public static Rectangle PaddleLeft;
        public static Rectangle PaddleRight;

        public static Rectangle Ball;
        public static Vector2 BallVelocity;
        public static Vector2 BallPosition;

        public static Texture2D Texture;
        public static Rectangle DrawRec = new Rectangle(0, 0, 3, 3);
        public static byte HitCounter = 0;
        static Random Rand = new Random();
        public static float BallSpeed = 15.0f;

        public static byte PointsLeft;
        public static byte PointsRight;
        public static List<byte> GamesWon;
        public static byte TotalGamesToPlay = 65;
        public static byte TotalGamesPlayed = 0;
        public static byte PointsPerGame = 3;

        public static Color LeftColor = new Color(255, 0, 234);
        public static Color RightColor = new Color(0, 255, 164);

        public static void Reset()
        {
            if (Texture == null)
            {   
                Texture = new Texture2D(Data.GDM.GraphicsDevice, 1, 1);
                Texture.SetData<Color>(new Color[] { Color.White });
                GamesWon = new List<byte>(TotalGamesToPlay);
                for (int i = 0; i < TotalGamesToPlay; i++)
                { GamesWon.Add(0); }
            }

            int PaddleHeight = 100;
            PaddleLeft = new Rectangle(100, 150, 30, PaddleHeight);
            PaddleRight = new Rectangle(700, 150, 30, PaddleHeight);

            BallPosition = new Vector2(200, 200);
            Ball = new Rectangle((int)BallPosition.X, (int)BallPosition.Y, 10, 10);
            BallVelocity = new Vector2(1, 0);

            PointsLeft = 0; PointsRight = 0;
        }

        public static void Update()
        {
            if (TotalGamesPlayed >= TotalGamesToPlay) { return; }


            #region Update Ball

            //limit how fast ball can move
            if (BallVelocity.X > 1.0f) { BallVelocity.X = 1.0f; }
            else if (BallVelocity.X < -1.0f) { BallVelocity.X = -1.0f; }
            if (BallVelocity.Y > 1.0f) { BallVelocity.Y = 1.0f; }
            else if (BallVelocity.Y < -1.0f) { BallVelocity.Y = -1.0f; }

            //apply velocity to position
            //Debug.WriteLine("velocity: " + BallVelocity.X + "," + BallVelocity.Y);
            BallPosition.X += BallVelocity.X * BallSpeed;
            BallPosition.Y += BallVelocity.Y * BallSpeed;

            //check for collision with paddles
            HitCounter++;
            if (HitCounter > 10)
            {
                if (PaddleLeft.Intersects(Ball))
                {
                    BallVelocity.X *= -1;
                    BallVelocity.Y += Rand.Next(-100, 101) * 0.001f;
                    HitCounter = 0;
                }
                if (PaddleRight.Intersects(Ball))
                {
                    BallVelocity.X *= -1;
                    BallVelocity.Y += Rand.Next(-100, 101) * 0.001f;
                    HitCounter = 0;
                }
            }

            //bounce on screen
            if (BallPosition.X < 0) //point for right
            {
                BallPosition.X = 1;
                BallVelocity.X *= -1;
                PointsRight++;
            }
            else if (BallPosition.X > 800) //point for left
            {
                BallPosition.X = 799;
                BallVelocity.X *= -1;
                PointsLeft++;
            }

            if (BallPosition.Y < 0)
            {
                BallPosition.Y = 1;
                BallVelocity.Y *= -(1 + Rand.Next(-100, 101) * 0.005f);
            }
            else if (BallPosition.Y > 400)
            {
                BallPosition.Y = 399;
                BallVelocity.Y *= -(1 + Rand.Next(-100, 101) * 0.005f);
            }

            #endregion

            UpdatePaddle(ref PaddleLeft, Rand.Next(0, 5));
            UpdatePaddle(ref PaddleRight, Rand.Next(0, 5));

            #region Check for win condition, reset

            if (PointsLeft >= PointsPerGame)
            {   //track as win for left
                GamesWon[TotalGamesPlayed] = 1; 
                TotalGamesPlayed++;
                Reset();
            }
            else if (PointsRight >= PointsPerGame)
            {   //track as win for right
                GamesWon[TotalGamesPlayed] = 2;
                TotalGamesPlayed++;
                Reset();
            }

            #endregion

        }

        public static void Draw()
        {
            Data.SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            DrawRectangle(PaddleLeft, LeftColor);
            DrawRectangle(PaddleRight, RightColor);

            Ball.X = (int)BallPosition.X;
            Ball.Y = (int)BallPosition.Y;
            if (BallVelocity.X > 0)
            { DrawRectangle(Ball, LeftColor); }
            else { DrawRectangle(Ball, RightColor); }

            //draw current game points
            for(int i = 0; i < PointsLeft; i++)
            { DrawRectangle(new Rectangle(0 + 10 + i * 12, 10, 10, 10), LeftColor * 1.0f); }
            for (int i = 0; i < PointsRight; i++)
            { DrawRectangle(new Rectangle(800 - 20 - i * 12, 10, 10, 10), RightColor * 1.0f); }

            //draw total games won
            for(int i = 0; i < TotalGamesToPlay; i++)
            {if (GamesWon[i] == 1) //left won game
                { DrawRectangle(new Rectangle(0 + 10 + i * 12, 400 - 20, 10, 10), LeftColor * 1.0f); }
                else if (GamesWon[i] == 2) //right won game
                { DrawRectangle(new Rectangle(0 + 10 + i * 12, 400 - 20, 10, 10), RightColor * 1.0f); }
                else //unplayed game
                { DrawRectangle(new Rectangle(0 + 10 + i * 12, 400 - 20, 10, 10), Color.White * 0.1f); }
            }

            Data.SB.End();
        }

        public static void DrawRectangle(Rectangle Rec, Color color)
        {
            Vector2 pos = new Vector2(Rec.X, Rec.Y);
            Data.SB.Draw(Texture, pos, Rec,
                color * 1.0f,
                0, Vector2.Zero, 1.0f,
                SpriteEffects.None, 0.00001f);
        }
        
        public static void UpdatePaddle(ref Rectangle Paddle, int amount)
        {
            int Paddle_Center = Paddle.Y + Paddle.Height / 2;
            if (Paddle_Center < BallPosition.Y - 20) { Paddle.Y += amount; }
            else if (Paddle_Center > BallPosition.Y + 20) { Paddle.Y -= amount; }
        }

    }

    public static class Data
    {
        public static GraphicsDeviceManager GDM;
        public static ContentManager CM;
        public static SpriteBatch SB;
        public static Game1 GAME;
    }

    public class Game1 : Game
    {
        public Game1()
        {
            Data.GDM = new GraphicsDeviceManager(this);
            Data.GDM.GraphicsProfile = GraphicsProfile.HiDef;
            Data.CM = Content;
            Data.GAME = this;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Data.GDM.PreferredBackBufferWidth = 800;
            Data.GDM.PreferredBackBufferHeight = 400;
        }

        protected override void Initialize() { base.Initialize(); }

        protected override void LoadContent()
        {
            Data.SB = new SpriteBatch(GraphicsDevice);
            AutoPong.Reset();
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            AutoPong.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(40, 40, 40));
            AutoPong.Draw();
            base.Draw(gameTime);
        }
    }

}