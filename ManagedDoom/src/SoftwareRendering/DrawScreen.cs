using System;

namespace ManagedDoom.SoftwareRendering
{
    public sealed class DrawScreen
    {
        private int width;
        private int height;
        private byte[] data;

        public DrawScreen(int width, int height)
        {
            this.width = width;
            this.height = height;
            data = new byte[width * height];
        }

        public void DrawPatch(Patch patch, int x, int y, int ratio)
        {
            var drawX = x;
            var drawWidth = ratio * patch.Width;

            var i = 0;
            var frac = Fixed.One / (2 * ratio);
            var step = Fixed.One / ratio;

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
                DrawColumn(patch.Columns[frac.ToIntFloor()], drawX + i, y, ratio);
                frac += step;
            }
        }

        private void DrawColumn(Column[] source, int x, int y, int ratio)
        {
            foreach (var column in source)
            {
                var exTopDelta = ratio * column.TopDelta;
                var exLength = ratio * column.Length;

                var sourceIndex = column.Offset;
                var drawY = y + exTopDelta;
                var drawLength = exLength;

                var i = 0;
                var p = height * x + drawY;
                var frac = Fixed.FromInt(sourceIndex) + Fixed.One / (2 * ratio);
                var step = Fixed.One / ratio;

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
                    data[p] = column.Data[frac.ToIntFloor()];
                    p++;
                    frac += step;
                }
            }
        }

        public int Width => width;
        public int Height => height;
        public byte[] Data => data;
    }
}
