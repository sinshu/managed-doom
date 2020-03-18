using System;

namespace ManagedDoom
{
    public sealed class MapCollision
    {
        private World world;

        public MapCollision(World world)
        {
            this.world = world;
        }

        //
        // P_LineOpening
        // Sets opentop and openbottom to the window
        // through a two sided line.
        // OPTIMIZE: keep this precalculated
        //
        public Fixed openTop;
        public Fixed openBottom;
        public Fixed openRange;
        public Fixed lowFloor;

        public void LineOpening(LineDef linedef)
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
    }
}
