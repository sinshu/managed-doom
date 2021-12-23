//
// Copyright (C) 1993-1996 Id Software, Inc.
// Copyright (C) 2019-2020 Nobuaki Tanaka
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//



using System;

namespace ManagedDoom
{
    public sealed class DoomMenu
    {
        private Doom doom;

        private SelectableMenu main;
        private SelectableMenu episodeMenu;
        private SelectableMenu skillMenu;
        private SelectableMenu optionMenu;
        private SelectableMenu volume;
        private LoadMenu load;
        private SaveMenu save;
        private HelpScreen help;

        private PressAnyKey thisIsShareware;
        private PressAnyKey saveFailed;
        private YesNoConfirm nightmareConfirm;
        private YesNoConfirm endGameConfirm;
        private QuitConfirm quitConfirm;

        private MenuDef current;

        private bool active;

        private int tics;

        private int selectedEpisode;

        private SaveSlots saveSlots;

        public DoomMenu(Doom doom)
        {
            this.doom = doom;

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
                () => doom.NewGame(GameSkill.Nightmare, selectedEpisode, 1));

            endGameConfirm = new YesNoConfirm(
                this,
                DoomInfo.Strings.ENDGAME,
                () => doom.EndGame());

            quitConfirm = new QuitConfirm(
                this,
                doom);

            skillMenu = new SelectableMenu(
                this,
                "M_NEWG", 96, 14,
                "M_SKILL", 54, 38,
                2,

                new SimpleMenuItem(
                    "M_JKILL", 16, 58, 48, 63,
                    () => doom.NewGame(GameSkill.Baby, selectedEpisode, 1),
                    null),

                new SimpleMenuItem(
                    "M_ROUGH", 16, 74, 48, 79,
                    () => doom.NewGame(GameSkill.Easy, selectedEpisode, 1),
                    null),

                new SimpleMenuItem(
                    "M_HURT", 16, 90, 48, 95,
                    () => doom.NewGame(GameSkill.Medium, selectedEpisode, 1),
                    null),

                new SimpleMenuItem(
                    "M_ULTRA", 16, 106, 48, 111,
                    () => doom.NewGame(GameSkill.Hard, selectedEpisode, 1),
                    null),

                new SimpleMenuItem(
                    "M_NMARE", 16, 122, 48, 127,
                    null,
                    nightmareConfirm));

            if (doom.Options.GameMode == GameMode.Retail)
            {
                episodeMenu = new SelectableMenu(
                    this,
                    "M_EPISOD", 54, 38,
                    0,

                    new SimpleMenuItem(
                        "M_EPI1", 16, 58, 48, 63,
                        () => selectedEpisode = 1,
                        skillMenu),

                    new SimpleMenuItem(
                        "M_EPI2", 16, 74, 48, 79,
                        () => selectedEpisode = 2,
                        skillMenu),

                    new SimpleMenuItem(
                        "M_EPI3", 16, 90, 48, 95,
                        () => selectedEpisode = 3,
                        skillMenu),

                    new SimpleMenuItem(
                        "M_EPI4", 16, 106, 48, 111,
                        () => selectedEpisode = 4,
                        skillMenu));
            }
            else
            {
                if (doom.Options.GameMode == GameMode.Shareware)
                {
                    episodeMenu = new SelectableMenu(
                        this,
                        "M_EPISOD", 54, 38,
                        0,

                        new SimpleMenuItem(
                            "M_EPI1", 16, 58, 48, 63,
                            () => selectedEpisode = 1,
                            skillMenu),

                        new SimpleMenuItem(
                            "M_EPI2", 16, 74, 48, 79,
                            null,
                            thisIsShareware),

                        new SimpleMenuItem(
                            "M_EPI3", 16, 90, 48, 95,
                            null,
                            thisIsShareware));
                }
                else
                {
                    episodeMenu = new SelectableMenu(
                        this,
                        "M_EPISOD", 54, 38,
                        0,

                        new SimpleMenuItem(
                            "M_EPI1", 16, 58, 48, 63,
                            () => selectedEpisode = 1,
                            skillMenu),
                        new SimpleMenuItem(
                            "M_EPI2", 16, 74, 48, 79,
                            () => selectedEpisode = 2,
                            skillMenu),
                        new SimpleMenuItem(
                            "M_EPI3", 16, 90, 48, 95,
                            () => selectedEpisode = 3,
                            skillMenu));
                }
            }

