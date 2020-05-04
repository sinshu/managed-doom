using System;

namespace ManagedDoom
{
    public sealed class MenuItem
    {
        // 0 = no cursor here, 1 = ok, 2 = arrows ok
        public short Status;

        public string Name;

        // choice = menu item #.
        // if status = 2,
        //   choice = 0: leftarrow,
        //            1: rightarrow
        public Action<int> Routine;

        // hotkey in menu
        public char AlphaKey;
    }
}
