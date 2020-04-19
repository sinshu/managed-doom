using System;

namespace ManagedDoom
{
    public static partial class DoomInfo
    {
        public static class PowerDuration
        {
            public static readonly int Invulnerability = 30 * GameConstants.TicRate;
            public static readonly int Invisibility = 60 * GameConstants.TicRate;
            public static readonly int Infrared = 120 * GameConstants.TicRate;
            public static readonly int IronFeet = 60 * GameConstants.TicRate;
        }
    }
}
