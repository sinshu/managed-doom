using System;
using System.Security.Cryptography.X509Certificates;

namespace ManagedDoom.SoftwareRendering
{
    public sealed class AutoMapRenderer2
    {
        private DrawScreen screen;

        private float minX;
        private float maxX;
        private float width;
        private float minY;
        private float maxY;
        private float height;

        public AutoMapRenderer2(DrawScreen screen)
        {
            this.screen = screen;
        }

        public void Render(Player player)
        {
            var scale = screen.Width / 320;
            screen.FillRect(0, 0, screen.Width, screen.Height - 32 * scale, 0);

            var world = player.Mobj.World;
            var am = world.AutoMap;

            minX = am.MinX.ToFloat();
            maxX = am.MaxX.ToFloat();
            width = maxX - minX;
            minY = am.MinY.ToFloat();
            maxY = am.MaxY.ToFloat();
            height = maxY - minY;

            foreach (var line in world.Map.Lines)
            {
                var v1 = ToScreenPos(line.Vertex1);
                var v2 = ToScreenPos(line.Vertex2);
                screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, 120);
            }
        }

        private DrawPos ToScreenPos(Vertex v)
        {
            var x = screen.Width * (v.X.ToFloat() - minX) / width;
            var y = screen.Height * (v.Y.ToFloat() - minY) / height;
            return new DrawPos(x, y);
        }

        private struct DrawPos
        {
            public float X;
            public float Y;

            public DrawPos(float x, float y)
            {
                X = x;
                Y = y;
            }
        }
    }
}
