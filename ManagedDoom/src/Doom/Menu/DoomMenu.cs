using System;

namespace ManagedDoom
{
    public sealed class DoomMenu
    {
        private MenuDef main;

        public DoomMenu()
        {
            main = new MenuDef(
                "M_DOOM", 94, 2,
                new SimpleMenuItem("M_NGAME", 65, 67, 97, 72),
                new SimpleMenuItem("M_OPTION", 50, 80, 97, 88),
                new SimpleMenuItem("M_LOADG", 50, 80, 97, 104),
                new SimpleMenuItem("M_SAVEG", 50, 80, 97, 120),
                new SimpleMenuItem("M_QUITG", 50, 80, 97, 136));
        }

        public MenuDef Current => main;
    }
}
