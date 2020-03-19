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

                if ((Options.GameMode == GameMode.Commercial)
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
                    // even if cheated.
                    if ((newweapon != (int)WeaponType.Plasma
                        && newweapon != (int)WeaponType.Bfg)
                        || (Options.GameMode != GameMode.Shareware))
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
                    UseLines(player);
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
                if (player.Powers[(int)PowerType.Invulnerability] > 4 * 32
                    || (player.Powers[(int)PowerType.Invulnerability] & 8) != 0)
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
                if (player.Powers[(int)PowerType.Infrared] > 4 * 32
                    || (player.Powers[(int)PowerType.Infrared] & 8) != 0)
                {
                    // almost full bright
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
        public void P_BringUpWeapon(Player player)
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




        private static readonly int BONUSADD = 6;



        //
        // P_GiveAmmo
        // Num is the number of clip loads,
        // not the individual count (0= 1/2 clip).
        // Returns false if the ammo can't be picked up at all
        //

        public bool GiveAmmo(Player player, AmmoType ammo, int num)
        {
            if (ammo == AmmoType.NoAmmo)
            {
                return false;
            }

            if (ammo < 0 || (int)ammo > (int)AmmoType.Count)
            {
                throw new Exception("P_GiveAmmo: bad type " + ammo);
            }

            if (player.Ammo[(int)ammo] == player.MaxAmmo[(int)ammo])
            {
                return false;
            }

            if (num != 0)
            {
                num *= AmmoInfo.Clip[(int)ammo];
            }
            else
            {
                num = AmmoInfo.Clip[(int)ammo] / 2;
            }

            if (Options.GameSkill == Skill.Baby
                || Options.GameSkill == Skill.Nightmare)
            {
                // give double ammo in trainer mode,
                // you'll need in nightmare
                num <<= 1;
            }

            var oldammo = player.Ammo[(int)ammo];
            player.Ammo[(int)ammo] += num;

            if (player.Ammo[(int)ammo] > player.MaxAmmo[(int)ammo])
            {
                player.Ammo[(int)ammo] = player.MaxAmmo[(int)ammo];
            }

            // If non zero ammo, 
            // don't change up weapons,
            // player was lower on purpose.
            if (oldammo != 0)
            {
                return true;
            }

            // We were down to zero,
            // so select a new weapon.
            // Preferences are not user selectable.
            switch (ammo)
            {
                case AmmoType.Clip:
                    if (player.ReadyWeapon == WeaponType.Fist)
                    {
                        if (player.WeaponOwned[(int)WeaponType.Chaingun])
                        {
                            player.PendingWeapon = WeaponType.Chaingun;
                        }
                        else
                        {
                            player.PendingWeapon = WeaponType.Pistol;
                        }
                    }
                    break;

                case AmmoType.Shell:
                    if (player.ReadyWeapon == WeaponType.Fist
                        || player.ReadyWeapon == WeaponType.Pistol)
                    {
                        if (player.WeaponOwned[(int)WeaponType.Shotgun])
                        {
                            player.PendingWeapon = WeaponType.Shotgun;
                        }
                    }
                    break;

                case AmmoType.Cell:
                    if (player.ReadyWeapon == WeaponType.Fist
                        || player.ReadyWeapon == WeaponType.Pistol)
                    {
                        if (player.WeaponOwned[(int)WeaponType.Plasma])
                        {
                            player.PendingWeapon = WeaponType.Plasma;
                        }
                    }
                    break;

                case AmmoType.Missile:
                    if (player.ReadyWeapon == WeaponType.Fist)
                    {
                        if (player.WeaponOwned[(int)WeaponType.Missile])
                        {
                            player.PendingWeapon = WeaponType.Missile;
                        }
                    }
                    break;

                default:
                    break;
            }

            return true;
        }


        //
        // P_GiveWeapon
        // The weapon name may have a MF_DROPPED flag ored in.
        //
        public bool GiveWeapon(Player player, WeaponType weapon, bool dropped)
        {
            if (Options.NetGame && (Options.Deathmatch != 2)
             && !dropped)
            {
                // leave placed weapons forever on net games
                if (player.WeaponOwned[(int)weapon])
                {
                    return false;
                }

                player.BonusCount += BONUSADD;
                player.WeaponOwned[(int)weapon] = true;

                if (Options.Deathmatch != 0)
                {
                    GiveAmmo(player, Info.WeaponInfos[(int)weapon].Ammo, 5);
                }
                else
                {
                    GiveAmmo(player, Info.WeaponInfos[(int)weapon].Ammo, 2);
                }

                player.PendingWeapon = weapon;

                if (player == Players[consoleplayer])
                {
                    StartSound(null, Sfx.WPNUP);
                }

                return false;
            }

            bool gaveammo;
            if (Info.WeaponInfos[(int)weapon].Ammo != AmmoType.NoAmmo)
            {
                // give one clip with a dropped weapon,
                // two clips with a found weapon
                if (dropped)
                {
                    gaveammo = GiveAmmo(player, Info.WeaponInfos[(int)weapon].Ammo, 1);
                }
                else
                {
                    gaveammo = GiveAmmo(player, Info.WeaponInfos[(int)weapon].Ammo, 2);
                }
            }
            else
            {
                gaveammo = false;
            }

            bool gaveweapon;
            if (player.WeaponOwned[(int)weapon])
            {
                gaveweapon = false;
            }
            else
            {
                gaveweapon = true;
                player.WeaponOwned[(int)weapon] = true;
                player.PendingWeapon = weapon;
            }

            return (gaveweapon || gaveammo);
        }



        //
        // P_GiveBody
        // Returns false if the body isn't needed at all
        //
        public bool GiveBody(Player player, int num)
        {
            if (player.Health >= Player.MAXHEALTH)
            {
                return false;
            }

            player.Health += num;
            if (player.Health > Player.MAXHEALTH)
            {
                player.Health = Player.MAXHEALTH;
            }

            player.Mobj.Health = player.Health;

            return true;
        }



        //
        // P_GiveArmor
        // Returns false if the armor is worse
        // than the current armor.
        //
        public bool GiveArmor(Player player, int armortype)
        {
            var hits = armortype * 100;

            if (player.ArmorPoints >= hits)
            {
                // don't pick up
                return false;
            }

            player.ArmorType = armortype;
            player.ArmorPoints = hits;

            return true;
        }



        //
        // P_GiveCard
        //
        private void GiveCard(Player player, CardType card)
        {
            if (player.Cards[(int)card])
            {
                return;
            }

            player.BonusCount = BONUSADD;
            player.Cards[(int)card] = true;
        }


        //
        // P_GivePower
        //
        public bool GivePower(Player player, PowerType power)
        {
            if (power == PowerType.Invulnerability)
            {
                player.Powers[(int)power] = PowerDuration.Invulnerability;
                return true;
            }

            if (power == PowerType.Invisibility)
            {
                player.Powers[(int)power] = PowerDuration.Invisibility;
                player.Mobj.Flags |= MobjFlags.Shadow;
                return true;
            }

            if (power == PowerType.Infrared)
            {
                player.Powers[(int)power] = PowerDuration.Infrared;
                return true;
            }

            if (power == PowerType.IronFeet)
            {
                player.Powers[(int)power] = PowerDuration.IronFeet;
                return true;
            }

            if (power == PowerType.Strength)
            {
                GiveBody(player, 100);
                player.Powers[(int)power] = 1;
                return true;
            }

            if (player.Powers[(int)power] != 0)
            {
                // already got it
                return false;
            }

            player.Powers[(int)power] = 1;

            return true;
        }





        //
        // P_TouchSpecialThing
        //
        public void TouchSpecialThing(Mobj special, Mobj toucher)
        {
            var delta = special.Z - toucher.Z;

            if (delta > toucher.Height
                || delta < Fixed.FromInt(-8))
            {
                // out of reach
                return;
            }

            var sound = Sfx.ITEMUP;
            var player = toucher.Player;

            // Dead thing touching.
            // Can happen with a sliding player corpse.
            if (toucher.Health <= 0)
            {
                return;
            }

            // Identify by sprite.
            switch (special.Sprite)
            {
                // armor
                case Sprite.ARM1:
                    if (!GiveArmor(player, 1))
                    {
                        return;
                    }
                    //player.Message = GOTARMOR;
                    break;

                case Sprite.ARM2:
                    if (!GiveArmor(player, 2))
                    {
                        return;
                    }
                    //player.Message = GOTMEGA;
                    break;

                // bonus items
                case Sprite.BON1:
                    player.Health++; // can go over 100%
                    if (player.Health > 200)
                    {
                        player.Health = 200;
                    }
                    player.Mobj.Health = player.Health;
                    //player.Message = GOTHTHBONUS;
                    break;

                case Sprite.BON2:
                    player.ArmorPoints++; // can go over 100%
                    if (player.ArmorPoints > 200)
                    {
                        player.ArmorPoints = 200;
                    }
                    if (player.ArmorType == 0)
                    {
                        player.ArmorType = 1;
                    }
                    //player.Message = GOTARMBONUS;
                    break;

                case Sprite.SOUL:
                    player.Health += 100;
                    if (player.Health > 200)
                    {
                        player.Health = 200;
                    }
                    player.Mobj.Health = player.Health;
                    //player.Message = GOTSUPER;
                    sound = Sfx.GETPOW;
                    break;

                case Sprite.MEGA:
                    if (Options.GameMode != GameMode.Commercial)
                    {
                        return;
                    }

                    player.Health = 200;
                    player.Mobj.Health = player.Health;
                    GiveArmor(player, 2);
                    //player.Message = GOTMSPHERE;
                    sound = Sfx.GETPOW;
                    break;

                // cards
                // leave cards for everyone
                case Sprite.BKEY:
                    if (!player.Cards[(int)CardType.BlueCard])
                    {
                        //player.Message = GOTBLUECARD;
                    }
                    GiveCard(player, CardType.BlueCard);
                    if (!Options.NetGame)
                    {
                        break;
                    }
                    return;

                case Sprite.YKEY:
                    if (!player.Cards[(int)CardType.YellowCard])
                    {
                        //player.Message = GOTYELWCARD;
                    }
                    GiveCard(player, CardType.YellowCard);
                    if (!Options.NetGame)
                    {
                        break;
                    }
                    return;

                case Sprite.RKEY:
                    if (!player.Cards[(int)CardType.RedCard])
                    {
                        //player.Message = GOTREDCARD;
                    }
                    GiveCard(player, CardType.RedCard);
                    if (!Options.NetGame)
                    {
                        break;
                    }
                    return;

                case Sprite.BSKU:
                    if (!player.Cards[(int)CardType.BlueSkull])
                    {
                        //player.Message = GOTBLUESKUL;
                    }
                    GiveCard(player, CardType.BlueSkull);
                    if (!Options.NetGame)
                    {
                        break;
                    }
                    return;

                case Sprite.YSKU:
                    if (!player.Cards[(int)CardType.YellowSkull])
                    {
                        //player.Message = GOTYELWSKUL;
                    }
                    GiveCard(player, CardType.YellowSkull);
                    if (!Options.NetGame)
                    {
                        break;
                    }
                    return;

                case Sprite.RSKU:
                    if (!player.Cards[(int)CardType.RedSkull])
                    {
                        //player.Message = GOTREDSKULL;
                    }
                    GiveCard(player, CardType.RedSkull);
                    if (!Options.NetGame)
                    {
                        break;
                    }
                    return;

                // medikits, heals
                case Sprite.STIM:
                    if (!GiveBody(player, 10))
                    {
                        return;
                    }
                    //player.Message = GOTSTIM;
                    break;

                case Sprite.MEDI:
                    if (!GiveBody(player, 25))
                    {
                        return;
                    }
                    if (player.Health < 25)
                    {
                        //player.Message = GOTMEDINEED;
                    }
                    else
                    {
                        //player.Message = GOTMEDIKIT;
                    }
                    break;


                // power ups
                case Sprite.PINV:
                    if (!GivePower(player, PowerType.Invulnerability))
                    {
                        return;
                    }
                    //player.Message = GOTINVUL;
                    sound = Sfx.GETPOW;
                    break;

                case Sprite.PSTR:
                    if (!GivePower(player, PowerType.Strength))
                    {
                        return;
                    }
                    //player.Message = GOTBERSERK;
                    if (player.ReadyWeapon != WeaponType.Fist)
                    {
                        player.PendingWeapon = WeaponType.Fist;
                    }
                    sound = Sfx.GETPOW;
                    break;

                case Sprite.PINS:
                    if (!GivePower(player, PowerType.Invisibility))
                    {
                        return;
                    }
                    //player.Message = GOTINVIS;
                    sound = Sfx.GETPOW;
                    break;

                case Sprite.SUIT:
                    if (!GivePower(player, PowerType.IronFeet))
                    {
                        return;
                    }
                    //player.Message = GOTSUIT;
                    sound = Sfx.GETPOW;
                    break;

                case Sprite.PMAP:
                    if (!GivePower(player, PowerType.AllMap))
                    {
                        return;
                    }
                    //player.Message = GOTMAP;
                    sound = Sfx.GETPOW;
                    break;

                case Sprite.PVIS:
                    if (!GivePower(player, PowerType.Infrared))
                    {
                        return;
                    }
                    //player.Message = GOTVISOR;
                    sound = Sfx.GETPOW;
                    break;

                // ammo
                case Sprite.CLIP:
                    if ((special.Flags & MobjFlags.Dropped) != 0)
                    {
                        if (!GiveAmmo(player, AmmoType.Clip, 0))
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (!GiveAmmo(player, AmmoType.Clip, 1))
                        {
                            return;
                        }
                    }
                    //player.Message = GOTCLIP;
                    break;

                case Sprite.AMMO:
                    if (!GiveAmmo(player, AmmoType.Clip, 5))
                    {
                        return;
                    }
                    //player.Message = GOTCLIPBOX;
                    break;

                case Sprite.ROCK:
                    if (!GiveAmmo(player, AmmoType.Missile, 1))
                    {
                        return;
                    }
                    //player.Message = GOTROCKET;
                    break;

                case Sprite.BROK:
                    if (!GiveAmmo(player, AmmoType.Missile, 5))
                    {
                        return;
                    }
                    //player.Message = GOTROCKBOX;
                    break;

                case Sprite.CELL:
                    if (!GiveAmmo(player, AmmoType.Cell, 1))
                    {
                        return;
                    }
                    //player.Message = GOTCELL;
                    break;

                case Sprite.CELP:
                    if (!GiveAmmo(player, AmmoType.Cell, 5))
                    {
                        return;
                    }
                    //player.Message = GOTCELLBOX;
                    break;

                case Sprite.SHEL:
                    if (!GiveAmmo(player, AmmoType.Shell, 1))
                    {
                        return;
                    }
                    //player.Message = GOTSHELLS;
                    break;

                case Sprite.SBOX:
                    if (!GiveAmmo(player, AmmoType.Shell, 5))
                    {
                        return;
                    }
                    //player.Message = GOTSHELLBOX;
                    break;

                case Sprite.BPAK:
                    if (!player.Backpack)
                    {
                        for (var i = 0; i < (int)AmmoType.Count; i++)
                        {
                            player.MaxAmmo[i] *= 2;
                        }
                        player.Backpack = true;
                    }
                    for (var i = 0; i < (int)AmmoType.Count; i++)
                    {
                        GiveAmmo(player, (AmmoType)i, 1);
                    }
                    //player.Message = GOTBACKPACK;
                    break;

                // weapons
                case Sprite.BFUG:
                    if (!GiveWeapon(player, WeaponType.Bfg, false))
                    {
                        return;
                    }
                    //player.Message = GOTBFG9000;
                    sound = Sfx.WPNUP;
                    break;

                case Sprite.MGUN:
                    if (!GiveWeapon(player, WeaponType.Chaingun, (special.Flags & MobjFlags.Dropped) != 0))
                    {
                        return;
                    }
                    //player.Message = GOTCHAINGUN;
                    sound = Sfx.WPNUP;
                    break;

                case Sprite.CSAW:
                    if (!GiveWeapon(player, WeaponType.Chainsaw, false))
                    {
                        return;
                    }
                    //player.Message = GOTCHAINSAW;
                    sound = Sfx.WPNUP;
                    break;

                case Sprite.LAUN:
                    if (!GiveWeapon(player, WeaponType.Missile, false))
                    {
                        return;
                    }
                    //player.Message = GOTLAUNCHER;
                    sound = Sfx.WPNUP;
                    break;

                case Sprite.PLAS:
                    if (!GiveWeapon(player, WeaponType.Plasma, false))
                    {
                        return;
                    }
                    //player.Message = GOTPLASMA;
                    sound = Sfx.WPNUP;
                    break;

                case Sprite.SHOT:
                    if (!GiveWeapon(player, WeaponType.Shotgun, (special.Flags & MobjFlags.Dropped) != 0))
                    {
                        return;
                    }
                    //player.Message = GOTSHOTGUN;
                    sound = Sfx.WPNUP;
                    break;

                case Sprite.SGN2:
                    if (!GiveWeapon(player, WeaponType.SuperShotgun, (special.Flags & MobjFlags.Dropped) != 0))
                    {
                        return;
                    }
                    //player.Message = GOTSHOTGUN2;
                    sound = Sfx.WPNUP;
                    break;

                default:
                    throw new Exception("P_SpecialThing: Unknown gettable thing");
            }

            if ((special.Flags & MobjFlags.CountItem) != 0)
            {
                player.ItemCount++;
            }

            ThingAllocation.RemoveMobj(special);

            player.BonusCount += BONUSADD;

            if (player == Players[consoleplayer])
            {
                StartSound(null, sound);
            }
        }
    }
}
