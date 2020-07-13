using System;

namespace ManagedDoom
{
    public sealed class StrobeFlash : Thinker
    {
        public static readonly int StrobeBright = 5;
        public static readonly int FastDark = 15;
        public static readonly int SlowDark = 35;

        private World world;

        private Sector sector;
        private int count;
        private int minLight;
        private int maxLight;
        private int darkTime;
        private int brightTime;

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

            if (sector.LightLevel == minLight)
            {
                sector.LightLevel = maxLight;
                count = brightTime;
            }
            else
            {
                sector.LightLevel = minLight;
                count = darkTime;
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

        public int MinLight
        {
            get => minLight;
            set => minLight = value;
        }

        public int MaxLight
        {
            get => maxLight;
            set => maxLight = value;
        }

        public int DarkTime
        {
            get => darkTime;
            set => darkTime = value;
        }

        public int BrightTime
        {
            get => brightTime;
            set => brightTime = value;
        }
    }
}
