using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.CompatibilityTests
{
    [TestClass]
    public class PwadDemo
    {
        [TestMethod]
        public void RequiemDemo1_Final2()
        {
            using (var resource = CommonResource.CreateDummy(WadPath.Doom2, WadPath.Requiem))
            {
                var demo = new Demo(resource.Wad.ReadLump("DEMO1"));
                var world = new World(resource, demo.Options, demo.Players);

                var lastMobjHash = 0;
                var aggMobjHash = 0;
                var lastSectorHash = 0;
                var aggSectorHash = 0;

                while (true)
                {
                    demo.ReadCmd();
                    world.Update();
                    lastMobjHash = world.GetMobjHash();
                    aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                    lastSectorHash = world.GetSectorHash();
                    aggSectorHash = DoomDebug.CombineHash(aggSectorHash, lastSectorHash);

                    if (world.levelTime == 18003)
                    {
                        break;
                    }
                }

                Assert.AreEqual(0x62d5d8f5u, (uint)lastMobjHash);
                Assert.AreEqual(0x05ce9c00u, (uint)aggMobjHash);
                Assert.AreEqual(0x94015cdau, (uint)lastSectorHash);
                Assert.AreEqual(0x1ae3ca8eu, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void RequiemDemo2()
        {
            using (var resource = CommonResource.CreateDummy(WadPath.Doom2, WadPath.Requiem))
            {
                var demo = new Demo(resource.Wad.ReadLump("DEMO2"));
                var world = new World(resource, demo.Options, demo.Players);

                var lastMobjHash = 0;
                var aggMobjHash = 0;
                var lastSectorHash = 0;
                var aggSectorHash = 0;

                while (true)
                {
                    demo.ReadCmd();
                    world.Update();
                    lastMobjHash = world.GetMobjHash();
                    aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                    lastSectorHash = world.GetSectorHash();
                    aggSectorHash = DoomDebug.CombineHash(aggSectorHash, lastSectorHash);

                    if (world.levelTime == 24659)
                    {
                        break;
                    }
                }

                Assert.AreEqual(0x083125a6u, (uint)lastMobjHash);
                Assert.AreEqual(0x50237ab4u, (uint)aggMobjHash);
                Assert.AreEqual(0x732a5645u, (uint)lastSectorHash);
                Assert.AreEqual(0x36f64dd0u, (uint)aggSectorHash);
            }
        }
    }
}
