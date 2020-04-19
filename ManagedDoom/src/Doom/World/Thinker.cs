using System;

namespace ManagedDoom
{
    public class Thinker
    {
        public Thinker Prev;
        public Thinker Next;
        public bool Active;
        public bool Removed;

        public Thinker()
        {
            Active = true;
            Removed = false;
        }

        public virtual void Run()
        {
        }
    }
}
