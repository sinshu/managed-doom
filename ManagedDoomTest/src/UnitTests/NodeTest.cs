using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.UnitTests
{
    [TestClass]
    public class NodeTest
    {
        private const double delta = 1.0E-9;

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
                var nodes = Node.FromWad(wad, map + 7, subsectors);

                Assert.AreEqual(238, nodes.Length);

                Assert.AreEqual(1784, nodes[0].X.ToDouble(), delta);
                Assert.AreEqual(-3448, nodes[0].Y.ToDouble(), delta);
                Assert.AreEqual(-240, nodes[0].Dx.ToDouble(), delta);
                Assert.AreEqual(64, nodes[0].Dy.ToDouble(), delta);
                Assert.AreEqual(-3104, nodes[0].BoundingBox[0][Box.Top].ToDouble(), delta);
                Assert.AreEqual(-3448, nodes[0].BoundingBox[0][Box.Bottom].ToDouble(), delta);
                Assert.AreEqual(1520, nodes[0].BoundingBox[0][Box.Left].ToDouble(), delta);
                Assert.AreEqual(2128, nodes[0].BoundingBox[0][Box.Right].ToDouble(), delta);
                Assert.AreEqual(-3384, nodes[0].BoundingBox[1][Box.Top].ToDouble(), delta);
                Assert.AreEqual(-3448, nodes[0].BoundingBox[1][Box.Bottom].ToDouble(), delta);
                Assert.AreEqual(1544, nodes[0].BoundingBox[1][Box.Left].ToDouble(), delta);
                Assert.AreEqual(1784, nodes[0].BoundingBox[1][Box.Right].ToDouble(), delta);
                Assert.AreEqual(32768, nodes[0].Children[0] + 0x10000);
                Assert.AreEqual(32769, nodes[0].Children[1] + 0x10000);

                Assert.AreEqual(928, nodes[57].X.ToDouble(), delta);
                Assert.AreEqual(-3360, nodes[57].Y.ToDouble(), delta);
                Assert.AreEqual(0, nodes[57].Dx.ToDouble(), delta);
                Assert.AreEqual(256, nodes[57].Dy.ToDouble(), delta);
                Assert.AreEqual(-3104, nodes[57].BoundingBox[0][Box.Top].ToDouble(), delta);
                Assert.AreEqual(-3360, nodes[57].BoundingBox[0][Box.Bottom].ToDouble(), delta);
                Assert.AreEqual(928, nodes[57].BoundingBox[0][Box.Left].ToDouble(), delta);
                Assert.AreEqual(1344, nodes[57].BoundingBox[0][Box.Right].ToDouble(), delta);
                Assert.AreEqual(-3104, nodes[57].BoundingBox[1][Box.Top].ToDouble(), delta);
                Assert.AreEqual(-3360, nodes[57].BoundingBox[1][Box.Bottom].ToDouble(), delta);
                Assert.AreEqual(704, nodes[57].BoundingBox[1][Box.Left].ToDouble(), delta);
                Assert.AreEqual(928, nodes[57].BoundingBox[1][Box.Right].ToDouble(), delta);
                Assert.AreEqual(32825, nodes[57].Children[0] + 0x10000);
                Assert.AreEqual(56, nodes[57].Children[1]);

                Assert.AreEqual(2176, nodes[237].X.ToDouble(), delta);
                Assert.AreEqual(-2304, nodes[237].Y.ToDouble(), delta);
                Assert.AreEqual(0, nodes[237].Dx.ToDouble(), delta);
                Assert.AreEqual(-256, nodes[237].Dy.ToDouble(), delta);
                Assert.AreEqual(-2048, nodes[237].BoundingBox[0][Box.Top].ToDouble(), delta);
                Assert.AreEqual(-4064, nodes[237].BoundingBox[0][Box.Bottom].ToDouble(), delta);
                Assert.AreEqual(-768, nodes[237].BoundingBox[0][Box.Left].ToDouble(), delta);
                Assert.AreEqual(2176, nodes[237].BoundingBox[0][Box.Right].ToDouble(), delta);
                Assert.AreEqual(-2048, nodes[237].BoundingBox[1][Box.Top].ToDouble(), delta);
                Assert.AreEqual(-4864, nodes[237].BoundingBox[1][Box.Bottom].ToDouble(), delta);
                Assert.AreEqual(2176, nodes[237].BoundingBox[1][Box.Left].ToDouble(), delta);
                Assert.AreEqual(3808, nodes[237].BoundingBox[1][Box.Right].ToDouble(), delta);
                Assert.AreEqual(131, nodes[237].Children[0]);
                Assert.AreEqual(236, nodes[237].Children[1]);
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
                var nodes = Node.FromWad(wad, map + 7, subsectors);

                Assert.AreEqual(193, nodes.Length);

                Assert.AreEqual(64, nodes[0].X.ToDouble(), delta);
                Assert.AreEqual(1024, nodes[0].Y.ToDouble(), delta);
                Assert.AreEqual(0, nodes[0].Dx.ToDouble(), delta);
                Assert.AreEqual(-64, nodes[0].Dy.ToDouble(), delta);
                Assert.AreEqual(1173, nodes[0].BoundingBox[0][Box.Top].ToDouble(), delta);
                Assert.AreEqual(960, nodes[0].BoundingBox[0][Box.Bottom].ToDouble(), delta);
                Assert.AreEqual(-64, nodes[0].BoundingBox[0][Box.Left].ToDouble(), delta);
                Assert.AreEqual(64, nodes[0].BoundingBox[0][Box.Right].ToDouble(), delta);
                Assert.AreEqual(1280, nodes[0].BoundingBox[1][Box.Top].ToDouble(), delta);
                Assert.AreEqual(1024, nodes[0].BoundingBox[1][Box.Bottom].ToDouble(), delta);
                Assert.AreEqual(64, nodes[0].BoundingBox[1][Box.Left].ToDouble(), delta);
                Assert.AreEqual(128, nodes[0].BoundingBox[1][Box.Right].ToDouble(), delta);
                Assert.AreEqual(32770, nodes[0].Children[0] + 0x10000);
                Assert.AreEqual(32771, nodes[0].Children[1] + 0x10000);

                Assert.AreEqual(640, nodes[57].X.ToDouble(), delta);
                Assert.AreEqual(856, nodes[57].Y.ToDouble(), delta);
                Assert.AreEqual(-88, nodes[57].Dx.ToDouble(), delta);
                Assert.AreEqual(-16, nodes[57].Dy.ToDouble(), delta);
                Assert.AreEqual(856, nodes[57].BoundingBox[0][Box.Top].ToDouble(), delta);
                Assert.AreEqual(840, nodes[57].BoundingBox[0][Box.Bottom].ToDouble(), delta);
                Assert.AreEqual(552, nodes[57].BoundingBox[0][Box.Left].ToDouble(), delta);
                Assert.AreEqual(640, nodes[57].BoundingBox[0][Box.Right].ToDouble(), delta);
                Assert.AreEqual(856, nodes[57].BoundingBox[1][Box.Top].ToDouble(), delta);
                Assert.AreEqual(760, nodes[57].BoundingBox[1][Box.Bottom].ToDouble(), delta);
                Assert.AreEqual(536, nodes[57].BoundingBox[1][Box.Left].ToDouble(), delta);
                Assert.AreEqual(704, nodes[57].BoundingBox[1][Box.Right].ToDouble(), delta);
                Assert.AreEqual(32829, nodes[57].Children[0] + 0x10000);
                Assert.AreEqual(56, nodes[57].Children[1]);

                Assert.AreEqual(96, nodes[192].X.ToDouble(), delta);
                Assert.AreEqual(1280, nodes[192].Y.ToDouble(), delta);
                Assert.AreEqual(32, nodes[192].Dx.ToDouble(), delta);
                Assert.AreEqual(0, nodes[192].Dy.ToDouble(), delta);
                Assert.AreEqual(1280, nodes[192].BoundingBox[0][Box.Top].ToDouble(), delta);
                Assert.AreEqual(-960, nodes[192].BoundingBox[0][Box.Bottom].ToDouble(), delta);
                Assert.AreEqual(-1304, nodes[192].BoundingBox[0][Box.Left].ToDouble(), delta);
                Assert.AreEqual(2072, nodes[192].BoundingBox[0][Box.Right].ToDouble(), delta);
                Assert.AreEqual(2688, nodes[192].BoundingBox[1][Box.Top].ToDouble(), delta);
                Assert.AreEqual(1280, nodes[192].BoundingBox[1][Box.Bottom].ToDouble(), delta);
                Assert.AreEqual(-1304, nodes[192].BoundingBox[1][Box.Left].ToDouble(), delta);
                Assert.AreEqual(2072, nodes[192].BoundingBox[1][Box.Right].ToDouble(), delta);
                Assert.AreEqual(147, nodes[192].Children[0]);
                Assert.AreEqual(191, nodes[192].Children[1]);
            }
        }
    }
}
