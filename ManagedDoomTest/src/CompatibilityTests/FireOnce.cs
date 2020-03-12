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
            using (var resources = new Resources(WadPath.Doom2))
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

                var world = new World(resources, options, players);

                var tics = 350;
                var pressFireUntil = 20;

                for (var i = 0; i < tics; i++)
                {
                    if (i < pressFireUntil)
                    {
                        players[0].Cmd.Buttons = Buttons.Attack;
                    }
                    else
                    {
                        players[0].Cmd.Buttons = 0;
                    }

                    world.Update();
                }

                Assert.AreEqual(0x3000f816u, (uint)world.GetMobjHash());
            }
        }
    }
}
