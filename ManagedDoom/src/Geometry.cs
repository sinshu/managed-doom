using System;

namespace ManagedDoom
{
    public static class Geometry
    {
        public static Fixed PointToDist(Fixed fromX, Fixed fromY, Fixed toX, Fixed toY)
        {
            var dx = Fixed.Abs(toX - fromX);
            var dy = Fixed.Abs(toY - fromY);

            if (dy > dx)
            {
                var temp = dx;
                dx = dy;
                dy = temp;
            }

            Fixed frac;
            if (dx != Fixed.Zero)
            {
                frac = dy / dx;
            }
            else
            {
                frac = Fixed.Zero;
            }

            var angle = (Trig.TanToAngle((uint)frac.Data >> Trig.DBits) + Angle.Ang90).Data >> Trig.AngleToFineShift;

            // use as cosine
            var dist = dx / Trig.Sin((int)angle);

            return dist;
        }

        public static int PointOnSide(Fixed x, Fixed y, Node node)
        {
            if (node.Dx == Fixed.Zero)
            {
                if (x <= node.X)
                {
                    return node.Dy > Fixed.Zero ? 1 : 0;
                }
                else
                {
                    return node.Dy < Fixed.Zero ? 1 : 0;
                }
            }

            if (node.Dy == Fixed.Zero)
            {
                if (y <= node.Y)
                {
                    return node.Dx < Fixed.Zero ? 1 : 0;
                }
                else
                {
                    return node.Dx > Fixed.Zero ? 1 : 0;
                }
            }

            var dx = (x - node.X);
            var dy = (y - node.Y);

            // Try to quickly decide by looking at sign bits.
            if (((node.Dy.Data ^ node.Dx.Data ^ dx.Data ^ dy.Data) & 0x80000000) != 0)
            {
                if (((node.Dy.Data ^ dx.Data) & 0x80000000) != 0)
                {
                    // (left is negative)
                    return 1;
                }

                return 0;
            }

            var left = new Fixed(node.Dy.Data >> Fixed.FracBits) * dx;
            var right = dy * new Fixed(node.Dx.Data >> Fixed.FracBits);

            if (right < left)
            {
                // front side
                return 0;
            }
            else
            {
                // back side
                return 1;
            }
        }

        public static Angle PointToAngle(Fixed fromX, Fixed fromY, Fixed toX, Fixed toY)
        {
            var x = toX - fromX;
            var y = toY - fromY;

            if (x == Fixed.Zero && y == Fixed.Zero)
            {
                return Angle.Ang0;
            }

            if (x >= Fixed.Zero)
            {
                // x >= 0
                if (y >= Fixed.Zero)
                {
                    // y >= 0
                    if (x > y)
                    {
                        // octant 0
                        // return tantoangle[SlopeDiv(y, x)];
                        return Trig.TanToAngle(Trig.SlopeDiv(y, x));
                    }
                    else
                    {
                        // octant 1
                        // return ANG90 - 1 - tantoangle[SlopeDiv(x, y)];
                        return new Angle(Angle.Ang90.Data - 1) - Trig.TanToAngle(Trig.SlopeDiv(x, y));
                    }
                }
                else
                {
                    // y < 0
                    y = -y;

                    if (x > y)
                    {
                        // octant 8
                        // return -tantoangle[SlopeDiv(y, x)];
                        return -Trig.TanToAngle(Trig.SlopeDiv(y, x));
                    }
                    else
                    {
                        // octant 7
                        // return ANG270 + tantoangle[SlopeDiv(x, y)];
                        return Angle.Ang270 + Trig.TanToAngle(Trig.SlopeDiv(x, y));
                    }
                }
            }
            else
            {
                // x < 0
                x = -x;

                if (y >= Fixed.Zero)
                {
                    // y >= 0
                    if (x > y)
                    {
                        // octant 3
                        // return ANG180 - 1 - tantoangle[SlopeDiv(y, x)];
                        return new Angle(Angle.Ang180.Data - 1) - Trig.TanToAngle(Trig.SlopeDiv(y, x));
                    }
                    else
                    {
                        // octant 2
                        // return ANG90 + tantoangle[SlopeDiv(x, y)];
                        return Angle.Ang90 + Trig.TanToAngle(Trig.SlopeDiv(x, y));
                    }
                }
                else
                {
                    // y < 0
                    y = -y;

                    if (x > y)
                    {
                        // octant 4
                        // return ANG180 + tantoangle[SlopeDiv(y, x)];
                        return Angle.Ang180 + Trig.TanToAngle(Trig.SlopeDiv(y, x));
                    }
                    else
                    {
                        // octant 5
                        // return ANG270 - 1 - tantoangle[SlopeDiv(x, y)];
                        return new Angle(Angle.Ang270.Data - 1) - Trig.TanToAngle(Trig.SlopeDiv(x, y));
                    }
                }
            }
        }

        public static Subsector PointInSubsector(Fixed x, Fixed y, Map map)
        {
            // single subsector is a special case
            if (map.Nodes.Length == 0)
            {
                return map.Subsectors[0];
            }

            var nodenum = map.Nodes.Length - 1;

            while (!Node.IsSubsector(nodenum))
            {
                var node = map.Nodes[nodenum];
                var side = PointOnSide(x, y, node);
                nodenum = node.Children[side];
            }

            return map.Subsectors[Node.GetSubsector(nodenum)];
        }

