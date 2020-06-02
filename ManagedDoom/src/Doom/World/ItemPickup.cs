using System;

namespace ManagedDoom
{
    public sealed class ItemPickup
    {
        private World world;

        public ItemPickup(World world)
        {
            this.world = world;
        }



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
                num *= DoomInfo.AmmoInfos.Clip[(int)ammo];
            }
            else
            {
                num = DoomInfo.AmmoInfos.Clip[(int)ammo] / 2;
            }

            if (world.Options.Skill == GameSkill.Baby
                || world.Options.Skill == GameSkill.Nightmare)
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

        private static readonly int bonusAdd = 6;

        public bool GiveWeapon(Player player, WeaponType weapon, bool dropped)
        {
            if (world.Options.NetGame && (world.Options.Deathmatch != 2) && !dropped)
            {
                // Leave placed weapons forever on net games.
                if (player.WeaponOwned[(int)weapon])
                {
                    return false;
                }

                player.BonusCount += bonusAdd;
                player.WeaponOwned[(int)weapon] = true;

                if (world.Options.Deathmatch != 0)
                {
                    GiveAmmo(player, DoomInfo.WeaponInfos[(int)weapon].Ammo, 5);
                }
                else
                {
                    GiveAmmo(player, DoomInfo.WeaponInfos[(int)weapon].Ammo, 2);
                }

                player.PendingWeapon = weapon;

                if (player == world.Players[world.consoleplayer])
                {
                    world.StartSound(null, Sfx.WPNUP);
                }

                return false;
            }

            bool gaveAmmo;
            if (DoomInfo.WeaponInfos[(int)weapon].Ammo != AmmoType.NoAmmo)
            {
                // Give one clip with a dropped weapon, two clips with a found weapon.
                if (dropped)
                {
                    gaveAmmo = GiveAmmo(player, DoomInfo.WeaponInfos[(int)weapon].Ammo, 1);
                }
                else
                {
                    gaveAmmo = GiveAmmo(player, DoomInfo.WeaponInfos[(int)weapon].Ammo, 2);
                }
            }
            else
            {
                gaveAmmo = false;
            }

            bool gaveWeapon;
            if (player.WeaponOwned[(int)weapon])
            {
                gaveWeapon = false;
            }
            else
            {
                gaveWeapon = true;
                player.WeaponOwned[(int)weapon] = true;
                player.PendingWeapon = weapon;
            }

            return (gaveWeapon || gaveAmmo);
        }



        //
        // P_GiveBody
        // Returns false if the body isn't needed at all
        //
        private bool GiveHealth(Player player, int amount)
        {
            if (player.Health >= Player.MAXHEALTH)
            {
                return false;
            }

            player.Health += amount;
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
        private bool GiveArmor(Player player, int type)
        {
            var hits = type * 100;

            if (player.ArmorPoints >= hits)
            {
                // Don't pick up.
                return false;
            }

            player.ArmorType = type;
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

            player.BonusCount = bonusAdd;
            player.Cards[(int)card] = true;
        }


        //
        // P_GivePower
        //
        private bool GivePower(Player player, PowerType type)
        {
            if (type == PowerType.Invulnerability)
            {
                player.Powers[(int)type] = DoomInfo.PowerDuration.Invulnerability;
                return true;
            }

            if (type == PowerType.Invisibility)
            {
                player.Powers[(int)type] = DoomInfo.PowerDuration.Invisibility;
                player.Mobj.Flags |= MobjFlags.Shadow;
                return true;
            }

            if (type == PowerType.Infrared)
            {
                player.Powers[(int)type] = DoomInfo.PowerDuration.Infrared;
                return true;
            }

            if (type == PowerType.IronFeet)
            {
                player.Powers[(int)type] = DoomInfo.PowerDuration.IronFeet;
                return true;
            }

            if (type == PowerType.Strength)
            {
                GiveHealth(player, 100);
                player.Powers[(int)type] = 1;
                return true;
            }

            if (player.Powers[(int)type] != 0)
            {
                // Already got it.
                return false;
            }

            player.Powers[(int)type] = 1;

            return true;
        }





        //
        // P_TouchSpecialThing
        //
        public void TouchSpecialThing(Mobj special, Mobj toucher)
        {
            var delta = special.Z - toucher.Z;

            if (delta > toucher.Height || delta < Fixed.FromInt(-8))
            {
                // Out of reach.
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
                // Armor.
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

                // Bonus items.
                case Sprite.BON1:
                    // Can go over 100%.
                    player.Health++;
                    if (player.Health > 200)
                    {
                        player.Health = 200;
                    }
                    player.Mobj.Health = player.Health;
                    //player.Message = GOTHTHBONUS;
                    break;

                case Sprite.BON2:
                    // Can go over 100%.
                    player.ArmorPoints++;
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
                    if (world.Options.GameMode != GameMode.Commercial)
                    {
                        return;
                    }

                    player.Health = 200;
                    player.Mobj.Health = player.Health;
                    GiveArmor(player, 2);
                    //player.Message = GOTMSPHERE;
                    sound = Sfx.GETPOW;
                    break;

                // Cards.
                // Leave cards for everyone.
                case Sprite.BKEY:
                    if (!player.Cards[(int)CardType.BlueCard])
                    {
                        //player.Message = GOTBLUECARD;
                    }
                    GiveCard(player, CardType.BlueCard);
                    if (!world.Options.NetGame)
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
                    if (!world.Options.NetGame)
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
                    if (!world.Options.NetGame)
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
                    if (!world.Options.NetGame)
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
                    if (!world.Options.NetGame)
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
                    if (!world.Options.NetGame)
                    {
                        break;
                    }
                    return;

                // Medikits, heals.
                case Sprite.STIM:
                    if (!GiveHealth(player, 10))
                    {
                        return;
                    }
                    //player.Message = GOTSTIM;
                    break;

                case Sprite.MEDI:
                    if (!GiveHealth(player, 25))
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


                // Power ups.
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

                // Ammo.
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

                // Weapons.
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
                    throw new Exception("Unknown gettable thing!");
            }

            if ((special.Flags & MobjFlags.CountItem) != 0)
            {
                player.ItemCount++;
            }

            world.ThingAllocation.RemoveMobj(special);

            player.BonusCount += bonusAdd;

            if (player == world.Players[world.consoleplayer])
            {
                world.StartSound(null, sound);
            }
        }
    }
}
