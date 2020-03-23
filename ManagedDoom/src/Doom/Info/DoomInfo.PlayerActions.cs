using System;

namespace ManagedDoom
{
    public static partial class DoomInfo
    {
        private class PlayerActions
        {
            public void Light0(World world, Player player, PlayerSpriteDef psp)
            {
                world.WeaponBehavior.Light0(player);
            }

            public void WeaponReady(World world, Player player, PlayerSpriteDef psp)
            {
                world.WeaponBehavior.WeaponReady(player, psp);
            }

            public void Lower(World world, Player player, PlayerSpriteDef psp)
            {
                world.WeaponBehavior.Lower(player, psp);
            }

            public void Raise(World world, Player player, PlayerSpriteDef psp)
            {
                world.WeaponBehavior.Raise(player, psp);
            }

            public void Punch(World world, Player player, PlayerSpriteDef psp)
            {
                world.WeaponBehavior.Punch(player);
            }

            public void ReFire(World world, Player player, PlayerSpriteDef psp)
            {
                world.WeaponBehavior.ReFire(player, psp);
            }

            public void FirePistol(World world, Player player, PlayerSpriteDef psp)
            {
                world.WeaponBehavior.FirePistol(player);
            }

            public void Light1(World world, Player player, PlayerSpriteDef psp)
            {
                world.WeaponBehavior.Light1(player);
            }

            public void FireShotgun(World world, Player player, PlayerSpriteDef psp)
            {
                world.WeaponBehavior.FireShotgun(player, psp);
            }

            public void Light2(World world, Player player, PlayerSpriteDef psp)
            {
                world.WeaponBehavior.Light2(player);
            }

            public void FireShotgun2(World world, Player player, PlayerSpriteDef psp)
            {
            }

            public void CheckReload(World world, Player player, PlayerSpriteDef psp)
            {
            }

            public void OpenShotgun2(World world, Player player, PlayerSpriteDef psp)
            {
            }

            public void LoadShotgun2(World world, Player player, PlayerSpriteDef psp)
            {
            }

            public void CloseShotgun2(World world, Player player, PlayerSpriteDef psp)
            {
            }

            public void FireCGun(World world, Player player, PlayerSpriteDef psp)
            {
                world.WeaponBehavior.FireCGun(player, psp);
            }

            public void GunFlash(World world, Player player, PlayerSpriteDef psp)
            {
            }

            public void FireMissile(World world, Player player, PlayerSpriteDef psp)
            {
            }

            public void Saw(World world, Player player, PlayerSpriteDef psp)
            {
            }

            public void FirePlasma(World world, Player player, PlayerSpriteDef psp)
            {
            }

            public void BFGsound(World world, Player player, PlayerSpriteDef psp)
            {
            }

            public void FireBFG(World world, Player player, PlayerSpriteDef psp)
            {
            }
        }
    }
}
