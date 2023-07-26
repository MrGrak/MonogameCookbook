using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace Game1
{
    //encode an entity into a double,
    //show that this double is a primitive,
    //then put double into a fixed array,
    //get double from fixed array and convert it to entity
    //check that all fields load and store correctly

    //some enums to make struct mapping easier
    public enum Team : byte { Player, Enemy, World }
    public enum Id : byte { Avatar1, Avatar2, Avatar3, Avatar4 }
    public enum State : byte { Idle, Moving, Attack, Hit, Death }

    //an 8 byte struct we'll encode into a double
    public struct EntityAsDouble
    {
        public Team Team;
        public Id Id;
        public State State;
        public byte SoundFxId;
        public uint Uid;
    }

    //a fixed buffer of doubles
    unsafe struct FixedBufferExample
    {
        public fixed double _buffer[256];
    }

    //a test class
    public static class TestA
    {
        static FixedBufferExample FBE = new FixedBufferExample();

        public unsafe static void Store()
        {
            //create entity to store in buffer
            EntityAsDouble E = new EntityAsDouble();
            E.Team = Team.Player;
            E.Id = Id.Avatar3;
            E.State = State.Idle;
            E.Uid = 42069;

            //write entity fields to output
            Debug.WriteLine("store values");
            Debug.WriteLine("team: " + E.Team);
            Debug.WriteLine("id: " + E.Id);
            Debug.WriteLine("state: " + E.State);
            Debug.WriteLine("uid: " + E.Uid);

            //initialize unmanged memory to hold entity
            IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(E));

            try
            {   //copy entity to unmanaged memory
                Marshal.StructureToPtr(E, pnt, false);

                //marshal entity to double
                double D = (Double)Marshal.PtrToStructure(pnt, typeof(Double));

                //write entity as double into buffer
                fixed (double* buffer = FBE._buffer) { buffer[0] = D; }
            }
            finally
            {
                //free unmanaged memory
                Marshal.FreeHGlobal(pnt);
            }
        }

        public unsafe static void Load()
        {
            fixed (double* buffer = FBE._buffer)
            {
                //load double from buffer
                double D = buffer[0];

                //convert double into byte array
                byte[] bytes = BitConverter.GetBytes(D);

                //create entity from byte array
                EntityAsDouble E = new EntityAsDouble();
                E.Team = (Team)bytes[0];
                E.Id = (Id)bytes[1];
                E.State = (State)bytes[2];
                //convert last 4 bytes to uint
                E.Uid = BitConverter.ToUInt32(bytes, 4);

                //write entity fields to output
                Debug.WriteLine("load values");
                Debug.WriteLine("team: " + E.Team);
                Debug.WriteLine("id: " + E.Id);
                Debug.WriteLine("state: " + E.State);
                Debug.WriteLine("uid: " + E.Uid);
            }
        }
    }

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize() 
        { base.Initialize(); }

        protected override void LoadContent() 
        { 
            Debug.WriteLine("is double primitive: " + typeof(System.Double).IsPrimitive);
            TestA.Store();
            TestA.Load();
            Exit();
        }
        protected override void UnloadContent() { }
        protected override void Update(GameTime gameTime) { }
        protected override void Draw(GameTime gameTime) { }

    }
}