using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.CompatibilityTests
{
    [TestClass]
    public class DoNothing
    {
        [TestMethod]
        public void E1M1()
        {
            using (var resource = CommonResource.CreateDummy(WadPath.Doom1))
            {
                var options = new GameOptions();
                options.Skill = Skill.Hard;
                options.Map = 1;

                var players = new Player[Player.MaxPlayerCount];
                for (var i = 0; i < Player.MaxPlayerCount; i++)
                {
                    players[i] = new Player(i);
                    players[i].PlayerState = PlayerState.Reborn;
                }
                players[0].InGame = true;

                var world = new World(resource, options, players);

                var tics = 350;

                var aggMobjHash = 0;
                var aggSectorHash = 0;
                for (var i = 0; i < tics; i++)
                {
                    world.Update();
                    aggMobjHash = DoomDebug.CombineHash(aggMobjHash, world.GetMobjHash());
                    aggSectorHash = DoomDebug.CombineHash(aggSectorHash, world.GetSectorHash());
                }

                Assert.AreEqual(0x66be313bu, (uint)world.GetMobjHash());
                Assert.AreEqual(0xbd67b2b2u, (uint)aggMobjHash);
                Assert.AreEqual(0x2cef7a1du, (uint)world.GetSectorHash());
                Assert.AreEqual(0x5b99ca23u, (uint)aggSectorHash);
            }
        }

        [TestMethod]
        public void Map01()
        {
            using (var resource = CommonResource.CreateDummy(WadPath.Doom2))
            {
                var options = new GameOptions();
                options.Skill = Skill.Hard;

                var players = new Player[Player.MaxPlayerCount];
                for (var i = 0; i < Player.MaxPlayerCount; i++)
                {
                    players[i] = new Player(i);
                    players[i].PlayerState = PlayerState.Reborn;
                }
                players[0].InGame = true;

                var world = new World(resource, options, players);

                var tics = 350;

                var aggHash = 0;
                for (var i = 0; i < tics; i++)
                {
                    world.Update();
                    aggHash = DoomDebug.CombineHash(aggHash, world.GetMobjHash());
                }

                Assert.AreEqual(0xc108ff16u, (uint)world.GetMobjHash());
                Assert.AreEqual(0x3bd5113cu, (uint)aggHash);
            }
        }

        [TestMethod]
        public void Map11Nomonsters()
        {
            using (var resource = CommonResource.CreateDummy(WadPath.Doom2))
            {
                var options = new GameOptions();
                options.Skill = Skill.Medium;
                options.Map = 11;
                options.NoMonsters = true;

                var players = new Player[Player.MaxPlayerCount];
                for (var i = 0; i < Player.MaxPlayerCount; i++)
                {
                    players[i] = new Player(i);
                    players[i].PlayerState = PlayerState.Reborn;
                }
                players[0].InGame = true;

                var world = new World(resource, options, players);

                var tics = 350;

                var aggMobjHash = 0;
                var aggSectorHash = 0;
                for (var i = 0; i < tics; i++)
                {
                    world.Update();
                    aggMobjHash = DoomDebug.CombineHash(aggMobjHash, world.GetMobjHash());
                    aggSectorHash = DoomDebug.CombineHash(aggSectorHash, world.GetSectorHash());
                }

                Assert.AreEqual(0x21187a94u, (uint)world.GetMobjHash());
                Assert.AreEqual(0x55752988u, (uint)aggMobjHash);
                Assert.AreEqual(0xead9e45bu, (uint)world.GetSectorHash());
                Assert.AreEqual(0x1397c7cbu, (uint)aggSectorHash);
            }
        }
    }
}
