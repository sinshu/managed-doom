using System;

namespace ManagedDoom
{
    public static class PlayerActions
    {
        public static void Light0(Player player, PlayerSpriteDef psp)
        {
        }

        public static void WeaponReady(Player player, PlayerSpriteDef psp)
        {
            // bob the weapon based on movement speed
            var angle = (128 * player.Mobj.World.levelTime) & Trig.FineMask;
            psp.Sx = Fixed.One + player.Bob * Trig.Cos(angle);
            angle &= Trig.FineAngleCount / 2 - 1;
            psp.Sy = World.WEAPONTOP + player.Bob * Trig.Sin(angle);
        }

        public static void Lower(Player player, PlayerSpriteDef psp)
        {
        }

        public static void Raise(Player player, PlayerSpriteDef psp)
        {
            psp.Sy -= World.RAISESPEED;

            if (psp.Sy > World.WEAPONTOP)
            {
                return;
            }

            psp.Sy = World.WEAPONTOP;

            // The weapon has been raised all the way,
            //  so change to the ready state.
            var newstate = Info.WeaponInfos[(int)player.ReadyWeapon].ReadyState;

            player.Mobj.World.P_SetPsprite(player, PlayerSprite.Weapon, newstate);
        }

        public static void Punch(Player player, PlayerSpriteDef psp)
        {
        }

        public static void ReFire(Player player, PlayerSpriteDef psp)
        {
        }

        public static void FirePistol(Player player, PlayerSpriteDef psp)
        {
        }

        public static void Light1(Player player, PlayerSpriteDef psp)
        {
        }

        public static void FireShotgun(Player player, PlayerSpriteDef psp)
        {
        }

        public static void Light2(Player player, PlayerSpriteDef psp)
        {
        }

        public static void FireShotgun2(Player player, PlayerSpriteDef psp)
        {
        }

        public static void CheckReload(Player player, PlayerSpriteDef psp)
        {
        }

        public static void OpenShotgun2(Player player, PlayerSpriteDef psp)
        {
        }

        public static void LoadShotgun2(Player player, PlayerSpriteDef psp)
        {
        }

        public static void CloseShotgun2(Player player, PlayerSpriteDef psp)
        {
        }

        public static void FireCGun(Player player, PlayerSpriteDef psp)
        {
        }

        public static void GunFlash(Player player, PlayerSpriteDef psp)
        {
        }

        public static void FireMissile(Player player, PlayerSpriteDef psp)
        {
        }

        public static void Saw(Player player, PlayerSpriteDef psp)
        {
        }

        public static void FirePlasma(Player player, PlayerSpriteDef psp)
        {
        }

        public static void BFGsound(Player player, PlayerSpriteDef psp)
        {
        }

        public static void FireBFG(Player player, PlayerSpriteDef psp)
        {
        }
    }
}
