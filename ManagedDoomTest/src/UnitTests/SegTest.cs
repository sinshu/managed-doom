using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.UnitTests
{
    [TestClass]
    public class SegTest
    {
        private const double delta = 1.0E-9;

        private static double ToRadian(int angle)
        {
            if (angle < 0)
            {
                angle += 0x10000;
            }
            return 2 * Math.PI * ((double)angle / 0x10000);
        }

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

                Assert.AreEqual(747, segs.Length);

                Assert.IsTrue(segs[0].Vertex1 == vertices[132]);
                Assert.IsTrue(segs[0].Vertex2 == vertices[133]);
                Assert.AreEqual(ToRadian(4156), segs[0].Angle.ToRadian(), delta);
                Assert.IsTrue(segs[0].LineDef == lines[160]);
                Assert.IsTrue((segs[0].LineDef.Flags & LineFlags.TwoSided) != 0);
                Assert.IsTrue(segs[0].FrontSector == segs[0].LineDef.FrontSide.Sector);
                Assert.IsTrue(segs[0].BackSector == segs[0].LineDef.BackSide.Sector);
                Assert.AreEqual(0, segs[0].Offset.ToDouble(), delta);

                Assert.IsTrue(segs[28].Vertex1 == vertices[390]);
                Assert.IsTrue(segs[28].Vertex2 == vertices[131]);
                Assert.AreEqual(ToRadian(-32768), segs[28].Angle.ToRadian(), delta);
                Assert.IsTrue(segs[28].LineDef == lines[480]);
                Assert.IsTrue((segs[0].LineDef.Flags & LineFlags.TwoSided) != 0);
                Assert.IsTrue(segs[28].FrontSector == segs[28].LineDef.BackSide.Sector);
                Assert.IsTrue(segs[28].BackSector == segs[28].LineDef.FrontSide.Sector);
                Assert.AreEqual(0, segs[28].Offset.ToDouble(), delta);

                Assert.IsTrue(segs[744].Vertex1 == vertices[446]);
                Assert.IsTrue(segs[744].Vertex2 == vertices[374]);
                Assert.AreEqual(ToRadian(-16384), segs[744].Angle.ToRadian(), delta);
                Assert.IsTrue(segs[744].LineDef == lines[452]);
                Assert.IsTrue((segs[744].LineDef.Flags & LineFlags.TwoSided) == 0);
                Assert.IsTrue(segs[744].FrontSector == segs[744].LineDef.FrontSide.Sector);
                Assert.IsTrue(segs[744].BackSector == null);
                Assert.AreEqual(154, segs[744].Offset.ToDouble(), delta);

                Assert.IsTrue(segs[746].Vertex1 == vertices[374]);
                Assert.IsTrue(segs[746].Vertex2 == vertices[368]);
                Assert.AreEqual(ToRadian(-13828), segs[746].Angle.ToRadian(), delta);
                Assert.IsTrue(segs[746].LineDef == lines[451]);
                Assert.IsTrue((segs[746].LineDef.Flags & LineFlags.TwoSided) == 0);
                Assert.IsTrue(segs[746].FrontSector == segs[746].LineDef.FrontSide.Sector);
                Assert.IsTrue(segs[746].BackSector == null);
                Assert.AreEqual(0, segs[746].Offset.ToDouble(), delta);
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

                Assert.AreEqual(601, segs.Length);

                Assert.IsTrue(segs[0].Vertex1 == vertices[9]);
                Assert.IsTrue(segs[0].Vertex2 == vertices[316]);
                Assert.AreEqual(ToRadian(-32768), segs[0].Angle.ToRadian(), delta);
                Assert.IsTrue(segs[0].LineDef == lines[8]);
                Assert.IsTrue((segs[0].LineDef.Flags & LineFlags.TwoSided) != 0);
                Assert.IsTrue(segs[0].FrontSector == segs[0].LineDef.FrontSide.Sector);
                Assert.IsTrue(segs[0].BackSector == segs[0].LineDef.BackSide.Sector);
                Assert.AreEqual(0, segs[0].Offset.ToDouble(), delta);

                Assert.IsTrue(segs[42].Vertex1 == vertices[26]);
                Assert.IsTrue(segs[42].Vertex2 == vertices[320]);
                Assert.AreEqual(ToRadian(-22209), segs[42].Angle.ToRadian(), delta);
                Assert.IsTrue(segs[42].LineDef == lines[33]);
                Assert.IsTrue((segs[42].LineDef.Flags & LineFlags.TwoSided) != 0);
                Assert.IsTrue(segs[42].FrontSector == segs[42].LineDef.BackSide.Sector);
                Assert.IsTrue(segs[42].BackSector == segs[42].LineDef.FrontSide.Sector);
                Assert.AreEqual(0, segs[42].Offset.ToDouble(), delta);

                Assert.IsTrue(segs[103].Vertex1 == vertices[331]);
                Assert.IsTrue(segs[103].Vertex2 == vertices[329]);
                Assert.AreEqual(ToRadian(16384), segs[103].Angle.ToRadian(), delta);
                Assert.IsTrue(segs[103].LineDef == lines[347]);
                Assert.IsTrue((segs[103].LineDef.Flags & LineFlags.TwoSided) == 0);
                Assert.IsTrue(segs[103].FrontSector == segs[103].LineDef.FrontSide.Sector);
                Assert.IsTrue(segs[103].BackSector == null);
                Assert.AreEqual(64, segs[103].Offset.ToDouble(), delta);

                Assert.IsTrue(segs[600].Vertex1 == vertices[231]);
                Assert.IsTrue(segs[600].Vertex2 == vertices[237]);
                Assert.AreEqual(ToRadian(-16384), segs[600].Angle.ToRadian(), delta);
                Assert.IsTrue(segs[600].LineDef == lines[271]);
                Assert.IsTrue((segs[600].LineDef.Flags & LineFlags.TwoSided) != 0);
                Assert.IsTrue(segs[600].FrontSector == segs[600].LineDef.BackSide.Sector);
                Assert.IsTrue(segs[600].BackSector == segs[600].LineDef.FrontSide.Sector);
                Assert.AreEqual(0, segs[600].Offset.ToDouble(), delta);
            }
        }
    }
}
