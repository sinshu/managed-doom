using System;
using System.Collections;
using System.Collections.Generic;

namespace ManagedDoom
{
    public sealed class Thinkers
    {
        private World world;

        private Thinker cap;

        public Thinkers(World world)
        {
            this.world = world;

            cap = new Thinker();
            cap.Prev = cap.Next = cap;
        }

        public void Add(Thinker thinker)
        {
            cap.Prev.Next = thinker;
            thinker.Next = cap;
            thinker.Prev = cap.Prev;
            cap.Prev = thinker;
        }

        public void Remove(Thinker thinker)
        {
            thinker.Removed = true;
        }

        public void Run()
        {
            var current = cap.Next;
            while (current != cap)
            {
                if (current.Removed)
                {
                    // Time to remove it.
                    current.Next.Prev = current.Prev;
                    current.Prev.Next = current.Next;
                    ThinkerPool.Return(current);
                }
                else
                {
                    current.Run();
                }
                current = current.Next;
            }
        }

        public ThinkerEnumerator GetEnumerator()
        {
            return new ThinkerEnumerator(this);
        }



        public struct ThinkerEnumerator : IEnumerator<Thinker>
        {
            private Thinkers thinkers;
            private Thinker current;

            public ThinkerEnumerator(Thinkers thinkers)
            {
                this.thinkers = thinkers;
                current = thinkers.cap;
            }

            public bool MoveNext()
            {
                while (true)
                {
                    current = current.Next;
                    if (current == thinkers.cap)
                    {
                        return false;
                    }
                    else if (!current.Removed)
                    {
                        return true;
                    }
                }
            }

            public void Reset()
            {
                current = thinkers.cap;
            }

            public void Dispose()
            {
            }

            public Thinker Current => current;

            object IEnumerator.Current => throw new NotImplementedException();
        }
    }
}
