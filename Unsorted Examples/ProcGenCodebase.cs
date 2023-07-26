using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//can we generate a 1mil loc codebase in public static style?
//lets do this by passing params between a chain of public static methods
//we should have a value that controls how many methods we chain
//mrgrak jan 2021 mit license

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
            GenerateCodebase();
            Exit(); //then exit window, lol
        }

        protected override void Update(GameTime gameTime){ }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    
        //

        public void GenerateCodebase()
        {
            StringBuilder sb = new StringBuilder();
            string NameSpace = "Namespace_" + DateTime.Now.Ticks;
            int total = 100; //total number of methods added to codebase

            sb.Append("//procgen codebase test mrgrak2021\n");
            sb.Append("namespace " + NameSpace + "\n");
            sb.Append("{\n");
            sb.Append("\tpublic static class Test_" + DateTime.Now.Ticks + "\n");
            sb.Append("\t{\n");
            sb.Append("\t\tpublic static void Start()\n");
            sb.Append("\t\t{\n");
            sb.Append("\t\t\tMethod_0();\n");
            sb.Append("\t\t}\n");
            //write expanding method chain
            for (int i = 0; i < total; i++)
            {
                sb.Append("\t\tpublic static bool Method_" + i + "()\n");
                sb.Append("\t\t{\n");
                //call the next method
                sb.Append("\t\t\treturn Method_" + (i+1) + "();\n");
                sb.Append("\t\t}\n");
            }
            //write last method, returning true
            sb.Append("\t\tpublic static bool Method_" + total + "()\n");
            sb.Append("\t\t{\n");
            sb.Append("\t\t\treturn true;\n");
            sb.Append("\t\t}\n");
            //append closing brackets for class and namespace
            sb.Append("\t}\n");
            sb.Append("}\n");
            //write to output file
            string dir = Path.Combine(GetExecutingDirectoryName(), "Output.cs");
            Debug.WriteLine("writing to: " + dir);
            using (StreamWriter writer = new StreamWriter(dir))
            { writer.WriteLine(sb.ToString()); }
            //Debug.WriteLine(sb.ToString());
        }

        public static string GetExecutingDirectoryName()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            return new FileInfo(location.AbsolutePath).Directory.FullName;
        }

    }
}