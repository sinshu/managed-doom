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

        private Vertex player;
        private Angle playerViewAngle;

        public World(TextureLookup textures, FlatLookup flats, Wad wad, string mapName)
        {
            map = new Map(textures, flats, wad, mapName);

            random = new DoomRandom();

            LoadThings();

            var playerThing = map.Things.First(t => (int)t.Type == 1);
            player = new Vertex(playerThing.X, playerThing.Y);
            playerViewAngle = playerThing.Facing;

            var viewZ = GetSector(player.X, player.Y).FloorHeight + Fixed.FromInt(41);
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
            if (Deathmatch && (Info.MobjInfos[i].Flags & MobjFlags.NotDmatch) != 0)
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

            mobj.Angle = new Angle(Angle.Ang45.Data * (mthing.Facing.Data / 45));

            if ((mthing.Flags & ThingFlags.Ambush) != 0)
            {
                mobj.Flags |= MobjFlags.Ambush;
            }
        }

        public void Update(bool up, bool down, bool left, bool right)
        {
            var speed = 8;

            if (up)
            {
                var x = player.X + speed * Trig.Cos(playerViewAngle);
                var y = player.Y + speed * Trig.Sin(playerViewAngle);
                player = new Vertex(x, y);
            }

            if (down)
            {
                var x = player.X - speed * Trig.Cos(playerViewAngle);
                var y = player.Y - speed * Trig.Sin(playerViewAngle);
                player = new Vertex(x, y);
            }

            if (left)
            {
                playerViewAngle += Angle.FromDegree(3);
            }

            if (right)
            {
                playerViewAngle -= Angle.FromDegree(3);
            }
        }

        private Sector GetSector(Fixed x, Fixed y)
        {
            var subsector = Geometry.PointInSubsector(x, y, map.Nodes, map.Subsectors);
            return subsector.Sector;
        }





        public void SetThingPosition(Mobj thing)
        {
            // link into subsector
            var ss = Geometry.PointInSubsector(thing.X, thing.Y, map.Nodes, map.Subsectors);

            thing.Subsector = ss;

            if ((thing.Flags & MobjFlags.NoSector) == 0)
            {
                // invisible things don't go into the sector links
                var sec = ss.Sector;

                thing.SPrev = null;
                thing.SNext = sec.ThingList;

                if (sec.ThingList != null)
                {
                    sec.ThingList.SPrev = thing;
                }

                sec.ThingList = thing;
            }

            // The code below must be implemented later when blockmap related code is done.

            /*
            // link into blockmap
            if (!(thing->flags & MF_NOBLOCKMAP))
            {
                // inert things don't need to be in blockmap		
                blockx = (thing->x - bmaporgx) >> MAPBLOCKSHIFT;
                blocky = (thing->y - bmaporgy) >> MAPBLOCKSHIFT;

                if (blockx >= 0
                    && blockx < bmapwidth
                    && blocky >= 0
                    && blocky < bmapheight)
                {
                    link = &blocklinks[blocky * bmapwidth + blockx];
                    thing->bprev = NULL;
                    thing->bnext = *link;
                    if (*link)
                        (*link)->bprev = thing;

                    *link = thing;
                }
                else
                {
                    // thing is off the map
                    thing->bnext = thing->bprev = NULL;
                }
            }
            */
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

            //P_AddThinker(&mobj->thinker);

            return mobj;
        }




        public Map Map => map;
        public Fixed ViewX => player.X;
        public Fixed ViewY => player.Y;
        public Fixed ViewZ => GetSector(player.X, player.Y).FloorHeight + Fixed.FromInt(41);
        public Angle ViewAngle => playerViewAngle;
    }
}
