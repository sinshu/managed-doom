using System;
using System.Security.Cryptography.X509Certificates;

namespace ManagedDoom.SoftwareRendering
{
    public sealed class AutoMapRenderer
    {
        // For use if I do walls with outsides / insides.
        private static readonly int Reds = (256 - 5 * 16);
        private static readonly int RedRange = 16;
        private static readonly int Blues = (256 - 4 * 16 + 8);
        private static readonly int BlueRange = 8;
        private static readonly int Greens = (7 * 16);
        private static readonly int GreenRange = 16;
        private static readonly int Grays = (6 * 16);
        private static readonly int GrayRange = 16;
        private static readonly int Browns = (4 * 16);
        private static readonly int BrownRange = 16;
        private static readonly int Yellows = (256 - 32 + 7);
        private static readonly int YellowRange = 1;
        private static readonly int Black = 0;
        private static readonly int White = (256 - 47);

        // Automap colors
        private static readonly int Background = Black;
        private static readonly int YourColors = White;
        private static readonly int YourRange = 0;
        private static readonly int WallColors = Reds;
        private static readonly int WallRange = RedRange;
        private static readonly int TsWallColors = Grays;
        private static readonly int TsWallRange = GrayRange;
        private static readonly int FdWallColors = Browns;
        private static readonly int FdWallRange = BrownRange;
        private static readonly int CdWallColors = Yellows;
        private static readonly int CdWallRange = YellowRange;
        private static readonly int ThingColors = Greens;
        private static readonly int ThingRange = GreenRange;
        private static readonly int SecretWallColors = WallColors;
        private static readonly int SecretWallRange = WallRange;

        private DrawScreen screen;

        private float minX;
        private float maxX;
        private float width;
        private float minY;
        private float maxY;
        private float height;

        public AutoMapRenderer(DrawScreen screen)
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

                var cheating = am.State != AutoMapState.None;

                if (cheating || (line.Flags & LineFlags.Mapped) != 0)
                {
                    if ((line.Flags & LineFlags.DontDraw) != 0 && !cheating)
                    {
                        continue;
                    }

                    if (line.BackSector == null)
                    {
                        screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, WallColors);
                    }
                    else
                    {
                        if (line.Special == (LineSpecial)39)
                        {
                            // Teleporters.
                            screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, WallColors + WallRange / 2);
                        }
                        else if ((line.Flags & LineFlags.Secret) != 0)
                        {
                            // Secret door.
                            if (cheating)
                            {
                                screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, SecretWallColors);
                            }
                            else
                            {
                                screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, WallColors);
                            }
                        }
                        else if (line.BackSector.FloorHeight != line.FrontSector.FloorHeight)
                        {
                            // Floor level change.
                            screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, FdWallColors);
                        }
                        else if (line.BackSector.CeilingHeight != line.FrontSector.CeilingHeight)
                        {
                            // Ceiling level change.
                            screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, CdWallColors);
                        }
                        else if (cheating)
                        {
                            screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, TsWallColors);
                        }
                    }
                }
                else if (player.Powers[(int)PowerType.AllMap] > 0)
                {
                    if ((line.Flags & LineFlags.DontDraw) == 0)
                    {
                        screen.DrawLine(v1.X, v1.Y, v2.X, v2.Y, Grays + 3);
                    }
                }
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
