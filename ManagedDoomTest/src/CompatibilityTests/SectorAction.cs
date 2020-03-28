﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.CompatibilityTests
{
    [TestClass]
    public class SectorAction
    {
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
