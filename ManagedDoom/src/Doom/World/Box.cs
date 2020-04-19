using System;

namespace ManagedDoom
{
    public static class Box
    {
        public const int Top = 0;
        public const int Bottom = 1;
        public const int Left = 2;
        public const int Right = 3;

        public static void Clear(Fixed[] box)
        {
            box[Box.Top] = box[Box.Right] = Fixed.MinValue;
            box[Box.Bottom] = box[Box.Left] = Fixed.MaxValue;
        }

        public static void AddPoint(Fixed[] box, Fixed x, Fixed y)
        {
            if (x < box[Box.Left])
            {
                box[Box.Left] = x;
            }
            else if (x > box[Box.Right])
            {
                box[Box.Right] = x;
            }

            if (y < box[Box.Bottom])
            {
                box[Box.Bottom] = y;
            }
            else if (y > box[Box.Top])
            {
                box[Box.Top] = y;
            }
        }
    }
}
