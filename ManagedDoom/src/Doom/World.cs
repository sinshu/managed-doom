using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagedDoom
{
    public class World
    {
        private GameOptions gameOptions;

        private Map map;
        private DoomRandom random;
        private Thinker thinkerCap;

        private int totalKills = 0;
        private int totalItems = 0;

        private Fixed playerX;
        private Fixed playerY;
        private Fixed playerZ;
        private Angle playerViewAngle;

        public World(Resources resorces, string mapName, GameOptions options)
        {
            gameOptions = options;

            map = new Map(resorces, mapName);

            random = new DoomRandom();

            thinkerCap = new Thinker(this);
            thinkerCap.Prev = thinkerCap.Next = thinkerCap;

            LoadThings();

            var playerThing = map.Things.First(t => (int)t.Type == 1);
            playerX = playerThing.X;
            playerY = playerThing.Y;
            playerZ = Geometry.PointInSubsector(playerX, playerY, map).Sector.FloorHeight;
            playerViewAngle = Angle.FromDegree(playerThing.Angle);

            InitPathTraverse();
        }

        private void LoadThings()
        {
            totalKills = 0;
            totalItems = 0;

            for (var i = 0; i < map.Things.Length; i++)
            {
                var mt = map.Things[i];

                var spawn = true;

                // Do not spawn cool, new monsters if !commercial
                if (gameOptions.GameMode != GameMode.Commercial)
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
            if (!gameOptions.NetGame && ((int)mthing.Flags & 16) != 0)
            {
                return;
            }

            int bit;
            if (gameOptions.GameSkill == Skill.Baby)
            {
                bit = 1;
            }
            else if (gameOptions.GameSkill == Skill.Nightmare)
            {
                bit = 4;
            }
            else
            {
                bit = 1 << ((int)gameOptions.GameSkill - 1);
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
            if (gameOptions.Deathmatch
                && (Info.MobjInfos[i].Flags & MobjFlags.NotDeathmatch) != 0)
            {
                return;
            }

            // don't spawn any monsters if -nomonsters
            if (gameOptions.NoMonsters
                && (i == (int)MobjType.Skull
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
                totalKills++;
            }

            if ((mobj.Flags & MobjFlags.CountItem) != 0)
            {
                totalItems++;
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



            /*
            var distance = Fixed.FromInt(1024);
            var x1 = playerX;
            var y1 = playerY;
            var x2 = x1 + distance * Trig.Cos(playerViewAngle);
            var y2 = y1 + distance * Trig.Sin(playerViewAngle);
            var flags = PathTraverseFlags.AddLines | PathTraverseFlags.AddThings;

            PathTraverse(x1, y1, x2, y2, flags, ic =>
                {
                    if (ic.Line != null)
                    {
                        ic.Line.Side0.RowOffset += Fixed.One;
                    }

                    if (ic.Thing != null)
                    {
                        SetMobjState(ic.Thing, ic.Thing.Info.PainState);
                    }

                    return true;
                });
            */
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

            if (gameOptions.GameSkill != Skill.Nightmare)
            {
                mobj.ReactionTime = info.ReactionTime;
            }

            mobj.LastLook = random.Next() % Player.MaxPlayerCount;

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











        private Intercept[] intercepts;
        private int intercept_p;
        private DivLine trace;
        private bool earlyOut;
        private PathTraverseFlags ptflags;

        private int validCount;

        private void InitPathTraverse()
        {
            intercepts = new Intercept[256];
            for (var i = 0; i < intercepts.Length; i++)
            {
                intercepts[i] = new Intercept();
            }

            trace = new DivLine();

            validCount = 0;
        }

        private bool PIT_AddLineIntercepts(LineDef ld)
        {
            int s1;
            int s2;

            // avoid precision problems with two routines
            if (trace.Dx > Fixed.FromInt(16)
             || trace.Dy > Fixed.FromInt(16)
             || trace.Dx < -Fixed.FromInt(16)
             || trace.Dy < -Fixed.FromInt(16))
            {
                s1 = Geometry.PointOnDivLineSide(ld.Vertex1.X, ld.Vertex1.Y, trace);
                s2 = Geometry.PointOnDivLineSide(ld.Vertex2.X, ld.Vertex2.Y, trace);
            }
            else
            {
                s1 = Geometry.PointOnLineSide(trace.X, trace.Y, ld);
                s2 = Geometry.PointOnLineSide(trace.X + trace.Dx, trace.Y + trace.Dy, ld);
            }

            if (s1 == s2)
            {
                // line isn't crossed
                return true;
            }

            // hit the line
            var dl = new DivLine();
            dl.MakeFrom(ld);
            var frac = P_InterceptVector(trace, dl);

            if (frac < Fixed.Zero)
            {
                // behind source
                return true;
            }

            // try to early out the check
            if (earlyOut && frac < Fixed.One && ld.BackSector == null)
            {
                return false;   // stop checking
            }

            intercepts[intercept_p].Frac = frac;
            intercepts[intercept_p].Line = ld;
            intercepts[intercept_p].Thing = null;
            intercept_p++;

            return true;	// continue
        }

        private bool PIT_AddThingIntercepts(Mobj thing)
        {
            var tracepositive = (trace.Dx.Data ^ trace.Dy.Data) > 0;

            Fixed x1;
            Fixed y1;
            Fixed x2;
            Fixed y2;

            // check a corner to corner crossection for hit
            if (tracepositive)
            {
                x1 = thing.X - thing.Radius;
                y1 = thing.Y + thing.Radius;

                x2 = thing.X + thing.Radius;
                y2 = thing.Y - thing.Radius;
            }
            else
            {
                x1 = thing.X - thing.Radius;
                y1 = thing.Y - thing.Radius;

                x2 = thing.X + thing.Radius;
                y2 = thing.Y + thing.Radius;
            }

            var s1 = Geometry.PointOnDivLineSide(x1, y1, trace);
            var s2 = Geometry.PointOnDivLineSide(x2, y2, trace);

            if (s1 == s2)
            {
                // line isn't crossed
                return true;
            }

            var dl = new DivLine();
            dl.X = x1;
            dl.Y = y1;
            dl.Dx = x2 - x1;
            dl.Dy = y2 - y1;

            var frac = P_InterceptVector(trace, dl);

            if (frac < Fixed.Zero)
            {
                // behind source
                return true;
            }

            intercepts[intercept_p].Frac = frac;
            intercepts[intercept_p].Line = null;
            intercepts[intercept_p].Thing = thing;
            intercept_p++;

            return true;		// keep going
        }

        private Fixed P_InterceptVector(DivLine v2, DivLine v1)
        {
            var den = new Fixed(v1.Dy.Data >> 8) * v2.Dx - new Fixed(v1.Dx.Data >> 8) * v2.Dy;

            if (den == Fixed.Zero)
            {
                return Fixed.Zero;
            }

            var num =
            new Fixed((v1.X - v2.X).Data >> 8) * v1.Dy
            + new Fixed((v2.Y - v1.Y).Data >> 8) * v1.Dx;

            var frac = num / den;

            return frac;
        }

        private bool P_TraverseIntercepts(Func<Intercept, bool> func, Fixed maxfrac)
        {
            var count = intercept_p;

            Intercept ic = null;

            while (count-- > 0)
            {
                var dist = Fixed.MaxValue;
                for (var scan = 0; scan < intercept_p; scan++)
                {
                    if (intercepts[scan].Frac < dist)
                    {
                        dist = intercepts[scan].Frac;
                        ic = intercepts[scan];
                    }
                }

                if (dist > maxfrac)
                {
                    // checked everything in range
                    return true;
                }

                if (!func(ic))
                {
                    // don't bother going farther
                    return false;
                }

                ic.Frac = Fixed.MaxValue;
            }

            // everything was traversed
            return true;
        }

        public bool PathTraverse(Fixed x1, Fixed y1, Fixed x2, Fixed y2, PathTraverseFlags flags, Func<Intercept, bool> trav)
        {
            earlyOut = (flags & PathTraverseFlags.EarlyOut) != 0;

            validCount++;

            intercept_p = 0;

            if (((x1 - map.BlockMap.OriginX).Data & (BlockMap.MapBlockSize.Data - 1)) == 0)
            {
                x1 += Fixed.One; // don't side exactly on a line
            }

            if (((y1 - map.BlockMap.OriginY).Data & (BlockMap.MapBlockSize.Data - 1)) == 0)
            {
                y1 += Fixed.One; // don't side exactly on a line
            }

            trace.X = x1;
            trace.Y = y1;
            trace.Dx = x2 - x1;
            trace.Dy = y2 - y1;

            x1 -= map.BlockMap.OriginX;
            y1 -= map.BlockMap.OriginY;
            
            var xt1 = x1.Data >> BlockMap.MapBlockShift;
            var yt1 = y1.Data >> BlockMap.MapBlockShift;

            x2 -= map.BlockMap.OriginX;
            y2 -= map.BlockMap.OriginY;

            var xt2 = x2.Data >> BlockMap.MapBlockShift;
            var yt2 = y2.Data >> BlockMap.MapBlockShift;

            Fixed xstep;
            Fixed ystep;

            Fixed partial;

            int mapxstep;
            int mapystep;

            if (xt2 > xt1)
            {
                mapxstep = 1;
                partial = new Fixed(Fixed.FracUnit - ((x1.Data >> BlockMap.MapBToFrac) & (Fixed.FracUnit - 1)));
                ystep = (y2 - y1) / Fixed.Abs(x2 - x1);
            }
            else if (xt2 < xt1)
            {
                mapxstep = -1;
                partial = new Fixed((x1.Data >> BlockMap.MapBToFrac) & (Fixed.FracUnit - 1));
                ystep = (y2 - y1) / Fixed.Abs(x2 - x1);
            }
            else
            {
                mapxstep = 0;
                partial = Fixed.One;
                ystep = Fixed.FromInt(256);
            }

            var yintercept = new Fixed(y1.Data >> BlockMap.MapBToFrac) + (partial * ystep);


            if (yt2 > yt1)
            {
                mapystep = 1;
                partial = new Fixed(Fixed.FracUnit - ((y1.Data >> BlockMap.MapBToFrac) & (Fixed.FracUnit - 1)));
                xstep = (x2 - x1) / Fixed.Abs(y2 - y1);
            }
            else if (yt2 < yt1)
            {
                mapystep = -1;
                partial = new Fixed((y1.Data >> BlockMap.MapBToFrac) & (Fixed.FracUnit - 1));
                xstep = (x2 - x1) / Fixed.Abs(y2 - y1);
            }
            else
            {
                mapystep = 0;
                partial = Fixed.One;
                xstep = Fixed.FromInt(256);
            }

            var xintercept = new Fixed(x1.Data >> BlockMap.MapBToFrac) + (partial * xstep);

            // Step through map blocks.
            // Count is present to prevent a round off error
            // from skipping the break.
            var mapx = xt1;
            var mapy = yt1;

            for (var count = 0; count < 64; count++)
            {
                if ((flags & PathTraverseFlags.AddLines) != 0)
                {
                    if (!map.BlockMap.EnumBlockLines(mapx, mapy, PIT_AddLineIntercepts, validCount))
                    {
                        return false;   // early out
                    }
                }

                if ((flags & PathTraverseFlags.AddThings) != 0)
                {
                    if (!map.BlockMap.EnumBlockThings(mapx, mapy, PIT_AddThingIntercepts))
                    {
                        return false;   // early out
                    }
                }

                if (mapx == xt2 && mapy == yt2)
                {
                    break;
                }

                if ((yintercept.Data >> Fixed.FracBits) == mapy)
                {
                    yintercept += ystep;
                    mapx += mapxstep;
                }
                else if ((xintercept.Data >> Fixed.FracBits) == mapx)
                {
                    xintercept += xstep;
                    mapy += mapystep;
                }

            }

            // go through the sorted list
            return P_TraverseIntercepts(trav, Fixed.One);
        }















        public Map Map => map;
        public Fixed ViewX => playerX;
        public Fixed ViewY => playerY;
        public Fixed ViewZ => playerZ + Fixed.FromInt(41);
        public Angle ViewAngle => playerViewAngle;
    }
}
