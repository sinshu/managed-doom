using System;
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
        public void TeleporterTest()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\teleporter_test.wad"))
            {
                var demo = new Demo(@"data\teleporter_test.lmp");
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastMobjHash = 0;
                var aggMobjHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastMobjHash = DoomDebug.GetMobjHash(game.World);
                    aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                }

                Assert.AreEqual(0x3450bb23u, (uint)lastMobjHash);
                Assert.AreEqual(0x2669e089u, (uint)aggMobjHash);
            }
        }

        [TestMethod]
        public void LocalDoorTest()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\localdoor_test.wad"))
            {
                var demo = new Demo(@"data\localdoor_test.lmp");
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastMobjHash = 0;
                var aggMobjHash = 0;
                var lastSectorHash = 0;
                var aggSectorHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastMobjHash = DoomDebug.GetMobjHash(game.World);
                    aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                    lastSectorHash = DoomDebug.GetSectorHash(game.World);
                    aggSectorHash = DoomDebug.CombineHash(aggSectorHash, lastSectorHash);
                }

                Assert.AreEqual(0x9d6c0abeu, (uint)lastMobjHash);
                Assert.AreEqual(0x7e1bb5f2u, (uint)aggMobjHash);
                Assert.AreEqual(0xfdf3e7a0u, (uint)lastSectorHash);
                Assert.AreEqual(0x0a0f1980u, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void PlatformTest()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\platform_test.wad"))
            {
                var demo = new Demo(@"data\platform_test.lmp");
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastMobjHash = 0;
                var aggMobjHash = 0;
                var lastSectorHash = 0;
                var aggSectorHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastMobjHash = DoomDebug.GetMobjHash(game.World);
                    aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                    lastSectorHash = DoomDebug.GetSectorHash(game.World);
                    aggSectorHash = DoomDebug.CombineHash(aggSectorHash, lastSectorHash);
                }

                Assert.AreEqual(0x3da2f507u, (uint)lastMobjHash);
                Assert.AreEqual(0x3402f715u, (uint)aggMobjHash);
                Assert.AreEqual(0xc71b4d00u, (uint)lastSectorHash);
                Assert.AreEqual(0x2fb8dd00u, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void SilentCrusherTest()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\silent_crusher_test.wad"))
            {
                var demo = new Demo(@"data\silent_crusher_test.lmp");
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastMobjHash = 0;
                var aggMobjHash = 0;
                var lastSectorHash = 0;
                var aggSectorHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastMobjHash = DoomDebug.GetMobjHash(game.World);
                    aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                    lastSectorHash = DoomDebug.GetSectorHash(game.World);
                    aggSectorHash = DoomDebug.CombineHash(aggSectorHash, lastSectorHash);
                }

                Assert.AreEqual(0xee31a164u, (uint)lastMobjHash);
                Assert.AreEqual(0x1f3fc7b4u, (uint)aggMobjHash);
                Assert.AreEqual(0x6d6a1f20u, (uint)lastSectorHash);
                Assert.AreEqual(0x34b4f740u, (uint)aggSectorHash);
            }
        }
    }
}