            var sound = doom.Options.Sound;
            var music = doom.Options.Music;
            volume = new SelectableMenu(
                this,
                "M_SVOL", 60, 38,
                0,

                new SliderMenuItem(
                    "M_SFXVOL", 48, 59, 80, 64,
                    sound.MaxVolume + 1,
                    () => sound.Volume,
                    vol => sound.Volume = vol),

                new SliderMenuItem("M_MUSVOL", 48, 91, 80, 96,
                    music.MaxVolume + 1,
                    () => music.Volume,
                    vol => music.Volume = vol));

            var video = doom.Options.Video;
            var userInput = doom.Options.UserInput;
            optionMenu = new SelectableMenu(
                this,
                "M_OPTTTL", 108, 15,
                0,

                new SimpleMenuItem(
                    "M_ENDGAM", 28, 32, 60, 37,
                    null,
                    endGameConfirm,
                    () => doom.State == DoomState.Game),

                new ToggleMenuItem(
                    "M_MESSG", 28, 48, 60, 53, "M_MSGON", "M_MSGOFF", 180,
                    () => video.DisplayMessage ? 0 : 1,
                    value => video.DisplayMessage = value == 0),

                new SliderMenuItem(
                    "M_SCRNSZ", 28, 80 - 16, 60, 85 - 16,
                    video.MaxWindowSize + 1,
                    () => video.WindowSize,
                    size => video.WindowSize = size),

                new SliderMenuItem(
                    "M_MSENS", 28, 112 - 16, 60, 117 - 16,
                    userInput.MaxMouseSensitivity + 1,
                    () => userInput.MouseSensitivity,
                    ms => userInput.MouseSensitivity = ms),

                new SimpleMenuItem(
                    "M_SVOL", 28, 144 - 16, 60, 149 - 16,
                    null,
                    volume));

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

            help = new HelpScreen(this);

            if (doom.Options.GameMode == GameMode.Commercial)
            {
                main = new SelectableMenu(
                this,
                "M_DOOM", 94, 2,
                0,
                new SimpleMenuItem("M_NGAME", 65, 67, 97, 72, null, skillMenu),
                new SimpleMenuItem("M_OPTION", 65, 83, 97, 88, null, optionMenu),
                new SimpleMenuItem("M_LOADG", 65, 99, 97, 104, null, load),
                new SimpleMenuItem("M_SAVEG", 65, 115, 97, 120, null, save,
                    () => !(doom.State == DoomState.Game &&
                        doom.Game.State != GameState.Level)),
                new SimpleMenuItem("M_QUITG", 65, 131, 97, 136, null, quitConfirm));
            }
            else
            {
                main = new SelectableMenu(
                this,
                "M_DOOM", 94, 2,
                0,
                new SimpleMenuItem("M_NGAME", 65, 59, 97, 64, null, episodeMenu),
                new SimpleMenuItem("M_OPTION", 65, 75, 97, 80, null, optionMenu),
                new SimpleMenuItem("M_LOADG", 65, 91, 97, 96, null, load),
                new SimpleMenuItem("M_SAVEG", 65, 107, 97, 112, null, save,
                    () => !(doom.State == DoomState.Game &&
                        doom.Game.State != GameState.Level)),
                new SimpleMenuItem("M_RDTHIS", 65, 123, 97, 128, null, help),
                new SimpleMenuItem("M_QUITG", 65, 139, 97, 144, null, quitConfirm));
            }

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

