using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.CompatibilityTests
{
    [TestClass]
    public class PlayDemo
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
    }
}
