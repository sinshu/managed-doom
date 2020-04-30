using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.UnitTests
{
    [TestClass]
    public class DemoTest
    {
        [TestMethod]
        public void Doom2()
        {
            using (var resource = CommonResource.CreateDummy(WadPath.Doom2))
            {
                {
                    var demo = new Demo(resource.Wad.ReadLump("DEMO1"));
                    Assert.AreEqual(11, demo.Options.Map);
                    Assert.AreEqual(0, demo.Options.ConsolePlayer);
                    Assert.AreEqual(true, demo.Options.PlayerInGame[0]);
                    Assert.AreEqual(false, demo.Options.PlayerInGame[1]);
                    Assert.AreEqual(false, demo.Options.PlayerInGame[2]);
                    Assert.AreEqual(false, demo.Options.PlayerInGame[3]);
                }

                {
                    var demo = new Demo(resource.Wad.ReadLump("DEMO2"));
                    Assert.AreEqual(5, demo.Options.Map);
                    Assert.AreEqual(0, demo.Options.ConsolePlayer);
                    Assert.AreEqual(true, demo.Options.PlayerInGame[0]);
                    Assert.AreEqual(false, demo.Options.PlayerInGame[1]);
                    Assert.AreEqual(false, demo.Options.PlayerInGame[2]);
                    Assert.AreEqual(false, demo.Options.PlayerInGame[3]);
                }

                {
                    var demo = new Demo(resource.Wad.ReadLump("DEMO3"));
                    Assert.AreEqual(26, demo.Options.Map);
                    Assert.AreEqual(0, demo.Options.ConsolePlayer);
                    Assert.AreEqual(true, demo.Options.PlayerInGame[0]);
                    Assert.AreEqual(false, demo.Options.PlayerInGame[1]);
                    Assert.AreEqual(false, demo.Options.PlayerInGame[2]);
                    Assert.AreEqual(false, demo.Options.PlayerInGame[3]);
                }
            }
        }
    }
}
