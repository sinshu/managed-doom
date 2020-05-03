using System;

namespace ManagedDoom
{
    public static partial class DoomInfo
    {
        public static class FaceInfos
        {
            public static readonly int ST_NUMPAINFACES = 5;
            public static readonly int ST_NUMSTRAIGHTFACES = 3;
            public static readonly int ST_NUMTURNFACES = 2;
            public static readonly int ST_NUMSPECIALFACES = 3;

            public static readonly int ST_FACESTRIDE = (ST_NUMSTRAIGHTFACES + ST_NUMTURNFACES + ST_NUMSPECIALFACES);

            public static readonly int ST_NUMEXTRAFACES = 2;

            public static readonly int ST_NUMFACES = (ST_FACESTRIDE * ST_NUMPAINFACES + ST_NUMEXTRAFACES);

            public static readonly int ST_TURNOFFSET = (ST_NUMSTRAIGHTFACES);
            public static readonly int ST_OUCHOFFSET = (ST_TURNOFFSET + ST_NUMTURNFACES);
            public static readonly int ST_EVILGRINOFFSET = (ST_OUCHOFFSET + 1);
            public static readonly int ST_RAMPAGEOFFSET = (ST_EVILGRINOFFSET + 1);
            public static readonly int ST_GODFACE = (ST_NUMPAINFACES * ST_FACESTRIDE);
            public static readonly int ST_DEADFACE = (ST_GODFACE + 1);

            public static readonly int ST_EVILGRINCOUNT = (2 * GameConstants.TicRate);
            public static readonly int ST_STRAIGHTFACECOUNT = (GameConstants.TicRate / 2);
            public static readonly int ST_TURNCOUNT = (1 * GameConstants.TicRate);
            public static readonly int ST_OUCHCOUNT = (1 * GameConstants.TicRate);
            public static readonly int ST_RAMPAGEDELAY = (2 * GameConstants.TicRate);

            public static readonly int ST_MUCHPAIN = 20;
        }
    }
}
