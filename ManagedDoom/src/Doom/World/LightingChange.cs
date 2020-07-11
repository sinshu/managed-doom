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

            var flick = ThinkerPool.RentFireFlicker(world);

            world.Thinkers.Add(flick);

            flick.Sector = sector;
            flick.MaxLight = sector.LightLevel;
            flick.MinLight = FindMinSurroundingLight(sector, sector.LightLevel) + 16;
            flick.Count = 4;
        }


        public void SpawnLightFlash(Sector sector)
        {
            // Nothing special about it during gameplay.
            sector.Special = 0;

            var flash = ThinkerPool.RentLightFlash(world);

            world.Thinkers.Add(flash);

            flash.sector = sector;
            flash.maxlight = sector.LightLevel;

            flash.minlight = FindMinSurroundingLight(sector, sector.LightLevel);
            flash.maxtime = 64;
            flash.mintime = 7;
            flash.count = (world.Random.Next() & flash.maxtime) + 1;
        }


        public void SpawnStrobeFlash(Sector sector, int fastOrSlow, int inSync)
        {
            var flash = ThinkerPool.RentStrobeFlash(world);

            world.Thinkers.Add(flash);

            flash.sector = sector;
            flash.darktime = fastOrSlow;
            flash.brighttime = StrobeFlash.STROBEBRIGHT;
            flash.maxlight = sector.LightLevel;
            flash.minlight = FindMinSurroundingLight(sector, sector.LightLevel);

            if (flash.minlight == flash.maxlight)
            {
                flash.minlight = 0;
            }

            // Nothing special about it during gameplay.
            sector.Special = 0;

            if (inSync == 0)
            {
                flash.count = (world.Random.Next() & 7) + 1;
            }
            else
            {
                flash.count = 1;
            }
        }


        public void SpawnGlowingLight(Sector sector)
        {
            var g = ThinkerPool.RentGlowLight(world);

            world.Thinkers.Add(g);

            g.Sector = sector;
            g.MinLight = FindMinSurroundingLight(sector, sector.LightLevel);
            g.MaxLight = sector.LightLevel;
            g.Direction = -1;

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
