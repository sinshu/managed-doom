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
