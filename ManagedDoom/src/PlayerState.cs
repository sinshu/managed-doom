using System;

namespace ManagedDoom
{
    public enum PlayerState
    {
        // Playing or camping.
        Live,

        // Dead on the ground, view follows killer.
        Dead,

        // Ready to restart/respawn???
        Reborn
    }
}
