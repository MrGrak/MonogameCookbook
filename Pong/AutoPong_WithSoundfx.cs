using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Game1
{
    public enum WaveType { Sin, Tan, Log, Square, Noise }

    public enum Note
    {
        NaN, //not a note lol
        C1, D1, E1, F1, G1, A1, B1,
        C2, D2, E2, F2, G2, A2, B2,
        C3, D3, E3, F3, G3, A3, B3,
        C4, D4, E4, F4, G4, A4, B4,
        C5, D5, E5, F5, G5, A5, B5,
        C6, D6, E6, F6, G6, A6, B6,
    }

    public static class Notes
    {
        public static float GetNote(Note ID)
        {   //convert note to frequency
            switch (ID)
            {
                case Note.C1: return 32.70f;
                case Note.D1: return 36.71f;
                case Note.E1: return 41.20f;
                case Note.F1: return 43.65f;
                case Note.G1: return 49.00f;
                case Note.A1: return 55.00f;
                case Note.B1: return 61.74f;

                case Note.C2: return 65.41f;
                case Note.D2: return 73.42f;
                case Note.E2: return 82.41f;
                case Note.F2: return 87.31f;
                case Note.G2: return 98.00f;
                case Note.A2: return 110.00f;
                case Note.B2: return 123.47f;

                case Note.C3: return 130.81f;
                case Note.D3: return 146.83f;
                case Note.E3: return 164.81f;
                case Note.F3: return 174.61f;
                case Note.G3: return 196.00f;
                case Note.A3: return 220.00f;
                case Note.B3: return 246.94f;

                case Note.C4: return 261.63f;
                case Note.D4: return 293.66f;
                case Note.E4: return 329.63f;
                case Note.F4: return 349.23f;
                case Note.G4: return 392.00f;
                case Note.A4: return 440.00f;
                case Note.B4: return 493.88f;

                case Note.C5: return 523.25f;
                case Note.D5: return 587.33f;
                case Note.E5: return 659.25f;
                case Note.F5: return 698.46f;
                case Note.G5: return 783.99f;
                case Note.A5: return 880.00f;
                case Note.B5: return 987.77f;

                case Note.C6: return 1046.50f;
                case Note.D6: return 1174.66f;
                case Note.E6: return 1318.51f;
                case Note.F6: return 1396.91f;
                case Note.G6: return 1567.98f;
                case Note.A6: return 1760.00f;
                case Note.B6: return 1975.53f;
            }

            return 0.0f; //note unsupported/NaN
        }
    }

    public static class Phrases
    {
        public static Note[] BassPhrase = new Note[] 
        {   //root, 4, root, 5, 4 progression
            Note.C4, Note.C4, Note.C4, Note.C4,
            Note.F4, Note.F4, Note.F4, Note.F4,
            Note.C4, Note.C4, Note.C4, Note.C4,
            Note.G4, Note.G4, Note.G4, Note.G4,
            Note.F4, Note.F4, Note.F4, Note.F4,
            //root, 2, 3, 4 progression
            Note.C4, Note.C4, Note.C4, Note.C4,
            Note.D4, Note.D4, Note.D4, Note.D4,
            Note.E4, Note.E4, Note.E4, Note.E4,
            Note.F4, Note.F4, Note.F4, Note.F4,
            //root, 6, 4, 5 progression
            Note.C4, Note.C4, Note.C4, Note.C4,
            Note.A3, Note.A3, Note.A3, Note.A3,
            Note.F4, Note.F4, Note.F4, Note.F4,
            Note.G4, Note.G4, Note.G4, Note.G4,
        };
        public static Note[] GuitarPhrase = new Note[]
        {   //just some random phrases in C major
            Note.C5, Note.D5, Note.E5, Note.F5,
            Note.NaN, Note.NaN, Note.NaN, Note.NaN,
            Note.G5, Note.A5, Note.B5, Note.C6,
            Note.NaN, Note.NaN, Note.NaN, Note.NaN,
            Note.D6, Note.E6, Note.F6, Note.G6,
            Note.NaN, Note.NaN, Note.NaN, Note.NaN,
            Note.C6, Note.B5, Note.A5, Note.G5,
            Note.NaN, Note.NaN, Note.NaN, Note.NaN,
            Note.F5, Note.E5, Note.D5, Note.C5,
            Note.NaN, Note.NaN, Note.NaN, Note.NaN,
            //2nd phrase with some variation
            Note.C5, Note.D5, Note.E5, Note.F5,
            Note.NaN, Note.NaN, Note.NaN, Note.NaN,
            Note.C6, Note.D6, Note.C6, Note.D6,
            Note.C6, Note.C6, Note.NaN, Note.NaN,
        };
    }

    public class AudioSource
    {
        public int SampleRate = 48000;
        public DynamicSoundEffectInstance DSEI;
        public byte[] Buffer;
        public int BufferSize;
        public int TotalTime = 0;

        public AudioSource()
        {
            DSEI = new DynamicSoundEffectInstance(SampleRate, AudioChannels.Mono);
            BufferSize = DSEI.GetSampleSizeInBytes(TimeSpan.FromMilliseconds(500));
            Buffer = new byte[BufferSize];
            DSEI.Volume = 0.4f;
            DSEI.IsLooped = false;
        }
    }

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

        public static AudioSource SoundFX;

        public static AudioSource Music_Bass;
        public static short BassCounter = 0;
        public static short BassNoteLength = 20;
        public static short BassNoteID = 0;

        public static AudioSource Music_Guitar;
        public static short GuitarCounter = 0;
        public static short GuitarNoteLength = 20;
        public static short GuitarNoteID = 0;

        public static AudioSource Music_Drums;
        public static short DrumCounter = 0;


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

            //setup sound sources
            if (SoundFX == null) { SoundFX = new AudioSource(); }
            if (Music_Bass == null) { Music_Bass = new AudioSource(); }
            if (Music_Guitar == null) { Music_Guitar = new AudioSource(); }
            if (Music_Drums == null) { Music_Drums = new AudioSource(); }
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
                    //PlayWave(Notes.GetNote(Note.C5), 50, WaveType.Log, SoundFX, 0.2f);
                }
                if (PaddleRight.Intersects(Ball))
                {
                    BallVelocity.X *= -1;
                    BallVelocity.Y += Rand.Next(-100, 101) * 0.001f;
                    HitCounter = 0;
                    //PlayWave(Notes.GetNote(Note.C6), 50, WaveType.Log, SoundFX, 0.2f);
                }
            }

            //bounce on screen
            if (BallPosition.X < 0) //point for right
            {
                BallPosition.X = 1;
                BallVelocity.X *= -1;
                PointsRight++;
                PlayWave(1000, 50, WaveType.Sin, SoundFX, 1.0f);
            }
            else if (BallPosition.X > 800) //point for left
            {
                BallPosition.X = 799;
                BallVelocity.X *= -1;
                PointsLeft++;
                PlayWave(1000, 50, WaveType.Sin, SoundFX, 1.0f);
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


            //create music tracks

            #region Generate Bass Instrument
            
            BassCounter++;
            if(BassCounter >= BassNoteLength)
            {
                PlayWave(Notes.GetNote(Phrases.BassPhrase[BassNoteID]),
                    3000, WaveType.Sin, Music_Bass, 0.2f);

                BassCounter = 0;
                //keep bass steady
                BassNoteLength = 40;

                //goto next note, handle overflow
                BassNoteID++;
                if (BassNoteID > Phrases.BassPhrase.Length - 1)
                { BassNoteID = 0; }
            }

            //add some vibrato to bass
            //Music_Bass.DSEI.Pitch = 0.0f + Rand.Next(-2, 3) * 0.01f;
            
            #endregion

            #region Generate Guitar Instrument
            
            GuitarCounter++;
            if (GuitarCounter >= GuitarNoteLength)
            {
                PlayWave(Notes.GetNote(Phrases.GuitarPhrase[GuitarNoteID]),
                    2000, WaveType.Sin, Music_Guitar, 0.1f);

                GuitarCounter = 0;
                //randomize length of next note
                //GuitarNoteLength = (short)Rand.Next(10, 30);
                GuitarNoteLength = 20;
                if (Rand.Next(0, 101) > 50)
                { GuitarNoteLength = 10; }

                //goto next note, handle overflow
                GuitarNoteID++;
                if (GuitarNoteID > Phrases.GuitarPhrase.Length - 1)
                { GuitarNoteID = 0; }
            }

            #endregion

            #region Generate Drum Rhythm

            DrumCounter++;
            if(DrumCounter == 40)
            {
                PlayWave(200, 50, WaveType.Noise, Music_Drums, 0.3f);
            }
            else if (DrumCounter == 50)
            {   //apply a little syncopation
                PlayWave(200, 50, WaveType.Noise, Music_Drums, 0.05f);
            }
            else if (DrumCounter == 80)
            {
                PlayWave(200, 50, WaveType.Noise, Music_Drums, 0.3f);
                DrumCounter = 0;
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

        public static void PlayWave(
            double freq, short durMS, 
            WaveType Wt, AudioSource Src,
            float Volume)
        {
            Src.DSEI.Stop();

            Src.BufferSize = Src.DSEI.GetSampleSizeInBytes(TimeSpan.FromMilliseconds(durMS));
            Src.Buffer = new byte[Src.BufferSize];

            int size = Src.BufferSize - 1;
            for (int i = 0; i < size; i += 2)
            {
                double time = (double)Src.TotalTime / (double)Src.SampleRate;

                short currentSample = 0;
                switch (Wt)
                {
                    case WaveType.Sin:
                        {
                            currentSample = (short)(Math.Sin(2 * Math.PI * freq * time) * (double)short.MaxValue * Volume);
                            break;
                        }
                    case WaveType.Tan:
                        {
                            currentSample = (short)(Math.Tan(2 * Math.PI * freq * time) * (double)short.MaxValue * Volume);
                            break;
                        }
                    case WaveType.Log: //bit crushed kinda
                        {
                            currentSample = (short)(Math.Log(Math.Sin(2 * Math.PI * freq * time)) * (double)short.MaxValue * Volume);
                            break;
                        }
                    case WaveType.Square:
                        {
                            currentSample = (short)(Math.Sign(Math.Sin(2 * Math.PI * freq * time)) * (double)short.MaxValue * Volume);
                            break;
                        }
                    case WaveType.Noise:
                        {
                            currentSample = (short)(Rand.Next(-short.MaxValue, short.MaxValue) * Volume);
                            break;
                        }
                }

                Src.Buffer[i] = (byte)(currentSample & 0xFF);
                Src.Buffer[i + 1] = (byte)(currentSample >> 8);
                Src.TotalTime += 2;
            }

            Src.DSEI.SubmitBuffer(Src.Buffer);
            Src.DSEI.Play();
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