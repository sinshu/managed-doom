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
        private RaylibMusic music;
        private RaylibUserInput userInput;

        private Doom doom;

        public RaylibDoom(CommandLineArgs args)
        {
            config = new Config(ConfigUtilities.GetConfigPath());

            content = new GameContent(ConfigUtilities.GetWadPaths(args));

            int windowWidth = 2 * 640;
            int windowHeight = 2 * 400;

            Raylib.InitWindow(windowWidth, windowHeight, ApplicationInfo.Title);
            Raylib.InitAudioDevice();

            var audioBufferSize = 4096;
            Raylib.SetAudioStreamBufferSizeDefault(audioBufferSize);

            Raylib.SetExitKey(KeyboardKey.KEY_NULL);
            Raylib.SetTargetFPS(35);

            video = new RaylibVideo(config, content);
            sound = new RaylibSound(config, content.Wad);
            music = RaylibConfigUtilities.GetMusicInstance(config, content.Wad, audioBufferSize);
            userInput = new RaylibUserInput(config, !args.nomouse.Present);

            doom = new Doom(args, config, content, video, sound, music, userInput);
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

                if (music != null)
                {
                    music.Update();
                }
            }
        }

        public void Dispose()
        {
            if (userInput != null)
            {
                userInput.Dispose();
                userInput = null;
            }

            if (music != null)
            {
                music.Dispose();
                music = null;
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
