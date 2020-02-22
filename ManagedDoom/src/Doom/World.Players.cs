using System;

namespace ManagedDoom
{
    public sealed partial class World
    {
        //
        // P_Thrust
        // Moves the given origin along a given angle.
        //
        public void Thrust(Player player, Angle angle, Fixed move)
        {
            player.Mobj.MomX += move * Trig.Cos(angle);
            player.Mobj.MomY += move * Trig.Sin(angle);
        }



        private static readonly Fixed MAXBOB = new Fixed(0x100000);

        private bool onground;

        //
        // P_CalcHeight
        // Calculate the walking / running height adjustment
        //
        public void CalcHeight(Player player)
        {
            // Regular movement bobbing
            // (needs to be calculated for gun swing
            // even if not on ground)
            // OPTIMIZE: tablify angle
            // Note: a LUT allows for effects
            //  like a ramp with low health.
            player.Bob =
                player.Mobj.MomX * player.Mobj.MomX
                + player.Mobj.MomY * player.Mobj.MomY;

            player.Bob = new Fixed(player.Bob.Data >> 2);

            if (player.Bob > MAXBOB)
            {
                player.Bob = MAXBOB;
            }

            if ((player.Cheats & CheatFlags.NoMomentum) != 0 || !onground)
            {
                player.ViewZ = player.Mobj.Z + Player.VIEWHEIGHT;

                if (player.ViewZ > player.Mobj.CeilingZ - Fixed.FromInt(4))
                {
                    player.ViewZ = player.Mobj.CeilingZ - Fixed.FromInt(4);
                }

                player.ViewZ = player.Mobj.Z + player.ViewHeight;

                return;
            }

            var angle = (Trig.FineAngleCount / 20 * levelTime) & Trig.FineMask;
            var bob = (player.Bob / 2) * Trig.Sin(angle);

            // move viewheight
            if (player.PlayerState == PlayerState.Live)
            {
                player.ViewHeight += player.DeltaViewHeight;

                if (player.ViewHeight > Player.VIEWHEIGHT)
                {
                    player.ViewHeight = Player.VIEWHEIGHT;
                    player.DeltaViewHeight = Fixed.Zero;
                }

                if (player.ViewHeight < Player.VIEWHEIGHT / 2)
                {
                    player.ViewHeight = Player.VIEWHEIGHT / 2;

                    if (player.DeltaViewHeight <= Fixed.Zero)
                    {
                        player.DeltaViewHeight = new Fixed(1);
                    }
                }

                if (player.DeltaViewHeight != Fixed.Zero)
                {
                    player.DeltaViewHeight += Fixed.One / 4;

                    if (player.DeltaViewHeight == Fixed.Zero)
                    {
                        player.DeltaViewHeight = new Fixed(1);
                    }
                }
            }

            player.ViewZ = player.Mobj.Z + player.ViewHeight + bob;

            if (player.ViewZ > player.Mobj.CeilingZ - Fixed.FromInt(4))
            {
                player.ViewZ = player.Mobj.CeilingZ - Fixed.FromInt(4);
            }
        }


        //
        // P_MovePlayer
        //
        public void MovePlayer(Player player)
        {
            var cmd = player.Cmd;

            player.Mobj.Angle += new Angle(cmd.AngleTurn << 16);

            // Do not let the player control movement
            //  if not onground.
            onground = (player.Mobj.Z <= player.Mobj.FloorZ);

            if (cmd.ForwardMove != 0 && onground)
            {
                Thrust(player, player.Mobj.Angle, new Fixed(cmd.ForwardMove * 2048));
            }

            if (cmd.SideMove != 0 && onground)
            {
                Thrust(player, player.Mobj.Angle - Angle.Ang90, new Fixed(cmd.SideMove * 2048));
            }

            if ((cmd.ForwardMove != 0 || cmd.SideMove != 0)
                && player.Mobj.State == Info.States[(int)State.Play])
            {
                SetMobjState(player.Mobj, State.PlayRun1);
            }
        }






