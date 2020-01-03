using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest
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
            using (var wad = new Wad(WadPath.Doom1))
            {
                var flats = new FlatLookup(wad);
                var textures = new TextureLookup(wad);
                var map = wad.GetLumpNumber("E1M1");
                var vertices = Vertex.FromWad(wad, map + 4);
                var sectors = Sector.FromWad(wad, map + 8, flats);
                var sides = SideDef.FromWad(wad, map + 3, textures, sectors);
                var lines = LineDef.FromWad(wad, map + 2, vertices, sides);
                var segs = Seg.FromWad(wad, map + 5, vertices, lines);
                var subsectors = Subsector.FromWad(wad, map + 6, segs);
                var nodes = Node.FromWad(wad, map + 7, subsectors);

                var ok = 0;
                var count = 0;

                foreach (var subsector in subsectors)
                {
                    for (var i = 0; i < subsector.SegCount; i++)
                    {
                        var seg = segs[subsector.FirstSeg + i];

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

                        var result = Geometry.PointInSubsector(fx, fy, nodes, subsectors);

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
            using (var wad = new Wad(WadPath.Doom2))
            {
                var flats = new FlatLookup(wad);
                var textures = new TextureLookup(wad);
                var map = wad.GetLumpNumber("MAP01");
                var vertices = Vertex.FromWad(wad, map + 4);
                var sectors = Sector.FromWad(wad, map + 8, flats);
                var sides = SideDef.FromWad(wad, map + 3, textures, sectors);
                var lines = LineDef.FromWad(wad, map + 2, vertices, sides);
                var segs = Seg.FromWad(wad, map + 5, vertices, lines);
                var subsectors = Subsector.FromWad(wad, map + 6, segs);
                var nodes = Node.FromWad(wad, map + 7, subsectors);

                var ok = 0;
                var count = 0;

                foreach (var subsector in subsectors)
                {
                    for (var i = 0; i < subsector.SegCount; i++)
                    {
                        var seg = segs[subsector.FirstSeg + i];

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

                        var result = Geometry.PointInSubsector(fx, fy, nodes, subsectors);

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
    }
}
