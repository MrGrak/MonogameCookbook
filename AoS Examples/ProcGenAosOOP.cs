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
            sb.Append("\n\n\n//let's generate some boilerplate aos\n");




            //proc gen an enum string
            String MobEnumName = "EntityID";
            List<string> _EnumValues = new List<string>();
            _EnumValues.Add("RedTeam");
            _EnumValues.Add("BlueTeam");
            _EnumValues.Add("GreenTeam");
            //write enum
            sb.Append("public enum " + MobEnumName + " { ");
            for (int i = 0; i < _EnumValues.Count; i++)
            { sb.Append(_EnumValues[i] + ", "); }
            sb.Append("} \n");
            sb.Append("\n");




            //proc gen struct, with enum above + other fields
            String structName = "Entity";
            List<string> _StructFields = new List<string>();
            _StructFields.Add("public float X, Y;");
            _StructFields.Add("public int W, H;");
            _StructFields.Add("public " + MobEnumName + " ID;");
            //write struct
            sb.Append("public struct " + structName + "\n");
            sb.Append("{ \n");
            for (int i = 0; i < _StructFields.Count; i++)
            { sb.Append("\t" + _StructFields[i] + "\n"); }
            sb.Append("} \n");
            sb.Append("\n");




            //proc gen AOS class with struct above
            String className = "EntityManager";
            int sizeOfArray = 1024;
            String arrayName = "Entities";
            List<string> _ClassMethods = new List<string>();
            _ClassMethods.Add("void Reset()");
            _ClassMethods.Add("void Update()");
            _ClassMethods.Add("void Draw()");
            //write class
            sb.Append("public class " + className + "\n");
            sb.Append("{ \n");
            //write constructor
            sb.Append("\tpublic " + className + "() { }\n");
            sb.Append("\n");
            //write methods, with tbi for loop over array
            for (int i = 0; i < _ClassMethods.Count; i++)
            {
                sb.Append("\tpublic virtual " + _ClassMethods[i] + " { }\n");
                sb.Append("\n");
            }
            sb.Append("} \n");
            sb.Append("\n");






            //design classes that inherit from base class above
            List<string> Classes = new List<string>();
            Classes.Add("MobSystemA");
            Classes.Add("MobSystemB");
            Classes.Add("MobSystemC");
            Classes.Add("MobSystemD");

            //methods specific to inherited classes
            List<string> Methods = new List<string>();
            Methods.Add("public void Fight()");
            Methods.Add("public void Animate()");
            Methods.Add("void Spawn(int X, int Y, " + MobEnumName + " ID)");

            //create classes with desired methods + signatures
            for (int i = 0; i < Classes.Count; i++)
            {
                //gimme some space between classes
                sb.Append("\n\n");
                sb.Append("public class " + Classes[i] +  " : " + className + "\n");
                sb.Append("{\n");
                sb.Append("\tpublic int size = " + sizeOfArray + ";\n");
                sb.Append("\t" + structName + "[] " + arrayName + ";\n");
                sb.Append("\n");

                //write constructor
                sb.Append("\t" + "public " + Classes[i] + "()\n");
                sb.Append("\t{ \n");
                sb.Append("\t\t" + arrayName + " = new " + structName + "[size];\n");
                sb.Append("\t} \n");
                sb.Append("\n");
                //write inherited methods
                for (int g = 0; g < _ClassMethods.Count; g++)
                {
                    sb.Append("\tpublic override " + _ClassMethods[g] + "\n");
                    sb.Append("\t{ \n");
                    sb.Append("\t\tfor(int i = 0; i < size; i++)\n");
                    sb.Append("\t\t{ \n");
                    sb.Append("\t\t\t//" + arrayName + "[i]\n");
                    sb.Append("\t\t} \n");
                    sb.Append("\t} \n");
                    sb.Append("\n");
                }
                //write bespoke methods 
                for (int g = 0; g < Methods.Count; g++)
                {
                    sb.Append("\t" + Methods[g] + "\n");
                    sb.Append("\t{ \n");
                    sb.Append("\t\tfor(int i = 0; i < size; i++)\n");
                    sb.Append("\t\t{ \n");
                    sb.Append("\t\t\t//" + arrayName + "[i]\n");
                    sb.Append("\t\t} \n");
                    sb.Append("\t} \n");
                    sb.Append("\n");
                }
                sb.Append("}\n");
            }

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