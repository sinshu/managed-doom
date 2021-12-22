using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.UnitTests
{
    [TestClass]
    public class SubsectorTest
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
                var segs = Seg.FromWad(wad, map + 5, vertices, lines);
                var subsectors = Subsector.FromWad(wad, map + 6, segs);

                Assert.AreEqual(239, subsectors.Length);

                Assert.AreEqual(8, subsectors[0].SegCount);
                for (var i = 0; i < 8; i++)
                {
                    Assert.IsTrue(segs[subsectors[0].FirstSeg + i] == segs[0 + i]);
                }

                Assert.AreEqual(1, subsectors[54].SegCount);
                for (var i = 0; i < 1; i++)
                {
                    Assert.IsTrue(segs[subsectors[54].FirstSeg + i] == segs[181 + i]);
                }

                Assert.AreEqual(2, subsectors[238].SegCount);
                for (var i = 0; i < 2; i++)
                {
                    Assert.IsTrue(segs[subsectors[238].FirstSeg + i] == segs[745 + i]);
                }
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
                var segs = Seg.FromWad(wad, map + 5, vertices, lines);
                var subsectors = Subsector.FromWad(wad, map + 6, segs);

                Assert.AreEqual(194, subsectors.Length);

                Assert.AreEqual(4, subsectors[0].SegCount);
                for (var i = 0; i < 4; i++)
                {
                    Assert.IsTrue(segs[subsectors[0].FirstSeg + i] == segs[i]);
                }

                Assert.AreEqual(4, subsectors[57].SegCount);
                for (var i = 0; i < 4; i++)
                {
                    Assert.IsTrue(segs[subsectors[57].FirstSeg + i] == segs[179 + i]);
                }

                Assert.AreEqual(4, subsectors[193].SegCount);
                for (var i = 0; i < 4; i++)
                {
                    Assert.IsTrue(segs[subsectors[193].FirstSeg + i] == segs[597 + i]);
                }
            }
        }
    }
}
