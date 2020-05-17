using System;

namespace ManagedDoom
{
    public sealed class PlayerBehavior
    {
        private World world;

        public PlayerBehavior(World world)
        {
            this.world = world;
        }


        public void Thrust(Player player, Angle angle, Fixed move)
        {
            player.Mobj.MomX += move * Trig.Cos(angle);
            player.Mobj.MomY += move * Trig.Sin(angle);
        }



        private static readonly Fixed maxBob = new Fixed(0x100000);

        private bool onGround;


        public void CalcHeight(Player player)
        {
            // Regular movement bobbing.
            // It needs to be calculated for gun swing even if not on ground.
            //
            // OPTIMIZE:
            //     Tablify angle.
            //
            // Note:
            //     A LUT allows for effects like a ramp with low health.

            player.Bob = player.Mobj.MomX * player.Mobj.MomX + player.Mobj.MomY * player.Mobj.MomY;
            player.Bob >>= 2;
            if (player.Bob > maxBob)
            {
                player.Bob = maxBob;
            }

            if ((player.Cheats & CheatFlags.NoMomentum) != 0 || !onGround)
            {
                player.ViewZ = player.Mobj.Z + Player.VIEWHEIGHT;

                if (player.ViewZ > player.Mobj.CeilingZ - Fixed.FromInt(4))
                {
                    player.ViewZ = player.Mobj.CeilingZ - Fixed.FromInt(4);
                }

                player.ViewZ = player.Mobj.Z + player.ViewHeight;

                return;
            }

            var angle = (Trig.FineAngleCount / 20 * world.levelTime) & Trig.FineMask;

            var bob = (player.Bob / 2) * Trig.Sin(angle);

            // Move viewheight.
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


        public void MovePlayer(Player player)
        {
            var cmd = player.Cmd;

            player.Mobj.Angle += new Angle(cmd.AngleTurn << 16);

            // Do not let the player control movement if not onground.
            onGround = (player.Mobj.Z <= player.Mobj.FloorZ);

            if (cmd.ForwardMove != 0 && onGround)
            {
                Thrust(player, player.Mobj.Angle, new Fixed(cmd.ForwardMove * 2048));
            }

            if (cmd.SideMove != 0 && onGround)
            {
                Thrust(player, player.Mobj.Angle - Angle.Ang90, new Fixed(cmd.SideMove * 2048));
            }

            if ((cmd.ForwardMove != 0 || cmd.SideMove != 0) &&
                player.Mobj.State == DoomInfo.States[(int)MobjState.Play])
            {
                player.Mobj.SetState(MobjState.PlayRun1);
            }
        }


        public void PlayerThink(Player player)
        {
            // FIXME:
            //     Do this in the cheat code.
            if ((player.Cheats & CheatFlags.NoClip) != 0)
            {
                player.Mobj.Flags |= MobjFlags.NoClip;
            }
            else
            {
                player.Mobj.Flags &= ~MobjFlags.NoClip;
            }

            // Chain saw run forward.
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
                DeathThink(player);
                return;
            }

            // Move around.
            // Reactiontime is used to prevent movement for a bit after a teleport.
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
                PlayerInSpecialSector(player);
            }

            // Check for weapon change.

            // A special event has no other buttons.
            if ((cmd.Buttons & TicCmdButtons.Special) != 0)
            {
                cmd.Buttons = 0;
            }

