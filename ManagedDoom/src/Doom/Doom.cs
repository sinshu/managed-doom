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
using ManagedDoom.Video;
using ManagedDoom.UserInput;

namespace ManagedDoom
{
    public class Doom
    {
        private CommandLineArgs args;
        private Config config;
        private GameContent content;
        private IVideo video;
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

        private WipeEffect wipeEffect;
        private bool wiping;

        private DoomState currentState;
        private DoomState nextState;
        private bool needWipe;

        private bool sendPause;

        private bool quit;
        private string quitMessage;

        private bool mouseGrabbed;

        public Doom(CommandLineArgs args, Config config, GameContent content, IVideo video, ISound sound, IMusic music, IUserInput userInput)
        {
            video = video ?? NullVideo.GetInstance();
            sound = sound ?? NullSound.GetInstance();
            music = music ?? NullMusic.GetInstance();
            userInput = userInput ?? NullUserInput.GetInstance();

            this.args = args;
            this.config = config;
            this.content = content;
            this.video = video;
            this.sound = sound;
            this.music = music;
            this.userInput = userInput;

            events = new List<DoomEvent>();

            options = new GameOptions(args, content);
            options.Video = video;
            options.Sound = sound;
            options.Music = music;
            options.UserInput = userInput;

            menu = new DoomMenu(this);

            opening = new OpeningSequence(content, options);

            cmds = new TicCmd[Player.MaxPlayerCount];
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                cmds[i] = new TicCmd();
            }
            game = new DoomGame(content, options);

            wipeEffect = new WipeEffect(video.WipeBandCount, video.WipeHeight);
            wiping = false;

            currentState = DoomState.None;
            nextState = DoomState.Opening;
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
                nextState = DoomState.Game;
                options.Episode = args.warp.Value.Item1;
                options.Map = args.warp.Value.Item2;
                game.DeferedInitNew();
            }
            else if (args.episode.Present)
            {
                nextState = DoomState.Game;
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
                nextState = DoomState.Game;
                game.LoadGame(args.loadgame.Value);
            }

            if (args.playdemo.Present)
            {
                nextState = DoomState.DemoPlayback;
                demoPlayback = new DemoPlayback(args, content, options, args.playdemo.Value);
            }

            if (args.timedemo.Present)
            {
                nextState = DoomState.DemoPlayback;
                demoPlayback = new DemoPlayback(args, content, options, args.timedemo.Value);
            }
        }

        public void NewGame(GameSkill skill, int episode, int map)
        {
            game.DeferedInitNew(skill, episode, map);
            nextState = DoomState.Game;
        }

        public void EndGame()
        {
            nextState = DoomState.Opening;
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

                if (currentState == DoomState.Game)
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
                else if (currentState == DoomState.DemoPlayback)
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
                    if (currentState == DoomState.Game)
                    {
                        menu.EndGame();
                    }
                    else
                    {
                        options.Sound.StartSound(Sfx.OOF);
                    }
                    return true;

                case DoomKey.F8:
                    video.DisplayMessage = !video.DisplayMessage;
                    if (currentState == DoomState.Game && game.State == GameState.Level)
                    {
                        string msg;
                        if (video.DisplayMessage)
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
                    var gcl = video.GammaCorrectionLevel;
                    gcl++;
                    if (gcl > video.MaxGammaCorrectionLevel)
                    {
                        gcl = 0;
                    }
                    video.GammaCorrectionLevel = gcl;
                    if (currentState == DoomState.Game && game.State == GameState.Level)
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
                    if (currentState == DoomState.Game &&
                        game.State == GameState.Level &&
                        game.World.AutoMap.Visible)
                    {
                        return false;
                    }
                    else
                    {
                        video.WindowSize = Math.Min(video.WindowSize + 1, video.MaxWindowSize);
                        sound.StartSound(Sfx.STNMOV);
                        return true;
                    }

                case DoomKey.Subtract:
                case DoomKey.Hyphen:
                case DoomKey.Semicolon:
                    if (currentState == DoomState.Game &&
                        game.State == GameState.Level &&
                        game.World.AutoMap.Visible)
                    {
                        return false;
                    }
                    else
                    {
                        video.WindowSize = Math.Max(video.WindowSize - 1, 0);
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
                    if (nextState != DoomState.Opening)
                    {
                        opening.Reset();
                    }

                    if (nextState != DoomState.DemoPlayback)
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
                    case DoomState.Opening:
                        if (opening.Update() == UpdateResult.NeedWipe)
                        {
                            StartWipe();
                        }
                        break;

                    case DoomState.DemoPlayback:
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

                    case DoomState.Game:
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
                if (wipeEffect.Update() == UpdateResult.Completed)
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
            if (!video.HasFocus())
            {
                mouseShouldBeGrabbed = false;
            }
            else if (config.video_fullscreen)
            {
                mouseShouldBeGrabbed = true;
            }
            else
            {
                mouseShouldBeGrabbed = currentState == DoomState.Game && !menu.Active;
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
            wipeEffect.Start();
            video.InitializeWipe();
            wiping = true;
        }

        public void PauseGame()
        {
            if (currentState == DoomState.Game &&
                game.State == GameState.Level &&
                !game.Paused && !sendPause)
            {
                sendPause = true;
            }
        }

        public void ResumeGame()
        {
            if (currentState == DoomState.Game &&
                game.State == GameState.Level &&
                game.Paused && !sendPause)
            {
                sendPause = true;
            }
        }

        public bool SaveGame(int slotNumber, string description)
        {
            if (currentState == DoomState.Game && game.State == GameState.Level)
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
            nextState = DoomState.Game;
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

        public DoomState State => currentState;
        public OpeningSequence Opening => opening;
        public DemoPlayback DemoPlayback => demoPlayback;
        public GameOptions Options => options;
        public DoomGame Game => game;
        public DoomMenu Menu => menu;
        public WipeEffect WipeEffect => wipeEffect;
        public bool Wiping => wiping;
        public string QuitMessage => quitMessage;
    }
}
