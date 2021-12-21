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

namespace ManagedDoom
{
    public sealed class GameData : IDisposable
    {
        private Wad wad;
        private Palette palette;
        private ColorMap colorMap;
        private ITextureLookup textures;
        private IFlatLookup flats;
        private ISpriteLookup sprites;
        private TextureAnimation animation;

        private GameData()
        {
        }

        public GameData(string[] wadPaths, bool loadDehLump)
        {
            wad = new Wad(wadPaths);

            if (loadDehLump)
            {
                DeHackEd.ReadDeHackEdLump(wad);
            }

            palette = new Palette(wad);
            colorMap = new ColorMap(wad);
            textures = new TextureLookup(wad);
            flats = new FlatLookup(wad);
            sprites = new SpriteLookup(wad);
            animation = new TextureAnimation(textures, flats);
        }

        public static GameData CreateDummy(params string[] wadPaths)
        {
            var gd = new GameData();

            gd.wad = new Wad(wadPaths);
            gd.palette = new Palette(gd.wad);
            gd.colorMap = new ColorMap(gd.wad);
            gd.textures = new TextureLookup(gd.wad);
            gd.flats = new FlatLookup(gd.wad);
            gd.sprites = new SpriteLookup(gd.wad);
            gd.animation = new TextureAnimation(gd.textures, gd.flats);

            return gd;
        }

        public void Dispose()
        {
            if (wad != null)
            {
                wad.Dispose();
                wad = null;
            }
        }

        public Wad Wad => wad;
        public Palette Palette => palette;
        public ColorMap ColorMap => colorMap;
        public ITextureLookup Textures => textures;
        public IFlatLookup Flats => flats;
        public ISpriteLookup Sprites => sprites;
        public TextureAnimation Animation => animation;
    }
}
