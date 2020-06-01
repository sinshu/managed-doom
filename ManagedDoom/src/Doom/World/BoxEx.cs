using System;
using System.Runtime.CompilerServices;

namespace ManagedDoom
{
    public static class BoxEx
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed Top(this Fixed[] box)
        {
            return box[Box.Top];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed Bottom(this Fixed[] box)
        {
            return box[Box.Bottom];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed Left(this Fixed[] box)
        {
            return box[Box.Left];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Fixed Right(this Fixed[] box)
        {
            return box[Box.Right];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Top(this int[] box)
        {
            return box[Box.Top];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Bottom(this int[] box)
        {
            return box[Box.Bottom];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Left(this int[] box)
        {
            return box[Box.Left];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Right(this int[] box)
        {
            return box[Box.Right];
        }
    }
}
