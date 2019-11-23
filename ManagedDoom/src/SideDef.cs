using System;

namespace ManagedDoom
{
    public sealed class SideDef
    {
        public const int DataSize = 30;

        private Fixed textureOffset;
        private Fixed rowOffset;
        private int topTexture;
        private int bottomTexture;
        private int middleTexture;
        private Sector sector;

        public SideDef(
            Fixed textureOffset,
            Fixed rowOffset,
            int topTexture,
            int bottomTexture,
            int middleTexture,
            Sector sector)
        {
            this.textureOffset = textureOffset;
            this.rowOffset = rowOffset;
            this.topTexture = topTexture;
            this.bottomTexture = bottomTexture;
            this.middleTexture = middleTexture;
            this.sector = sector;
        }

        public static SideDef FromData(byte[] data, int offset, TextureLookup textures, Sector[] sectors)
        {
            var textureOffset = BitConverter.ToInt16(data, offset);
            var rowOffset = BitConverter.ToInt16(data, offset + 2);
            var topTextureName = DoomInterop.ToString(data, offset + 4, 8);
            var bottomTextureName = DoomInterop.ToString(data, offset + 12, 8);
            var middleTextureName = DoomInterop.ToString(data, offset + 20, 8);
            var sectorNum = BitConverter.ToInt16(data, offset + 28);

            return new SideDef(
                Fixed.FromInt(textureOffset),
                Fixed.FromInt(rowOffset),
                textures.GetNumber(topTextureName),
                textures.GetNumber(bottomTextureName),
                textures.GetNumber(middleTextureName),
                sectorNum != -1 ? sectors[sectorNum] : null);
        }

        public static SideDef[] FromWad(Wad wad, int lump, TextureLookup textures, Sector[] sectors)
        {
            var length = wad.GetLumpSize(lump);
            if (length % SideDef.DataSize != 0)
            {
                throw new Exception();
            }

            var data = wad.ReadLump(lump);
            var count = length / SideDef.DataSize;
            var sides = new SideDef[count]; ;

            for (var i = 0; i < count; i++)
            {
                var offset = SideDef.DataSize * i;
                sides[i] = SideDef.FromData(data, offset, textures, sectors);
            }

            return sides;
        }

        public Fixed TextureOffset => textureOffset;
        public Fixed RowOffset => rowOffset;
        public int TopTexture => topTexture;
        public int BottomTexture => bottomTexture;
        public int MiddleTexture => middleTexture;
        public Sector Sector => sector;
    }
}
