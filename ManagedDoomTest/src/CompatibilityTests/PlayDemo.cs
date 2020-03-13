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
            using (var resources = new Resources(WadPath.Doom2, @"data\player_movement_test.wad"))
            {
                var demo = new Demo(resources, @"data\player_movement_test.lmp");
                var world = new World(resources, demo.Options, demo.Players);

                var aggHash = 0;
                while (true)
                {
                    var hasNext = demo.ReadCmd();
                    world.Update();

                    if (!hasNext)
                    {
                        break;
                    }

                    aggHash = DoomDebug.CombineHash(aggHash, world.GetMobjHash());
                }

                Assert.AreEqual(0xe9a6d7d2u, (uint)world.GetMobjHash());
                Assert.AreEqual(0x5e70c62du, (uint)aggHash);
            }
        }
    }
}
