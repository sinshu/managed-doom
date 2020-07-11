using System;

namespace ManagedDoom
{
    public sealed class GlowLight : Thinker
    {
        private static readonly int glowSpeed = 8;

        private World world;

        private Sector sector;
        private int minLight;
        private int maxLight;
        private int direction;

        public GlowLight(World world)
        {
            this.world = world;
        }

        public override void Run()
        {
            switch (direction)
            {
                case -1:
                    // Down.
                    sector.LightLevel -= glowSpeed;
                    if (sector.LightLevel <= minLight)
                    {
                        sector.LightLevel += glowSpeed;
                        direction = 1;
                    }
                    break;

                case 1:
                    // Up.
                    sector.LightLevel += glowSpeed;
                    if (sector.LightLevel >= maxLight)
                    {
                        sector.LightLevel -= glowSpeed;
                        direction = -1;
                    }
                    break;
            }
        }

        public Sector Sector
        {
            get => sector;
            set => sector = value;
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

        public int Direction
        {
            get => direction;
            set => direction = value;
        }
    }
}
