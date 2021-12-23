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
using System.Runtime.ExceptionServices;
using SFML.Graphics;
using SFML.System;
using ManagedDoom.Video;

namespace ManagedDoom.SFML
{
    public sealed class SfmlVideo : IVideo, IDisposable
    {
        private Renderer renderer;

        private RenderWindow window;

        private int windowWidth;
        private int windowHeight;

        private int textureWidth;
        private int textureHeight;

        private byte[] textureData;
        private global::SFML.Graphics.Texture texture;
        private global::SFML.Graphics.Sprite sprite;
        private global::SFML.Graphics.RenderStates renderStates;

        public SfmlVideo(Config config, GameContent content, RenderWindow window)
        {
            try
            {
                Console.Write("Initialize video: ");

                renderer = new Renderer(config, content);

                config.video_gamescreensize = Math.Clamp(config.video_gamescreensize, 0, MaxWindowSize);
                config.video_gammacorrection = Math.Clamp(config.video_gammacorrection, 0, MaxGammaCorrectionLevel);

                this.window = window;

                windowWidth = (int)window.Size.X;
                windowHeight = (int)window.Size.Y;

                if (config.video_highresolution)
                {
                    textureWidth = 512;
                    textureHeight = 1024;
                }
                else
                {
                    textureWidth = 256;
                    textureHeight = 512;
                }

                textureData = new byte[4 * renderer.Width * renderer.Height];

                texture = new global::SFML.Graphics.Texture((uint)textureWidth, (uint)textureHeight);
                sprite = new global::SFML.Graphics.Sprite(texture);

                sprite.Position = new Vector2f(0, 0);
                sprite.Rotation = 90;
                var scaleX = (float)windowWidth / renderer.Width;
                var scaleY = (float)windowHeight / renderer.Height;
                sprite.Scale = new Vector2f(scaleY, -scaleX);
                sprite.TextureRect = new IntRect(0, 0, renderer.Height, renderer.Width);

                renderStates = new RenderStates(BlendMode.None);

                Console.WriteLine("OK");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed");
                Dispose();
                ExceptionDispatchInfo.Throw(e);
            }
        }

        public void Render(Doom doom)
        {
            renderer.Render(doom, textureData);

            texture.Update(textureData, (uint)renderer.Height, (uint)renderer.Width, 0, 0);
            window.Draw(sprite, renderStates);
            window.Display();
        }

        public void InitializeWipe()
        {
            renderer.InitializeWipe();
        }

        public bool HasFocus()
        {
            return window.HasFocus();
        }

        public void Dispose()
        {
            Console.WriteLine("Shutdown renderer.");

            if (sprite != null)
            {
                sprite.Dispose();
                sprite = null;
            }

            if (texture != null)
            {
                texture.Dispose();
                texture = null;
            }
        }

        public int WipeBandCount => renderer.WipeBandCount;
        public int WipeHeight => renderer.WipeHeight;

        public int MaxWindowSize => renderer.MaxWindowSize;

        public int WindowSize
        {
            get => renderer.WindowSize;
            set => renderer.WindowSize = value;
        }

        public bool DisplayMessage
        {
            get => renderer.DisplayMessage;
            set => renderer.DisplayMessage = value;
        }

        public int MaxGammaCorrectionLevel => renderer.MaxGammaCorrectionLevel;

        public int GammaCorrectionLevel
        {
            get => renderer.GammaCorrectionLevel;
            set => renderer.GammaCorrectionLevel = value;
        }
    }
}
