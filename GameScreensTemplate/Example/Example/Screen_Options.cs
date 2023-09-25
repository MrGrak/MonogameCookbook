using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    public class Screen_Options : Screen
    {

        public Screen_Options()
        {
            Name = "options";
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
            //tbi
        }

        public override void Update()
        {

            #region Handle Screen Display States

            if (displayState == DisplayState.Opening)
            {
                //maybe fade in or slide ui on screen?
            }
            else if (displayState == DisplayState.Opened)
            {
                //handle player input or idle animations?
            }
            else if (displayState == DisplayState.Closing)
            {
                //fade out or slide ui off screen
            }
            else if (displayState == DisplayState.Closed)
            {
                //depending on exit action, remove screen?
            }

            #endregion

        }

        public override void Draw()
        {
            //draw screen view
            ScreenManager.SB.Begin(SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend,
                        SamplerState.PointClamp,
                        null, null, null, null);

            //tbi

            ScreenManager.SB.End();
        }

    }
}