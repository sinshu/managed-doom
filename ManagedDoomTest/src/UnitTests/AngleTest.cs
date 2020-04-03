using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.UnitTests
{
    [TestClass]
    public class AngleTest
    {
        private static readonly double delta = 1.0E-3;

        [TestMethod]
        public void ToRadian()
        {
            Assert.AreEqual(0.00 * Math.PI, Angle.Ang0.ToRadian(), delta);
            Assert.AreEqual(0.25 * Math.PI, Angle.Ang45.ToRadian(), delta);
            Assert.AreEqual(0.50 * Math.PI, Angle.Ang90.ToRadian(), delta);
            Assert.AreEqual(1.00 * Math.PI, Angle.Ang180.ToRadian(), delta);
            Assert.AreEqual(1.50 * Math.PI, Angle.Ang270.ToRadian(), delta);
        }

        [TestMethod]
        public void FromDegrees()
        {
            for (var deg = -720; deg <= 720; deg++)
            {
                var expectedSin = Math.Sin(2 * Math.PI * deg / 360);
                var expectedCos = Math.Cos(2 * Math.PI * deg / 360);

                var angle = Angle.FromDegree(deg);
                var actualSin = Math.Sin(angle.ToRadian());
                var actualCos = Math.Cos(angle.ToRadian());

                Assert.AreEqual(expectedSin, actualSin, delta);
                Assert.AreEqual(expectedCos, actualCos, delta);
            }
        }

        [TestMethod]
        public void FromRadianToDegrees()
        {
            Assert.AreEqual(Angle.FromRadian(0.00 * Math.PI).ToDegree(), 0, delta);
            Assert.AreEqual(Angle.FromRadian(0.25 * Math.PI).ToDegree(), 45, delta);
            Assert.AreEqual(Angle.FromRadian(0.50 * Math.PI).ToDegree(), 90, delta);
            Assert.AreEqual(Angle.FromRadian(1.00 * Math.PI).ToDegree(), 180, delta);
            Assert.AreEqual(Angle.FromRadian(1.50 * Math.PI).ToDegree(), 270, delta);
        }

        [TestMethod]
        public void Sign()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = random.Next(1440) - 720;
                var b = +a;
                var c = -a;

                var aa = Angle.FromDegree(a);
                var ab = +aa;
                var ac = -aa;

                {
                    var expectedSin = Math.Sin(2 * Math.PI * b / 360);
                    var expectedCos = Math.Cos(2 * Math.PI * b / 360);

                    var actualSin = Math.Sin(ab.ToRadian());
                    var actualCos = Math.Cos(ab.ToRadian());

                    Assert.AreEqual(expectedSin, actualSin, delta);
                    Assert.AreEqual(expectedCos, actualCos, delta);
                }

                {
                    var expectedSin = Math.Sin(2 * Math.PI * c / 360);
                    var expectedCos = Math.Cos(2 * Math.PI * c / 360);

                    var actualSin = Math.Sin(ac.ToRadian());
                    var actualCos = Math.Cos(ac.ToRadian());

                    Assert.AreEqual(expectedSin, actualSin, delta);
                    Assert.AreEqual(expectedCos, actualCos, delta);
                }
            }
        }

        [TestMethod]
        public void Abs()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = random.Next(120) - 60;
                var b = Math.Abs(a);

                var aa = Angle.FromDegree(a);
                var ab = Angle.Abs(aa);

                var expectedSin = Math.Sin(2 * Math.PI * b / 360);
                var expectedCos = Math.Cos(2 * Math.PI * b / 360);

                var actualSin = Math.Sin(ab.ToRadian());
                var actualCos = Math.Cos(ab.ToRadian());

                Assert.AreEqual(expectedSin, actualSin, delta);
                Assert.AreEqual(expectedCos, actualCos, delta);
            }
        }

        [TestMethod]
        public void Addition()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = random.Next(1440) - 720;
                var b = random.Next(1440) - 720;
                var c = a + b;

                var fa = Angle.FromDegree(a);
                var fb = Angle.FromDegree(b);
                var fc = fa + fb;

                var expectedSin = Math.Sin(2 * Math.PI * c / 360);
                var expectedCos = Math.Cos(2 * Math.PI * c / 360);

                var actualSin = Math.Sin(fc.ToRadian());
                var actualCos = Math.Cos(fc.ToRadian());

                Assert.AreEqual(expectedSin, actualSin, delta);
                Assert.AreEqual(expectedCos, actualCos, delta);
            }
        }

        [TestMethod]
        public void Subtraction()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = random.Next(1440) - 720;
                var b = random.Next(1440) - 720;
                var c = a - b;

                var fa = Angle.FromDegree(a);
                var fb = Angle.FromDegree(b);
                var fc = fa - fb;

                var expectedSin = Math.Sin(2 * Math.PI * c / 360);
                var expectedCos = Math.Cos(2 * Math.PI * c / 360);

                var actualSin = Math.Sin(fc.ToRadian());
                var actualCos = Math.Cos(fc.ToRadian());

                Assert.AreEqual(expectedSin, actualSin, delta);
                Assert.AreEqual(expectedCos, actualCos, delta);
            }
        }

        [TestMethod]
        public void Multiplication1()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = (uint)random.Next(30);
                var b = (uint)random.Next(12);
                var c = a * b;

                var fa = Angle.FromDegree(a);
                var fc = fa * b;

                Assert.AreEqual(c, fc.ToDegree(), delta);
            }
        }

        [TestMethod]
        public void Multiplication2()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = (uint)random.Next(30);
                var b = (uint)random.Next(12);
                var c = a * b;

                var fb = Angle.FromDegree(b);
                var fc = a * fb;

                Assert.AreEqual(c, fc.ToDegree(), delta);
            }
        }

        [TestMethod]
        public void Division()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = (double)random.Next(360);
                var b = (uint)(random.Next(30) + 1);
                var c = a / b;

                var fa = Angle.FromDegree(a);
                var fc = fa / b;

                Assert.AreEqual(c, fc.ToDegree(), delta);
            }
        }

        [TestMethod]
        public void Comparison()
        {
            var random = new Random(666);
            for (var i = 0; i < 10000; i++)
            {
                var a = random.Next(1140) - 720;
                var b = random.Next(1140) - 720;

                var fa = Angle.FromDegree(a);
                var fb = Angle.FromDegree(b);

                a = ((a % 360) + 360) % 360;
                b = ((b % 360) + 360) % 360;

                Assert.IsTrue((a == b) == (fa == fb));
                Assert.IsTrue((a != b) == (fa != fb));
                Assert.IsTrue((a < b) == (fa < fb));
                Assert.IsTrue((a > b) == (fa > fb));
                Assert.IsTrue((a <= b) == (fa <= fb));
                Assert.IsTrue((a >= b) == (fa >= fb));
            }
        }
    }
}
