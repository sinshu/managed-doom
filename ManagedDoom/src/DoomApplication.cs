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
using ManagedDoom.UserInput;

namespace ManagedDoom
{
    public sealed class DoomApplication : IDisposable
    {
        private Config config;

        private RenderWindow window;

        private GameData resource;

        private SfmlRenderer renderer;
        private SfmlSound sound;
        private SfmlMusic music;
        private SfmlUserInput userInput;

        private Doom doom;

        public DoomApplication(CommandLineArgs args)
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
                    style = Styles.Fullscreen;
                }
                window = new RenderWindow(videoMode, ApplicationInfo.Title, style);
                window.Clear(new Color(64, 64, 64));
                window.Display();

                resource = new GameData(GetWadPaths(args), !args.nodeh.Present);

                renderer = new SfmlRenderer(config, window, resource);

                if (!args.nosound.Present && !args.nosfx.Present)
                {
                    sound = new SfmlSound(config, resource.Wad);
                }

                if (!args.nosound.Present && !args.nomusic.Present)
                {
                    music = ConfigUtilities.GetSfmlMusicInstance(config, resource.Wad);
                }

                userInput = new SfmlUserInput(config, window, !args.nomouse.Present);

                window.Closed += (sender, e) => window.Close();
                window.KeyPressed += KeyPressed;
                window.KeyReleased += KeyReleased;

                if (!args.timedemo.Present)
                {
                    window.SetFramerateLimit(35);
                }

                doom = new Doom(args, config, resource, renderer, sound, music, userInput);
            }
            catch (Exception e)
            {
                Dispose();
                ExceptionDispatchInfo.Throw(e);
            }
        }

        private string[] GetWadPaths(CommandLineArgs args)
        {
            var wadPaths = new List<string>();

            if (args.iwad.Present)
            {
                wadPaths.Add(args.iwad.Value);
            }
            else
            {
                wadPaths.Add(ConfigUtilities.GetDefaultIwadPath());
            }

            if (args.file.Present)
            {
                foreach (var path in args.file.Value)
                {
                    wadPaths.Add(path);
                }
            }

            return wadPaths.ToArray();
        }

        public void Run()
        {
            while (window.IsOpen)
            {
                window.DispatchEvents();
                if (doom.Update() == UpdateResult.Completed)
                {
                    break;
                }

                renderer.Render(doom);
            }

            config.Save(ConfigUtilities.GetConfigPath());
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            doom.PostEvent(new DoomEvent(EventType.KeyDown, (DoomKey)e.Code));
        }

        private void KeyReleased(object sender, KeyEventArgs e)
        {
            doom.PostEvent(new DoomEvent(EventType.KeyUp, (DoomKey)e.Code));
        }

        public void Dispose()
        {
            if (userInput != null)
            {
                userInput.Dispose();
                userInput = null;
            }

            if (music != null)
            {
                music.Dispose();
                music = null;
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

        public string QuitMessage => doom.QuitMessage;
    }
}
