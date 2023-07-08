using System;
using System.Numerics;
using System.Runtime.ExceptionServices;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using TrippyGL;
using ManagedDoom.Video;

namespace ManagedDoom.Silk
{
    public sealed class SilkVideo : IVideo, IDisposable
    {
        private readonly Renderer renderer;

        private GraphicsDevice device;

        private readonly int textureWidth;
        private readonly int textureHeight;

        private readonly byte[] textureData;
        private Texture2D texture;

        private TextureBatcher batcher;
        private SimpleShaderProgram shader;

        private int silkWindowWidth;
        private int silkWindowHeight;

        public SilkVideo(Config config, GameContent content, IWindow window, GL gl)
        {
            try
            {
                Console.Write("Initialize video: ");

                renderer = new Renderer(config, content);

                device = new GraphicsDevice(gl);

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
                texture = new Texture2D(device, (uint)textureWidth, (uint)textureHeight);
                texture.SetTextureFilters(TrippyGL.TextureMinFilter.Nearest, TrippyGL.TextureMagFilter.Nearest);

                batcher = new TextureBatcher(device);
                shader = SimpleShaderProgram.Create<VertexColorTexture>(device);
                batcher.SetShaderProgram(shader);

                device.BlendingEnabled = false;

                Resize(window.Size.X, window.Size.Y);

                Console.WriteLine("OK");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed");
                Dispose();
                ExceptionDispatchInfo.Throw(e);
            }
        }

        public unsafe void Render(Doom doom, Fixed frameFrac)
        {
            renderer.Render(doom, textureData, frameFrac);

            texture.SetData<byte>(textureData, 0, 0, (uint)renderer.Height, (uint)renderer.Width);

            var u = (float)renderer.Height / textureWidth;
            var v = (float)renderer.Width / textureHeight;
            var tl = new VertexColorTexture(new Vector3(0, 0, 0), Color4b.White, new Vector2(0, 0));
            var tr = new VertexColorTexture(new Vector3(silkWindowWidth, 0, 0), Color4b.White, new Vector2(0, v));
            var br = new VertexColorTexture(new Vector3(silkWindowWidth, silkWindowHeight, 0), Color4b.White, new Vector2(u, v));
            var bl = new VertexColorTexture(new Vector3(0, silkWindowHeight, 0), Color4b.White, new Vector2(u, 0));

            batcher.Begin();
            batcher.DrawRaw(texture, tl, tr, br, bl);
            batcher.End();
        }

        public void Resize(int width, int height)
        {
            silkWindowWidth = width;
            silkWindowHeight = height;
            device.SetViewport(0, 0, (uint)width, (uint)height);
            shader.Projection = Matrix4x4.CreateOrthographicOffCenter(0, width, height, 0, 0, 1);
        }

        public void InitializeWipe()
        {
            renderer.InitializeWipe();
        }

        public bool HasFocus()
        {
            return true;
        }

        public void Dispose()
        {
            Console.WriteLine("Shutdown video.");

            if (shader != null)
            {
                shader.Dispose();
                shader = null;
            }

            if (batcher != null)
            {
                batcher.Dispose();
                batcher = null;
            }

            if (texture != null)
            {
                texture.Dispose();
                texture = null;
            }

            if (device != null)
            {
                device.Dispose();
                device = null;
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