        //
        // P_PlayerThink
        //
        public void PlayerThink(Player player)
        {
            // fixme: do this in the cheat code
            if ((player.Cheats & CheatFlags.NoClip) != 0)
            {
                player.Mobj.Flags |= MobjFlags.NoClip;
            }
            else
            {
                player.Mobj.Flags &= ~MobjFlags.NoClip;
            }

            // chain saw run forward
            var cmd = player.Cmd;
            if ((player.Mobj.Flags & MobjFlags.JustAttacked) != 0)
            {
                cmd.AngleTurn = 0;
                cmd.ForwardMove = 0xc800 / 512;
                cmd.SideMove = 0;
                player.Mobj.Flags &= ~MobjFlags.JustAttacked;
            }


            if (player.PlayerState == PlayerState.Dead)
            {
                //P_DeathThink(player);
                return;
            }

            // Move around.
            // Reactiontime is used to prevent movement
            //  for a bit after a teleport.
            if (player.Mobj.ReactionTime > 0)
            {
                player.Mobj.ReactionTime--;
            }
            else
            {
                MovePlayer(player);
            }

            CalcHeight(player);

            if (player.Mobj.Subsector.Sector.Special != 0)
            {
                //P_PlayerInSpecialSector(player);
            }

            // Check for weapon change.

            // A special event has no other buttons.
            if ((cmd.Buttons & Buttons.Special) != 0)
            {
                cmd.Buttons = 0;
            }

            if ((cmd.Buttons & Buttons.Change) != 0)
            {
                // The actual changing of the weapon is done
                //  when the weapon psprite can do it
                //  (read: not in the middle of an attack).
                var newweapon = (cmd.Buttons & Buttons.WeaponMask) >> Buttons.WeaponShift;

                if (newweapon == (int)WeaponType.Fist
                    && player.WeaponOwned[(int)WeaponType.Chainsaw]
                    && !(player.ReadyWeapon == WeaponType.Chainsaw
                    && player.Powers[(int)PowerType.Strength] != 0))
                {
                    newweapon = (int)WeaponType.Chainsaw;
                }

                //if ((gamemode == commercial)
                if (true
                    && newweapon == (int)WeaponType.Shotgun
                    && player.WeaponOwned[(int)WeaponType.SuperShotgun]
                    && player.ReadyWeapon != WeaponType.SuperShotgun)
                {
                    newweapon = (int)WeaponType.SuperShotgun;
                }

                if (player.WeaponOwned[newweapon]
                    && newweapon != (int)player.ReadyWeapon)
                {
                    // Do not go to plasma or BFG in shareware,
                    //  even if cheated.
                    if ((newweapon != (int)WeaponType.Plasma
                        && newweapon != (int)WeaponType.Bfg)
                            //|| (gamemode != shareware))
                            || false)
                    {
                        player.PendingWeapon = (WeaponType)newweapon;
                    }
                }
            }

            // check for use
            if ((cmd.Buttons & Buttons.Use) != 0)
            {
                if (!player.UseDown)
                {
                    //P_UseLines(player);
                    player.UseDown = true;
                }
            }
            else
            {
                player.UseDown = false;
            }

            // cycle psprites
            P_MovePsprites(player);

            /*

            // Counters, time dependend power ups.

            // Strength counts up to diminish fade.
            if (player.Powers[(int)PowerType.Strength] != 0)
            {
                player.Powers[(int)PowerType.Strength]++;
            }

            if (player.Powers[(int)PowerType.Invulnerability] > 0)
            {
                player.Powers[(int)PowerType.Invulnerability]--;
            }

            if (player.Powers[(int)PowerType.Invisibility] > 0)
            {
                if (--player.Powers[(int)PowerType.Invisibility] == 0)
                {
                    player.Mobj.Flags &= ~MobjFlags.Shadow;
                }
            }

            if (player.Powers[(int)PowerType.Infrared] > 0)
            {
                player.Powers[(int)PowerType.Infrared]--;
            }

            if (player.Powers[(int)PowerType.IronFeet] > 0)
            {
                player.Powers[(int)PowerType.IronFeet]--;
            }

            if (player.DamageCount > 0)
            {
                player.DamageCount--;
            }

            if (player.BonusCount)
            {
                player.BonusCount--;
            }

            // Handling colormaps.
            if (player->powers[pw_invulnerability])
            {
                if (player->powers[pw_invulnerability] > 4 * 32
                    || (player->powers[pw_invulnerability] & 8))
                    player->fixedcolormap = INVERSECOLORMAP;
                else
                    player->fixedcolormap = 0;
            }
            else if (player->powers[pw_infrared])
            {
                if (player->powers[pw_infrared] > 4 * 32
                    || (player->powers[pw_infrared] & 8))
                {
                    // almost full bright
                    player->fixedcolormap = 1;
                }
                else
                    player->fixedcolormap = 0;
            }
            else
                player->fixedcolormap = 0;
            */
        }


