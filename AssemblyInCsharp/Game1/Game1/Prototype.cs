using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using System.Security;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Text;

namespace Game1
{
    public static class Prototype
    {
        //perform work in c# code, time it, draw it
        //perform work in assembly, time it, draw it

        public const int screen_width = 1280;
        public const int screen_height = 720;

        public static Game1 Game;
        public static GraphicsDeviceManager GraphicsDeviceMan;
        public static ContentManager ContentMan;
        public static SpriteBatch SpriteBat;
        public static Texture2D LineTexture;

        public static Stopwatch Timer;





        

        static Prototype() { }

        public static void Constructor()
        {
            GraphicsDeviceMan = new GraphicsDeviceManager(Game);
            GraphicsDeviceMan.GraphicsProfile = GraphicsProfile.HiDef;
            ContentMan = Game.Content;
            Game.Content.RootDirectory = "Content";
            Game.IsMouseVisible = true;
        }

        public static void LoadContent()
        {
            GraphicsDeviceMan.PreferredBackBufferWidth = screen_width;
            GraphicsDeviceMan.PreferredBackBufferHeight = screen_height;
            GraphicsDeviceMan.ApplyChanges();

            SpriteBat = new SpriteBatch(GraphicsDeviceMan.GraphicsDevice);
            

            //setup texture to use for drawing rectangles + color + alpha
            LineTexture = new Texture2D(GraphicsDeviceMan.GraphicsDevice, 1, 1);
            LineTexture.SetData<Color>(new Color[] { Color.White });
            
            Timer = new Stopwatch();



            






            StringBuilder SB = new StringBuilder(2048);

            //read cpu id 0
            SB.Append("manufacturer id\n");
            var buffer = CpuID.Invoke(0);
            var chars = Encoding.ASCII.GetChars(buffer);
            
            {   //display highest function implemented on cpu / intel model
                string hex = Convert.ToByte(chars[0]).ToString("x2");
                if (chars[0] == 0x02) { SB.Append(hex + " - Pentium Pro, Pentium II, Celeron, Pentium 4, Xeon, Pentium M"); }
                else if (chars[0] == 0x03) { SB.Append(hex + " - Pentium III"); }
                else if (chars[0] == 0x05) { SB.Append(hex + " - Pentium 4 with Hyper-Threading, Pentium D (8xx)"); }
                else if (chars[0] == 0x06) { SB.Append(hex + " - Pentium D (9xx)"); }
                else if (chars[0] == 0x06) { SB.Append(hex + " - Pentium D (9xx)"); }
                else if (chars[0] == 0x0A) { SB.Append(hex + " - Core Duo/Core 2 Duo/Xeon 3000, 5100, 5200, 5300, 5400, Atom "); }
                else if (chars[0] == 0x0d) { SB.Append(hex + " - Ivy Bridge/Core 2 Duo 8000 series"); }
                else if (chars[0] == 0x16) { SB.Append(hex + " - Skylake"); }
                else if (chars[0] == 0x17) { SB.Append(hex + " - Ivy Bridge"); }
                SB.Append("\n");
            }
            //append CPU's manufacturer ID string
            string str = new string(chars, 4, 12);
            SB.Append(str + "\n");



            //read cpu id 1 (cpu signature + model info)
            SB.Append("\ncpu signature + model info\n");

            buffer = CpuID.Invoke(1);
            SB.Append("eax 08: " + buffer[0] + "\n");
            SB.Append("eax 16: " + buffer[1] + "\n");
            SB.Append("eax 24: " + buffer[2] + "\n");
            SB.Append("eax 32: " + buffer[3] + "\n");

            SB.Append("ebx 08: " + buffer[4] + "\n");
            SB.Append("ebx 16: " + buffer[5] + "\n");
            SB.Append("ebx 24: " + buffer[6] + "\n");
            SB.Append("ebx 32: " + buffer[7] + "\n");

            SB.Append("edx 08: " + buffer[8] + "\n");
            SB.Append("edx 16: " + buffer[9] + "\n");
            SB.Append("edx 24: " + buffer[10] + "\n");
            SB.Append("edx 32: " + buffer[11] + "\n");

            SB.Append("ecx 08: " + buffer[12] + "\n");
            SB.Append("ecx 16: " + buffer[13] + "\n");
            SB.Append("ecx 24: " + buffer[14] + "\n");
            SB.Append("ecx 32: " + buffer[15] + "\n");
            
            


            //read cpu id 2 (cache info)
            buffer = CpuID.Invoke(2);
            SB.Append("\ncache info\n");
            SB.Append("eax 08: " + buffer[0] + "\n");
            SB.Append("eax 16: " + buffer[1] + "\n");
            SB.Append("eax 24: " + buffer[2] + "\n");
            SB.Append("eax 32: " + buffer[3] + "\n");

            SB.Append("ebx 08: " + buffer[4] + "\n");
            SB.Append("ebx 16: " + buffer[5] + "\n");
            SB.Append("ebx 24: " + buffer[6] + "\n");
            SB.Append("ebx 32: " + buffer[7] + "\n");

            SB.Append("edx 08: " + buffer[8] + "\n");
            SB.Append("edx 16: " + buffer[9] + "\n");
            SB.Append("edx 24: " + buffer[10] + "\n");
            SB.Append("edx 32: " + buffer[11] + "\n");

            SB.Append("ecx 08: " + buffer[12] + "\n");
            SB.Append("ecx 16: " + buffer[13] + "\n");
            SB.Append("ecx 24: " + buffer[14] + "\n");
            SB.Append("ecx 32: " + buffer[15] + "\n");

            //str = new string(chars);
            //SB.Append(str + "\n");




            //debug write contents of sb
            Debug.WriteLine(SB.ToString());






            Game.Exit();
        }

        public static void Update()
        {
            Timer.Restart();
        }

        public static void Draw()
        {


            #region Draw Timer

            Timer.Stop();
            
            GraphicsDeviceMan.GraphicsDevice.SetRenderTarget(null);

            SpriteBat.Begin(SpriteSortMode.BackToFront,
                BlendState.AlphaBlend, SamplerState.PointClamp,
                null, null, null, null);

            //draw background rec
            Draw_Rectangle(0, 0, screen_width, 20,
                Color.Black, 1.0f, 0.02f);

            //draw timing rec
            Draw_Rectangle(10, 10, (int)Timer.ElapsedTicks, 8,
                Color.Green, 1.0f, 0.01f);

            SpriteBat.End();

            #endregion

        }

        //util methods

        public static void Draw_Rectangle(
            int X, int Y, int W, int H,
            Color color, float alpha, float layer)
        {
            Rectangle DrawRec = new Rectangle();
            DrawRec.X = X; DrawRec.Y = Y;
            DrawRec.Width = W; DrawRec.Height = H;
            SpriteBat.Draw(LineTexture, new Vector2(X, Y),
                DrawRec, color * alpha, 0f, Vector2.Zero, 1.0f,
                SpriteEffects.None, layer);
        }
        
    }
}