using System.IO;
using AudioSynthesis.Util.Riff;

namespace AudioSynthesis.Sf2.Chunks
{
    public class ModulatorChunk : Chunk
    {
        private Modulator[] modulators;

        public Modulator[] Modulators
        {
            get { return modulators; }
        }

        public ModulatorChunk(string id, int size, BinaryReader reader)
            : base(id, size)
        {
            if (size % 10 != 0)
                throw new InvalidDataException("Invalid SoundFont. The presetzone chunk was invalid.");
            modulators = new Modulator[(size / 10) - 1];
            for (int x = 0; x < modulators.Length; x++)
                modulators[x] = new Modulator(reader);
            new Modulator(reader); //terminal record
        }
    }
}
