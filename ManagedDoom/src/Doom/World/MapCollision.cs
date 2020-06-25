using System;

namespace ManagedDoom
{
    public sealed class MapCollision
    {
        private World world;

        private Fixed openTop;
        private Fixed openBottom;
        private Fixed openRange;
        private Fixed lowFloor;

        public MapCollision(World world)
        {
            this.world = world;
        }

        /// <summary>
        /// Sets opentop and openbottom to the window through a two sided line.
        /// </summary>
        public void LineOpening(LineDef line)
        {
            if (line.Side1 == null)
            {
                // If the line is single sided, nothing can pass through.
                openRange = Fixed.Zero;
                return;
            }

            var front = line.FrontSector;
            var back = line.BackSector;

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

        public Fixed OpenTop => openTop;
        public Fixed OpenBottom => openBottom;
        public Fixed OpenRange => openRange;
        public Fixed LowFloor => lowFloor;
    }
}
