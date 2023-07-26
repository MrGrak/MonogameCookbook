using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            StringBuilder sb = new StringBuilder();
            sb.Append("\n\n\n//put this into a .cs file\n");



            //design a base class
            string BaseClass = "Screen";
            List<string> BaseMethods = new List<string>();
            BaseMethods.Add("Load()");
            BaseMethods.Add("Open()");
            BaseMethods.Add("Close()");
            BaseMethods.Add("Input()");
            BaseMethods.Add("Update(GameTime gameTime)");
            BaseMethods.Add("Draw(GameTime gameTime)");
            //write base class def
            sb.Append("public class " + BaseClass + "\n");
            sb.Append("{\n");
            sb.Append("\t" + "public " + BaseClass + "()\n");
            sb.Append("\t{\n");
            sb.Append("\t\t //tbi\n");
            sb.Append("\t{\n");
            //write base methods
            for (int i = 0; i < BaseMethods.Count; i++)
            {
                sb.Append("\t" + "public void " + BaseMethods[i] + "\n");
                sb.Append("\t{\n");
                sb.Append("\t\t //tbi\n");
                sb.Append("\t{\n");
            }
            sb.Append("}\n");

            



            //design classes that inherit from base class
            List<string> Classes = new List<string>();
            Classes.Add("Screen_Dialog");
            Classes.Add("Screen_Level");

            //methods specific to inherited classes
            List<string> Methods = new List<string>();
            Methods.Add("public void Spawn(int X, int Y, int ID)");
            Methods.Add("public void Animate(ref SpriteStruct, ref Animation)");
            Methods.Add("public void Push(ref SpriteStruct, Vector2 Acceleration)");
            Methods.Add("public void SimPhysics(ref PhysicsStruct, Vector2 Gravity)");

            //create classes with desired methods + signatures
            for (int i = 0; i < Classes.Count; i++)
            {
                //gimme some space between classes
                sb.Append("\n\n");
                sb.Append("public class " + Classes[i] + " : " + BaseClass + "\n");
                sb.Append("{\n");
                //write constructor
                sb.Append("\t" + "public " + Classes[i] + "()\n");
                sb.Append("\t{\n");
                sb.Append("\t\t//tbi\n");
                sb.Append("\t{\n");
                //write inherited methods
                for (int g = 0; g < BaseMethods.Count; g++)
                {
                    sb.Append("\tpublic override void " + BaseMethods[g] + "\n");
                    sb.Append("\t{\n");
                    sb.Append("\t\t//tbi\n");
                    sb.Append("\t{\n");
                    sb.Append("\n");
                }
                //write custom methods 
                for (int g = 0; g < Methods.Count; g++)
                {
                    sb.Append("\t" + Methods[g] + "\n");
                    sb.Append("\t{\n");
                    sb.Append("\t\t//tbi\n");
                    sb.Append("\t{\n");
                    sb.Append("\n");
                }
                //close class scope
                sb.Append("}\n");
            }
            
            //just write string builder to output window for checking
            Debug.WriteLine(sb.ToString());
            Exit(); //then exit window, lol
        }

        protected override void Update(GameTime gameTime) { }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}