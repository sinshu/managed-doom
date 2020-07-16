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

        public static VerticalDoor RentVlDoor(World world)
        {
            return new VerticalDoor(world);
        }

        public static Platform RentPlatform(World world)
        {
            return new Platform(world);
        }

        public static FloorMove RentFloorMove(World world)
        {
            return new FloorMove(world);
        }

        public static CeilingMove RentCeiligMove(World world)
        {
            return new CeilingMove(world);
        }

        public static FireFlicker RentFireFlicker(World world)
        {
            return new FireFlicker(world);
        }

        public static LightFlash RentLightFlash(World world)
        {
            return new LightFlash(world);
        }

        public static StrobeFlash RentStrobeFlash(World world)
        {
            return new StrobeFlash(world);
        }

        public static GlowingLight RentGlowingLight(World world)
        {
            return new GlowingLight(world);
        }

        public static void Return(Thinker thinker)
        {
        }
    }
}
