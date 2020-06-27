using System;
using System.Reflection.Metadata.Ecma335;

namespace ManagedDoom
{
    public sealed class DoomMenu
    {
        private DoomApplication app;

        private SelectableMenu main;
        private SelectableMenu episodeMenu;
        private SelectableMenu skillMenu;
        private SelectableMenu optionMenu;
        private SelectableMenu volume;
        private LoadMenu load;
        private SaveMenu save;

        private PressAnyKey thisIsShareware;
        private PressAnyKey saveFailed;
        private YesNoConfirm nightmareConfirm;
        private QuitConfirm quitConfirm;

        private MenuDef current;

        private bool active;

        private int tics;

        private int selectedEpisode;

        private SaveSlots saveSlots;

        public DoomMenu(DoomApplication app)
        {
            this.app = app;

            thisIsShareware = new PressAnyKey(
                this,
                DoomInfo.Strings.SWSTRING,
                null);

            saveFailed = new PressAnyKey(
                this,
                DoomInfo.Strings.SAVEDEAD,
                null);

            nightmareConfirm = new YesNoConfirm(
                this,
                DoomInfo.Strings.NIGHTMARE,
                () => app.NewGame(GameSkill.Nightmare, selectedEpisode, 1));

            quitConfirm = new QuitConfirm(
                this,
                app);

            skillMenu = new SelectableMenu(
                this,
                "M_NEWG", 96, 14,
                "M_SKILL", 54, 38,
                2,
                new SimpleMenuItem("M_JKILL", 16, 58, 48, 63, () => app.NewGame(GameSkill.Baby, selectedEpisode, 1), null),
                new SimpleMenuItem("M_ROUGH", 16, 74, 48, 79, () => app.NewGame(GameSkill.Easy, selectedEpisode, 1), null),
                new SimpleMenuItem("M_HURT", 16, 90, 48, 95, () => app.NewGame(GameSkill.Medium, selectedEpisode, 1), null),
                new SimpleMenuItem("M_ULTRA", 16, 106, 48, 111, () => app.NewGame(GameSkill.Hard, selectedEpisode, 1), null),
                new SimpleMenuItem("M_NMARE", 16, 122, 48, 127, null, nightmareConfirm));

            if (app.GameMode == GameMode.Retail)
            {
                episodeMenu = new SelectableMenu(
                    this,
                    "M_EPISOD", 54, 38,
                    0,
                    new SimpleMenuItem("M_EPI1", 16, 58, 48, 63, () => selectedEpisode = 1, skillMenu),
                    new SimpleMenuItem("M_EPI2", 16, 74, 48, 79, () => selectedEpisode = 2, skillMenu),
                    new SimpleMenuItem("M_EPI3", 16, 90, 48, 95, () => selectedEpisode = 3, skillMenu),
                    new SimpleMenuItem("M_EPI4", 16, 106, 48, 111, () => selectedEpisode = 4, skillMenu));
            }
            else
            {
                if (app.GameMode == GameMode.Shareware)
                {
                    episodeMenu = new SelectableMenu(
                        this,
                        "M_EPISOD", 54, 38,
                        0,
                        new SimpleMenuItem("M_EPI1", 16, 58, 48, 63, () => selectedEpisode = 1, skillMenu),
                        new SimpleMenuItem("M_EPI2", 16, 74, 48, 79, null, thisIsShareware),
                        new SimpleMenuItem("M_EPI3", 16, 90, 48, 95, null, thisIsShareware));
                }
                else
                {
                    episodeMenu = new SelectableMenu(
                        this,
                        "M_EPISOD", 54, 38,
                        0,
                        new SimpleMenuItem("M_EPI1", 16, 58, 48, 63, () => selectedEpisode = 1, skillMenu),
                        new SimpleMenuItem("M_EPI2", 16, 74, 48, 79, () => selectedEpisode = 2, skillMenu),
                        new SimpleMenuItem("M_EPI3", 16, 90, 48, 95, () => selectedEpisode = 3, skillMenu));
                }
            }

            volume = new SelectableMenu(
                this,
                "M_SVOL", 60, 38,
                0,
                new SliderMenuItem("M_SFXVOL", 48, 59, 80, 64, 16, 8),
                new SliderMenuItem("M_MUSVOL", 48, 91, 80, 96, 16, 8));

            optionMenu = new SelectableMenu(
                this,
                "M_OPTTTL", 108, 15,
                0,
                new SimpleMenuItem("M_ENDGAM", 28, 32, 60, 37, null, null),
                new ToggleMenuItem("M_MESSG", 28, 48, 60, 53, "M_MSGON", "M_MSGOFF", 180, 0),
                new SliderMenuItem("M_SCRNSZ", 28, 80 - 16, 60, 85 - 16, 9, 3),
                new SliderMenuItem("M_MSENS", 28, 112 - 16, 60, 117 - 16, 10, 3),
                new SimpleMenuItem("M_SVOL", 28, 144 - 16, 60, 149 - 16, null, volume));

            load = new LoadMenu(
                this,
                "M_LOADG", 72, 28,
                0,
                new TextBoxMenuItem(48, 49, 72, 61),
                new TextBoxMenuItem(48, 65, 72, 77),
                new TextBoxMenuItem(48, 81, 72, 93),
                new TextBoxMenuItem(48, 97, 72, 109),
                new TextBoxMenuItem(48, 113, 72, 125),
                new TextBoxMenuItem(48, 129, 72, 141));

            save = new SaveMenu(
                this,
                "M_SAVEG", 72, 28,
                0,
                new TextBoxMenuItem(48, 49, 72, 61),
                new TextBoxMenuItem(48, 65, 72, 77),
                new TextBoxMenuItem(48, 81, 72, 93),
                new TextBoxMenuItem(48, 97, 72, 109),
                new TextBoxMenuItem(48, 113, 72, 125),
                new TextBoxMenuItem(48, 129, 72, 141));

            MenuDef newGameMenu;
            if (app.GameMode == GameMode.Commercial)
            {
                newGameMenu = skillMenu;
            }
            else
            {
                newGameMenu = episodeMenu;
            }
            main = new SelectableMenu(
                this,
                "M_DOOM", 94, 2,
                0,
                new SimpleMenuItem("M_NGAME", 65, 67, 97, 72, null, newGameMenu),
                new SimpleMenuItem("M_OPTION", 65, 83, 97, 88, null, optionMenu),
                new SimpleMenuItem("M_LOADG", 65, 99, 97, 104, null, load),
                new SimpleMenuItem("M_SAVEG", 65, 115, 97, 120, null, save),
                new SimpleMenuItem("M_QUITG", 65, 131, 97, 136, null, quitConfirm));

            current = main;
            active = false;

            tics = 0;

            selectedEpisode = 1;

            saveSlots = new SaveSlots();
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
            current.Open();
        }

        public void Open()
        {
            active = true;

            if (!app.Options.NetGame)
            {
                app.PauseGame();
            }
        }

        public void Close()
        {
            active = false;

            if (!app.Options.NetGame)
            {
                app.ResumeGame();
            }
        }

        public void NotifySaveFailed()
        {
            SetCurrent(saveFailed);
        }

        public DoomApplication Application => app;
        public MenuDef Current => current;
        public bool Active => active;
        public int Tics => tics;
        public SaveSlots SaveSlots => saveSlots;
    }
}
