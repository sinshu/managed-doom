using System;
using System.Reflection.Metadata.Ecma335;

namespace ManagedDoom
{
    public sealed class DoomMenu
    {
        private MenuDef main;
        private MenuDef current;

        public DoomMenu()
        {
            main = new MenuDef(
                "M_DOOM", 94, 2,
                new SimpleMenuItem("M_NGAME", 65, 67, 97, 72),
                new SimpleMenuItem("M_OPTION", 65, 83, 97, 88),
                new SimpleMenuItem("M_LOADG", 65, 99, 97, 104),
                new SimpleMenuItem("M_SAVEG", 65, 115, 97, 120),
                new SimpleMenuItem("M_QUITG", 65, 131, 97, 136));

            current = main;
        }

        public bool DoEvent(DoomEvent e)
        {
            return current.DoEvent(e);
        }

        public MenuDef Current => current;
    }
}
