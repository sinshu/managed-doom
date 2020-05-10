using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.UnitTests
{
    [TestClass]
    public class ThingTest
    {
        private const double delta = 1.0E-9;

        [TestMethod]
        public void LoadE1M1()
        {
            using (var wad = new Wad(WadPath.Doom1))
            {
                var map = wad.GetLumpNumber("E1M1");
                var things = MapThing.FromWad(wad, map + 1);

                Assert.AreEqual(143, things.Length);

                Assert.AreEqual(1056, things[0].X.ToDouble(), delta);
                Assert.AreEqual(-3616, things[0].Y.ToDouble(), delta);
                Assert.AreEqual(90, things[0].Angle.ToDegree(), delta);
                Assert.AreEqual(1, things[0].Type);
                Assert.AreEqual(7, (int)things[0].Flags);

                Assert.AreEqual(3072, things[57].X.ToDouble(), delta);
                Assert.AreEqual(-4832, things[57].Y.ToDouble(), delta);
                Assert.AreEqual(180, things[57].Angle.ToDegree(), delta);
                Assert.AreEqual(2015, things[57].Type);
                Assert.AreEqual(7, (int)things[57].Flags);

                Assert.AreEqual(736, things[142].X.ToDouble(), delta);
                Assert.AreEqual(-2976, things[142].Y.ToDouble(), delta);
                Assert.AreEqual(90, things[142].Angle.ToDegree(), delta);
                Assert.AreEqual(2001, things[142].Type);
                Assert.AreEqual(23, (int)things[142].Flags);
            }
        }

        [TestMethod]
        public void LoadMap01()
        {
            using (var wad = new Wad(WadPath.Doom2))
            {
                var map = wad.GetLumpNumber("MAP01");
                var things = MapThing.FromWad(wad, map + 1);

                Assert.AreEqual(69, things.Length);

                Assert.AreEqual(-96, things[0].X.ToDouble(), delta);
                Assert.AreEqual(784, things[0].Y.ToDouble(), delta);
                Assert.AreEqual(90, things[0].Angle.ToDegree(), delta);
                Assert.AreEqual(1, things[0].Type);
                Assert.AreEqual(7, (int)things[0].Flags);

                Assert.AreEqual(-288, things[57].X.ToDouble(), delta);
                Assert.AreEqual(976, things[57].Y.ToDouble(), delta);
                Assert.AreEqual(270, things[57].Angle.ToDegree(), delta);
                Assert.AreEqual(2006, things[57].Type);
                Assert.AreEqual(23, (int)things[57].Flags);

                Assert.AreEqual(-480, things[68].X.ToDouble(), delta);
                Assert.AreEqual(848, things[68].Y.ToDouble(), delta);
                Assert.AreEqual(0, things[68].Angle.ToDegree(), delta);
                Assert.AreEqual(2005, things[68].Type);
                Assert.AreEqual(7, (int)things[68].Flags);
            }
        }
    }
}
