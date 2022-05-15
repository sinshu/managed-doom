using System;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using DrippyAL;

namespace ManagedDoom.Silk
{
    public class SilkDoom : IDisposable
    {
        private CommandLineArgs args;

        private Config config;

        private IWindow window;
        private GL gl;
        private AudioDevice audioDevice;

        private GameContent content;

        private SilkVideo video;
        private SilkSound sound;
        private SilkMusic music;
        private SilkUserInput userInput;

        private Doom doom;

        public SilkDoom(CommandLineArgs args)
        {
            this.args = args;

            config = new Config(ConfigUtilities.GetConfigPath());

            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(2 * 640, 2 * 400);
            options.Title = "Silky Doom";
            window = Window.Create(options);

            window.Load += OnLoad;
            window.Update += OnUpdate;
            window.Render += OnRender;
            window.Resize += Window_Resize;
            window.Closing += OnClose;

            if (!args.timedemo.Present)
            {
                window.UpdatesPerSecond = 35;
                window.FramesPerSecond = 35;
            }
            window.VSync = false;
        }

        public void Run()
        {
            window.Run();
        }

        public void KeyDown(Key key)
        {
            doom.PostEvent(new DoomEvent(EventType.KeyDown, SilkUserInput.SilkToDoom(key)));
        }

        public void KeyUp(Key key)
        {
            doom.PostEvent(new DoomEvent(EventType.KeyUp, SilkUserInput.SilkToDoom(key)));
        }

        private void OnLoad()
        {
            gl = window.CreateOpenGL();

            content = new GameContent(args);

            video = new SilkVideo(config, content, window, gl);

            audioDevice = new AudioDevice();
            sound = new SilkSound(config, content.Wad, audioDevice);
            music = new SilkMusic(config, content.Wad, audioDevice, "TimGM6mb.sf2");

            userInput = new SilkUserInput(config, window, this);

            doom = new Doom(args, config, content, video, sound, music, userInput);
        }

        private void OnUpdate(double obj)
        {
            if (doom.Update() == UpdateResult.Completed)
            {
                window.Close();
            }
        }

        private void OnRender(double obj)
        {
            video.Render(doom);
        }

        private void Window_Resize(Vector2D<int> obj)
        {
            video.Resize(obj.X, obj.Y);
        }

        private void OnClose()
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

            if (audioDevice != null)
            {
                audioDevice.Dispose();
                audioDevice = null;
            }

            if (video != null)
            {
                video.Dispose();
                video = null;
            }

            config.Save(ConfigUtilities.GetConfigPath());
        }

        public void Dispose()
        {
            if (window != null)
            {
                window.Close();
                window.Dispose();
                window = null;
            }
        }

        public string QuitMessage => doom.QuitMessage;
    }
}
