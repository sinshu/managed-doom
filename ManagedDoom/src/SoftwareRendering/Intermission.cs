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

        public void DrawTest(Patch patch, int x, int y)
        {
            var x1 = x;
            var x2 = x + patch.Width;
            var c1 = 0;
            var c2 = patch.Width;

            if (x1 < 0)
            {
                c1 -= x1;
                x1 = 0;
            }

            if (x2 > screenWidth)
            {
                c2 -= x2 - screenWidth;
                x2 = screenWidth;
            }

            x = x1;
            for (var c = c1; c < c2; c++)
            {
                for (var i = 0; i < ratio; i++)
                {
                    DrawColumn(patch.Columns[c], ratio * x + i, ratio * y, ratio);
                }
                x++;
            }
        }

        private void DrawColumn(Column[] source, int x, int y, int ratio)
        {
            foreach (var column in source)
            {
                var colTopDelta = ratio * column.TopDelta;
                var colLength = ratio * column.Length;

                var sourceIndex = column.Offset;
                var drawY = y + colTopDelta;
                var length = colLength;

                var topExceedance = -(y + colTopDelta);
                if (topExceedance > 0)
                {
                    sourceIndex += topExceedance;
                    drawY += topExceedance;
                    length -= topExceedance;
                }

                var bottomExceedance = y + colTopDelta + colLength - screenHeight;
                if (bottomExceedance > 0)
                {
                    length -= bottomExceedance;
                }

                if (length > 0)
                {
                    var p = screenHeight * x + drawY;
                    var frac = Fixed.FromInt(sourceIndex);
                    var step = Fixed.One / ratio;
                    for (var i = 0; i < length; i++)
                    {
                        screenData[p] = column.Data[frac.ToIntFloor()];
                        p++;
                        frac += step;
                    }
                }
            }
        }
    }
}
