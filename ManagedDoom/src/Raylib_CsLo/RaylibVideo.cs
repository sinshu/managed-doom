using System;
using System.Numerics;
using System.Runtime.ExceptionServices;
using Raylib_CsLo;
using ManagedDoom.Video;

namespace ManagedDoom.Raylib_CsLo
{
    public class RaylibVideo : IVideo, IDisposable
    {
        private Renderer renderer;

        private global::Raylib_CsLo.Image textureData;
        private global::Raylib_CsLo.Texture texture;

        private Vector2[] positions;
        private Vector2[] texcoords;

        public RaylibVideo(Config config, GameContent content)
        {
            try
            {
                Console.Write("Initialize video: ");

                renderer = new Renderer(config, content);

                config.video_gamescreensize = Math.Clamp(config.video_gamescreensize, 0, MaxWindowSize);
                config.video_gammacorrection = Math.Clamp(config.video_gammacorrection, 0, MaxGammaCorrectionLevel);

                textureData = Raylib.GenImageColor(renderer.Height, renderer.Width, Raylib.RED);
                texture = Raylib.LoadTextureFromImage(textureData);

                var x = Raylib.GetScreenWidth() / 2;
                var y = Raylib.GetScreenHeight() / 2;

                positions = new Vector2[]
                {
                    new Vector2(-x, -y),
                    new Vector2(-x, +y),
                    new Vector2(+x, +y),
                    new Vector2(+x, -y),
                    new Vector2(-x, -y)
                };

                texcoords = new Vector2[]
                {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 0)
                };

                Console.WriteLine("OK");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed");
                Dispose();
                ExceptionDispatchInfo.Throw(e);
            }
        }

        public unsafe void Render(Doom doom)
        {
            renderer.Render(doom, new Span<byte>(textureData.data, 4 * textureData.width * textureData.height));
            Raylib.UpdateTexture(texture, textureData.data);

            Raylib.BeginDrawing();

            Raylib.DrawTexturePoly(
                texture,
                new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2),
                positions,
                texcoords,
                positions.Length,
                Raylib.WHITE);

            Raylib.EndDrawing();
        }

        public void InitializeWipe()
        {
            renderer.InitializeWipe();
        }

        public bool HasFocus()
        {
            return Raylib.IsWindowFocused();
        }

        public void Dispose()
        {
            Console.WriteLine("Shutdown renderer.");

            Raylib.UnloadTexture(texture);
            Raylib.UnloadImage(textureData);
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
