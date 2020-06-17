using System;
using System.Reflection.Metadata.Ecma335;

namespace ManagedDoom
{
    public sealed class DoomMenu
    {
        private DoomApplication app;

        private MenuDef main;
        private MenuDef episode;
        private MenuDef skill;
        private MenuDef options;
        private MenuDef volume;
        private MenuDef load;
        private MenuDef save;
        private MenuDef current;
        private bool active;

        private int tics;

        public DoomMenu(DoomApplication app)
        {
            this.app = app;

            skill = new MenuDef(
                this,
                "M_NEWG", 96, 14,
                "M_SKILL", 54, 38,
                2,
                new SimpleMenuItem("M_JKILL", 16, 58, 48, 63, () => app.NewGame(), null),
                new SimpleMenuItem("M_ROUGH", 16, 74, 48, 79, () => app.NewGame(), null),
                new SimpleMenuItem("M_HURT", 16, 90, 48, 95, () => app.NewGame(), null),
                new SimpleMenuItem("M_ULTRA", 16, 106, 48, 111, () => app.NewGame(), null),
                new SimpleMenuItem("M_NMARE", 16, 122, 48, 127, () => app.NewGame(), null));

            episode = new MenuDef(
                this,
                "M_EPISOD", 54, 38,
                0,
                new SimpleMenuItem("M_EPI1", 16, 58, 48, 63, null, skill),
                new SimpleMenuItem("M_EPI2", 16, 74, 48, 79, null, skill),
                new SimpleMenuItem("M_EPI3", 16, 90, 48, 95, null, skill),
                new SimpleMenuItem("M_EPI4", 16, 106, 48, 111, null, skill));

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
                new SimpleMenuItem("M_ENDGAM", 28, 32, 60, 37, null, null),
                new ToggleMenuItem("M_MESSG", 28, 48, 60, 53, "M_MSGON", "M_MSGOFF", 180, 0),
                new SliderMenuItem("M_SCRNSZ", 28, 80 - 16, 60, 85 - 16, 9, 3),
                new SliderMenuItem("M_MSENS", 28, 112 - 16, 60, 117 - 16, 10, 3),
                new SimpleMenuItem("M_SVOL", 28, 144 - 16, 60, 149 - 16, null, volume));

            load = new MenuDef(
                this,
                "M_LOADG", 72, 28,
                0,
                new TextBoxMenuItem("TEST!!!", 48, 49, 72, 61),
                new TextBoxMenuItem("TEST!!!", 48, 65, 72, 77),
                new TextBoxMenuItem("TE ST!!!", 48, 81, 72, 93),
                new TextBoxMenuItem("TE  ST!!!", 48, 97, 72, 109),
                new TextBoxMenuItem("TEST!!!", 48, 113, 72, 125),
                new TextBoxMenuItem("TEST!!!", 48, 129, 72, 141));

            save = new MenuDef(
                this,
                "M_SAVEG", 72, 28,
                0,
                new TextBoxMenuItem("TEST!!!", 48, 49, 72, 61),
                new TextBoxMenuItem("TEST!!!", 48, 65, 72, 77),
                new TextBoxMenuItem("TE ST!!!", 48, 81, 72, 93),
                new TextBoxMenuItem("TE ST!!!", 48, 97, 72, 109),
                new TextBoxMenuItem("test???", 48, 113, 72, 125),
                new TextBoxMenuItem("testtest___", 48, 129, 72, 141));

            MenuDef newGameMenu;
            if (app.GameMode == GameMode.Commercial)
            {
                newGameMenu = skill;
            }
            else
            {
                newGameMenu = episode;
            }
            main = new MenuDef(
                this,
                "M_DOOM", 94, 2,
                0,
                new SimpleMenuItem("M_NGAME", 65, 67, 97, 72, null, newGameMenu),
                new SimpleMenuItem("M_OPTION", 65, 83, 97, 88, null, options),
                new SimpleMenuItem("M_LOADG", 65, 99, 97, 104, null, load),
                new SimpleMenuItem("M_SAVEG", 65, 115, 97, 120, null, save),
                new SimpleMenuItem("M_QUITG", 65, 131, 97, 136, null, null));

            current = main;
            active = false;

            tics = 0;
        }

        public bool DoEvent(DoomEvent e)
        {
            if (active)
            {
                if (current.DoEvent(e))
                {
                    return true;
                }

                if (e.Key == DoomKeys.Escape && e.Type == EventType.KeyDown)
                {
                    Close();
                }

                return true;
            }
            else
            {
                if (e.Key == DoomKeys.Escape && e.Type == EventType.KeyDown)
                {
                    current = main;
                    Open();
                    return true;
                }

                if (e.Type == EventType.KeyDown && app.State == ApplicationState.Opening)
                {
                    current = main;
                    Open();
                    return true;
                }

                return false;
            }
        }

        public void Update()
        {
            tics++;
        }

        public void SetCurrent(MenuDef next)
        {
            current = next;
        }

        public void Open()
        {
            active = true;
        }

        public void Close()
        {
            active = false;
        }

        public MenuDef Current => current;
        public bool Active => active;
        public int Tics => tics;
    }
}
