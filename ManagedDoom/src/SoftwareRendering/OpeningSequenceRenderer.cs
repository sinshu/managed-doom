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



﻿using System;

namespace ManagedDoom.SoftwareRendering
{
    public class OpeningSequenceRenderer
    {
        private Patch titlePic;
        private DrawScreen screen;
        private SfmlRenderer parent;

        public OpeningSequenceRenderer(Wad wad, DrawScreen screen, SfmlRenderer parent)
        {
            titlePic = Patch.FromWad("TITLEPIC", wad);
            this.screen = screen;
            this.parent = parent;
        }

        public void Render(OpeningSequence sequence)
        {
            var scale = screen.Width / 320;
            screen.DrawPatch(titlePic, 0, 0, scale);
        }
    }
}
