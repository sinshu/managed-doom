using System;

namespace ManagedDoom
{
    public sealed class ThingMovement
    {
        private World world;

        public ThingMovement(World world)
        {
            this.world = world;

            InitThingMovement();
            InitSlideMovement();
        }

        public static readonly Fixed FloatSpeed = Fixed.FromInt(4);

        private static readonly int maxSpecialCross = 16;
        private static readonly Fixed MaxMove = Fixed.FromInt(30);
        private static readonly Fixed Gravity = Fixed.One;

        private Mobj currentThing;
        private MobjFlags currentFlags;
        private Fixed currentX;
        private Fixed currentY;
        private Fixed[] currentBox;

        private Fixed currentFloorZ;
        private Fixed currentCeilingZ;
        private Fixed currentDropoffZ;
        private bool floatOk;

        private LineDef currentCeilingLine;

        public int hitSpecialCount;
        public LineDef[] hitSpecialLines;

        private Func<LineDef, bool> checkLineFunc;
        private Func<Mobj, bool> checkThingFunc;


        private void InitThingMovement()
        {
            currentBox = new Fixed[4];

            hitSpecialLines = new LineDef[maxSpecialCross];

            checkLineFunc = CheckLine;
            checkThingFunc = CheckThing;
        }


        public void SetThingPosition(Mobj thing)
        {
            var map = world.Map;

            var subsector = Geometry.PointInSubsector(thing.X, thing.Y, map);

            thing.Subsector = subsector;

            // Invisible things don't go into the sector links.
            if ((thing.Flags & MobjFlags.NoSector) == 0)
            {
                var sector = subsector.Sector;

                thing.SPrev = null;
                thing.SNext = sector.ThingList;

                if (sector.ThingList != null)
                {
                    sector.ThingList.SPrev = thing;
                }

                sector.ThingList = thing;
            }

            // Inert things don't need to be in blockmap.
            if ((thing.Flags & MobjFlags.NoBlockMap) == 0)
            {
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
                    // Thing is off the map.
                    thing.BNext = null;
                    thing.BPrev = null;
                }
            }
        }


