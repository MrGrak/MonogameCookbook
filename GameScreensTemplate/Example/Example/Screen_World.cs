using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    public class Screen_World : Screen
    {
        //simple 2d camera
        public static Matrix Camera2D = Matrix.Identity;
        public static Vector3 translateCenter_right;
        public static Vector3 translateBody_right;

        public static MouseState currentMouseState = new MouseState();
        public static MouseState lastMouseState = new MouseState();


        public Screen_World()
        {
            Name = "world";
        }

        public override void Open()
        {
            displayState = DisplayState.Opening;
        }

        public override void Close(ExitAction EA)
        {
            exitAction = EA;
            displayState = DisplayState.Closing;
        }

        public override void HandleInput()
        {
            //process cursor input
            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            if (IsNewRightClick()) { ScreenManager.IterateResolutions(); }
        }

        public override void Update()
        {

            #region Handle Screen Display States

            if (displayState == DisplayState.Opening)
            {

            }
            else if (displayState == DisplayState.Opened)
            {

            }
            else if (displayState == DisplayState.Closing)
            {

            }
            else if (displayState == DisplayState.Closed)
            {

            }

            #endregion

        }

        public override void Draw()
        {
            //draw world view
            ScreenManager.SB.Begin(SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend,
                        SamplerState.PointClamp,
                        null, null, null, Camera2D);

            //tbi

            ScreenManager.SB.End();

            //draw screen view
            ScreenManager.SB.Begin(SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend,
                        SamplerState.PointClamp,
                        null, null, null, null);

            //tbi

            ScreenManager.SB.End();
        }

        //

        public static bool IsNewRightClick()
        {
            return (currentMouseState.RightButton == ButtonState.Pressed
                && lastMouseState.RightButton == ButtonState.Released);
        }

        public static void UpdateCameraView(int X, int Y)
        {
            Matrix matZoom = Matrix.CreateScale(1, 1, 1);

            translateCenter_right.X = 0;
            translateCenter_right.Y = 0;
            translateCenter_right.Z = 0;

            translateBody_right.X = X;
            translateBody_right.Y = Y;
            translateBody_right.Z = 0;

            Camera2D = Matrix.CreateTranslation(translateBody_right) *
                    Matrix.CreateRotationZ(0.0f) * matZoom *
                    Matrix.CreateTranslation(translateCenter_right);
        }



    }
}
