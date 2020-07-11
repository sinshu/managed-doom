using System;

namespace ManagedDoom
{
    public sealed class DivLine
    {
        private Fixed x;
        private Fixed y;
        private Fixed dx;
        private Fixed dy;

        public void MakeFrom(LineDef line)
        {
            x = line.Vertex1.X;
            y = line.Vertex1.Y;
            dx = line.Dx;
            dy = line.Dy;
        }

        public Fixed X
        {
            get => x;
            set => x = value;
        }

        public Fixed Y
        {
            get => y;
            set => y = value;
        }

        public Fixed Dx
        {
            get => dx;
            set => dx = value;
        }

        public Fixed Dy
        {
            get => dy;
            set => dy = value;
        }
    }
}
