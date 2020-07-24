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
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using SFML.Graphics;
using SFML.Window;
using ManagedDoom.SoftwareRendering;
using ManagedDoom.Audio;

namespace ManagedDoom
{
    public sealed class DoomApplication : IDisposable
    {
        private Config config;

        private RenderWindow window;

        private CommonResource resource;
        private SfmlRenderer renderer;
        private SfmlSound sound;
        private SfmlUserInput userInput;

        private List<DoomEvent> events;

        private GameOptions options;

        private DoomMenu menu;

        private OpeningSequence opening;

        private TicCmd[] cmds;
        private DoomGame game;

        private Wipe wipe;
        private bool wiping;

        private ApplicationState currentState;
        private ApplicationState nextState;
        private bool needWipe;

        private bool sendPause;
        private bool quit;

        public DoomApplication()
        {
            config = new Config(ConfigUtilities.GetConfigPath());

            try
            {
                config.video_screenwidth = Math.Clamp(config.video_screenwidth, 320, 3200);
                config.video_screenheight = Math.Clamp(config.video_screenheight, 200, 2000);
                var videoMode = new VideoMode((uint)config.video_screenwidth, (uint)config.video_screenheight);
                var style = Styles.Close | Styles.Titlebar;
                if (config.video_fullscreen)
                {
                    style |= Styles.Fullscreen;
                }
                window = new RenderWindow(videoMode, "Managed Doom", style);
                window.Clear(new Color(128, 128, 128));
                window.Display();

                resource = new CommonResource(ConfigUtilities.GetDefaultIwadPath());
                renderer = new SfmlRenderer(config, window, resource);
                sound = new SfmlSound(config, resource.Wad);
                userInput = new SfmlUserInput(config, window);

                events = new List<DoomEvent>();

                options = new GameOptions();
                options.GameMode = resource.Wad.GameMode;
                options.MissionPack = resource.Wad.MissionPack;
                options.Renderer = renderer;
                options.Sound = sound;
                options.UserInput = userInput;

                menu = new DoomMenu(this);

                opening = new OpeningSequence(resource, options);

                cmds = new TicCmd[Player.MaxPlayerCount];
                for (var i = 0; i < Player.MaxPlayerCount; i++)
                {
                    cmds[i] = new TicCmd();
                }
                game = new DoomGame(resource, options);

                wipe = new Wipe(renderer.WipeBandCount, renderer.WipeHeight);
                wiping = false;

                window.Closed += (sender, e) => window.Close();
                window.KeyPressed += KeyPressed;
                window.KeyReleased += KeyReleased;
                window.SetFramerateLimit(35);

                currentState = ApplicationState.None;
                nextState = ApplicationState.Opening;
                needWipe = false;

                sendPause = false;
                quit = false;
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Throw(e);
            }
        }

        public void Run()
        {
            while (window.IsOpen)
            {
                window.DispatchEvents();
                DoEvents();
                if (Update() == UpdateResult.Completed)
                {
                    config.Save(ConfigUtilities.GetConfigPath());
                    return;
                }
            }
        }

        public void NewGame(GameSkill skill, int episode, int map)
        {
            game.DeferedInitNew(skill, episode, map);
            nextState = ApplicationState.Game;
        }

        public void EndGame()
        {
            nextState = ApplicationState.Opening;
            needWipe = true;
        }

        private void DoEvents()
        {
            if (wiping)
            {
                return;
            }

            foreach (var e in events)
            {
                if (menu.DoEvent(e))
                {
                    continue;
                }

                if (e.Type == EventType.KeyDown)
                {
                    if (CheckFunctionKey(e.Key))
                    {
                        continue;
                    }
                }

                if (currentState == ApplicationState.Game)
                {
                    if (e.Key == DoomKey.Pause && e.Type == EventType.KeyDown)
                    {
                        sendPause = true;
                        continue;
                    }

                    if (game.DoEvent(e))
                    {
                        continue;
                    }
                }
            }

            events.Clear();
        }

