using System;
using System.Reflection.Metadata.Ecma335;

namespace ManagedDoom
{
    public sealed class DoomMenu
    {
        private MenuDef main;
        private MenuDef skill;
        private MenuDef options;
        private MenuDef volume;
        private MenuDef load;
        private MenuDef save;
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

            volume = new MenuDef(
                this,
                "M_SVOL", 60, 38,
                0,
                new SliderMenuItem("M_SFXVOL", 48, 59, 80, 64, 16, 8),
                new SliderMenuItem("M_MUSVOL", 48, 91, 80, 96, 16, 8));

            options = new MenuDef(
                this,
                "M_OPTTTL", 108, 15,
                0,
                new SimpleMenuItem("M_ENDGAM", 28, 32, 60, 37, null),
                new ToggleMenuItem("M_MESSG", 28, 48, 60, 53, "M_MSGON", "M_MSGOFF", 180, 0),
                new SliderMenuItem("M_SCRNSZ", 28, 80 - 16, 60, 85 - 16, 9, 3),
                new SliderMenuItem("M_MSENS", 28, 112 - 16, 60, 117 - 16, 10, 3),
                new SimpleMenuItem("M_SVOL", 28, 144 - 16, 60, 149 - 16, volume));

            load = new MenuDef(
                this,
                "M_LOADG", 72, 28,
                0,
                new TextBoxMenuItem("TEST!!!", 48, 49, 72, 61),
                new TextBoxMenuItem("TEST!!!", 48, 65, 72, 77),
                new TextBoxMenuItem("TEST!!!", 48, 81, 72, 93),
                new TextBoxMenuItem("TEST!!!", 48, 97, 72, 109),
                new TextBoxMenuItem("TEST!!!", 48, 113, 72, 125),
                new TextBoxMenuItem("TEST!!!", 48, 129, 72, 141));

            save = new MenuDef(
                this,
                "M_SAVEG", 72, 28,
                0,
                new TextBoxMenuItem("TEST!!!", 48, 49, 72, 61),
                new TextBoxMenuItem("TEST!!!", 48, 65, 72, 77),
                new TextBoxMenuItem("TEST!!!", 48, 81, 72, 93),
                new TextBoxMenuItem("TEST!!!", 48, 97, 72, 109),
                new TextBoxMenuItem("TEST!!!", 48, 113, 72, 125),
                new TextBoxMenuItem("TEST!!!", 48, 129, 72, 141));

            main = new MenuDef(
                this,
                "M_DOOM", 94, 2,
                0,
                new SimpleMenuItem("M_NGAME", 65, 67, 97, 72, skill),
                new SimpleMenuItem("M_OPTION", 65, 83, 97, 88, options),
                new SimpleMenuItem("M_LOADG", 65, 99, 97, 104, load),
                new SimpleMenuItem("M_SAVEG", 65, 115, 97, 120, save),
                new SimpleMenuItem("M_QUITG", 65, 131, 97, 136, null));

            current = main;
            active = false;
        }

        public bool DoEvent(DoomEvent e)
        {
            if (active)
            {
                if (current.DoEvent(e))
                {
                    return true;
                }

                if (e.Key == SFML.Window.Keyboard.Key.Escape && e.Type == EventType.KeyDown)
                {
                    active = false;
                }

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

        public void Close()
        {
            active = false;
        }

        public MenuDef Current => current;
        public bool Active => active;
    }
}
