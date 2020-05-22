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
            box[Top] = box[Right] = Fixed.MinValue;
            box[Bottom] = box[Left] = Fixed.MaxValue;
        }

        public static void AddPoint(Fixed[] box, Fixed x, Fixed y)
        {
            if (x < box[Left])
            {
                box[Left] = x;
            }
            else if (x > box[Right])
            {
                box[Right] = x;
            }

            if (y < box[Bottom])
            {
                box[Bottom] = y;
            }
            else if (y > box[Top])
            {
                box[Top] = y;
            }
        }
    }
}
