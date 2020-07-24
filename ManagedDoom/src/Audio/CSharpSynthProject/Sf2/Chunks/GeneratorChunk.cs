using AudioSynthesis.Util.Riff;
using System.IO;

namespace AudioSynthesis.Sf2.Chunks
{
    public class GeneratorChunk : Chunk
    {
        private Generator[] generators;

        public Generator[] Generators
        {
            get { return generators; }
            set { generators = value; }
        }

        public GeneratorChunk(string id, int size, BinaryReader reader)
            : base(id, size)
        {
            if (size % 4 != 0)
                throw new InvalidDataException("Invalid SoundFont. The presetzone chunk was invalid.");
            generators = new Generator[(size / 4) - 1];
            for (int x = 0; x < generators.Length; x++)
                generators[x] = new Generator(reader);
            new Generator(reader); //terminal record
        }
    }
}
