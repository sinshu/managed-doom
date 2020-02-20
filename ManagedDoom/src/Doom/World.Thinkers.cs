using System;

namespace ManagedDoom
{
    public sealed partial class World
    {
        private Thinker thinkerCap;
        private ThinkerPool thinkerPool;

        public void InitThinkers()
        {
            thinkerCap = new Thinker(this);
            thinkerCap.Prev = thinkerCap.Next = thinkerCap;

            thinkerPool = new ThinkerPool(this);
        }

        public void AddThinker(Thinker thinker)
        {
            thinkerCap.Prev.Next = thinker;
            thinker.Next = thinkerCap;
            thinker.Prev = thinkerCap.Prev;
            thinkerCap.Prev = thinker;
        }

        public void RemoveThinker(Thinker thinker)
        {
            thinker.Removed = true;
        }

        public void RunThinkers()
        {
            Thinker currentthinker;

            currentthinker = thinkerCap.Next;
            while (currentthinker != thinkerCap)
            {
                if (currentthinker.Removed)
                {
                    // time to remove it
                    currentthinker.Next.Prev = currentthinker.Prev;
                    currentthinker.Prev.Next = currentthinker.Next;
                    thinkerPool.Return(currentthinker);
                }
                else
                {
                    /*
                    if (currentthinker->function.acp1)
                    {
                        currentthinker->function.acp1(currentthinker);
                    }
                    */
                    currentthinker.Run();
                }
                currentthinker = currentthinker.Next;
            }
        }
    }
}
