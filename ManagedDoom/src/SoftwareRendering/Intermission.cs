using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace ManagedDoom.SoftwareRendering
{
    public sealed class Intermission
    {
        private int screenWidth;
        private int screenHeight;
        private byte[] screenData;

        private int ratio;

        public Intermission(
           CommonPatches patches,
           int screenWidth,
           int screenHeight,
           byte[] screenData)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.screenData = screenData;

            ratio = screenWidth / 320;
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

            if (drawX + drawWidth > screenWidth)
            {
                var exceed = drawX + drawWidth - screenWidth;
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
                var p = screenHeight * x + drawY;
                var frac = Fixed.FromInt(sourceIndex) + Fixed.One / (2 * ratio);
                var step = Fixed.One / ratio;

                if (drawY < 0)
                {
                    var exceed = -drawY;
                    p += exceed;
                    frac += exceed * step;
                    i += exceed;
                }

                if (drawY + drawLength > screenHeight)
                {
                    var exceed = drawY + drawLength - screenHeight;
                    drawLength -= exceed;
                }

                for (; i < drawLength; i++)
                {
                    screenData[p] = column.Data[frac.ToIntFloor()];
                    p++;
                    frac += step;
                }
            }
        }
    }
}
