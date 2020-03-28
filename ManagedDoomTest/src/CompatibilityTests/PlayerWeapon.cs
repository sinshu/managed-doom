using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.CompatibilityTests
{
    [TestClass]
    public class PlayerWeapon
    {
        [TestMethod]
        public void ShotgunTest()
        {
            using (var resource = new CommonResource(WadPath.Doom2, @"data\shotgun_test.wad"))
            {
                var demo = new Demo(@"data\shotgun_test.lmp");
                var world = new World(resource, demo.Options, demo.Players);

                var lastHash = 0;
                var aggHash = 0;
                while (true)
                {
                    var hasNext = demo.ReadCmd();
                    world.Update();

                    if (!hasNext)
                    {
                        break;
                    }

                    lastHash = world.GetMobjHash();
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0x3dd50799u, (uint)lastHash);
                Assert.AreEqual(0x4ddd814fu, (uint)aggHash);
            }
        }

        [TestMethod]
        public void ChaingunTest()
        {
            using (var resource = new CommonResource(WadPath.Doom2, @"data\chaingun_test.wad"))
            {
                var demo = new Demo(@"data\chaingun_test.lmp");
                var world = new World(resource, demo.Options, demo.Players);

                var lastHash = 0;
                var aggHash = 0;
                while (true)
                {
                    var hasNext = demo.ReadCmd();
                    world.Update();

                    if (!hasNext)
                    {
                        break;
                    }

                    lastHash = world.GetMobjHash();
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0x0b30e14bu, (uint)lastHash);
                Assert.AreEqual(0xb2104158u, (uint)aggHash);
            }
        }
    }
}
