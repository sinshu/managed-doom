using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Silk.NET.Windowing;

namespace ManagedDoom.Silk
{
    public partial class SilkDoom : IDisposable
    {
        // The main loop provided by Silk.NET with the IWindow.Run method uses a busy loop
        // to control the drawing timing, which is CPU-intensive.
        // Here, I have implemented my own main loop to reduce the CPU load, targeting only
        // Windows, which I can test at hand.
        // In other environments, the IWindow.Run method provided by Silk.NET is used.

#if !WINDOWS_RELEASE
        public void Run()
        {
            if (args.timedemo.Present)
            {
                window.UpdatesPerSecond = 0;
                window.FramesPerSecond = 0;
            }
            else
            {
                config.video_fpsscale = Math.Clamp(config.video_fpsscale, 1, 100);
                var targetFps = 35 * config.video_fpsscale;
                window.UpdatesPerSecond = targetFps;
                window.FramesPerSecond = targetFps;
            }

            window.Run();

            Quit();
        }
#else
        [DllImport("winmm.dll")]
        private static extern uint timeBeginPeriod(uint uPeriod);

        [DllImport("winmm.dll")]
        private static extern uint timeEndPeriod(uint uPeriod);

        private void Sleep(int ms)
        {
            timeBeginPeriod(1);
            Thread.Sleep(ms);
            timeEndPeriod(1);
        }

        public void Run()
        {
            config.video_fpsscale = Math.Clamp(config.video_fpsscale, 1, 100);
            var targetFps = 35 * config.video_fpsscale;

            window.FramesPerSecond = 0;
            window.UpdatesPerSecond = 0;

            if (args.timedemo.Present)
            {
                window.Run();
            }
            else
            {
                window.Initialize();

                var gameTime = TimeSpan.Zero;
                var gameTimeStep = TimeSpan.FromSeconds(1.0 / targetFps);

                var sw = new Stopwatch();
                sw.Start();

                while (true)
                {
                    window.DoEvents();

                    if (!window.IsClosing)
                    {
                        window.DoUpdate();
                        gameTime += gameTimeStep;
                    }

                    if (!window.IsClosing)
                    {
                        if (sw.Elapsed < gameTime)
                        {
                            window.DoRender();
                            var sleepTime = gameTime - sw.Elapsed;
                            var ms = (int)sleepTime.TotalMilliseconds;
                            if (ms > 0)
                            {
                                Sleep(ms);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                window.DoEvents();
                window.Reset();
            }

            Quit();
        }
#endif
    }
}
