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
using SFML.Window;

namespace ManagedDoom
{
    public sealed class KeyBinding
    {
        private Item forward;
        private Item backward;

        private Item strafeLeft;
        private Item strafeRight;

        private Item turnLeft;
        private Item turnRight;

        private Item fire;
        private Item use;

        private Item run;

        private Item keyStrafe;
        private Item mouseStrafe;

        public KeyBinding()
        {
            forward = new Item(
                new DoomKeys[]
                {
                    DoomKeys.Up,
                    DoomKeys.W
                });

            backward = new Item(
                new DoomKeys[]
                {
                    DoomKeys.Down,
                    DoomKeys.S
                });

            strafeLeft = new Item(
                new DoomKeys[]
                {
                    DoomKeys.A
                });

            strafeRight = new Item(
                new DoomKeys[]
                {
                    DoomKeys.D
                });

            turnLeft = new Item(
                new DoomKeys[]
                {
                    DoomKeys.Left
                });

            turnRight = new Item(
                new DoomKeys[]
                {
                    DoomKeys.Right
                });

            fire = new Item(
                new DoomKeys[]
                {
                    DoomKeys.LControl,
                    DoomKeys.RControl
                },
                new DoomMouseButtons[]
                {
                    DoomMouseButtons.Mouse1
                });

            use = new Item(
                new DoomKeys[]
                {
                    DoomKeys.Space
                },
                new DoomMouseButtons[]
                {
                    DoomMouseButtons.Mouse2
                });

            run = new Item(
                new DoomKeys[]
                {
                    DoomKeys.LShift,
                    DoomKeys.RShift
                });

            keyStrafe = new Item(
                new DoomKeys[]
                {
                    DoomKeys.LAlt,
                    DoomKeys.RAlt
                });

            mouseStrafe = new Item();
        }

        public Item Forward => forward;
        public Item Backward => backward;
        public Item StrafeLeft => strafeLeft;
        public Item StrafeRight => strafeRight;
        public Item TurnLeft => turnLeft;
        public Item TurnRight => turnRight;
        public Item Fire => fire;
        public Item Use => use;
        public Item Run => run;
        public Item KeyStrafe => keyStrafe;
        public Item MouseStrafe => mouseStrafe;



        public class Item
        {
            private DoomKeys[] keys;
            private DoomMouseButtons[] mouseButtons;

            public Item()
            {
                this.keys = Array.Empty<DoomKeys>();
                this.mouseButtons = Array.Empty<DoomMouseButtons>();
            }

            public Item(IReadOnlyList<DoomKeys> keys)
            {
                this.keys = keys.ToArray();
                this.mouseButtons = Array.Empty<DoomMouseButtons>();
            }

            public Item(IReadOnlyList<DoomKeys> keys, IReadOnlyList<DoomMouseButtons> mouseButtons)
            {
                this.keys = keys.ToArray();
                this.mouseButtons = mouseButtons.ToArray();
            }

            public bool IsPressed()
            {
                foreach (var key in keys)
                {
                    if (Keyboard.IsKeyPressed((Keyboard.Key)key))
                    {
                        return true;
                    }
                }

                foreach (var mouseButton in mouseButtons)
                {
                    if (Mouse.IsButtonPressed((Mouse.Button)mouseButton))
                    {
                        return true;
                    }
                }

                return false;
            }

            public IReadOnlyList<DoomKeys> Keys => keys;
            public IReadOnlyList<DoomMouseButtons> MouseButtons => mouseButtons;
        }
    }
}
