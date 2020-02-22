using System;

namespace ManagedDoom
{
    public sealed class PlayerSpriteDef
    {
        public StateDef State;
        public int Tics;
        public Fixed Sx;
        public Fixed Sy;

        public void Clear()
        {
            State = null;
            Tics = 0;
            Sx = Fixed.Zero;
            Sy = Fixed.Zero;
        }
    }
}
