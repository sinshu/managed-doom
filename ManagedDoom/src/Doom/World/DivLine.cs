using System;

namespace ManagedDoom
{
    public sealed class DivLine
    {
        public Fixed X;
        public Fixed Y;
        public Fixed Dx;
        public Fixed Dy;

        public void MakeFrom(LineDef li)
        {
            X = li.Vertex1.X;
            Y = li.Vertex1.Y;
            Dx = li.Dx;
            Dy = li.Dy;
        }
    }
}
