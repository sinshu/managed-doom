using System;

namespace ManagedDoom
{
    public sealed class FireFlicker : Thinker
    {
        private World world;

        private Sector sector;
        private int count;
        private int maxLight;
        private int minLight;

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

            if (sector.LightLevel - amount < minLight)
            {
                sector.LightLevel = minLight;
            }
            else
            {
                sector.LightLevel = maxLight - amount;
            }

            count = 4;
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
    }
}
