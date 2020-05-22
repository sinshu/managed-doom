using System;

namespace ManagedDoom
{
    public static class BoxEx
    {
        public static Fixed Top(this Fixed[] box)
        {
            return box[Box.Top];
        }

        public static Fixed Bottom(this Fixed[] box)
        {
            return box[Box.Bottom];
        }

        public static Fixed Left(this Fixed[] box)
        {
            return box[Box.Left];
        }

        public static Fixed Right(this Fixed[] box)
        {
            return box[Box.Right];
        }
    }
}
