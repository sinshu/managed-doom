using System;

namespace ManagedDoom
{
    public sealed partial class World
    {
        private Intercept[] intercepts;
        private int intercept_p;
        private DivLine trace;
        private bool earlyOut;
        private PathTraverseFlags ptflags;

        private int validCount;

        private void InitPathTraversal()
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
    }
}
