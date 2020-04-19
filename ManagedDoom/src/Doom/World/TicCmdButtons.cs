using System;

namespace ManagedDoom
{
    public static class TicCmdButtons
    {
        public const int Attack = 1;

        // Use button, to open doors, activate switches.
        public const int Use = 2;

        // Flag: game events, not really buttons.
        public const int Special = 128;
        public const int SpecialMask = 3;

        // Flag, weapon change pending.
        // If true, the next 3 bits hold weapon num.
        public const int Change = 4;

        // The 3bit weapon mask and shift, convenience.
        public const int WeaponMask = 8 + 16 + 32;
        public const int WeaponShift = 3;

        // Pause the game.
        public const int Pause = 1;

        // Save the game at each console.
        public const int SaveGame = 2;

        // Savegame slot numbers
        //  occupy the second byte of buttons.    
        public const int SaveMask = 4 + 8 + 16;
        public const int SaveShift = 2;
    }
}
