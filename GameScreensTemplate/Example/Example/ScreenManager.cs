using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Example
{
    public static class ScreenManager
    {
        public static GraphicsDeviceManager Graphics;
        public static SpriteBatch SB;
        public static ContentManager CM;
        public static GameWindow Window;

        public const int RT_WIDTH = 320;
        public const int RT_HEIGHT = 180;
        public static RenderTarget2D RT;

        public static Point GameResolution = new Point(640, 360);
        
        public static List<Screen> screens = new List<Screen>();
        public static Screen activeScreen;

        //reuse static instances of screens
        public static Screen_World World;
        public static Screen_Options Options;
        public static Screen_Title Title;

        static ScreenManager() {}

        public static void Init() 
        {
            RT = new RenderTarget2D(Graphics.GraphicsDevice, RT_WIDTH, RT_HEIGHT);

            Graphics.PreferredBackBufferWidth = GameResolution.X;
            Graphics.PreferredBackBufferHeight = GameResolution.Y;
            Graphics.ApplyChanges();

            World = new Screen_World();
            Options = new Screen_Options();
            Title = new Screen_Title();

            AddScreen(World);
        }

        public static void AddScreen(Screen screen)
        {
            screens.Add(screen);
        }

        public static void RemoveScreen(Screen screen)
        {
            screens.Remove(screen);
        }

        public static void ExitAndLoad(Screen screenToLoad)
        {
            while (screens.Count > 0)
            { screens.Remove(screens[0]); }

            AddScreen(screenToLoad);
            activeScreen = screenToLoad;
            screenToLoad.Open();
        }

        public static void Update()
        {
            if (screens.Count > 0)
            {
                activeScreen = screens[screens.Count - 1];
                activeScreen.HandleInput();
                activeScreen.Update();
            }
        }

        public static void Draw()
        {
            //set render target as destination for drawing
            Graphics.GraphicsDevice.SetRenderTarget(RT);
            Graphics.GraphicsDevice.Clear(Color.Black);

            foreach (Screen screen in screens) { screen.Draw(); }

            //set backbuffer as destination for drawing
            Graphics.GraphicsDevice.SetRenderTarget(null);

            SB.Begin(SpriteSortMode.Deferred,
                BlendState.Opaque,
                SamplerState.PointClamp);

            //draw render target to window
            SB.Draw(RT, new Rectangle(
                Graphics.PreferredBackBufferWidth / 2 - GameResolution.X / 2,
                Graphics.PreferredBackBufferHeight / 2 - GameResolution.Y / 2,
                GameResolution.X,
                GameResolution.Y),
                Color.White);

            SB.End();

        }

        public static void DrawOnlyTopScreen()
        {
            if (screens.Count > 0)
            {
                activeScreen = screens[screens.Count - 1];
                activeScreen.Draw();
            }
        }
    
        //util methods (could be moved elsewhere)
    
        public static void IterateResolutions()
        {
            Window.IsBorderless = false;

            Point r640 = new Point(640, 360);
            Point r1280 = new Point(1280, 720);
            Point r1920 = new Point(1920, 1080);

            Point fullscreen = new Point(
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);

            if (GameResolution == r640)
            {
                GameResolution.X = 1280;
                GameResolution.Y = 720;
            }
            else if (GameResolution == r1280)
            {
                GameResolution.X = 1920;
                GameResolution.Y = 1080;
            }
            else if (GameResolution == r1920)
            {
                GameResolution.X = fullscreen.X;
                GameResolution.Y = fullscreen.Y;
                Window.IsBorderless = true;
            }
            else if (GameResolution == fullscreen)
            {
                GameResolution.X = 640;
                GameResolution.Y = 360;
            }
            else
            {
                GameResolution.X = 640;
                GameResolution.Y = 360;
            }

            Graphics.PreferredBackBufferWidth = GameResolution.X;
            Graphics.PreferredBackBufferHeight = GameResolution.Y;
            Graphics.ApplyChanges();

        }
    
    
    
    }
}