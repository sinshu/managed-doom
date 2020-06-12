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
                options.Skill = GameSkill.Hard;
                options.Map = 1;

                options.Players[0].InGame = true;
                options.Players[0].PlayerState = PlayerState.Reborn;

                var world = new World(resource, options);

                var tics = 700;
                var pressFireUntil = 20;

                var aggHash = 0;
                for (var i = 0; i < tics; i++)
                {
                    if (i < pressFireUntil)
                    {
                        options.Players[0].Cmd.Buttons = TicCmdButtons.Attack;
                    }
                    else
                    {
                        options.Players[0].Cmd.Buttons = 0;
                    }

                    world.Update();
                    aggHash = DoomDebug.CombineHash(aggHash, DoomDebug.GetMobjHash(world));
                }

                Assert.AreEqual(0xef1aa1d8u, (uint)DoomDebug.GetMobjHash(world));
                Assert.AreEqual(0xe6edcf39u, (uint)aggHash);
            }
        }
    }
}
