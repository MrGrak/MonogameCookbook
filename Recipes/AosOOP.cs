using System;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Game1
{

    public abstract class BaseClass
    {
        //encapsulate/hide internal data
        private int[] _AoS = new int[1024];

        //abstract into getter/setter methods, why not
        public int[] AoS
        {
            get { return this._AoS; }
            set { this._AoS = value; }
        }
        
        //add a base method to override later
        public virtual byte BaseMethod()
        {
            return 0;
        }

        //add some polymorphic methods too, why not
        public int Add(int a, int b, int c)
        {
            return a + b + c;
        }
        public int Add(int a, int b)
        {
            return a + b;
        }
    }

    public class ChildClass : BaseClass
    { 
        public override byte BaseMethod()
        {
            return 1;
        }
    }

    public static class TestClass
    {
        public static ChildClass Child;

        public static void Constructor()
        {
            Child = new ChildClass();
            Child.AoS[1] = 0;
            Child.Add(1, 2, 3);
            Child.Add(1, 2);
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
            //testing complete, exit program
            Exit();
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime) { }

        protected override void Draw(GameTime gameTime) { }

    }
}