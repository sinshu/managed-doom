using System;

namespace ManagedDoom
{
    public class Thinker
    {
        public World World;
        public bool Removed;

        public Thinker(World world)
        {
            World = world;
            Removed = false;
        }

        public virtual void Run()
        {
        }
    }
}
