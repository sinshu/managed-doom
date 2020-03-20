using System;

namespace ManagedDoom
{
    public sealed class GlowLight : Thinker
    {
        private static readonly int GLOWSPEED = 8;

        private World world;

        public Sector sector;
        public int minlight;
        public int maxlight;
        public int direction;

        public GlowLight(World world)
        {
            this.world = world;
        }

        public override void Run()
        {
            switch (direction)
            {
                case -1:
                    // DOWN
                    sector.LightLevel -= GLOWSPEED;
                    if (sector.LightLevel <= minlight)
                    {
                        sector.LightLevel += GLOWSPEED;
                        direction = 1;
                    }
                    break;

                case 1:
                    // UP
                    sector.LightLevel += GLOWSPEED;
                    if (sector.LightLevel >= maxlight)
                    {
                        sector.LightLevel -= GLOWSPEED;
                        direction = -1;
                    }
                    break;
            }
        }
    }
}
