using System;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Game1
{
    //an example of how to define custom value types using an enum
    public enum MyName : byte { Bob, Ann, Jo, Pat }

    //a class containing values + methods to serialize to/from byte arrays
    public static class Data
    {
        public static bool MyBoolean;
        public static byte MyByte;
        public static short MyShort;
        public static int MyInt;
        public static float MyFloat;
        public static MyName MyName = MyName.Jo;

        public static byte[] Collect()
        {   //write byte array in same order as its read below
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    //examples of writing different values
                    writer.Write((bool)MyBoolean);
                    writer.Write((byte)MyByte);
                    writer.Write((short)MyShort);
                    writer.Write((int)MyInt);
                    writer.Write((Single)MyFloat);
                    //cast enum to proper value type
                    writer.Write((byte)MyName);
                    return stream.ToArray();
                }
            }
        }

        public static void Load(byte[] data)
        {   //read byte array in same order as its written above
            using (MemoryStream stream = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    MyBoolean = reader.ReadBoolean();
                    MyByte = reader.ReadByte();
                    MyShort = reader.ReadInt16();
                    MyInt = reader.ReadInt32();
                    MyFloat = reader.ReadSingle();
                    //you'll need to cast enums when reading too
                    MyName = (MyName)reader.ReadByte();
                }
            }
        }

    }

    public class Game1 : Game
    {
        public static Stopwatch timer = new Stopwatch();
        
        public Game1()
        {
            GraphicsDeviceManager GDM = new GraphicsDeviceManager(this);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            //lets test writing and reading a binary file
            timer.Restart();
            //get path to save file
            string savePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\";
            //add the file name and extension you want
            savePath = Path.Combine(savePath, "test.sav");
            Console.WriteLine("saving to: " + savePath);
            //collect player data into a byte array
            byte[] data = Data.Collect();
            //write byte array to file
            using (var stream = File.Open(savePath, FileMode.Create, FileAccess.Write))
            { stream.Write(data, 0, data.Length); }
            //read byte array from file
            data = File.ReadAllBytes(savePath);
            //load byte values back into data
            Data.Load(data);
            //how long did saving and loading take?
            timer.Stop();
            Console.WriteLine("elapsed ms: " + timer.ElapsedMilliseconds);
            //testing complete, exit program
            Exit();
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime) { }

        protected override void Draw(GameTime gameTime) { }

    }
}