using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace Game1
{
    public static class Prototype
    {
        //prototype goals:
        //split screen into two render targets
        //based on cursor position, update + draw one of the two rts
        //provide a debug view that draws and updates all ui at once
        //compare debug vs partitoned modes via on screen timer
        //draw time should be lower when we update+draw less

        //r click to switch into debug mode (red)
        //l click to switch into partitioned rt mode (green)
        
        public static RenderTarget2D rt_screen_left;
        public static RenderTarget2D rt_screen_right;
        public static byte rt_active = 0; //tracks which rt has focus

        public static RenderTarget2D rt_screen_backbuffer;
        public static bool rt_dirty; //trigger redraw of full frame

        public const int screen_width = 1280;
        public const int screen_height = 720;

        public static Game1 Game;
        public static GraphicsDeviceManager GraphicsDeviceMan;
        public static ContentManager ContentMan;
        public static SpriteBatch SpriteBat;
        public static Texture2D LineTexture;
        
        public static MouseState currentMouseState = new MouseState();
        public static MouseState lastMouseState = new MouseState();

        public static List<Rectangle> Ui_Left = new List<Rectangle>();
        public static List<Rectangle> Ui_Right = new List<Rectangle>();
        public const int Ui_total = 100; //value * 2 (for each list of buttons)
        public static int current_selection = -1; //no button under cursor
        public static int selectionOffset = 100;
        
        public static Color Ui_Button_Left_Color = Color.Green;
        public static Color Ui_Button_Right_Color = Color.Blue;
        public static Color Ui_Button_Over_Color = Color.Yellow;
        public static Color Ui_Cursor_Color = new Color(20, 20, 20, 100);
        
        //left view/camera
        public static Matrix view_left = Matrix.Identity;
        public static Vector3 translateCenter_left;
        public static Vector3 translateBody_left;

        //right view/camera
        public static Matrix view_right = Matrix.Identity;
        public static Vector3 translateCenter_right;
        public static Vector3 translateBody_right;

        //time + swap between debug draw all and partitioned rts
        public static Rectangle drawTime;
        public static Stopwatch Timer;
        public static bool DebugView = false;

        

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

            //create rts that preserve contents
            SurfaceFormat SF = SurfaceFormat.Color;
            DepthFormat DF = DepthFormat.Depth24;

            rt_screen_left = new RenderTarget2D(
                GraphicsDeviceMan.GraphicsDevice,
                screen_width / 2, screen_height, false,
                SF, DF, 0, RenderTargetUsage.PreserveContents);

            rt_screen_right = new RenderTarget2D(
                GraphicsDeviceMan.GraphicsDevice,
                screen_width, screen_height, false,
                SF, DF, 0, RenderTargetUsage.PreserveContents);
            
            rt_screen_backbuffer = new RenderTarget2D(
                GraphicsDeviceMan.GraphicsDevice,
                screen_width, screen_height, false,
                SF, DF, 0, RenderTargetUsage.PreserveContents);
            
            //setup texture to use for drawing rectangles + color + alpha
            LineTexture = new Texture2D(GraphicsDeviceMan.GraphicsDevice, 1, 1);
            LineTexture.SetData<Color>(new Color[] { Color.White });

            //populate ui lists with buttons
            int XposOffset = 0;
            int Ypos = 0;
            for (int i = 0; i < Ui_total; i++)
            {
                //push right every 20 iterations
                if (i >= 20 && i % 20 == 0) { XposOffset += 110; Ypos = 0; }

                Ui_Left.Add(new Rectangle(0 + 32 + XposOffset, 
                    64 + (Ypos * 30), 100, 25));
                Ui_Right.Add(new Rectangle(screen_width / 2 + 32 + XposOffset, 
                    64 + (Ypos * 30), 100, 25));

                Ypos++;
            }
            
            SetViews(); //set camera positions + targets
            Timer = new Stopwatch();
        }

        public static void Update()
        {
            Timer.Restart();

            //process cursor input
            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            //clear any button selection
            current_selection = -1;

            #region Debug Path

            if (DebugView) //update all ui elements
            {
                for (int i = 0; i < Ui_total; i++)
                {   
                    //check all ui elements on left
                    if (Contains(Ui_Left[i].X, Ui_Left[i].Y, Ui_Left[i].Width, Ui_Left[i].Height,
                        currentMouseState.X, currentMouseState.Y))
                    { current_selection = i; }

                    //check all ui elements on right
                    if (Contains(Ui_Right[i].X, Ui_Right[i].Y, Ui_Right[i].Width, Ui_Right[i].Height,
                        currentMouseState.X, currentMouseState.Y))
                    { current_selection = i + selectionOffset; } //hack, using an offset
                }
            }

            #endregion

            #region Partitioned Path with rts

            else
            {
                rt_dirty = false; //assume clean

                //based on cursor pos, update left or right ui elements, set active rt
                if (Contains(0, 0, screen_width / 2, screen_height, currentMouseState.X, currentMouseState.Y))
                {   //cursor is over left rt, rt is dirty
                    rt_active = 1; rt_dirty = true;
                    for (int i = 0; i < Ui_total; i++)
                    {   //check all ui elements on left
                        if (Contains(Ui_Left[i].X, Ui_Left[i].Y, Ui_Left[i].Width, Ui_Left[i].Height,
                            currentMouseState.X, currentMouseState.Y))
                        { current_selection = i; }
                    }
                }
                else if (Contains(screen_width / 2, 0, screen_width / 2, screen_height, currentMouseState.X, currentMouseState.Y))
                {   //cursor is over right rt, rt is dirty
                    rt_active = 2; rt_dirty = true;
                    for (int i = 0; i < Ui_total; i++)
                    {   //check all ui elements on right
                        if (Contains(Ui_Right[i].X, Ui_Right[i].Y, Ui_Right[i].Width, Ui_Right[i].Height,
                            currentMouseState.X, currentMouseState.Y))
                        { current_selection = i + selectionOffset; } //hack, using an offset
                    }
                }
                else { rt_active = 0; }
            }

            #endregion


            //check for debug swapping
            if (IsNewRightClick()) { DebugView = true; }
            if (IsNewLeftClick()) { DebugView = false; }
        }

        public static void Draw()
        {
            //dont clear the back buffer, just draw over it
            //GraphicsDeviceMan.GraphicsDevice.Clear(Color.Black);

            if (DebugView)
            {
                #region Draw all ui elements

                GraphicsDeviceMan.GraphicsDevice.SetRenderTarget(null);

                SpriteBat.Begin(SpriteSortMode.BackToFront,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    null, null, null, null);

                Draw_Rectangle(currentMouseState.X - 4, currentMouseState.Y - 4,
                    8, 8, Ui_Cursor_Color, 0.05f, 0.1f);

                Color buttonColor = Ui_Button_Over_Color;
                for (int i = 0; i < Ui_total; i++)
                {
                    //draw all left ui buttons
                    if (current_selection == i)
                    {  buttonColor = Ui_Button_Over_Color; }
                    else { buttonColor = Ui_Button_Left_Color; }

                    Draw_Rectangle(Ui_Left[i].X, Ui_Left[i].Y,
                        Ui_Left[i].Width, Ui_Left[i].Height,
                        buttonColor, 1.0f, 0.01f);
                    
                    //draw all right ui buttons
                    if (current_selection - selectionOffset == i)
                    { buttonColor = Ui_Button_Over_Color; }
                    else { buttonColor = Ui_Button_Right_Color; }

                    Draw_Rectangle(Ui_Right[i].X, Ui_Right[i].Y,
                        Ui_Right[i].Width, Ui_Right[i].Height,
                        buttonColor, 1.0f, 0.01f);
                }
                SpriteBat.End();

                #endregion
            }
            else
            {
                #region Left Rt is active

                if (rt_active == 1 && rt_dirty) //left rt is active + dirty
                {
                    //draw game to left rt
                    GraphicsDeviceMan.GraphicsDevice.SetRenderTarget(rt_screen_left);
                    SpriteBat.Begin(SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend,
                        SamplerState.PointClamp,
                        null, null, null, null);

                    Draw_Rectangle(currentMouseState.X - 4, currentMouseState.Y - 4, 
                        8, 8, Ui_Cursor_Color, 0.05f, 0.005f);

                    for (int i = 0; i < Ui_total; i++)
                    {
                        if (current_selection == i)
                        {
                            Draw_Rectangle(Ui_Left[i].X, Ui_Left[i].Y,
                              Ui_Left[i].Width, Ui_Left[i].Height,
                              Ui_Button_Over_Color, 1.0f, 0.01f);
                        }
                        else
                        {
                            Draw_Rectangle(Ui_Left[i].X, Ui_Left[i].Y,
                              Ui_Left[i].Width, Ui_Left[i].Height,
                              Ui_Button_Left_Color, 1.0f, 0.01f);
                        }
                    }

                    SpriteBat.End();

                    //draw left rt to rt_screen_backbuffer
                    GraphicsDeviceMan.GraphicsDevice.SetRenderTarget(rt_screen_backbuffer);
                    SpriteBat.Begin(SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend, SamplerState.PointClamp,
                        null, null, null, null);
                    SpriteBat.Draw(rt_screen_left, new Rectangle(0, 0, 
                        screen_width / 2, screen_height), Color.White);
                    SpriteBat.End();
                }

                #endregion

                #region Right Rt is active

                else if (rt_active == 2 && rt_dirty)  //right rt is active + dirty
                {
                    //draw game to right rt
                    GraphicsDeviceMan.GraphicsDevice.SetRenderTarget(rt_screen_right);
                    SpriteBat.Begin(SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend,
                        SamplerState.PointClamp,
                        null, null, null, view_right);

                    Draw_Rectangle(currentMouseState.X - 4, currentMouseState.Y - 4, 
                        8, 8, Ui_Cursor_Color, 0.05f, 0.005f);

                    for (int i = 0; i < Ui_total; i++)
                    {
                        if ((current_selection - selectionOffset) == i)
                        {
                            Draw_Rectangle(Ui_Right[i].X, Ui_Right[i].Y,
                                Ui_Right[i].Width, Ui_Right[i].Height,
                                Ui_Button_Over_Color, 1.0f, 0.01f);
                        }
                        else
                        {
                            Draw_Rectangle(Ui_Right[i].X, Ui_Right[i].Y,
                                Ui_Right[i].Width, Ui_Right[i].Height,
                                Ui_Button_Right_Color, 1.0f, 0.01f);
                        }
                    }

                    SpriteBat.End();

                    //draw right rt to rt_screen_backbuffer
                    GraphicsDeviceMan.GraphicsDevice.SetRenderTarget(rt_screen_backbuffer);
                    SpriteBat.Begin(SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend, SamplerState.PointClamp,
                        null, null, null, null);
                    SpriteBat.Draw(rt_screen_right, new Rectangle(
                        (screen_width / 2), 0, screen_width, screen_height), Color.White);
                    SpriteBat.End();
                }

                #endregion

                #region Redraw full screen if left or right rts were updated/marked dirty

                if (rt_dirty)
                {
                    GraphicsDeviceMan.GraphicsDevice.SetRenderTarget(null);
                    SpriteBat.Begin(SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend, SamplerState.PointClamp,
                        null, null, null, null);
                    SpriteBat.Draw(rt_screen_backbuffer, new Rectangle(0, 0, screen_width, screen_height), Color.White);
                    SpriteBat.End();
                }

                #endregion

            }
            
            Timer.Stop(); //draw time it took to draw as width always
            Color TimeColor = Color.Green;
            if (DebugView) { TimeColor = Color.Red; }
            GraphicsDeviceMan.GraphicsDevice.SetRenderTarget(null);
            SpriteBat.Begin(SpriteSortMode.BackToFront,
                BlendState.AlphaBlend, SamplerState.PointClamp,
                null, null, null, null);
            //draw background rec
            Draw_Rectangle(0, 0, screen_width, 20,
                Color.Black, 1.0f, 0.02f);
            //draw timing rec
            Draw_Rectangle(10, 10, (int)Timer.ElapsedTicks, 8,
                TimeColor, 1.0f, 0.01f);
            SpriteBat.End();
        }

        //util methods

        public static void Draw_Rectangle(
            int X, int Y, int W, int H,
            Color color, float alpha, float layer)
        {
            Rectangle DrawRec = new Rectangle();
            DrawRec.X = X; DrawRec.Y = Y;
            DrawRec.Width = W; DrawRec.Height = H;
            SpriteBat.Draw( LineTexture, new Vector2(X, Y),
                DrawRec, color * alpha, 0f, Vector2.Zero, 1.0f,
                SpriteEffects.None, layer);
        }

        public static Boolean Contains(int X, int Y, int W, int H, float x, float y)
        {
            return ((((X <= x) && (x < (X + W))) && (Y <= y)) && (y < (Y + H)));
        }

        public static bool IsNewLeftClick()
        {
            return (currentMouseState.LeftButton == ButtonState.Pressed
                && lastMouseState.LeftButton == ButtonState.Released);
        }

        public static bool IsNewRightClick()
        {
            return (currentMouseState.RightButton == ButtonState.Pressed
                && lastMouseState.RightButton == ButtonState.Released);
        }

        public static void SetViews()
        {
            Matrix matZoom = Matrix.CreateScale(1, 1, 1);

            translateCenter_left.X = 0;
            translateCenter_left.Y = 0;
            translateCenter_left.Z = 0;

            translateBody_left.X = 0; //x pos
            translateBody_left.Y = 0; //y pos
            translateBody_left.Z = 0;

            view_left = Matrix.CreateTranslation(translateBody_left) *
                    Matrix.CreateRotationZ(0.0f) * matZoom *
                    Matrix.CreateTranslation(translateCenter_left);

            translateCenter_right.X = 0;
            translateCenter_right.Y = 0;
            translateCenter_right.Z = 0;

            translateBody_right.X = (screen_width / 2) * -1; //x pos
            translateBody_right.Y = 0; //y pos
            translateBody_right.Z = 0;
            
            view_right = Matrix.CreateTranslation(translateBody_right) *
                    Matrix.CreateRotationZ(0.0f) * matZoom *
                    Matrix.CreateTranslation(translateCenter_right);
        }

    }
}