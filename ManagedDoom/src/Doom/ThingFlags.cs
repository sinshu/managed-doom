using System;

namespace ManagedDoom
{
    [Flags]
    public enum ThingFlags
    {
        Easy = 1,
        Normal = 2,
        Hard = 4,
        Ambush = 8
    }
}
