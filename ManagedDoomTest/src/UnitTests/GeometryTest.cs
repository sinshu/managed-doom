using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.UnitTests
{
    [TestClass]
    public class GeometryTest
    {
        [TestMethod]
        public void PointOnSide1()
        {
            var random = new Random(666);
            for (var i = 0; i < 1000; i++)
            {
                var startX = -1 - 666 * random.NextDouble();
                var endX = +1 + 666 * random.NextDouble();

                var pointX = 666 * random.NextDouble() - 333;
                var frontSideY = -1 - 666 * random.NextDouble();
                var backSideY = -frontSideY;

                var node = new Node(
                    Fixed.FromDouble(startX),
                    Fixed.Zero,
                    Fixed.FromDouble(endX - startX),
                    Fixed.Zero,
                    Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                    Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                    0, 0);

                var x = Fixed.FromDouble(pointX);
                {
                    var y = Fixed.FromDouble(frontSideY);
                    Assert.AreEqual(0, Geometry.PointOnSide(x, y, node));
                }
                {
                    var y = Fixed.FromDouble(backSideY);
                    Assert.AreEqual(1, Geometry.PointOnSide(x, y, node));
                }
            }
        }

        [TestMethod]
        public void PointOnSide2()
        {
            var random = new Random(666);
            for (var i = 0; i < 1000; i++)
            {
                var startY = +1 + 666 * random.NextDouble();
                var endY = -1 - 666 * random.NextDouble();

                var pointY = 666 * random.NextDouble() - 333;
                var frontSideX = -1 - 666 * random.NextDouble();
                var backSideX = -frontSideX;

                var node = new Node(
                    Fixed.Zero,
                    Fixed.FromDouble(startY),
                    Fixed.Zero,
                    Fixed.FromDouble(endY - startY),
                    Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                    Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                    0, 0);

                var y = Fixed.FromDouble(pointY);
                {
                    var x = Fixed.FromDouble(frontSideX);
                    Assert.AreEqual(0, Geometry.PointOnSide(x, y, node));
                }
                {
                    var x = Fixed.FromDouble(backSideX);
                    Assert.AreEqual(1, Geometry.PointOnSide(x, y, node));
                }
            }
        }

        [TestMethod]
        public void PointOnSide3()
        {
            var random = new Random(666);
            for (var i = 0; i < 1000; i++)
            {
                var startX = -1 - 666 * random.NextDouble();
                var endX = +1 + 666 * random.NextDouble();

                var pointX = 666 * random.NextDouble() - 333;
                var frontSideY = -1 - 666 * random.NextDouble();
                var backSideY = -frontSideY;

                for (var j = 0; j < 100; j++)
                {
                    var theta = 2 * Math.PI * random.NextDouble();
                    var ox = 666 * random.NextDouble() - 333;
                    var oy = 666 * random.NextDouble() - 333;

                    var node = new Node(
                        Fixed.FromDouble(ox + startX * Math.Cos(theta)),
                        Fixed.FromDouble(oy + startX * Math.Sin(theta)),
                        Fixed.FromDouble((endX - startX) * Math.Cos(theta)),
                        Fixed.FromDouble((endX - startX) * Math.Sin(theta)),
                        Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                        Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                        0, 0);

                    {
                        var x = Fixed.FromDouble(ox + pointX * Math.Cos(theta) - frontSideY * Math.Sin(theta));
                        var y = Fixed.FromDouble(oy + pointX * Math.Sin(theta) + frontSideY * Math.Cos(theta));
                        Assert.AreEqual(0, Geometry.PointOnSide(x, y, node));
                    }
                    {
                        var x = Fixed.FromDouble(ox + pointX * Math.Cos(theta) - backSideY * Math.Sin(theta));
                        var y = Fixed.FromDouble(oy + pointX * Math.Sin(theta) + backSideY * Math.Cos(theta));
                        Assert.AreEqual(1, Geometry.PointOnSide(x, y, node));
                    }
                }
            }
        }

        [TestMethod]
        public void PointToDist()
        {
            var random = new Random(666);
            for (var i = 0; i < 1000; i += 3)
            {
                var expected = i;
                for (var j = 0; j < 100; j++)
                {
                    var r = i;
                    var theta = 2 * Math.PI * random.NextDouble();
                    var ox = 666 * random.NextDouble() - 333;
                    var oy = 666 * random.NextDouble() - 333;
                    var x = ox + r * Math.Cos(theta);
                    var y = oy + r * Math.Sin(theta);
                    var fromX = Fixed.FromDouble(ox);
                    var fromY = Fixed.FromDouble(oy);
                    var toX = Fixed.FromDouble(x);
                    var toY = Fixed.FromDouble(y);
                    var dist = Geometry.PointToDist(fromX, fromY, toX, toY);
                    Assert.AreEqual(expected, dist.ToDouble(), (double)i / 100);
                }
            }
        }

        [TestMethod]
        public void PointToAngle()
        {
            var random = new Random(666);
            for (var i = 0; i < 100; i++)
            {
                var expected = 2 * Math.PI * random.NextDouble();
                for (var j = 0; j < 100; j++)
                {
                    var r = 666 * random.NextDouble();
                    var ox = 666 * random.NextDouble() - 333;
                    var oy = 666 * random.NextDouble() - 333;
                    var x = ox + r * Math.Cos(expected);
                    var y = oy + r * Math.Sin(expected);
                    var fromX = Fixed.FromDouble(ox);
                    var fromY = Fixed.FromDouble(oy);
                    var toX = Fixed.FromDouble(x);
                    var toY = Fixed.FromDouble(y);
                    var angle = Geometry.PointToAngle(fromX, fromY, toX, toY);
                    var actual = angle.ToRadian();
                    Assert.AreEqual(expected, actual, 0.01);
                }
            }
        }

        [TestMethod]
        public void PointInSubsectorE1M1()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom1))
            {
                var options = new GameOptions();
                var world = new World(content, options);
                var map = new Map(content, world);

                var ok = 0;
                var count = 0;

                foreach (var subsector in map.Subsectors)
                {
                    for (var i = 0; i < subsector.SegCount; i++)
                    {
                        var seg = map.Segs[subsector.FirstSeg + i];

                        var p1x = seg.Vertex1.X.ToDouble();
                        var p1y = seg.Vertex1.Y.ToDouble();
                        var p2x = seg.Vertex2.X.ToDouble();
                        var p2y = seg.Vertex2.Y.ToDouble();

                        var dx = p2x - p1x;
                        var dy = p2y - p1y;
                        var length = Math.Sqrt(dx * dx + dy * dy);

                        var centerX = (p1x + p2x) / 2;
                        var centerY = (p1y + p2y) / 2;
                        var stepX = dy / length;
                        var stepY = -dx / length;

                        var targetX = centerX + 3 * stepX;
                        var targetY = centerY + 3 * stepY;

                        var fx = Fixed.FromDouble(targetX);
                        var fy = Fixed.FromDouble(targetY);

                        var result = Geometry.PointInSubsector(fx, fy, map);

                        if (result == subsector)
                        {
                            ok++;
                        }
                        count++;
                    }
                }

                Assert.IsTrue((double)ok / count >= 0.995);
            }
        }

        [TestMethod]
        public void PointInSubsectorMap01()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2))
            {
                var options = new GameOptions();
                var world = new World(content, options);
                var map = new Map(content, world);

                var ok = 0;
                var count = 0;

                foreach (var subsector in map.Subsectors)
                {
                    for (var i = 0; i < subsector.SegCount; i++)
                    {
                        var seg = map.Segs[subsector.FirstSeg + i];

                        var p1x = seg.Vertex1.X.ToDouble();
                        var p1y = seg.Vertex1.Y.ToDouble();
                        var p2x = seg.Vertex2.X.ToDouble();
                        var p2y = seg.Vertex2.Y.ToDouble();

                        var dx = p2x - p1x;
                        var dy = p2y - p1y;
                        var length = Math.Sqrt(dx * dx + dy * dy);

                        var centerX = (p1x + p2x) / 2;
                        var centerY = (p1y + p2y) / 2;
                        var stepX = dy / length;
                        var stepY = -dx / length;

                        var targetX = centerX + 3 * stepX;
                        var targetY = centerY + 3 * stepY;

                        var fx = Fixed.FromDouble(targetX);
                        var fy = Fixed.FromDouble(targetY);

                        var result = Geometry.PointInSubsector(fx, fy, map);

                        if (result == subsector)
                        {
                            ok++;
                        }
                        count++;
                    }
                }

                Assert.IsTrue((double)ok / count >= 0.995);
            }
        }

        [TestMethod]
        public void PointOnSegSide1()
        {
            var random = new Random(666);
            for (var i = 0; i < 1000; i++)
            {
                var startX = -1 - 666 * random.NextDouble();
                var endX = +1 + 666 * random.NextDouble();

                var pointX = 666 * random.NextDouble() - 333;
                var frontSideY = -1 - 666 * random.NextDouble();
                var backSideY = -frontSideY;

                var vertex1 = new Vertex(Fixed.FromDouble(startX), Fixed.Zero);
                var vertex2 = new Vertex(Fixed.FromDouble(endX - startX), Fixed.Zero);

                var seg = new Seg(
                    vertex1,
                    vertex2,
                    Fixed.Zero, Angle.Ang0, null, null, null, null);

                var x = Fixed.FromDouble(pointX);
                {
                    var y = Fixed.FromDouble(frontSideY);
                    Assert.AreEqual(0, Geometry.PointOnSegSide(x, y, seg));
                }
                {
                    var y = Fixed.FromDouble(backSideY);
                    Assert.AreEqual(1, Geometry.PointOnSegSide(x, y, seg));
                }
            }
        }

        [TestMethod]
        public void PointOnSegSide2()
        {
            var random = new Random(666);
            for (var i = 0; i < 1000; i++)
            {
                var startY = +1 + 666 * random.NextDouble();
                var endY = -1 - 666 * random.NextDouble();

                var pointY = 666 * random.NextDouble() - 333;
                var frontSideX = -1 - 666 * random.NextDouble();
                var backSideX = -frontSideX;

                var vertex1 = new Vertex(Fixed.Zero, Fixed.FromDouble(startY));
                var vertex2 = new Vertex(Fixed.Zero, Fixed.FromDouble(endY - startY));

                var seg = new Seg(
                    vertex1,
                    vertex2,
                    Fixed.Zero, Angle.Ang0, null, null, null, null);

                var y = Fixed.FromDouble(pointY);
                {
                    var x = Fixed.FromDouble(frontSideX);
                    Assert.AreEqual(0, Geometry.PointOnSegSide(x, y, seg));
                }
                {
                    var x = Fixed.FromDouble(backSideX);
                    Assert.AreEqual(1, Geometry.PointOnSegSide(x, y, seg));
                }
            }
        }

        [TestMethod]
        public void PointOnSegSide3()
        {
            var random = new Random(666);
            for (var i = 0; i < 1000; i++)
            {
                var startX = -1 - 666 * random.NextDouble();
                var endX = +1 + 666 * random.NextDouble();

                var pointX = 666 * random.NextDouble() - 333;
                var frontSideY = -1 - 666 * random.NextDouble();
                var backSideY = -frontSideY;

                for (var j = 0; j < 100; j++)
                {
                    var theta = 2 * Math.PI * random.NextDouble();
                    var ox = 666 * random.NextDouble() - 333;
                    var oy = 666 * random.NextDouble() - 333;

                    var vertex1 = new Vertex(
                        Fixed.FromDouble(ox + startX * Math.Cos(theta)),
                        Fixed.FromDouble(oy + startX * Math.Sin(theta)));

                    var vertex2 = new Vertex(
                        vertex1.X + Fixed.FromDouble((endX - startX) * Math.Cos(theta)),
                        vertex1.Y + Fixed.FromDouble((endX - startX) * Math.Sin(theta)));

                    var seg = new Seg(
                        vertex1,
                        vertex2,
                        Fixed.Zero, Angle.Ang0, null, null, null, null);

                    {
                        var x = Fixed.FromDouble(ox + pointX * Math.Cos(theta) - frontSideY * Math.Sin(theta));
                        var y = Fixed.FromDouble(oy + pointX * Math.Sin(theta) + frontSideY * Math.Cos(theta));
                        Assert.AreEqual(0, Geometry.PointOnSegSide(x, y, seg));
                    }
                    {
                        var x = Fixed.FromDouble(ox + pointX * Math.Cos(theta) - backSideY * Math.Sin(theta));
                        var y = Fixed.FromDouble(oy + pointX * Math.Sin(theta) + backSideY * Math.Cos(theta));
                        Assert.AreEqual(1, Geometry.PointOnSegSide(x, y, seg));
                    }
                }
            }
        }

        [TestMethod]
        public void PointOnLineSide1()
        {
            var random = new Random(666);
            for (var i = 0; i < 1000; i++)
            {
                var startX = -1 - 666 * random.NextDouble();
                var endX = +1 + 666 * random.NextDouble();

                var pointX = 666 * random.NextDouble() - 333;
                var frontSideY = -1 - 666 * random.NextDouble();
                var backSideY = -frontSideY;

                var vertex1 = new Vertex(Fixed.FromDouble(startX), Fixed.Zero);
                var vertex2 = new Vertex(Fixed.FromDouble(endX - startX), Fixed.Zero);

                var line = new LineDef(
                    vertex1,
                    vertex2,
                    0, 0, 0, null, null);

                var x = Fixed.FromDouble(pointX);
                {
                    var y = Fixed.FromDouble(frontSideY);
                    Assert.AreEqual(0, Geometry.PointOnLineSide(x, y, line));
                }
                {
                    var y = Fixed.FromDouble(backSideY);
                    Assert.AreEqual(1, Geometry.PointOnLineSide(x, y, line));
                }
            }
        }

        [TestMethod]
        public void PointOnLineSide2()
        {
            var random = new Random(666);
            for (var i = 0; i < 1000; i++)
            {
                var startY = +1 + 666 * random.NextDouble();
                var endY = -1 - 666 * random.NextDouble();

                var pointY = 666 * random.NextDouble() - 333;
                var frontSideX = -1 - 666 * random.NextDouble();
                var backSideX = -frontSideX;

                var vertex1 = new Vertex(Fixed.Zero, Fixed.FromDouble(startY));
                var vertex2 = new Vertex(Fixed.Zero, Fixed.FromDouble(endY - startY));

                var line = new LineDef(
                    vertex1,
                    vertex2,
                    0, 0, 0, null, null);

                var y = Fixed.FromDouble(pointY);
                {
                    var x = Fixed.FromDouble(frontSideX);
                    Assert.AreEqual(0, Geometry.PointOnLineSide(x, y, line));
                }
                {
                    var x = Fixed.FromDouble(backSideX);
                    Assert.AreEqual(1, Geometry.PointOnLineSide(x, y, line));
                }
            }
        }

        [TestMethod]
        public void PointOnLineSide3()
        {
            var random = new Random(666);
            for (var i = 0; i < 1000; i++)
            {
                var startX = -1 - 666 * random.NextDouble();
                var endX = +1 + 666 * random.NextDouble();

                var pointX = 666 * random.NextDouble() - 333;
                var frontSideY = -1 - 666 * random.NextDouble();
                var backSideY = -frontSideY;

                for (var j = 0; j < 100; j++)
                {
                    var theta = 2 * Math.PI * random.NextDouble();
                    var ox = 666 * random.NextDouble() - 333;
                    var oy = 666 * random.NextDouble() - 333;

                    var vertex1 = new Vertex(
                        Fixed.FromDouble(ox + startX * Math.Cos(theta)),
                        Fixed.FromDouble(oy + startX * Math.Sin(theta)));

                    var vertex2 = new Vertex(
                        vertex1.X + Fixed.FromDouble((endX - startX) * Math.Cos(theta)),
                        vertex1.Y + Fixed.FromDouble((endX - startX) * Math.Sin(theta)));

                    var line = new LineDef(
                        vertex1,
                        vertex2,
                        0, 0, 0, null, null);

                    {
                        var x = Fixed.FromDouble(ox + pointX * Math.Cos(theta) - frontSideY * Math.Sin(theta));
                        var y = Fixed.FromDouble(oy + pointX * Math.Sin(theta) + frontSideY * Math.Cos(theta));
                        Assert.AreEqual(0, Geometry.PointOnLineSide(x, y, line));
                    }
                    {
                        var x = Fixed.FromDouble(ox + pointX * Math.Cos(theta) - backSideY * Math.Sin(theta));
                        var y = Fixed.FromDouble(oy + pointX * Math.Sin(theta) + backSideY * Math.Cos(theta));
                        Assert.AreEqual(1, Geometry.PointOnLineSide(x, y, line));
                    }
                }
            }
        }

        [TestMethod]
        public void BoxOnLineSide1()
        {
            var random = new Random(666);
            for (var i = 0; i < 1000; i++)
            {
                var radius = 33 + 33 * random.NextDouble();

                var startX = -1 - 666 * random.NextDouble();
                var endX = +1 + 666 * random.NextDouble();

                var pointX = 666 * random.NextDouble() - 333;
                var frontSideY = -1 - radius - 666 * random.NextDouble();
                var backSideY = -frontSideY;
                var crossingY = radius * 1.9 * (random.NextDouble() - 0.5);

                var frontBox = new Fixed[]
                {
                    Fixed.FromDouble(frontSideY + radius),
                    Fixed.FromDouble(frontSideY - radius),
                    Fixed.FromDouble(pointX - radius),
                    Fixed.FromDouble(pointX + radius)
                };

                var backBox = new Fixed[]
                {
                    Fixed.FromDouble(backSideY + radius),
                    Fixed.FromDouble(backSideY - radius),
                    Fixed.FromDouble(pointX - radius),
                    Fixed.FromDouble(pointX + radius)
                };

                var crossingBox = new Fixed[]
                {
                    Fixed.FromDouble(crossingY + radius),
                    Fixed.FromDouble(crossingY - radius),
                    Fixed.FromDouble(pointX - radius),
                    Fixed.FromDouble(pointX + radius)
                };

                var vertex1 = new Vertex(Fixed.FromDouble(startX), Fixed.Zero);
                var vertex2 = new Vertex(Fixed.FromDouble(endX - startX), Fixed.Zero);

                var line = new LineDef(
                    vertex1,
                    vertex2,
                    0, 0, 0, null, null);

                Assert.AreEqual(0, Geometry.BoxOnLineSide(frontBox, line));
                Assert.AreEqual(1, Geometry.BoxOnLineSide(backBox, line));
                Assert.AreEqual(-1, Geometry.BoxOnLineSide(crossingBox, line));
            }
        }

        [TestMethod]
        public void BoxOnLineSide2()
        {
            var random = new Random(666);
            for (var i = 0; i < 1000; i++)
            {
                var radius = 33 + 33 * random.NextDouble();

                var startY = +1 + 666 * random.NextDouble();
                var endY = -1 - 666 * random.NextDouble();

                var pointY = 666 * random.NextDouble() - 333;
                var frontSideX = -1 - radius - 666 * random.NextDouble();
                var backSideX = -frontSideX;
                var crossingX = radius * 1.9 * (random.NextDouble() - 0.5);

                var frontBox = new Fixed[]
                {
                    Fixed.FromDouble(pointY + radius),
                    Fixed.FromDouble(pointY - radius),
                    Fixed.FromDouble(frontSideX - radius),
                    Fixed.FromDouble(frontSideX + radius)
                };

                var backBox = new Fixed[]
                {
                    Fixed.FromDouble(pointY + radius),
                    Fixed.FromDouble(pointY - radius),
                    Fixed.FromDouble(backSideX - radius),
                    Fixed.FromDouble(backSideX + radius)
                };

                var crossingBox = new Fixed[]
                {
                    Fixed.FromDouble(pointY + radius),
                    Fixed.FromDouble(pointY - radius),
                    Fixed.FromDouble(crossingX - radius),
                    Fixed.FromDouble(crossingX + radius)
                };

                var vertex1 = new Vertex(Fixed.Zero, Fixed.FromDouble(startY));
                var vertex2 = new Vertex(Fixed.Zero, Fixed.FromDouble(endY - startY));

                var line = new LineDef(
                    vertex1,
                    vertex2,
                    0, 0, 0, null, null);

                Assert.AreEqual(0, Geometry.BoxOnLineSide(frontBox, line));
                Assert.AreEqual(1, Geometry.BoxOnLineSide(backBox, line));
                Assert.AreEqual(-1, Geometry.BoxOnLineSide(crossingBox, line));
            }
        }

        [TestMethod]
        public void BoxOnLineSide3()
        {
            var random = new Random(666);
            for (var i = 0; i < 1000; i++)
            {
                var radius = 33 + 33 * random.NextDouble();

                var startX = -1 - 666 * random.NextDouble();
                var endX = +1 + 666 * random.NextDouble();

                var pointX = 666 * random.NextDouble() - 333;
                var frontSideY = -1 - 1.5 * radius - 666 * random.NextDouble();
                var backSideY = -frontSideY;
                var crossingY = radius * 1.9 * (random.NextDouble() - 0.5);

                for (var j = 0; j < 100; j++)
                {
                    var theta = 2 * Math.PI * random.NextDouble();
                    var ox = 666 * random.NextDouble() - 333;
                    var oy = 666 * random.NextDouble() - 333;

                    var frontBox = new Fixed[]
                    {
                        Fixed.FromDouble(oy + pointX * Math.Sin(theta) + frontSideY * Math.Cos(theta) + radius),
                        Fixed.FromDouble(oy + pointX * Math.Sin(theta) + frontSideY * Math.Cos(theta) - radius),
                        Fixed.FromDouble(ox + pointX * Math.Cos(theta) - frontSideY * Math.Sin(theta) - radius),
                        Fixed.FromDouble(ox + pointX * Math.Cos(theta) - frontSideY * Math.Sin(theta) + radius)
                    };

                    var backBox = new Fixed[]
                    {
                        Fixed.FromDouble(oy + pointX * Math.Sin(theta) + backSideY * Math.Cos(theta) + radius),
                        Fixed.FromDouble(oy + pointX * Math.Sin(theta) + backSideY * Math.Cos(theta) - radius),
                        Fixed.FromDouble(ox + pointX * Math.Cos(theta) - backSideY * Math.Sin(theta) - radius),
                        Fixed.FromDouble(ox + pointX * Math.Cos(theta) - backSideY * Math.Sin(theta) + radius)
                    };

                    var crossingBox = new Fixed[]
                    {
                        Fixed.FromDouble(oy + pointX * Math.Sin(theta) + crossingY * Math.Cos(theta) + radius),
                        Fixed.FromDouble(oy + pointX * Math.Sin(theta) + crossingY * Math.Cos(theta) - radius),
                        Fixed.FromDouble(ox + pointX * Math.Cos(theta) - crossingY * Math.Sin(theta) - radius),
                        Fixed.FromDouble(ox + pointX * Math.Cos(theta) - crossingY * Math.Sin(theta) + radius)
                    };

                    var vertex1 = new Vertex(
                        Fixed.FromDouble(ox + startX * Math.Cos(theta)),
                        Fixed.FromDouble(oy + startX * Math.Sin(theta)));

                    var vertex2 = new Vertex(
                        vertex1.X + Fixed.FromDouble((endX - startX) * Math.Cos(theta)),
                        vertex1.Y + Fixed.FromDouble((endX - startX) * Math.Sin(theta)));

                    var line = new LineDef(
                        vertex1,
                        vertex2,
                        0, 0, 0, null, null);

                    Assert.AreEqual(0, Geometry.BoxOnLineSide(frontBox, line));
                    Assert.AreEqual(1, Geometry.BoxOnLineSide(backBox, line));
                    Assert.AreEqual(-1, Geometry.BoxOnLineSide(crossingBox, line));
                }
            }
        }

        [TestMethod]
        public void PointOnDivLineSide1()
        {
            var random = new Random(666);
            for (var i = 0; i < 1000; i++)
            {
                var startX = -1 - 666 * random.NextDouble();
                var endX = +1 + 666 * random.NextDouble();

                var pointX = 666 * random.NextDouble() - 333;
                var frontSideY = -1 - 666 * random.NextDouble();
                var backSideY = -frontSideY;

                var vertex1 = new Vertex(Fixed.FromDouble(startX), Fixed.Zero);
                var vertex2 = new Vertex(Fixed.FromDouble(endX - startX), Fixed.Zero);

                var line = new LineDef(
                    vertex1,
                    vertex2,
                    0, 0, 0, null, null);

                var divLine = new DivLine();
                divLine.MakeFrom(line);

                var x = Fixed.FromDouble(pointX);
                {
                    var y = Fixed.FromDouble(frontSideY);
                    Assert.AreEqual(0, Geometry.PointOnDivLineSide(x, y, divLine));
                }
                {
                    var y = Fixed.FromDouble(backSideY);
                    Assert.AreEqual(1, Geometry.PointOnDivLineSide(x, y, divLine));
                }
            }
        }

        [TestMethod]
        public void PointOnDivLineSide2()
        {
            var random = new Random(666);
            for (var i = 0; i < 1000; i++)
            {
                var startY = +1 + 666 * random.NextDouble();
                var endY = -1 - 666 * random.NextDouble();

                var pointY = 666 * random.NextDouble() - 333;
                var frontSideX = -1 - 666 * random.NextDouble();
                var backSideX = -frontSideX;

                var vertex1 = new Vertex(Fixed.Zero, Fixed.FromDouble(startY));
                var vertex2 = new Vertex(Fixed.Zero, Fixed.FromDouble(endY - startY));

                var line = new LineDef(
                    vertex1,
                    vertex2,
                    0, 0, 0, null, null);

                var divLine = new DivLine();
                divLine.MakeFrom(line);

                var y = Fixed.FromDouble(pointY);
                {
                    var x = Fixed.FromDouble(frontSideX);
                    Assert.AreEqual(0, Geometry.PointOnDivLineSide(x, y, divLine));
                }
                {
                    var x = Fixed.FromDouble(backSideX);
                    Assert.AreEqual(1, Geometry.PointOnDivLineSide(x, y, divLine));
                }
            }
        }

        [TestMethod]
        public void PointOnDivLineSide3()
        {
            var random = new Random(666);
            for (var i = 0; i < 1000; i++)
            {
                var startX = -1 - 666 * random.NextDouble();
                var endX = +1 + 666 * random.NextDouble();

                var pointX = 666 * random.NextDouble() - 333;
                var frontSideY = -1 - 666 * random.NextDouble();
                var backSideY = -frontSideY;

                for (var j = 0; j < 100; j++)
                {
                    var theta = 2 * Math.PI * random.NextDouble();
                    var ox = 666 * random.NextDouble() - 333;
                    var oy = 666 * random.NextDouble() - 333;

                    var vertex1 = new Vertex(
                        Fixed.FromDouble(ox + startX * Math.Cos(theta)),
                        Fixed.FromDouble(oy + startX * Math.Sin(theta)));

                    var vertex2 = new Vertex(
                        vertex1.X + Fixed.FromDouble((endX - startX) * Math.Cos(theta)),
                        vertex1.Y + Fixed.FromDouble((endX - startX) * Math.Sin(theta)));

                    var line = new LineDef(
                        vertex1,
                        vertex2,
                        0, 0, 0, null, null);

                    var divLine = new DivLine();
                    divLine.MakeFrom(line);

                    {
                        var x = Fixed.FromDouble(ox + pointX * Math.Cos(theta) - frontSideY * Math.Sin(theta));
                        var y = Fixed.FromDouble(oy + pointX * Math.Sin(theta) + frontSideY * Math.Cos(theta));
                        Assert.AreEqual(0, Geometry.PointOnDivLineSide(x, y, divLine));
                    }
                    {
                        var x = Fixed.FromDouble(ox + pointX * Math.Cos(theta) - backSideY * Math.Sin(theta));
                        var y = Fixed.FromDouble(oy + pointX * Math.Sin(theta) + backSideY * Math.Cos(theta));
                        Assert.AreEqual(1, Geometry.PointOnDivLineSide(x, y, divLine));
                    }
                }
            }
        }

        [TestMethod]
        public void AproxDistance()
        {
            var random = new Random(666);
            for (var i = 0; i < 1000; i++)
            {
                var dx = 666 * random.NextDouble() - 333;
                var dy = 666 * random.NextDouble() - 333;

                var adx = Math.Abs(dx);
                var ady = Math.Abs(dy);
                var expected = adx + ady - Math.Min(adx, ady) / 2;

                var actual = Geometry.AproxDistance(Fixed.FromDouble(dx), Fixed.FromDouble(dy));

                Assert.AreEqual(expected, actual.ToDouble(), 1.0E-3);
            }
        }

        [TestMethod]
        public void DivLineSide1()
        {
            var random = new Random(666);
            for (var i = 0; i < 1000; i++)
            {
                var startX = -1 - 666 * random.NextDouble();
                var endX = +1 + 666 * random.NextDouble();

                var pointX = 666 * random.NextDouble() - 333;
                var frontSideY = -1 - 666 * random.NextDouble();
                var backSideY = -frontSideY;

                var vertex1 = new Vertex(Fixed.FromDouble(startX), Fixed.Zero);
                var vertex2 = new Vertex(Fixed.FromDouble(endX - startX), Fixed.Zero);

                var line = new LineDef(
                    vertex1,
                    vertex2,
                    0, 0, 0, null, null);

                var divLine = new DivLine();
                divLine.MakeFrom(line);

                var node = new Node(
                    Fixed.FromDouble(startX),
                    Fixed.Zero,
                    Fixed.FromDouble(endX - startX),
                    Fixed.Zero,
                    Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                    Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                    0, 0);

                var x = Fixed.FromDouble(pointX);
                {
                    var y = Fixed.FromDouble(frontSideY);
                    Assert.AreEqual(0, Geometry.DivLineSide(x, y, divLine));
                    Assert.AreEqual(0, Geometry.DivLineSide(x, y, node));
                }
                {
                    var y = Fixed.FromDouble(backSideY);
                    Assert.AreEqual(1, Geometry.DivLineSide(x, y, divLine));
                    Assert.AreEqual(1, Geometry.DivLineSide(x, y, node));
                }
            }
        }

        [TestMethod]
        public void DivLineSide2()
        {
            var random = new Random(666);
            for (var i = 0; i < 1000; i++)
            {
                var startY = +1 + 666 * random.NextDouble();
                var endY = -1 - 666 * random.NextDouble();

                var pointY = 666 * random.NextDouble() - 333;
                var frontSideX = -1 - 666 * random.NextDouble();
                var backSideX = -frontSideX;

                var vertex1 = new Vertex(Fixed.Zero, Fixed.FromDouble(startY));
                var vertex2 = new Vertex(Fixed.Zero, Fixed.FromDouble(endY - startY));

                var line = new LineDef(
                    vertex1,
                    vertex2,
                    0, 0, 0, null, null);

                var divLine = new DivLine();
                divLine.MakeFrom(line);

                var node = new Node(
                    Fixed.Zero,
                    Fixed.FromDouble(startY),
                    Fixed.Zero,
                    Fixed.FromDouble(endY - startY),
                    Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                    Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                    0, 0);

                var y = Fixed.FromDouble(pointY);
                {
                    var x = Fixed.FromDouble(frontSideX);
                    Assert.AreEqual(0, Geometry.DivLineSide(x, y, divLine));
                    Assert.AreEqual(0, Geometry.DivLineSide(x, y, node));
                }
                {
                    var x = Fixed.FromDouble(backSideX);
                    Assert.AreEqual(1, Geometry.DivLineSide(x, y, divLine));
                    Assert.AreEqual(1, Geometry.DivLineSide(x, y, node));
                }
            }
        }

        [TestMethod]
        public void DivLineSide3()
        {
            var random = new Random(666);
            for (var i = 0; i < 1000; i++)
            {
                var startX = -1 - 666 * random.NextDouble();
                var endX = +1 + 666 * random.NextDouble();

                var pointX = 666 * random.NextDouble() - 333;
                var frontSideY = -1 - 666 * random.NextDouble();
                var backSideY = -frontSideY;

                for (var j = 0; j < 100; j++)
                {
                    var theta = 2 * Math.PI * random.NextDouble();
                    var ox = 666 * random.NextDouble() - 333;
                    var oy = 666 * random.NextDouble() - 333;

                    var vertex1 = new Vertex(
                        Fixed.FromDouble(ox + startX * Math.Cos(theta)),
                        Fixed.FromDouble(oy + startX * Math.Sin(theta)));

                    var vertex2 = new Vertex(
                        vertex1.X + Fixed.FromDouble((endX - startX) * Math.Cos(theta)),
                        vertex1.Y + Fixed.FromDouble((endX - startX) * Math.Sin(theta)));

                    var line = new LineDef(
                        vertex1,
                        vertex2,
                        0, 0, 0, null, null);

                    var divLine = new DivLine();
                    divLine.MakeFrom(line);

                    var node = new Node(
                        Fixed.FromDouble(ox + startX * Math.Cos(theta)),
                        Fixed.FromDouble(oy + startX * Math.Sin(theta)),
                        Fixed.FromDouble((endX - startX) * Math.Cos(theta)),
                        Fixed.FromDouble((endX - startX) * Math.Sin(theta)),
                        Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                        Fixed.Zero, Fixed.Zero, Fixed.Zero, Fixed.Zero,
                        0, 0);

                    {
                        var x = Fixed.FromDouble(ox + pointX * Math.Cos(theta) - frontSideY * Math.Sin(theta));
                        var y = Fixed.FromDouble(oy + pointX * Math.Sin(theta) + frontSideY * Math.Cos(theta));
                        Assert.AreEqual(0, Geometry.DivLineSide(x, y, divLine));
                        Assert.AreEqual(0, Geometry.DivLineSide(x, y, node));
                    }
                    {
                        var x = Fixed.FromDouble(ox + pointX * Math.Cos(theta) - backSideY * Math.Sin(theta));
                        var y = Fixed.FromDouble(oy + pointX * Math.Sin(theta) + backSideY * Math.Cos(theta));
                        Assert.AreEqual(1, Geometry.DivLineSide(x, y, divLine));
                        Assert.AreEqual(1, Geometry.DivLineSide(x, y, node));
                    }
                }
            }
        }
    }
}
