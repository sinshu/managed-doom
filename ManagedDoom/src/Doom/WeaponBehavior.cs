using System;

namespace ManagedDoom
{
    public sealed class WeaponBehavior
    {
        private World world;

        public WeaponBehavior(World world)
        {
            this.world = world;
        }

        public void Light0(Player player)
        {
            player.ExtraLight = 0;
        }

        public void WeaponReady(Player player, PlayerSpriteDef psp)
        {
            var pb = world.PlayerBehavior;

            // get out of attack state
            if (player.Mobj.State == DoomInfo.States[(int)State.PlayAtk1]
                || player.Mobj.State == DoomInfo.States[(int)State.PlayAtk2])
            {
                player.Mobj.SetState(State.Play);
            }

            if (player.ReadyWeapon == WeaponType.Chainsaw
                && psp.State == DoomInfo.States[(int)State.Saw])
            {
                world.StartSound(player.Mobj, Sfx.SAWIDL);
            }

            // check for change
            //  if player is dead, put the weapon away
            if (player.PendingWeapon != WeaponType.NoChange || player.Health == 0)
            {
                // change weapon
                //  (pending weapon should allready be validated)
                var newstate = DoomInfo.WeaponInfos[(int)player.ReadyWeapon].DownState;
                pb.P_SetPsprite(player, PlayerSprite.Weapon, newstate);
                return;
            }

            // check for fire
            //  the missile launcher and bfg do not auto fire
            if ((player.Cmd.Buttons & TicCmdButtons.Attack) != 0)
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
            psp.Sy = PlayerBehavior.WEAPONTOP + player.Bob * Trig.Sin(angle);
        }

        private static readonly int BFGCELLS = 40;

        //
        // P_CheckAmmo
        // Returns true if there is enough ammo to shoot.
        // If not, selects the next weapon to use.
        //
        private bool CheckAmmo(Player player)
        {
            var pb = world.PlayerBehavior;

            var ammo = DoomInfo.WeaponInfos[(int)player.ReadyWeapon].Ammo;

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
            pb.P_SetPsprite(
                player,
                PlayerSprite.Weapon,
                DoomInfo.WeaponInfos[(int)player.ReadyWeapon].DownState);

            return false;
        }



        private void P_RecursiveSound(Sector sec, int soundblocks, Mobj soundtarget, int validcount)
        {
            var mc = world.MapCollision;

            // wake up all monsters in this sector
            if (sec.ValidCount == validcount
                && sec.SoundTraversed <= soundblocks + 1)
            {
                // already flooded
                return;
            }

            sec.ValidCount = validcount;
            sec.SoundTraversed = soundblocks + 1;
            sec.SoundTarget = soundtarget;

            for (var i = 0; i < sec.Lines.Length; i++)
            {
                var check = sec.Lines[i];
                if ((check.Flags & LineFlags.TwoSided) == 0)
                {
                    continue;
                }

                mc.LineOpening(check);

                if (mc.OpenRange <= Fixed.Zero)
                {
                    // closed door
                    continue;
                }

                Sector other;
                if (check.Side0.Sector == sec)
                {
                    other = check.Side1.Sector;
                }
                else
                {
                    other = check.Side0.Sector;
                }

                if ((check.Flags & LineFlags.SoundBlock) != 0)
                {
                    if (soundblocks == 0)
                    {
                        P_RecursiveSound(other, 1, soundtarget, validcount);
                    }
                }
                else
                {
                    P_RecursiveSound(other, soundblocks, soundtarget, validcount);
                }
            }
        }


        //
        // P_NoiseAlert
        // If a monster yells at a player,
        // it will alert other monsters to the player.
        //
        private void P_NoiseAlert(Mobj target, Mobj emmiter)
        {
            P_RecursiveSound(
                emmiter.Subsector.Sector,
                0,
                target,
                world.GetNewValidCount());
        }



        //
        // P_FireWeapon.
        //
        private void FireWeapon(Player player)
        {
            if (!CheckAmmo(player))
            {
                return;
            }

            player.Mobj.SetState(State.PlayAtk1);
            var newstate = DoomInfo.WeaponInfos[(int)player.ReadyWeapon].AttackState;
            world.PlayerBehavior.P_SetPsprite(player, PlayerSprite.Weapon, newstate);
            P_NoiseAlert(player.Mobj, player.Mobj);
        }


