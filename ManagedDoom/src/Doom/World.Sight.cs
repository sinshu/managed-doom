using System;

namespace ManagedDoom
{
    public sealed partial class World
    {
        // eye z of looker
        private Fixed sightzstart;

        // from t1 to t2
        private DivLine strace;
        private Fixed t2x;
        private Fixed t2y;

        private void InitSight()
        {
            strace = new DivLine();
        }


    }
}
