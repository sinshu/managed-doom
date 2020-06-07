using System;
using System.Collections.Generic;

namespace ManagedDoom
{
    public sealed class ThingAllocation
    {
        private World world;

        private MapThing[] playerStarts;
        private List<MapThing> deathmatchStarts;


        public ThingAllocation(World world)
        {
            this.world = world;

            playerStarts = new MapThing[Player.MaxPlayerCount];
            deathmatchStarts = new List<MapThing>();
        }


        public void SpawnMapThing(MapThing mt)
        {
            // Count deathmatch start positions.
            if (mt.Type == 11)
            {
                if (deathmatchStarts.Count < 10)
                {
                    deathmatchStarts.Add(mt);
                }

                return;
            }

            // Check for players specially.
            if (mt.Type <= 4)
            {
                var playerNumber = mt.Type - 1;

                // This check is neccesary in Plutonia MAP12,
                // which contains an unknown thing with type 0.
                if (playerNumber < 0)
                {
                    return;
                }

                // Save spots for respawning in network games.
                playerStarts[playerNumber] = mt;

                if (world.Options.Deathmatch == 0)
                {
                    SpawnPlayer(mt);
                }

                return;
            }

            if (mt.Type == 11 || mt.Type <= 4)
            {
                return;
            }

            // Check for apropriate skill level.
            if (!world.Options.NetGame && ((int)mt.Flags & 16) != 0)
            {
                return;
            }

            int bit;
            if (world.Options.Skill == GameSkill.Baby)
            {
                bit = 1;
            }
            else if (world.Options.Skill == GameSkill.Nightmare)
            {
                bit = 4;
            }
            else
            {
                bit = 1 << ((int)world.Options.Skill - 1);
            }

            if (((int)mt.Flags & bit) == 0)
            {
                return;
            }

            // Find which type to spawn.
            int i;
            for (i = 0; i < DoomInfo.MobjInfos.Length; i++)
            {
                if (mt.Type == DoomInfo.MobjInfos[i].DoomEdNum)
                {
                    break;
                }
            }

            if (i == DoomInfo.MobjInfos.Length)
            {
                throw new Exception("P_SpawnMapThing: Unknown type!");
            }

            // Don't spawn keycards and players in deathmatch.
            if (world.Options.Deathmatch != 0 &&
                (DoomInfo.MobjInfos[i].Flags & MobjFlags.NotDeathmatch) != 0)
            {
                return;
            }

            // Don't spawn any monsters if -nomonsters.
            if (world.Options.NoMonsters && (i == (int)MobjType.Skull ||
                (DoomInfo.MobjInfos[i].Flags & MobjFlags.CountKill) != 0))
            {
                return;
            }

            // Spawn it.
            Fixed x = mt.X;
            Fixed y = mt.Y;
            Fixed z;
            if ((DoomInfo.MobjInfos[i].Flags & MobjFlags.SpawnCeiling) != 0)
            {
                z = Mobj.OnCeilingZ;
            }
            else
            {
                z = Mobj.OnFloorZ;
            }

            var mobj = SpawnMobj(x, y, z, (MobjType)i);

            mobj.SpawnPoint = mt;

            if (mobj.Tics > 0)
            {
                mobj.Tics = 1 + (world.Random.Next() % mobj.Tics);
            }

            if ((mobj.Flags & MobjFlags.CountKill) != 0)
            {
                world.totalKills++;
            }

            if ((mobj.Flags & MobjFlags.CountItem) != 0)
            {
                world.totalItems++;
            }

            mobj.Angle = mt.Angle;

            if ((mt.Flags & ThingFlags.Ambush) != 0)
            {
                mobj.Flags |= MobjFlags.Ambush;
            }
        }


