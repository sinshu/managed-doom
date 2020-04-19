using System;

namespace ManagedDoom
{
    public sealed class StrobeFlash : Thinker
    {
        public static readonly int STROBEBRIGHT = 5;
        public static readonly int FASTDARK = 15;
        public static readonly int SLOWDARK = 35;

        private World world;

        public Sector sector;
        public int count;
        public int minlight;
        public int maxlight;
        public int darktime;
        public int brighttime;

        public StrobeFlash(World world)
        {
            this.world = world;
        }

        public override void Run()
        {
            if (--count > 0)
            {
                return;
            }

            if (sector.LightLevel == minlight)
            {
                sector.LightLevel = maxlight;
                count = brighttime;
            }
            else
            {
                sector.LightLevel = minlight;
                count = darktime;
            }
        }
    }
}
