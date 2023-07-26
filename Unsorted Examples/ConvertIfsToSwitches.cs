using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

//in this example, we look for a c# Test.txt file in the exe directory
//we do string comparisons on the text, looking for ifs and else ifs
//then we convert those into cases within a switch statement
//the formatting of the source text is limited
//only works on patterned ifs, that aren't indented
//mrgrak2021

namespace Game1
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ConvertIfs();
            Exit();
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime) { }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }

        //

        public static void ConvertIfs()
        {
            string ExeFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string ReadPath = Path.Combine(ExeFolder, "Test.txt");
            string WritePath = Path.Combine(ExeFolder, "Test_Converted.txt");
            string nl = Environment.NewLine;
            List<string> Source = new List<string>();
            List<string> Converted = new List<string>();

            try
            {
                File.OpenText(ReadPath);
                var data = File.ReadAllLines(ReadPath);
                int size = data.Count();
                for (int i = 0; i < size; i++)
                {
                    //Debug.WriteLine(data[i]);
                    Source.Add(data[i]);
                }
            }
            catch (Exception E) { Debug.WriteLine("!LOAD exception: " + E); }
            

            
            {
                int size = Source.Count();
                bool ifFound = false;

                //alter source from ifs to switches
                for (int i = 0; i < size; i++)
                {
                    int count = Source[i].Count();
                    bool matched = false;


                    #region Remove Region/EndRegion directives
                    
                    if (Source[i].Count() > 8)
                    {
                        //check for region
                        if (Source[i][0] == '#' &&
                            Source[i][1] == 'r' &&
                            Source[i][2] == 'e' &&
                            Source[i][3] == 'g' &&
                            Source[i][4] == 'i' &&
                            Source[i][5] == 'o' &&
                            Source[i][6] == 'n' &&
                            Source[i][7] == ' ')
                        {
                            Source[i] = "";
                            count = 0;
                        }
                    }

                    if (Source[i].Count() > 8)
                    {
                        //check for end region
                        if (Source[i][0] == '#' &&
                            Source[i][1] == 'e' &&
                            Source[i][2] == 'n' &&
                            Source[i][3] == 'd' &&
                            Source[i][4] == 'r' &&
                            Source[i][5] == 'e' &&
                            Source[i][6] == 'g' &&
                            Source[i][7] == 'i')
                        {
                            Source[i] = "";
                            count = 0;
                        }
                    }
                    
                    #endregion


                    #region Check for Ifs

                    if (count > 3)
                    {
                        //check for 'if('
                        if (Source[i][0] == 'i' &&
                            Source[i][1] == 'f' &&
                            Source[i][2] == '(')
                        {
                            matched = true;
                            //locate equality operator
                            int equals = Source[i].IndexOf('=');
                            //substring after if to equals
                            string comp = Source[i].Substring(2, equals - 4);
                            if (ifFound == false)
                            {
                                ifFound = true;
                                Converted.Add("switch " + comp + ")");
                                Converted.Add("{");
                            }

                            //substring to comparison
                            comp = Source[i].Substring(equals + 3);
                            comp = comp.Substring(0, comp.Length - 1);
                            Converted.Add("\t" + @"#region " + comp + nl);
                            Converted.Add("\tcase " + comp + ":");
                        }
                    }

                    if(count > 4)
                    {
                        //check for 'if ('
                        if (Source[i][0] == 'i' &&
                            Source[i][1] == 'f' &&
                            Source[i][2] == ' ' &&
                            Source[i][3] == '(')
                        {
                            matched = true;
                            //locate equality operator
                            int equals = Source[i].IndexOf('=');
                            //substring after if to equals
                            string comp = Source[i].Substring(3, equals - 4);
                            if (ifFound == false)
                            {
                                ifFound = true;
                                Converted.Add("switch " + comp + ")");
                                Converted.Add("{");
                            }
                            
                            //substring to comparison
                            comp = Source[i].Substring(equals + 3);
                            comp = comp.Substring(0, comp.Length - 1);
                            Converted.Add("\t" + @"#region " + comp + nl);
                            Converted.Add("\tcase " + comp + ":");
                        }
                    }

                    #endregion


                    #region Check Else Ifs

                    if (Source[i].Count() > 8)
                    {
                        //check for 'else if('
                        if (Source[i][0] == 'e' &&
                            Source[i][1] == 'l' &&
                            Source[i][2] == 's' &&
                            Source[i][3] == 'e' &&
                            Source[i][4] == ' ' &&
                            Source[i][5] == 'i' &&
                            Source[i][6] == 'f' &&
                            Source[i][7] == '(')
                        {
                            matched = true;

                            //locate equality operator
                            int equals = Source[i].IndexOf('=');
                            //substring to comparison
                            string comp = Source[i].Substring(equals + 3);
                            comp = comp.Substring(0, comp.Length - 1);
                            Converted.Add("\t" + @"#region " + comp + nl);
                            Converted.Add("\tcase " + comp + ":");
                        }
                    }


                    if (Source[i].Count() > 9)
                    {
                        //check for 'else if ('
                        if (Source[i][0] == 'e' &&
                            Source[i][1] == 'l' &&
                            Source[i][2] == 's' &&
                            Source[i][3] == 'e' &&
                            Source[i][4] == ' ' &&
                            Source[i][5] == 'i' &&
                            Source[i][6] == 'f' &&
                            Source[i][7] == ' ' &&
                            Source[i][8] == '(')
                        {
                            matched = true;
                            //locate equality operator
                            int equals = Source[i].IndexOf('=');
                            //substring to comparison
                            string comp = Source[i].Substring(equals + 3);
                            comp = comp.Substring(0, comp.Length - 1);
                            Converted.Add("\t" + @"#region " + comp + nl);
                            Converted.Add("\tcase " + comp + ":");
                        }
                    }

                    #endregion




                    //if we didn't convert/match string, just paste it w/ tab
                    if (matched == false)
                    {
                        //remove empty lines
                        if (Source[i] == "") { }

                        //add regioning directives around lonely brackets
                        else if (Source[i] == "}")
                        {
                            Converted.Add("\t" + Source[i]);
                            Converted.Add(nl + "\t" + @"#endregion" + nl);
                        }

                        //pass along source with just tab added
                        else { Converted.Add("\t" + Source[i]); }
                    }


                    if (i == size - 1)
                    {
                        //we converted if to switch, add closing bracket
                        if (ifFound) { Converted.Add("}"); }
                    }
                }
            }

            
            //write converted source to file
            using (StreamWriter outputFile = new StreamWriter(
                WritePath, false))
            {
                for (int i = 0; i < Converted.Count(); i++)
                {
                    outputFile.WriteLine(Converted[i]);
                    Debug.WriteLine(Converted[i]);
                }
            }
        }



    }
}