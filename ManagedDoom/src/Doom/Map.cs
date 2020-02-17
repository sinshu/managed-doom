using System;

namespace ManagedDoom
{
    public sealed class Map
    {
        private TextureLookup textures;
        private FlatLookup flats;

        private Vertex[] vertices;
        private Sector[] sectors;
        private SideDef[] sides;
        private LineDef[] lines;
        private Seg[] segs;
        private Subsector[] subsectors;
        private Node[] nodes;
        private Thing[] things;
        private BlockMap blockMap;

        private Texture skyTexture;

        public Map(Resources resorces, string name)
            : this(resorces.Wad, resorces.Textures, resorces.Flats, name)
        {
        }

        public Map(Wad wad, TextureLookup textures, FlatLookup flats, string name)
        {
            this.textures = textures;
            this.flats = flats;

            var map = wad.GetLumpNumber(name);
            vertices = Vertex.FromWad(wad, map + 4);
            sectors = Sector.FromWad(wad, map + 8, flats);
            sides = SideDef.FromWad(wad, map + 3, textures, sectors);
            lines = LineDef.FromWad(wad, map + 2, vertices, sides);
            segs = Seg.FromWad(wad, map + 5, vertices, lines);
            subsectors = Subsector.FromWad(wad, map + 6, segs);
            nodes = Node.FromWad(wad, map + 7, subsectors);
            things = Thing.FromWad(wad, map + 1);
            blockMap = BlockMap.FromWad(wad, map + 10, lines);

            skyTexture = GetSkyTextureByMapName(name);
        }

        private Texture GetSkyTextureByMapName(string name)
        {
            if (name.Length == 4)
            {
                return textures["SKY1"];
            }
            else
            {
                var number = int.Parse(name.Substring(3));
                if (number <= 11)
                {
                    return textures["SKY1"];
                }
                else if (number <= 21)
                {
                    return textures["SKY2"];
                }
                else
                {
                    return textures["SKY3"];
                }
            }
        }

        public Vertex[] Vertices => vertices;
        public Sector[] Sectors => sectors;
        public SideDef[] Sides => sides;
        public LineDef[] Lines => lines;
        public Seg[] Segs => segs;
        public Subsector[] Subsectors => subsectors;
        public Node[] Nodes => nodes;
        public Thing[] Things => things;
        public BlockMap BlockMap => blockMap;
        public Texture SkyTexture => skyTexture;
        public int SkyFlatNumber => flats.SkyFlatNumber;
    }
}
