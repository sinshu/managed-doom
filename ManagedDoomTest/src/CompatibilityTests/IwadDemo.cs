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
        public void Doom1Demo1()
        {
            using (var resource = CommonResource.CreateDummy(WadPath.Doom1))
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

                Assert.AreEqual(0xd94f3553u, (uint)lastMobjHash);
                Assert.AreEqual(0x056b5d73u, (uint)aggMobjHash);

                Assert.AreEqual(0x88a4b9c8u, (uint)lastSectorHash);
                Assert.AreEqual(0xede720f6u, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void Doom2Demo1()
        {
            using (var resource = CommonResource.CreateDummy(WadPath.Doom2))
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

        [TestMethod]
        public void Doom2Demo2()
        {
            using (var resource = CommonResource.CreateDummy(WadPath.Doom2))
            {
                var demo = new Demo(resource.Wad.ReadLump("DEMO2"));
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

                Assert.AreEqual(0x45384a05u, (uint)lastMobjHash);
                Assert.AreEqual(0xde6d3531u, (uint)aggMobjHash);

                Assert.AreEqual(0x49c96600u, (uint)lastSectorHash);
                Assert.AreEqual(0x82f0e2d0u, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void Doom2Demo3_Final2()
        {
            using (var resource = CommonResource.CreateDummy(WadPath.Doom2))
            {
                var demo = new Demo(resource.Wad.ReadLump("DEMO3"));
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

                Assert.AreEqual(0x6daadf6du, (uint)lastMobjHash);
                Assert.AreEqual(0xdfba83c6u, (uint)aggMobjHash);

                Assert.AreEqual(0xfe1f6052u, (uint)lastSectorHash);
                Assert.AreEqual(0x6f6e779eu, (uint)aggSectorHash);
            }
        }
    }
}
