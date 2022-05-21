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
using Silk.NET.Maths;
using Silk.NET.Windowing;
using DrippyAL;

namespace ManagedDoom.Silk
{
    public static class SilkConfigUtilities
    {
        public static Config GetConfig()
        {
            var config = new Config(ConfigUtilities.GetConfigPath());

            if (!config.IsRestoredFromFile)
            {
                var vm = GetDefaultVideoMode();
                config.video_screenwidth = vm.Resolution.Value.X;
                config.video_screenheight = vm.Resolution.Value.Y;
            }

            return config;
        }

        public static VideoMode GetDefaultVideoMode()
        {
            var monitor = Monitor.GetMainMonitor(null);

            var baseWidth = 640;
            var baseHeight = 400;

            var currentWidth = baseWidth;
            var currentHeight = baseHeight;

            while (true)
            {
                var nextWidth = currentWidth + baseWidth;
                var nextHeight = currentHeight + baseHeight;

                if (nextWidth >= 0.9 * monitor.VideoMode.Resolution.Value.X ||
                    nextHeight >= 0.9 * monitor.VideoMode.Resolution.Value.Y)
                {
                    break;
                }

                currentWidth = nextWidth;
                currentHeight = nextHeight;
            }

            return new VideoMode(new Vector2D<int>(currentWidth, currentHeight));
        }

        public static SilkMusic GetMusicInstance(Config config, GameContent content, AudioDevice device)
        {
            var sfPath = Path.Combine(ConfigUtilities.GetExeDirectory(), config.audio_soundfont);
            if (File.Exists(sfPath))
            {
                return new SilkMusic(config, content, device, sfPath);
            }
            else
            {
                Console.WriteLine("SoundFont '" + config.audio_soundfont + "' was not found!");
                return null;
            }
        }
    }
}
