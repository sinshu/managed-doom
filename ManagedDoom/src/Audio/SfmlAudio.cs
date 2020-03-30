using System;
using SFML.Audio;

namespace ManagedDoom
{
    public sealed class SfmlAudio : IDisposable
    {
        private SoundBuffer[] buffers;
        private Sound[] channels;

        public SfmlAudio(Wad wad)
        {
            buffers = new SoundBuffer[DoomInfo.SfxNames.Length];
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
            }

            channels = new Sound[16];
            for (var i = 0; i < channels.Length; i++)
            {
                channels[i] = new Sound();
            }
        }

        public void StartSound(Mobj mobj, Sfx sfx)
        {
            for (var i = 0; i < channels.Length; i++)
            {
                if (channels[i].Status == SoundStatus.Stopped)
                {
                    channels[i].SoundBuffer = buffers[(int)sfx];
                    channels[i].Play();
                    return;
                }
            }
        }

        public void StopSound(Mobj mobj)
        {
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
