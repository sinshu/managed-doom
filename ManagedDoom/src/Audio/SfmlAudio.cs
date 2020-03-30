using System;
using SFML.Audio;
using SFML.System;

namespace ManagedDoom
{
    public sealed class SfmlAudio : IDisposable
    {
        private static readonly int channelCount = 16;

        private SoundBuffer[] buffers;
        private short[] amplitudes;

        private Sound[] channels;
        private Mobj[] sources;
        private float[] priorities;

        private World world;

        public SfmlAudio(Wad wad)
        {
            buffers = new SoundBuffer[DoomInfo.SfxNames.Length];
            amplitudes = new short[DoomInfo.SfxNames.Length];
            for (var i = 0; i < DoomInfo.SfxNames.Length; i++)
            {
                var lump = wad.GetLumpNumber("DS" + DoomInfo.SfxNames[i]);

                if (lump == -1)
                {
                    continue;
                }

                var data = wad.ReadLump(lump);

                var sampleRate = BitConverter.ToUInt16(data, 2);
                var sampleCount = BitConverter.ToInt32(data, 4) - 32;
                var samples = new short[sampleCount];
                for (var t = 0; t < samples.Length; t++)
                {
                    samples[t] = (short)((data[24 + t] - 128) << 8);
                }

                buffers[i] = new SoundBuffer(samples, 1, sampleRate);

                short max = 0;
                if (sampleCount > 0)
                {
                    var count = Math.Min(sampleRate / 5, sampleCount);
                    for (var t = 0; t < count; t++)
                    {
                        var a = samples[t];
                        if (a == short.MinValue)
                        {
                            max = short.MaxValue;
                            break;
                        }
                        if (a < 0)
                        {
                            a = (short)(-a);
                        }
                        if (a > max)
                        {
                            max = a;
                        }
                    }
                }
                amplitudes[i] = max;
            }

            channels = new Sound[channelCount];
            sources = new Mobj[channelCount];
            priorities = new float[channelCount];
            for (var i = 0; i < channels.Length; i++)
            {
                channels[i] = new Sound();
            }
        }

        public void BindWorld(World world)
        {
            this.world = world;
            world.Audio = this;
        }

        public void UnbindWorld()
        {
            StopAll();

            for (var i = 0; i < sources.Length; i++)
            {
                sources[i] = null;
            }

            world.Audio = null;
            world = null;
        }

        public void StartSound(Mobj mobj, Sfx sfx)
        {
            if (mobj == null)
            {
                for (var i = 0; i < channels.Length; i++)
                {
                    if (channels[i].Status == SoundStatus.Stopped)
                    {
                        sources[i] = null;
                        channels[i].SoundBuffer = buffers[(int)sfx];
                        channels[i].Position = new Vector3f(0, 1, 0);
                        channels[i].Volume = 100;
                        channels[i].Play();
                        return;
                    }
                }
            }
            else
            {
                for (var i = 0; i < channels.Length; i++)
                {
                    if (sources[i] == mobj && channels[i].Status == SoundStatus.Playing)
                    {
                        channels[i].Stop();
                        channels[i].SoundBuffer = buffers[(int)sfx];
                        SetParam(channels[i], mobj);
                        channels[i].Play();
                        return;
                    }
                }
                for (var i = 0; i < channels.Length; i++)
                {
                    if (channels[i].Status == SoundStatus.Stopped)
                    {
                        sources[i] = mobj;
                        channels[i].SoundBuffer = buffers[(int)sfx];
                        SetParam(channels[i], mobj);
                        channels[i].Play();
                        return;
                    }
                }
            }
        }

        public void StopSound(Mobj mobj)
        {
        }

        public void StopAll()
        {
            for (var i = 0; i < channels.Length; i++)
            {
                channels[i].Stop();
            }
        }

        private void SetParam(Sound sound, Mobj mobj)
        {
            var player = world.Players[world.Options.ConsolePlayer].Mobj;
            var x = (mobj.X - player.X).ToDouble();
            var y = (mobj.Y - player.Y).ToDouble();
            var dist = Math.Sqrt(x * x + y * y);

            if (Math.Abs(x) < 0.01 && Math.Abs(y) < 0.01)
            {
                sound.Position = new Vector3f(0, 1, 0);
            }
            else
            {
                var angle = Math.Atan2(y, x) - player.Angle.ToRadian() + Math.PI / 2;
                sound.Position = new Vector3f((float)Math.Cos(angle), (float)Math.Sin(angle), 0);
            }
            sound.Volume = (float)(100 * Math.Pow(2, -dist / 512));
        }

        public void Dispose()
        {
            for (var i = 0; i < channels.Length; i++)
            {
                if (channels[i] != null)
                {
                    channels[i].Stop();
                    channels[i].Dispose();
                    channels[i] = null;
                }
            }

            for (var i = 0; i < buffers.Length; i++)
            {
                if (buffers[i] != null)
                {
                    buffers[i].Dispose();
                    buffers[i] = null;
                }
            }

            Console.WriteLine("Audio resources are disposed.");
        }
    }
}
