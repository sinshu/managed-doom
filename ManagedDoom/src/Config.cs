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

namespace ManagedDoom
{
    public sealed class Config
    {
        public KeyBinding key_forward;
        public KeyBinding key_backward;
        public KeyBinding key_strafeleft;
        public KeyBinding key_straferight;
        public KeyBinding key_turnleft;
        public KeyBinding key_turnright;
        public KeyBinding key_fire;
        public KeyBinding key_use;
        public KeyBinding key_run;
        public KeyBinding key_strafe;
        public KeyBinding key_mousestrafe;

        // Default settings.
        public Config()
        {
            key_forward = new KeyBinding(
                new DoomKey[]
                {
                    DoomKey.Up,
                    DoomKey.W
                });

            key_backward = new KeyBinding(
                new DoomKey[]
                {
                    DoomKey.Down,
                    DoomKey.S
                });

            key_strafeleft = new KeyBinding(
                new DoomKey[]
                {
                    DoomKey.A
                });

            key_straferight = new KeyBinding(
                new DoomKey[]
                {
                    DoomKey.D
                });

            key_turnleft = new KeyBinding(
                new DoomKey[]
                {
                    DoomKey.Left
                });

            key_turnright = new KeyBinding(
                new DoomKey[]
                {
                    DoomKey.Right
                });

            key_fire = new KeyBinding(
                new DoomKey[]
                {
                    DoomKey.LControl,
                    DoomKey.RControl
                },
                new DoomMouseButton[]
                {
                    DoomMouseButton.Mouse1
                });

            key_use = new KeyBinding(
                new DoomKey[]
                {
                    DoomKey.Space
                },
                new DoomMouseButton[]
                {
                    DoomMouseButton.Mouse2
                });

            key_run = new KeyBinding(
                new DoomKey[]
                {
                    DoomKey.LShift,
                    DoomKey.RShift
                });

            key_strafe = new KeyBinding(
                new DoomKey[]
                {
                    DoomKey.LAlt,
                    DoomKey.RAlt
                });

            key_mousestrafe = KeyBinding.Empty;
        }

        public void Save(string path)
        {
            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(nameof(key_forward) + " = " + key_forward);
                writer.WriteLine(nameof(key_strafeleft) + " = " + key_strafeleft);
                writer.WriteLine(nameof(key_straferight) + " = " + key_straferight);
                writer.WriteLine(nameof(key_turnleft) + " = " + key_turnleft);
                writer.WriteLine(nameof(key_turnright) + " = " + key_turnright);
                writer.WriteLine(nameof(key_fire) + " = " + key_fire);
                writer.WriteLine(nameof(key_use) + " = " + key_use);
                writer.WriteLine(nameof(key_run) + " = " + key_run);
                writer.WriteLine(nameof(key_strafe) + " = " + key_strafe);
                writer.WriteLine(nameof(key_mousestrafe) + " = " + key_mousestrafe);
            }
        }
    }
}
