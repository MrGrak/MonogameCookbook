using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }








        public void TraceValue(byte value)
        {
            Console.WriteLine("val: \t" + value.ToString("000")
                + " " + Convert.ToString(value, 2).PadLeft(8, '0'));
        }

        public struct Player 
        { 
            public byte BitField;
            //0 isAlive
            //1-2 team

            public byte IsAlive
            {
                get { return (byte)(BitField & 0b00000001); }
                set { BitField = Set0(BitField, value); }
            }

            public byte Team
            {
                get { return (byte)((BitField & 0b00000110) >> 1); }
                set { BitField = Set1To2(BitField, value); }
            }

            public byte Set0(byte src, byte val)
            { return (byte)((src & 0b11111110) | (val & 0b00000001)); }

            public byte Set1To2(byte src, byte val)
            { return (byte)((src & 0b11111001) | ((val & 0b00000011) << 1)); }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Player p = new Player();
            TraceValue(p.BitField);

            p.IsAlive = 1; //range 0-1
            TraceValue(p.IsAlive);
            p.Team = 3; //range 0-3
            TraceValue(p.Team);
            TraceValue(p.BitField);

            Console.WriteLine("clear");
            p.Team = 0;
            p.IsAlive = 0;
            TraceValue(p.BitField);

            Exit();
        }

        














        protected override void Update(GameTime gameTime)
        {
            
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    
        //
        
    }
}