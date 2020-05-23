using System;
using System.Reflection.Metadata.Ecma335;

namespace ManagedDoom
{
    public sealed class DoomMenu
    {
        private MenuDef main;
        private MenuDef skill;
        private MenuDef current;
        private bool active;

        public DoomMenu()
        {
            skill = new MenuDef(
                this,
                "M_NEWG", 96, 14,
                "M_SKILL", 54, 38,
                2,
                new SimpleMenuItem("M_JKILL", 16, 58, 48, 63, null),
                new SimpleMenuItem("M_ROUGH", 16, 74, 48, 79, null),
                new SimpleMenuItem("M_HURT", 16, 90, 48, 95, null),
                new SimpleMenuItem("M_ULTRA", 16, 106, 48, 111, null),
                new SimpleMenuItem("M_NMARE", 16, 122, 48, 127, null));

            main = new MenuDef(
                this,
                "M_DOOM", 94, 2,
                0,
                new SimpleMenuItem("M_NGAME", 65, 67, 97, 72, skill),
                new SimpleMenuItem("M_OPTION", 65, 83, 97, 88, null),
                new SimpleMenuItem("M_LOADG", 65, 99, 97, 104, null),
                new SimpleMenuItem("M_SAVEG", 65, 115, 97, 120, null),
                new SimpleMenuItem("M_QUITG", 65, 131, 97, 136, null));

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
                    current = main;
                    active = true;
                    return true;
                }

                return false;
            }
        }

        public void SetCurrent(MenuDef next)
        {
            current = next;
        }

        public MenuDef Current => current;
        public bool Active => active;
    }
}
