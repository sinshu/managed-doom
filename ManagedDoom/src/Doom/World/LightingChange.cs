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

        //
        // P_SpawnFireFlicker
        //
        public void SpawnFireFlicker(Sector sector)
        {
            // Note that we are resetting sector attributes.
            // Nothing special about it during gameplay.
            sector.Special = 0;

            var flick = ThinkerPool.RentFireFlicker(world);

            world.Thinkers.Add(flick);

            flick.sector = sector;
            flick.maxlight = sector.LightLevel;
            flick.minlight = P_FindMinSurroundingLight(sector, sector.LightLevel) + 16;
            flick.count = 4;
        }

        //
        // P_SpawnLightFlash
        // After the map has been loaded, scan each sector
        // for specials that spawn thinkers
        //
        public void SpawnLightFlash(Sector sector)
        {
            // nothing special about it during gameplay
            sector.Special = 0;

            var flash = ThinkerPool.RentLightFlash(world);

            world.Thinkers.Add(flash);

            flash.sector = sector;
            flash.maxlight = sector.LightLevel;

            flash.minlight = P_FindMinSurroundingLight(sector, sector.LightLevel);
            flash.maxtime = 64;
            flash.mintime = 7;
            flash.count = (world.Random.Next() & flash.maxtime) + 1;
        }

        //
        // P_SpawnStrobeFlash
        // After the map has been loaded, scan each sector
        // for specials that spawn thinkers
        //
        public void SpawnStrobeFlash(Sector sector, int fastOrSlow, int inSync)
        {
            var flash = ThinkerPool.RentStrobeFlash(world);

            world.Thinkers.Add(flash);

            flash.sector = sector;
            flash.darktime = fastOrSlow;
            flash.brighttime = StrobeFlash.STROBEBRIGHT;
            flash.maxlight = sector.LightLevel;
            flash.minlight = P_FindMinSurroundingLight(sector, sector.LightLevel);

            if (flash.minlight == flash.maxlight)
            {
                flash.minlight = 0;
            }

            // nothing special about it during gameplay
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

            g.sector = sector;
            g.minlight = P_FindMinSurroundingLight(sector, sector.LightLevel);
            g.maxlight = sector.LightLevel;
            g.direction = -1;

            sector.Special = 0;
        }

        //
        // Find minimum light from an adjacent sector
        //
        private int P_FindMinSurroundingLight(Sector sector, int max)
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

        //
        // getNextSector()
        // Return sector_t * of sector next to current.
        // NULL if not two-sided line
        //
        private Sector GetNextSector(LineDef line, Sector sec)
        {
            if ((line.Flags & LineFlags.TwoSided) == 0)
            {
                return null;
            }

            if (line.FrontSector == sec)
            {
                return line.BackSector;
            }

            return line.FrontSector;
        }
    }
}