            if ((cmd.Buttons & TicCmdButtons.Change) != 0)
            {
                // The actual changing of the weapon is done when the weapon psprite can do it.
                // Not in the middle of an attack.
                var newWeapon = (cmd.Buttons & TicCmdButtons.WeaponMask) >> TicCmdButtons.WeaponShift;

                if (newWeapon == (int)WeaponType.Fist &&
                    player.WeaponOwned[(int)WeaponType.Chainsaw] &&
                    !(player.ReadyWeapon == WeaponType.Chainsaw && player.Powers[(int)PowerType.Strength] != 0))
                {
                    newWeapon = (int)WeaponType.Chainsaw;
                }

                if ((world.Options.GameMode == GameMode.Commercial) &&
                    newWeapon == (int)WeaponType.Shotgun &&
                    player.WeaponOwned[(int)WeaponType.SuperShotgun] &&
                    player.ReadyWeapon != WeaponType.SuperShotgun)
                {
                    newWeapon = (int)WeaponType.SuperShotgun;
                }

                if (player.WeaponOwned[newWeapon] &&
                    newWeapon != (int)player.ReadyWeapon)
                {
                    // Do not go to plasma or BFG in shareware, even if cheated.
                    if ((newWeapon != (int)WeaponType.Plasma && newWeapon != (int)WeaponType.Bfg) ||
                        (world.Options.GameMode != GameMode.Shareware))
                    {
                        player.PendingWeapon = (WeaponType)newWeapon;
                    }
                }
            }

            // Check for use.
            if ((cmd.Buttons & TicCmdButtons.Use) != 0)
            {
                if (!player.UseDown)
                {
                    world.MapInteraction.UseLines(player);
                    player.UseDown = true;
                }
            }
            else
            {
                player.UseDown = false;
            }

            // cycle psprites
            P_MovePsprites(player);

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

            if (player.BonusCount > 0)
            {
                player.BonusCount--;
            }

            // Handling colormaps.
            if (player.Powers[(int)PowerType.Invulnerability] > 0)
            {
                if (player.Powers[(int)PowerType.Invulnerability] > 4 * 32 ||
                    (player.Powers[(int)PowerType.Invulnerability] & 8) != 0)
                {
                    player.FixedColorMap = 0; // INVERSECOLORMAP;
                }
                else
                {
                    player.FixedColorMap = 0;
                }
            }
            else if (player.Powers[(int)PowerType.Infrared] > 0)
            {
                if (player.Powers[(int)PowerType.Infrared] > 4 * 32 ||
                    (player.Powers[(int)PowerType.Infrared] & 8) != 0)
                {
                    // Almost full bright.
                    player.FixedColorMap = 1;
                }
                else
                {
                    player.FixedColorMap = 0;
                }
            }
            else
            {
                player.FixedColorMap = 0;
            }
        }


        private void PlayerInSpecialSector(Player player)
        {
            var sector = player.Mobj.Subsector.Sector;

            // Falling, not all the way down yet?
            if (player.Mobj.Z != sector.FloorHeight)
            {
                return;
            }

            var ti = world.ThingInteraction;

            // Has hitten ground.
            switch ((int)sector.Special)
            {
                case 5:
                    // HELLSLIME DAMAGE
                    if (player.Powers[(int)PowerType.IronFeet] == 0)
                    {
                        if ((world.levelTime & 0x1f) == 0)
                        {
                            ti.DamageMobj(player.Mobj, null, null, 10);
                        }
                    }
                    break;

                case 7:
                    // NUKAGE DAMAGE
                    if (player.Powers[(int)PowerType.IronFeet] == 0)
                    {
                        if ((world.levelTime & 0x1f) == 0)
                        {
                            ti.DamageMobj(player.Mobj, null, null, 5);
                        }
                    }
                    break;

                case 16:
                // SUPER HELLSLIME DAMAGE
                case 4:
                    // STROBE HURT
                    if (player.Powers[(int)PowerType.IronFeet] == 0
                        || (world.Random.Next() < 5))
                    {
                        if ((world.levelTime & 0x1f) == 0)
                        {
                            ti.DamageMobj(player.Mobj, null, null, 20);
                        }
                    }
                    break;

                case 9:
                    // SECRET SECTOR
                    player.SecretCount++;
                    sector.Special = 0;
                    break;

                case 11:
                    // EXIT SUPER DAMAGE! (for E1M8 finale)
                    player.Cheats &= ~CheatFlags.GodMode;

                    if ((world.levelTime & 0x1f) == 0)
                    {
                        ti.DamageMobj(player.Mobj, null, null, 20);
                    }

                    if (player.Health <= 10)
                    {
                        world.G_ExitLevel();
                    }

                    break;

                default:
                    throw new Exception("Unknown sector special: " + (int)sector.Special);
            }
        }


