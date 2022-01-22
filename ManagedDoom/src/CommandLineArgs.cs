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
using System.IO;
using System.Linq;

namespace ManagedDoom
{
    public sealed class CommandLineArgs
    {
        public readonly Arg<string> iwad;
        public readonly Arg<string[]> file;
        public readonly Arg<string[]> deh;

        public readonly Arg<Tuple<int, int>> warp;
        public readonly Arg<int> episode;
        public readonly Arg<int> skill;

        public readonly Arg deathmatch;
        public readonly Arg altdeath;
        public readonly Arg fast;
        public readonly Arg respawn;
        public readonly Arg nomonsters;
        public readonly Arg solonet;

        public readonly Arg<string> playdemo;
        public readonly Arg<string> timedemo;

        public readonly Arg<int> loadgame;

        public readonly Arg nomouse;
        public readonly Arg nosound;
        public readonly Arg nosfx;
        public readonly Arg nomusic;

        public readonly Arg nodeh;

        public CommandLineArgs(string[] args)
        {
            iwad = GetString(args, "-iwad");
            file = Check_file(args);
            deh = Check_deh(args);

            warp = Check_warp(args);
            episode = GetInt(args, "-episode");
            skill = GetInt(args, "-skill");

            deathmatch = new Arg(args.Contains("-deathmatch"));
            altdeath = new Arg(args.Contains("-altdeath"));
            fast = new Arg(args.Contains("-fast"));
            respawn = new Arg(args.Contains("-respawn"));
            nomonsters = new Arg(args.Contains("-nomonsters"));
            solonet = new Arg(args.Contains("-solo-net"));

            playdemo = GetString(args, "-playdemo");
            timedemo = GetString(args, "-timedemo");

            loadgame = GetInt(args, "-loadgame");

            nomouse = new Arg(args.Contains("-nomouse"));
            nosound = new Arg(args.Contains("-nosound"));
            nosfx = new Arg(args.Contains("-nosfx"));
            nomusic = new Arg(args.Contains("-nomusic"));

            nodeh = new Arg(args.Contains("-nodeh"));

            // Check for drag & drop.
            if (args.Length > 0 && args.All(arg => arg.FirstOrDefault() != '-'))
            {
                string iwadPath = null;
                var pwadPaths = new List<string>();
                var dehPaths = new List<string>();

                foreach (var path in args)
                {
                    var extension = Path.GetExtension(path).ToLower();

                    if (extension == ".wad")
                    {
                        if (ConfigUtilities.IsIwad(path))
                        {
                            iwadPath = path;
                        }
                        else
                        {
                            pwadPaths.Add(path);
                        }
                    }
                    else if (extension == ".deh")
                    {
                        dehPaths.Add(path);
                    }
                }

                if (iwadPath != null)
                {
                    iwad = new Arg<string>(iwadPath);
                }

                if (pwadPaths.Count > 0)
                {
                    file = new Arg<string[]>(pwadPaths.ToArray());
                }

                if (dehPaths.Count > 0)
                {
                    deh = new Arg<string[]>(dehPaths.ToArray());
                }
            }
        }

        private static Arg<string[]> Check_file(string[] args)
        {
            var values = GetValues(args, "-file");
            if (values.Length >= 1)
            {
                return new Arg<string[]>(values);
            }

            return new Arg<string[]>();
        }

        private static Arg<string[]> Check_deh(string[] args)
        {
            var values = GetValues(args, "-deh");
            if (values.Length >= 1)
            {
                return new Arg<string[]>(values);
            }

            return new Arg<string[]>();
        }

        private static Arg<Tuple<int, int>> Check_warp(string[] args)
        {
            var values = GetValues(args, "-warp");
            if (values.Length == 1)
            {
                int map;
                if (int.TryParse(values[0], out map))
                {
                    return new Arg<Tuple<int, int>>(Tuple.Create(1, map));
                }
            }
            else if (values.Length == 2)
            {
                int episode;
                int map;
                if (int.TryParse(values[0], out episode) && int.TryParse(values[1], out map))
                {
                    return new Arg<Tuple<int, int>>(Tuple.Create(episode, map));
                }
            }

            return new Arg<Tuple<int, int>>();
        }

        private static Arg<string> GetString(string[] args, string name)
        {
            var values = GetValues(args, name);
            if (values.Length == 1)
            {
                return new Arg<string>(values[0]);
            }

            return new Arg<string>();
        }

        private static Arg<int> GetInt(string[] args, string name)
        {
            var values = GetValues(args, name);
            if (values.Length == 1)
            {
                int result;
                if (int.TryParse(values[0], out result))
                {
                    return new Arg<int>(result);
                }
            }

            return new Arg<int>();
        }

        private static string[] GetValues(string[] args, string name)
        {
            return args
                .SkipWhile(arg => arg != name)
                .Skip(1)
                .TakeWhile(arg => arg[0] != '-')
                .ToArray();
        }



        public class Arg
        {
            private bool present;

            public Arg()
            {
                this.present = false;
            }

            public Arg(bool present)
            {
                this.present = present;
            }

            public bool Present => present;
        }

        public class Arg<T>
        {
            private bool present;
            private T value;

            public Arg()
            {
                this.present = false;
                this.value = default;
            }

            public Arg(T value)
            {
                this.present = true;
                this.value = value;
            }

            public bool Present => present;
            public T Value => value;
        }
    }
}
