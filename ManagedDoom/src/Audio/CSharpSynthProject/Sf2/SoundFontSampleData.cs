namespace AudioSynthesis.Sf2
{
    using System;
    using System.IO;
    using AudioSynthesis.Util;

    public class SoundFontSampleData
    {
        //--Fields
        private float[] samples;
        private int bitsPerSample;
        //--Properties
        public int BitsPerSample
        {
            get { return bitsPerSample; }
        }
        public float[] SampleData
        {
            get { return samples; }
        }
        //--Methods
        public SoundFontSampleData(BinaryReader reader)
        {
            if (new string(IOHelper.Read8BitChars(reader, 4)).ToLower().Equals("list") == false)
                throw new InvalidDataException("Invalid soundfont. Could not find SDTA LIST chunk.");
            long readTo = reader.ReadInt32();
            readTo += reader.BaseStream.Position;
            if (new string(IOHelper.Read8BitChars(reader, 4)).Equals("sdta") == false)
                throw new InvalidDataException("Invalid soundfont. List is not of type sdta.");
            bitsPerSample = 0;
            byte[] rawSampleData = null;
            while (reader.BaseStream.Position < readTo)
            {
                string subID = new string(IOHelper.Read8BitChars(reader, 4));
                int size = reader.ReadInt32();
                switch (subID.ToLower())
                {
                    case "smpl":
                        bitsPerSample = 16;
                        rawSampleData = reader.ReadBytes(size);
                        samples = new float[rawSampleData.Length / 2];
                        break;
                    case "sm24":
                        if (rawSampleData == null || size != (int)Math.Ceiling(samples.Length / 2.0))
                        {//ignore this chunk if wrong size or if it comes first
                            reader.ReadBytes(size);
                        }
                        else
                        {
                            bitsPerSample = 24;
                            for (int x = 0; x < samples.Length; x++)
                            {
                                samples[x] = IOHelper.ReadInt24(new byte[] { reader.ReadByte(), rawSampleData[2 * x], rawSampleData[2 * x + 1] }, 0) / 8388608f;
                            }
                        }
                        if (size % 2 == 1 && reader.PeekChar() == 0)
                            reader.ReadByte();
                        break;
                    default:
                        throw new InvalidDataException("Invalid soundfont. Unknown chunk id: " + subID + ".");
                }
            }
            if (bitsPerSample == 16)
            {
                for (int x = 0; x < samples.Length; x++)
                {
                    samples[x] = IOHelper.ReadInt16(rawSampleData, 2 * x) / 32768f;
                }
            }
            else if (bitsPerSample != 24)
                throw new NotSupportedException("Only 16 and 24 bit samples are supported.");
        }
    }
}
