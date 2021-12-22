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
using ManagedDoom.Audio;
using ManagedDoom.SoftwareRendering;
using ManagedDoom.UserInput;

namespace ManagedDoom
{
    public class Doom
    {
        private CommandLineArgs args;
        private Config config;
        private GameData data;
        private IRenderer renderer;
        private ISound sound;
        private IMusic music;
        private IUserInput userInput;

        private List<DoomEvent> events;

        private GameOptions options;

        private DoomMenu menu;

        private OpeningSequence opening;

        private DemoPlayback demoPlayback;

        private TicCmd[] cmds;
        private DoomGame game;

        private WipeEffect wipe;
        private bool wiping;

        private ApplicationState currentState;
        private ApplicationState nextState;
        private bool needWipe;

        private bool sendPause;

        private bool quit;
        private string quitMessage;

        private bool mouseGrabbed;

        public Doom(CommandLineArgs args, Config config, GameData data, IRenderer renderer, ISound sound, IMusic music, IUserInput userInput)
        {
            this.args = args;
            this.config = config;
            this.data = data;
            this.renderer = renderer;
            this.sound = sound;
            this.music = music;
            this.userInput = userInput;

            if (args.deh.Present)
            {
                DeHackEd.ReadFiles(args.deh.Value);
            }

            events = new List<DoomEvent>();

            options = new GameOptions();
            options.GameVersion = data.Wad.GameVersion;
            options.GameMode = data.Wad.GameMode;
            options.MissionPack = data.Wad.MissionPack;
            options.Renderer = renderer;
            options.Sound = sound;
            options.Music = music;
            options.UserInput = userInput;

            menu = new DoomMenu(this);

            opening = new OpeningSequence(data, options);

            cmds = new TicCmd[Player.MaxPlayerCount];
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                cmds[i] = new TicCmd();
            }
            game = new DoomGame(data, options);

            wipe = new WipeEffect(renderer.WipeBandCount, renderer.WipeHeight);
            wiping = false;

            currentState = ApplicationState.None;
            nextState = ApplicationState.Opening;
            needWipe = false;

            sendPause = false;

            quit = false;
            quitMessage = null;

            mouseGrabbed = false;

            CheckGameArgs();
        }

        private void CheckGameArgs()
        {
            if (args.warp.Present)
            {
                nextState = ApplicationState.Game;
                options.Episode = args.warp.Value.Item1;
                options.Map = args.warp.Value.Item2;
                game.DeferedInitNew();
            }
            else if (args.episode.Present)
            {
                nextState = ApplicationState.Game;
                options.Episode = args.episode.Value;
                options.Map = 1;
                game.DeferedInitNew();
            }

            if (args.skill.Present)
            {
                options.Skill = (GameSkill)(args.skill.Value - 1);
            }

            if (args.deathmatch.Present)
            {
                options.Deathmatch = 1;
            }

            if (args.altdeath.Present)
            {
                options.Deathmatch = 2;
            }

            if (args.fast.Present)
            {
                options.FastMonsters = true;
            }

            if (args.respawn.Present)
            {
                options.RespawnMonsters = true;
            }

            if (args.nomonsters.Present)
            {
                options.NoMonsters = true;
            }

            if (args.loadgame.Present)
            {
                nextState = ApplicationState.Game;
                game.LoadGame(args.loadgame.Value);
            }

            if (args.playdemo.Present)
            {
                nextState = ApplicationState.DemoPlayback;
                demoPlayback = new DemoPlayback(data, options, args.playdemo.Value);
            }

            if (args.timedemo.Present)
            {
                nextState = ApplicationState.DemoPlayback;
                demoPlayback = new DemoPlayback(data, options, args.timedemo.Value);
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
                else if (currentState == ApplicationState.DemoPlayback)
                {
                    demoPlayback.DoEvent(e);
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

                case DoomKey.Add:
                case DoomKey.Quote:
                case DoomKey.Equal:
                    if (currentState == ApplicationState.Game &&
                        game.State == GameState.Level &&
                        game.World.AutoMap.Visible)
                    {
                        return false;
                    }
                    else
                    {
                        renderer.WindowSize = Math.Min(renderer.WindowSize + 1, renderer.MaxWindowSize);
                        sound.StartSound(Sfx.STNMOV);
                        return true;
                    }

                case DoomKey.Subtract:
                case DoomKey.Hyphen:
                case DoomKey.Semicolon:
                    if (currentState == ApplicationState.Game &&
                        game.State == GameState.Level &&
                        game.World.AutoMap.Visible)
                    {
                        return false;
                    }
                    else
                    {
                        renderer.WindowSize = Math.Max(renderer.WindowSize - 1, 0);
                        sound.StartSound(Sfx.STNMOV);
                        return true;
                    }

                default:
                    return false;
            }
        }

        public UpdateResult Update()
        {
            DoEvents();

            if (!wiping)
            {
                menu.Update();

                if (nextState != currentState)
                {
                    if (nextState != ApplicationState.Opening)
                    {
                        opening.Reset();
                    }

                    if (nextState != ApplicationState.DemoPlayback)
                    {
                        demoPlayback = null;
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

                    case ApplicationState.DemoPlayback:
                        var result = demoPlayback.Update();
                        if (result == UpdateResult.NeedWipe)
                        {
                            StartWipe();
                        }
                        else if (result == UpdateResult.Completed)
                        {
                            Quit("FPS: " + demoPlayback.Fps.ToString("0.0"));
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
                if (wipe.Update() == UpdateResult.Completed)
                {
                    wiping = false;
                }
            }

            sound.Update();

            CheckMouseState();

            return UpdateResult.None;
        }

        private void CheckMouseState()
        {
            bool mouseShouldBeGrabbed;
            if (!renderer.HasFocus())
            {
                mouseShouldBeGrabbed = false;
            }
            else if (config.video_fullscreen)
            {
                mouseShouldBeGrabbed = true;
            }
            else
            {
                mouseShouldBeGrabbed = currentState == ApplicationState.Game && !menu.Active;
            }

            if (mouseGrabbed)
            {
                if (!mouseShouldBeGrabbed)
                {
                    userInput.ReleaseMouse();
                    mouseGrabbed = false;
                }
            }
            else
            {
                if (mouseShouldBeGrabbed)
                {
                    userInput.GrabMouse();
                    mouseGrabbed = true;
                }
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

        public void Quit(string message)
        {
            quit = true;
            quitMessage = message;
        }

        public void PostEvent(DoomEvent e)
        {
            if (events.Count < 64)
            {
                events.Add(e);
            }
        }

        public ApplicationState State => currentState;
        public OpeningSequence Opening => opening;
        public DemoPlayback DemoPlayback => demoPlayback;
        public GameOptions Options => options;
        public DoomGame Game => game;
        public DoomMenu Menu => menu;
        public string QuitMessage => quitMessage;
    }
}
