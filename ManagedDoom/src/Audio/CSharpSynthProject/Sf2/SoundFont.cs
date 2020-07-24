using System.IO;
using AudioSynthesis.Util;

namespace AudioSynthesis.Sf2
{
    public class SoundFont
    {
        //--Fields
        private SoundFontInfo info;
        private SoundFontSampleData data;
        private SoundFontPresets presets;

        //--Properties
        public SoundFontInfo Info
        {
            get { return info; }
        }
        public SoundFontSampleData SampleData
        {
            get { return data; }
        }
        public SoundFontPresets Presets
        {
            get { return presets; }
        }


        //--Methods
        public SoundFont(string fileName)
        {
            if (!Path.GetExtension(fileName).ToLower().Equals(".sf2"))
                throw new InvalidDataException("Invalid soundfont : " + fileName + ".");
            Load(CrossPlatformHelper.OpenResource(fileName));
        }
        public SoundFont(Stream stream)
        {
            Load(stream);
        }

        private void Load(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                string id = new string(IOHelper.Read8BitChars(reader, 4));
                int size = reader.ReadInt32();
                if (!id.ToLower().Equals("riff"))
                    throw new InvalidDataException("Invalid soundfont. Could not find RIFF header.");
                id = new string(IOHelper.Read8BitChars(reader, 4));
                if (!id.ToLower().Equals("sfbk"))
                    throw new InvalidDataException("Invalid soundfont. Riff type is invalid.");
                info = new SoundFontInfo(reader);
                data = new SoundFontSampleData(reader);
                presets = new SoundFontPresets(reader);
            }
        }
    }
}
