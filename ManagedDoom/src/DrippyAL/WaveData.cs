using System;
using System.Runtime.ExceptionServices;
using Silk.NET.OpenAL;

namespace DrippyAL
{
    public unsafe sealed class WaveData : IDisposable
    {
        private AL? al;

        private uint alBuffer;

        private TimeSpan duration;

        public WaveData(AudioDevice device, int sampleRate, int channelCount, Span<short> data)
        {
            try
            {
                al = device.AL;

                alBuffer = al.GenBuffer();

                BufferFormat format;
                switch (channelCount)
                {
                    case 1:
                        format = BufferFormat.Mono16;
                        break;

                    case 2:
                        format = BufferFormat.Stereo16;
                        break;

                    default:
                        throw new ArgumentException(nameof(channelCount));
                }

                fixed (short* p = data)
                {
                    var size = sizeof(short) * data.Length;
                    al.BufferData(alBuffer, format, p, size, sampleRate);
                }

                duration = TimeSpan.FromSeconds(data.Length / channelCount / (double)sampleRate);
            }
            catch (Exception e)
            {
                Dispose();
                ExceptionDispatchInfo.Throw(e);
            }
        }

        public WaveData(AudioDevice device, int sampleRate, int channelCount, Span<byte> data)
        {
            try
            {
                al = device.AL;

                alBuffer = al.GenBuffer();

                BufferFormat format;
                switch (channelCount)
                {
                    case 1:
                        format = BufferFormat.Mono8;
                        break;

                    case 2:
                        format = BufferFormat.Stereo8;
                        break;

                    default:
                        throw new ArgumentException(nameof(channelCount));
                }

                fixed (byte* p = data)
                {
                    var size = sizeof(byte) * data.Length;
                    al.BufferData(alBuffer, format, p, size, sampleRate);
                }

                duration = TimeSpan.FromSeconds(data.Length / channelCount / (double)sampleRate);
            }
            catch (Exception e)
            {
                Dispose();
                ExceptionDispatchInfo.Throw(e);
            }
        }

        public void Dispose()
        {
            if (al != null)
            {
                if (alBuffer != 0)
                {
                    al.DeleteBuffer(alBuffer);
                    alBuffer = 0;
                }

                al = null;
            }
        }

        internal uint AlBuffer
        {
            get
            {
                if (al == null)
                {
                    throw new ObjectDisposedException(nameof(WaveData));
                }

                return alBuffer;
            }
        }

        public TimeSpan Duration => duration;
    }
}