        public void SpawnPlayer(MapThing mt)
        {
            var playerNumber = mt.Type - 1;

            // Not playing?
            if (!world.Players[playerNumber].InGame)
            {
                return;
            }

            var player = world.Players[playerNumber];

            if (player.PlayerState == PlayerState.Reborn)
            {
                world.Players[playerNumber].Reborn();
            }

            var x = mt.X;
            var y = mt.Y;
            var z = Mobj.OnFloorZ;
            var mobj = SpawnMobj(x, y, z, MobjType.Player);

            // Set color translations for player sprites.
            if (playerNumber >= 1)
            {
                mobj.Flags |= (MobjFlags)((mt.Type - 1) << (int)MobjFlags.TransShift);
            }

            mobj.Angle = mt.Angle;
            mobj.Player = player;
            mobj.Health = player.Health;

            player.Mobj = mobj;
            player.PlayerState = PlayerState.Live;
            player.Refire = 0;
            player.Message = null;
            player.MessageTime = 0;
            player.DamageCount = 0;
            player.BonusCount = 0;
            player.ExtraLight = 0;
            player.FixedColorMap = 0;
            player.ViewHeight = Player.VIEWHEIGHT;

            // Setup gun psprite.
            world.PlayerBehavior.SetupPsprites(player);

            // Give all cards in death match mode.
            if (world.Options.Deathmatch != 0)
            {
                for (var i = 0; i < (int)CardType.Count; i++)
                {
                    player.Cards[i] = true;
                }
            }

            if (mt.Type - 1 == world.Options.ConsolePlayer)
            {
                // Wake up the status bar.
                world.StatusBar.Reset();

                // Wake up the heads up text.
                // HU_Start();
            }
        }


        public Mobj SpawnMobj(Fixed x, Fixed y, Fixed z, MobjType type)
        {
            var mobj = ThinkerPool.RentMobj(world);

            var info = DoomInfo.MobjInfos[(int)type];

            mobj.Type = type;
            mobj.Info = info;
            mobj.X = x;
            mobj.Y = y;
            mobj.Radius = info.Radius;
            mobj.Height = info.Height;
            mobj.Flags = info.Flags;
            mobj.Health = info.SpawnHealth;

            if (world.Options.Skill != GameSkill.Nightmare)
            {
                mobj.ReactionTime = info.ReactionTime;
            }

            mobj.LastLook = world.Random.Next() % Player.MaxPlayerCount;

            // Do not set the state with P_SetMobjState,
            // because action routines can not be called yet.
            var st = DoomInfo.States[(int)info.SpawnState];

            mobj.State = st;
            mobj.Tics = st.Tics;
            mobj.Sprite = st.Sprite;
            mobj.Frame = st.Frame;

            // Set subsector and/or block links.
            world.ThingMovement.SetThingPosition(mobj);

            mobj.FloorZ = mobj.Subsector.Sector.FloorHeight;
            mobj.CeilingZ = mobj.Subsector.Sector.CeilingHeight;

            if (z == Mobj.OnFloorZ)
            {
                mobj.Z = mobj.FloorZ;
            }
            else if (z == Mobj.OnCeilingZ)
            {
                mobj.Z = mobj.CeilingZ - mobj.Info.Height;
            }
            else
            {
                mobj.Z = z;
            }

            world.Thinkers.Add(mobj);

            return mobj;
        }


        public void RemoveMobj(Mobj mobj)
        {
            var tm = world.ThingMovement;

            if ((mobj.Flags & MobjFlags.Special) != 0 &&
                (mobj.Flags & MobjFlags.Dropped) == 0 &&
                (mobj.Type != MobjType.Inv) &&
                (mobj.Type != MobjType.Ins))
            {
                //itemrespawnque[iquehead] = mobj->spawnpoint;
                //itemrespawntime[iquehead] = leveltime;
                //iquehead = (iquehead + 1) & (ITEMQUESIZE - 1);

                // lose one off the end?
                //if (iquehead == iquetail)
                //    iquetail = (iquetail + 1) & (ITEMQUESIZE - 1);
            }

            // Unlink from sector and block lists.
            tm.UnsetThingPosition(mobj);

            // Stop any playing sound.
            world.StopSound(mobj);

            // Free block.
            world.Thinkers.Remove(mobj);
        }


