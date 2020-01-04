using System;

namespace ManagedDoom
{
    public enum GameMode
    {
        Shareware,  // DOOM 1 shareware, E1, M9
        Registered, // DOOM 1 registered, E3, M27
        Commercial, // DOOM 2 retail, E1 M34
                    // DOOM 2 german edition not handled
        Retail, // DOOM 1 retail, E4, M36
        Indetermined	// Well, no IWAD found.
    }
}
