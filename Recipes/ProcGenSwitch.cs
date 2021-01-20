using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//let's procgen some aos code, showing how dod and oop can 
//live together in harmony! -mrgrak jan 2021 mit license

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
            //can we proc gen an aos of structs?
            sb.Append("\n\n\n//let's generate a switch statement\n");




            //proc gen an enum string
            String MobEnumName = "EntityID";
            List<string> _EnumValues = new List<string>();
            _EnumValues.Add("Team_A");
            _EnumValues.Add("Team_B");
            _EnumValues.Add("Team_C");
            _EnumValues.Add("Team_D");
            _EnumValues.Add("Team_E");
            _EnumValues.Add("Team_F");
            _EnumValues.Add("Team_G");
            //write enum
            sb.Append("\npublic enum " + MobEnumName + " \n{ ");
            for (int i = 0; i < _EnumValues.Count; i++)
            { 
                sb.Append("\n\t" + _EnumValues[i] + ", "); 
            }
            sb.Append("\n}");
            sb.Append("\n");



            //proc gen struct, with enum above + other fields
            String structName = "Entity";
            List<string> _StructFields = new List<string>();
            _StructFields.Add("public float X, Y;");
            _StructFields.Add("public int W, H;");
            _StructFields.Add("public " + MobEnumName + " ID;");
            //write struct
            sb.Append("\npublic struct " + structName);
            sb.Append("\n{");
            for (int i = 0; i < _StructFields.Count; i++)
            { sb.Append("\n\t" + _StructFields[i]); }
            sb.Append("\n}");
            sb.Append("\n");



            //proc gen AOS class with struct above
            String className = "EntityManager";
            int sizeOfArray = 1024;
            String arrayName = "Entities";
            List<string> _ClassMethods = new List<string>();
            _ClassMethods.Add("void Reset()");
            //_ClassMethods.Add("void Update()");
            _ClassMethods.Add("void Draw()");
            //write class
            sb.Append("\npublic static class " + className + "");
            sb.Append("\n{ ");

            //create aos array
            sb.Append("\n\tpublic static int size = " + sizeOfArray + ";");
            sb.Append("\n\tpublic static " + structName + "[] " + arrayName + ";");
            sb.Append("\n");

            //write constructor
            sb.Append("\n\tstatic " + className + "() { }");
            sb.Append("\n");

            //write tbi methods
            for (int i = 0; i < _ClassMethods.Count; i++)
            {
                sb.Append("\n\tpublic static " + _ClassMethods[i] + " { }");
                sb.Append("\n");
            }

            //put switch statement into update method
            sb.Append("\n\tpublic static void Update()");
            sb.Append("\n\t{");
            sb.Append("\n\t\tfor(int i = 0; i < size; i++)");
            sb.Append("\n\t\t{");
            sb.Append("\n\t\t\tswitch(" + arrayName + "[i].ID)");
            sb.Append("\n\t\t\t{");

            for (int i = 0; i < _EnumValues.Count; i++)
            {
                sb.Append("\n\t\t\t\tcase " + MobEnumName + "." + _EnumValues[i] + " : ");
                sb.Append("\n\t\t\t\t{");
                sb.Append("\n\t\t\t\t\t//" + arrayName + "[i].ID.value");
                sb.Append("\n\t\t\t\t\tbreak;");
                sb.Append("\n\t\t\t\t}");
            }

            sb.Append("\n\t\t\t}");
            sb.Append("\n\t\t}");
            sb.Append("\n\t}");
            sb.Append("\n");
            sb.Append("\n");
            sb.Append("\n}");
            sb.Append("\n");


            //write to output file
            string dir = Path.Combine(GetExecutingDirectoryName(), "Output.cs");
            Debug.WriteLine("writing to: " + dir);
            using (StreamWriter writer = new StreamWriter(dir))
            { writer.WriteLine(sb.ToString()); }
            //write to debug output too
            Debug.WriteLine(sb.ToString());
            Exit(); //then exit window, lol
        }

        public static string GetExecutingDirectoryName()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            return new FileInfo(location.AbsolutePath).Directory.FullName;
        }

        protected override void Update(GameTime gameTime){ }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}