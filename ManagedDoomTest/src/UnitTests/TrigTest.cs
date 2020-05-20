using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.UnitTests
{
    [TestClass]
    public class TrigTest
    {
        [TestMethod]
        public void Tan()
        {
            for (var deg = 1; deg < 180; deg++)
            {
                var angle = Angle.FromDegree(deg);
                var fineAngle = (int)(angle.Data >> Trig.AngleToFineShift);

                var radian = 2 * Math.PI * (deg + 90) / 360;
                var expected = Math.Tan(radian);

                {
                    var actual = Trig.Tan(angle).ToDouble();
                    var delta = Math.Max(Math.Abs(expected) / 50, 1.0E-3);
                    Assert.AreEqual(expected, actual, delta);
                }

                {
                    var actual = Trig.Tan(fineAngle).ToDouble();
                    var delta = Math.Max(Math.Abs(expected) / 50, 1.0E-3);
                    Assert.AreEqual(expected, actual, delta);
                }
            }
        }

        [TestMethod]
        public void Sin()
        {
            for (var deg = -720; deg <= 720; deg++)
            {
                var angle = Angle.FromDegree(deg);
                var fineAngle = (int)(angle.Data >> Trig.AngleToFineShift);

                var radian = 2 * Math.PI * deg / 360;
                var expected = Math.Sin(radian);

                {
                    var actual = Trig.Sin(angle).ToDouble();
                    Assert.AreEqual(expected, actual, 1.0E-3);
                }

                {
                    var actual = Trig.Sin(fineAngle).ToDouble();
                    Assert.AreEqual(expected, actual, 1.0E-3);
                }
            }
        }

        [TestMethod]
        public void Cos()
        {
            for (var deg = -720; deg <= 720; deg++)
            {
                var angle = Angle.FromDegree(deg);
                var fineAngle = (int)(angle.Data >> Trig.AngleToFineShift);

                var radian = 2 * Math.PI * deg / 360;
                var expected = Math.Cos(radian);

                {
                    var actual = Trig.Cos(angle).ToDouble();
                    Assert.AreEqual(expected, actual, 1.0E-3);
                }

                {
                    var actual = Trig.Cos(fineAngle).ToDouble();
                    Assert.AreEqual(expected, actual, 1.0E-3);
                }
            }
        }
    }
}
