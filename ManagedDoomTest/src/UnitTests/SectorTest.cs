using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.UnitTests
{
    [TestClass]
    public class SectorTest
    {
        private const double delta = 1.0E-9;

        [TestMethod]
        public void LoadE1M1()
        {
            using (var wad = new Wad(WadPath.Doom1))
            {
                var map = wad.GetLumpNumber("E1M1");
                var flats = new FlatLookup(wad);
                var sectors = Sector.FromWad(wad, map + 8, flats);

                Assert.AreEqual(88, sectors.Length);

                Assert.AreEqual(-80, sectors[0].FloorHeight.ToDouble(), delta);
                Assert.AreEqual(216, sectors[0].CeilingHeight.ToDouble(), delta);
                Assert.AreEqual("NUKAGE3", flats[sectors[0].FloorFlat].Name);
                Assert.AreEqual("F_SKY1", flats[sectors[0].CeilingFlat].Name);
                Assert.AreEqual(255, sectors[0].LightLevel);
                Assert.AreEqual((SectorSpecial)7, sectors[0].Special);
                Assert.AreEqual(0, sectors[0].Tag);

                Assert.AreEqual(0, sectors[42].FloorHeight.ToDouble(), delta);
                Assert.AreEqual(264, sectors[42].CeilingHeight.ToDouble(), delta);
                Assert.AreEqual("FLOOR7_1", flats[sectors[42].FloorFlat].Name);
                Assert.AreEqual("F_SKY1", flats[sectors[42].CeilingFlat].Name);
                Assert.AreEqual(255, sectors[42].LightLevel);
                Assert.AreEqual((SectorSpecial)0, sectors[42].Special);
                Assert.AreEqual(0, sectors[42].Tag);

                Assert.AreEqual(104, sectors[87].FloorHeight.ToDouble(), delta);
                Assert.AreEqual(184, sectors[87].CeilingHeight.ToDouble(), delta);
                Assert.AreEqual("FLOOR4_8", flats[sectors[87].FloorFlat].Name);
                Assert.AreEqual("FLOOR6_2", flats[sectors[87].CeilingFlat].Name);
                Assert.AreEqual(128, sectors[87].LightLevel);
                Assert.AreEqual((SectorSpecial)9, sectors[87].Special);
                Assert.AreEqual(2, sectors[87].Tag);
            }
        }

        [TestMethod]
        public void LoadMap01()
        {
            using (var wad = new Wad(WadPath.Doom2))
            {
                var map = wad.GetLumpNumber("MAP01");
                var flats = new FlatLookup(wad);
                var sectors = Sector.FromWad(wad, map + 8, flats);

                Assert.AreEqual(59, sectors.Length);

                Assert.AreEqual(8, sectors[0].FloorHeight.ToDouble(), delta);
                Assert.AreEqual(264, sectors[0].CeilingHeight.ToDouble(), delta);
                Assert.AreEqual("FLOOR0_1", flats[sectors[0].FloorFlat].Name);
                Assert.AreEqual("FLOOR4_1", flats[sectors[0].CeilingFlat].Name);
                Assert.AreEqual(128, sectors[0].LightLevel);
                Assert.AreEqual(SectorSpecial.Normal, sectors[0].Special);
                Assert.AreEqual(0, sectors[0].Tag);

                Assert.AreEqual(56, sectors[57].FloorHeight.ToDouble(), delta);
                Assert.AreEqual(184, sectors[57].CeilingHeight.ToDouble(), delta);
                Assert.AreEqual("FLOOR3_3", flats[sectors[57].FloorFlat].Name);
                Assert.AreEqual("CEIL3_3", flats[sectors[57].CeilingFlat].Name);
                Assert.AreEqual(144, sectors[57].LightLevel);
                Assert.AreEqual((SectorSpecial)9, sectors[57].Special);
                Assert.AreEqual(0, sectors[57].Tag);

                Assert.AreEqual(56, sectors[58].FloorHeight.ToDouble(), delta);
                Assert.AreEqual(56, sectors[58].CeilingHeight.ToDouble(), delta);
                Assert.AreEqual("FLOOR3_3", flats[sectors[58].FloorFlat].Name);
                Assert.AreEqual("FLAT20", flats[sectors[58].CeilingFlat].Name);
                Assert.AreEqual(144, sectors[58].LightLevel);
                Assert.AreEqual(SectorSpecial.Normal, sectors[58].Special);
                Assert.AreEqual(6, sectors[58].Tag);
            }
        }
    }
}
