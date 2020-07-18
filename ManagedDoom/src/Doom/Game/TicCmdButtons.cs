using System;

namespace ManagedDoom
{
    public static class TicCmdButtons
    {
        public static readonly byte Attack = 1;

        // Use button, to open doors, activate switches.
        public static readonly byte Use = 2;

        // Flag: game events, not really buttons.
        public static readonly byte Special = 128;
        public static readonly byte SpecialMask = 3;

        // Flag, weapon change pending.
        // If true, the next 3 bits hold weapon num.
        public static readonly byte Change = 4;

        // The 3bit weapon mask and shift, convenience.
        public static readonly byte WeaponMask = 8 + 16 + 32;
        public static readonly byte WeaponShift = 3;

        // Pause the game.
        public static readonly byte Pause = 1;
    }
}
