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
            using (var content = GameContent.CreateDummy(WadPath.Doom2, WadPath.Requiem))
            {
                var demo = new Demo(content.Wad.ReadLump("DEMO1"));
                demo.Options.GameVersion = GameVersion.Final2;
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastMobjHash = 0;
                var aggMobjHash = 0;
                var lastSectorHash = 0;
                var aggSectorHash = 0;

                while (true)
                {
                    demo.ReadCmd(cmds);
                    game.Update(cmds);
                    lastMobjHash = DoomDebug.GetMobjHash(game.World);
                    aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                    lastSectorHash = DoomDebug.GetSectorHash(game.World);
                    aggSectorHash = DoomDebug.CombineHash(aggSectorHash, lastSectorHash);

                    if (game.World.LevelTime == 18003)
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
            using (var content = GameContent.CreateDummy(WadPath.Doom2, WadPath.Requiem))
            {
                var demo = new Demo(content.Wad.ReadLump("DEMO2"));
                demo.Options.Players[0].PlayerState = PlayerState.Reborn;
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastMobjHash = 0;
                var aggMobjHash = 0;
                var lastSectorHash = 0;
                var aggSectorHash = 0;

                while (true)
                {
                    demo.ReadCmd(cmds);
                    game.Update(cmds);
                    lastMobjHash = DoomDebug.GetMobjHash(game.World);
                    aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                    lastSectorHash = DoomDebug.GetSectorHash(game.World);
                    aggSectorHash = DoomDebug.CombineHash(aggSectorHash, lastSectorHash);

                    if (game.World.LevelTime == 24659)
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

        [TestMethod]
        public void RequiemDemo3()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, WadPath.Requiem))
            {
                var demo = new Demo(content.Wad.ReadLump("DEMO3"));
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastMobjHash = 0;
                var aggMobjHash = 0;
                var lastSectorHash = 0;
                var aggSectorHash = 0;

                while (true)
                {
                    demo.ReadCmd(cmds);
                    game.Update(cmds);
                    lastMobjHash = DoomDebug.GetMobjHash(game.World);
                    aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                    lastSectorHash = DoomDebug.GetSectorHash(game.World);
                    aggSectorHash = DoomDebug.CombineHash(aggSectorHash, lastSectorHash);

                    if (game.World.LevelTime == 52487)
                    {
                        break;
                    }
                }

                Assert.AreEqual(0xb76035c8u, (uint)lastMobjHash);
                Assert.AreEqual(0x87651774u, (uint)aggMobjHash);
                Assert.AreEqual(0xa2d7d335u, (uint)lastSectorHash);
                Assert.AreEqual(0xabf7609au, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void TntBloodDemo1()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, WadPath.TntBlood))
            {
                var demo = new Demo(content.Wad.ReadLump("DEMO1"));
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

                Assert.AreEqual(0xa8343166u, (uint)lastMobjHash);
                Assert.AreEqual(0xd1d5c433u, (uint)aggMobjHash);
                Assert.AreEqual(0x9e70ce46u, (uint)lastSectorHash);
                Assert.AreEqual(0x71eb6e2cu, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void TntBloodDemo2()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, WadPath.TntBlood))
            {
                var demo = new Demo(content.Wad.ReadLump("DEMO2"));
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

                Assert.AreEqual(0x6fde0422u, (uint)lastMobjHash);
                Assert.AreEqual(0xbae1086eu, (uint)aggMobjHash);
                Assert.AreEqual(0x9708f97du, (uint)lastSectorHash);
                Assert.AreEqual(0xfc771056u, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void TntBloodDemo3()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, WadPath.TntBlood))
            {
                var demo = new Demo(content.Wad.ReadLump("DEMO3"));
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

                Assert.AreEqual(0x9d24c7d8u, (uint)lastMobjHash);
                Assert.AreEqual(0xd37240f4u, (uint)aggMobjHash);
                Assert.AreEqual(0xf3f4db97u, (uint)lastSectorHash);
                Assert.AreEqual(0xa0acc43eu, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void MementoMoriDemo1()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, WadPath.MementoMori))
            {
                var demo = new Demo(content.Wad.ReadLump("DEMO1"));
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

                Assert.AreEqual(0x9c24cf97u, (uint)lastMobjHash);
                Assert.AreEqual(0x58a33c2au, (uint)aggMobjHash);
                Assert.AreEqual(0xf0f84e3du, (uint)lastSectorHash);
                Assert.AreEqual(0x563d30fbu, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void MementoMoriDemo2()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, WadPath.MementoMori))
            {
                var demo = new Demo(content.Wad.ReadLump("DEMO2"));
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

                Assert.AreEqual(0x02bdcde5u, (uint)lastMobjHash);
                Assert.AreEqual(0x228756a5u, (uint)aggMobjHash);
                Assert.AreEqual(0xac3d6ccfu, (uint)lastSectorHash);
                Assert.AreEqual(0xb9311befu, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void MementoMoriDemo3()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, WadPath.MementoMori))
            {
                var demo = new Demo(content.Wad.ReadLump("DEMO3"));
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

                Assert.AreEqual(0x2c3bf1e3u, (uint)lastMobjHash);
                Assert.AreEqual(0x40d3fc5cu, (uint)aggMobjHash);
                Assert.AreEqual(0xdc871ca2u, (uint)lastSectorHash);
                Assert.AreEqual(0x388e5e4fu, (uint)aggSectorHash);
            }
        }
    }
}
