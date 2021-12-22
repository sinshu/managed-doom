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
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ManagedDoom
{
    public static class ConfigUtilities
    {
        private static readonly string[] iwadNames = new string[]
        {
            "DOOM2.WAD",
            "PLUTONIA.WAD",
            "TNT.WAD",
            "DOOM.WAD",
            "DOOM1.WAD",
            "FREEDOOM2.WAD",
            "FREEDOOM1.WAD"
        };

        public static string GetExeDirectory()
        {
            return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        }

        public static string GetConfigPath()
        {
            return Path.Combine(GetExeDirectory(), "managed-doom.cfg");
        }

        public static string GetDefaultIwadPath()
        {
            var exeDirectory = GetExeDirectory();
            foreach (var name in iwadNames)
            {
                var path = Path.Combine(exeDirectory, name);
                if (File.Exists(path))
                {
                    return path;
                }
            }

            var currentDirectory = Directory.GetCurrentDirectory();
            foreach (var name in iwadNames)
            {
                var path = Path.Combine(currentDirectory, name);
                if (File.Exists(path))
                {
                    return path;
                }
            }

            throw new Exception("No IWAD was found!");
        }

        public static bool IsIwad(string path)
        {
            var name = Path.GetFileName(path).ToUpper();
            return iwadNames.Contains(name);
        }

        public static string[] GetWadPaths(CommandLineArgs args)
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
    }
}
