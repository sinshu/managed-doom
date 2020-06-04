using System;

namespace ManagedDoom
{
    public enum CheatFlags
    {
        // No clipping, walk through barriers.
        NoClip = 1,

        // No damage, no health loss.
        GodMode = 2,

        // Not really a cheat, just a debug aid.
        NoMomentum = 4
    }
}
