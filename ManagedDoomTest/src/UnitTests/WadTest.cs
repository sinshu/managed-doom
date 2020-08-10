using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.UnitTests
{
    [TestClass]
    public class WadTest
    {
        [TestMethod]
        public void LumpNumberDoom1()
        {
            using (var wad = new Wad(WadPath.Doom1))
            {
                Assert.AreEqual(0, wad.GetLumpNumber("PLAYPAL"));
                Assert.AreEqual(1, wad.GetLumpNumber("COLORMAP"));
                Assert.AreEqual(7, wad.GetLumpNumber("E1M1"));
                Assert.AreEqual(2305, wad.GetLumpNumber("F_END"));
                Assert.AreEqual(2306, wad.LumpInfos.Count);
            }
        }

        [TestMethod]
        public void LumpNumberDoom2()
        {
            using (var wad = new Wad(WadPath.Doom2))
            {
                Assert.AreEqual(0, wad.GetLumpNumber("PLAYPAL"));
                Assert.AreEqual(1, wad.GetLumpNumber("COLORMAP"));
                Assert.AreEqual(6, wad.GetLumpNumber("MAP01"));
                Assert.AreEqual(2918, wad.GetLumpNumber("F_END"));
                Assert.AreEqual(2919, wad.LumpInfos.Count);
            }
        }

        [TestMethod]
        public void FlatSizeDoom1()
        {
            using (var wad = new Wad(WadPath.Doom1))
            {
                var start = wad.GetLumpNumber("F_START") + 1;
                var end = wad.GetLumpNumber("F_END");
                for (var lump = start; lump < end; lump++)
                {
                    var size = wad.GetLumpSize(lump);
                    Assert.IsTrue(size == 0 || size == 4096);
                }
            }
        }

        [TestMethod]
        public void FlatSizeDoom2()
        {
            using (var wad = new Wad(WadPath.Doom2))
            {
                var start = wad.GetLumpNumber("F_START") + 1;
                var end = wad.GetLumpNumber("F_END");
                for (var lump = start; lump < end; lump++)
                {
                    var size = wad.GetLumpSize(lump);
                    Assert.IsTrue(size == 0 || size == 4096);
                }
            }
        }
    }
}
