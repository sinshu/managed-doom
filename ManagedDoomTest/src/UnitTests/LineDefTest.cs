using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.UnitTests
{
    [TestClass]
    public class LineDefTest
    {
        [TestMethod]
        public void LoadE1M1()
        {
            using (var wad = new Wad(WadPath.Doom1))
            {
                var flats = new DummyFlatLookup(wad);
                var textures = new DummyTextureLookup(wad);
                var map = wad.GetLumpNumber("E1M1");
                var vertices = Vertex.FromWad(wad, map + 4);
                var sectors = Sector.FromWad(wad, map + 8, flats);
                var sides = SideDef.FromWad(wad, map + 3, textures, sectors);
                var lines = LineDef.FromWad(wad, map + 2, vertices, sides);

                Assert.AreEqual(486, lines.Length);

                Assert.IsTrue(lines[0].Vertex1 == vertices[0]);
                Assert.IsTrue(lines[0].Vertex2 == vertices[1]);
                Assert.IsTrue((int)lines[0].Flags == 1);
                Assert.IsTrue((int)lines[0].Special == 0);
                Assert.IsTrue(lines[0].Tag == 0);
                Assert.IsTrue(lines[0].FrontSide == sides[0]);
                Assert.IsTrue(lines[0].BackSide == null);
                Assert.IsTrue(lines[0].FrontSector == sides[0].Sector);
                Assert.IsTrue(lines[0].BackSector == null);

                Assert.IsTrue(lines[136].Vertex1 == vertices[110]);
                Assert.IsTrue(lines[136].Vertex2 == vertices[111]);
                Assert.IsTrue((int)lines[136].Flags == 28);
                Assert.IsTrue((int)lines[136].Special == 63);
                Assert.IsTrue(lines[136].Tag == 3);
                Assert.IsTrue(lines[136].FrontSide == sides[184]);
                Assert.IsTrue(lines[136].BackSide == sides[185]);
                Assert.IsTrue(lines[136].FrontSector == sides[184].Sector);
                Assert.IsTrue(lines[136].BackSector == sides[185].Sector);

                Assert.IsTrue(lines[485].Vertex1 == vertices[309]);
                Assert.IsTrue(lines[485].Vertex2 == vertices[294]);
                Assert.IsTrue((int)lines[485].Flags == 12);
                Assert.IsTrue((int)lines[485].Special == 0);
                Assert.IsTrue(lines[485].Tag == 0);
                Assert.IsTrue(lines[485].FrontSide == sides[664]);
                Assert.IsTrue(lines[485].BackSide == sides[665]);
                Assert.IsTrue(lines[485].FrontSector == sides[664].Sector);
                Assert.IsTrue(lines[485].BackSector == sides[665].Sector);
            }
        }

        [TestMethod]
        public void LoadMap01()
        {
            using (var wad = new Wad(WadPath.Doom2))
            {
                var flats = new DummyFlatLookup(wad);
                var textures = new DummyTextureLookup(wad);
                var map = wad.GetLumpNumber("MAP01");
                var vertices = Vertex.FromWad(wad, map + 4);
                var sectors = Sector.FromWad(wad, map + 8, flats);
                var sides = SideDef.FromWad(wad, map + 3, textures, sectors);
                var lines = LineDef.FromWad(wad, map + 2, vertices, sides);

                Assert.AreEqual(370, lines.Length);

                Assert.IsTrue(lines[0].Vertex1 == vertices[0]);
                Assert.IsTrue(lines[0].Vertex2 == vertices[1]);
                Assert.IsTrue((int)lines[0].Flags == 1);
                Assert.IsTrue((int)lines[0].Special == 0);
                Assert.IsTrue(lines[0].Tag == 0);
                Assert.IsTrue(lines[0].FrontSide == sides[0]);
                Assert.IsTrue(lines[0].BackSide == null);
                Assert.IsTrue(lines[0].FrontSector == sides[0].Sector);
                Assert.IsTrue(lines[0].BackSector == null);

                Assert.IsTrue(lines[75].Vertex1 == vertices[73]);
                Assert.IsTrue(lines[75].Vertex2 == vertices[74]);
                Assert.IsTrue((int)lines[75].Flags == 4);
                Assert.IsTrue((int)lines[75].Special == 103);
                Assert.IsTrue(lines[75].Tag == 4);
                Assert.IsTrue(lines[75].FrontSide == sides[97]);
                Assert.IsTrue(lines[75].BackSide == sides[98]);
                Assert.IsTrue(lines[75].FrontSector == sides[97].Sector);
                Assert.IsTrue(lines[75].BackSector == sides[98].Sector);

                Assert.IsTrue(lines[369].Vertex1 == vertices[293]);
                Assert.IsTrue(lines[369].Vertex2 == vertices[299]);
                Assert.IsTrue((int)lines[369].Flags == 21);
                Assert.IsTrue((int)lines[369].Special == 0);
                Assert.IsTrue(lines[369].Tag == 0);
                Assert.IsTrue(lines[369].FrontSide == sides[527]);
                Assert.IsTrue(lines[369].BackSide == sides[528]);
                Assert.IsTrue(lines[369].FrontSector == sides[527].Sector);
                Assert.IsTrue(lines[369].BackSector == sides[528].Sector);
            }
        }
    }
}
