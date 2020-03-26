using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.CompatibilityTests
{
    [TestClass]
    public class PlayTestDemo
    {
        [TestMethod]
        public void PlayerMovementTest()
        {
            using (var resource = new CommonResource(WadPath.Doom2, @"data\player_movement_test.wad"))
            {
                var demo = new Demo(@"data\player_movement_test.lmp");
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

                Assert.AreEqual(0xe9a6d7d2u, (uint)lastHash);
                Assert.AreEqual(0x5e70c62du, (uint)aggHash);
            }
        }

        [TestMethod]
        public void ThingCollisionTest()
        {
            using (var resource = new CommonResource(WadPath.Doom2, @"data\thing_collision_test.wad"))
            {
                var demo = new Demo(@"data\thing_collision_test.lmp");
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

                Assert.AreEqual(0x63ff9173u, (uint)lastHash);
                Assert.AreEqual(0xb9cd0f6fu, (uint)aggHash);
            }
        }

        [TestMethod]
        public void AutoAimTest()
        {
            using (var resource = new CommonResource(WadPath.Doom2, @"data\autoaim_test.wad"))
            {
                var demo = new Demo(@"data\autoaim_test.lmp");
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

                Assert.AreEqual(0xe0d5d327u, (uint)lastHash);
                Assert.AreEqual(0x1a00fde9u, (uint)aggHash);
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
        public void LocalDoorTest()
        {
            using (var resource = new CommonResource(WadPath.Doom2, @"data\localdoor_test.wad"))
            {
                var demo = new Demo(@"data\localdoor_test.lmp");
                var world = new World(resource, demo.Options, demo.Players);

                var lastMobjHash = 0;
                var aggMobjHash = 0;
                var lastSectorHash = 0;
                var aggSectorHash = 0;
                while (true)
                {
                    var hasNext = demo.ReadCmd();
                    world.Update();

                    if (!hasNext)
                    {
                        break;
                    }

                    lastMobjHash = world.GetMobjHash();
                    aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);

                    lastSectorHash = world.GetSectorHash();
                    aggSectorHash = DoomDebug.CombineHash(aggSectorHash, lastSectorHash);
                }

                Assert.AreEqual(0x9d6c0abeu, (uint)lastMobjHash);
                Assert.AreEqual(0x7e1bb5f2u, (uint)aggMobjHash);

                Assert.AreEqual(0xfdf3e7a0u, (uint)lastSectorHash);
                Assert.AreEqual(0x0a0f1980u, (uint)aggSectorHash);
            }
        }
    }
}
