using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.CompatibilityTests
{
    [TestClass]
    public class IwadDemo
    {
        [TestMethod]
        public void Doom2Demo1()
        {
            using (var resource = new CommonResource(WadPath.Doom2))
            {
                var demo = new Demo(resource.Wad.ReadLump("DEMO1"));
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

                Assert.AreEqual(0x8541f2acu, (uint)lastMobjHash);
                Assert.AreEqual(0xe60b0af3u, (uint)aggMobjHash);

                Assert.AreEqual(0x3376327bu, (uint)lastSectorHash);
                Assert.AreEqual(0x4140c1c2u, (uint)aggSectorHash);
            }
        }
    }
}