        public void UnsetThingPosition(Mobj thing)
        {
            var map = world.Map;

            // Invisible things don't go into the sector links.
            if ((thing.Flags & MobjFlags.NoSector) == 0)
            {
                // Unlink from subsector.
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

            // Inert things don't need to be in blockmap.
            if ((thing.Flags & MobjFlags.NoBlockMap) == 0)
            {
                // Unlink from block map.
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


        private bool CheckLine(LineDef line)
        {
            var mc = world.MapCollision;

            if (currentBox[Box.Right] <= line.BboxLeft
                || currentBox[Box.Left] >= line.BboxRight
                || currentBox[Box.Top] <= line.BboxBottom
                || currentBox[Box.Bottom] >= line.BboxTop)
            {
                return true;
            }

            if (Geometry.BoxOnLineSide(currentBox, line) != -1)
            {
                return true;
            }

            // A line has been hit.

            // The moving thing's destination position will cross
            // the given line.
            // If this should not be allowed, return false.
            // If the line is special, keep track of it
            // to process later if the move is proven ok.
            // NOTE: specials are NOT sorted by order,
            // so two special lines that are only 8 pixels apart
            // could be crossed in either order.

            if (line.BackSector == null)
            {
                // One sided line.
                return false;
            }

            if ((currentThing.Flags & MobjFlags.Missile) == 0)
            {
                if ((line.Flags & LineFlags.Blocking) != 0)
                {
                    // Explicitly blocking everything.
                    return false;
                }

                if (currentThing.Player == null && (line.Flags & LineFlags.BlockMonsters) != 0)
                {
                    // Block monsters only.
                    return false;
                }
            }

            // Set openrange, opentop, openbottom.
            mc.LineOpening(line);

            // Adjust floor / ceiling heights.
            if (mc.OpenTop < currentCeilingZ)
            {
                currentCeilingZ = mc.OpenTop;
                currentCeilingLine = line;
            }

            if (mc.OpenBottom > currentFloorZ)
            {
                currentFloorZ = mc.OpenBottom;
            }

            if (mc.LowFloor < currentDropoffZ)
            {
                currentDropoffZ = mc.LowFloor;
            }

            // If contacted a special line, add it to the list
            if (line.Special != 0)
            {
                hitSpecialLines[hitSpecialCount] = line;
                hitSpecialCount++;
            }

            return true;
        }


        private bool CheckThing(Mobj thing)
        {
            if ((thing.Flags & (MobjFlags.Solid | MobjFlags.Special | MobjFlags.Shootable)) == 0)
            {
                return true;
            }

            var blockdist = thing.Radius + currentThing.Radius;

            if (Fixed.Abs(thing.X - currentX) >= blockdist || Fixed.Abs(thing.Y - currentY) >= blockdist)
            {
                // Didn't hit it.
                return true;
            }

            // Don't clip against self.
            if (thing == currentThing)
            {
                return true;
            }

            // Check for skulls slamming into things.
            if ((currentThing.Flags & MobjFlags.SkullFly) != 0)
            {
                var damage = ((world.Random.Next() % 8) + 1) * currentThing.Info.Damage;

                world.ThingInteraction.DamageMobj(thing, currentThing, currentThing, damage);

                currentThing.Flags &= ~MobjFlags.SkullFly;
                currentThing.MomX = currentThing.MomY = currentThing.MomZ = Fixed.Zero;

                currentThing.SetState(currentThing.Info.SpawnState);

                // Stop moving.
                return false;
            }


            // Missiles can hit other things.
            if ((currentThing.Flags & MobjFlags.Missile) != 0)
            {
                // See if it went over / under.
                if (currentThing.Z > thing.Z + thing.Height)
                {
                    // Overhead.
                    return true;
                }

                if (currentThing.Z + currentThing.Height < thing.Z)
                {
                    // Underneath.
                    return true;
                }

                if (currentThing.Target != null
                    && (currentThing.Target.Type == thing.Type
                        || (currentThing.Target.Type == MobjType.Knight && thing.Type == MobjType.Bruiser)
                        || (currentThing.Target.Type == MobjType.Bruiser && thing.Type == MobjType.Knight)))
                {
                    // Don't hit same species as originator.
                    if (thing == currentThing.Target)
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
                    // Didn't do any damage.
                    return (thing.Flags & MobjFlags.Solid) == 0;
                }

                // Damage / explode.
                var damage = ((world.Random.Next() % 8) + 1) * currentThing.Info.Damage;
                world.ThingInteraction.DamageMobj(thing, currentThing, currentThing.Target, damage);

                // Don't traverse any more.
                return false;
            }

            // Check for special pickup.
            if ((thing.Flags & MobjFlags.Special) != 0)
            {
                var solid = (thing.Flags & MobjFlags.Solid) != 0;
                if ((currentFlags & MobjFlags.PickUp) != 0)
                {
                    // Can remove thing.
                    world.ItemPickup.TouchSpecialThing(thing, currentThing);
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
        public bool CheckPosition(Mobj thing, Fixed x, Fixed y)
        {
            var map = world.Map;
            var bm = map.BlockMap;

            currentThing = thing;
            currentFlags = thing.Flags;

            currentX = x;
            currentY = y;

            currentBox[Box.Top] = y + currentThing.Radius;
            currentBox[Box.Bottom] = y - currentThing.Radius;
            currentBox[Box.Right] = x + currentThing.Radius;
            currentBox[Box.Left] = x - currentThing.Radius;

            var newsubsec = Geometry.PointInSubsector(x, y, map);

            currentCeilingLine = null;

            // The base floor / ceiling is from the subsector
            // that contains the point.
            // Any contacted lines the step closer together
            // will adjust them.
            currentFloorZ = currentDropoffZ = newsubsec.Sector.FloorHeight;
            currentCeilingZ = newsubsec.Sector.CeilingHeight;

            var validCount = world.GetNewValidCount();

            hitSpecialCount = 0;

            if ((currentFlags & MobjFlags.NoClip) != 0)
            {
                return true;
            }

            // Check things first, possibly picking things up.
            // The bounding box is extended by MAXRADIUS
            // because mobj_ts are grouped into mapblocks
            // based on their origin point, and can overlap
            // into adjacent blocks by up to MAXRADIUS units.
            {
                var blockX1 = bm.GetBlockX(currentBox[Box.Left] - GameConstants.MaxThingRadius);
                var blockX2 = bm.GetBlockX(currentBox[Box.Right] + GameConstants.MaxThingRadius);
                var blockY1 = bm.GetBlockY(currentBox[Box.Bottom] - GameConstants.MaxThingRadius);
                var blockY2 = bm.GetBlockY(currentBox[Box.Top] + GameConstants.MaxThingRadius);

                for (var bx = blockX1; bx <= blockX2; bx++)
                {
                    for (var by = blockY1; by <= blockY2; by++)
                    {
                        if (!map.BlockMap.IterateThings(bx, by, checkThingFunc))
                        {
                            return false;
                        }
                    }
                }
            }

            // Check lines.
            {
                var blockX1 = bm.GetBlockX(currentBox[Box.Left]);
                var blockX2 = bm.GetBlockX(currentBox[Box.Right]);
                var blockY1 = bm.GetBlockY(currentBox[Box.Bottom]);
                var blockY2 = bm.GetBlockY(currentBox[Box.Top]);

                for (var bx = blockX1; bx <= blockX2; bx++)
                {
                    for (var by = blockY1; by <= blockY2; by++)
                    {
                        if (!map.BlockMap.IterateLines(bx, by, checkLineFunc, validCount))
                        {
                            return false;
                        }
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
        public bool TryMove(Mobj thing, Fixed x, Fixed y)
        {
            floatOk = false;

            if (!CheckPosition(thing, x, y))
            {
                // Solid wall or thing.
                return false;
            }

            if ((thing.Flags & MobjFlags.NoClip) == 0)
            {
                if (currentCeilingZ - currentFloorZ < thing.Height)
                {
                    // Doesn't fit.
                    return false;
                }

                floatOk = true;

                if ((thing.Flags & MobjFlags.Teleport) == 0
                    && currentCeilingZ - thing.Z < thing.Height)
                {
                    // Mobj must lower itself to fit.
                    return false;
                }

                if ((thing.Flags & MobjFlags.Teleport) == 0
                     && currentFloorZ - thing.Z > Fixed.FromInt(24))
                {
                    // Too big a step up.
                    return false;
                }

                if ((thing.Flags & (MobjFlags.DropOff | MobjFlags.Float)) == 0
                     && currentFloorZ - currentDropoffZ > Fixed.FromInt(24))
                {
                    // Don't stand over a dropoff.
                    return false;
                }
            }

            // The move is ok,
            // so link the thing into its new position.
            UnsetThingPosition(thing);

            var oldx = thing.X;
            var oldy = thing.Y;
            thing.FloorZ = currentFloorZ;
            thing.CeilingZ = currentCeilingZ;
            thing.X = x;
            thing.Y = y;

            SetThingPosition(thing);

            // If any special lines were hit, do the effect.
            if ((thing.Flags & (MobjFlags.Teleport | MobjFlags.NoClip)) == 0)
            {
                while (hitSpecialCount-- > 0)
                {
                    // See if the line was crossed.
                    var line = hitSpecialLines[hitSpecialCount];
                    var newSide = Geometry.PointOnLineSide(thing.X, thing.Y, line);
                    var oldSide = Geometry.PointOnLineSide(oldx, oldy, line);
                    if (newSide != oldSide)
                    {
                        if (line.Special != 0)
                        {
                            //P_CrossSpecialLine(ld - lines, oldside, thing);
                        }
                    }
                }
            }

            return true;
        }


        //
        // P_XYMovement  
        //
        private static readonly Fixed stopSpeed = new Fixed(0x1000);
        private static readonly Fixed friction = new Fixed(0xe800);

        public void XYMovement(Mobj thing)
        {
            if (thing.MomX == Fixed.Zero && thing.MomY == Fixed.Zero)
            {
                if ((thing.Flags & MobjFlags.SkullFly) != 0)
                {
                    // The skull slammed into something.
                    thing.Flags &= ~MobjFlags.SkullFly;
                    thing.MomX = thing.MomY = thing.MomZ = Fixed.Zero;

                    thing.SetState(thing.Info.SpawnState);
                }

                return;
            }

            var player = thing.Player;

            if (thing.MomX > MaxMove)
            {
                thing.MomX = MaxMove;
            }
            else if (thing.MomX < -MaxMove)
            {
                thing.MomX = -MaxMove;
            }

            if (thing.MomY > MaxMove)
            {
                thing.MomY = MaxMove;
            }
            else if (thing.MomY < -MaxMove)
            {
                thing.MomY = -MaxMove;
            }

            var moveX = thing.MomX;
            var moveY = thing.MomY;

            do
            {
                Fixed pMoveX;
                Fixed pMoveY;

                if (moveX > MaxMove / 2 || moveY > MaxMove / 2)
                {
                    pMoveX = thing.X + moveX / 2;
                    pMoveY = thing.Y + moveY / 2;
                    moveX = new Fixed(moveX.Data >> 1);
                    moveY = new Fixed(moveY.Data >> 1);
                }
                else
                {
                    pMoveX = thing.X + moveX;
                    pMoveY = thing.Y + moveY;
                    moveX = moveY = Fixed.Zero;
                }

                if (!TryMove(thing, pMoveX, pMoveY))
                {
                    // Blocked move.
                    if (thing.Player != null)
                    {   // Try to slide along it.
                        SlideMove(thing);
                    }
                    else if ((thing.Flags & MobjFlags.Missile) != 0)
                    {
                        // Explode a missile.
                        if (currentCeilingLine != null
                            && currentCeilingLine.BackSector != null
                            && currentCeilingLine.BackSector.CeilingFlat == world.Map.SkyFlatNumber)
                        {
                            // Hack to prevent missiles exploding against the sky.
                            // Does not handle sky floors.
                            world.ThingAllocation.RemoveMobj(thing);
                            return;
                        }
                        world.ThingInteraction.ExplodeMissile(thing);
                    }
                    else
                    {
                        thing.MomX = thing.MomY = Fixed.Zero;
                    }
                }
            }
            while (moveX != Fixed.Zero || moveY != Fixed.Zero);

            // Slow down.
            if (player != null && (player.Cheats & CheatFlags.NoMomentum) != 0)
            {
                // Debug option for no sliding at all.
                thing.MomX = thing.MomY = Fixed.Zero;
                return;
            }

            if ((thing.Flags & (MobjFlags.Missile | MobjFlags.SkullFly)) != 0)
            {
                // No friction for missiles ever.
                return;
            }

            if (thing.Z > thing.FloorZ)
            {
                // No friction when airborne.
                return;
            }

            if ((thing.Flags & MobjFlags.Corpse) != 0)
            {
                // Do not stop sliding if halfway off a step with some momentum.
                if (thing.MomX > Fixed.One / 4
                    || thing.MomX < -Fixed.One / 4
                    || thing.MomY > Fixed.One / 4
                    || thing.MomY < -Fixed.One / 4)
                {
                    if (thing.FloorZ != thing.Subsector.Sector.FloorHeight)
                    {
                        return;
                    }
                }
            }

            if (thing.MomX > -stopSpeed
                && thing.MomX < stopSpeed
                && thing.MomY > -stopSpeed
                && thing.MomY < stopSpeed
                && (player == null || (player.Cmd.ForwardMove == 0 && player.Cmd.SideMove == 0)))
            {
                // If in a walking frame, stop moving.
                if (player != null && player.Mobj.State.Frame < 4)
                {
                    player.Mobj.SetState(State.Play);
                }

                thing.MomX = Fixed.Zero;
                thing.MomY = Fixed.Zero;
            }
            else
            {
                thing.MomX = thing.MomX * friction;
                thing.MomY = thing.MomY * friction;
            }
        }


        //
        // P_ZMovement
        //
        public void ZMovement(Mobj thing)
        {
            // Check for smooth step up.
            if (thing.Player != null && thing.Z < thing.FloorZ)
            {
                thing.Player.ViewHeight -= thing.FloorZ - thing.Z;

                thing.Player.DeltaViewHeight = new Fixed((Player.VIEWHEIGHT - thing.Player.ViewHeight).Data >> 3);
            }

            // Adjust height.
            thing.Z += thing.MomZ;

            if ((thing.Flags & MobjFlags.Float) != 0 && thing.Target != null)
            {
                // Float down towards target if too close.
                if ((thing.Flags & MobjFlags.SkullFly) == 0 && (thing.Flags & MobjFlags.InFloat) == 0)
                {
                    var dist = Geometry.AproxDistance(thing.X - thing.Target.X, thing.Y - thing.Target.Y);

                    var delta = (thing.Target.Z + new Fixed(thing.Height.Data >> 1)) - thing.Z;

                    if (delta < Fixed.Zero && dist < -(delta * 3))
                    {
                        thing.Z -= FloatSpeed;
                    }
                    else if (delta > Fixed.Zero && dist < (delta * 3))
                    {
                        thing.Z += FloatSpeed;
                    }
                }
            }

            // Clip movement.
            if (thing.Z <= thing.FloorZ)
            {
                // Hit the floor.

                // Note (id):
                // Somebody left this after the setting momz to 0, kinda useless there.
                if ((thing.Flags & MobjFlags.SkullFly) != 0)
                {
                    // The skull slammed into something.
                    thing.MomZ = -thing.MomZ;
                }

                if (thing.MomZ < Fixed.Zero)
                {
                    if (thing.Player != null && thing.MomZ < -Gravity * 8)
                    {
                        // Squat down.
                        // Decrease viewheight for a moment
                        // after hitting the ground (hard),
                        // and utter appropriate sound.
                        thing.Player.DeltaViewHeight = new Fixed(thing.MomZ.Data >> 3);
                        world.StartSound(thing, Sfx.OOF);
                    }
                    thing.MomZ = Fixed.Zero;
                }
                thing.Z = thing.FloorZ;

                if ((thing.Flags & MobjFlags.Missile) != 0 && (thing.Flags & MobjFlags.NoClip) == 0)
                {
                    world.ThingInteraction.ExplodeMissile(thing);
                    return;
                }
            }
            else if ((thing.Flags & MobjFlags.NoGravity) == 0)
            {
                if (thing.MomZ == Fixed.Zero)
                {
                    thing.MomZ = -Gravity * 2;
                }
                else
                {
                    thing.MomZ -= Gravity;
                }
            }

            if (thing.Z + thing.Height > thing.CeilingZ)
            {
                // Hit the ceiling.
                if (thing.MomZ > Fixed.Zero)
                {
                    thing.MomZ = Fixed.Zero;
                }

                {
                    thing.Z = thing.CeilingZ - thing.Height;
                }

                if ((thing.Flags & MobjFlags.SkullFly) != 0)
                {
                    // The skull slammed into something.
                    thing.MomZ = -thing.MomZ;
                }

                if ((thing.Flags & MobjFlags.Missile) != 0 && (thing.Flags & MobjFlags.NoClip) == 0)
                {
                    world.ThingInteraction.ExplodeMissile(thing);
                    return;
                }
            }
        }

        public Fixed CurrentFloorZ => currentFloorZ;
        public Fixed CurrentCeilingZ => currentCeilingZ;
        public Fixed CurrentDropoffZ => currentDropoffZ;
        public bool FloatOk => floatOk;



        //
        // SLIDE MOVE
        // Allows the player to slide along any angled walls.
        //
        private Fixed bestSlideFrac;
        private Fixed secondSlideFrac;

        private LineDef bestSlideLine;
        private LineDef secondSlideLine;

        private Mobj slideThing;
        private Fixed slideMoveX;
        private Fixed slideMoveY;

        private Func<Intercept, bool> slideTraverseFunc;


        private void InitSlideMovement()
        {
            slideTraverseFunc = SlideTraverse;
        }


        //
        // P_HitSlideLine
        // Adjusts the xmove / ymove
        // so that the next move will slide along the wall.
        //
        private void HitSlideLine(LineDef line)
        {
            if (line.SlopeType == SlopeType.Horizontal)
            {
                slideMoveY = Fixed.Zero;
                return;
            }

            if (line.SlopeType == SlopeType.Vertical)
            {
                slideMoveX = Fixed.Zero;
                return;
            }

            var side = Geometry.PointOnLineSide(slideThing.X, slideThing.Y, line);

            var lineAngle = Geometry.PointToAngle(Fixed.Zero, Fixed.Zero, line.Dx, line.Dy);
            if (side == 1)
            {
                lineAngle += Angle.Ang180;
            }

            var moveAngle = Geometry.PointToAngle(Fixed.Zero, Fixed.Zero, slideMoveX, slideMoveY);

            var deltaAngle = moveAngle - lineAngle;
            if (deltaAngle > Angle.Ang180)
            {
                deltaAngle += Angle.Ang180;
            }

            var moveDist = Geometry.AproxDistance(slideMoveX, slideMoveY);
            var newDist = moveDist * Trig.Cos(deltaAngle);

            slideMoveX = newDist * Trig.Cos(lineAngle);
            slideMoveY = newDist * Trig.Sin(lineAngle);
        }


        //
        // PTR_SlideTraverse
        //
        private bool SlideTraverse(Intercept intercept)
        {
            var mc = world.MapCollision;

            if (intercept.Line == null)
            {
                throw new Exception("PTR_SlideTraverse: not a line?");
            }

            var line = intercept.Line;

            if ((line.Flags & LineFlags.TwoSided) == 0)
            {
                if (Geometry.PointOnLineSide(slideThing.X, slideThing.Y, line) != 0)
                {
                    // Don't hit the back side.
                    return true;
                }

                goto isBlocking;
            }

            // Set openrange, opentop, openbottom.
            mc.LineOpening(line);

            if (mc.OpenRange < slideThing.Height)
            {
                // Doesn't fit.
                goto isBlocking;
            }

            if (mc.OpenTop - slideThing.Z < slideThing.Height)
            {
                // Mobj is too high.
                goto isBlocking;
            }

            if (mc.OpenBottom - slideThing.Z > Fixed.FromInt(24))
            {
                // Too big a step up.
                goto isBlocking;
            }

            // This line doesn't block movement.
            return true;

        // The line does block movement,
        // see if it is closer than best so far.
        isBlocking:
            if (intercept.Frac < bestSlideFrac)
            {
                secondSlideFrac = bestSlideFrac;
                secondSlideLine = bestSlideLine;
                bestSlideFrac = intercept.Frac;
                bestSlideLine = line;
            }

            // Stop.
            return false;
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
        private void SlideMove(Mobj thing)
        {
            var pt = world.PathTraversal;

            slideThing = thing;

            var hitCount = 0;

        retry:
            // Don't loop forever.
            if (++hitCount == 3)
            {
                // The move most have hit the middle, so stairstep.
                StairStep(thing);
                return;
            }

            Fixed leadX;
            Fixed leadY;
            Fixed trailX;
            Fixed trailY;

            // Trace along the three leading corners.
            if (thing.MomX > Fixed.Zero)
            {
                leadX = thing.X + thing.Radius;
                trailX = thing.X - thing.Radius;
            }
            else
            {
                leadX = thing.X - thing.Radius;
                trailX = thing.X + thing.Radius;
            }

            if (thing.MomY > Fixed.Zero)
            {
                leadY = thing.Y + thing.Radius;
                trailY = thing.Y - thing.Radius;
            }
            else
            {
                leadY = thing.Y - thing.Radius;
                trailY = thing.Y + thing.Radius;
            }

            bestSlideFrac = new Fixed(Fixed.FracUnit + 1);

            pt.PathTraverse(
                leadX, leadY, leadX + thing.MomX, leadY + thing.MomY,
                PathTraverseFlags.AddLines, slideTraverseFunc);

            pt.PathTraverse(
                trailX, leadY, trailX + thing.MomX, leadY + thing.MomY,
                PathTraverseFlags.AddLines, slideTraverseFunc);

            pt.PathTraverse(
                leadX, trailY, leadX + thing.MomX, trailY + thing.MomY,
                PathTraverseFlags.AddLines, slideTraverseFunc);

            // Move up to the wall.
            if (bestSlideFrac == new Fixed(Fixed.FracUnit + 1))
            {
                // The move most have hit the middle, so stairstep.
                StairStep(thing);
                return;
            }

            // Fudge a bit to make sure it doesn't hit.
            bestSlideFrac = new Fixed(bestSlideFrac.Data - 0x800);
            if (bestSlideFrac > Fixed.Zero)
            {
                var newX = thing.MomX * bestSlideFrac;
                var newY = thing.MomY * bestSlideFrac;

                if (!TryMove(thing, thing.X + newX, thing.Y + newY))
                {
                    // The move most have hit the middle, so stairstep.
                    StairStep(thing);
                    return;
                }
            }

            // Now continue along the wall.
            // First calculate remainder.
            bestSlideFrac = new Fixed(Fixed.FracUnit - (bestSlideFrac.Data + 0x800));

            if (bestSlideFrac > Fixed.One)
            {
                bestSlideFrac = Fixed.One;
            }

            if (bestSlideFrac <= Fixed.Zero)
            {
                return;
            }

            slideMoveX = thing.MomX * bestSlideFrac;
            slideMoveY = thing.MomY * bestSlideFrac;

            // Clip the moves.
            HitSlideLine(bestSlideLine);

            thing.MomX = slideMoveX;
            thing.MomY = slideMoveY;

            if (!TryMove(thing, thing.X + slideMoveX, thing.Y + slideMoveY))
            {
                goto retry;
            }
        }

        private void StairStep(Mobj thing)
        {
            if (!TryMove(thing, thing.X, thing.Y + thing.MomY))
            {
                TryMove(thing, thing.X + thing.MomX, thing.Y);
            }
        }
    }
}