        private bool CheckFunctionKey(DoomKey key)
        {
            switch (key)
            {
                case DoomKey.F1:
                    menu.ShowHelpScreen();
                    return true;

                case DoomKey.F2:
                    menu.ShowSaveScreen();
                    return true;

                case DoomKey.F3:
                    menu.ShowLoadScreen();
                    return true;

                case DoomKey.F4:
                    menu.ShowVolumeControl();
                    return true;

                case DoomKey.F6:
                    menu.QuickSave();
                    return true;

                case DoomKey.F7:
                    if (currentState == ApplicationState.Game)
                    {
                        menu.EndGame();
                    }
                    else
                    {
                        options.Sound.StartSound(Sfx.OOF);
                    }
                    return true;

                case DoomKey.F8:
                    renderer.DisplayMessage = !renderer.DisplayMessage;
                    if (currentState == ApplicationState.Game && game.State == GameState.Level)
                    {
                        string msg;
                        if (renderer.DisplayMessage)
                        {
                            msg = DoomInfo.Strings.MSGON;
                        }
                        else
                        {
                            msg = DoomInfo.Strings.MSGOFF;
                        }
                        game.World.ConsolePlayer.SendMessage(msg);
                    }
                    menu.StartSound(Sfx.SWTCHN);
                    return true;

                case DoomKey.F9:
                    menu.QuickLoad();
                    return true;

                case DoomKey.F10:
                    menu.Quit();
                    return true;

                case DoomKey.F11:
                    var gcl = renderer.GammaCorrectionLevel;
                    gcl++;
                    if (gcl > renderer.MaxGammaCorrectionLevel)
                    {
                        gcl = 0;
                    }
                    renderer.GammaCorrectionLevel = gcl;
                    if (currentState == ApplicationState.Game && game.State == GameState.Level)
                    {
                        string msg;
                        if (gcl == 0)
                        {
                            msg = DoomInfo.Strings.GAMMALVL0;
                        }
                        else
                        {
                            msg = "Gamma correction level " + gcl;
                        }
                        game.World.ConsolePlayer.SendMessage(msg);
                    }
                    return true;

                case DoomKey.F12:
                    if (options.Deathmatch == 0 &&
                        currentState == ApplicationState.Game &&
                        game.State == GameState.Level)
                    {
                        game.World.ChangeDisplayPlayer();
                    }
                    return true;

                case DoomKey.Add:
                case DoomKey.Quote:
                    if (currentState == ApplicationState.Game &&
                        game.State == GameState.Level &&
                        game.World.AutoMap.Visible)
                    {
                        return false;
                    }
                    else
                    {
                        renderer.WindowSize = Math.Min(renderer.WindowSize + 1, renderer.MaxWindowSize);
                        options.Sound.StartSound(Sfx.STNMOV);
                        return true;
                    }

                case DoomKey.Subtract:
                case DoomKey.Hyphen:
                    if (currentState == ApplicationState.Game &&
                        game.State == GameState.Level &&
                        game.World.AutoMap.Visible)
                    {
                        return false;
                    }
                    else
                    {
                        renderer.WindowSize = Math.Max(renderer.WindowSize - 1, 0);
                        options.Sound.StartSound(Sfx.STNMOV);
                        return true;
                    }

                default:
                    return false;
            }
        }

        private UpdateResult Update()
        {
            if (!wiping)
            {
                menu.Update();

                if (nextState != currentState)
                {
                    if (nextState != ApplicationState.Opening)
                    {
                        opening.Reset();
                    }

                    currentState = nextState;
                }

                if (quit)
                {
                    return UpdateResult.Completed;
                }

                if (needWipe)
                {
                    needWipe = false;
                    StartWipe();
                }
            }

            if (!wiping)
            {
                switch (currentState)
                {
                    case ApplicationState.Opening:
                        if (opening.Update() == UpdateResult.NeedWipe)
                        {
                            StartWipe();
                        }
                        break;

                    case ApplicationState.Game:
                        userInput.BuildTicCmd(cmds[options.ConsolePlayer]);
                        if (sendPause)
                        {
                            sendPause = false;
                            cmds[options.ConsolePlayer].Buttons |= (byte)(TicCmdButtons.Special | TicCmdButtons.Pause);
                        }
                        if (game.Update(cmds) == UpdateResult.NeedWipe)
                        {
                            StartWipe();
                        }
                        break;

                    default:
                        throw new Exception("Invalid application state!");
                }
            }

            if (wiping)
            {
                var result = wipe.Update();
                renderer.RenderWipe(this, wipe);
                if (result == UpdateResult.Completed)
                {
                    wiping = false;
                }
            }
            else
            {
                renderer.Render(this);
            }

            options.Sound.Update();

            return UpdateResult.None;
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (events.Count < 64)
            {
                var de = new DoomEvent();
                de.Type = EventType.KeyDown;
                de.Key = (DoomKey)e.Code;
                events.Add(de);
            }
        }

        private void KeyReleased(object sender, KeyEventArgs e)
        {
            if (events.Count < 64)
            {
                var de = new DoomEvent();
                de.Type = EventType.KeyUp;
                de.Key = (DoomKey)e.Code;
                events.Add(de);
            }
        }

        private void StartWipe()
        {
            wipe.Start();
            renderer.InitializeWipe();
            wiping = true;
        }

        public void PauseGame()
        {
            if (currentState == ApplicationState.Game &&
                game.State == GameState.Level &&
                !game.Paused && !sendPause)
            {
                sendPause = true;
            }
        }

        public void ResumeGame()
        {
            if (currentState == ApplicationState.Game &&
                game.State == GameState.Level &&
                game.Paused && !sendPause)
            {
                sendPause = true;
            }
        }

        public bool SaveGame(int slotNumber, string description)
        {
            if (currentState == ApplicationState.Game && game.State == GameState.Level)
            {
                game.SaveGame(slotNumber, description);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void LoadGame(int slotNumber)
        {
            game.LoadGame(slotNumber);
            nextState = ApplicationState.Game;
        }

        public void Quit()
        {
            quit = true;
        }

        public void Dispose()
        {
            if (userInput != null)
            {
                userInput.Dispose();
                userInput = null;
            }

            if (sound != null)
            {
                sound.Dispose();
                sound = null;
            }

            if (renderer != null)
            {
                renderer.Dispose();
                renderer = null;
            }

            if (resource != null)
            {
                resource.Dispose();
                resource = null;
            }

            if (window != null)
            {
                window.Dispose();
                window = null;
            }
        }

        public ApplicationState State => currentState;
        public OpeningSequence Opening => opening;
        public GameOptions Options => options;
        public DoomGame Game => game;
        public DoomMenu Menu => menu;
    }
}
