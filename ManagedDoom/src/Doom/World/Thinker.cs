using System;

namespace ManagedDoom
{
    public class Thinker
    {
        private Thinker prev;
        private Thinker next;
        private ThinkerState thinkerState;

        public Thinker()
        {
        }

        public virtual void Run()
        {
        }

        public Thinker Prev
        {
            get => prev;
            set => prev = value;
        }

        public Thinker Next
        {
            get => next;
            set => next = value;
        }

        public ThinkerState ThinkerState
        {
            get => thinkerState;
            set => thinkerState = value;
        }
    }
}
