﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace ManagedDoom.SoftwareRendering
{
    public sealed class Renderer : IDisposable
    {
        private RenderWindow sfmlWindow;
        private Palette palette;

        private uint[] colors;

        private int sfmlWindowWidth;
        private int sfmlWindowHeight;

        private int screenWidth;
        private int screenHeight;
        private int sfmlTextureWidth;
        private int sfmlTextureHeight;

        private byte[] screenData;
        private byte[] sfmlTextureData;
        private SFML.Graphics.Texture sfmlTexture;
        private SFML.Graphics.Sprite sfmlSprite;

        private ThreeDRenderer threeD;

        public Renderer(
            RenderWindow window,
            Palette palette,
            ColorMap colorMap,
            TextureLookup textures,
            FlatLookup flats,
            SpriteLookup sprites,
            bool highResolution)
        {
            try
            {
                this.sfmlWindow = window;
                this.palette = palette;

                colors = InitColors(palette);

                sfmlWindowWidth = (int)window.Size.X;
                sfmlWindowHeight = (int)window.Size.Y;

                if (highResolution)
                {
                    screenWidth = 640;
                    screenHeight = 400;
                    sfmlTextureWidth = 512;
                    sfmlTextureHeight = 1024;
                }
                else
                {
                    screenWidth = 320;
                    screenHeight = 200;
                    sfmlTextureWidth = 256;
                    sfmlTextureHeight = 512;
                }

                screenData = new byte[screenWidth * screenHeight];
                sfmlTextureData = new byte[4 * screenWidth * screenHeight];
                sfmlTexture = new SFML.Graphics.Texture((uint)sfmlTextureWidth, (uint)sfmlTextureHeight);
                sfmlSprite = new SFML.Graphics.Sprite(sfmlTexture);

                sfmlSprite.Position = new Vector2f(0, 0);
                sfmlSprite.Rotation = 90;
                var scaleX = (float)sfmlWindowWidth / screenWidth;
                var scaleY = (float)sfmlWindowHeight / screenHeight;
                sfmlSprite.Scale = new Vector2f(scaleY, -scaleX);

                threeD = new ThreeDRenderer(
                    colorMap,
                    textures,
                    flats,
                    sprites,
                    screenWidth, screenHeight, screenData);
            }
            catch (Exception e)
            {
                Dispose();
                ExceptionDispatchInfo.Capture(e).Throw();
            }
        }

        private static uint[] InitColors(Palette palette)
        {
            var colors = new uint[256];
            for (var i = 0; i < 256; i++)
            {
                var offset = 3 * i;
                var r = palette.Data[offset + 0];
                var g = palette.Data[offset + 1];
                var b = palette.Data[offset + 2];
                var a = 255;
                var color = new SFML.Graphics.Color(r, g, b);
                colors[i] = (uint)((r << 0) | (g << 8) | (b << 16) | (a << 24));
            }
            return colors;
        }

        public void BindWorld(World world)
        {
            threeD.BindWorld(world);
        }

        public void UnbindWorld()
        {
            threeD.UnbindWorld();
        }

        public void Render()
        {
            threeD.Render();

            var p = MemoryMarshal.Cast<byte, uint>(sfmlTextureData);
            for (var i = 0; i < p.Length; i++)
            {
                p[i] = colors[screenData[i]];
            }

            sfmlTexture.Update(sfmlTextureData, (uint)screenHeight, (uint)screenWidth, 0, 0);

            sfmlWindow.Draw(sfmlSprite);

            sfmlWindow.Display();
        }

        public void Dispose()
        {
            if (sfmlTexture != null)
            {
                sfmlTexture.Dispose();
                sfmlTexture = null;
            }
        }
    }
}
