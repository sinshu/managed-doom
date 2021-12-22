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
        public void Doom1SharewareDemo1()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom1Shareware))
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

                Assert.AreEqual(0xa497cb7fu, (uint)lastMobjHash);
                Assert.AreEqual(0x5a1776fdu, (uint)aggMobjHash);
                Assert.AreEqual(0x55d373a2u, (uint)lastSectorHash);
                Assert.AreEqual(0xcaafd23bu, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void Doom1SharewareDemo2()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom1Shareware))
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

                Assert.AreEqual(0xf7f5daddu, (uint)lastMobjHash);
                Assert.AreEqual(0xb576525au, (uint)aggMobjHash);
                Assert.AreEqual(0xf2e936b0u, (uint)lastSectorHash);
                Assert.AreEqual(0xe62009fau, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void Doom1SharewareDemo3()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom1Shareware))
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

                Assert.AreEqual(0x893f32d2u, (uint)lastMobjHash);
                Assert.AreEqual(0x22b21b86u, (uint)aggMobjHash);
                Assert.AreEqual(0xfef34aafu, (uint)lastSectorHash);
                Assert.AreEqual(0xa881ce6fu, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void Doom1Demo1()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom1))
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

                Assert.AreEqual(0xd94f3553u, (uint)lastMobjHash);
                Assert.AreEqual(0x056b5d73u, (uint)aggMobjHash);
                Assert.AreEqual(0x88a4b9c8u, (uint)lastSectorHash);
                Assert.AreEqual(0xede720f6u, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void Doom1Demo2()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom1))
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

                Assert.AreEqual(0xb292f1f1u, (uint)lastMobjHash);
                Assert.AreEqual(0xee8ecabau, (uint)aggMobjHash);
                Assert.AreEqual(0xd1d09995u, (uint)lastSectorHash);
                Assert.AreEqual(0x21d4589bu, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void Doom1Demo3()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom1))
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

                Assert.AreEqual(0xcdce0d85u, (uint)lastMobjHash);
                Assert.AreEqual(0x9e5f7780u, (uint)aggMobjHash);
                Assert.AreEqual(0x553d0ec9u, (uint)lastSectorHash);
                Assert.AreEqual(0x9991bb03u, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void Doom1Demo4()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom1))
            {
                var demo = new Demo(content.Wad.ReadLump("DEMO4"));
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

                Assert.AreEqual(0x89d50ff6u, (uint)lastMobjHash);
                Assert.AreEqual(0xb1a634c8u, (uint)aggMobjHash);
                Assert.AreEqual(0x8a94e89au, (uint)lastSectorHash);
                Assert.AreEqual(0x2e1bf98du, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void Doom2Demo1()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2))
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

                Assert.AreEqual(0x8541f2acu, (uint)lastMobjHash);
                Assert.AreEqual(0xe60b0af3u, (uint)aggMobjHash);
                Assert.AreEqual(0x3376327bu, (uint)lastSectorHash);
                Assert.AreEqual(0x4140c1c2u, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void Doom2Demo2()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2))
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

                Assert.AreEqual(0x45384a05u, (uint)lastMobjHash);
                Assert.AreEqual(0xde6d3531u, (uint)aggMobjHash);
                Assert.AreEqual(0x49c96600u, (uint)lastSectorHash);
                Assert.AreEqual(0x82f0e2d0u, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void Doom2Demo3_Final2()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2))
            {
                var demo = new Demo(content.Wad.ReadLump("DEMO3"));
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

                Assert.AreEqual(0x6daadf6du, (uint)lastMobjHash);
                Assert.AreEqual(0xdfba83c6u, (uint)aggMobjHash);
                Assert.AreEqual(0xfe1f6052u, (uint)lastSectorHash);
                Assert.AreEqual(0x6f6e779eu, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void TntDemo1()
        {
            using (var content = GameContent.CreateDummy(WadPath.Tnt))
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

                Assert.AreEqual(0x428a3538u, (uint)lastMobjHash);
                Assert.AreEqual(0x7bd7efb1u, (uint)aggMobjHash);
                Assert.AreEqual(0x5da0944cu, (uint)lastSectorHash);
                Assert.AreEqual(0x9a9aa180u, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void TntDemo2()
        {
            using (var content = GameContent.CreateDummy(WadPath.Tnt))
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

                Assert.AreEqual(0x03b2fe31u, (uint)lastMobjHash);
                Assert.AreEqual(0xa67458d0u, (uint)aggMobjHash);
                Assert.AreEqual(0xee0bf323u, (uint)lastSectorHash);
                Assert.AreEqual(0xcb6929e1u, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void TntDemo3()
        {
            using (var content = GameContent.CreateDummy(WadPath.Tnt))
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

                Assert.AreEqual(0xb289efaeu, (uint)lastMobjHash);
                Assert.AreEqual(0x08d04ef6u, (uint)aggMobjHash);
                Assert.AreEqual(0x8ab75d90u, (uint)lastSectorHash);
                Assert.AreEqual(0x08d5fbb0u, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void PlutoniaDemo1()
        {
            using (var content = GameContent.CreateDummy(WadPath.Plutonia))
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

                Assert.AreEqual(0x919a2c10u, (uint)lastMobjHash);
                Assert.AreEqual(0x50740a10u, (uint)aggMobjHash);
                Assert.AreEqual(0x67f448a4u, (uint)lastSectorHash);
                Assert.AreEqual(0x7cbaf2f8u, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void PlutoniaDemo2()
        {
            using (var content = GameContent.CreateDummy(WadPath.Plutonia))
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

                Assert.AreEqual(0x5d8b0a25u, (uint)lastMobjHash);
                Assert.AreEqual(0x780eb548u, (uint)aggMobjHash);
                Assert.AreEqual(0x027a2765u, (uint)lastSectorHash);
                Assert.AreEqual(0xdc23992bu, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void PlutoniaDemo3_Final2()
        {
            using (var content = GameContent.CreateDummy(WadPath.Plutonia))
            {
                var demo = new Demo(content.Wad.ReadLump("DEMO3"));
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

                Assert.AreEqual(0xa69f484fu, (uint)lastMobjHash);
                Assert.AreEqual(0xa62991cbu, (uint)aggMobjHash);
                Assert.AreEqual(0x796a4b06u, (uint)lastSectorHash);
                Assert.AreEqual(0xfa506444u, (uint)aggSectorHash);
            }
        }
    }
}
