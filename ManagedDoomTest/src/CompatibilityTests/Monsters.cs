using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.CompatibilityTests
{
    [TestClass]
    public class Monsters
    {
        [TestMethod]
        public void NightmareTest()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2))
            {
                var demo = new Demo(@"data\nightmare_test.lmp");
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastHash = DoomDebug.GetMobjHash(game.World);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0x9278a07au, (uint)lastHash);
                Assert.AreEqual(0xb2d9a9a0u, (uint)aggHash);
            }
        }

        [TestMethod]
        public void BarrelTest()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\barrel_test.wad"))
            {
                var demo = new Demo(@"data\barrel_test.lmp");
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastHash = DoomDebug.GetMobjHash(game.World);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0xfb76dc03u, (uint)lastHash);
                Assert.AreEqual(0xccc38bc3u, (uint)aggHash);
            }
        }

        [TestMethod]
        public void ZombiemanTest()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\zombieman_test.wad"))
            {
                var demo = new Demo(@"data\zombieman_test.lmp");
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastHash = DoomDebug.GetMobjHash(game.World);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0xe6ce947cu, (uint)lastHash);
                Assert.AreEqual(0xb4b0d9a0u, (uint)aggHash);
            }
        }

        [TestMethod]
        public void ZombiemanTest2()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\zombieman_test2.wad"))
            {
                var demo = new Demo(@"data\zombieman_test2.lmp");
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastHash = DoomDebug.GetMobjHash(game.World);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0x97af3257u, (uint)lastHash);
                Assert.AreEqual(0x994fe30au, (uint)aggHash);
            }
        }

        [TestMethod]
        public void ShotgunguyTest()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\shotgunguy_test.wad"))
            {
                var demo = new Demo(@"data\shotgunguy_test.lmp");
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastHash = DoomDebug.GetMobjHash(game.World);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0x7bc7cdbau, (uint)lastHash);
                Assert.AreEqual(0x8010e4ffu, (uint)aggHash);
            }
        }

        [TestMethod]
        public void ChaingunguyTest()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\chaingunguy_test.wad"))
            {
                var demo = new Demo(@"data\chaingunguy_test.lmp");
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastHash = DoomDebug.GetMobjHash(game.World);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0xc135229fu, (uint)lastHash);
                Assert.AreEqual(0x7b9590d8u, (uint)aggHash);
            }
        }

        [TestMethod]
        public void ImpTest()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\imp_test.wad"))
            {
                var demo = new Demo(@"data\imp_test.lmp");
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastHash = DoomDebug.GetMobjHash(game.World);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0xaeee7433u, (uint)lastHash);
                Assert.AreEqual(0x64f0da30u, (uint)aggHash);
            }
        }

        [TestMethod]
        public void FastImpTest()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\imp_test.wad"))
            {
                var demo = new Demo(@"data\fast_imp_test.lmp");
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastHash = DoomDebug.GetMobjHash(game.World);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0x314b23f3u, (uint)lastHash);
                Assert.AreEqual(0x7ffd501du, (uint)aggHash);
            }
        }

        [TestMethod]
        public void DemonTest()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\demon_test.wad"))
            {
                var demo = new Demo(@"data\demon_test.lmp");
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastHash = DoomDebug.GetMobjHash(game.World);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0xcfdcb5d1u, (uint)lastHash);
                Assert.AreEqual(0x37ad1000u, (uint)aggHash);
            }
        }

        [TestMethod]
        public void FastDemonTest()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\demon_test.wad"))
            {
                var demo = new Demo(@"data\fast_demon_test.lmp");
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastHash = DoomDebug.GetMobjHash(game.World);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0x195cbb15u, (uint)lastHash);
                Assert.AreEqual(0x18bdbd50u, (uint)aggHash);
            }
        }

        [TestMethod]
        public void LostSoulTest_Final2()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\lostsoul_test.wad"))
            {
                var demo = new Demo(@"data\lostsoul_test_final2.lmp");
                demo.Options.GameVersion = GameVersion.Final2;
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastHash = DoomDebug.GetMobjHash(game.World);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0x2cdb1c94u, (uint)lastHash);
                Assert.AreEqual(0x99d18c88u, (uint)aggHash);
            }
        }

        [TestMethod]
        public void CacoDemonTest()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\cacodemon_test.wad"))
            {
                var demo = new Demo(@"data\cacodemon_test.lmp");
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastHash = DoomDebug.GetMobjHash(game.World);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0x76c0d9f4u, (uint)lastHash);
                Assert.AreEqual(0xf40d2331u, (uint)aggHash);
            }
        }

        [TestMethod]
        public void FastCacoDemonTest()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\cacodemon_test.wad"))
            {
                var demo = new Demo(@"data\fast_cacodemon_test.lmp");
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastHash = DoomDebug.GetMobjHash(game.World);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0x73316e3bu, (uint)lastHash);
                Assert.AreEqual(0x7219647fu, (uint)aggHash);
            }
        }

        [TestMethod]
        public void BaronTest()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\baron_test.wad"))
            {
                var demo = new Demo(@"data\baron_test.lmp");
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastHash = DoomDebug.GetMobjHash(game.World);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0x3b6c14d3u, (uint)lastHash);
                Assert.AreEqual(0xdb003628u, (uint)aggHash);
            }
        }

        [TestMethod]
        public void FastBaronTest()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\baron_test.wad"))
            {
                var demo = new Demo(@"data\fast_baron_test.lmp");
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastHash = DoomDebug.GetMobjHash(game.World);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0x79fb12efu, (uint)lastHash);
                Assert.AreEqual(0x1f5070bdu, (uint)aggHash);
            }
        }

        [TestMethod]
        public void RevenantTest()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\revenant_test.wad"))
            {
                var demo = new Demo(@"data\revenant_test.lmp");
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastHash = DoomDebug.GetMobjHash(game.World);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0x8b9fe3aeu, (uint)lastHash);
                Assert.AreEqual(0x24e038d7u, (uint)aggHash);
            }
        }

        [TestMethod]
        public void FatsoTest()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\fatso_test.wad"))
            {
                var demo = new Demo(@"data\fatso_test.lmp");
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastHash = DoomDebug.GetMobjHash(game.World);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0xadc6371eu, (uint)lastHash);
                Assert.AreEqual(0x196eebe6u, (uint)aggHash);
            }
        }

        [TestMethod]
        public void ArachnotronTest()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\arachnotron_test.wad"))
            {
                var demo = new Demo(@"data\arachnotron_test.lmp");
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastHash = DoomDebug.GetMobjHash(game.World);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0xa630a85eu, (uint)lastHash);
                Assert.AreEqual(0x9881a8ffu, (uint)aggHash);
            }
        }

        [TestMethod]
        public void PainElementalTest_Final2()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\painelemental_test.wad"))
            {
                var demo = new Demo(@"data\painelemental_test_final2.lmp");
                demo.Options.GameVersion = GameVersion.Final2;
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastHash = DoomDebug.GetMobjHash(game.World);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0x6984f76fu, (uint)lastHash);
                Assert.AreEqual(0x50ba7933u, (uint)aggHash);
            }
        }

        [TestMethod]
        public void ArchvileTest()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\archvile_test.wad"))
            {
                var demo = new Demo(@"data\archvile_test.lmp");
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastHash = DoomDebug.GetMobjHash(game.World);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0xaa5531f2u, (uint)lastHash);
                Assert.AreEqual(0xeb4456c4u, (uint)aggHash);
            }
        }

        [TestMethod]
        public void TelefragTest()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2, @"data\telefrag_test.wad"))
            {
                var demo = new Demo(@"data\telefrag_test.lmp");
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(content, demo.Options);
                game.DeferedInitNew();

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    game.Update(cmds);
                    lastHash = DoomDebug.GetMobjHash(game.World);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0x4c27ebc9u, (uint)lastHash);
                Assert.AreEqual(0xa93ecd0eu, (uint)aggHash);
            }
        }
    }
}
