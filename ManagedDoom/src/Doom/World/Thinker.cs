using System;

namespace ManagedDoom
{
    public class Thinker
    {
        public Thinker Prev;
        public Thinker Next;
        public ThinkerState ThinkerState;

        public Thinker()
        {
        }

        public virtual void Run()
        {
        }
    }
}
