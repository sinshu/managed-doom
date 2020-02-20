using System;

namespace ManagedDoom
{
    public enum AmmoType
    {
        // Pistol / chaingun ammo.
        Clip,

        // Shotgun / double barreled shotgun.
        Shell,

        // Plasma rifle, BFG.
        Cell,

        // Missile launcher.
        Missile,

        Count,

        // Unlimited for chainsaw / fist.
        NoAmmo
    }
}
