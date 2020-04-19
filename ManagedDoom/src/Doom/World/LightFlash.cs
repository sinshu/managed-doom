using System;

namespace ManagedDoom
{
    public sealed class LightFlash : Thinker
    {
        private World world;

        public Sector sector;
        public int count;
        public int maxlight;
        public int minlight;
        public int maxtime;
        public int mintime;

        public LightFlash(World world)
        {
            this.world = world;
        }

        public override void Run()
        {
            if (--count > 0)
            {
                return;
            }

            if (sector.LightLevel == maxlight)
            {
                sector.LightLevel = minlight;
                count = (world.Random.Next() & mintime) + 1;
            }
            else
            {
                sector.LightLevel = maxlight;
                count = (world.Random.Next() & maxtime) + 1;
            }
        }
    }
}
