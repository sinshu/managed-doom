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
        public void Map01()
        {
            using (var resource = new CommonResource(WadPath.Doom2))
            {
                var options = new GameOptions();
                options.GameSkill = Skill.Hard;

                var players = new Player[Player.MaxPlayerCount];
                for (var i = 0; i < Player.MaxPlayerCount; i++)
                {
                    players[i] = new Player();
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
    }
}
