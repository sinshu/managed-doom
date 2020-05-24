using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.UnitTests
{
    [TestClass]
    public class FixedTest
    {
        private static readonly double delta = 1.0E-3;

        [TestMethod]
        public void Conversion()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var da = 666 * random.NextDouble() - 333;
                var sa = (float)da;

                var fda = Fixed.FromDouble(da);
                var fsa = Fixed.FromFloat(sa);

                Assert.AreEqual(da, fda.ToDouble(), delta);
                Assert.AreEqual(sa, fsa.ToFloat(), delta);
            }
        }

        [TestMethod]
        public void Abs1()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = random.Next(666) - 333;
                var b = Math.Abs(a);

                var fa = Fixed.FromDouble(a);
                var fb = Fixed.Abs(fa);

                Assert.AreEqual(b, fb.ToDouble(), delta);
            }
        }

        [TestMethod]
        public void Abs2()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = 666 * random.NextDouble() - 333;
                var b = Math.Abs(a);

                var fa = Fixed.FromDouble(a);
                var fb = Fixed.Abs(fa);

                Assert.AreEqual(b, fb.ToDouble(), delta);
            }
        }

        [TestMethod]
        public void Sign1()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = random.Next(666) - 333;

                var fa = Fixed.FromDouble(a);

                Assert.AreEqual(+a, (+fa).ToDouble(), delta);
                Assert.AreEqual(-a, (-fa).ToDouble(), delta);
            }
        }

        [TestMethod]
        public void Sign2()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = 666 * random.NextDouble() - 333;

                var fa = Fixed.FromDouble(a);

                Assert.AreEqual(+a, (+fa).ToDouble(), delta);
                Assert.AreEqual(-a, (-fa).ToDouble(), delta);
            }
        }

        [TestMethod]
        public void Addition1()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = random.Next(666) - 333;
                var b = random.Next(666) - 333;
                var c = a + b;

                var fa = Fixed.FromInt(a);
                var fb = Fixed.FromInt(b);
                var fc = fa + fb;

                Assert.AreEqual(c, fc.ToDouble(), delta);
            }
        }

        [TestMethod]
        public void Addition2()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = 666 * random.NextDouble() - 333;
                var b = 666 * random.NextDouble() - 333;
                var c = a + b;

                var fa = Fixed.FromDouble(a);
                var fb = Fixed.FromDouble(b);
                var fc = fa + fb;

                Assert.AreEqual(c, fc.ToDouble(), delta);
            }
        }

        [TestMethod]
        public void Subtraction1()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = random.Next(666) - 333;
                var b = random.Next(666) - 333;
                var c = a - b;

                var fa = Fixed.FromInt(a);
                var fb = Fixed.FromInt(b);
                var fc = fa - fb;

                Assert.AreEqual(c, fc.ToDouble(), delta);
            }
        }

        [TestMethod]
        public void Subtraction2()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = 666 * random.NextDouble() - 333;
                var b = 666 * random.NextDouble() - 333;
                var c = a - b;

                var fa = Fixed.FromDouble(a);
                var fb = Fixed.FromDouble(b);
                var fc = fa - fb;

                Assert.AreEqual(c, fc.ToDouble(), delta);
            }
        }

        [TestMethod]
        public void Multiplication1()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = random.Next(66) - 33;
                var b = random.Next(66) - 33;
                var c = a * b;

                var fa = Fixed.FromInt(a);
                var fb = Fixed.FromInt(b);
                var fc = fa * fb;

                Assert.AreEqual(c, fc.ToDouble(), delta);
            }
        }

        [TestMethod]
        public void Multiplication2()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = 66 * random.NextDouble() - 33;
                var b = 66 * random.NextDouble() - 33;
                var c = a * b;

                var fa = Fixed.FromDouble(a);
                var fb = Fixed.FromDouble(b);
                var fc = fa * fb;

                Assert.AreEqual(c, fc.ToDouble(), delta);
            }
        }

        [TestMethod]
        public void Multiplication3()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = random.Next(66) - 33;
                var b = 66 * random.NextDouble() - 33;
                var c = a * b;

                var fb = Fixed.FromDouble(b);
                var fc = a * fb;

                Assert.AreEqual(c, fc.ToDouble(), delta);
            }
        }

        [TestMethod]
        public void Multiplication4()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = 66 * random.NextDouble() - 33;
                var b = random.Next(66) - 33;
                var c = a * b;

                var fa = Fixed.FromDouble(a);
                var fc = fa * b;

                Assert.AreEqual(c, fc.ToDouble(), delta);
            }
        }

        [TestMethod]
        public void Division1()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = random.Next(66) - 33;
                var b = (2 * random.Next(2) - 1) * (random.Next(33) + 33);
                var c = (double)a / b;

                var fa = Fixed.FromInt(a);
                var fb = Fixed.FromInt(b);
                var fc = fa / fb;

                Assert.AreEqual(c, fc.ToDouble(), delta);
            }
        }

        [TestMethod]
        public void Division2()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = 66 * random.NextDouble() - 33;
                var b = (2 * random.Next(2) - 1) * (33 * random.NextDouble() + 33);
                var c = a / b;

                var fa = Fixed.FromDouble(a);
                var fb = Fixed.FromDouble(b);
                var fc = fa / fb;

                Assert.AreEqual(c, fc.ToDouble(), delta);
            }
        }

        [TestMethod]
        public void Division3()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = random.Next(66) - 33;
                var b = (2 * random.Next(2) - 1) * (33 * random.NextDouble() + 33);
                var c = a / b;

                var fb = Fixed.FromDouble(b);
                var fc = a / fb;

                Assert.AreEqual(c, fc.ToDouble(), delta);
            }
        }

        [TestMethod]
        public void Division4()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = 66 * random.NextDouble() - 33;
                var b = (2 * random.Next(2) - 1) * (random.Next(33) + 33);
                var c = a / b;

                var fa = Fixed.FromDouble(a);
                var fc = fa / b;

                Assert.AreEqual(c, fc.ToDouble(), delta);
            }
        }

        [TestMethod]
        public void Bitshift()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = random.Next(666666) - 333333;
                var b = random.Next(16);
                var c = a << b;
                var d = a >> b;

                var fa = new Fixed(a);
                var fc = fa << b;
                var fd = fa >> b;

                Assert.AreEqual(c, fc.Data);
                Assert.AreEqual(d, fd.Data);
            }
        }

        [TestMethod]
        public void Comparison()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = random.Next(5);
                var b = random.Next(5);

                var fa = Fixed.FromInt(a);
                var fb = Fixed.FromInt(b);

                Assert.AreEqual(a == b, fa == fb);
                Assert.AreEqual(a != b, fa != fb);
                Assert.AreEqual(a < b, fa < fb);
                Assert.AreEqual(a > b, fa > fb);
                Assert.AreEqual(a <= b, fa <= fb);
                Assert.AreEqual(a >= b, fa >= fb);
            }
        }

        [TestMethod]
        public void MinMax()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = random.Next(5);
                var b = random.Next(5);
                var min = Math.Min(a, b);
                var max = Math.Max(a, b);

                var fa = Fixed.FromInt(a);
                var fb = Fixed.FromInt(b);
                var fmin = Fixed.Min(fa, fb);
                var fmax = Fixed.Max(fa, fb);

                Assert.AreEqual(min, fmin.ToDouble(), 1.0E-9);
                Assert.AreEqual(max, fmax.ToDouble(), 1.0E-9);
            }
        }

        [TestMethod]
        public void ToInt1()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = random.Next(666);

                var fa = Fixed.FromDouble(a);
                var ffloor = fa.ToIntFloor();
                var fceiling = fa.ToIntCeiling();

                Assert.AreEqual(a, ffloor, 1.0E-9);
                Assert.AreEqual(a, fceiling, 1.0E-9);
            }
        }

        [TestMethod]
        public void ToInt2()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var a = (double)random.Next(666666) / 1000;
                var floor = Math.Floor(a);
                var ceiling = Math.Ceiling(a);

                var fa = Fixed.FromDouble(a);
                var ffloor = fa.ToIntFloor();
                var fceiling = fa.ToIntCeiling();

                Assert.AreEqual(floor, ffloor, 1.0E-9);
                Assert.AreEqual(ceiling, fceiling, 1.0E-9);
            }
        }
    }
}