        public void Lower(Player player, PlayerSpriteDef psp)
        {
            var pb = world.PlayerBehavior;

            psp.Sy += PlayerBehavior.LOWERSPEED;

            // Is already down.
            if (psp.Sy < PlayerBehavior.WEAPONBOTTOM)
            {
                return;
            }

            // Player is dead.
            if (player.PlayerState == PlayerState.Dead)
            {
                psp.Sy = PlayerBehavior.WEAPONBOTTOM;

                // don't bring weapon back up
                return;
            }

            // The old weapon has been lowered off the screen,
            // so change the weapon and start raising it
            if (player.Health == 0)
            {
                // Player is dead, so keep the weapon off screen.
                pb.P_SetPsprite(player, PlayerSprite.Weapon, State.Null);
                return;
            }

            player.ReadyWeapon = player.PendingWeapon;

            pb.P_BringUpWeapon(player);
        }

        public void Raise(Player player, PlayerSpriteDef psp)
        {
            var pb = world.PlayerBehavior;

            psp.Sy -= PlayerBehavior.RAISESPEED;

            if (psp.Sy > PlayerBehavior.WEAPONTOP)
            {
                return;
            }

            psp.Sy = PlayerBehavior.WEAPONTOP;

            // The weapon has been raised all the way,
            //  so change to the ready state.
            var newstate = DoomInfo.WeaponInfos[(int)player.ReadyWeapon].ReadyState;

            pb.P_SetPsprite(player, PlayerSprite.Weapon, newstate);
        }

        public void Punch(Player player)
        {
            var hs = world.Hitscan;

            var damage = (world.Random.Next() % 10 + 1) << 1;

            if (player.Powers[(int)PowerType.Strength] != 0)
            {
                damage *= 10;
            }

            var angle = player.Mobj.Angle;
            angle += new Angle((world.Random.Next() - world.Random.Next()) << 18);
            var slope = hs.AimLineAttack(player.Mobj, angle, World.MELEERANGE);
            hs.LineAttack(player.Mobj, angle, World.MELEERANGE, slope, damage);

            // turn to face target
            if (hs.linetarget != null)
            {
                world.StartSound(player.Mobj, Sfx.PUNCH);
                player.Mobj.Angle = Geometry.PointToAngle(
                    player.Mobj.X,
                    player.Mobj.Y,
                    hs.linetarget.X,
                    hs.linetarget.Y);
            }
        }

        public void Saw(Player player)
        {
            var damage = 2 * (world.Random.Next() % 10 + 1);
            var angle = player.Mobj.Angle;
            angle += new Angle((world.Random.Next() - world.Random.Next()) << 18);

            var hs = world.Hitscan;

            // use meleerange + 1 se the puff doesn't skip the flash
            var slope = hs.AimLineAttack(player.Mobj, angle, World.MELEERANGE + new Fixed(1));
            hs.LineAttack(player.Mobj, angle, World.MELEERANGE + new Fixed(1), slope, damage);

            if (hs.linetarget == null)
            {
                world.StartSound(player.Mobj, Sfx.SAWFUL);
                return;
            }
            world.StartSound(player.Mobj, Sfx.SAWHIT);

            // turn to face target
            angle = Geometry.PointToAngle(
                player.Mobj.X, player.Mobj.Y,
                hs.linetarget.X, hs.linetarget.Y);
            if (angle - player.Mobj.Angle > Angle.Ang180)
            {
                // The cast to int is necessary to prevent demo desync. Why?
                if ((int)(angle - player.Mobj.Angle).Data < -Angle.Ang90.Data / 20)
                {
                    player.Mobj.Angle = angle + Angle.Ang90 / 21;
                }
                else
                {
                    player.Mobj.Angle -= Angle.Ang90 / 20;
                }
            }
            else
            {
                if (angle - player.Mobj.Angle > Angle.Ang90 / 20)
                {
                    player.Mobj.Angle = angle - Angle.Ang90 / 21;
                }
                else
                {
                    player.Mobj.Angle += Angle.Ang90 / 20;
                }
            }
            player.Mobj.Flags |= MobjFlags.JustAttacked;
        }

