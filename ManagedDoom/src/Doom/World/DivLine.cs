using System;

namespace ManagedDoom
{
    public sealed class DivLine
    {
        public Fixed X;
        public Fixed Y;
        public Fixed Dx;
        public Fixed Dy;

        public void MakeFrom(LineDef line)
        {
            X = line.Vertex1.X;
            Y = line.Vertex1.Y;
            Dx = line.Dx;
            Dy = line.Dy;
        }
    }
}