        public static int PointOnSegSide(Fixed x, Fixed y, Seg line)
        {
            var lx = line.Vertex1.X;
            var ly = line.Vertex1.Y;

            var ldx = line.Vertex2.X - lx;
            var ldy = line.Vertex2.Y - ly;

            if (ldx == Fixed.Zero)
            {
                if (x <= lx)
                {
                    return ldy > Fixed.Zero ? 1 : 0;
                }
                else
                {
                    return ldy < Fixed.Zero ? 1 : 0;
                }
            }

            if (ldy == Fixed.Zero)
            {
                if (y <= ly)
                {
                    return ldx < Fixed.Zero ? 1 : 0;
                }
                else
                {
                    return ldx > Fixed.Zero ? 1 : 0;
                }
            }

            var dx = (x - lx);
            var dy = (y - ly);

            // Try to quickly decide by looking at sign bits.
            if (((ldy.Data ^ ldx.Data ^ dx.Data ^ dy.Data) & 0x80000000) != 0)
            {
                if (((ldy.Data ^ dx.Data) & 0x80000000) != 0)
                {
                    // (left is negative)
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

            var left = new Fixed(ldy.Data >> Fixed.FracBits) * dx;
            var right = dy * new Fixed(ldx.Data >> Fixed.FracBits);

            if (right < left)
            {
                // front side
                return 0;
            }
            else
            {
                // back side
                return 1;
            }
        }

        public static int PointOnLineSide(Fixed x, Fixed y, LineDef line)
        {
            if (line.Dx == Fixed.Zero)
            {
                if (x <= line.Vertex1.X)
                {
                    return line.Dy > Fixed.Zero ? 1 : 0;
                }
                else
                {
                    return line.Dy < Fixed.Zero ? 1 : 0;
                }
            }

            if (line.Dy == Fixed.Zero)
            {
                if (y <= line.Vertex1.Y)
                {
                    return line.Dx < Fixed.Zero ? 1 : 0;
                }
                else
                {
                    return line.Dx > Fixed.Zero ? 1 : 0;
                }
            }

            var dx = (x - line.Vertex1.X);
            var dy = (y - line.Vertex1.Y);

            var left = new Fixed(line.Dy.Data >> Fixed.FracBits) * dx;
            var right = dy * new Fixed(line.Dx.Data >> Fixed.FracBits);

            if (right < left)
            {
                // front side
                return 0;
            }
            else
            {
                // back side
                return 1;
            }
        }

        public static int BoxOnLineSide(Fixed[] tmbox, LineDef ld)
        {
            int p1;
            int p2;

            switch (ld.SlopeType)
            {
                case SlopeType.Horizontal:
                    p1 = tmbox[Box.Top] > ld.Vertex1.Y ? 1 : 0;
                    p2 = tmbox[Box.Bottom] > ld.Vertex1.Y ? 1 : 0;
                    if (ld.Dx < Fixed.Zero)
                    {
                        p1 ^= 1;
                        p2 ^= 1;
                    }
                    break;

                case SlopeType.Vertical:
                    p1 = tmbox[Box.Right] < ld.Vertex1.X ? 1 : 0;
                    p2 = tmbox[Box.Left] < ld.Vertex1.X ? 1 : 0;
                    if (ld.Dy < Fixed.Zero)
                    {
                        p1 ^= 1;
                        p2 ^= 1;
                    }
                    break;

                case SlopeType.Positive:
                    p1 = PointOnLineSide(tmbox[Box.Left], tmbox[Box.Top], ld);
                    p2 = PointOnLineSide(tmbox[Box.Right], tmbox[Box.Bottom], ld);
                    break;

                case SlopeType.Negative:
                    p1 = PointOnLineSide(tmbox[Box.Right], tmbox[Box.Top], ld);
                    p2 = PointOnLineSide(tmbox[Box.Left], tmbox[Box.Bottom], ld);
                    break;
                default:
                    throw new Exception();
            }

            if (p1 == p2)
            {
                return p1;
            }
            else
            {
                return -1;
            }
        }

        public static int PointOnDivLineSide(Fixed x, Fixed y, DivLine line)
        {
            if (line.Dx == Fixed.Zero)
            {
                if (x <= line.X)
                {
                    return line.Dy > Fixed.Zero ? 1 : 0;
                }
                else
                {
                    return line.Dy < Fixed.Zero ? 1 : 0;
                }
            }

            if (line.Dy == Fixed.Zero)
            {
                if (y <= line.Y)
                {
                    return line.Dx < Fixed.Zero ? 1 : 0;
                }
                else
                {
                    return line.Dx > Fixed.Zero ? 1 : 0;
                }
            }

            var dx = (x - line.X);
            var dy = (y - line.Y);

            // try to quickly decide by looking at sign bits
            if (((line.Dy.Data ^ line.Dx.Data ^ dx.Data ^ dy.Data) & 0x80000000) != 0)
            {
                if (((line.Dy.Data ^ dx.Data) & 0x80000000) != 0)
                {
                    // (left is negative)
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

            var left = new Fixed(line.Dy.Data >> 8) * new Fixed(dx.Data >> 8);
            var right = new Fixed(dy.Data >> 8) * new Fixed(line.Dx.Data >> 8);

            if (right < left)
            {
                // front side
                return 0;
            }
            else
            {
                // back side
                return 1;
            }
        }
    }
}
