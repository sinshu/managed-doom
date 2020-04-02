using System;

namespace ManagedDoom
{
    public static class DoomDebug
    {
        public static int CombineHash(int a, int b)
        {
            return (3 * a) ^ b;
        }
    }
}
