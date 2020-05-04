using System;

namespace ManagedDoom
{
    public sealed class Menu
    {
        // # of menu items
        public int NumItems;

        // previous menu
        public Menu PrevMenu;

        // menu items
        public MenuItem MenuItems;

        // draw routine
        public Action Routine;

        // x,y of menu
        public int X;
        public int Y;

        // last item user was on in menu
        public int LastOn;
    }
}
