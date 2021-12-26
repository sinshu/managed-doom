using System;
using Raylib_CsLo;
using ManagedDoom;

namespace ManagedDoom.Raylib_CsLo
{
    public class RaylibDoom : IDisposable
    {
        private Config config;

        private GameContent content;

        private RaylibVideo video;

        private Doom doom;

        public RaylibDoom(CommandLineArgs args)
        {
            config = new Config(ConfigUtilities.GetConfigPath());

            content = new GameContent(ConfigUtilities.GetWadPaths(args));

            int windowWidth = 2 * 640;
            int windowHeight = 2 * 400;

            Raylib.InitWindow(windowWidth, windowHeight, "Raylib-CsLo Doom");

            Raylib.SetTargetFPS(35);

            video = new RaylibVideo(config, content);

            doom = new Doom(args, config, content, video, null, null, null);
        }

        public void Run()
        {
            while (!Raylib.WindowShouldClose())
            {
                doom.Update();
                video.Render(doom);
            }
        }

        public void Dispose()
        {
            Raylib.CloseWindow();
        }

        public string QuitMessage => null;
    }
}
