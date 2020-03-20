using System;

namespace ManagedDoom
{
    public sealed class FireFlicker : Thinker
    {
        private World world;

        public Sector sector;
        public int count;
        public int maxlight;
        public int minlight;

        public FireFlicker(World world)
        {
            this.world = world;
        }

        public override void Run()
        {
            if (--count > 0)
            {
                return;
            }

            var amount = (world.Random.Next() & 3) * 16;

            if (sector.LightLevel - amount < minlight)
            {
                sector.LightLevel = minlight;
            }
            else
            {
                sector.LightLevel = maxlight - amount;
            }

            count = 4;
        }
    }
}
