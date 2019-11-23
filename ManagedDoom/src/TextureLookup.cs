using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ManagedDoom
{
    public sealed class TextureLookup : IReadOnlyList<Texture>
    {
        private List<Texture> textures;
        private Dictionary<string, Texture> nameToTexture;
        private Dictionary<string, int> nameToNumber;

        public TextureLookup(Wad wad)
        {
            textures = new List<Texture>();
            nameToTexture = new Dictionary<string, Texture>();
            nameToNumber = new Dictionary<string, int>();

            var patches = LoadPatches(wad);

            for (var n = 1; n <= 2; n++)
            {
                var lumpNumber = wad.GetLumpNumber("TEXTURE" + n);
                if (lumpNumber == -1)
                {
                    break;
                }

                var data = wad.ReadLump(lumpNumber);
                var count = BitConverter.ToInt32(data, 0);
                for (var i = 0; i < count; i++)
                {
                    var offset = BitConverter.ToInt32(data, 4 + 4 * i);
                    var texture = Texture.FromData(data, offset, patches);
                    nameToNumber.Add(texture.Name, textures.Count);
                    textures.Add(texture);
                    nameToTexture.Add(texture.Name, texture);
                }
            }
        }

        public int GetNumber(string name)
        {
            if (name[0] == '-')
            {
                return 0;
            }

            return nameToNumber[name];
        }

        private static Patch[] LoadPatches(Wad wad)
        {
            var patchNames = LoadPatchNames(wad);
            var patches = new Patch[patchNames.Length];
            for (var i = 0; i < patches.Length; i++)
            {
                var name = patchNames[i];
                var data = wad.ReadLump(name);
                patches[i] = Patch.FromData(name, data);
            }
            return patches;
        }

        private static string[] LoadPatchNames(Wad wad)
        {
            var data = wad.ReadLump("PNAMES");
            var count = BitConverter.ToInt32(data, 0);
            var names = new string[count];
            for (var i = 0; i < names.Length; i++)
            {
                names[i] = DoomInterop.ToString(data, 4 + 8 * i, 8);
            }
            return names;
        }

        public IEnumerator<Texture> GetEnumerator()
        {
            return textures.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return textures.GetEnumerator();
        }

        public int Count => textures.Count;
        public Texture this[int num] => textures[num];
        public Texture this[string name] => nameToTexture[name];
    }
}