        public void CheckMissileSpawn(Mobj missile)
        {
            missile.Tics -= world.Random.Next() & 3;
            if (missile.Tics < 1)
            {
                missile.Tics = 1;
            }

            // Move a little forward so an angle can be computed if it immediately explodes.
            missile.X += (missile.MomX >> 1);
            missile.Y += (missile.MomY >> 1);
            missile.Z += (missile.MomZ >> 1);

            if (!world.ThingMovement.TryMove(missile, missile.X, missile.Y))
            {
                world.ThingInteraction.ExplodeMissile(missile);
            }
        }


        public Mobj SpawnMissile(Mobj source, Mobj dest, MobjType type)
        {
            var missile = SpawnMobj(
                source.X,
                source.Y,
                source.Z + Fixed.FromInt(32), type);

            if (missile.Info.SeeSound != 0)
            {
                world.StartSound(missile, missile.Info.SeeSound);
            }

            // Where it came from?
            missile.Target = source;

            var angle = Geometry.PointToAngle(
                source.X, source.Y,
                dest.X, dest.Y);

            // Fuzzy player.
            if ((dest.Flags & MobjFlags.Shadow) != 0)
            {
                var random = world.Random;
                angle += new Angle((random.Next() - random.Next()) << 20);
            }

            var speed = GetMissileSpeed(missile.Type);

            missile.Angle = angle;
            missile.MomX = new Fixed(speed) * Trig.Cos(angle);
            missile.MomY = new Fixed(speed) * Trig.Sin(angle);

            var dist = Geometry.AproxDistance(
                dest.X - source.X,
                dest.Y - source.Y);

            var num = (dest.Z - source.Z).Data;
            var den = (dist / speed).Data;
            if (den < 1)
            {
                den = 1;
            }

            missile.MomZ = new Fixed(num / den);

            CheckMissileSpawn(missile);

            return missile;
        }


        private int GetMissileSpeed(MobjType type)
        {
            if (world.Options.FastMonsters || world.Options.Skill == GameSkill.Nightmare)
            {
                switch (type)
                {
                    case MobjType.Bruisershot:
                    case MobjType.Headshot:
                    case MobjType.Troopshot:
                        return 20 * Fixed.FracUnit;
                    default:
                        return DoomInfo.MobjInfos[(int)type].Speed;
                }
            }
            else
            {
                return DoomInfo.MobjInfos[(int)type].Speed;
            }
        }


        public void SpawnPlayerMissile(Mobj source, MobjType type)
        {
            var hs = world.Hitscan;

            // See which target is to be aimed at.
            var angle = source.Angle;
            var slope = hs.AimLineAttack(source, angle, Fixed.FromInt(16 * 64));

            if (hs.LineTarget == null)
            {
                angle += new Angle(1 << 26);
                slope = hs.AimLineAttack(source, angle, Fixed.FromInt(16 * 64));

                if (hs.LineTarget == null)
                {
                    angle -= new Angle(2 << 26);
                    slope = hs.AimLineAttack(source, angle, Fixed.FromInt(16 * 64));
                }

                if (hs.LineTarget == null)
                {
                    angle = source.Angle;
                    slope = Fixed.Zero;
                }
            }

            var x = source.X;
            var y = source.Y;
            var z = source.Z + Fixed.FromInt(32);

            var missile = SpawnMobj(x, y, z, type);

            if (missile.Info.SeeSound != 0)
            {
                world.StartSound(missile, missile.Info.SeeSound);
            }

            missile.Target = source;
            missile.Angle = angle;
            missile.MomX = new Fixed(missile.Info.Speed) * Trig.Cos(angle);
            missile.MomY = new Fixed(missile.Info.Speed) * Trig.Sin(angle);
            missile.MomZ = new Fixed(missile.Info.Speed) * slope;

            CheckMissileSpawn(missile);
        }

        public IReadOnlyList<MapThing> PlayerStarts => playerStarts;
        public IReadOnlyList<MapThing> DeathmatchStarts => deathmatchStarts;
    }
}
