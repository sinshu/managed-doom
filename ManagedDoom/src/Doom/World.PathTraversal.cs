using System;

namespace ManagedDoom
{
    public sealed partial class World
    {
        private static readonly Fixed USERANGE = Fixed.FromInt(64);
        private static readonly Fixed MELEERANGE = Fixed.FromInt(64);
        private static readonly Fixed MISSILERANGE = Fixed.FromInt(32 * 64);

        private Intercept[] intercepts;
        private int interceptCount;
        private DivLine tempDiv;
        private DivLine trace;
        private bool earlyOut;
        //private PathTraverseFlags ptflags;

        private Func<LineDef, bool> addLineIntercepts;
        private Func<Mobj, bool> addThingIntercepts;

        private void InitPathTraversal()
        {
            intercepts = new Intercept[256];
            for (var i = 0; i < intercepts.Length; i++)
            {
                intercepts[i] = new Intercept();
            }

            trace = new DivLine();
            tempDiv = new DivLine();

            addLineIntercepts = PIT_AddLineIntercepts;
            addThingIntercepts = PIT_AddThingIntercepts;
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
            tempDiv.MakeFrom(ld);
            var frac = P_InterceptVector(trace, tempDiv);

            if (frac < Fixed.Zero)
            {
                // behind source
                return true;
            }

            // try to early out the check
            if (earlyOut && frac < Fixed.One && ld.BackSector == null)
            {
                // stop checking
                return false;
            }

            intercepts[interceptCount].Frac = frac;
            intercepts[interceptCount].Line = ld;
            intercepts[interceptCount].Thing = null;
            interceptCount++;

            // continue
            return true;
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

            tempDiv.X = x1;
            tempDiv.Y = y1;
            tempDiv.Dx = x2 - x1;
            tempDiv.Dy = y2 - y1;

            var frac = P_InterceptVector(trace, tempDiv);

            if (frac < Fixed.Zero)
            {
                // behind source
                return true;
            }

            intercepts[interceptCount].Frac = frac;
            intercepts[interceptCount].Line = null;
            intercepts[interceptCount].Thing = thing;
            interceptCount++;

            // keep going
            return true;
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
            var count = interceptCount;

            Intercept ic = null;

            while (count-- > 0)
            {
                var dist = Fixed.MaxValue;
                for (var scan = 0; scan < interceptCount; scan++)
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

            interceptCount = 0;

            if (((x1 - map.BlockMap.OriginX).Data & (BlockMap.MapBlockSize.Data - 1)) == 0)
            {
                // don't side exactly on a line
                x1 += Fixed.One;
            }

            if (((y1 - map.BlockMap.OriginY).Data & (BlockMap.MapBlockSize.Data - 1)) == 0)
            {
                // don't side exactly on a line
                y1 += Fixed.One;
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
                    if (!map.BlockMap.IterateLines(mapx, mapy, addLineIntercepts, validCount))
                    {
                        // early out
                        return false;
                    }
                }

                if ((flags & PathTraverseFlags.AddThings) != 0)
                {
                    if (!map.BlockMap.IterateThings(mapx, mapy, addThingIntercepts))
                    {
                        // early out
                        return false;
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








        //
        // P_LineAttack
        //

        // who got hit (or NULL)
        private Mobj linetarget;

        private Mobj shootthing;

        // Height if not aiming up or down
        // ???: use slope for monsters?
        private Fixed shootz;

        private int la_damage;
        private Fixed attackrange;

        private Fixed aimslope;

        // slopes to top and bottom of target
        private Fixed topslope;
        private Fixed bottomslope;




        //
        // PTR_AimTraverse
        // Sets linetaget and aimslope when a target is aimed at.
        //
        private bool PTR_AimTraverse(Intercept ic)
        {
            if (ic.Line != null)
            {
                var li = ic.Line;

                if ((li.Flags & LineFlags.TwoSided) == 0)
                {
                    // stop
                    return false;
                }

                // Crosses a two sided line.
                // A two sided line will restrict
                // the possible target ranges.
                LineOpening(li);

                if (openBottom >= openTop)
                {
                    // stop
                    return false;
                }

                var dist = attackrange * ic.Frac;

                if (li.FrontSector.FloorHeight != li.BackSector.FloorHeight)
                {
                    var slope = (openBottom - shootz) / dist;
                    if (slope > bottomslope)
                    {
                        bottomslope = slope;
                    }
                }

                if (li.FrontSector.CeilingHeight != li.BackSector.CeilingHeight)
                {
                    var slope = (openTop - shootz) / dist;
                    if (slope < topslope)
                    {
                        topslope = slope;
                    }
                }

                if (topslope <= bottomslope)
                {
                    // stop
                    return false;
                }

                // shot continues
                return true;
            }

            // shoot a thing
            var th = ic.Thing;
            if (th == shootthing)
            {
                // can't shoot self
                return true;
            }

            {
                if ((th.Flags & MobjFlags.Shootable) == 0)
                {
                    // corpse or something
                    return true;
                }

                // check angles to see if the thing can be aimed at
                var dist = attackrange * ic.Frac;
                var thingtopslope = (th.Z + th.Height - shootz) / dist;

                if (thingtopslope < bottomslope)
                {
                    // shot over the thing
                    return true;
                }

                var thingbottomslope = (th.Z - shootz) / dist;

                if (thingbottomslope > topslope)
                {
                    // shot under the thing
                    return true;
                }

                // this thing can be hit!
                if (thingtopslope > topslope)
                {
                    thingtopslope = topslope;
                }

                if (thingbottomslope < bottomslope)
                {
                    thingbottomslope = bottomslope;
                }

                aimslope = (thingtopslope + thingbottomslope) / 2;
                linetarget = th;

                // don't go any farther
                return false;
            }
        }




        //
        // PTR_ShootTraverse
        //
        private bool PTR_ShootTraverse(Intercept ic)
        {
            if (ic.Line != null)
            {
                var li = ic.Line;

                if (li.Special != 0)
                {
                    //P_ShootSpecialLine(shootthing, li);
                }

                if ((li.Flags & LineFlags.TwoSided) == 0)
                {
                    goto hitline;
                }

                // crosses a two sided line
                LineOpening(li);

                var dist = attackrange * ic.Frac;

                if (li.FrontSector.FloorHeight != li.BackSector.FloorHeight)
                {
                    var slope = (openBottom - shootz) / dist;
                    if (slope > aimslope)
                    {
                        goto hitline;
                    }
                }

                if (li.FrontSector.CeilingHeight != li.BackSector.CeilingHeight)
                {
                    var slope = (openTop - shootz) / dist;
                    if (slope < aimslope)
                    {
                        goto hitline;
                    }
                }

                // shot continues
                return true;

            // hit line
            hitline:
                // position a bit closer
                var frac = ic.Frac - Fixed.FromInt(4) / attackrange;
                var x = trace.X + trace.Dx * frac;
                var y = trace.Y + trace.Dy * frac;
                var z = shootz + aimslope * (frac * attackrange);

                if (li.FrontSector.CeilingFlat == map.SkyFlatNumber)
                {
                    // don't shoot the sky!
                    if (z > li.FrontSector.CeilingHeight)
                    {
                        return false;
                    }

                    // it's a sky hack wall
                    if (li.BackSector != null && li.BackSector.CeilingFlat == map.SkyFlatNumber)
                    {
                        return false;
                    }
                }

                // Spawn bullet puffs.
                SpawnPuff(x, y, z);

                // don't go any farther
                return false;
            }

            // shoot a thing
            var th = ic.Thing;
            if (th == shootthing)
            {
                // can't shoot self
                return true;
            }

            {
                if ((th.Flags & MobjFlags.Shootable) == 0)
                {
                    // corpse or something
                    return true;
                }

                // check angles to see if the thing can be aimed at
                var dist = attackrange * ic.Frac;
                var thingtopslope = (th.Z + th.Height - shootz) / dist;

                if (thingtopslope < aimslope)
                {
                    // shot over the thing
                    return true;
                }

                var thingbottomslope = (th.Z - shootz) / dist;

                if (thingbottomslope > aimslope)
                {
                    // shot under the thing
                    return true;
                }

                // hit thing
                // position a bit closer
                var frac = ic.Frac - Fixed.FromInt(10) / attackrange;

                var x = trace.X + trace.Dx * frac;
                var y = trace.Y + trace.Dy * frac;
                var z = shootz + aimslope * (frac * attackrange);

                // Spawn bullet puffs or blod spots,
                // depending on target type.
                if ((ic.Thing.Flags & MobjFlags.NoBlood) != 0)
                {
                    SpawnPuff(x, y, z);
                }
                else
                {
                    SpawnBlood(x, y, z, la_damage);
                }

                if (la_damage != 0)
                {
                    DamageMobj(th, shootthing, shootthing, la_damage);
                }

                // don't go any farther
                return false;
            }
        }


        //
        // P_AimLineAttack
        //
        public Fixed AimLineAttack(Mobj t1, Angle angle, Fixed distance)
        {
            //angle >>= ANGLETOFINESHIFT;
            shootthing = t1;

            var x2 = t1.X + (distance.Data >> Fixed.FracBits) * Trig.Cos(angle); // finecosine[angle];
            var y2 = t1.Y + (distance.Data >> Fixed.FracBits) * Trig.Sin(angle); // finesine[angle];
            shootz = t1.Z + new Fixed((t1.Height.Data >> 1) + 8 * Fixed.FracUnit);

            // can't shoot outside view angles
            topslope = new Fixed(100 * Fixed.FracUnit / 160);
            bottomslope = new Fixed(-100 * Fixed.FracUnit / 160);

            attackrange = distance;
            linetarget = null;

            PathTraverse(t1.X, t1.Y,
                x2, y2,
                PathTraverseFlags.AddLines | PathTraverseFlags.AddThings,
                ic => PTR_AimTraverse(ic));

            if (linetarget != null)
            {
                return aimslope;
            }

            return Fixed.Zero;
        }


        //
        // P_LineAttack
        // If damage == 0, it is just a test trace
        // that will leave linetarget set.
        //
        public void LineAttack(
            Mobj t1,
            Angle angle,
            Fixed distance,
            Fixed slope,
            int damage)
        {
            //angle >>= ANGLETOFINESHIFT;
            shootthing = t1;
            la_damage = damage;
            var x2 = t1.X + (distance.Data >> Fixed.FracBits) * Trig.Cos(angle); // finecosine[angle];
            var y2 = t1.Y + (distance.Data >> Fixed.FracBits) * Trig.Sin(angle); // finesine[angle];
            shootz = t1.Z + new Fixed((t1.Height.Data >> 1) + 8 * Fixed.FracUnit);
            attackrange = distance;
            aimslope = slope;

            PathTraverse(t1.X, t1.Y,
                x2, y2,
                PathTraverseFlags.AddLines | PathTraverseFlags.AddThings,
                ic => PTR_ShootTraverse(ic));
        }



        private void SpawnPuff(Fixed x, Fixed y, Fixed z)
        {
            z += new Fixed((random.Next() - random.Next()) << 10);

            var th = SpawnMobj(x, y, z, MobjType.Puff);
            th.MomZ = Fixed.One;
            th.Tics -= random.Next() & 3;

            if (th.Tics < 1)
            {
                th.Tics = 1;
            }

            // don't make punches spark on the wall
            if (attackrange == MELEERANGE)
            {
                SetMobjState(th, State.Puff3);
            }
        }



        //
        // P_SpawnBlood
        // 
        private void SpawnBlood(Fixed x, Fixed y, Fixed z, int damage)
        {
            z += new Fixed((random.Next() - random.Next()) << 10);
            var th = SpawnMobj(x, y, z, MobjType.Blood);
            th.MomZ = Fixed.FromInt(2);
            th.Tics -= random.Next() & 3;

            if (th.Tics < 1)
            {
                th.Tics = 1;
            }

            if (damage <= 12 && damage >= 9)
            {
                SetMobjState(th, State.Blood2);
            }
            else if (damage < 9)
            {
                SetMobjState(th, State.Blood3);
            }
        }
    }
}
