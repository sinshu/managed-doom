using System;
using System.Collections.Generic;

namespace ManagedDoom
{
    public sealed class MenuDef
    {
        // # of menu items
        public int NumItems;

        // previous menu
        public MenuDef PrevMenu;

        // menu items
        public List<MenuItem> MenuItems = new List<MenuItem>();

        // draw routine
        public Action Routine;

        // x,y of menu
        public int X;
        public int Y;

        // last item user was on in menu
        public int LastOn;
    }
}