        public void ReFire(Player player)
        {
            // check for fire
            //  (if a weaponchange is pending, let it go through instead)
            if ((player.Cmd.Buttons & TicCmdButtons.Attack) != 0
                && player.PendingWeapon == WeaponType.NoChange
                && player.Health != 0)
            {
                player.Refire++;
                FireWeapon(player);
            }
            else
            {
                player.Refire = 0;
                CheckAmmo(player);
            }
        }

        private void P_BulletSlope(Mobj mo)
        {
            var hs = world.Hitscan;

            // see which target is to be aimed at
            var an = mo.Angle;
            hs.bulletslope = hs.AimLineAttack(mo, an, new Fixed(16 * 64 * Fixed.FracUnit));

            if (hs.linetarget == null)
            {
                an += new Angle(1 << 26);
                hs.bulletslope = hs.AimLineAttack(mo, an, new Fixed(16 * 64 * Fixed.FracUnit));
                if (hs.linetarget == null)
                {
                    an -= new Angle(2 << 26);
                    hs.bulletslope = hs.AimLineAttack(mo, an, new Fixed(16 * 64 * Fixed.FracUnit));
                }
            }
        }

        //
        // P_GunShot
        //
        private void P_GunShot(Mobj mo, bool accurate)
        {
            var hs = world.Hitscan;

            var damage = 5 * (world.Random.Next() % 3 + 1);
            var angle = mo.Angle;

            if (!accurate)
            {
                angle += new Angle((world.Random.Next() - world.Random.Next()) << 18);
            }

            hs.LineAttack(mo, angle, World.MISSILERANGE, hs.bulletslope, damage);
        }


        public void FirePistol(Player player)
        {
            world.StartSound(player.Mobj, Sfx.PISTOL);

            player.Mobj.SetState(State.PlayAtk2);
            player.Ammo[(int)DoomInfo.WeaponInfos[(int)player.ReadyWeapon].Ammo]--;

            world.PlayerBehavior.P_SetPsprite(player,
                PlayerSprite.Flash,
                DoomInfo.WeaponInfos[(int)player.ReadyWeapon].FlashState);

            P_BulletSlope(player.Mobj);
            P_GunShot(player.Mobj, player.Refire == 0);
        }

        public void Light1(Player player)
        {
            player.ExtraLight = 1;
        }

        public void FireShotgun(Player player)
        {
            world.StartSound(player.Mobj, Sfx.SHOTGN);
            player.Mobj.SetState(State.PlayAtk2);

            player.Ammo[(int)DoomInfo.WeaponInfos[(int)player.ReadyWeapon].Ammo]--;

            world.PlayerBehavior.P_SetPsprite(
                player,
                PlayerSprite.Flash,
                DoomInfo.WeaponInfos[(int)player.ReadyWeapon].FlashState);

            P_BulletSlope(player.Mobj);

            for (var i = 0; i < 7; i++)
            {
                P_GunShot(player.Mobj, false);
            }
        }

        public void Light2(Player player)
        {
            player.ExtraLight = 2;
        }

        //
        // A_FireCGun
        //
        public void FireCGun(Player player, PlayerSpriteDef psp)
        {
            world.StartSound(player.Mobj, Sfx.PISTOL);

            if (player.Ammo[(int)DoomInfo.WeaponInfos[(int)player.ReadyWeapon].Ammo] == 0)
            {
                return;
            }

            player.Mobj.SetState(State.PlayAtk2);
            player.Ammo[(int)DoomInfo.WeaponInfos[(int)player.ReadyWeapon].Ammo]--;

            world.PlayerBehavior.P_SetPsprite(
                player,
                PlayerSprite.Flash,
                DoomInfo.WeaponInfos[(int)player.ReadyWeapon].FlashState
                + psp.State.Number - DoomInfo.States[(int)State.Chain1].Number);

            P_BulletSlope(player.Mobj);

            P_GunShot(player.Mobj, player.Refire == 0);
        }

