using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.CompatibilityTests
{
    [TestClass]
    public class FireOnce
    {
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

                var tics = 700;
                var pressFireUntil = 20;

                var aggHash = 0;
                for (var i = 0; i < tics; i++)
                {
                    if (i < pressFireUntil)
                    {
                        players[0].Cmd.Buttons = TicCmdButtons.Attack;
                    }
                    else
                    {
                        players[0].Cmd.Buttons = 0;
                    }

                    world.Update();
                    aggHash = DoomDebug.CombineHash(aggHash, world.GetMobjHash());
                }

                Assert.AreEqual(0xef1aa1d8u, (uint)world.GetMobjHash());
                Assert.AreEqual(0xe6edcf39u, (uint)aggHash);
            }
        }
    }
}
