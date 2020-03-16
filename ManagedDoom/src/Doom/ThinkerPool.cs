using System;

namespace ManagedDoom
{
    public static class ThinkerPool
    {
        static ThinkerPool()
        {
        }

        public static Mobj RentMobj(World world)
        {
            return new Mobj(world);
        }

        public static VlDoor RentVlDoor(World world)
        {
            return new VlDoor(world);
        }

        public static void Return(Thinker thinker)
        {
        }
    }
}
