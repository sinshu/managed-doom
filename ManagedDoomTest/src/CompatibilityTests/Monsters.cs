using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.CompatibilityTests
{
    [TestClass]
    public class Monsters
    {
        [TestMethod]
        public void BarrelTest()
        {
            using (var resource = new CommonResource(WadPath.Doom2, @"data\barrel_test.wad"))
            {
                var demo = new Demo(@"data\barrel_test.lmp");
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

                Assert.AreEqual(0xfb76dc03u, (uint)lastHash);
                Assert.AreEqual(0xccc38bc3u, (uint)aggHash);
            }
        }

        [TestMethod]
        public void ZombiemanTest()
        {
            using (var resource = new CommonResource(WadPath.Doom2, @"data\zombieman_test.wad"))
            {
                var demo = new Demo(@"data\zombieman_test.lmp");
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

                Assert.AreEqual(0xe6ce947cu, (uint)lastHash);
                Assert.AreEqual(0xb4b0d9a0u, (uint)aggHash);
            }
        }

        [TestMethod]
        public void ZombiemanTest2()
        {
            using (var resource = new CommonResource(WadPath.Doom2, @"data\zombieman_test2.wad"))
            {
                var demo = new Demo(@"data\zombieman_test2.lmp");
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

                Assert.AreEqual(0x97af3257u, (uint)lastHash);
                Assert.AreEqual(0x994fe30au, (uint)aggHash);
            }
        }

        [TestMethod]
        public void ShotgunguyTest()
        {
            using (var resource = new CommonResource(WadPath.Doom2, @"data\shotgunguy_test.wad"))
            {
                var demo = new Demo(@"data\shotgunguy_test.lmp");
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

                Assert.AreEqual(0x7bc7cdbau, (uint)lastHash);
                Assert.AreEqual(0x8010e4ffu, (uint)aggHash);
            }
        }

        [TestMethod]
        public void ChaingunguyTest()
        {
            using (var resource = new CommonResource(WadPath.Doom2, @"data\chaingunguy_test.wad"))
            {
                var demo = new Demo(@"data\chaingunguy_test.lmp");
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

                Assert.AreEqual(0xc135229fu, (uint)lastHash);
                Assert.AreEqual(0x7b9590d8u, (uint)aggHash);
            }
        }

        [TestMethod]
        public void ImpTest()
        {
            using (var resource = new CommonResource(WadPath.Doom2, @"data\imp_test.wad"))
            {
                var demo = new Demo(@"data\imp_test.lmp");
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

                Assert.AreEqual(0xaeee7433u, (uint)lastHash);
                Assert.AreEqual(0x64f0da30u, (uint)aggHash);
            }
        }

        [TestMethod]
        public void FastImpTest()
        {
            using (var resource = new CommonResource(WadPath.Doom2, @"data\imp_test.wad"))
            {
                var demo = new Demo(@"data\fast_imp_test.lmp");
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

                Assert.AreEqual(0x314b23f3u, (uint)lastHash);
                Assert.AreEqual(0x7ffd501du, (uint)aggHash);
            }
        }

        [TestMethod]
        public void DemonTest()
        {
            using (var resource = new CommonResource(WadPath.Doom2, @"data\demon_test.wad"))
            {
                var demo = new Demo(@"data\demon_test.lmp");
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

                Assert.AreEqual(0xcfdcb5d1u, (uint)lastHash);
                Assert.AreEqual(0x37ad1000u, (uint)aggHash);
            }
        }

        [TestMethod]
        public void FastDemonTest()
        {
            using (var resource = new CommonResource(WadPath.Doom2, @"data\demon_test.wad"))
            {
                var demo = new Demo(@"data\fast_demon_test.lmp");
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

                Assert.AreEqual(0x195cbb15u, (uint)lastHash);
                Assert.AreEqual(0x18bdbd50u, (uint)aggHash);
            }
        }
    }
}
