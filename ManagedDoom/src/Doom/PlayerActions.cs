using System;

namespace ManagedDoom
{
    public static class PlayerActions
    {
        public static void Light0(Player player, PlayerSpriteDef psp)
        {
            player.ExtraLight = 0;
        }

        public static void WeaponReady(Player player, PlayerSpriteDef psp)
        {
            var world = player.Mobj.World;

            // get out of attack state
            if (player.Mobj.State == Info.States[(int)State.PlayAtk1]
                || player.Mobj.State == Info.States[(int)State.PlayAtk2])
            {
                world.SetMobjState(player.Mobj, State.Play);
            }

            if (player.ReadyWeapon == WeaponType.Chainsaw
                && psp.State == Info.States[(int)State.Saw])
            {
                world.StartSound(player.Mobj, Sfx.SAWIDL);
            }

            // check for change
            //  if player is dead, put the weapon away
            if (player.PendingWeapon != WeaponType.NoChange || player.Health == 0)
            {
                // change weapon
                //  (pending weapon should allready be validated)
                var newstate = Info.WeaponInfos[(int)player.ReadyWeapon].DownState;
                world.P_SetPsprite(player, PlayerSprite.Weapon, newstate);
                return;
            }

            // check for fire
            //  the missile launcher and bfg do not auto fire
            if ((player.Cmd.Buttons & Buttons.Attack) != 0)
            {
                if (!player.AttackDown
                    || (player.ReadyWeapon != WeaponType.Missile
                        && player.ReadyWeapon != WeaponType.Bfg))
                {
                    player.AttackDown = true;
                    FireWeapon(player);
                    return;
                }
            }
            else
            {
                player.AttackDown = false;
            }

            // bob the weapon based on movement speed
            var angle = (128 * player.Mobj.World.levelTime) & Trig.FineMask;
            psp.Sx = Fixed.One + player.Bob * Trig.Cos(angle);
            angle &= Trig.FineAngleCount / 2 - 1;
            psp.Sy = World.WEAPONTOP + player.Bob * Trig.Sin(angle);
        }

        private static readonly int BFGCELLS = 40;

        //
        // P_CheckAmmo
        // Returns true if there is enough ammo to shoot.
        // If not, selects the next weapon to use.
        //
        private static bool CheckAmmo(Player player)
        {
            var world = player.Mobj.World;

            var ammo = Info.WeaponInfos[(int)player.ReadyWeapon].Ammo;

            // Minimal amount for one shot varies.
            int count;
            if (player.ReadyWeapon == WeaponType.Bfg)
            {
                count = BFGCELLS;
            }
            else if (player.ReadyWeapon == WeaponType.SuperShotgun)
            {
                // Double barrel.
                count = 2;
            }
            else
            {
                // Regular.
                count = 1;
            }

            // Some do not need ammunition anyway.
            // Return if current ammunition sufficient.
            if (ammo == AmmoType.NoAmmo || player.Ammo[(int)ammo] >= count)
            {
                return true;
            }

            // Out of ammo, pick a weapon to change to.
            // Preferences are set here.
            do
            {
                if (player.WeaponOwned[(int)WeaponType.Plasma]
                    && player.Ammo[(int)AmmoType.Cell] > 0
                    && (world.Options.GameMode != GameMode.Shareware))
                {
                    player.PendingWeapon = WeaponType.Plasma;
                }
                else if (player.WeaponOwned[(int)WeaponType.SuperShotgun]
                    && player.Ammo[(int)AmmoType.Shell] > 2
                    && (world.Options.GameMode == GameMode.Commercial))
                {
                    player.PendingWeapon = WeaponType.SuperShotgun;
                }
                else if (player.WeaponOwned[(int)WeaponType.Chaingun]
                    && player.Ammo[(int)AmmoType.Clip] > 0)
                {
                    player.PendingWeapon = WeaponType.Chaingun;
                }
                else if (player.WeaponOwned[(int)WeaponType.Shotgun]
                    && player.Ammo[(int)AmmoType.Shell] > 0)
                {
                    player.PendingWeapon = WeaponType.Shotgun;
                }
                else if (player.Ammo[(int)AmmoType.Clip] > 0)
                {
                    player.PendingWeapon = WeaponType.Pistol;
                }
                else if (player.WeaponOwned[(int)WeaponType.Chainsaw])
                {
                    player.PendingWeapon = WeaponType.Chainsaw;
                }
                else if (player.WeaponOwned[(int)WeaponType.Missile]
                    && player.Ammo[(int)AmmoType.Missile] > 0)
                {
                    player.PendingWeapon = WeaponType.Missile;
                }
                else if (player.WeaponOwned[(int)WeaponType.Bfg]
                    && player.Ammo[(int)AmmoType.Cell] > 40
                    && (world.Options.GameMode != GameMode.Shareware))
                {
                    player.PendingWeapon = WeaponType.Bfg;
                }
                else
                {
                    // If everything fails.
                    player.PendingWeapon = WeaponType.Fist;
                }

            } while (player.PendingWeapon == WeaponType.NoChange);

            // Now set appropriate weapon overlay.
            world.P_SetPsprite(
                player,
                PlayerSprite.Weapon,
                Info.WeaponInfos[(int)player.ReadyWeapon].DownState);

            return false;
        }


