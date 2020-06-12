using System;
using System.Diagnostics;

namespace ManagedDoom
{
    public sealed class LoadGame
    {
        private static readonly int descriptionSize = 24;
        private static readonly int versionSize = 16;

        private byte[] data;
        private int save_p;

        private GameOptions options;
        private Player[] players;

        public LoadGame(byte[] data, World world)
        {
            this.data = data;

            var description = ReadDescription();
            var version = ReadVersion();

            options = new GameOptions();
            options.Skill = (GameSkill)data[save_p++];
            options.Episode = data[save_p++];
            options.Map = data[save_p++];
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                options.PlayerInGame[i] = data[save_p++] != 0;
            }

            players = new Player[Player.MaxPlayerCount];
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                players[i] = new Player(i);
                players[i].InGame = options.PlayerInGame[i];
            }

            var a = data[save_p++];
            var b = data[save_p++];
            var c = data[save_p++];
            var levelTime = (a << 16) + (b << 8) + c;

            UnArchivePlayers();
            UnArchiveWorld(world);
            UnArchiveThinkers(world);
            UnArchiveSpecials(world);

            Console.WriteLine("END");
        }

        private void PADSAVEP()
        {
            save_p += (4 - (save_p & 3)) & 3;
        }

        private string ReadDescription()
        {
            var value = DoomInterop.ToString(data, save_p, descriptionSize);
            save_p += descriptionSize;
            return value;
        }

        private string ReadVersion()
        {
            var value = DoomInterop.ToString(data, save_p, versionSize);
            save_p += versionSize;
            return value;
        }

        private void UnArchivePlayers()
        {
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (!options.PlayerInGame[i])
                {
                    continue;
                }

                PADSAVEP();

                save_p = UnArchivePlayer(players[i], data, save_p);
            }
        }

        private void UnArchiveWorld(World world)
        {
            // Do sectors.
            var sectors = world.Map.Sectors;
            for (var i = 0; i < sectors.Length; i++)
            {
                save_p = UnArchiveSector(sectors[i], data, save_p);
            }

            // Do lines.
            var lines = world.Map.Lines;
            for (var i = 0; i < lines.Length; i++)
            {
                save_p = UnArchiveLine(lines[i], data, save_p);
            }
        }

        private void UnArchiveThinkers(World world)
        {
            var thinkers = world.Thinkers;
            var ta = world.ThingAllocation;

            // Remove all the current thinkers.
            foreach (var thinker in thinkers)
            {
                var mobj = thinker as Mobj;
                if (mobj != null)
                {
                    ta.RemoveMobj(mobj);
                }
            }
            thinkers.Reset();

            // Read in saved thinkers.
            while (true)
            {
                var tclass = (ThinkerClass)data[save_p++];
                switch (tclass)
                {
                    case ThinkerClass.End:
                        // End of list.
                        return;

                    case ThinkerClass.Mobj:
                        PADSAVEP();
                        var mobj = ThinkerPool.RentMobj(world);
                        mobj.ThinkerState = ReadThinkerState(data, save_p + 8);
                        mobj.X = new Fixed(BitConverter.ToInt32(data, save_p + 12));
                        mobj.Y = new Fixed(BitConverter.ToInt32(data, save_p + 16));
                        mobj.Z = new Fixed(BitConverter.ToInt32(data, save_p + 20));
                        mobj.Angle = new Angle(BitConverter.ToInt32(data, save_p + 32));
                        mobj.Sprite = (Sprite)BitConverter.ToInt32(data, save_p + 36);
                        mobj.Frame = BitConverter.ToInt32(data, save_p + 40);
                        mobj.Radius = new Fixed(BitConverter.ToInt32(data, save_p + 64));
                        mobj.Height = new Fixed(BitConverter.ToInt32(data, save_p + 68));
                        mobj.MomX = new Fixed(BitConverter.ToInt32(data, save_p + 72));
                        mobj.MomY = new Fixed(BitConverter.ToInt32(data, save_p + 76));
                        mobj.MomZ = new Fixed(BitConverter.ToInt32(data, save_p + 80));
                        mobj.Type = (MobjType)BitConverter.ToInt32(data, save_p + 88);
                        mobj.Info = DoomInfo.MobjInfos[(int)mobj.Type];
                        mobj.Tics = BitConverter.ToInt32(data, save_p + 96);
                        mobj.State = DoomInfo.States[BitConverter.ToInt32(data, save_p + 100)];
                        mobj.Flags = (MobjFlags)BitConverter.ToInt32(data, save_p + 104);
                        mobj.Health = BitConverter.ToInt32(data, save_p + 108);
                        mobj.MoveDir = (Direction)BitConverter.ToInt32(data, save_p + 112);
                        mobj.MoveCount = BitConverter.ToInt32(data, save_p + 116);
                        mobj.ReactionTime = BitConverter.ToInt32(data, save_p + 124);
                        mobj.Threshold = BitConverter.ToInt32(data, save_p + 128);
                        var playerNumber = BitConverter.ToInt32(data, save_p + 132);
                        if (playerNumber != 0)
                        {
                            mobj.Player = world.Players[playerNumber - 1];
                            mobj.Player.Mobj = mobj;
                        }
                        mobj.LastLook = BitConverter.ToInt32(data, save_p + 136);
                        mobj.SpawnPoint = new MapThing(
                            Fixed.FromInt(BitConverter.ToInt16(data, save_p + 140)),
                            Fixed.FromInt(BitConverter.ToInt16(data, save_p + 142)),
                            new Angle(Angle.Ang45.Data * (uint)(BitConverter.ToInt16(data, save_p + 144) / 45)),
                            BitConverter.ToInt16(data, save_p + 146),
                            (ThingFlags)BitConverter.ToInt16(data, save_p + 148));
                        save_p += 154;

                        world.ThingMovement.SetThingPosition(mobj);
                        mobj.FloorZ = mobj.Subsector.Sector.FloorHeight;
                        mobj.CeilingZ = mobj.Subsector.Sector.CeilingHeight;
                        thinkers.Add(mobj);
                        break;

                    default:
                        throw new Exception("Unknown thinker class in savegame!");
                }
            }
        }

        private void UnArchiveSpecials(World world)
        {
            var thinkers = world.Thinkers;
            var sa = world.SectorAction;

            // read in saved thinkers
            while (true)
            {
                var tclass = (SpecialClass)data[save_p++];
                switch (tclass)
                {
                    case SpecialClass.EndSpecials:
                        // End of list.
                        return;

                    case SpecialClass.Ceiling:
                        PADSAVEP();
                        var ceiling = ThinkerPool.RentCeiligMove(world);
                        ceiling.ThinkerState = ReadThinkerState(data, save_p + 8);
                        ceiling.Type = (CeilingMoveType)BitConverter.ToInt32(data, save_p + 12);
                        ceiling.Sector = world.Map.Sectors[BitConverter.ToInt32(data, save_p + 16)];
                        ceiling.Sector.SpecialData = ceiling;
                        ceiling.BottomHeight = new Fixed(BitConverter.ToInt32(data, save_p + 20));
                        ceiling.TopHeight = new Fixed(BitConverter.ToInt32(data, save_p + 24));
                        ceiling.Speed = new Fixed(BitConverter.ToInt32(data, save_p + 28));
                        ceiling.Crush = BitConverter.ToInt32(data, save_p + 32) != 0;
                        ceiling.Direction = BitConverter.ToInt32(data, save_p + 36);
                        ceiling.Tag = BitConverter.ToInt32(data, save_p + 40);
                        ceiling.OldDirection = BitConverter.ToInt32(data, save_p + 44);
                        save_p += 48;

                        thinkers.Add(ceiling);
                        sa.AddActiveCeiling(ceiling);
                        break;

                    case SpecialClass.Door:
                        PADSAVEP();
                        var door = ThinkerPool.RentVlDoor(world);
                        door.ThinkerState = ReadThinkerState(data, save_p + 8);
                        door.Type = (VlDoorType)BitConverter.ToInt32(data, save_p + 12);
                        door.Sector = world.Map.Sectors[BitConverter.ToInt32(data, save_p + 16)];
                        door.Sector.SpecialData = door;
                        door.TopHeight = new Fixed(BitConverter.ToInt32(data, save_p + 20));
                        door.Speed = new Fixed(BitConverter.ToInt32(data, save_p + 24));
                        door.Direction = BitConverter.ToInt32(data, save_p + 28);
                        door.TopWait = BitConverter.ToInt32(data, save_p + 32);
                        door.TopCountDown = BitConverter.ToInt32(data, save_p + 36);
                        save_p += 40;

                        thinkers.Add(door);
                        break;

                    case SpecialClass.Floor:
                        PADSAVEP();
                        var floor = ThinkerPool.RentFloorMove(world);
                        floor.ThinkerState = ReadThinkerState(data, save_p + 8);
                        floor.Type = (FloorMoveType)BitConverter.ToInt32(data, save_p + 12);
                        floor.Crush = BitConverter.ToInt32(data, save_p + 16) != 0;
                        floor.Sector = world.Map.Sectors[BitConverter.ToInt32(data, save_p + 20)];
                        floor.Sector.SpecialData = floor;
                        floor.Direction = BitConverter.ToInt32(data, save_p + 24);
                        floor.NewSpecial = (SectorSpecial)BitConverter.ToInt32(data, save_p + 28);
                        floor.Texture = BitConverter.ToInt32(data, save_p + 32);
                        floor.FloorDestHeight = new Fixed(BitConverter.ToInt32(data, save_p + 36));
                        floor.Speed = new Fixed(BitConverter.ToInt32(data, save_p + 40));
                        save_p += 44;

                        thinkers.Add(floor);
                        break;

                    case SpecialClass.Plat:
                        PADSAVEP();
                        var plat = ThinkerPool.RentPlatform(world);
                        plat.ThinkerState = ReadThinkerState(data, save_p + 8);
                        plat.Sector = world.Map.Sectors[BitConverter.ToInt32(data, save_p + 12)];
                        plat.Sector.SpecialData = plat;
                        plat.Speed = new Fixed(BitConverter.ToInt32(data, save_p + 16));
                        plat.Low = new Fixed(BitConverter.ToInt32(data, save_p + 20));
                        plat.High = new Fixed(BitConverter.ToInt32(data, save_p + 24));
                        plat.Wait = BitConverter.ToInt32(data, save_p + 28);
                        plat.Count = BitConverter.ToInt32(data, save_p + 32);
                        plat.Status = (PlatformState)BitConverter.ToInt32(data, save_p + 36);
                        plat.Oldstatus = (PlatformState)BitConverter.ToInt32(data, save_p + 40);
                        plat.Crush = BitConverter.ToInt32(data, save_p + 44) != 0;
                        plat.Tag = BitConverter.ToInt32(data, save_p + 48);
                        plat.Type = (PlatformType)BitConverter.ToInt32(data, save_p + 52);
                        save_p += 56;

                        thinkers.Add(plat);
                        sa.AddActivePlatform(plat);
                        break;

                    case SpecialClass.Flash:
                        PADSAVEP();
                        var flash = ThinkerPool.RentLightFlash(world);
                        flash.ThinkerState = ReadThinkerState(data, save_p + 8);
                        flash.sector = world.Map.Sectors[BitConverter.ToInt32(data, save_p + 12)];
                        flash.count = BitConverter.ToInt32(data, save_p + 16);
                        flash.maxlight = BitConverter.ToInt32(data, save_p + 20);
                        flash.minlight = BitConverter.ToInt32(data, save_p + 24);
                        flash.maxtime = BitConverter.ToInt32(data, save_p + 28);
                        flash.mintime = BitConverter.ToInt32(data, save_p + 32);
                        save_p += 36;

                        thinkers.Add(flash);
                        break;

                    case SpecialClass.Strobe:
                        PADSAVEP();
                        var strobe = ThinkerPool.RentStrobeFlash(world);
                        strobe.ThinkerState = ReadThinkerState(data, save_p + 8);
                        strobe.sector = world.Map.Sectors[BitConverter.ToInt32(data, save_p + 12)];
                        strobe.count = BitConverter.ToInt32(data, save_p + 16);
                        strobe.minlight = BitConverter.ToInt32(data, save_p + 20);
                        strobe.maxlight = BitConverter.ToInt32(data, save_p + 24);
                        strobe.darktime = BitConverter.ToInt32(data, save_p + 28);
                        strobe.brighttime = BitConverter.ToInt32(data, save_p + 32);
                        save_p += 36;

                        thinkers.Add(strobe);
                        break;

                    case SpecialClass.Glow:
                        PADSAVEP();
                        var glow = ThinkerPool.RentGlowLight(world);
                        glow.ThinkerState = ReadThinkerState(data, save_p + 8);
                        glow.sector = world.Map.Sectors[BitConverter.ToInt32(data, save_p + 12)];
                        glow.minlight = BitConverter.ToInt32(data, save_p + 16);
                        glow.maxlight = BitConverter.ToInt32(data, save_p + 20);
                        glow.direction = BitConverter.ToInt32(data, save_p + 24);
                        save_p += 28;

                        thinkers.Add(glow);
                        break;

                    default:
                        throw new Exception("Unknown thinker class in savegame!");
                }
            }
        }




        private static ThinkerState ReadThinkerState(byte[] data, int p)
        {
            switch (BitConverter.ToInt32(data, p))
            {
                case 0:
                    return ThinkerState.InStasis;
                default:
                    return ThinkerState.Active;
            }
        }

        private static int UnArchivePlayer(Player player, byte[] data, int p)
        {
            player.Clear();

            player.ViewZ = new Fixed(BitConverter.ToInt32(data, p + 16));
            player.ViewHeight = new Fixed(BitConverter.ToInt32(data, p + 20));
            player.DeltaViewHeight = new Fixed(BitConverter.ToInt32(data, p + 24));
            player.Bob = new Fixed(BitConverter.ToInt32(data, p + 28));
            player.Health = BitConverter.ToInt32(data, p + 32);
            player.ArmorPoints = BitConverter.ToInt32(data, p + 36);
            player.ArmorType = BitConverter.ToInt32(data, p + 40);
            for (var i = 0; i < (int)PowerType.Count; i++)
            {
                player.Powers[i] = BitConverter.ToInt32(data, p + 44 + 4 * i);
            }
            for (var i = 0; i < (int)PowerType.Count; i++)
            {
                player.Cards[i] = BitConverter.ToInt32(data, p + 68 + 4 * i) != 0;
            }
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                player.Frags[i] = BitConverter.ToInt32(data, p + 96 + 4 * i);
            }
            player.ReadyWeapon = (WeaponType)BitConverter.ToInt32(data, p + 112);
            player.PendingWeapon = (WeaponType)BitConverter.ToInt32(data, p + 116);
            for (var i = 0; i < (int)WeaponType.Count; i++)
            {
                player.WeaponOwned[i] = BitConverter.ToInt32(data, p + 120 + 4 * i) != 0;
            }
            for (var i = 0; i < (int)AmmoType.Count; i++)
            {
                player.Ammo[i] = BitConverter.ToInt32(data, p + 156 + 4 * i);
            }
            for (var i = 0; i < (int)AmmoType.Count; i++)
            {
                player.MaxAmmo[i] = BitConverter.ToInt32(data, p + 172 + 4 * i);
            }
            player.AttackDown = BitConverter.ToInt32(data, p + 188) != 0;
            player.UseDown = BitConverter.ToInt32(data, p + 192) != 0;
            player.Cheats = (CheatFlags)BitConverter.ToInt32(data, p + 196);
            player.Refire = BitConverter.ToInt32(data, p + 200);
            player.KillCount = BitConverter.ToInt32(data, p + 204);
            player.ItemCount = BitConverter.ToInt32(data, p + 208);
            player.SecretCount = BitConverter.ToInt32(data, p + 212);
            player.DamageCount = BitConverter.ToInt32(data, p + 220);
            player.BonusCount = BitConverter.ToInt32(data, p + 224);
            player.ExtraLight = BitConverter.ToInt32(data, p + 232);
            player.FixedColorMap = BitConverter.ToInt32(data, p + 236);
            player.ColorMap = BitConverter.ToInt32(data, p + 240);
            for (var i = 0; i < (int)PlayerSprite.Count; i++)
            {
                player.PlayerSprites[i].State = DoomInfo.States[BitConverter.ToInt32(data, p + 244 + 16 * i)];
                player.PlayerSprites[i].Tics = BitConverter.ToInt32(data, p + 244 + 16 * i + 4);
                player.PlayerSprites[i].Sx = new Fixed(BitConverter.ToInt32(data, p + 244 + 16 * i + 8));
                player.PlayerSprites[i].Sy = new Fixed(BitConverter.ToInt32(data, p + 244 + 16 * i + 12));
            }
            player.DidSecret = BitConverter.ToInt32(data, p + 276) != 0;

            return p + 280;
        }

        private static int UnArchiveSector(Sector sector, byte[] data, int p)
        {
            sector.FloorHeight = Fixed.FromInt(BitConverter.ToInt16(data, p));
            sector.CeilingHeight = Fixed.FromInt(BitConverter.ToInt16(data, p + 2));
            sector.FloorFlat = BitConverter.ToInt16(data, p + 4);
            sector.CeilingFlat = BitConverter.ToInt16(data, p + 6);
            sector.LightLevel = BitConverter.ToInt16(data, p + 8);
            sector.Special = (SectorSpecial)BitConverter.ToInt16(data, p + 10);
            sector.Tag = BitConverter.ToInt16(data, p + 12);
            sector.SpecialData = null;
            sector.SoundTarget = null;
            return p + 14;
        }

        private static int UnArchiveLine(LineDef line, byte[] data, int p)
        {
            line.Flags = (LineFlags)BitConverter.ToInt16(data, p);
            line.Special = (LineSpecial)BitConverter.ToInt16(data, p + 2);
            line.Tag = BitConverter.ToInt16(data, p + 4);
            p += 6;

            if (line.Side0 != null)
            {
                var side = line.Side0;
                side.TextureOffset = Fixed.FromInt(BitConverter.ToInt16(data, p));
                side.RowOffset = Fixed.FromInt(BitConverter.ToInt16(data, p + 2));
                side.TopTexture = BitConverter.ToInt16(data, p + 4);
                side.BottomTexture = BitConverter.ToInt16(data, p + 6);
                side.MiddleTexture = BitConverter.ToInt16(data, p + 8);
                p += 10;
            }

            if (line.Side1 != null)
            {
                var side = line.Side1;
                side.TextureOffset = Fixed.FromInt(BitConverter.ToInt16(data, p));
                side.RowOffset = Fixed.FromInt(BitConverter.ToInt16(data, p + 2));
                side.TopTexture = BitConverter.ToInt16(data, p + 4);
                side.BottomTexture = BitConverter.ToInt16(data, p + 6);
                side.MiddleTexture = BitConverter.ToInt16(data, p + 8);
                p += 10;
            }

            return p;
        }
    }
}
