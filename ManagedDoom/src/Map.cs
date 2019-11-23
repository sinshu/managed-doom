using System;

namespace ManagedDoom
{
    public sealed class Map
    {
        private FlatLookup flats;
        private TextureLookup textures;
        private Vertex[] vertices;
        private Sector[] sectors;
        private SideDef[] sides;
        private LineDef[] lines;
        private Seg[] segs;
        private Subsector[] subsectors;
        private Node[] nodes;
        private Thing[] things;

        public Map(Wad wad, string name)
        {
            flats = new FlatLookup(wad);
            textures = new TextureLookup(wad);

            var map = wad.GetLumpNumber(name);
            vertices = Vertex.FromWad(wad, map + 4);
            sectors = Sector.FromWad(wad, map + 8, flats);
            sides = SideDef.FromWad(wad, map + 3, textures, sectors);
            lines = LineDef.FromWad(wad, map + 2, vertices, sides);
            segs = Seg.FromWad(wad, map + 5, vertices, lines);
            subsectors = Subsector.FromWad(wad, map + 6, segs);
            nodes = Node.FromWad(wad, map + 7, subsectors);
            things = Thing.FromWad(wad, map + 1);
        }

        public FlatLookup Flats => flats;
        public TextureLookup Textures => textures;
        public Vertex[] Vertices => vertices;
        public Sector[] Sectors => sectors;
        public SideDef[] Sides => sides;
        public LineDef[] Lines => lines;
        public Seg[] Segs => segs;
        public Subsector[] Subsectors => subsectors;
        public Node[] Nodes => nodes;
        public Thing[] Things => things;
    }
}
