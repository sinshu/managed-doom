using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.UnitTests
{
    [TestClass]
    public class SideDefTest
    {
        private const double delta = 1.0E-9;

        [TestMethod]
        public void LoadE1M1()
        {
            using (var wad = new Wad(WadPath.Doom1))
            {
                var flats = new DummyFlatLookup(wad);
                var textures = new TextureLookup(wad);
                var map = wad.GetLumpNumber("E1M1");
                var vertices = Vertex.FromWad(wad, map + 4);
                var sectors = Sector.FromWad(wad, map + 8, flats);
                var sides = SideDef.FromWad(wad, map + 3, textures, sectors);

                Assert.AreEqual(666, sides.Length);

                Assert.AreEqual(0, sides[0].TextureOffset.ToDouble(), delta);
                Assert.AreEqual(0, sides[0].RowOffset.ToDouble(), delta);
                Assert.AreEqual(0, sides[0].TopTexture);
                Assert.AreEqual(0, sides[0].BottomTexture);
                Assert.AreEqual("DOOR3", textures[sides[0].MiddleTexture].Name);
                Assert.IsTrue(sides[0].Sector == sectors[30]);

                Assert.AreEqual(32, sides[480].TextureOffset.ToDouble(), delta);
                Assert.AreEqual(0, sides[480].RowOffset.ToDouble(), delta);
                Assert.AreEqual("EXITSIGN", textures[sides[480].TopTexture].Name);
                Assert.AreEqual(0, sides[480].BottomTexture);
                Assert.AreEqual(0, sides[480].MiddleTexture);
                Assert.IsTrue(sides[480].Sector == sectors[70]);

                Assert.AreEqual(0, sides[650].TextureOffset.ToDouble(), delta);
                Assert.AreEqual(88, sides[650].RowOffset.ToDouble(), delta);
                Assert.AreEqual("STARTAN3", textures[sides[650].TopTexture].Name);
                Assert.AreEqual("STARTAN3", textures[sides[650].BottomTexture].Name);
                Assert.AreEqual(0, sides[650].MiddleTexture);
                Assert.IsTrue(sides[650].Sector == sectors[1]);

                Assert.AreEqual(0, sides[665].TextureOffset.ToDouble(), delta);
                Assert.AreEqual(0, sides[665].RowOffset.ToDouble(), delta);
                Assert.AreEqual(0, sides[665].TopTexture);
                Assert.AreEqual(0, sides[665].BottomTexture);
                Assert.AreEqual(0, sides[665].MiddleTexture);
                Assert.IsTrue(sides[665].Sector == sectors[23]);
            }
        }

        [TestMethod]
        public void LoadMap01()
        {
            using (var wad = new Wad(WadPath.Doom2))
            {
                var flats = new DummyFlatLookup(wad);
                var textures = new TextureLookup(wad);
                var map = wad.GetLumpNumber("MAP01");
                var vertices = Vertex.FromWad(wad, map + 4);
                var sectors = Sector.FromWad(wad, map + 8, flats);
                var sides = SideDef.FromWad(wad, map + 3, textures, sectors);

                Assert.AreEqual(529, sides.Length);

                Assert.AreEqual(0, sides[0].TextureOffset.ToDouble(), delta);
                Assert.AreEqual(0, sides[0].RowOffset.ToDouble(), delta);
                Assert.AreEqual(0, sides[0].TopTexture);
                Assert.AreEqual(0, sides[0].BottomTexture);
                Assert.AreEqual("BRONZE1", textures[sides[0].MiddleTexture].Name);
                Assert.IsTrue(sides[0].Sector == sectors[9]);

                Assert.AreEqual(0, sides[312].TextureOffset.ToDouble(), delta);
                Assert.AreEqual(0, sides[312].RowOffset.ToDouble(), delta);
                Assert.AreEqual(0, sides[312].TopTexture);
                Assert.AreEqual(0, sides[312].BottomTexture);
                Assert.AreEqual("DOORTRAK", textures[sides[312].MiddleTexture].Name);
                Assert.IsTrue(sides[312].Sector == sectors[31]);

                Assert.AreEqual(24, sides[512].TextureOffset.ToDouble(), delta);
                Assert.AreEqual(0, sides[512].RowOffset.ToDouble(), delta);
                Assert.AreEqual(0, sides[512].TopTexture);
                Assert.AreEqual(0, sides[512].BottomTexture);
                Assert.AreEqual("SUPPORT2", textures[sides[512].MiddleTexture].Name);
                Assert.IsTrue(sides[512].Sector == sectors[52]);

                Assert.AreEqual(0, sides[528].TextureOffset.ToDouble(), delta);
                Assert.AreEqual(0, sides[528].RowOffset.ToDouble(), delta);
                Assert.AreEqual(0, sides[528].TopTexture);
                Assert.AreEqual(0, sides[528].BottomTexture);
                Assert.AreEqual(0, sides[528].MiddleTexture);
                Assert.IsTrue(sides[528].Sector == sectors[11]);
            }
        }
    }
}