        //
        // A_FireShotgun2
        //
        public void FireShotgun2(Player player)
        {
            world.StartSound(player.Mobj, Sfx.DSHTGN);
            player.Mobj.SetState(State.PlayAtk2);

            player.Ammo[(int)DoomInfo.WeaponInfos[(int)player.ReadyWeapon].Ammo] -= 2;

            world.PlayerBehavior.P_SetPsprite(
                player,
                PlayerSprite.Flash,
                DoomInfo.WeaponInfos[(int)player.ReadyWeapon].FlashState);

            P_BulletSlope(player.Mobj);

            var hs = world.Hitscan;
            for (var i = 0; i < 20; i++)
            {
                var damage = 5 * (world.Random.Next() % 3 + 1);
                var angle = player.Mobj.Angle;
                angle += new Angle((world.Random.Next() - world.Random.Next()) << 19);
                hs.LineAttack(
                    player.Mobj,
                    angle,
                    World.MISSILERANGE,
                    hs.bulletslope + new Fixed((world.Random.Next() - world.Random.Next()) << 5),
                    damage);
            }
        }

        public void CheckReload(Player player)
        {
            CheckAmmo(player);
        }

        public void OpenShotgun2(Player player)
        {
            world.StartSound(player.Mobj, Sfx.DBOPN);
        }

        public void LoadShotgun2(Player player)
        {
            world.StartSound(player.Mobj, Sfx.DBLOAD);
        }

        public void CloseShotgun2(Player player)
        {
            world.StartSound(player.Mobj, Sfx.DBCLS);
            ReFire(player);
        }




        //
        // A_GunFlash
        //
        public void GunFlash(Player player)
        {
            player.Mobj.SetState(State.PlayAtk2);
            world.PlayerBehavior.P_SetPsprite(
                player,
                PlayerSprite.Flash,
                DoomInfo.WeaponInfos[(int)player.ReadyWeapon].FlashState);
        }

        //
        // A_FireMissile
        //
        public void FireMissile(Player player)
        {
            player.Ammo[(int)DoomInfo.WeaponInfos[(int)player.ReadyWeapon].Ammo]--;
            world.ThingAllocation.SpawnPlayerMissile(player.Mobj, MobjType.Rocket);
        }








        //
        // A_FirePlasma
        //
        public void FirePlasma(Player player)
        {
            player.Ammo[(int)DoomInfo.WeaponInfos[(int)player.ReadyWeapon].Ammo]--;

            world.PlayerBehavior.P_SetPsprite(
                player,
                PlayerSprite.Flash,
                DoomInfo.WeaponInfos[(int)player.ReadyWeapon].FlashState + (world.Random.Next() & 1));

            world.ThingAllocation.SpawnPlayerMissile(player.Mobj, MobjType.Plasma);
        }

        //
        // A_BFGsound
        //
        public void A_BFGsound(Player player)
        {
            world.StartSound(player.Mobj, Sfx.BFG);
        }


        //
        // A_FireBFG
        //
        public void FireBFG(Player player)
        {
            player.Ammo[(int)DoomInfo.WeaponInfos[(int)player.ReadyWeapon].Ammo] -= BFGCELLS;
            world.ThingAllocation.SpawnPlayerMissile(player.Mobj, MobjType.Bfg);
        }



        //
        // A_BFGSpray
        // Spawn a BFG explosion on every monster in view
        //
        public void BFGSpray(Mobj mo)
        {
            // offset angles from its attack angle
            for (var i = 0; i < 40; i++)
            {
                var an = mo.Angle - Angle.Ang90 / 2 + Angle.Ang90 / 40 * (uint)i;

                var hs = world.Hitscan;

                // mo->target is the originator (player)
                //  of the missile
                hs.AimLineAttack(mo.Target, an, Fixed.FromInt(16 * 64));

                if (hs.linetarget == null)
                {
                    continue;
                }

                world.ThingAllocation.SpawnMobj(
                    hs.linetarget.X,
                    hs.linetarget.Y,
                    hs.linetarget.Z + new Fixed(hs.linetarget.Height.Data >> 2),
                    MobjType.Extrabfg);

                var damage = 0;
                for (var j = 0; j < 15; j++)
                {
                    damage += (world.Random.Next() & 7) + 1;
                }

                world.ThingInteraction.DamageMobj(
                    hs.linetarget, mo.Target, mo.Target, damage);
            }
        }
    }
}
