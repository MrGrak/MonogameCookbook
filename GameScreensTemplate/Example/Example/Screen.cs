using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    public enum DisplayState : byte
    {
        Opening, Opened, Closing, Closed
    }

    public enum ExitAction
    {
        CloseScreen,
        ReloadScreen,

        LoadTitleScreen,
    }

    public abstract class Screen
    {
        public String Name = "Base Screen";
        public DisplayState displayState;
        public ExitAction exitAction;

        public Screen() { }
        public virtual void Open() { }
        public virtual void Close(ExitAction EA) { }
        public virtual void HandleInput() { }
        public virtual void Update() { }
        public virtual void Draw() { }
    }
}