        private static Angle ang5 = new Angle(Angle.Ang90.Data / 18);

        private void DeathThink(Player player)
        {
            P_MovePsprites(player);

            // Fall to the ground.
            if (player.ViewHeight > Fixed.FromInt(6))
            {
                player.ViewHeight -= Fixed.One;
            }

            if (player.ViewHeight < Fixed.FromInt(6))
            {
                player.ViewHeight = Fixed.FromInt(6);
            }

            player.DeltaViewHeight = Fixed.Zero;
            onGround = (player.Mobj.Z <= player.Mobj.FloorZ);
            CalcHeight(player);

            if (player.Attacker != null && player.Attacker != player.Mobj)
            {
                var angle = Geometry.PointToAngle(
                    player.Mobj.X, player.Mobj.Y,
                    player.Attacker.X, player.Attacker.Y);

                var delta = angle - player.Mobj.Angle;

                if (delta < ang5 || delta.Data > (-ang5).Data)
                {
                    // Looking at killer, so fade damage flash down.
                    player.Mobj.Angle = angle;

                    if (player.DamageCount > 0)
                    {
                        player.DamageCount--;
                    }
                }
                else if (delta < Angle.Ang180)
                {
                    player.Mobj.Angle += ang5;
                }
                else
                {
                    player.Mobj.Angle -= ang5;
                }
            }
            else if (player.DamageCount > 0)
            {
                player.DamageCount--;
            }

            if ((player.Cmd.Buttons & TicCmdButtons.Use) != 0)
            {
                player.PlayerState = PlayerState.Reborn;
            }
        }


        public void SetupPsprites(Player player)
        {
            // Remove all psprites.
            for (var i = 0; i < (int)PlayerSprite.Count; i++)
            {
                player.PSprites[i].State = null;
            }

            // Spawn the gun.
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
        public void P_BringUpWeapon(Player player)
        {
            if (player.PendingWeapon == WeaponType.NoChange)
            {
                player.PendingWeapon = player.ReadyWeapon;
            }

            if (player.PendingWeapon == WeaponType.Chainsaw)
            {
                world.StartSound(player.Mobj, Sfx.SAWUP);
            }

            var newstate = DoomInfo.WeaponInfos[(int)player.PendingWeapon].UpState;

            player.PendingWeapon = WeaponType.NoChange;
            player.PSprites[(int)PlayerSprite.Weapon].Sy = WEAPONBOTTOM;

            P_SetPsprite(player, PlayerSprite.Weapon, newstate);
        }

        //
        // P_SetPsprite
        //
        public void P_SetPsprite(Player player, PlayerSprite position, MobjState stnum)
        {
            var psp = player.PSprites[(int)position];

            do
            {
                if (stnum == MobjState.Null)
                {
                    // object removed itself
                    psp.State = null;
                    break;
                }

                var state = DoomInfo.States[(int)stnum];
                psp.State = state;
                psp.Tics = state.Tics; // could be 0

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
                    state.PlayerAction(world, player, psp);
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

                MobjStateDef state;
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


        //
        // P_DropWeapon
        // Player died, so put the weapon away.
        //
        public void DropWeapon(Player player)
        {
            P_SetPsprite(
                player,
                PlayerSprite.Weapon,
                DoomInfo.WeaponInfos[(int)player.ReadyWeapon].DownState);
        }


        public void PlayerScream(Mobj mo)
        {
            // Default death sound.
            var sound = Sfx.PLDETH;

            if ((world.Options.GameMode == GameMode.Commercial)
                && (mo.Health < -50))
            {
                // IF THE PLAYER DIES
                // LESS THAN -50% WITHOUT GIBBING
                sound = Sfx.PDIEHI;
            }

            world.StartSound(mo, sound);
        }
    }
}
