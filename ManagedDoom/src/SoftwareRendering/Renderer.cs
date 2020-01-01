using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace ManagedDoom.SoftwareRendering
{
    public sealed class Renderer : IDisposable
    {
        private RenderWindow sfmlWindow;
        private Palette palette;

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
            bool highResolution)
        {
            try
            {
                this.sfmlWindow = window;
                this.palette = palette;

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

                threeD = new ThreeDRenderer(colorMap, textures, flats, screenWidth, screenHeight, screenData);
            }
            catch (Exception e)
            {
                Dispose();
                ExceptionDispatchInfo.Capture(e).Throw();
            }
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

            for (var i = 0; i < screenData.Length; i++)
            {
                var po = 3 * screenData[i];
                var offset = 4 * i;
                sfmlTextureData[offset + 0] = palette.Data[po + 0];
                sfmlTextureData[offset + 1] = palette.Data[po + 1];
                sfmlTextureData[offset + 2] = palette.Data[po + 2];
                sfmlTextureData[offset + 3] = 255;
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
