using AudioSynthesis.Util.Riff;
using System.IO;

namespace AudioSynthesis.Sf2.Chunks
{
    public class SampleHeaderChunk : Chunk
    {
        private SampleHeader[] sampleHeaders;

        public SampleHeader[] SampleHeaders
        {
            get { return sampleHeaders; }
            set { sampleHeaders = value; }
        }

        public SampleHeaderChunk(string id, int size, BinaryReader reader)
            : base(id, size)
        {
            if (size % 46 != 0)
                throw new InvalidDataException("Invalid SoundFont. The sample header chunk was invalid.");
            sampleHeaders = new SampleHeader[(size / 46) - 1];
            for (int x = 0; x < sampleHeaders.Length; x++)
            {
                sampleHeaders[x] = new SampleHeader(reader);
            }
            new SampleHeader(reader); //read terminal record
        }
    }
}
