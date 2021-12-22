using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest.UnitTests
{
    [TestClass]
    public class MapTest
    {
        private static readonly double maxRadius = 32;

        [TestMethod]
        public void LoadE1M1()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom1))
            {
                var options = new GameOptions();
                var world = new World(content, options);
                var map = new Map(content, world);

                var mapMinX = map.Lines.Min(line => Fixed.Min(line.Vertex1.X, line.Vertex2.X).ToDouble());
                var mapMaxX = map.Lines.Max(line => Fixed.Max(line.Vertex1.X, line.Vertex2.X).ToDouble());
                var mapMinY = map.Lines.Min(line => Fixed.Min(line.Vertex1.Y, line.Vertex2.Y).ToDouble());
                var mapMaxY = map.Lines.Max(line => Fixed.Max(line.Vertex1.Y, line.Vertex2.Y).ToDouble());

                for (var i = 0; i < map.Sectors.Length; i++)
                {
                    var sector = map.Sectors[i];
                    var sLines = map.Lines.Where(line => line.FrontSector == sector || line.BackSector == sector).ToArray();

                    CollectionAssert.AreEqual(sLines, sector.Lines);

                    var minX = sLines.Min(line => Fixed.Min(line.Vertex1.X, line.Vertex2.X).ToDouble()) - maxRadius;
                    minX = Math.Max(minX, mapMinX);
                    var maxX = sLines.Max(line => Fixed.Max(line.Vertex1.X, line.Vertex2.X).ToDouble()) + maxRadius;
                    maxX = Math.Min(maxX, mapMaxX);
                    var minY = sLines.Min(line => Fixed.Min(line.Vertex1.Y, line.Vertex2.Y).ToDouble()) - maxRadius;
                    minY = Math.Max(minY, mapMinY);
                    var maxY = sLines.Max(line => Fixed.Max(line.Vertex1.Y, line.Vertex2.Y).ToDouble()) + maxRadius;
                    maxY = Math.Min(maxY, mapMaxY);

                    var bboxTop = (map.BlockMap.OriginY + BlockMap.BlockSize * (sector.BlockBox[Box.Top] + 1)).ToDouble();
                    var bboxBottom = (map.BlockMap.OriginY + BlockMap.BlockSize * sector.BlockBox[Box.Bottom]).ToDouble();
                    var bboxLeft = (map.BlockMap.OriginX + BlockMap.BlockSize * sector.BlockBox[Box.Left]).ToDouble();
                    var bboxRight = (map.BlockMap.OriginX + BlockMap.BlockSize * (sector.BlockBox[Box.Right] + 1)).ToDouble();

                    Assert.IsTrue(bboxLeft <= minX);
                    Assert.IsTrue(bboxRight >= maxX);
                    Assert.IsTrue(bboxTop >= maxY);
                    Assert.IsTrue(bboxBottom <= minY);

                    Assert.IsTrue(Math.Abs(bboxLeft - minX) <= 128);
                    Assert.IsTrue(Math.Abs(bboxRight - maxX) <= 128);
                    Assert.IsTrue(Math.Abs(bboxTop - maxY) <= 128);
                    Assert.IsTrue(Math.Abs(bboxBottom - minY) <= 128);
                }
            }
        }

        [TestMethod]
        public void LoadMap01()
        {
            using (var content = GameContent.CreateDummy(WadPath.Doom2))
            {
                var options = new GameOptions();
                var world = new World(content, options);
                var map = new Map(content, world);

                var mapMinX = map.Lines.Min(line => Fixed.Min(line.Vertex1.X, line.Vertex2.X).ToDouble());
                var mapMaxX = map.Lines.Max(line => Fixed.Max(line.Vertex1.X, line.Vertex2.X).ToDouble());
                var mapMinY = map.Lines.Min(line => Fixed.Min(line.Vertex1.Y, line.Vertex2.Y).ToDouble());
                var mapMaxY = map.Lines.Max(line => Fixed.Max(line.Vertex1.Y, line.Vertex2.Y).ToDouble());

                for (var i = 0; i < map.Sectors.Length; i++)
                {
                    var sector = map.Sectors[i];
                    var sLines = map.Lines.Where(line => line.FrontSector == sector || line.BackSector == sector).ToArray();

                    CollectionAssert.AreEqual(sLines, sector.Lines);

                    var minX = sLines.Min(line => Fixed.Min(line.Vertex1.X, line.Vertex2.X).ToDouble()) - maxRadius;
                    minX = Math.Max(minX, mapMinX);
                    var maxX = sLines.Max(line => Fixed.Max(line.Vertex1.X, line.Vertex2.X).ToDouble()) + maxRadius;
                    maxX = Math.Min(maxX, mapMaxX);
                    var minY = sLines.Min(line => Fixed.Min(line.Vertex1.Y, line.Vertex2.Y).ToDouble()) - maxRadius;
                    minY = Math.Max(minY, mapMinY);
                    var maxY = sLines.Max(line => Fixed.Max(line.Vertex1.Y, line.Vertex2.Y).ToDouble()) + maxRadius;
                    maxY = Math.Min(maxY, mapMaxY);

                    var bboxTop = (map.BlockMap.OriginY + BlockMap.BlockSize * (sector.BlockBox[Box.Top] + 1)).ToDouble();
                    var bboxBottom = (map.BlockMap.OriginY + BlockMap.BlockSize * sector.BlockBox[Box.Bottom]).ToDouble();
                    var bboxLeft = (map.BlockMap.OriginX + BlockMap.BlockSize * sector.BlockBox[Box.Left]).ToDouble();
                    var bboxRight = (map.BlockMap.OriginX + BlockMap.BlockSize * (sector.BlockBox[Box.Right] + 1)).ToDouble();

                    Assert.IsTrue(bboxLeft <= minX);
                    Assert.IsTrue(bboxRight >= maxX);
                    Assert.IsTrue(bboxTop >= maxY);
                    Assert.IsTrue(bboxBottom <= minY);

                    Assert.IsTrue(Math.Abs(bboxLeft - minX) <= 128);
                    Assert.IsTrue(Math.Abs(bboxRight - maxX) <= 128);
                    Assert.IsTrue(Math.Abs(bboxTop - maxY) <= 128);
                    Assert.IsTrue(Math.Abs(bboxBottom - minY) <= 128);
                }
            }
        }
    }
}
