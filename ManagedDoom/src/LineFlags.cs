using System;

namespace ManagedDoom
{
    [Flags]
    public enum LineFlags
    {
        Blocking = 1,
        BlockMonsters = 2,
        TwoSided = 4,
        DontPegTop = 8,
        DontPegBottom = 16,
        Secret = 32,
        SoundBlock = 64,
        DontDraw = 128,
        Mapped = 256
    }
}
