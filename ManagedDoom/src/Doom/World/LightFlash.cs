using System;

namespace ManagedDoom
{
    public sealed class LightFlash : Thinker
    {
        private World world;

        private Sector sector;
        private int count;
        private int maxLight;
        private int minLight;
        private int maxTime;
        private int minTime;

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

            if (sector.LightLevel == maxLight)
            {
                sector.LightLevel = minLight;
                count = (world.Random.Next() & minTime) + 1;
            }
            else
            {
                sector.LightLevel = maxLight;
                count = (world.Random.Next() & maxTime) + 1;
            }
        }

        public Sector Sector
        {
            get => sector;
            set => sector = value;
        }

        public int Count
        {
            get => count;
            set => count = value;
        }

        public int MaxLight
        {
            get => maxLight;
            set => maxLight = value;
        }

        public int MinLight
        {
            get => minLight;
            set => minLight = value;
        }

        public int MaxTime
        {
            get => maxTime;
            set => maxTime = value;
        }

        public int MinTime
        {
            get => minTime;
            set => minTime = value;
        }
    }
}
