using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest
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
                options.GameMode = GameMode.Commercial;
                options.NetGame = false;
                options.Deathmatch = 0;
                options.GameSkill = Skill.Hard;
                options.NoMonsters = false;
                options.FastMonsters = false;

                var players = new Player[Player.MaxPlayerCount];
                for (var i = 0; i < Player.MaxPlayerCount; i++)
                {
                    players[i] = new Player();
                    players[i].PlayerState = PlayerState.Reborn;
                }
                players[0].InGame = true;

                var world = new World(resources, "MAP01", options, players);

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
