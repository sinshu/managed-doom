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
            using (var resource = CommonResource.CreateDummy(WadPath.Doom2, @"data\shotgun_test.wad"))
            {
                var demo = new Demo(@"data\shotgun_test.lmp");
                var world = new World(resource, demo.Options, demo.Players);

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd())
                    {
                        break;
                    }

                    world.Update();
                    lastHash = world.GetMobjHash();
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0x3dd50799u, (uint)lastHash);
                Assert.AreEqual(0x4ddd814fu, (uint)aggHash);
            }
        }

        [TestMethod]
        public void SuperShotgunTest()
        {
            using (var resource = CommonResource.CreateDummy(WadPath.Doom2, @"data\supershotgun_test.wad"))
            {
                var demo = new Demo(@"data\supershotgun_test.lmp");
                var world = new World(resource, demo.Options, demo.Players);

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd())
                    {
                        break;
                    }

                    world.Update();
                    lastHash = world.GetMobjHash();
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0xe2f7936eu, (uint)lastHash);
                Assert.AreEqual(0x538061e4u, (uint)aggHash);
            }
        }

        [TestMethod]
        public void ChaingunTest()
        {
            using (var resource = CommonResource.CreateDummy(WadPath.Doom2, @"data\chaingun_test.wad"))
            {
                var demo = new Demo(@"data\chaingun_test.lmp");
                var world = new World(resource, demo.Options, demo.Players);

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd())
                    {
                        break;
                    }

                    world.Update();
                    lastHash = world.GetMobjHash();
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0x0b30e14bu, (uint)lastHash);
                Assert.AreEqual(0xb2104158u, (uint)aggHash);
            }
        }

        [TestMethod]
        public void RocketTest()
        {
            using (var resource = CommonResource.CreateDummy(WadPath.Doom2, @"data\rocket_test.wad"))
            {
                var demo = new Demo(@"data\rocket_test.lmp");
                var world = new World(resource, demo.Options, demo.Players);

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd())
                    {
                        break;
                    }

                    world.Update();
                    lastHash = world.GetMobjHash();
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0x8dce774bu, (uint)lastHash);
                Assert.AreEqual(0x87f45b5bu, (uint)aggHash);
            }
        }

        [TestMethod]
        public void PlasmaTest()
        {
            using (var resource = CommonResource.CreateDummy(WadPath.Doom2, @"data\plasma_test.wad"))
            {
                var demo = new Demo(@"data\plasma_test.lmp");
                var world = new World(resource, demo.Options, demo.Players);

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd())
                    {
                        break;
                    }

                    world.Update();
                    lastHash = world.GetMobjHash();
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0x116924d3u, (uint)lastHash);
                Assert.AreEqual(0x88fc9e68u, (uint)aggHash);
            }
        }
    }
}
