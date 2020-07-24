namespace AudioSynthesis.Wave
{
    using System;
    using AudioSynthesis.Util;

    public static class WaveHelper
    {
        //--Methods
        public static float[] GetSampleData(WaveFile wave, int expectedChannels)
        {
            int samplesPerChannel = (wave.Data.RawSampleData.Length / (wave.Format.BitsPerSample / 8)) / wave.Format.ChannelCount;
            float[] sampleData = new float[samplesPerChannel * expectedChannels];
            int channels = Math.Min(expectedChannels, wave.Format.ChannelCount);
            for (int x = 0; x < channels; x++)
                ToSamplesPcm(wave.Data.RawSampleData, wave.Format.BitsPerSample, wave.Format.ChannelCount, sampleData, x);
            return sampleData;
        }
        public static float[] GetMonoSampleData(byte[] rawData, int bitsPerSample, int channelCount)
        {
            int samplesPerChannel = (rawData.Length / (bitsPerSample / 8)) / channelCount;
            float[] sampleData = new float[samplesPerChannel];
            ToSamplesPcm(rawData, bitsPerSample, channelCount, sampleData, 0);
            return sampleData;
        }
        public static float[] Deinterleave(float[] data, int channelCount, int channel)
        {
            CrossPlatformHelper.Assert(channel >= 0 && channel < channelCount, "Channel must be non-negative and less than the channel count.");
            CrossPlatformHelper.Assert(data != null && data.Length % channelCount == 0, "The data provided is invalid or channel count is incorrect.");

            float[] sampleData = new float[data.Length / channelCount];

            for (int x = 0; x < sampleData.Length; x++)
            {
                sampleData[x] = data[x * channelCount + channel];
            }
            return sampleData;
        }
        public static byte[] GetRawData(float[] left, float[] right, int bitsPerSample)
        {
            byte[] output = new byte[2 * left.Length * bitsPerSample / 8];
            if (left != null)
                ToBytes(left, bitsPerSample, 2, output, 0);
            if(right != null)
                ToBytes(right, bitsPerSample, 2, output, bitsPerSample / 8);
            return output;
        }
        public static byte[] GetRawData(float[] buffer, int bitsPerSample)
        {
            byte[] output = new byte[buffer.Length * bitsPerSample / 8];
            ToBytes(buffer, bitsPerSample, 1, output, 0);
            return output;
        }

        //returns raw audio data in little endian form
        private static void ToBytes(float[] input, int bitsPerSample, int channels, byte[] output, int index)
        {
            switch (bitsPerSample)
            {
                case 8:
                    for (int x = 0; x < input.Length; x++)
                    {
                        output[index] = (byte)((input[x] + 1f) / 2f * 255f);
                        index += channels;
                    }
                    break;
                case 16:
                    for (int x = 0; x < input.Length; x++)
                    {
                        IOHelper.WriteInt16((short)(input[x] * 32767f), output, index);
                        index += channels * 2;
                    }
                    break;
                case 24:
                    for (int x = 0; x < input.Length; x++)
                    {
                        IOHelper.WriteInt24((int)(input[x] * 8388607f), output, index);
                        index += channels * 3;
                    }
                    break;
                case 32:
                    for (int x = 0; x < input.Length; x++)
                    {
                        IOHelper.WriteInt32((int)(input[x] * 2147483647f), output, index);
                        index += channels * 4;
                    }
                    break;
                default:
                    throw new ArgumentException("Invalid bitspersample value. Supported values are 8, 16, 24, and 32.");
            }
        }
        private static void ToSamplesPcm(byte[] input, int bitsPerSample, int channelCount, float[] output, int channel)
        {
            switch (bitsPerSample)
            {
                case 8:
                    for (int x = channel; x < output.Length; x += channelCount)
                    {
                        output[x] = ((input[x] / 255f) * 2f) - 1f;
                    }
                    break;
                case 16:
                    for (int x = channel; x < output.Length; x += channelCount)
                    {
                        output[x] = IOHelper.ReadInt16(input, x * 2) / 32768f;
                    }
                    break;
                case 24:
                    for (int x = channel; x < output.Length; x += channelCount)
                    {
                        output[x] = IOHelper.ReadInt24(input, x * 3) / 8388608f;
                    }
                    break;
                case 32:
                    for (int x = channel; x < output.Length; x += channelCount)
                    {
                        output[x] = IOHelper.ReadInt32(input, x * 4) / 2147483648f;
                    }
                    break;
                default:
                    throw new Exception("Invalid sample format: PCM " + bitsPerSample + "bps.");
            }
        }
        private static void ToSamplesFloat(byte[] input, int bitsPerSample, int channelCount, float[] output, int channel)
        {
            byte[] buffer = new byte[bitsPerSample / 8];
            switch (bitsPerSample)
            {
                case 32:
                    for (int x = channel; x < output.Length; x += channelCount)
                    {
                        Array.Copy(input, x * 4, buffer, 0, 4);
                        if (!BitConverter.IsLittleEndian)
                            Array.Reverse(buffer);
                        output[x] = BitConverter.ToSingle(buffer, 0);
                    }
                    break;
                case 64:
                    for (int x = channel; x < output.Length; x += channelCount)
                    {
                        Array.Copy(input, x * 8, buffer, 0, 8);
                        if (!BitConverter.IsLittleEndian)
                            Array.Reverse(buffer);
                        output[x] = (float)BitConverter.ToDouble(buffer, 0);
                    }
                    break;
                default:
                    throw new Exception("Invalid sample format: FLOAT " + bitsPerSample + "bps.");
            }
        }
    }
}
