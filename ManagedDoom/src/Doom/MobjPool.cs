using System;

namespace ManagedDoom
{
    public sealed class MobjPool
    {
        private World world;

        public MobjPool(World world)
        {
            this.world = world;
        }

        public Mobj Rent()
        {
            return new Mobj(world);
        }

        public void Return(Mobj mobj)
        {
        }
    }
}
