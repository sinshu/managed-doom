using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Silk.NET.Windowing;

namespace ManagedDoom.Silk
{
    public partial class SilkDoom : IDisposable
    {
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
                window.UpdatesPerSecond = 35;
                window.FramesPerSecond = 35;
            }

            window.Run();
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
                var gameTimeStep = TimeSpan.FromSeconds(1.0 / 35);

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
        }
#endif
    }
}
