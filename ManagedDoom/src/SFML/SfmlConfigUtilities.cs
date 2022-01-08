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
using System.IO;
using SFML.Window;

namespace ManagedDoom.SFML
{
    public static class SfmlConfigUtilities
    {
        public static Config GetConfig()
        {
            var config = new Config(ConfigUtilities.GetConfigPath());

            if (!config.IsRestoredFromFile)
            {
                var vm = GetDefaultVideoMode();
                config.video_screenwidth = (int)vm.Width;
                config.video_screenheight = (int)vm.Height;
            }

            return config;
        }

        public static VideoMode GetDefaultVideoMode()
        {
            var desktop = VideoMode.DesktopMode;

            var baseWidth = 640;
            var baseHeight = 400;

            var currentWidth = baseWidth;
            var currentHeight = baseHeight;

            while (true)
            {
                var nextWidth = currentWidth + baseWidth;
                var nextHeight = currentHeight + baseHeight;

                if (nextWidth >= 0.9 * desktop.Width ||
                    nextHeight >= 0.9 * desktop.Height)
                {
                    break;
                }

                currentWidth = nextWidth;
                currentHeight = nextHeight;
            }

            return new VideoMode((uint)currentWidth, (uint)currentHeight);
        }

        public static SfmlMusic GetMusicInstance(Config config, Wad wad)
        {
            var sfPath = Path.Combine(ConfigUtilities.GetExeDirectory(), config.audio_soundfont);
            if (File.Exists(sfPath))
            {
                return new SfmlMusic(config, wad, sfPath);
            }
            else
            {
                Console.WriteLine("SoundFont '" + config.audio_soundfont + "' was not found!");
                return null;
            }
        }
    }
}
