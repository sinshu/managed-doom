using System;
using System.Runtime.ExceptionServices;
using Raylib_CsLo;
using ManagedDoom.Audio;

namespace ManagedDoom.Raylib_CsLo
{
    public sealed class RaylibSound : ISound, IDisposable
    {
        private static readonly float fastDecay = (float)Math.Pow(0.5, 1.0 / (35 / 5));
        private static readonly float slowDecay = (float)Math.Pow(0.5, 1.0 / 35);

        private static readonly float clipDist = 1200;
        private static readonly float closeDist = 160;
        private static readonly float attenuator = clipDist - closeDist;

        private Config config;

        private Sound[] buffers;

        private Mobj listener;


        private float masterVolumeDecay;

        public unsafe RaylibSound(Config config, Wad wad)
        {
            try
            {
                Console.Write("Initialize sound: ");

                this.config = config;

                config.audio_soundvolume = Math.Clamp(config.audio_soundvolume, 0, MaxVolume);

                buffers = new Sound[DoomInfo.SfxNames.Length];

                for (var i = 0; i < DoomInfo.SfxNames.Length; i++)
                {
                    var name = "DS" + DoomInfo.SfxNames[i].ToString().ToUpper();
                    var lump = wad.GetLumpNumber(name);
                    if (lump == -1)
                    {
                        continue;
                    }

                    int sampleRate;
                    int sampleCount;
                    var samples = GetSamples(wad, name, out sampleRate, out sampleCount);
                    if (samples != null)
                    {
                        fixed (void* data = samples)
                        {
                            var wave = new Wave();
                            wave.frameCount = (uint)samples.Length;
                            wave.sampleRate = (uint)sampleRate;
                            wave.sampleSize = 16;
                            wave.channels = 1;
                            wave.data = data;
                            buffers[i] = Raylib.LoadSoundFromWave(wave);
                        }
                    }
                }

                masterVolumeDecay = (float)config.audio_soundvolume / MaxVolume;

                Console.WriteLine("OK");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed");
                Dispose();
                ExceptionDispatchInfo.Throw(e);
            }
        }

        private static short[] GetSamples(Wad wad, string name, out int sampleRate, out int sampleCount)
        {
            var data = wad.ReadLump(name);

            if (data.Length < 8)
            {
                sampleRate = -1;
                sampleCount = -1;
                return null;
            }

            sampleRate = BitConverter.ToUInt16(data, 2);
            sampleCount = BitConverter.ToInt32(data, 4);

            var offset = 8;
            if (ContainsDmxPadding(data))
            {
                offset += 16;
                sampleCount -= 32;
            }

            if (sampleCount > 0)
            {
                var samples = new short[sampleCount];
                for (var t = 0; t < samples.Length; t++)
                {
                    samples[t] = (short)((data[offset + t] - 128) << 8);
                }
                return samples;
            }
            else
            {
                return null;
            }
        }

        // Check if the data contains pad bytes.
        // If the first and last 16 samples are the same,
        // the data should contain pad bytes.
        // https://doomwiki.org/wiki/Sound
        private static bool ContainsDmxPadding(byte[] data)
        {
            var sampleCount = BitConverter.ToInt32(data, 4);
            if (sampleCount < 32)
            {
                return false;
            }
            else
            {
                var first = data[8];
                for (var i = 1; i < 16; i++)
                {
                    if (data[8 + i] != first)
                    {
                        return false;
                    }
                }

                var last = data[8 + sampleCount - 1];
                for (var i = 1; i < 16; i++)
                {
                    if (data[8 + sampleCount - i - 1] != last)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void SetListener(Mobj listener)
        {
            this.listener = listener;
        }

        public void Update()
        {
        }

        public void StartSound(Sfx sfx)
        {
            var sound = buffers[(int)sfx];

            if (Raylib.IsSoundPlaying(sound))
            {
                Raylib.StopSound(sound);
            }

            Raylib.SetSoundVolume(sound, masterVolumeDecay);
            Raylib.PlaySound(sound);
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

            var sound = buffers[(int)sfx];

            if (Raylib.IsSoundPlaying(sound))
            {
                Raylib.StopSound(sound);
            }

            Raylib.SetSoundVolume(sound, masterVolumeDecay * GetDistanceDecay(dist) * (volume / 100F));
            Raylib.PlaySound(sound);
        }

        public void StopSound(Mobj mobj)
        {
        }

        public void Reset()
        {
        }

        public void Pause()
        {
        }

        public void Resume()
        {
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
            Console.WriteLine("Shutdown sound.");

            if (buffers != null)
            {
                for (var i = 0; i < buffers.Length; i++)
                {
                    Raylib.UnloadSound(buffers[i]);
                }
            }
        }

        public int MaxVolume
        {
            get
            {
                return 15;
            }
        }

        public int Volume
        {
            get
            {
                return config.audio_soundvolume;
            }

            set
            {
                config.audio_soundvolume = value;
                masterVolumeDecay = (float)config.audio_soundvolume / MaxVolume;
            }
        }
    }
}
