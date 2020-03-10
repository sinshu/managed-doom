using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagedDoom;

namespace ManagedDoomTest
{
    [TestClass]
    public class MapTest
    {
        private static readonly double maxRadius = 32;

        [TestMethod]
        public void LoadE1M1()
        {
            using (var wad = new Wad(WadPath.Doom1))
            {
                var flats = new FlatLookup(wad);
                var textures = new TextureLookup(wad);
                var map = new Map(wad, textures, flats, "E1M1");

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

                    var bboxTop = (map.BlockMap.OriginY + BlockMap.MapBlockSize * (sector.BlockBox[Box.Top] + 1)).ToDouble();
                    var bboxBottom = (map.BlockMap.OriginY + BlockMap.MapBlockSize * sector.BlockBox[Box.Bottom]).ToDouble();
                    var bboxLeft = (map.BlockMap.OriginX + BlockMap.MapBlockSize * sector.BlockBox[Box.Left]).ToDouble();
                    var bboxRight = (map.BlockMap.OriginX + BlockMap.MapBlockSize * (sector.BlockBox[Box.Right] + 1)).ToDouble();

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
            using (var wad = new Wad(WadPath.Doom2))
            {
                var flats = new FlatLookup(wad);
                var textures = new TextureLookup(wad);
                var map = new Map(wad, textures, flats, "MAP01");

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

                    var bboxTop = (map.BlockMap.OriginY + BlockMap.MapBlockSize * (sector.BlockBox[Box.Top] + 1)).ToDouble();
                    var bboxBottom = (map.BlockMap.OriginY + BlockMap.MapBlockSize * sector.BlockBox[Box.Bottom]).ToDouble();
                    var bboxLeft = (map.BlockMap.OriginX + BlockMap.MapBlockSize * sector.BlockBox[Box.Left]).ToDouble();
                    var bboxRight = (map.BlockMap.OriginX + BlockMap.MapBlockSize * (sector.BlockBox[Box.Right] + 1)).ToDouble();

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
