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

namespace ManagedDoom
{
    public static class ThinkerPool
    {
        static ThinkerPool()
        {
        }

        public static Mobj RentMobj(World world)
        {
            return new Mobj(world);
        }

        public static VerticalDoor RentVlDoor(World world)
        {
            return new VerticalDoor(world);
        }

        public static Platform RentPlatform(World world)
        {
            return new Platform(world);
        }

        public static FloorMove RentFloorMove(World world)
        {
            return new FloorMove(world);
        }

        public static CeilingMove RentCeiligMove(World world)
        {
            return new CeilingMove(world);
        }

        public static FireFlicker RentFireFlicker(World world)
        {
            return new FireFlicker(world);
        }

        public static LightFlash RentLightFlash(World world)
        {
            return new LightFlash(world);
        }

        public static StrobeFlash RentStrobeFlash(World world)
        {
            return new StrobeFlash(world);
        }

        public static GlowingLight RentGlowingLight(World world)
        {
            return new GlowingLight(world);
        }

        public static void Return(Thinker thinker)
        {
        }
    }
}