        //
        // P_FireWeapon.
        //
        private static void FireWeapon(Player player)
        {
            var world = player.Mobj.World;

            if (!CheckAmmo(player))
            {
                return;
            }

            world.SetMobjState(player.Mobj, State.PlayAtk1);
            var newstate = Info.WeaponInfos[(int)player.ReadyWeapon].AttackState;
            world.P_SetPsprite(player, PlayerSprite.Weapon, newstate);
            //P_NoiseAlert(player->mo, player->mo);
        }








        public static void Lower(Player player, PlayerSpriteDef psp)
        {
            var world = player.Mobj.World;

            psp.Sy += World.LOWERSPEED;

            // Is already down.
            if (psp.Sy < World.WEAPONBOTTOM)
            {
                return;
            }

            // Player is dead.
            if (player.PlayerState == PlayerState.Dead)
            {
                psp.Sy = World.WEAPONBOTTOM;

                // don't bring weapon back up
                return;
            }

            // The old weapon has been lowered off the screen,
            // so change the weapon and start raising it
            if (player.Health == 0)
            {
                // Player is dead, so keep the weapon off screen.
                world.P_SetPsprite(player, PlayerSprite.Weapon, State.Null);
                return;
            }

            player.ReadyWeapon = player.PendingWeapon;

            world.P_BringUpWeapon(player);
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
            var world = player.Mobj.World;

            var damage = (world.Random.Next() % 10 + 1) << 1;

            if (player.Powers[(int)PowerType.Strength] != 0)
            {
                damage *= 10;
            }

            var angle = player.Mobj.Angle;
            angle += new Angle((world.Random.Next() - world.Random.Next()) << 18);
            var slope = world.AimLineAttack(player.Mobj, angle, World.MELEERANGE);
            world.LineAttack(player.Mobj, angle, World.MELEERANGE, slope, damage);

            // turn to face target
            if (world.linetarget != null)
            {
                world.StartSound(player.Mobj, Sfx.PUNCH);
                player.Mobj.Angle = Geometry.PointToAngle(
                    player.Mobj.X,
                    player.Mobj.Y,
                    world.linetarget.X,
                    world.linetarget.Y);
            }
        }

        public static void ReFire(Player player, PlayerSpriteDef psp)
        {
            // check for fire
            //  (if a weaponchange is pending, let it go through instead)
            if ((player.Cmd.Buttons & Buttons.Attack) != 0
                && player.PendingWeapon == WeaponType.NoChange
                && player.Health != 0)
            {
                player.Refire++;
                FireWeapon(player);
            }
            else
            {
                player.Refire = 0;
                //P_CheckAmmo(player);
            }
        }

        private static void P_BulletSlope(Mobj mo)
        {
            var world = mo.World;

            // see which target is to be aimed at
            var an = mo.Angle;
            world.bulletslope = world.AimLineAttack(mo, an, new Fixed(16 * 64 * Fixed.FracUnit));

            if (world.linetarget == null)
            {
                an += new Angle(1 << 26);
                world.bulletslope = world.AimLineAttack(mo, an, new Fixed(16 * 64 * Fixed.FracUnit));
                if (world.linetarget == null)
                {
                    an -= new Angle(2 << 26);
                    world.bulletslope = world.AimLineAttack(mo, an, new Fixed(16 * 64 * Fixed.FracUnit));
                }
            }
        }

        //
        // P_GunShot
        //
        private static void P_GunShot(Mobj mo, bool accurate)
        {
            var world = mo.World;

            var damage = 5 * (world.Random.Next() % 3 + 1);
            var angle = mo.Angle;

            if (!accurate)
            {
                angle += new Angle((world.Random.Next() - world.Random.Next()) << 18);
            }

            world.LineAttack(mo, angle, World.MISSILERANGE, world.bulletslope, damage);
        }


        public static void FirePistol(Player player, PlayerSpriteDef psp)
        {
            var world = player.Mobj.World;

            world.StartSound(player.Mobj, Sfx.PISTOL);

            world.SetMobjState(player.Mobj, State.PlayAtk2);
            player.Ammo[(int)Info.WeaponInfos[(int)player.ReadyWeapon].Ammo]--;

            world.P_SetPsprite(player,
                PlayerSprite.Flash,
                Info.WeaponInfos[(int)player.ReadyWeapon].FlashState);

            P_BulletSlope(player.Mobj);
            P_GunShot(player.Mobj, player.Refire == 0);
        }

        public static void Light1(Player player, PlayerSpriteDef psp)
        {
            player.ExtraLight = 1;
        }

        public static void FireShotgun(Player player, PlayerSpriteDef psp)
        {
            player.ExtraLight = 2;
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
