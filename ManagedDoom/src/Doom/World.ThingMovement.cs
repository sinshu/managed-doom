using System;

namespace ManagedDoom
{
    public sealed partial class World
    {
        private void InitThingMovement()
        {
        }

        public void SetThingPosition(Mobj thing)
        {
            var ss = Geometry.PointInSubsector(thing.X, thing.Y, map);

            thing.Subsector = ss;

            // invisible things don't go into the sector links
            if ((thing.Flags & MobjFlags.NoSector) == 0)
            {
                // link into subsector    

                var sec = ss.Sector;

                thing.SPrev = null;
                thing.SNext = sec.ThingList;

                if (sec.ThingList != null)
                {
                    sec.ThingList.SPrev = thing;
                }

                sec.ThingList = thing;
            }

            // inert things don't need to be in blockmap
            if ((thing.Flags & MobjFlags.NoBlockMap) == 0)
            {
                // link into blockmap

                var index = map.BlockMap.GetIndex(thing.X, thing.Y);

                if (index != -1)
                {
                    var link = map.BlockMap.ThingLists[index];

                    thing.BPrev = null;
                    thing.BNext = link;

                    if (link != null)
                    {
                        link.BPrev = thing;
                    }

                    map.BlockMap.ThingLists[index] = thing;
                }
                else
                {
                    // thing is off the map
                    thing.BNext = null;
                    thing.BPrev = null;
                }
            }
        }

        public void UnsetThingPosition(Mobj thing)
        {
            // invisible things don't go into the sector links
            if ((thing.Flags & MobjFlags.NoSector) == 0)
            {
                // unlink from subsector

                if (thing.SNext != null)
                {
                    thing.SNext.SPrev = thing.SPrev;
                }

                if (thing.SPrev != null)
                {
                    thing.SPrev.SNext = thing.SNext;
                }
                else
                {
                    thing.Subsector.Sector.ThingList = thing.SNext;
                }
            }

            // inert things don't need to be in blockmap
            if ((thing.Flags & MobjFlags.NoBlockMap) == 0)
            {
                // unlink from block map

                if (thing.BNext != null)
                {
                    thing.BNext.BPrev = thing.BPrev;
                }

                if (thing.BPrev != null)
                {
                    thing.BPrev.BNext = thing.BNext;
                }
                else
                {
                    var index = map.BlockMap.GetIndex(thing.X, thing.Y);

                    if (index != -1)
                    {
                        map.BlockMap.ThingLists[index] = thing.BNext;
                    }
                }
            }
        }
    }
}
