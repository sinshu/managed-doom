using System;
using System.Collections.Generic;

namespace ManagedDoom
{
    public static partial class DoomInfo
    {
        public static class ParTimes
        {
            public static readonly IReadOnlyList<IReadOnlyList<int>> Doom1 = new int[][]
            {
                new int[] {  30,  75, 120,  90, 165, 180, 180,  30, 165 },
                new int[] {  90,  90,  90, 120,  90, 360, 240,  30, 170 },
                new int[] {  90,  45,  90, 150,  90,  90, 165,  30, 135 },
                new int[] { 165, 255, 135, 150, 180, 390, 135, 360, 180 }
            };

            public static readonly IReadOnlyList<int> Doom2 = new int[]
            {
                30,   90, 120, 120,  90, 150, 120, 120, 270,  90,
                210, 150, 150, 150, 210, 150, 420, 150, 210, 150,
                240, 150, 180, 150, 150, 300, 330, 420, 300, 180,
                120,  30
            };
        }
    }
}
