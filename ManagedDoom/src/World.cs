using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagedDoom
{
    public class World
    {
        private Skill GameSkill = Skill.Hard;
        private GameMode GameMode = GameMode.Commercial;
        private bool NetGame = false;
        private bool Deathmatch = false;
        private bool NoMonsters = false;

        private int TotalKills = 0;
        private int TotalItems = 0;

        private Map map;

        private DoomRandom random;

        private Thinker thinkerCap;

        private Fixed playerX;
        private Fixed playerY;
        private Fixed playerZ;
        private Angle playerViewAngle;

        public World(TextureLookup textures, FlatLookup flats, Wad wad, string mapName)
        {
            map = new Map(textures, flats, wad, mapName);

            random = new DoomRandom();

            thinkerCap = new Thinker(this);
            thinkerCap.Prev = thinkerCap.Next = thinkerCap;

            LoadThings();

            var playerThing = map.Things.First(t => (int)t.Type == 1);
            playerX = playerThing.X;
            playerY = playerThing.Y;
            playerZ = Geometry.PointInSubsector(playerX, playerY, map).Sector.FloorHeight;
            playerViewAngle = Angle.FromDegree(playerThing.Angle);
        }

        private void LoadThings()
        {
            for (var i = 0; i < map.Things.Length; i++)
            {
                var mt = map.Things[i];

                var spawn = true;

                // Do not spawn cool, new monsters if !commercial
                if (GameMode != GameMode.Commercial)
                {
                    switch (mt.Type)
                    {
                        case 68:    // Arachnotron
                        case 64:    // Archvile
                        case 88:    // Boss Brain
                        case 89:    // Boss Shooter
                        case 69:    // Hell Knight
                        case 67:    // Mancubus
                        case 71:    // Pain Elemental
                        case 65:    // Former Human Commando
                        case 66:    // Revenant
                        case 84:    // Wolf SS
                            spawn = false;
                            break;
                    }
                }

                if (spawn == false)
                {
                    break;
                }

                SpawnMapThing(mt);
            }
        }

        private void SpawnMapThing(Thing mthing)
        {
            /*
            // count deathmatch start positions
            if (mthing->type == 11)
            {
                if (deathmatch_p < &deathmatchstarts[10])
                {
                    memcpy(deathmatch_p, mthing, sizeof(*mthing));
                    deathmatch_p++;
                }
                return;
            }
            */

            /*
            // check for players specially
            if (mthing->type <= 4)
            {
                // save spots for respawning in network games
                playerstarts[mthing->type - 1] = *mthing;
                if (!deathmatch)
                    P_SpawnPlayer(mthing);

                return;
            }
            */

            // The code below must be removed later
            // when the player related code above is correctly implemented.
            if (mthing.Type == 11 || mthing.Type <= 4)
            {
                return;
            }

            // check for apropriate skill level
            if (!NetGame && ((int)mthing.Flags & 16) != 0)
            {
                return;
            }

            int bit;
            if (GameSkill == Skill.Baby)
            {
                bit = 1;
            }
            else if (GameSkill == Skill.Nightmare)
            {
                bit = 4;
            }
            else
            {
                bit = 1 << ((int)GameSkill - 1);
            }

            if (((int)mthing.Flags & bit) == 0)
            {
                return;
            }

            int i;
            // find which type to spawn
            for (i = 0; i < Info.MobjInfos.Length; i++)
            {
                if (mthing.Type == Info.MobjInfos[i].DoomEdNum)
                {
                    break;
                }
            }

            if (i == Info.MobjInfos.Length)
            {
                throw new Exception("P_SpawnMapThing: Unknown type!");
            }

            // don't spawn keycards and players in deathmatch
            if (Deathmatch && (Info.MobjInfos[i].Flags & MobjFlags.NotDeathmatch) != 0)
            {
                return;
            }

            // don't spawn any monsters if -nomonsters
            if (NoMonsters && (i == (int)MobjType.Skull
                || (Info.MobjInfos[i].Flags & MobjFlags.CountKill) != 0))
            {
                return;
            }

            // spawn it
            Fixed x = mthing.X;
            Fixed y = mthing.Y;
            Fixed z;
            if ((Info.MobjInfos[i].Flags & MobjFlags.SpawnCeiling) != 0)
            {
                z = Mobj.OnCeilingZ;
            }
            else
            {
                z = Mobj.OnFloorZ;
            }

            var mobj = SpawnMobj(x, y, z, (MobjType)i);

            mobj.SpawnPoint = mthing;

            if (mobj.Tics > 0)
            {
                mobj.Tics = 1 + (random.Next() % mobj.Tics);
            }

            if ((mobj.Flags & MobjFlags.CountKill) != 0)
            {
                TotalKills++;
            }

            if ((mobj.Flags & MobjFlags.CountItem) != 0)
            {
                TotalItems++;
            }

            // mobj->angle = ANG45 * (mthing->angle/45);
            mobj.Angle = new Angle(Angle.Ang45.Data * (uint)(mthing.Angle / 45));

            if ((mthing.Flags & ThingFlags.Ambush) != 0)
            {
                mobj.Flags |= MobjFlags.Ambush;
            }
        }

        public void Update(bool up, bool down, bool left, bool right)
        {
            RunThinkers();

            var speed = 8.0;

            if (up)
            {
                playerX += Fixed.FromDouble(speed) * Trig.Cos(playerViewAngle);
                playerY += Fixed.FromDouble(speed) * Trig.Sin(playerViewAngle);
            }

            if (down)
            {
                playerX -= Fixed.FromDouble(speed) * Trig.Cos(playerViewAngle);
                playerY -= Fixed.FromDouble(speed) * Trig.Sin(playerViewAngle);
            }

            if (left)
            {
                playerViewAngle += Angle.FromDegree(speed / 2);
            }

            if (right)
            {
                playerViewAngle -= Angle.FromDegree(speed / 2);
            }

            var floor = Geometry.PointInSubsector(playerX, playerY, map).Sector.FloorHeight;
            var dz = floor - playerZ;
            if (dz < Fixed.Zero)
            {
                dz = new Fixed(8192) * dz;
            }
            else
            {
                dz = new Fixed(32768) * dz;
            }
            playerZ += dz;
        }





        public void SetThingPosition(Mobj thing)
        {
            var ss = Geometry.PointInSubsector(thing.X, thing.Y, map);

            thing.Subsector = ss;

            // invisible things don't go into the sector links
            if ((thing.Flags & MobjFlags.NoSector) == 0)
            {
                // link into subsector    

                var sec = ss.Sector;

                thing.SPrev = null;
                thing.SNext = sec.ThingList;

                if (sec.ThingList != null)
                {
                    sec.ThingList.SPrev = thing;
                }

                sec.ThingList = thing;
            }

            // inert things don't need to be in blockmap
            if ((thing.Flags & MobjFlags.NoBlockMap) == 0)
            {
                // link into blockmap

                var index = map.BlockMap.GetIndex(thing.X, thing.Y);

                if (index != -1)
                {
                    var link = map.BlockMap.ThingLists[index];

                    thing.BPrev = null;
                    thing.BNext = link;

                    if (link != null)
                    {
                        link.BPrev = thing;
                    }

                    map.BlockMap.ThingLists[index] = thing;
                }
                else
                {
                    // thing is off the map
                    thing.BNext = null;
                    thing.BPrev = null;
                }
            }
        }

        public void UnsetThingPosition(Mobj thing)
        {
            // invisible things don't go into the sector links
            if ((thing.Flags & MobjFlags.NoSector) == 0)
            {
                // unlink from subsector

                if (thing.SNext != null)
                {
                    thing.SNext.SPrev = thing.SPrev;
                }

                if (thing.SPrev != null)
                {
                    thing.SPrev.SNext = thing.SNext;
                }
                else
                {
                    thing.Subsector.Sector.ThingList = thing.SNext;
                }
            }

            // inert things don't need to be in blockmap
            if ((thing.Flags & MobjFlags.NoBlockMap) == 0)
            {
                // unlink from block map

                if (thing.BNext != null)
                {
                    thing.BNext.BPrev = thing.BPrev;
                }

                if (thing.BPrev != null)
                {
                    thing.BPrev.BNext = thing.BNext;
                }
                else
                {
                    var index = map.BlockMap.GetIndex(thing.X, thing.Y);

                    if (index != -1)
                    {
                        map.BlockMap.ThingLists[index] = thing.BNext;
                    }
                }
            }
        }



        public Mobj SpawnMobj(Fixed x, Fixed y, Fixed z, MobjType type)
        {
            var mobj = new Mobj(this);

            var info = Info.MobjInfos[(int)type];

            mobj.Type = type;
            mobj.Info = info;
            mobj.X = x;
            mobj.Y = y;
            mobj.Radius = info.Radius;
            mobj.Height = info.Height;
            mobj.Flags = info.Flags;
            mobj.Health = info.SpawnHealth;

            if (GameSkill != Skill.Nightmare)
            {
                mobj.ReactionTime = info.ReactionTime;
            }

            mobj.LastLook = random.Next() % Player.MaxPlayers;

            // do not set the state with P_SetMobjState,
            // because action routines can not be called yet
            var st = Info.States[(int)info.SpawnState];

            mobj.State = st;
            mobj.Tics = st.Tics;
            mobj.Sprite = st.Sprite;
            mobj.Frame = st.Frame;

            // set subsector and/or block links
            SetThingPosition(mobj);

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

            //mobj->thinker.function.acp1 = (actionf_p1)P_MobjThinker;

            AddThinker(mobj);

            return mobj;
        }


        public void AddThinker(Thinker thinker)
        {
            thinkerCap.Prev.Next = thinker;
            thinker.Next = thinkerCap;
            thinker.Prev = thinkerCap.Prev;
            thinkerCap.Prev = thinker;
        }

        public void RunThinkers()
        {
            Thinker currentthinker;

            currentthinker = thinkerCap.Next;
            while (currentthinker != thinkerCap)
            {
                if (currentthinker.Removed)
                {
                    // time to remove it
                    currentthinker.Next.Prev = currentthinker.Prev;
                    currentthinker.Prev.Next = currentthinker.Next;
                    //Z_Free(currentthinker);
                }
                else
                {
                    /*
                    if (currentthinker->function.acp1)
                    {
                        currentthinker->function.acp1(currentthinker);
                    }
                    */
                    currentthinker.Run();
                }
                currentthinker = currentthinker.Next;
            }
        }

        public bool SetMobjState(Mobj mobj, State state)
        {
            StateDef st;

            do
            {
                if (state == State.Null)
                {
                    mobj.State = Info.States[(int)State.Null];
                    //P_RemoveMobj(mobj);
                    return false;
                }

                st = Info.States[(int)state];
                mobj.State = st;
                mobj.Tics = st.Tics;
                mobj.Sprite = st.Sprite;
                mobj.Frame = st.Frame;

                // Modified handling.
                // Call action functions when the state is set
                if (st.MobjAction != null)
                {
                    st.MobjAction(mobj);
                }

                state = st.Next;
            }
            while (mobj.Tics == 0);

            return true;
        }


        public Map Map => map;
        public Fixed ViewX => playerX;
        public Fixed ViewY => playerY;
        public Fixed ViewZ => playerZ + Fixed.FromInt(41);
        public Angle ViewAngle => playerViewAngle;
    }
}