                if (e.Key == DoomKey.Escape && e.Type == EventType.KeyDown)
                {
                    Close();
                }

                return true;
            }
            else
            {
                if (e.Key == DoomKey.Escape && e.Type == EventType.KeyDown)
                {
                    SetCurrent(main);
                    Open();
                    StartSound(Sfx.SWTCHN);
                    return true;
                }

                if (e.Type == EventType.KeyDown && doom.State == DoomState.Opening)
                {
                    if (e.Key == DoomKey.Enter ||
                        e.Key == DoomKey.Space ||
                        e.Key == DoomKey.LControl ||
                        e.Key == DoomKey.RControl ||
                        e.Key == DoomKey.Escape)
                    {
                        SetCurrent(main);
                        Open();
                        StartSound(Sfx.SWTCHN);
                        return true;
                    }
                }

                return false;
            }
        }

        public void Update()
        {
            tics++;

            if (current != null)
            {
                current.Update();
            }

            if (active && !doom.Options.NetGame)
            {
                doom.PauseGame();
            }
        }

        public void SetCurrent(MenuDef next)
        {
            current = next;
            current.Open();
        }

        public void Open()
        {
            active = true;
        }

        public void Close()
        {
            active = false;

            if (!doom.Options.NetGame)
            {
                doom.ResumeGame();
            }
        }

        public void StartSound(Sfx sfx)
        {
            doom.Options.Sound.StartSound(sfx);
        }

        public void NotifySaveFailed()
        {
            SetCurrent(saveFailed);
        }

        public void ShowHelpScreen()
        {
            SetCurrent(help);
            Open();
            StartSound(Sfx.SWTCHN);
        }

        public void ShowSaveScreen()
        {
            SetCurrent(save);
            Open();
            StartSound(Sfx.SWTCHN);
        }

        public void ShowLoadScreen()
        {
            SetCurrent(load);
            Open();
            StartSound(Sfx.SWTCHN);
        }

        public void ShowVolumeControl()
        {
            SetCurrent(volume);
            Open();
            StartSound(Sfx.SWTCHN);
        }

        public void QuickSave()
        {
            if (save.LastSaveSlot == -1)
            {
                ShowSaveScreen();
            }
            else
            {
                var desc = saveSlots[save.LastSaveSlot];
                var confirm = new YesNoConfirm(
                    this,
                    ((string)DoomInfo.Strings.QSPROMPT).Replace("%s", desc),
                    () => save.DoSave(save.LastSaveSlot));
                SetCurrent(confirm);
                Open();
                StartSound(Sfx.SWTCHN);
            }
        }

        public void QuickLoad()
        {
            if (save.LastSaveSlot == -1)
            {
                var pak = new PressAnyKey(
                    this,
                    DoomInfo.Strings.QSAVESPOT,
                    null);
                SetCurrent(pak);
                Open();
                StartSound(Sfx.SWTCHN);
            }
            else
            {
                var desc = saveSlots[save.LastSaveSlot];
                var confirm = new YesNoConfirm(
                    this,
                    ((string)DoomInfo.Strings.QLPROMPT).Replace("%s", desc),
                    () => load.DoLoad(save.LastSaveSlot));
                SetCurrent(confirm);
                Open();
                StartSound(Sfx.SWTCHN);
            }
        }

        public void EndGame()
        {
            SetCurrent(endGameConfirm);
            Open();
            StartSound(Sfx.SWTCHN);
        }

        public void Quit()
        {
            SetCurrent(quitConfirm);
            Open();
            StartSound(Sfx.SWTCHN);
        }

        public Doom Doom => doom;
        public GameOptions Options => doom.Options;
        public MenuDef Current => current;
        public bool Active => active;
        public int Tics => tics;
        public SaveSlots SaveSlots => saveSlots;
    }
}
