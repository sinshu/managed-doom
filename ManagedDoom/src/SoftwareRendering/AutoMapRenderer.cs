using System;
using System.Security.Cryptography.X509Certificates;

namespace ManagedDoom.SoftwareRendering
{
    public sealed class AutoMapRenderer
    {
        // For use if I do walls with outsides / insides.
        private static readonly int reds = (256 - 5 * 16);
        private static readonly int redRange = 16;
        private static readonly int blues = (256 - 4 * 16 + 8);
        private static readonly int blueRange = 8;
        private static readonly int greens = (7 * 16);
        private static readonly int greenRange = 16;
        private static readonly int grays = (6 * 16);
        private static readonly int grayRange = 16;
        private static readonly int browns = (4 * 16);
        private static readonly int brownRange = 16;
        private static readonly int yellows = (256 - 32 + 7);
        private static readonly int yellowRange = 1;
        private static readonly int black = 0;
        private static readonly int white = (256 - 47);

        // Automap colors
        private static readonly int background = black;
        private static readonly int yourColors = white;
        private static readonly int yourRange = 0;
        private static readonly int wallColors = reds;
        private static readonly int wallRange = redRange;
        private static readonly int tsWallColors = grays;
        private static readonly int tsWallRange = grayRange;
        private static readonly int fdWallColors = browns;
        private static readonly int fdWallRange = brownRange;
        private static readonly int cdWallColors = yellows;
        private static readonly int cdWallRange = yellowRange;
        private static readonly int thingColors = greens;
        private static readonly int thingRange = greenRange;
        private static readonly int secretWallColors = wallColors;
        private static readonly int secretWallRange = wallRange;

        private DrawScreen screen;

        private int amWidth;
        private int amHeight;
        private float ppu;

        private float minX;
        private float maxX;
        private float width;
        private float minY;
        private float maxY;
        private float height;

        private float viewX;
        private float viewY;
        private float zoom;

        public AutoMapRenderer(DrawScreen screen)
        {
            this.screen = screen;

            var scale = screen.Width / 320;

            amWidth = screen.Width;
            amHeight = screen.Height - scale * StatusBar.Height;
            ppu = (float)scale / 16;
        }

        public void Render(Player player)
        {
            screen.FillRect(0, 0, amWidth, amHeight, 0);

            var world = player.Mobj.World;
            var am = world.AutoMap;

            minX = am.MinX.ToFloat();
            maxX = am.MaxX.ToFloat();
            width = maxX - minX;
            minY = am.MinY.ToFloat();
            maxY = am.MaxY.ToFloat();
            height = maxY - minY;

            viewX = am.ViewX.ToFloat();
            viewY = am.ViewY.ToFloat();
            zoom = am.Zoom.ToFloat();

            foreach (var line in world.Map.Lines)
            {
                var v1 = ToScreenPos(line.Vertex1);
                var v2 = ToScreenPos(line.Vertex2);

                var cheating = am.State != AutoMapState.None;

                if (cheating || (line.Flags & LineFlags.Mapped) != 0)
                {
                    if ((line.Flags & LineFlags.DontDraw) != 0 && !cheating)
                    {
                        continue;
                    }

                    if (line.BackSector == null)
                    {
                        screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, wallColors);
                    }
                    else
                    {
                        if (line.Special == (LineSpecial)39)
                        {
                            // Teleporters.
                            screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, wallColors + wallRange / 2);
                        }
                        else if ((line.Flags & LineFlags.Secret) != 0)
                        {
                            // Secret door.
                            if (cheating)
                            {
                                screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, secretWallColors);
                            }
                            else
                            {
                                screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, wallColors);
                            }
                        }
                        else if (line.BackSector.FloorHeight != line.FrontSector.FloorHeight)
                        {
                            // Floor level change.
                            screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, fdWallColors);
                        }
                        else if (line.BackSector.CeilingHeight != line.FrontSector.CeilingHeight)
                        {
                            // Ceiling level change.
                            screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, cdWallColors);
                        }
                        else if (cheating)
                        {
                            screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, tsWallColors);
                        }
                    }
                }
                else if (player.Powers[(int)PowerType.AllMap] > 0)
                {
                    if ((line.Flags & LineFlags.DontDraw) == 0)
                    {
                        screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, grays + 3);
                    }
                }
            }
        }

        private DrawPos ToScreenPos(Vertex v)
        {
            var x = zoom * ppu * (v.X.ToFloat() - viewX) + amWidth / 2;
            var y = -zoom * ppu * (v.Y.ToFloat() - viewY) + amHeight / 2;
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
