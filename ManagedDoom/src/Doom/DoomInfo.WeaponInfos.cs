using System;

namespace ManagedDoom
{
    public static partial class DoomInfo
    {
        public static readonly WeaponInfo[] WeaponInfos = new WeaponInfo[]
        {
            // fist
            new WeaponInfo(
                AmmoType.NoAmmo,
                State.Punchup,
                State.Punchdown,
                State.Punch,
                State.Punch1,
                State.Null),

            // pistol
            new WeaponInfo(
                AmmoType.Clip,
                State.Pistolup,
                State.Pistoldown,
                State.Pistol,
                State.Pistol1,
                State.Pistolflash),

            // shotgun
            new WeaponInfo(
                AmmoType.Shell,
                State.Sgunup,
                State.Sgundown,
                State.Sgun,
                State.Sgun1,
                State.Sgunflash1),

            // chaingun
            new WeaponInfo(
                AmmoType.Clip,
                State.Chainup,
                State.Chaindown,
                State.Chain,
                State.Chain1,
                State.Chainflash1),

            // missile launcher
            new WeaponInfo(
                AmmoType.Missile,
                State.Missileup,
                State.Missiledown,
                State.Missile,
                State.Missile1,
                State.Missileflash1),

            // plasma rifle
            new WeaponInfo(
                AmmoType.Cell,
                State.Plasmaup,
                State.Plasmadown,
                State.Plasma,
                State.Plasma1,
                State.Plasmaflash1),

            // bfg 9000
            new WeaponInfo(
                AmmoType.Cell,
                State.Bfgup,
                State.Bfgdown,
                State.Bfg,
                State.Bfg1,
                State.Bfgflash1),

            // chainsaw
            new WeaponInfo(
                AmmoType.Cell,
                State.Sawup,
                State.Sawdown,
                State.Saw,
                State.Saw1,
                State.Null),

            // // super shotgun
            new WeaponInfo(
                AmmoType.Shell,
                State.Dsgunup,
                State.Dsgundown,
                State.Dsgun,
                State.Dsgun1,
                State.Dsgunflash1)
        };
    }
}
