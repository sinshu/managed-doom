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
        public void MultiLevelTest()
        {
            using (var resource = CommonResource.CreateDummy(WadPath.Doom2, @"data\multi_level_test.wad"))
            {
                var demo = new Demo(@"data\multi_level_test.lmp");
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
                        Console.WriteLine(game.World.levelTime);
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
                        Console.WriteLine(game.World.levelTime);
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
                        Console.WriteLine(game.World.levelTime);
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
                        Console.WriteLine(game.World.levelTime);
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