        //
        // P_SetupPsprites
        // Called at start of level for each player.
        //
        public void SetupPsprites(Player player)
        {
            // remove all psprites
            for (var i = 0; i < (int)PlayerSprite.Count; i++)
            {
                player.PSprites[i].State = null;
            }

            // spawn the gun
            player.PendingWeapon = player.ReadyWeapon;
            P_BringUpWeapon(player);
        }


        public static readonly Fixed LOWERSPEED = Fixed.FromInt(6);
        public static readonly Fixed RAISESPEED = Fixed.FromInt(6);

        public static readonly Fixed WEAPONBOTTOM = Fixed.FromInt(128);
        public static readonly Fixed WEAPONTOP = Fixed.FromInt(32);


        //
        // P_BringUpWeapon
        // Starts bringing the pending weapon up
        // from the bottom of the screen.
        // Uses player
        //
        private void P_BringUpWeapon(Player player)
        {
            if (player.PendingWeapon == WeaponType.NoChange)
            {
                player.PendingWeapon = player.ReadyWeapon;
            }

            if (player.PendingWeapon == WeaponType.Chainsaw)
            {
                StartSound(player.Mobj, Sfx.SAWUP);
            }

            var newstate = Info.WeaponInfos[(int)player.PendingWeapon].UpState;

            player.PendingWeapon = WeaponType.NoChange;
            player.PSprites[(int)PlayerSprite.Weapon].Sy = WEAPONBOTTOM;

            P_SetPsprite(player, PlayerSprite.Weapon, newstate);
        }

        //
        // P_SetPsprite
        //
        public void P_SetPsprite(Player player, PlayerSprite position, State stnum)
        {
            var psp = player.PSprites[(int)position];

            do
            {
                if (stnum == State.Null)
                {
                    // object removed itself
                    psp.State = null;
                    break;
                }

                var state = Info.States[(int)stnum];
                psp.State = state;
                psp.Tics = state.Tics;    // could be 0

                if (state.Misc1 != 0)
                {
                    // coordinate set
                    psp.Sx = Fixed.FromInt(state.Misc1);
                    psp.Sy = Fixed.FromInt(state.Misc2);
                }

                // Call action routine.
                // Modified handling.
                if (state.PlayerAction != null)
                {
                    state.PlayerAction(player, psp);
                    if (psp.State == null)
                    {
                        break;
                    }
                }

                stnum = psp.State.Next;

            } while (psp.Tics == 0);
            // an initial state of 0 could cycle through
        }


        //
        // P_MovePsprites
        // Called every tic by player thinking routine.
        //
        private void P_MovePsprites(Player player)
        {
            for (var i = 0; i < (int)PlayerSprite.Count; i++)
            {
                var psp = player.PSprites[i];

                StateDef state;
                // a null state means not active
                if ((state = psp.State) != null)
                {
                    // drop tic count and possibly change state

                    // a -1 tic count never changes
                    if (psp.Tics != -1)
                    {
                        psp.Tics--;
                        if (psp.Tics == 0)
                        {
                            P_SetPsprite(player, (PlayerSprite)i, psp.State.Next);
                        }
                    }
                }
            }

            player.PSprites[(int)PlayerSprite.Flash].Sx = player.PSprites[(int)PlayerSprite.Weapon].Sx;
            player.PSprites[(int)PlayerSprite.Flash].Sy = player.PSprites[(int)PlayerSprite.Weapon].Sy;
        }
    }
}
