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
        private RaylibSound sound;
        private RaylibUserInput userInput;

        private Doom doom;

        public RaylibDoom(CommandLineArgs args)
        {
            config = new Config(ConfigUtilities.GetConfigPath());

            content = new GameContent(ConfigUtilities.GetWadPaths(args));

            int windowWidth = 2 * 640;
            int windowHeight = 2 * 400;

            Raylib.InitWindow(windowWidth, windowHeight, "Raylib-CsLo Doom");
            Raylib.InitAudioDevice();

            Raylib.SetExitKey(KeyboardKey.KEY_NULL);
            Raylib.SetTargetFPS(35);

            video = new RaylibVideo(config, content);
            sound = new RaylibSound(config, content.Wad);
            userInput = new RaylibUserInput(config, !args.nomouse.Present);

            doom = new Doom(args, config, content, video, sound, null, userInput);
        }

        public void Run()
        {
            while (!Raylib.WindowShouldClose())
            {
                for (var i = 0; i <= (int)KeyboardKey.KEY_KB_MENU; i++)
                {
                    var key = (KeyboardKey)i;

                    if (Raylib.IsKeyPressed(key))
                    {
                        doom.PostEvent(new DoomEvent(EventType.KeyDown, RaylibUserInput.RayToDoom(key)));
                    }

                    if (Raylib.IsKeyReleased(key))
                    {
                        doom.PostEvent(new DoomEvent(EventType.KeyUp, RaylibUserInput.RayToDoom(key)));
                    }
                }

                if (doom.Update() == UpdateResult.Completed)
                {
                    break;
                }

                video.Render(doom);
            }
        }

        public void Dispose()
        {
            if (userInput != null)
            {
                userInput.Dispose();
                userInput = null;
            }

            if (sound != null)
            {
                sound.Dispose();
                sound = null;
            }

            if (video != null)
            {
                video.Dispose();
                video = null;
            }

            Raylib.CloseWindow();
        }

        public string QuitMessage => null;
    }
}
