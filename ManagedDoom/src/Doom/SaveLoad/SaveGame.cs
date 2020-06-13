using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace ManagedDoom
{
    public sealed class SaveGame
    {
        private static readonly int descriptionSize = 24;
        private static readonly int versionSize = 16;

        private byte[] data;
        private int save_p;

        public SaveGame(string description)
        {
            data = new byte[100000];
            save_p = 0;

            WriteDescription(description);
            WriteVersion();
        }

        private void WriteDescription(string description)
        {
            for (var i = 0; i < description.Length; i++)
            {
                data[i] = (byte)description[i];
            }
            save_p += descriptionSize;
        }

        private void WriteVersion()
        {
            var version = "version 109";
            for (var i = 0; i < version.Length; i++)
            {
                data[save_p + i] = (byte)version[i];
            }
            save_p += versionSize;
        }

        public void Save(World world, string path)
        {
            var options = world.Options;
            data[save_p++] = (byte)options.Skill;
            data[save_p++] = (byte)options.Episode;
            data[save_p++] = (byte)options.Map;
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                data[save_p++] = options.Players[i].InGame ? (byte)1 : (byte)0;
            }

            data[save_p++] = (byte)(world.levelTime >> 16);
            data[save_p++] = (byte)(world.levelTime >> 8);
            data[save_p++] = (byte)(world.levelTime);

            ArchivePlayers(world);
            ArchiveWorld(world);
            ArchiveThinkers(world);
            ArchiveSpecials(world);

            data[save_p++] = 0x1d;

            using (var writer = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                writer.Write(data, 0, save_p);
            }
        }



        private void PADSAVEP()
        {
            save_p += (4 - (save_p & 3)) & 3;
        }

        private void ArchivePlayers(World world)
        {
            var players = world.Options.Players;
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (!players[i].InGame)
                {
                    continue;
                }

                PADSAVEP();

                save_p = ArchivePlayer(players[i], data, save_p);
            }
        }

        private void ArchiveWorld(World world)
        {
            // Do sectors.
            var sectors = world.Map.Sectors;
            for (var i = 0; i < sectors.Length; i++)
            {
                save_p = ArchiveSector(sectors[i], data, save_p);
            }

            // Do lines.
            var lines = world.Map.Lines;
            for (var i = 0; i < lines.Length; i++)
            {
                save_p = ArchiveLine(lines[i], data, save_p);
            }
        }

        private void ArchiveThinkers(World world)
        {
            var thinkers = world.Thinkers;

            // Read in saved thinkers.
            foreach (var thinker in thinkers)
            {
                var mobj = thinker as Mobj;
                if (mobj != null)
                {
                    data[save_p++] = (byte)ThinkerClass.Mobj;
                    PADSAVEP();

                    WriteThinkerState(data, save_p + 8, mobj.ThinkerState);
                    Write(data, save_p + 12, mobj.X.Data);
                    Write(data, save_p + 16, mobj.Y.Data);
                    Write(data, save_p + 20, mobj.Z.Data);
                    Write(data, save_p + 32, mobj.Angle.Data);
                    Write(data, save_p + 36, (int)mobj.Sprite);
                    Write(data, save_p + 40, mobj.Frame);
                    Write(data, save_p + 56, mobj.FloorZ.Data);
                    Write(data, save_p + 60, mobj.CeilingZ.Data);
                    Write(data, save_p + 64, mobj.Radius.Data);
                    Write(data, save_p + 68, mobj.Height.Data);
                    Write(data, save_p + 72, mobj.MomX.Data);
                    Write(data, save_p + 76, mobj.MomY.Data);
                    Write(data, save_p + 80, mobj.MomZ.Data);
                    Write(data, save_p + 88, (int)mobj.Type);
                    Write(data, save_p + 96, mobj.Tics);
                    Write(data, save_p + 100, mobj.State.Number);
                    Write(data, save_p + 104, (int)mobj.Flags);
                    Write(data, save_p + 108, mobj.Health);
                    Write(data, save_p + 112, (int)mobj.MoveDir);
                    Write(data, save_p + 116, mobj.MoveCount);
                    Write(data, save_p + 124, mobj.ReactionTime);
                    Write(data, save_p + 128, mobj.Threshold);
                    if (mobj.Player == null)
                    {
                        Write(data, save_p + 132, 0);
                    }
                    else
                    {
                        Write(data, save_p + 132, mobj.Player.Number + 1);
                    }
                    Write(data, save_p + 136, mobj.LastLook);
                    if (mobj.SpawnPoint == null)
                    {
                        Write(data, save_p + 140, (short)0);
                        Write(data, save_p + 142, (short)0);
                        Write(data, save_p + 144, (short)0);
                        Write(data, save_p + 146, (short)0);
                        Write(data, save_p + 148, (short)0);
                    }
                    else
                    {
                        Write(data, save_p + 140, (short)mobj.SpawnPoint.X.ToIntFloor());
                        Write(data, save_p + 142, (short)mobj.SpawnPoint.Y.ToIntFloor());
                        Write(data, save_p + 144, (short)Math.Round(mobj.SpawnPoint.Angle.ToDegree()));
                        Write(data, save_p + 146, (short)mobj.SpawnPoint.Type);
                        Write(data, save_p + 148, (short)mobj.SpawnPoint.Flags);
                    }
                    save_p += 154;
                }
            }

            data[save_p++] = (byte)ThinkerClass.End;
        }

        private void ArchiveSpecials(World world)
        {
            var thinkers = world.Thinkers;
            var sa = world.SectorAction;

            // Read in saved thinkers.
            foreach (var thinker in thinkers)
            {
                if (thinker.ThinkerState == ThinkerState.InStasis)
                {
                    var ceiling = thinker as CeilingMove;
                    if (sa.CheckActiveCeiling(ceiling))
                    {
                        data[save_p++] = (byte)SpecialClass.Ceiling;
                        PADSAVEP();
                        WriteThinkerState(data, save_p + 8, ceiling.ThinkerState);
                        Write(data, save_p + 12, (int)ceiling.Type);
                        Write(data, save_p + 16, ceiling.Sector.Number);
                        Write(data, save_p + 20, ceiling.BottomHeight.Data);
                        Write(data, save_p + 24, ceiling.TopHeight.Data);
                        Write(data, save_p + 28, ceiling.Speed.Data);
                        Write(data, save_p + 32, ceiling.Crush ? 1 : 0);
                        Write(data, save_p + 36, ceiling.Direction);
                        Write(data, save_p + 40, ceiling.Tag);
                        Write(data, save_p + 44, ceiling.OldDirection);
                        save_p += 48;
                    }
                    continue;
                }

                {
                    var ceiling = thinker as CeilingMove;
                    if (ceiling != null)
                    {
                        data[save_p++] = (byte)SpecialClass.Ceiling;
                        PADSAVEP();
                        WriteThinkerState(data, save_p + 8, ceiling.ThinkerState);
                        Write(data, save_p + 12, (int)ceiling.Type);
                        Write(data, save_p + 16, ceiling.Sector.Number);
                        Write(data, save_p + 20, ceiling.BottomHeight.Data);
                        Write(data, save_p + 24, ceiling.TopHeight.Data);
                        Write(data, save_p + 28, ceiling.Speed.Data);
                        Write(data, save_p + 32, ceiling.Crush ? 1 : 0);
                        Write(data, save_p + 36, ceiling.Direction);
                        Write(data, save_p + 40, ceiling.Tag);
                        Write(data, save_p + 44, ceiling.OldDirection);
                        save_p += 48;
                        continue;
                    }
                }

                {
                    var door = thinker as VlDoor;
                    if (door != null)
                    {
                        data[save_p++] = (byte)SpecialClass.Door;
                        PADSAVEP();
                        WriteThinkerState(data, save_p + 8, door.ThinkerState);
                        Write(data, save_p + 12, (int)door.Type);
                        Write(data, save_p + 16, door.Sector.Number);
                        Write(data, save_p + 20, door.TopHeight.Data);
                        Write(data, save_p + 24, door.Speed.Data);
                        Write(data, save_p + 28, door.Direction);
                        Write(data, save_p + 32, door.TopWait);
                        Write(data, save_p + 36, door.TopCountDown);
                        save_p += 40;
                        continue;
                    }
                }

                {
                    var floor = thinker as FloorMove;
                    if (floor != null)
                    {
                        data[save_p++] = (byte)SpecialClass.Floor;
                        PADSAVEP();
                        WriteThinkerState(data, save_p + 8, floor.ThinkerState);
                        Write(data, save_p + 12, (int)floor.Type);
                        Write(data, save_p + 16, floor.Crush ? 1 : 0);
                        Write(data, save_p + 20, floor.Sector.Number);
                        Write(data, save_p + 24, floor.Direction);
                        Write(data, save_p + 28, (int)floor.NewSpecial);
                        Write(data, save_p + 32, floor.Texture);
                        Write(data, save_p + 36, floor.FloorDestHeight.Data);
                        Write(data, save_p + 40, floor.Speed.Data);
                        save_p += 44;
                        continue;
                    }
                }

                {
                    var plat = thinker as Platform;
                    if (plat != null)
                    {
                        data[save_p++] = (byte)SpecialClass.Plat;
                        PADSAVEP();
                        WriteThinkerState(data, save_p + 8, plat.ThinkerState);
                        Write(data, save_p + 12, plat.Sector.Number);
                        Write(data, save_p + 16, plat.Speed.Data);
                        Write(data, save_p + 20, plat.Low.Data);
                        Write(data, save_p + 24, plat.High.Data);
                        Write(data, save_p + 28, plat.Wait);
                        Write(data, save_p + 32, plat.Count);
                        Write(data, save_p + 36, (int)plat.Status);
                        Write(data, save_p + 40, (int)plat.Oldstatus);
                        Write(data, save_p + 44, plat.Crush ? 1 : 0);
                        Write(data, save_p + 48, plat.Tag);
                        Write(data, save_p + 52, (int)plat.Type);
                        save_p += 56;
                        continue;
                    }
                }

                {
                    var flash = thinker as LightFlash;
                    if (flash != null)
                    {
                        data[save_p++] = (byte)SpecialClass.Flash;
                        PADSAVEP();
                        WriteThinkerState(data, save_p + 8, flash.ThinkerState);
                        Write(data, save_p + 12, flash.sector.Number);
                        Write(data, save_p + 16, flash.count);
                        Write(data, save_p + 20, flash.maxlight);
                        Write(data, save_p + 24, flash.minlight);
                        Write(data, save_p + 28, flash.maxtime);
                        Write(data, save_p + 32, flash.mintime);
                        save_p += 36;
                        continue;
                    }
                }

                {
                    var strobe = thinker as StrobeFlash;
                    if (strobe != null)
                    {
                        data[save_p++] = (byte)SpecialClass.Strobe;
                        PADSAVEP();
                        WriteThinkerState(data, save_p + 8, strobe.ThinkerState);
                        Write(data, save_p + 12, strobe.sector.Number);
                        Write(data, save_p + 16, strobe.count);
                        Write(data, save_p + 20, strobe.minlight);
                        Write(data, save_p + 24, strobe.maxlight);
                        Write(data, save_p + 28, strobe.darktime);
                        Write(data, save_p + 32, strobe.brighttime);
                        save_p += 36;
                        continue;
                    }
                }

                {
                    var glow = thinker as GlowLight;
                    if (glow != null)
                    {
                        data[save_p++] = (byte)SpecialClass.Glow;
                        PADSAVEP();
                        WriteThinkerState(data, save_p + 8, glow.ThinkerState);
                        Write(data, save_p + 12, glow.sector.Number);
                        Write(data, save_p + 16, glow.minlight);
                        Write(data, save_p + 20, glow.maxlight);
                        Write(data, save_p + 24, glow.direction);
                        save_p += 28;
                        continue;
                    }
                }
            }

            data[save_p++] = (byte)SpecialClass.EndSpecials;
        }



        private static int ArchivePlayer(Player player, byte[] data, int p)
        {
            Write(data, p + 4, (int)player.PlayerState);
            Write(data, p + 16, player.ViewZ.Data);
            Write(data, p + 20, player.ViewHeight.Data);
            Write(data, p + 24, player.DeltaViewHeight.Data);
            Write(data, p + 28, player.Bob.Data);
            Write(data, p + 32, player.Health);
            Write(data, p + 36, player.ArmorPoints);
            Write(data, p + 40, player.ArmorType);
            for (var i = 0; i < (int)PowerType.Count; i++)
            {
                Write(data, p + 44 + 4 * i, player.Powers[i]);
            }
            for (var i = 0; i < (int)PowerType.Count; i++)
            {
                Write(data, p + 68 + 4 * i, player.Cards[i] ? 1 : 0);
            }
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                Write(data, p + 96 + 4 * i, player.Frags[i]);
            }
            Write(data, p + 112, (int)player.ReadyWeapon);
            Write(data, p + 116, (int)player.PendingWeapon);
            for (var i = 0; i < (int)WeaponType.Count; i++)
            {
                Write(data, p + 120 + 4 * i, player.WeaponOwned[i] ? 1 : 0);
            }
            for (var i = 0; i < (int)AmmoType.Count; i++)
            {
                Write(data, p + 156 + 4 * i, player.Ammo[i]);
            }
            for (var i = 0; i < (int)AmmoType.Count; i++)
            {
                Write(data, p + 172 + 4 * i, player.MaxAmmo[i]);
            }
            Write(data, p + 188, player.AttackDown ? 1 : 0);
            Write(data, p + 192, player.UseDown ? 1 : 0);
            Write(data, p + 196, (int)player.Cheats);
            Write(data, p + 200, player.Refire);
            Write(data, p + 204, player.KillCount);
            Write(data, p + 208, player.ItemCount);
            Write(data, p + 212, player.SecretCount);
            Write(data, p + 220, player.DamageCount);
            Write(data, p + 224, player.BonusCount);
            Write(data, p + 232, player.ExtraLight);
            Write(data, p + 236, player.FixedColorMap);
            Write(data, p + 240, player.ColorMap);
            for (var i = 0; i < (int)PlayerSprite.Count; i++)
            {
                if (player.PlayerSprites[i].State == null)
                {
                    Write(data, p + 244 + 16 * i, 0);
                }
                else
                {
                    Write(data, p + 244 + 16 * i, player.PlayerSprites[i].State.Number);
                }
                Write(data, p + 244 + 16 * i + 4, player.PlayerSprites[i].Tics);
                Write(data, p + 244 + 16 * i + 8, player.PlayerSprites[i].Sx.Data);
                Write(data, p + 244 + 16 * i + 12, player.PlayerSprites[i].Sy.Data);
            }
            Write(data, p + 276, player.DidSecret ? 1 : 0);

            return p + 280;
        }

        private static int ArchiveSector(Sector sector, byte[] data, int p)
        {
            Write(data, p, (short)(sector.FloorHeight.ToIntFloor()));
            Write(data, p + 2, (short)(sector.CeilingHeight.ToIntFloor()));
            Write(data, p + 4, (short)sector.FloorFlat);
            Write(data, p + 6, (short)sector.CeilingFlat);
            Write(data, p + 8, (short)sector.LightLevel);
            Write(data, p + 10, (short)sector.Special);
            Write(data, p + 12, (short)sector.Tag);
            return p + 14;
        }

        private static int ArchiveLine(LineDef line, byte[] data, int p)
        {
            Write(data, p, (short)line.Flags);
            Write(data, p + 2, (short)line.Special);
            Write(data, p + 4, (short)line.Tag);
            p += 6;

            if (line.Side0 != null)
            {
                var side = line.Side0;
                Write(data, p, (short)side.TextureOffset.ToIntFloor());
                Write(data, p + 2, (short)side.RowOffset.ToIntFloor());
                Write(data, p + 4, (short)side.TopTexture);
                Write(data, p + 6, (short)side.BottomTexture);
                Write(data, p + 8, (short)side.MiddleTexture);
                p += 10;
            }

            if (line.Side1 != null)
            {
                var side = line.Side1;
                Write(data, p, (short)side.TextureOffset.ToIntFloor());
                Write(data, p + 2, (short)side.RowOffset.ToIntFloor());
                Write(data, p + 4, (short)side.TopTexture);
                Write(data, p + 6, (short)side.BottomTexture);
                Write(data, p + 8, (short)side.MiddleTexture);
                p += 10;
            }

            return p;
        }

        private static void Write(byte[] data, int p, int value)
        {
            data[p] = (byte)value;
            data[p + 1] = (byte)(value >> 8);
            data[p + 2] = (byte)(value >> 16);
            data[p + 3] = (byte)(value >> 24);
        }

        private static void Write(byte[] data, int p, uint value)
        {
            data[p] = (byte)value;
            data[p + 1] = (byte)(value >> 8);
            data[p + 2] = (byte)(value >> 16);
            data[p + 3] = (byte)(value >> 24);
        }

        private static void Write(byte[] data, int p, short value)
        {
            data[p] = (byte)value;
            data[p + 1] = (byte)(value >> 8);
        }

        private static void WriteThinkerState(byte[] data, int p, ThinkerState state)
        {
            switch (state)
            {
                case ThinkerState.InStasis:
                    Write(data, p, 0);
                    break;
                default:
                    Write(data, p, 1);
                    break;
            }
        }
    }
}
