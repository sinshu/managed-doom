using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Text;
using SFML.Audio;
using SFML.System;

namespace ManagedDoom
{
    public sealed class SfmlAudio : IAudio, IDisposable
    {
        private static readonly int channelCount = 8;

        private static readonly float fastDecay = (float)Math.Pow(0.5, 1.0 / (35 / 5));
        private static readonly float slowDecay = (float)Math.Pow(0.5, 1.0 / 35);

        private static readonly float clipDist = 1200;
        private static readonly float closeDist = 160;
        private static readonly float attenuator = clipDist - closeDist;

        private SoundBuffer[] buffers;
        private float[] amplitudes;

        private Sound[] channels;
        private ChannelInfo[] infos;

        private Sound uiChannel;
        private Sfx uiReserved;

        private Mobj listener;

        private int masterVolume;
        private float masterVolumeDecay;

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

                uiChannel = new Sound();
                uiReserved = Sfx.NONE;
            }
            catch (Exception e)
            {
                Dispose();
                ExceptionDispatchInfo.Throw(e);
            }

            masterVolume = 8;
            masterVolumeDecay = (float)masterVolume / MaxSoundVolume;
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
            return (float)max / 32768;
        }

        public void SetListener(Mobj listener)
        {
            this.listener = listener;
        }

        public void Update()
        {
            for (var i = 0; i < infos.Length; i++)
            {
                var info = infos[i];
                var channel = channels[i];

                if (info.Playing != Sfx.NONE)
                {
                    if (channel.Status != SoundStatus.Stopped)
                    {
                        if (info.Type == SfxType.Diffuse)
                        {
                            info.Priority *= slowDecay;
                        }
                        else
                        {
                            info.Priority *= fastDecay;
                        }
                        SetParam(channel, info);
                    }
                    else
                    {
                        info.Playing = Sfx.NONE;
                        if (info.Reserved == Sfx.NONE)
                        {
                            info.Source = null;
                        }
                    }
                }

                if (info.Reserved != Sfx.NONE)
                {
                    if (info.Playing != Sfx.NONE)
                    {
                        channel.Stop();
                    }

                    channel.SoundBuffer = buffers[(int)info.Reserved];
                    SetParam(channel, info);
                    channel.Play();
                    info.Playing = info.Reserved;
                    info.Reserved = Sfx.NONE;
                }
            }

            if (uiReserved != Sfx.NONE)
            {
                if (uiChannel.Status == SoundStatus.Playing)
                {
                    uiChannel.Stop();
                }
                uiChannel.Volume = 100 * masterVolumeDecay;
                uiChannel.SoundBuffer = buffers[(int)uiReserved];
                uiChannel.Play();
                uiReserved = Sfx.NONE;
            }
        }

        public void StartSound(Sfx sfx)
        {
            uiReserved = sfx;
        }

        public void StartSound(Mobj mobj, Sfx sfx, SfxType type)
        {
            StartSound(mobj, sfx, type, 100);
        }

        public void StartSound(Mobj mobj, Sfx sfx, SfxType type, int volume)
        {
            var x = (mobj.X - listener.X).ToFloat();
            var y = (mobj.Y - listener.Y).ToFloat();
            var dist = MathF.Sqrt(x * x + y * y);

            float priority;
            if (type == SfxType.Diffuse)
            {
                priority = volume;
            }
            else
            {
                priority = amplitudes[(int)sfx] * GetDistanceDecay(dist) * volume;
            }

            if (priority < 0.001F)
            {
                return;
            }

            for (var i = 0; i < infos.Length; i++)
            {
                var info = infos[i];
                if (info.Source == mobj && info.Type == type)
                {
                    info.Reserved = sfx;
                    info.Priority = priority;
                    info.Volume = volume;
                    return;
                }
            }

            for (var i = 0; i < infos.Length; i++)
            {
                var info = infos[i];
                if (info.Reserved == Sfx.NONE && info.Playing == Sfx.NONE)
                {
                    info.Reserved = sfx;
                    info.Priority = priority;
                    info.Source = mobj;
                    info.Type = type;
                    info.Volume = volume;
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
            if (priority >= minPriority)
            {
                var info = infos[minChannel];
                info.Reserved = sfx;
                info.Priority = priority;
                info.Source = mobj;
                info.Type = type;
                info.Volume = volume;
            }
        }

        public void StopSound(Mobj mobj)
        {
            for (var i = 0; i < infos.Length; i++)
            {
                var info = infos[i];
                if (info.Source == mobj)
                {
                    info.LastX = info.Source.X;
                    info.LastY = info.Source.Y;
                    info.Source = null;
                    info.Volume /= 5;
                }
            }
        }

        public void Reset()
        {
            for (var i = 0; i < infos.Length; i++)
            {
                channels[i].Stop();
                infos[i].Clear();
            }

            listener = null;
        }

        public void Pause()
        {
            for (var i = 0; i < infos.Length; i++)
            {
                var channel = channels[i];

                if (channel.Status == SoundStatus.Playing &&
                    channel.SoundBuffer.Duration - channel.PlayingOffset > Time.FromMilliseconds(200))
                {
                    channels[i].Pause();
                }
            }
        }

        public void Resume()
        {
            for (var i = 0; i < infos.Length; i++)
            {
                var channel = channels[i];

                if (channel.Status == SoundStatus.Paused)
                {
                    channel.Play();
                }
            }
        }

        private void SetParam(Sound sound, ChannelInfo info)
        {
            if (info.Type == SfxType.Diffuse)
            {
                sound.Position = new Vector3f(0, 1, 0);
                sound.Volume = masterVolumeDecay * info.Volume;
            }
            else
            {
                Fixed sourceX;
                Fixed sourceY;
                if (info.Source == null)
                {
                    sourceX = info.LastX;
                    sourceY = info.LastY;
                }
                else
                {
                    sourceX = info.Source.X;
                    sourceY = info.Source.Y;
                }

                var x = (sourceX - listener.X).ToFloat();
                var y = (sourceY - listener.Y).ToFloat();

                if (Math.Abs(x) < 16 && Math.Abs(y) < 16)
                {
                    sound.Position = new Vector3f(0, 1, 0);
                    sound.Volume = masterVolumeDecay * info.Volume;
                }
                else
                {
                    var dist = MathF.Sqrt(x * x + y * y);
                    var angle = MathF.Atan2(y, x) - (float)listener.Angle.ToRadian() + MathF.PI / 2;
                    sound.Position = new Vector3f(MathF.Cos(angle), MathF.Sin(angle), 0);
                    sound.Volume = masterVolumeDecay * GetDistanceDecay(dist) * info.Volume;
                }
            }
        }

        private float GetDistanceDecay(float dist)
        {
            if (dist < closeDist)
            {
                return 1F;
            }
            else
            {
                return Math.Max((clipDist - dist) / attenuator, 0F);
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

            if (uiChannel != null)
            {
                uiChannel.Dispose();
                uiChannel = null;
            }

            Console.WriteLine("Audio resources are disposed.");
        }

        public int MaxSoundVolume
        {
            get
            {
                return 15;
            }
        }

        public int SoundVolume
        {
            get
            {
                return masterVolume;
            }

            set
            {
                masterVolume = value;
                masterVolumeDecay = (float)masterVolume / MaxSoundVolume;
            }
        }



        private class ChannelInfo
        {
            public Sfx Reserved;
            public Sfx Playing;
            public float Priority;

            public Mobj Source;
            public SfxType Type;
            public int Volume;
            public Fixed LastX;
            public Fixed LastY;

            public void Clear()
            {
                Reserved = Sfx.NONE;
                Playing = Sfx.NONE;
                Priority = 0;
                Source = null;
                Type = 0;
                Volume = 0;
                LastX = Fixed.Zero;
                LastY = Fixed.Zero;
            }
        }
    }
}
