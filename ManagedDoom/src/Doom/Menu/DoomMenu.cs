using System;
using System.Reflection.Metadata.Ecma335;

namespace ManagedDoom
{
    public sealed class DoomMenu
    {
        private MenuDef main;
        private MenuDef current;
        private bool active;

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
            active = false;
        }

        public bool DoEvent(DoomEvent e)
        {
            if (active)
            {
                if (e.Key == SFML.Window.Keyboard.Key.Escape && e.Type == EventType.KeyDown)
                {
                    active = false;
                    return true;
                }

                current.DoEvent(e);

                return true;
            }
            else
            {
                if (e.Key == SFML.Window.Keyboard.Key.Escape && e.Type == EventType.KeyDown)
                {
                    active = true;
                    return true;
                }

                return false;
            }
        }

        public MenuDef Current => current;
        public bool Active => active;
    }
}
