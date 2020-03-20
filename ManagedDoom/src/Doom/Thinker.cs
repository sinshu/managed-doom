using System;

namespace ManagedDoom
{
    public class Thinker
    {
        public Thinker Prev;
        public Thinker Next;
        public bool Removed;

        public Thinker()
        {
            Removed = false;
        }

        public virtual void Run()
        {
        }
    }
}
