using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.CompatibilityTests
{
    [TestClass]
    public class PlayerMovement
    {
        [TestMethod]
        public void PlayerMovementTest()
        {
            using (var resource = CommonResource.CreateDummy(WadPath.Doom2, @"data\player_movement_test.wad"))
            {
                var demo = new Demo(@"data\player_movement_test.lmp");
                var players = DoomTest.GetDefaultPlayers();
                var cmds = players.Select(player => player.Cmd).ToArray();
                var world = new World(resource, demo.Options, players);

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    world.Update();
                    lastHash = DoomDebug.GetMobjHash(world);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0xe9a6d7d2u, (uint)lastHash);
                Assert.AreEqual(0x5e70c62du, (uint)aggHash);
            }
        }

        [TestMethod]
        public void ThingCollisionTest()
        {
            using (var resource = CommonResource.CreateDummy(WadPath.Doom2, @"data\thing_collision_test.wad"))
            {
                var demo = new Demo(@"data\thing_collision_test.lmp");
                var players = DoomTest.GetDefaultPlayers();
                var cmds = players.Select(player => player.Cmd).ToArray();
                var world = new World(resource, demo.Options, players);

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    world.Update();
                    lastHash = DoomDebug.GetMobjHash(world);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0x63ff9173u, (uint)lastHash);
                Assert.AreEqual(0xb9cd0f6fu, (uint)aggHash);
            }
        }

        [TestMethod]
        public void AutoAimTest()
        {
            using (var resource = CommonResource.CreateDummy(WadPath.Doom2, @"data\autoaim_test.wad"))
            {
                var demo = new Demo(@"data\autoaim_test.lmp");
                var players = DoomTest.GetDefaultPlayers();
                var cmds = players.Select(player => player.Cmd).ToArray();
                var world = new World(resource, demo.Options, players);

                var lastHash = 0;
                var aggHash = 0;

                while (true)
                {
                    if (!demo.ReadCmd(cmds))
                    {
                        break;
                    }

                    world.Update();
                    lastHash = DoomDebug.GetMobjHash(world);
                    aggHash = DoomDebug.CombineHash(aggHash, lastHash);
                }

                Assert.AreEqual(0xe0d5d327u, (uint)lastHash);
                Assert.AreEqual(0x1a00fde9u, (uint)aggHash);
            }
        }
    }
}
