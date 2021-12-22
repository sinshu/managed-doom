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
            using (var content = GameContent.CreateDummy(WadPath.Doom2))
            {
                {
                    var demo = new Demo(content.Wad.ReadLump("DEMO1"));
                    Assert.AreEqual(11, demo.Options.Map);
                    Assert.AreEqual(0, demo.Options.ConsolePlayer);
                    Assert.AreEqual(true, demo.Options.Players[0].InGame);
                    Assert.AreEqual(false, demo.Options.Players[1].InGame);
                    Assert.AreEqual(false, demo.Options.Players[2].InGame);
                    Assert.AreEqual(false, demo.Options.Players[3].InGame);
                }

                {
                    var demo = new Demo(content.Wad.ReadLump("DEMO2"));
                    Assert.AreEqual(5, demo.Options.Map);
                    Assert.AreEqual(0, demo.Options.ConsolePlayer);
                    Assert.AreEqual(true, demo.Options.Players[0].InGame);
                    Assert.AreEqual(false, demo.Options.Players[1].InGame);
                    Assert.AreEqual(false, demo.Options.Players[2].InGame);
                    Assert.AreEqual(false, demo.Options.Players[3].InGame);
                }

                {
                    var demo = new Demo(content.Wad.ReadLump("DEMO3"));
                    Assert.AreEqual(26, demo.Options.Map);
                    Assert.AreEqual(0, demo.Options.ConsolePlayer);
                    Assert.AreEqual(true, demo.Options.Players[0].InGame);
                    Assert.AreEqual(false, demo.Options.Players[1].InGame);
                    Assert.AreEqual(false, demo.Options.Players[2].InGame);
                    Assert.AreEqual(false, demo.Options.Players[3].InGame);
                }
            }
        }
    }
}
