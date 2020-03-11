using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.UnitTests
{
    [TestClass]
    public class VertexTest
    {
        private const double delta = 1.0E-9;

        [TestMethod]
        public void LoadE1M1()
        {
            using (var wad = new Wad(WadPath.Doom1))
            {
                var map = wad.GetLumpNumber("E1M1");
                var vertices = Vertex.FromWad(wad, map + 4);

                Assert.AreEqual(470, vertices.Length);

                Assert.AreEqual(1088, vertices[0].X.ToDouble(), delta);
                Assert.AreEqual(-3680, vertices[0].Y.ToDouble(), delta);

                Assert.AreEqual(128, vertices[57].X.ToDouble(), delta);
                Assert.AreEqual(-3008, vertices[57].Y.ToDouble(), delta);

                Assert.AreEqual(2435, vertices[469].X.ToDouble(), delta);
                Assert.AreEqual(-3920, vertices[469].Y.ToDouble(), delta);
            }
        }

        [TestMethod]
        public void LoadMap01()
        {
            using (var wad = new Wad(WadPath.Doom2))
            {
                var map = wad.GetLumpNumber("MAP01");
                var vertices = Vertex.FromWad(wad, map + 4);

                Assert.AreEqual(383, vertices.Length);

                Assert.AreEqual(-448, vertices[0].X.ToDouble(), delta);
                Assert.AreEqual(768, vertices[0].Y.ToDouble(), delta);

                Assert.AreEqual(128, vertices[57].X.ToDouble(), delta);
                Assert.AreEqual(1808, vertices[57].Y.ToDouble(), delta);

                Assert.AreEqual(-64, vertices[382].X.ToDouble(), delta);
                Assert.AreEqual(2240, vertices[382].Y.ToDouble(), delta);
            }
        }
    }
}
