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
using System.Linq;

namespace ManagedDoom
{
    public sealed class CommandLineArgs
    {
        public readonly Arg<string> iwad;
        public readonly Arg<string[]> file;

        public readonly Arg<Tuple<int, int>> warp;
        public readonly Arg<int> skill;

        public CommandLineArgs(string[] args)
        {
            iwad = Check_iwad(args);
            file = Check_file(args);

            warp = Check_warp(args);
            skill = Check_skill(args);
        }

        private static Arg<string> Check_iwad(string[] args)
        {
            var values = GetValues(args, "-iwad");
            if (values.Length == 1)
            {
                return new Arg<string>(values[0]);
            }

            return new Arg<string>();
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

        private static Arg<int> Check_skill(string[] args)
        {
            var values = GetValues(args, "-skill");
            if (values.Length == 1)
            {
                int skill;
                if (int.TryParse(values[0], out skill))
                {
                    return new Arg<int>(skill);
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
