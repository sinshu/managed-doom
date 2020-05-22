using System;

namespace ManagedDoom
{
    public sealed class DoomMenu
    {
        private MenuDef main;

        public DoomMenu()
        {
            main = new MenuDef(
                "M_DOOM", 100, 50,
                new SimpleMenuItem("M_NGAME", 50, 60, 80, 60),
                new SimpleMenuItem("M_QUITG", 50, 80, 80, 80));
        }

        public MenuDef Current => main;
    }
}
