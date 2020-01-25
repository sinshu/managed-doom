using System;

namespace ManagedDoom
{
    public sealed partial class World
    {
        private Fixed[] tmbbox;
        private Mobj tmthing;
        private MobjFlags tmflags;
        private Fixed tmx;
        private Fixed tmy;

        private bool floatok;

        private Fixed tmfloorz;
        private Fixed tmceilingz;
        private Fixed tmdropoffz;

        private LineDef ceilingline;

        private LineDef[] spechit;
        private int numspechit;

        private void InitThingMovement()
        {
            tmbbox = new Fixed[4];

            spechit = new LineDef[16];
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



        //
        // P_LineOpening
        // Sets opentop and openbottom to the window
        // through a two sided line.
        // OPTIMIZE: keep this precalculated
        //
        private Fixed openTop;
        private Fixed openBottom;
        private Fixed openRange;
        private Fixed lowFloor;

        private void LineOpening(LineDef linedef)
        {
            if (linedef.Side1 == null)
            {
                // single sided line
                openRange = Fixed.Zero;
                return;
            }

            var front = linedef.FrontSector;
            var back = linedef.BackSector;

            if (front.CeilingHeight < back.CeilingHeight)
            {
                openTop = front.CeilingHeight;
            }
            else
            {
                openTop = back.CeilingHeight;
            }

            if (front.FloorHeight > back.FloorHeight)
            {
                openBottom = front.FloorHeight;
                lowFloor = back.FloorHeight;
            }
            else
            {
                openBottom = back.FloorHeight;
                lowFloor = front.FloorHeight;
            }

            openRange = openTop - openBottom;
        }

        //
        // PIT_CheckLine
        // Adjusts tmfloorz and tmceilingz as lines are contacted
        //
        private bool PIT_CheckLine(LineDef ld)
        {
            if (tmbbox[Box.Right] <= ld.BboxLeft
                || tmbbox[Box.Left] >= ld.BboxRight
                || tmbbox[Box.Top] <= ld.BboxBottom
                || tmbbox[Box.Bottom] >= ld.BboxTop)
            {
                return true;
            }

            if (Geometry.BoxOnLineSide(tmbbox, ld) != -1)
            {
                return true;
            }

            // A line has been hit

            // The moving thing's destination position will cross
            // the given line.
            // If this should not be allowed, return false.
            // If the line is special, keep track of it
            // to process later if the move is proven ok.
            // NOTE: specials are NOT sorted by order,
            // so two special lines that are only 8 pixels apart
            // could be crossed in either order.

            if (ld.BackSector == null)
            {
                // one sided line
                return false;
            }

            if ((tmthing.Flags & MobjFlags.Missile) == 0)
            {
                if ((ld.Flags & LineFlags.Blocking) != 0)
                {
                    // explicitly blocking everything
                    return false;
                }

                if (tmthing.Player == null && (ld.Flags & LineFlags.BlockMonsters) != 0)
                {
                    // block monsters only
                    return false;
                }
            }

            // set openrange, opentop, openbottom
            LineOpening(ld);

            // adjust floor / ceiling heights
            if (openTop < tmceilingz)
            {
                tmceilingz = openTop;
                ceilingline = ld;
            }

            if (openBottom > tmfloorz)
            {
                tmfloorz = openBottom;
            }

            if (lowFloor < tmdropoffz)
            {
                tmdropoffz = lowFloor;
            }

            // if contacted a special line, add it to the list
            if (ld.Special != 0)
            {
                spechit[numspechit] = ld;
                numspechit++;
            }

            return true;
        }

        //
        // PIT_CheckThing
        //
        private bool PIT_CheckThing(Mobj thing)
        {
            if ((thing.Flags & (MobjFlags.Solid | MobjFlags.Special | MobjFlags.Shootable)) == 0)
            {
                return true;
            }

            var blockdist = thing.Radius + tmthing.Radius;

            if (Fixed.Abs(thing.X - tmx) >= blockdist
                || Fixed.Abs(thing.Y - tmy) >= blockdist)
            {
                // didn't hit it
                return true;
            }

            // don't clip against self
            if (thing == tmthing)
            {
                return true;
            }

            // check for skulls slamming into things
            if ((tmthing.Flags & MobjFlags.SkullFly) != 0)
            {
                var damage = ((random.Next() % 8) + 1) * tmthing.Info.Damage;

                //P_DamageMobj(thing, tmthing, tmthing, damage);

                tmthing.Flags &= ~MobjFlags.SkullFly;
                tmthing.MomX = tmthing.MomY = tmthing.MomZ = Fixed.Zero;

                SetMobjState(tmthing, tmthing.Info.SpawnState);

                // stop moving
                return false;
            }


            // missiles can hit other things
            if ((tmthing.Flags & MobjFlags.Missile) != 0)
            {
                // see if it went over / under
                if (tmthing.Z > thing.Z + thing.Height)
                {
                    // overhead
                    return true;
                }

                if (tmthing.Z + tmthing.Height < thing.Z)
                {
                    // underneath
                    return true;
                }

                if (tmthing.Target != null
                    && (tmthing.Target.Type == thing.Type
                        || (tmthing.Target.Type == MobjType.Knight && thing.Type == MobjType.Bruiser)
                        || (tmthing.Target.Type == MobjType.Bruiser && thing.Type == MobjType.Knight)))
                {
                    // Don't hit same species as originator.
                    if (thing == tmthing.Target)
                    {
                        return true;
                    }

                    if (thing.Type != MobjType.Player)
                    {
                        // Explode, but do no damage.
                        // Let players missile other players.
                        return false;
                    }
                }

                if ((thing.Flags & MobjFlags.Shootable) == 0)
                {
                    // didn't do any damage
                    return (thing.Flags & MobjFlags.Solid) == 0;
                }

                // damage / explode
                var damage = ((random.Next() % 8) + 1) * tmthing.Info.Damage;
                //P_DamageMobj(thing, tmthing, tmthing->target, damage);

                // don't traverse any more
                return false;
            }

            // check for special pickup
            if ((thing.Flags & MobjFlags.Special) != 0)
            {
                var solid = (thing.Flags & MobjFlags.Solid) != 0;
                if ((tmflags & MobjFlags.PickUp) != 0)
                {
                    // can remove thing
                    //P_TouchSpecialThing(thing, tmthing);
                }
                return !solid;
            }

            return (thing.Flags & MobjFlags.Solid) == 0;
        }
    }
}
