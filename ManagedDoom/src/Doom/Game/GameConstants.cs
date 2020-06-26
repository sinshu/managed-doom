using System;

namespace ManagedDoom
{
    public static class GameConstants
    {
        public static readonly int TicRate = 35;

        public static readonly Fixed MaxThingRadius = Fixed.FromInt(32);

        public static readonly Fixed TURBOTHRESHOLD = new Fixed(0x32);
    }
}
