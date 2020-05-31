using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.CompatibilityTests
{
    [TestClass]
    public class MultiLevel
    {
        [TestMethod]
        public void MultiLevelTest_Doom1()
        {
            using (var resource = CommonResource.CreateDummy(WadPath.Doom1, @"data\multilevel_test_doom1.wad"))
            {
                var demo = new Demo(@"data\multilevel_test_doom1.lmp");
                demo.Options.GameMode = resource.Wad.GameMode;
                var players = DoomTest.GetDefaultPlayers();
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(players, resource, demo.Options);

                var lastMobjHash = 0;
                var aggMobjHash = 0;

                // E1M1
                {
                    for (var i = 0; i < 456; i++)
                    {
                        demo.ReadCmd(cmds);
                        game.Update(cmds);
                        lastMobjHash = DoomDebug.GetMobjHash(game.World);
                        aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                    }

                    Assert.AreEqual(0xd76283ddu, (uint)lastMobjHash);
                    Assert.AreEqual(0x8e50483eu, (uint)aggMobjHash);
                }

                // Intermission
                {
                    for (var i = 0; i < 523; i++)
                    {
                        demo.ReadCmd(cmds);
                        game.Update(cmds);
                    }
                }

                // E1M2
                {
                    for (var i = 0; i < 492; i++)
                    {
                        demo.ReadCmd(cmds);
                        game.Update(cmds);
                        lastMobjHash = DoomDebug.GetMobjHash(game.World);
                        aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                    }

                    Assert.AreEqual(0x4499dad4u, (uint)lastMobjHash);
                    Assert.AreEqual(0xf2bdb9dfu, (uint)aggMobjHash);
                }

                // Intermission
                {
                    for (var i = 0; i < 368; i++)
                    {
                        demo.ReadCmd(cmds);
                        game.Update(cmds);
                    }
                }

                // E1M3
                {
                    for (var i = 0; i < 424; i++)
                    {
                        demo.ReadCmd(cmds);
                        game.Update(cmds);
                        lastMobjHash = DoomDebug.GetMobjHash(game.World);
                        aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                    }

                    Assert.AreEqual(0x807bef69u, (uint)lastMobjHash);
                    Assert.AreEqual(0x7fcd8281u, (uint)aggMobjHash);
                }

                // Intermission
                {
                    for (var i = 0; i < 28; i++)
                    {
                        demo.ReadCmd(cmds);
                        game.Update(cmds);
                    }
                }

                // E1M4
                {
                    for (var i = 0; i < 507; i++)
                    {
                        demo.ReadCmd(cmds);
                        game.Update(cmds);
                        lastMobjHash = DoomDebug.GetMobjHash(game.World);
                        aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                    }

                    Assert.AreEqual(0xd79ad915u, (uint)lastMobjHash);
                    Assert.AreEqual(0x504e3b27u, (uint)aggMobjHash);
                }

                // Intermission
                {
                    for (var i = 0; i < 253; i++)
                    {
                        demo.ReadCmd(cmds);
                        game.Update(cmds);
                    }
                }

                // E1M5
                {
                    for (var i = 0; i < 532; i++)
                    {
                        demo.ReadCmd(cmds);
                        game.Update(cmds);
                        lastMobjHash = DoomDebug.GetMobjHash(game.World);
                        aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                    }

                    Assert.AreEqual(0x233e5471u, (uint)lastMobjHash);
                    Assert.AreEqual(0xc5060c4eu, (uint)aggMobjHash);
                }
            }
        }

        [TestMethod]
        public void MultiLevelTest_Doom2()
        {
            using (var resource = CommonResource.CreateDummy(WadPath.Doom2, @"data\multilevel_test_doom2.wad"))
            {
                var demo = new Demo(@"data\multilevel_test_doom2.lmp");
                var players = DoomTest.GetDefaultPlayers();
                var cmds = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
                var game = new DoomGame(players, resource, demo.Options);

                var lastMobjHash = 0;
                var aggMobjHash = 0;

                // MAP01
                {
                    for (var i = 0; i < 801; i++)
                    {
                        demo.ReadCmd(cmds);
                        game.Update(cmds);
                        lastMobjHash = DoomDebug.GetMobjHash(game.World);
                        aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                    }

                    Assert.AreEqual(0xc88d8c72u, (uint)lastMobjHash);
                    Assert.AreEqual(0x50d51db4u, (uint)aggMobjHash);
                }

                // Intermission
                {
                    for (var i = 0; i < 378; i++)
                    {
                        demo.ReadCmd(cmds);
                        game.Update(cmds);
                    }
                }

                // MAP02
                {
                    for (var i = 0; i < 334; i++)
                    {
                        demo.ReadCmd(cmds);
                        game.Update(cmds);
                        lastMobjHash = DoomDebug.GetMobjHash(game.World);
                        aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                    }

                    Assert.AreEqual(0xeb24ae67u, (uint)lastMobjHash);
                    Assert.AreEqual(0xa08bf6ceu, (uint)aggMobjHash);
                }

                // Intermission
                {
                    for (var i = 0; i < 116; i++)
                    {
                        demo.ReadCmd(cmds);
                        game.Update(cmds);
                    }
                }

                // MAP03
                {
                    for (var i = 0; i < 653; i++)
                    {
                        demo.ReadCmd(cmds);
                        game.Update(cmds);
                        lastMobjHash = DoomDebug.GetMobjHash(game.World);
                        aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                    }

                    Assert.AreEqual(0xb4d74694u, (uint)lastMobjHash);
                    Assert.AreEqual(0xd813ac0fu, (uint)aggMobjHash);
                }

                // Intermission
                {
                    for (var i = 0; i < 131; i++)
                    {
                        demo.ReadCmd(cmds);
                        game.Update(cmds);
                    }
                }

                // MAP04
                {
                    for (var i = 0; i < 469; i++)
                    {
                        demo.ReadCmd(cmds);
                        game.Update(cmds);
                        lastMobjHash = DoomDebug.GetMobjHash(game.World);
                        aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                    }

                    Assert.AreEqual(0xaf214214u, (uint)lastMobjHash);
                    Assert.AreEqual(0xad054ab5u, (uint)aggMobjHash);
                }

                // Intermission
                {
                    for (var i = 0; i < 236; i++)
                    {
                        demo.ReadCmd(cmds);
                        game.Update(cmds);
                    }
                }

                // MAP05
                {
                    for (var i = 0; i < 312; i++)
                    {
                        demo.ReadCmd(cmds);
                        game.Update(cmds);
                        lastMobjHash = DoomDebug.GetMobjHash(game.World);
                        aggMobjHash = DoomDebug.CombineHash(aggMobjHash, lastMobjHash);
                    }

                    Assert.AreEqual(0xeb01a1fau, (uint)lastMobjHash);
                    Assert.AreEqual(0x0e4e66ffu, (uint)aggMobjHash);
                }
            }
        }
    }
}
