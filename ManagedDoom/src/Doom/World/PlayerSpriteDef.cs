using System;

namespace ManagedDoom
{
    public sealed class PlayerSpriteDef
    {
        private MobjStateDef state;
        private int tics;
        private Fixed sx;
        private Fixed sy;

        public void Clear()
        {
            state = null;
            tics = 0;
            sx = Fixed.Zero;
            sy = Fixed.Zero;
        }

        public MobjStateDef State
        {
            get => state;
            set => state = value;
        }

        public int Tics
        {
            get => tics;
            set => tics = value;
        }

        public Fixed Sx
        {
            get => sx;
            set => sx = value;
        }

        public Fixed Sy
        {
            get => sy;
            set => sy = value;
        }
    }
}
