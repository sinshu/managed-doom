using System;

namespace ManagedDoom
{
    public sealed class AutoMap
    {
        private World world;

        private Fixed minX;
        private Fixed maxX;
        private Fixed minY;
        private Fixed maxY;

        private bool visible;
        private AutoMapState state;

        public AutoMap(World world)
        {
            this.world = world;

            minX = Fixed.MaxValue;
            maxX = Fixed.MinValue;
            minY = Fixed.MaxValue;
            maxY = Fixed.MinValue;
            foreach (var vertex in world.Map.Vertices)
            {
                if (vertex.X < minX)
                {
                    minX = vertex.X;
                }

                if (vertex.X > maxX)
                {
                    maxX = vertex.X;
                }

                if (vertex.Y < minY)
                {
                    minY = vertex.Y;
                }

                if (vertex.Y > maxY)
                {
                    maxY = vertex.Y;
                }
            }

            visible = false;
            state = AutoMapState.None;
        }

        public void Open()
        {
            visible = true;
        }

        public void Close()
        {
            visible = false;
        }

        public Fixed MinX => minX;
        public Fixed MaxX => maxX;
        public Fixed MinY => minY;
        public Fixed MaxY => maxY;
        public bool Visible => visible;
        public AutoMapState State => state;
    }
}
