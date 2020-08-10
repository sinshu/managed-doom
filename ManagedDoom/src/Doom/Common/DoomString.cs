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

namespace ManagedDoom
{
    public sealed class DoomString
    {
        private static Dictionary<string, DoomString> table = new Dictionary<string, DoomString>();

        private string original;
        private string replaced;

        public DoomString(string original)
        {
            this.original = original;
            replaced = original;

            if (!table.ContainsKey(original))
            {
                table.Add(original, this);
            }
        }

        public override string ToString()
        {
            return replaced;
        }

        public char this[int index]
        {
            get
            {
                return replaced[index];
            }
        }

        public static implicit operator string(DoomString ds)
        {
            return ds.replaced;
        }

        public static void Replace(string original, string replaced)
        {
            DoomString ds;
            if (table.TryGetValue(original, out ds))
            {
                ds.replaced = replaced;
            }
        }
    }
}
