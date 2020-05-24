using SFML.Graphics;
using System;
using System.Collections.Generic;

namespace ManagedDoom.SoftwareRendering
{
    public sealed class DrawScreen
    {
        private int width;
        private int height;
        private byte[] data;

        private Patch[] chars;

        public DrawScreen(Wad wad, int width, int height)
        {
            this.width = width;
            this.height = height;
            data = new byte[width * height];

            chars = new Patch[128];
            for (var i = 0; i < chars.Length; i++)
            {
                var name = "STCFN" + i.ToString("000");
                var lump = wad.GetLumpNumber(name);
                if (lump != -1)
                {
                    chars[i] = Patch.FromData(name, wad.ReadLump(lump));
                }
            }
        }

        public void DrawPatch(Patch patch, int x, int y, int scale)
        {
            var drawX = x - scale * patch.LeftOffset;
            var drawY = y - scale * patch.TopOffset;
            var drawWidth = scale * patch.Width;

            var i = 0;
            var frac = Fixed.One / scale - Fixed.Epsilon;
            var step = Fixed.One / scale;

            if (drawX < 0)
            {
                var exceed = -drawX;
                frac += exceed * step;
                i += exceed;
            }

            if (drawX + drawWidth > width)
            {
                var exceed = drawX + drawWidth - width;
                drawWidth -= exceed;
            }

            for (; i < drawWidth; i++)
            {
                DrawColumn(patch.Columns[frac.ToIntFloor()], drawX + i, drawY, scale);
                frac += step;
            }
        }

        private void DrawColumn(Column[] source, int x, int y, int scale)
        {
            var step = Fixed.One / scale;

            foreach (var column in source)
            {
                var exTopDelta = scale * column.TopDelta;
                var exLength = scale * column.Length;

                var sourceIndex = column.Offset;
                var drawY = y + exTopDelta;
                var drawLength = exLength;

                var i = 0;
                var p = height * x + drawY;
                var frac = Fixed.One / scale - Fixed.Epsilon;

                if (drawY < 0)
                {
                    var exceed = -drawY;
                    p += exceed;
                    frac += exceed * step;
                    i += exceed;
                }

                if (drawY + drawLength > height)
                {
                    var exceed = drawY + drawLength - height;
                    drawLength -= exceed;
                }

                for (; i < drawLength; i++)
                {
                    data[p] = column.Data[sourceIndex + frac.ToIntFloor()];
                    p++;
                    frac += step;
                }
            }
        }

        public void DrawText(IReadOnlyList<char> text, int x, int y, int scale)
        {
            var drawX = x;
            var drawY = y - 7 * scale;
            foreach (var ch in text)
            {
                if (ch >= chars.Length)
                {
                    continue;
                }

                if (ch == 32)
                {
                    drawX += 4 * scale;
                    continue;
                }

                var index = (int)ch;
                if ('a' <= index && index <= 'z')
                {
                    index = index - 'a' + 'A';
                }

                var patch = chars[index];
                if (patch == null)
                {
                    continue;
                }

                DrawPatch(patch, drawX, drawY, scale);

                drawX += scale * patch.Width;
            }
        }

        public void DrawText(string text, int x, int y, int scale)
        {
            var drawX = x;
            var drawY = y - 7 * scale;
            foreach (var ch in text)
            {
                if (ch >= chars.Length)
                {
                    continue;
                }

                if (ch == 32)
                {
                    drawX += 4 * scale;
                    continue;
                }

                var index = (int)ch;
                if ('a' <= index && index <= 'z')
                {
                    index = index - 'a' + 'A';
                }

                var patch = chars[index];
                if (patch == null)
                {
                    continue;
                }

                DrawPatch(patch, drawX, drawY, scale);

                drawX += scale * patch.Width;
            }
        }

        public int MeasureText(IReadOnlyList<char> text, int scale)
        {
            var width = 0;

            foreach (var ch in text)
            {
                if (ch >= chars.Length)
                {
                    continue;
                }

                if (ch == 32)
                {
                    width += 4 * scale;
                    continue;
                }

                var index = (int)ch;
                if ('a' <= index && index <= 'z')
                {
                    index = index - 'a' + 'A';
                }

                var patch = chars[index];
                if (patch == null)
                {
                    continue;
                }

                width += scale * patch.Width;
            }

            return width;
        }

        public int MeasureText(string text, int scale)
        {
            var width = 0;

            foreach (var ch in text)
            {
                if (ch >= chars.Length)
                {
                    continue;
                }

                if (ch == 32)
                {
                    width += 4 * scale;
                    continue;
                }

                var index = (int)ch;
                if ('a' <= index && index <= 'z')
                {
                    index = index - 'a' + 'A';
                }

                var patch = chars[index];
                if (patch == null)
                {
                    continue;
                }

                width += scale * patch.Width;
            }

            return width;
        }

        public void DrawLine(int x1, int y1, int x2, int y2, int color)
        {
            var dx = x2 - x1;
            var ax = 2 * (dx < 0 ? -dx : dx);
            var sx = dx < 0 ? -1 : 1;

            var dy = y2 - y1;
            var ay = 2 * (dy < 0 ? -dy : dy);
            var sy = dy < 0 ? -1 : 1;

            var x = x1;
            var y = y1;

            if (ax > ay)
            {
                var d = ay - ax / 2;

                while (true)
                {
                    data[height * x + y] = (byte)color;

                    if (x == x2)
                    {
                        return;
                    }

                    if (d >= 0)
                    {
                        y += sy;
                        d -= ax;
                    }

                    x += sx;
                    d += ay;
                }
            }
            else
            {
                var d = ax - ay / 2;
                while (true)
                {
                    data[height * x + y] = (byte)color;

                    if (y == y2)
                    {
                        return;
                    }

                    if (d >= 0)
                    {
                        x += sx;
                        d -= ay;
                    }

                    y += sy;
                    d += ax;
                }
            }
        }

        public int Width => width;
        public int Height => height;
        public byte[] Data => data;
    }
}
