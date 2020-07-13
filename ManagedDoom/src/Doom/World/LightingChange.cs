using System;

namespace ManagedDoom
{
    public sealed class LightingChange
    {
        private World world;

        public LightingChange(World world)
        {
            this.world = world;
        }

        public void SpawnFireFlicker(Sector sector)
        {
            // Note that we are resetting sector attributes.
            // Nothing special about it during gameplay.
            sector.Special = 0;

            var flicker = ThinkerPool.RentFireFlicker(world);

            world.Thinkers.Add(flicker);

            flicker.Sector = sector;
            flicker.MaxLight = sector.LightLevel;
            flicker.MinLight = FindMinSurroundingLight(sector, sector.LightLevel) + 16;
            flicker.Count = 4;
        }

        public void SpawnLightFlash(Sector sector)
        {
            // Nothing special about it during gameplay.
            sector.Special = 0;

            var light = ThinkerPool.RentLightFlash(world);

            world.Thinkers.Add(light);

            light.Sector = sector;
            light.MaxLight = sector.LightLevel;

            light.MinLight = FindMinSurroundingLight(sector, sector.LightLevel);
            light.MaxTime = 64;
            light.MinTime = 7;
            light.Count = (world.Random.Next() & light.MaxTime) + 1;
        }

        public void SpawnStrobeFlash(Sector sector, int fastOrSlow, int inSync)
        {
            var strobe = ThinkerPool.RentStrobeFlash(world);

            world.Thinkers.Add(strobe);

            strobe.Sector = sector;
            strobe.DarkTime = fastOrSlow;
            strobe.BrightTime = StrobeFlash.StrobeBright;
            strobe.MaxLight = sector.LightLevel;
            strobe.MinLight = FindMinSurroundingLight(sector, sector.LightLevel);

            if (strobe.MinLight == strobe.MaxLight)
            {
                strobe.MinLight = 0;
            }

            // Nothing special about it during gameplay.
            sector.Special = 0;

            if (inSync == 0)
            {
                strobe.Count = (world.Random.Next() & 7) + 1;
            }
            else
            {
                strobe.Count = 1;
            }
        }

        public void SpawnGlowingLight(Sector sector)
        {
            var glowing = ThinkerPool.RentGlowingLight(world);

            world.Thinkers.Add(glowing);

            glowing.Sector = sector;
            glowing.MinLight = FindMinSurroundingLight(sector, sector.LightLevel);
            glowing.MaxLight = sector.LightLevel;
            glowing.Direction = -1;

            sector.Special = 0;
        }

        private int FindMinSurroundingLight(Sector sector, int max)
        {
            var min = max;
            for (var i = 0; i < sector.Lines.Length; i++)
            {
                var line = sector.Lines[i];
                var check = GetNextSector(line, sector);

                if (check == null)
                {
                    continue;
                }

                if (check.LightLevel < min)
                {
                    min = check.LightLevel;
                }
            }
            return min;
        }

        private Sector GetNextSector(LineDef line, Sector sector)
        {
            if ((line.Flags & LineFlags.TwoSided) == 0)
            {
                return null;
            }

            if (line.FrontSector == sector)
            {
                return line.BackSector;
            }

            return line.FrontSector;
        }
    }
}
