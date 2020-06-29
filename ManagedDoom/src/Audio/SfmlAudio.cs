using System;
using System.Runtime.ExceptionServices;
using System.Text;
using SFML.Audio;
using SFML.System;

namespace ManagedDoom
{
    public sealed class SfmlAudio : IDisposable
    {
        private static readonly int channelCount = 16;
        private static readonly float decay = (float)Math.Pow(0.5, 1.0 / (35 / 5));

        private SoundBuffer[] buffers;
        private float[] amplitudes;

        private Sound[] channels;
        private ChannelInfo[] infos;

        public SfmlAudio(Wad wad)
        {
            buffers = new SoundBuffer[DoomInfo.SfxNames.Length];
            amplitudes = new float[DoomInfo.SfxNames.Length];

            try
            {
                for (var i = 0; i < DoomInfo.SfxNames.Length; i++)
                {
                    var name = "DS" + DoomInfo.SfxNames[i];
                    var lump = wad.GetLumpNumber(name);
                    if (lump == -1)
                    {
                        continue;
                    }

                    int sampleRate;
                    int sampleCount;
                    var samples = GetSamples(wad, name, out sampleRate, out sampleCount);
                    buffers[i] = new SoundBuffer(samples, 1, (uint)sampleRate);
                    amplitudes[i] = GetAmplitude(samples, sampleRate, sampleCount);
                }

                channels = new Sound[channelCount];
                infos = new ChannelInfo[channelCount];
                for (var i = 0; i < channels.Length; i++)
                {
                    channels[i] = new Sound();
                    infos[i] = new ChannelInfo();
                }
            }
            catch (Exception e)
            {
                Dispose();
                ExceptionDispatchInfo.Throw(e);
            }
        }

        private static short[] GetSamples(Wad wad, string name, out int sampleRate, out int sampleCount)
        {
            var data = wad.ReadLump(name);

            sampleRate = BitConverter.ToUInt16(data, 2);
            sampleCount = BitConverter.ToInt32(data, 4) - 32;

            var samples = new short[sampleCount];
            for (var t = 0; t < samples.Length; t++)
            {
                samples[t] = (short)((data[24 + t] - 128) << 8);
            }
            return samples;
        }

        private static float GetAmplitude(short[] samples, int sampleRate, int sampleCount)
        {
            var max = 0;
            if (sampleCount > 0)
            {
                var count = Math.Min(sampleRate / 5, sampleCount);
                for (var t = 0; t < count; t++)
                {
                    var a = (int)samples[t];
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
            return (float)max / 65536;
        }

        public void Update()
        {
            for (var i = 0; i < infos.Length; i++)
            {
                var info = infos[i];
                var channel = channels[i];

                info.Priority *= decay;

                if (info.Playing != Sfx.NONE && channel.Status == SoundStatus.Stopped)
                {
                    info.Playing = Sfx.NONE;
                    info.Source = null;
                }

                if (info.Reserved != Sfx.NONE)
                {
                    if (info.Playing != Sfx.NONE)
                    {
                        channel.Stop();
                    }

                    channel.SoundBuffer = buffers[(int)info.Reserved];
                    SetParam(channel, info.Source);
                    channel.Play();
                    info.Playing = info.Reserved;
                    info.Reserved = Sfx.NONE;
                }
            }
        }

        public void StartSound(Mobj mobj, Sfx sfx)
        {
            for (var i = 0; i < infos.Length; i++)
            {
                var info = infos[i];
                if (info.Source == mobj)
                {
                    info.Reserved = sfx;
                    info.Priority = amplitudes[(int)sfx];
                    return;
                }
            }

            for (var i = 0; i < infos.Length; i++)
            {
                var info = infos[i];
                if (info.Reserved == Sfx.NONE && info.Playing == Sfx.NONE)
                {
                    info.Reserved = sfx;
                    info.Priority = amplitudes[(int)sfx];
                    info.Source = mobj;
                    return;
                }
            }

            var minPriority = float.MaxValue;
            var minChannel = -1;
            for (var i = 0; i < infos.Length; i++)
            {
                var info = infos[i];
                if (info.Priority < minPriority)
                {
                    minPriority = info.Priority;
                    minChannel = i;
                }
            }
            if (amplitudes[(int)sfx] >= minPriority)
            {
                var info = infos[minChannel];
                info.Reserved = sfx;
                info.Priority = amplitudes[(int)sfx];
                info.Source = mobj;
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
            if (mobj != null)
            {
                var world = mobj.World;
                var player = world.Options.Players[world.Options.ConsolePlayer].Mobj;
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
            else
            {
                sound.Position = new Vector3f(0, 1, 0);
                sound.Volume = 100;
            }
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



        private class ChannelInfo
        {
            public Sfx Reserved;
            public Sfx Playing;
            public float Priority;

            public Mobj Source;
            public float X;
            public float Y;
            public float Volume;
        }
    }
}
