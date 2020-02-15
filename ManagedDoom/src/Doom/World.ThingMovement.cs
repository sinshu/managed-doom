using System;

namespace ManagedDoom
{
    public sealed partial class World
    {
        private static readonly Fixed MaxRadius = Fixed.FromInt(32);

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

        private Func<LineDef, bool> checkLine;
        private Func<Mobj, bool> checkThing;

        private void InitThingMovement()
        {
            tmbbox = new Fixed[4];

            spechit = new LineDef[16];

            checkLine = PIT_CheckLine;
            checkThing = PIT_CheckThing;
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

        //
        // P_CheckPosition
        // This is purely informative, nothing is modified
        // (except things picked up).
        // 
        // in:
        //  a mobj_t (can be valid or invalid)
        //  a position to be checked
        //   (doesn't need to be related to the mobj_t->x,y)
        //
        // during:
        //  special things are touched if MF_PICKUP
        //  early out on solid lines?
        //
        // out:
        //  newsubsec
        //  floorz
        //  ceilingz
        //  tmdropoffz
        //   the lowest point contacted
        //   (monsters won't move to a dropoff)
        //  speciallines[]
        //  numspeciallines
        //
        private bool CheckPosition(Mobj thing, Fixed x, Fixed y)
        {
            tmthing = thing;
            tmflags = thing.Flags;

            tmx = x;
            tmy = y;

            tmbbox[Box.Top] = y + tmthing.Radius;
            tmbbox[Box.Bottom] = y - tmthing.Radius;
            tmbbox[Box.Right] = x + tmthing.Radius;
            tmbbox[Box.Left] = x - tmthing.Radius;

            var newsubsec = Geometry.PointInSubsector(x, y, map);
            ceilingline = null;

            // The base floor / ceiling is from the subsector
            // that contains the point.
            // Any contacted lines the step closer together
            // will adjust them.
            tmfloorz = tmdropoffz = newsubsec.Sector.FloorHeight;
            tmceilingz = newsubsec.Sector.CeilingHeight;

            validCount++;
            numspechit = 0;

            if ((tmflags & MobjFlags.NoClip) != 0)
            {
                return true;
            }

            // Check things first, possibly picking things up.
            // The bounding box is extended by MAXRADIUS
            // because mobj_ts are grouped into mapblocks
            // based on their origin point, and can overlap
            // into adjacent blocks by up to MAXRADIUS units.
            var xl = (tmbbox[Box.Left] - map.BlockMap.OriginX - MaxRadius).Data >> BlockMap.MapBlockShift;
            var xh = (tmbbox[Box.Right] - map.BlockMap.OriginX + MaxRadius).Data >> BlockMap.MapBlockShift;
            var yl = (tmbbox[Box.Bottom] - map.BlockMap.OriginY - MaxRadius).Data >> BlockMap.MapBlockShift;
            var yh = (tmbbox[Box.Top] - map.BlockMap.OriginY + MaxRadius).Data >> BlockMap.MapBlockShift;

            for (var bx = xl; bx <= xh; bx++)
            {
                for (var by = yl; by <= yh; by++)
                {
                    if (!map.BlockMap.IterateThings(bx, by, checkThing))
                    {
                        return false;
                    }
                }
            }

            // check lines
            xl = (tmbbox[Box.Left] - map.BlockMap.OriginX).Data >> BlockMap.MapBlockShift;
            xh = (tmbbox[Box.Right] - map.BlockMap.OriginX).Data >> BlockMap.MapBlockShift;
            yl = (tmbbox[Box.Bottom] - map.BlockMap.OriginY).Data >> BlockMap.MapBlockShift;
            yh = (tmbbox[Box.Top] - map.BlockMap.OriginY).Data >> BlockMap.MapBlockShift;

            for (var bx = xl; bx <= xh; bx++)
            {
                for (var by = yl; by <= yh; by++)
                {
                    if (!map.BlockMap.IterateLines(bx, by, checkLine, validCount))
                    {
                        return false;
                    }
                }
            }

            return true;
        }




        //
        // P_TryMove
        // Attempt to move to a new position,
        // crossing special lines unless MF_TELEPORT is set.
        //
        private bool P_TryMove(Mobj thing, Fixed x, Fixed y)
        {
            floatok = false;

            if (!CheckPosition(thing, x, y))
            {
                // solid wall or thing
                return false;
            }

            if ((thing.Flags & MobjFlags.NoClip) == 0)
            {
                if (tmceilingz - tmfloorz < thing.Height)
                {
                    // doesn't fit
                    return false;
                }

                floatok = true;

                if ((thing.Flags & MobjFlags.Teleport) == 0
                    && tmceilingz - thing.Z < thing.Height)
                {
                    // mobj must lower itself to fit
                    return false;
                }

                if ((thing.Flags & MobjFlags.Teleport) == 0
                     && tmfloorz - thing.Z > Fixed.FromInt(24))
                {
                    // too big a step up
                    return false;
                }

                if ((thing.Flags & (MobjFlags.DropOff | MobjFlags.Float)) == 0
                     && tmfloorz - tmdropoffz > Fixed.FromInt(24))
                {
                    // don't stand over a dropoff
                    return false;
                }
            }

            // the move is ok,
            // so link the thing into its new position
            UnsetThingPosition(thing);

            var oldx = thing.X;
            var oldy = thing.Y;
            thing.FloorZ = tmfloorz;
            thing.CeilingZ = tmceilingz;
            thing.X = x;
            thing.Y = y;

            SetThingPosition(thing);

            // if any special lines were hit, do the effect
            if ((thing.Flags & (MobjFlags.Teleport | MobjFlags.NoClip)) == 0)
            {
                while (numspechit-- != 0)
                {
                    // see if the line was crossed
                    var ld = spechit[numspechit];
                    var side = Geometry.PointOnLineSide(thing.X, thing.Y, ld);
                    var oldside = Geometry.PointOnLineSide(oldx, oldy, ld);
                    if (side != oldside)
                    {
                        if (ld.Special != 0)
                        {
                            //P_CrossSpecialLine(ld - lines, oldside, thing);
                        }
                    }
                }
            }

            return true;
        }





        //
        // SLIDE MOVE
        // Allows the player to slide along any angled walls.
        //
        private Fixed bestslidefrac;
        private Fixed secondslidefrac;

        private LineDef bestslideline;
        private LineDef secondslideline;

        private Mobj slidemo;

        private Fixed tmxmove;
        private Fixed tmymove;

        //
        // P_HitSlideLine
        // Adjusts the xmove / ymove
        // so that the next move will slide along the wall.
        //
        private void P_HitSlideLine(LineDef ld)
        {
            if (ld.SlopeType == SlopeType.Horizontal)
            {
                tmymove = Fixed.Zero;
                return;
            }

            if (ld.SlopeType == SlopeType.Vertical)
            {
                tmxmove = Fixed.Zero;
                return;
            }

            var side = Geometry.PointOnLineSide(slidemo.X, slidemo.Y, ld);

            var lineangle = Geometry.PointToAngle(Fixed.Zero, Fixed.Zero, ld.Dx, ld.Dy);

            if (side == 1)
            {
                lineangle += Angle.Ang180;
            }

            var moveangle = Geometry.PointToAngle(Fixed.Zero, Fixed.Zero, tmxmove, tmymove);
            var deltaangle = moveangle - lineangle;

            if (deltaangle > Angle.Ang180)
            {
                deltaangle += Angle.Ang180;
            }
            //	I_Error ("SlideLine: ang>ANG180");

            var movelen = Geometry.AproxDistance(tmxmove, tmymove);
            var newlen = movelen * Trig.Cos(deltaangle);

            tmxmove = newlen * Trig.Cos(lineangle);
            tmymove = newlen * Trig.Sin(lineangle);
        }


        //
        // PTR_SlideTraverse
        //
        private bool PTR_SlideTraverse(Intercept ic)
        {
            if (ic.Line == null)
            {
                throw new Exception("PTR_SlideTraverse: not a line?");
            }

            LineDef li = ic.Line;

            if ((li.Flags & LineFlags.TwoSided) == 0)
            {
                if (Geometry.PointOnLineSide(slidemo.X, slidemo.Y, li) != 0)
                {
                    // don't hit the back side
                    return true;
                }

                goto isblocking;
            }

            // set openrange, opentop, openbottom
            LineOpening(li);

            if (openRange < slidemo.Height)
            {
                // doesn't fit
                goto isblocking;
            }

            if (openTop - slidemo.Z < slidemo.Height)
            {
                // mobj is too high
                goto isblocking;
            }

            if (openBottom - slidemo.Z > Fixed.FromInt(24))
            {
                // too big a step up
                goto isblocking;
            }

            // this line doesn't block movement
            return true;

        // the line does block movement,
        // see if it is closer than best so far
        isblocking:
            if (ic.Frac < bestslidefrac)
            {
                secondslidefrac = bestslidefrac;
                secondslideline = bestslideline;
                bestslidefrac = ic.Frac;
                bestslideline = li;
            }

            return false;   // stop
        }


        //
        // P_SlideMove
        // The momx / momy move is bad, so try to slide
        // along a wall.
        // Find the first line hit, move flush to it,
        // and slide along it
        //
        // This is a kludgy mess.
        //
        private void P_SlideMove(Mobj mo)
        {
            slidemo = mo;
            var hitcount = 0;

        retry:
            if (++hitcount == 3)
            {
                // don't loop forever

                // the move most have hit the middle, so stairstep
                if (!P_TryMove(mo, mo.X, mo.Y + mo.MomY))
                {
                    P_TryMove(mo, mo.X + mo.MomX, mo.Y);
                }
                return;
            }

            Fixed leadx;
            Fixed leady;
            Fixed trailx;
            Fixed traily;

            // trace along the three leading corners
            if (mo.MomX > Fixed.Zero)
            {
                leadx = mo.X + mo.Radius;
                trailx = mo.X - mo.Radius;
            }
            else
            {
                leadx = mo.X - mo.Radius;
                trailx = mo.X + mo.Radius;
            }

            if (mo.MomY > Fixed.Zero)
            {
                leady = mo.Y + mo.Radius;
                traily = mo.Y - mo.Radius;
            }
            else
            {
                leady = mo.Y - mo.Radius;
                traily = mo.Y + mo.Radius;
            }

            bestslidefrac = new Fixed(Fixed.FracUnit + 1);

            PathTraverse(leadx, leady, leadx + mo.MomX, leady + mo.MomY,
                PathTraverseFlags.AddLines, ic => PTR_SlideTraverse(ic));

            PathTraverse(trailx, leady, trailx + mo.MomX, leady + mo.MomY,
                PathTraverseFlags.AddLines, ic => PTR_SlideTraverse(ic));

            PathTraverse(leadx, traily, leadx + mo.MomX, traily + mo.MomY,
                PathTraverseFlags.AddLines, ic => PTR_SlideTraverse(ic));

            // move up to the wall
            if (bestslidefrac == new Fixed(Fixed.FracUnit + 1))
            {
                // the move most have hit the middle, so stairstep
                if (!P_TryMove(mo, mo.X, mo.Y + mo.MomY))
                {
                    P_TryMove(mo, mo.X + mo.MomX, mo.Y);
                }
                return;
            }

            // fudge a bit to make sure it doesn't hit
            bestslidefrac = new Fixed(bestslidefrac.Data - 0x800);
            if (bestslidefrac > Fixed.Zero)
            {
                var newx = mo.MomX * bestslidefrac;
                var newy = mo.MomY * bestslidefrac;

                if (!P_TryMove(mo, mo.X + newx, mo.Y + newy))
                {
                    // the move most have hit the middle, so stairstep
                    if (!P_TryMove(mo, mo.X, mo.Y + mo.MomY))
                    {
                        P_TryMove(mo, mo.X + mo.MomX, mo.Y);
                    }
                    return;
                }
            }

            // Now continue along the wall.
            // First calculate remainder.
            bestslidefrac = new Fixed(Fixed.FracUnit - (bestslidefrac.Data + 0x800));

            if (bestslidefrac > Fixed.One)
            {
                bestslidefrac = Fixed.One;
            }

            if (bestslidefrac <= Fixed.Zero)
            {
                return;
            }

            tmxmove = mo.MomX * bestslidefrac;
            tmymove = mo.MomY * bestslidefrac;

            // clip the moves
            P_HitSlideLine(bestslideline);

            mo.MomX = tmxmove;
            mo.MomY = tmymove;

            if (!P_TryMove(mo, mo.X + tmxmove, mo.Y + tmymove))
            {
                goto retry;
            }
        }
    }
}
