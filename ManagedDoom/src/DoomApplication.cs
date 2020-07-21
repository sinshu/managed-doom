using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using SFML.Graphics;
using SFML.Window;
using ManagedDoom.SoftwareRendering;
using ManagedDoom.Audio;
using SFML.System;

namespace ManagedDoom
{
    public sealed class DoomApplication : IDisposable
    {
        private RenderWindow window;
        private CommonResource resource;
        private SfmlRenderer renderer;
        private SfmlSound sound;

        private GameOptions options;
        private ApplicationState state;

        private DoomMenu menu;

        private OpeningSequence opening;

        private TicCmd[] cmds;
        private DoomGame game;

        private Wipe wipe;
        private bool wiping;

        private List<DoomEvent> events;

        //private Demo demo;

        private bool sendPause;

        private bool quit;

        public DoomApplication()
        {
            try
            {
                var style = Styles.Close | Styles.Titlebar;
                window = new RenderWindow(new VideoMode(640, 400), "Managed Doom", style);
                window.Clear(new Color(128, 128, 128));
                window.Display();

                resource = new CommonResource("DOOM2.WAD");
                renderer = new SfmlRenderer(window, resource, true);
                sound = new SfmlSound(resource.Wad);

                options = new GameOptions();
                options.Skill = GameSkill.Hard;
                options.GameMode = resource.Wad.GameMode;
                options.MissionPack = resource.Wad.MissionPack;
                options.Episode = 1;
                options.Map = 1;
                options.Players[0].InGame = true;
                options.Renderer = renderer;
                options.Sound = sound;

                state = ApplicationState.Opening;

                //demo = new Demo("test.lmp");
                //options = demo.Options;

                menu = new DoomMenu(this);

                opening = new OpeningSequence();

                cmds = new TicCmd[Player.MaxPlayerCount];
                for (var i = 0; i < Player.MaxPlayerCount; i++)
                {
                    cmds[i] = new TicCmd();
                }
                game = new DoomGame(resource, options);

                wipe = new Wipe(renderer.WipeBandCount, renderer.WipeHeight);
                wipe.Start();

                events = new List<DoomEvent>();

                window.Closed += (sender, e) => window.Close();
                window.KeyPressed += KeyPressed;
                window.KeyReleased += KeyReleased;
                window.SetFramerateLimit(35);

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
                if (Update())
                {
                    return;
                }
            }
        }

        public void NewGame(GameSkill skill, int episode, int map)
        {
            game.DeferedInitNew(skill, episode, map);
            state = ApplicationState.Game;
        }

        private void DoEvents()
        {
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

                if (state == ApplicationState.Game)
                {
                    if (e.Key == DoomKeys.Pause && e.Type == EventType.KeyDown)
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

        private bool CheckFunctionKey(DoomKeys key)
        {
            switch (key)
            {
                case DoomKeys.F1:
                    menu.ShowHelpScreen();
                    return true;

                case DoomKeys.F2:
                    menu.ShowSaveScreen();
                    return true;

                case DoomKeys.F3:
                    menu.ShowLoadScreen();
                    return true;

                case DoomKeys.F4:
                    menu.ShowVolumeControl();
                    return true;

                case DoomKeys.F8:
                    renderer.DisplayMessage = !renderer.DisplayMessage;
                    if (state == ApplicationState.Game && game.State == GameState.Level)
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

                case DoomKeys.F11:
                    var gcl = renderer.GammaCorrectionLevel;
                    gcl++;
                    if (gcl == renderer.MaxGammaCorrectionLevel)
                    {
                        gcl = 0;
                    }
                    renderer.GammaCorrectionLevel = gcl;
                    if (state == ApplicationState.Game && game.State == GameState.Level)
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

                default:
                    return false;
            }
        }

        private bool Update()
        {
            menu.Update();

            if (quit)
            {
                return true;
            }

            if (state == ApplicationState.Opening)
            {
                opening.Update();
            }
            else if (state == ApplicationState.Game)
            {
                UserInput.BuildTicCmd(cmds[options.ConsolePlayer]);
                if (sendPause)
                {
                    sendPause = false;
                    cmds[options.ConsolePlayer].Buttons |= (byte)(TicCmdButtons.Special | TicCmdButtons.Pause);
                }

                if (!wiping)
                {
                    //demo.ReadCmd(cmds);
                    var result = game.Update(cmds);
                    if (result == UpdateResult.NeedWipe)
                    {
                        wipe.Start();
                        renderer.InitializeWipe();
                        wiping = true;
                    }
                }
            }

            options.Sound.Update();
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

            return false;
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (events.Count < 64)
            {
                var de = new DoomEvent();
                de.Type = EventType.KeyDown;
                de.Key = (DoomKeys)e.Code;
                events.Add(de);
            }
        }

        private void KeyReleased(object sender, KeyEventArgs e)
        {
            if (events.Count < 64)
            {
                var de = new DoomEvent();
                de.Type = EventType.KeyUp;
                de.Key = (DoomKeys)e.Code;
                events.Add(de);
            }
        }

        public void PauseGame()
        {
            if (state == ApplicationState.Game &&
                game.State == GameState.Level &&
                !game.Paused && !sendPause)
            {
                sendPause = true;
            }
        }

        public void ResumeGame()
        {
            if (state == ApplicationState.Game &&
                game.State == GameState.Level &&
                game.Paused && !sendPause)
            {
                sendPause = true;
            }
        }

        public bool SaveGame(int slotNumber, string description)
        {
            if (state == ApplicationState.Game && game.State == GameState.Level)
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
            state = ApplicationState.Game;
        }

        public void Quit()
        {
            quit = true;
        }

        public void Dispose()
        {
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

        public ApplicationState State => state;
        public OpeningSequence Opening => opening;
        public GameOptions Options => options;
        public DoomGame Game => game;
        public DoomMenu Menu => menu;
        public GameMode GameMode => resource.Wad.GameMode;
    }
}
