using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace ManagedDoom
{
    public sealed class SoftwareRenderer
    {
        private const int FineFieldOfView = 2048;
        private const int HeightBits = 12;
        private const int HeightUnit = 1 << HeightBits;

        private const int LightLevels = 16;
        private const int LightSegShift = 4;
        private const int MaxLightScale = 48;
        private const int LightScaleShift = 12;
        private const int MaxLightZ = 128;
        private const int LightZShift = 20;
        private const int NumColorMaps = 32;

        private TextureLookup textures;
        private FlatLookup flats;



        private int screenWidth;
        private int screenHeight;
        private byte[] screenData;
        private byte[] glTextureData;
        private byte[] palette;

        private int windowX;
        private int windowY;
        private int windowWidth;
        private int windowHeight;
        private int centerX;
        private int centerY;
        private Fixed centerXFrac;
        private Fixed centerYFrac;
        private Fixed projection;

        private short[] upperClip;
        private short[] lowerClip;

        private int newEnd;
        private ClipRange[] solidSegs;

        private int clipDataLength;
        private short[] clipData;

        private int drawSegCount;
        private VisSeg[] drawSegs;


        // Texture mapping
        private int[] angleToX;
        private Angle[] xToAngle;
        private Angle clipAngle;
        private Angle clipAngle2;



        // Flats
        private Fixed[] flatYSlope;
        private Fixed[] flatDistScale;
        private Fixed flatBaseXScale;
        private Fixed flatBaseYScale;

        private int ceilingPrevX;
        private int ceilingPrevY1;
        private int ceilingPrevY2;
        private Fixed ceilingPrevHeight;
        private Fixed[] ceilingXFrac;
        private Fixed[] ceilingYFrac;
        private Fixed[] ceilingXStep;
        private Fixed[] ceilingYStep;
        private byte[][] ceilingLight;

        private int floorPrevX;
        private int floorPrevY1;
        private int floorPrevY2;
        private Fixed floorPrevHeight;
        private Fixed[] floorXFrac;
        private Fixed[] floorYFrac;
        private Fixed[] floorXStep;
        private Fixed[] floorYStep;
        private byte[][] floorLight;




        // Color map
        private byte[][] colorMap;
        private byte[][][] scaleLight;
        private byte[][][] zLight;



        private Fixed cameraX;
        private Fixed cameraY;
        private Fixed cameraZ;
        private Angle cameraAngle;



        public SoftwareRenderer(Wad wad)
        {
            textures = new TextureLookup(wad);
            flats = new FlatLookup(wad);

            screenWidth = 320;
            screenHeight = 200;
            screenData = new byte[screenWidth * screenHeight];
            glTextureData = new byte[4 * screenWidth * screenHeight];
            palette = wad.ReadLump("PLAYPAL");

            SetViewWindow(0, 0, 320, 200);
            InitTextureMapping();
            InitFlat();
            InitColorMap(wad);
        }

        private byte[][] ReadColorMap(Wad wad)
        {
            var data = wad.ReadLump("COLORMAP");
            var num = data.Length / 256;
            var map = new byte[num][];
            for (var i = 0; i < num; i++)
            {
                map[i] = new byte[256];
                var offset = 256 * i;
                for (var c = 0; c < 256; c++)
                {
                    map[i][c] = data[offset + c];
                }
            }
            return map;
        }

        public void SetViewWindow(int x, int y, int width, int height)
        {
            windowX = x;
            windowY = y;
            windowWidth = width;
            windowHeight = height;

            centerX = windowWidth / 2;
            centerY = windowHeight / 2;
            centerXFrac = Fixed.FromInt(centerX);
            centerYFrac = Fixed.FromInt(centerY);
            projection = centerXFrac;

            upperClip = new short[width];
            lowerClip = new short[width];

            newEnd = 0;
            solidSegs = new ClipRange[1024];
            for (var i = 0; i < solidSegs.Length; i++)
            {
                solidSegs[i] = new ClipRange();
            }

            clipDataLength = 0;
            clipData = new short[128 * width];

            drawSegCount = 0;
            drawSegs = new VisSeg[1024];
            for (var i = 0; i < drawSegs.Length; i++)
            {
                drawSegs[i] = new VisSeg();
            }
        }

        public void InitTextureMapping()
        {
            var focalLength = centerXFrac / Trig.Tan(Trig.FineAngleCount / 4 + FineFieldOfView / 2);

            angleToX = new int[Trig.FineAngleCount / 2];
            for (var i = 0; i < Trig.FineAngleCount / 2; i++)
            {
                int t;

                if (Trig.Tan(i) > Fixed.FromInt(2))
                {
                    t = -1;
                }
                else if (Trig.Tan(i) < Fixed.FromInt(-2))
                {
                    t = windowWidth + 1;
                }
                else
                {
                    var u = Trig.Tan(i) * focalLength;
                    t = (centerXFrac.Data - u.Data + Fixed.FracUnit - 1) >> Fixed.FracBits;

                    if (t < -1)
                    {
                        t = -1;
                    }
                    else if (t > windowWidth + 1)
                    {
                        t = windowWidth + 1;
                    }
                }

                angleToX[i] = t;
            }

            xToAngle = new Angle[windowWidth];
            for (var x = 0; x < windowWidth; x++)
            {
                var i = 0;
                while (angleToX[i] > x)
                {
                    i++;
                }
                xToAngle[x] = new Angle((uint)(i << Trig.AngleToFineShift)) - Angle.Ang90;
            }

            for (var i = 0; i < Trig.FineAngleCount / 2; i++)
            {
                if (angleToX[i] == -1)
                {
                    angleToX[i] = 0;
                }
                else if (angleToX[i] == windowWidth + 1)
                {
                    angleToX[i] = windowWidth;
                }
            }

            clipAngle = xToAngle[0];
            clipAngle2 = new Angle(2 * clipAngle.Data);
        }

        public void InitFlat()
        {
            // planes

            flatYSlope = new Fixed[windowHeight];
            for (int i = 0; i < windowHeight; i++)
            {
                //dy = ((i-viewheight/2)<<FRACBITS)+FRACUNIT/2;
                var dy = Fixed.FromInt(i - windowHeight / 2) + Fixed.One / 2;

                //dy = abs(dy);
                dy = Fixed.Abs(dy);

                //yslope[i] = FixedDiv ( (viewwidth<<detailshift)/2*FRACUNIT, dy);
                flatYSlope[i] = Fixed.FromInt(windowWidth / 2) / dy;
            }

            flatDistScale = new Fixed[windowWidth];
            for (var i = 0; i < windowWidth; i++)
            {
                //cosadj = abs(finecosine[xtoviewangle[i] >> ANGLETOFINESHIFT]);
                var cosadj = Fixed.Abs(Trig.Cos(xToAngle[i]));

                //distscale[i] = FixedDiv(FRACUNIT, cosadj);
                flatDistScale[i] = Fixed.One / cosadj;
            }

            ceilingXFrac = new Fixed[windowHeight];
            ceilingYFrac = new Fixed[windowHeight];
            ceilingXStep = new Fixed[windowHeight];
            ceilingYStep = new Fixed[windowHeight];
            ceilingLight = new byte[windowHeight][];
            floorXFrac = new Fixed[windowHeight];
            floorYFrac = new Fixed[windowHeight];
            floorXStep = new Fixed[windowHeight];
            floorYStep = new Fixed[windowHeight];
            floorLight = new byte[windowHeight][];
        }

        private void InitColorMap(Wad wad)
        {
            var distMap = 2;

            colorMap = ReadColorMap(wad);



            // Calculate the light levels to use
            //  for each level / scale combination.
            scaleLight = new byte[LightLevels][][];
            for (var i = 0; i < LightLevels; i++)
            {
                scaleLight[i] = new byte[MaxLightScale][];
                var startmap = ((LightLevels - 1 - i) * 2) * NumColorMaps / LightLevels;
                for (var j = 0; j < MaxLightScale; j++)
                {
                    var level = startmap - j * screenWidth / windowWidth / distMap;

                    if (level < 0)
                    {
                        level = 0;
                    }

                    if (level >= NumColorMaps)
                    {
                        level = NumColorMaps - 1;
                    }

                    scaleLight[i][j] = colorMap[level];
                }
            }

            // Calculate the light levels to use
            //  for each level / distance combination.
            zLight = new byte[LightLevels][][];
            for (var i = 0; i < LightLevels; i++)
            {
                zLight[i] = new byte[MaxLightZ][];
                var startmap = ((LightLevels - 1 - i) * 2) * NumColorMaps / LightLevels;
                for (var j = 0; j < MaxLightZ; j++)
                {
                    var scale = Fixed.FromInt(screenWidth / 2) / new Fixed((j + 1) << LightZShift);
                    scale = new Fixed(scale.Data >> LightScaleShift);
                    var level = startmap - scale.Data / distMap;

                    if (level < 0)
                    {
                        level = 0;
                    }

                    if (level >= NumColorMaps)
                    {
                        level = NumColorMaps - 1;
                    }

                    zLight[i][j] = colorMap[level];
                }
            }
        }



        public void ClearClipData()
        {
            for (var x = 0; x < upperClip.Length; x++)
            {
                upperClip[x] = -1;
            }
            for (var x = 0; x < lowerClip.Length; x++)
            {
                lowerClip[x] = (short)windowHeight;
            }
        }

        public void ClearClipSegs()
        {
            solidSegs[0].First = -0x7fffffff;
            solidSegs[0].Last = -1;
            solidSegs[1].First = windowWidth;
            solidSegs[1].Last = 0x7fffffff;
            newEnd = 2;
        }

        public void ClearDrawSegs()
        {
            clipDataLength = 0;
            drawSegCount = 0;
        }

        public void ClearFlat()
        {
            // angle = (viewangle-ANG90)>>ANGLETOFINESHIFT;
            var angle = cameraAngle - Angle.Ang90;

            // basexscale = FixedDiv (finecosine[angle],centerxfrac);
            flatBaseXScale = Trig.Cos(angle) / centerXFrac;

            // baseyscale = -FixedDiv (finesine[angle],centerxfrac);
            flatBaseYScale = -(Trig.Sin(angle) / centerXFrac);

            ceilingPrevX = int.MaxValue;
            floorPrevX = int.MaxValue;
        }

        public Fixed ScaleFromGlobalAngle(Angle visAngle, Angle viewAngle, Angle rwNormal, Fixed rwDistance)
        {
            var a = Trig.Sin(Angle.Ang90 + (visAngle - viewAngle));
            var b = Trig.Sin(Angle.Ang90 + (visAngle - rwNormal));
            var num = projection * b;
            var den = rwDistance * a;

            Fixed scale;
            if (den.Data > num.Data >> 16)
            {
                scale = num / den;

                if (scale > Fixed.FromInt(64))
                {
                    scale = Fixed.FromInt(64);
                }
                else if (scale.Data < 256)
                {
                    scale = new Fixed(256);
                }
            }
            else
            {
                scale = Fixed.FromInt(64);
            }

            return scale;
        }

        public void ClearData()
        {
            for (var i = 0; i < screenData.Length; i++)
            {
                screenData[i] = 252;
            }
        }

        public void Render(World world)
        {
            var x = world.ViewX;
            var y = world.ViewY;
            var z = world.ViewZ;
            var angle = world.ViewAngle;
            Go(x, y, z, angle, world);

            for (var i = 0; i < screenData.Length; i++)
            {
                var po = 3 * screenData[i];
                var offset = 4 * i;
                glTextureData[offset + 0] = palette[po + 0];
                glTextureData[offset + 1] = palette[po + 1];
                glTextureData[offset + 2] = palette[po + 2];
                glTextureData[offset + 3] = 255;
            }
        }

        public void Go(Fixed viewX, Fixed viewY, Fixed viewZ, Angle viewAngle,World world)
        {
            cameraX = viewX;
            cameraY = viewY;
            cameraZ = viewZ;
            cameraAngle = viewAngle;

            ClearClipSegs();
            ClearClipData();
            ClearDrawSegs();
            ClearFlat();

            GoSub(world, world.Map.Nodes.Last());

            DrawMasked();
        }

        public void GoSub(World world, Node node)
        {
            var map = world.Map;
            if (Geometry.PointOnSide(cameraX, cameraY, node) == 0)
            {
                if (Node.IsSubsector(node.Children0))
                {
                    var subsector = map.Subsectors[Node.GetSubsector(node.Children0)];
                    for (var i = 0; i < subsector.SegCount; i++)
                    {
                        DrawSeg(map.Segs[subsector.FirstSeg + i]);
                    }
                }
                else
                {
                    var next = map.Nodes[node.Children0];
                    GoSub(world, next);
                }

                if (Node.IsSubsector(node.Children1))
                {
                    var subsector = map.Subsectors[Node.GetSubsector(node.Children1)];
                    for (var i = 0; i < subsector.SegCount; i++)
                    {
                        DrawSeg(map.Segs[subsector.FirstSeg + i]);
                    }
                }
                else
                {
                    var next = map.Nodes[node.Children1];
                    GoSub(world, next);
                }
            }
            else
            {
                if (Node.IsSubsector(node.Children1))
                {
                    var subsector = map.Subsectors[Node.GetSubsector(node.Children1)];
                    for (var i = 0; i < subsector.SegCount; i++)
                    {
                        DrawSeg(map.Segs[subsector.FirstSeg + i]);
                    }
                }
                else
                {
                    var next = map.Nodes[node.Children1];
                    GoSub(world, next);
                }

                if (Node.IsSubsector(node.Children0))
                {
                    var subsector = map.Subsectors[Node.GetSubsector(node.Children0)];
                    for (var i = 0; i < subsector.SegCount; i++)
                    {
                        DrawSeg(map.Segs[subsector.FirstSeg + i]);
                    }
                }
                else
                {
                    var next = map.Nodes[node.Children0];
                    GoSub(world, next);
                }
            }
        }

        public void DrawSeg(Seg seg)
        {
            var angle1 = Geometry.PointToAngle(cameraX, cameraY, seg.Vertex1.X, seg.Vertex1.Y);
            var angle2 = Geometry.PointToAngle(cameraX, cameraY, seg.Vertex2.X, seg.Vertex2.Y);

            var span = angle1 - angle2;

            if (span >= Angle.Ang180)
            {
                return;
            }

            var rwAngle1 = angle1;
            angle1 -= cameraAngle;
            angle2 -= cameraAngle;

            var tspan = angle1 + clipAngle;
            if (tspan > clipAngle2)
            {
                tspan -= clipAngle2;

                if (tspan >= span)
                {
                    return;
                }

                angle1 = clipAngle;
            }

            tspan = clipAngle - angle2;
            if (tspan > clipAngle2)
            {
                tspan -= clipAngle2;

                if (tspan >= span)
                {
                    return;
                }

                angle2 = -clipAngle;
            }

            // The seg is in the view range,
            // but not necessarily visible.
            var x1 = angleToX[(angle1 + Angle.Ang90).Data >> Trig.AngleToFineShift];
            var x2 = angleToX[(angle2 + Angle.Ang90).Data >> Trig.AngleToFineShift];

            // Does not cross a pixel?
            if (x1 == x2)
            {
                return;
            }

            // Single sided line?
            if (seg.BackSector == null)
            {
                DrawSolidWall(seg, rwAngle1, x1, x2 - 1);
                return;
            }

            // Closed door.
            if (seg.BackSector.CeilingHeight <= seg.FrontSector.FloorHeight
                || seg.BackSector.FloorHeight >= seg.FrontSector.CeilingHeight)
            {
                DrawSolidWall(seg, rwAngle1, x1, x2 - 1);
                return;
            }

            // Window.
            DrawPassWall(seg, rwAngle1, x1, x2 - 1);
        }

        public void DrawSolidWall(Seg seg, Angle rwAngle1, int x1, int x2)
        {
            int next;
            int start;

            // Find the first range that touches the range
            //  (adjacent pixels are touching).
            start = 0;
            while (solidSegs[start].Last < x1 - 1)
            {
                start++;
            }

            if (x1 < solidSegs[start].First)
            {
                if (x2 < solidSegs[start].First - 1)
                {
                    // Post is entirely visible (above start),
                    //  so insert a new clippost.
                    DrawSolidWallRange(seg, rwAngle1, x1, x2);
                    next = newEnd;
                    newEnd++;

                    while (next != start)
                    {
                        solidSegs[next].CopyFrom(solidSegs[next - 1]);
                        next--;
                    }
                    solidSegs[next].First = x1;
                    solidSegs[next].Last = x2;
                    return;
                }

                // There is a fragment above *start.
                DrawSolidWallRange(seg, rwAngle1, x1, solidSegs[start].First - 1);
                // Now adjust the clip size.
                solidSegs[start].First = x1;
            }

            // Bottom contained in start?
            if (x2 <= solidSegs[start].Last)
            {
                return;
            }

            next = start;
            while (x2 >= solidSegs[next + 1].First - 1)
            {
                // There is a fragment between two posts.
                DrawSolidWallRange(seg, rwAngle1, solidSegs[next].Last + 1, solidSegs[next + 1].First - 1);
                next++;

                if (x2 <= solidSegs[next].Last)
                {
                    // Bottom is contained in next.
                    // Adjust the clip size.
                    solidSegs[start].Last = solidSegs[next].Last;
                    goto crunch;
                }
            }

            // There is a fragment after *next.
            DrawSolidWallRange(seg, rwAngle1, solidSegs[next].Last + 1, x2);
            // Adjust the clip size.
            solidSegs[start].Last = x2;

        // Remove start+1 to next from the clip list,
        // because start now covers their area.
        crunch:
            if (next == start)
            {
                // Post just extended past the bottom of one post.
                return;
            }

            while (next++ != newEnd)
            {
                // Remove a post.
                solidSegs[++start].CopyFrom(solidSegs[next]);
            }

            newEnd = start + 1;
        }

        public void DrawPassWall(Seg seg, Angle rwAngle1, int x1, int x2)
        {
            int start;

            // Find the first range that touches the range
            //  (adjacent pixels are touching).
            start = 0;
            while (solidSegs[start].Last < x1 - 1)
            {
                start++;
            }

            if (x1 < solidSegs[start].First)
            {
                if (x2 < solidSegs[start].First - 1)
                {
                    // Post is entirely visible (above start).
                    DrawPassWallRange(seg, rwAngle1, x1, x2);
                    return;
                }

                // There is a fragment above *start.
                DrawPassWallRange(seg, rwAngle1, x1, solidSegs[start].First - 1);
            }

            // Bottom contained in start?
            if (x2 <= solidSegs[start].Last)
            {
                return;
            }

            while (x2 >= solidSegs[start + 1].First - 1)
            {
                // There is a fragment between two posts.
                DrawPassWallRange(seg, rwAngle1, solidSegs[start].Last + 1, solidSegs[start + 1].First - 1);
                start++;

                if (x2 <= solidSegs[start].Last)
                {
                    return;
                }
            }

            // There is a fragment after *next.
            DrawPassWallRange(seg, rwAngle1, solidSegs[start].Last + 1, x2);
        }

        public void DrawSolidWallRange(Seg seg, Angle rwAngle1, int x1, int x2)
        {
            if (seg.BackSector != null)
            {
                DrawPassWallRange(seg, rwAngle1, x1, x2);
                return;
            }

            var rwNormalAngle = seg.Angle + Angle.Ang90;

            var offsetAngle = Angle.Abs(rwNormalAngle - rwAngle1);
            if (offsetAngle > Angle.Ang90)
            {
                offsetAngle = Angle.Ang90;
            }

            var distAngle = Angle.Ang90 - offsetAngle;

            var hyp = Geometry.PointToDist(cameraX, cameraY, seg.Vertex1.X, seg.Vertex1.Y);

            var rwDistance = hyp * Trig.Sin(distAngle);

            var rwScale = ScaleFromGlobalAngle(cameraAngle + xToAngle[x1], cameraAngle, rwNormalAngle, rwDistance);

            Fixed scale1 = rwScale;
            Fixed scale2;
            Fixed rwScaleStep;
            if (x2 > x1)
            {
                scale2 = ScaleFromGlobalAngle(cameraAngle + xToAngle[x2], cameraAngle, rwNormalAngle, rwDistance);
                rwScaleStep = (scale2 - rwScale) / (x2 - x1);
            }
            else
            {
                scale2 = scale1;
                rwScaleStep = Fixed.Zero;
            }

            var drawSeg = drawSegs[drawSegCount];
            drawSeg.Seg = seg;
            drawSeg.X1 = x1;
            drawSeg.X2 = x2;
            drawSeg.Scale1 = scale1;
            drawSeg.Scale2 = scale2;
            drawSeg.ScaleStep = rwScaleStep;

            var worldFrontY1 = seg.FrontSector.CeilingHeight - cameraZ;
            var worldFrontY2 = seg.FrontSector.FloorHeight - cameraZ;

            var wallTexture = textures[seg.SideDef.MiddleTexture];
            var wallWidthMask = wallTexture.Width - 1;

            Fixed rwMidTextureMid;
            if ((seg.LineDef.Flags & LineFlags.DontPegBottom) != 0)
            {
                var vTop = seg.FrontSector.FloorHeight + Fixed.FromInt(wallTexture.Height);
                rwMidTextureMid = vTop - cameraZ;
            }
            else
            {
                rwMidTextureMid = worldFrontY1;
            }
            rwMidTextureMid += seg.SideDef.RowOffset;

            offsetAngle = rwNormalAngle - rwAngle1;
            if (offsetAngle > Angle.Ang180)
            {
                offsetAngle = -offsetAngle;
            }
            if (offsetAngle > Angle.Ang90)
            {
                offsetAngle = Angle.Ang90;
            }

            var rwOffset = hyp * Trig.Sin(offsetAngle);
            if (rwNormalAngle - rwAngle1 < Angle.Ang180)
            {
                rwOffset = -rwOffset;
            }
            rwOffset += seg.Offset + seg.SideDef.TextureOffset;

            var rwCenterAngle = Angle.Ang90 + cameraAngle - rwNormalAngle;

            worldFrontY1 = new Fixed(worldFrontY1.Data >> 4);
            worldFrontY2 = new Fixed(worldFrontY2.Data >> 4);

            var wallY1Frac = new Fixed(centerYFrac.Data >> 4) - worldFrontY1 * rwScale;
            var wallY1Step = -rwScaleStep * worldFrontY1;
            var wallY2Frac = new Fixed(centerYFrac.Data >> 4) - worldFrontY2 * rwScale;
            var wallY2Step = -rwScaleStep * worldFrontY2;

            var drawWall = seg.SideDef.MiddleTexture != 0;
            var drawCeiling = worldFrontY1 > Fixed.Zero || seg.FrontSector.CeilingFlat == flats.SkyFlatNumber;
            var drawFloor = worldFrontY2 < Fixed.Zero;

            var wallLightLevel = (seg.FrontSector.LightLevel >> LightSegShift); // + extralight;

            if (seg.Vertex1.Y == seg.Vertex2.Y)
            {
                wallLightLevel--;
            }
            else if (seg.Vertex1.X == seg.Vertex2.X)
            {
                wallLightLevel++;
            }

            byte[][] wallLights;
            if (wallLightLevel < 0)
            {
                wallLights = scaleLight[0];
            }
            else if (wallLightLevel >= LightLevels)
            {
                wallLights = scaleLight[LightLevels - 1];
            }
            else
            {
                wallLights = scaleLight[wallLightLevel];
            }

            for (var x = x1; x <= x2; x++)
            {
                var drawWallY1 = (wallY1Frac.Data + HeightUnit - 1) >> HeightBits;
                var drawWallY2 = wallY2Frac.Data >> HeightBits;

                if (drawCeiling)
                {
                    var cy1 = upperClip[x] + 1;
                    var cy2 = Math.Min(drawWallY1 - 1, lowerClip[x] - 1);
                    DrawCeilingColumn(flats[seg.FrontSector.CeilingFlat], seg.FrontSector.CeilingHeight, seg.FrontSector.LightLevel, x, cy1, cy2);
                }

                if (drawWall)
                {
                    var wy1 = Math.Max(drawWallY1, upperClip[x] + 1);
                    var wy2 = Math.Min(drawWallY2, lowerClip[x] - 1);

                    var angle = rwCenterAngle + xToAngle[x];
                    angle = new Angle(angle.Data & 0x7FFFFFFF);

                    var textureColumn = (rwOffset - Trig.Tan(angle) * rwDistance).Data >> Fixed.FracBits;
                    var source = wallTexture.Composite.Columns[textureColumn & wallWidthMask][0];

                    var lightScale = rwScale.Data >> LightScaleShift;
                    if (lightScale >= MaxLightScale)
                    {
                        lightScale = MaxLightScale - 1;
                    }

                    var iScale = new Fixed((int)(0xffffffffu / (uint)rwScale.Data));
                    DrawColumn(source, wallLights[lightScale], x, wy1, wy2, iScale, rwMidTextureMid);
                }

                if (drawFloor)
                {
                    var fy1 = Math.Max(drawWallY2 + 1, upperClip[x] + 1);
                    var fy2 = lowerClip[x] - 1;
                    DrawFloorColumn(flats[seg.FrontSector.FloorFlat], seg.FrontSector.FloorHeight, seg.FrontSector.LightLevel, x, fy1, fy2);
                }

                rwScale += rwScaleStep;
                wallY1Frac += wallY1Step;
                wallY2Frac += wallY2Step;
            }

            drawSegCount++;
        }

        public void DrawPassWallRange(Seg seg, Angle rwAngle1, int x1, int x2)
        {
            var rwNormalAngle = seg.Angle + Angle.Ang90;

            var offsetAngle = Angle.Abs(rwNormalAngle - rwAngle1);
            if (offsetAngle > Angle.Ang90)
            {
                offsetAngle = Angle.Ang90;
            }

            var distAngle = Angle.Ang90 - offsetAngle;

            var hyp = Geometry.PointToDist(cameraX, cameraY, seg.Vertex1.X, seg.Vertex1.Y);

            var rwDistance = hyp * Trig.Sin(distAngle);

            var rwScale = ScaleFromGlobalAngle(cameraAngle + xToAngle[x1], cameraAngle, rwNormalAngle, rwDistance);

            Fixed scale1 = rwScale;
            Fixed scale2;
            Fixed rwScaleStep;
            if (x2 > x1)
            {
                scale2 = ScaleFromGlobalAngle(cameraAngle + xToAngle[x2], cameraAngle, rwNormalAngle, rwDistance);
                rwScaleStep = (scale2 - rwScale) / (x2 - x1);
            }
            else
            {
                scale2 = scale1;
                rwScaleStep = Fixed.Zero;
            }

            var drawSeg = drawSegs[drawSegCount];
            drawSeg.Seg = seg;
            drawSeg.X1 = x1;
            drawSeg.X2 = x2;
            drawSeg.Scale1 = scale1;
            drawSeg.Scale2 = scale2;
            drawSeg.ScaleStep = rwScaleStep;

            var worldFrontY1 = seg.FrontSector.CeilingHeight - cameraZ;
            var worldFrontY2 = seg.FrontSector.FloorHeight - cameraZ;
            var worldBackY1 = seg.BackSector.CeilingHeight - cameraZ;
            var worldBackY2 = seg.BackSector.FloorHeight - cameraZ;

            if (seg.FrontSector.CeilingFlat == flats.SkyFlatNumber
                && seg.BackSector.CeilingFlat == flats.SkyFlatNumber)
            {
                worldFrontY1 = worldBackY1;
            }

            var upperWallTexture = textures[seg.SideDef.TopTexture];
            var upperWallWidthMask = upperWallTexture.Width - 1;
            var lowerWallTexture = textures[seg.SideDef.BottomTexture];
            var lowerWallWidthMask = lowerWallTexture.Width - 1;

            Fixed rwTopTextureMid;
            if ((seg.LineDef.Flags & LineFlags.DontPegTop) != 0)
            {
                rwTopTextureMid = worldFrontY1;
            }
            else
            {
                var vTop = seg.BackSector.CeilingHeight + Fixed.FromInt(upperWallTexture.Height);
                rwTopTextureMid = vTop - cameraZ;
            }
            rwTopTextureMid += seg.SideDef.RowOffset;

            Fixed rwBottomTextureMid;
            if ((seg.LineDef.Flags & LineFlags.DontPegBottom) != 0)
            {
                rwBottomTextureMid = worldFrontY1;
            }
            else
            {
                rwBottomTextureMid = worldBackY2;
            }
            rwBottomTextureMid += seg.SideDef.RowOffset;

            offsetAngle = rwNormalAngle - rwAngle1;
            if (offsetAngle > Angle.Ang180)
            {
                offsetAngle = -offsetAngle;
            }
            if (offsetAngle > Angle.Ang90)
            {
                offsetAngle = Angle.Ang90;
            }

            var rwOffset = hyp * Trig.Sin(offsetAngle);
            if (rwNormalAngle - rwAngle1 < Angle.Ang180)
            {
                rwOffset = -rwOffset;
            }
            rwOffset += seg.Offset + seg.SideDef.TextureOffset;

            var rwCenterAngle = Angle.Ang90 + cameraAngle - rwNormalAngle;

            worldFrontY1 = new Fixed(worldFrontY1.Data >> 4);
            worldFrontY2 = new Fixed(worldFrontY2.Data >> 4);
            worldBackY1 = new Fixed(worldBackY1.Data >> 4);
            worldBackY2 = new Fixed(worldBackY2.Data >> 4);

            var skipAllowed = worldFrontY1 != worldFrontY2
                && worldFrontY1 == worldBackY1
                && worldFrontY2 == worldBackY2
                && seg.FrontSector.CeilingFlat == seg.BackSector.CeilingFlat
                && seg.FrontSector.FloorFlat == seg.BackSector.FloorFlat
                && seg.FrontSector.LightLevel == seg.BackSector.LightLevel;

            if (skipAllowed && seg.SideDef.MiddleTexture == 0)
            {
                return;
            }

            var upperWallY1Frac = new Fixed(centerYFrac.Data >> 4) - worldFrontY1 * rwScale;
            var upperWallY1Step = -rwScaleStep * worldFrontY1;

            Fixed upperWallY2Frac;
            Fixed upperWallY2Step;
            if (worldBackY1 > worldFrontY2)
            {
                upperWallY2Frac = new Fixed(centerYFrac.Data >> 4) - worldBackY1 * rwScale;
                upperWallY2Step = -rwScaleStep * worldBackY1;
            }
            else
            {
                upperWallY2Frac = new Fixed(centerYFrac.Data >> 4) - worldFrontY2 * rwScale;
                upperWallY2Step = -rwScaleStep * worldFrontY2;
            }

            Fixed lowerWallY1Frac;
            Fixed lowerWallY1Step;
            if (worldBackY2 < worldFrontY1)
            {
                lowerWallY1Frac = new Fixed(centerYFrac.Data >> 4) - worldBackY2 * rwScale;
                lowerWallY1Step = -rwScaleStep * worldBackY2;
            }
            else
            {
                lowerWallY1Frac = new Fixed(centerYFrac.Data >> 4) - worldFrontY1 * rwScale;
                lowerWallY1Step = -rwScaleStep * worldFrontY1;
            }

            var lowerWallY2Frac = new Fixed(centerYFrac.Data >> 4) - worldFrontY2 * rwScale;
            var lowerWallY2Step = -rwScaleStep * worldFrontY2;

            bool drawUpperWall = seg.SideDef.TopTexture != 0 && worldBackY1 < worldFrontY1;
            bool drawLowerWall = seg.SideDef.BottomTexture != 0 && worldBackY2 > worldFrontY2;
            bool drawCeiling = worldFrontY1 > Fixed.Zero || seg.FrontSector.CeilingFlat == flats.SkyFlatNumber;
            bool drawFloor = worldFrontY2 < Fixed.Zero;
            if (skipAllowed)
            {
                drawUpperWall = false;
                drawLowerWall = false;
                drawCeiling = false;
                drawFloor = false;
            }
            else
            {
                drawUpperWall = seg.SideDef.TopTexture != 0 && worldBackY1 < worldFrontY1;
                drawLowerWall = seg.SideDef.BottomTexture != 0 && worldBackY2 > worldFrontY2;
                drawCeiling = worldFrontY1 > Fixed.Zero || seg.FrontSector.CeilingFlat == flats.SkyFlatNumber;
                drawFloor = worldFrontY2 < Fixed.Zero;
            }
            var drawMiddleTexture = seg.SideDef.MiddleTexture != 0;

            var wallLightLevel = (seg.FrontSector.LightLevel >> LightSegShift); // + extraLight;

            if (seg.Vertex1.Y == seg.Vertex2.Y)
            {
                wallLightLevel--;
            }
            else if (seg.Vertex1.X == seg.Vertex2.X)
            {
                wallLightLevel++;
            }

            byte[][] wallLights;
            if (wallLightLevel < 0)
            {
                wallLights = scaleLight[0];
            }
            else if (wallLightLevel >= LightLevels)
            {
                wallLights = scaleLight[LightLevels - 1];
            }
            else
            {
                wallLights = scaleLight[wallLightLevel];
            }

            var maskedTextureColumn = 0;
            if (drawMiddleTexture)
            {
                var width = x2 - x1 + 1;
                maskedTextureColumn = clipDataLength - x1;
                drawSeg.MaskedTextureColumn = maskedTextureColumn;
                clipDataLength += width;
            }

            for (var x = x1; x <= x2; x++)
            {
                var drawMiddleWallY1 = (upperWallY1Frac.Data + HeightUnit - 1) >> HeightBits;
                var drawMiddleWallY2 = lowerWallY2Frac.Data >> HeightBits;

                Angle angle;
                var textureColumn = 0;
                var lightScale = 0;
                if (drawUpperWall || drawLowerWall || drawMiddleTexture)
                {
                    angle = rwCenterAngle + xToAngle[x];
                    angle = new Angle(angle.Data & 0x7FFFFFFF);
                    textureColumn = (rwOffset - Trig.Tan(angle) * rwDistance).Data >> Fixed.FracBits;

                    lightScale = rwScale.Data >> LightScaleShift;
                    if (lightScale >= MaxLightScale)
                    {
                        lightScale = MaxLightScale - 1;
                    }
                }

                if (drawUpperWall)
                {
                    var drawUpperWallY1 = (upperWallY1Frac.Data + HeightUnit - 1) >> HeightBits;
                    var drawUpperWallY2 = upperWallY2Frac.Data >> HeightBits;

                    if (drawCeiling)
                    {
                        var cy1 = upperClip[x] + 1;
                        var cy2 = Math.Min(drawMiddleWallY1 - 1, lowerClip[x] - 1);
                        DrawCeilingColumn(flats[seg.FrontSector.CeilingFlat], seg.FrontSector.CeilingHeight, seg.FrontSector.LightLevel, x, cy1, cy2);
                    }

                    var wy1 = Math.Max(drawUpperWallY1, upperClip[x] + 1);
                    var wy2 = Math.Min(drawUpperWallY2, lowerClip[x] - 1);
                    var source = upperWallTexture.Composite.Columns[textureColumn & upperWallWidthMask][0];
                    var iScale = new Fixed((int)(0xffffffffu / (uint)rwScale.Data));
                    DrawColumn(source, wallLights[lightScale], x, wy1, wy2, iScale, rwTopTextureMid);

                    if (upperClip[x] < wy2)
                    {
                        upperClip[x] = (short)wy2;
                    }
                }
                else if (drawCeiling)
                {
                    var cy1 = upperClip[x] + 1;
                    var cy2 = Math.Min(drawMiddleWallY1 - 1, lowerClip[x] - 1);
                    DrawCeilingColumn(flats[seg.FrontSector.CeilingFlat], seg.FrontSector.CeilingHeight, seg.FrontSector.LightLevel, x, cy1, cy2);

                    if (upperClip[x] < cy2)
                    {
                        upperClip[x] = (short)cy2;
                    }
                }

                if (drawLowerWall)
                {
                    var drawLowerWallY1 = (lowerWallY1Frac.Data + HeightUnit - 1) >> HeightBits;
                    var drawLowerWallY2 = lowerWallY2Frac.Data >> HeightBits;

                    var wy1 = Math.Max(drawLowerWallY1, upperClip[x] + 1);
                    var wy2 = Math.Min(drawLowerWallY2, lowerClip[x] - 1);
                    var source = lowerWallTexture.Composite.Columns[textureColumn & lowerWallWidthMask][0];
                    var iScale = new Fixed((int)(0xffffffffu / (uint)rwScale.Data));
                    DrawColumn(source, wallLights[lightScale], x, wy1, wy2, iScale, rwBottomTextureMid);

                    if (drawFloor)
                    {
                        var fy1 = Math.Max(drawMiddleWallY2 + 1, upperClip[x] + 1);
                        var fy2 = lowerClip[x] - 1;
                        DrawFloorColumn(flats[seg.FrontSector.FloorFlat], seg.FrontSector.FloorHeight, seg.FrontSector.LightLevel, x, fy1, fy2);
                    }

                    if (lowerClip[x] > wy1)
                    {
                        lowerClip[x] = (short)wy1;
                    }
                }
                else if (drawFloor)
                {
                    var fy1 = Math.Max(drawMiddleWallY2 + 1, upperClip[x] + 1);
                    var fy2 = lowerClip[x] - 1;
                    DrawFloorColumn(flats[seg.FrontSector.FloorFlat], seg.FrontSector.FloorHeight, seg.FrontSector.LightLevel, x, fy1, fy2);

                    if (lowerClip[x] > drawMiddleWallY2 + 1)
                    {
                        lowerClip[x] = (short)fy1;
                    }
                }

                if (drawMiddleTexture)
                {
                    clipData[maskedTextureColumn + x] = (short)textureColumn;
                }

                rwScale += rwScaleStep;
                upperWallY1Frac += upperWallY1Step;
                upperWallY2Frac += upperWallY2Step;
                lowerWallY1Frac += lowerWallY1Step;
                lowerWallY2Frac += lowerWallY2Step;
            }

            {
                var width = x2 - x1 + 1;
                Array.Copy(upperClip, x1, clipData, clipDataLength, width);
                drawSeg.TopClip = clipDataLength - x1;
                clipDataLength += width;
                Array.Copy(lowerClip, x1, clipData, clipDataLength, width);
                drawSeg.BottomClip = clipDataLength - x1;
                clipDataLength += width;
            }

            drawSegCount++;
        }

        public void DrawMasked()
        {
            for (var i = drawSegCount - 1; i >= 0; i--)
            {
                var drawSeg = drawSegs[i];
                var seg = drawSeg.Seg;

                if ((seg.LineDef.Flags & LineFlags.TwoSided) != 0
                    && seg.SideDef.MiddleTexture != 0)
                {
                    var lightnum = (seg.FrontSector.LightLevel >> LightSegShift); // + extraLight;

                    if (seg.Vertex1.Y == seg.Vertex2.Y)
                    {
                        lightnum--;
                    }
                    else if (seg.Vertex1.X == seg.Vertex2.X)
                    {
                        lightnum++;
                    }

                    byte[][] walllights;
                    if (lightnum < 0)
                    {
                        walllights = scaleLight[0];
                    }
                    else if (lightnum >= LightLevels)
                    {
                        walllights = scaleLight[LightLevels - 1];
                    }
                    else
                    {
                        walllights = scaleLight[lightnum];
                    }

                    var wallTexture = textures[seg.SideDef.MiddleTexture];
                    var mask = wallTexture.Width - 1;

                    Fixed rwMidTextureMid;
                    if ((seg.LineDef.Flags & LineFlags.DontPegBottom) != 0)
                    {
                        rwMidTextureMid = seg.FrontSector.FloorHeight > seg.BackSector.FloorHeight
                            ? seg.FrontSector.FloorHeight : seg.BackSector.FloorHeight;
                        rwMidTextureMid = rwMidTextureMid + Fixed.FromInt(wallTexture.Height) - cameraZ;
                    }
                    else
                    {
                        rwMidTextureMid = seg.FrontSector.CeilingHeight < seg.BackSector.CeilingHeight
                            ? seg.FrontSector.CeilingHeight : seg.BackSector.CeilingHeight;
                        rwMidTextureMid = rwMidTextureMid - cameraZ;
                    }
                    rwMidTextureMid += seg.SideDef.RowOffset;

                    var rwScaleStep = drawSeg.ScaleStep;
                    var spryscale = drawSeg.Scale1;


                    for (var x = drawSeg.X1; x <= drawSeg.X2; x++)
                    {
                        var index = spryscale.Data >> LightScaleShift;
                        if (index >= MaxLightScale)
                        {
                            index = MaxLightScale - 1;
                        }

                        var col = clipData[drawSeg.MaskedTextureColumn + x];
                        var sprtopscreen = centerYFrac - rwMidTextureMid * spryscale;
                        var iScale = new Fixed((int)(0xffffffffu / (uint)spryscale.Data));
                        var ceilClip = clipData[drawSeg.TopClip + x];
                        var floorClip = clipData[drawSeg.BottomClip + x];
                        DrawMaskedColumn(
                            wallTexture.Composite.Columns[col & mask],
                            walllights[index],
                            x, iScale,
                            rwMidTextureMid,
                            spryscale,
                            sprtopscreen,
                            ceilClip,
                            floorClip);

                        spryscale += rwScaleStep;
                    }


                }
            }
        }

        public void DrawColumn(int color, int x, int y1, int y2)
        {
            if (y2 - y1 < 0)
            {
                return;
            }

            y1 = Math.Max(y1, 0);
            y2 = Math.Min(y2, windowHeight - 1);

            var pos = screenHeight * (windowX + x) + windowY + y1;
            for (var y = y1; y <= y2; y++)
            {
                screenData[pos] = (byte)color;
                pos++;
            }
        }

        public void DrawCeilingColumn(Flat flat, Fixed height, int lightlevel, int x, int y1, int y2)
        {
            if (y2 - y1 < 0)
            {
                return;
            }

            y1 = Math.Max(y1, 0);
            y2 = Math.Min(y2, screenHeight - 1);

            var planeHeight = Fixed.Abs(height - cameraZ);

            var light = lightlevel >> LightSegShift;
            //light = Math.Max(light, 0);
            light = Math.Min(light, LightLevels - 1);
            var planezlight = zLight[light];

            if (ceilingPrevX == x - 1 && ceilingPrevHeight == planeHeight && ceilingPrevY1 <= y2 && y1 <= ceilingPrevY2)
            {
                var p1 = Math.Max(y1, ceilingPrevY1);
                var p2 = Math.Min(y2, ceilingPrevY2);

                var pos = screenHeight * (windowX + x) + windowY + y1;

                for (var y = y1; y < p1; y++)
                {
                    var distance = planeHeight * flatYSlope[y];
                    ceilingXStep[y] = distance * flatBaseXScale;
                    ceilingYStep[y] = distance * flatBaseYScale;

                    var length = distance * flatDistScale[x];
                    var angle = cameraAngle + xToAngle[x];
                    var xFrac = cameraX + Trig.Cos(angle) * length;
                    var yFrac = -cameraY - Trig.Sin(angle) * length;
                    ceilingXFrac[y] = xFrac;
                    ceilingYFrac[y] = yFrac;

                    var index = distance.Data >> LightZShift;
                    if (index < 0)
                    {
                        index = 0;
                    }
                    if (index >= MaxLightZ)
                    {
                        index = MaxLightZ - 1;
                    }
                    var table = planezlight[index];
                    ceilingLight[y] = table;

                    var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                    screenData[pos] = table[flat.Data[spot]];
                    pos++;
                }

                for (var y = p1; y <= p2; y++)
                {
                    var xFrac = ceilingXFrac[y] + ceilingXStep[y];
                    var yFrac = ceilingYFrac[y] + ceilingYStep[y];

                    var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                    screenData[pos] = ceilingLight[y][flat.Data[spot]];
                    pos++;

                    ceilingXFrac[y] = xFrac;
                    ceilingYFrac[y] = yFrac;
                }

                for (var y = p2 + 1; y <= y2; y++)
                {
                    var distance = planeHeight * flatYSlope[y];
                    ceilingXStep[y] = distance * flatBaseXScale;
                    ceilingYStep[y] = distance * flatBaseYScale;

                    var length = distance * flatDistScale[x];
                    var angle = cameraAngle + xToAngle[x];
                    var xFrac = cameraX + Trig.Cos(angle) * length;
                    var yFrac = -cameraY - Trig.Sin(angle) * length;
                    ceilingXFrac[y] = xFrac;
                    ceilingYFrac[y] = yFrac;

                    var index = distance.Data >> LightZShift;
                    if (index < 0)
                    {
                        index = 0;
                    }
                    if (index >= MaxLightZ)
                    {
                        index = MaxLightZ - 1;
                    }
                    var table = planezlight[index];
                    ceilingLight[y] = table;

                    var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                    screenData[pos] = table[flat.Data[spot]];
                    pos++;
                }
            }
            else
            {
                var pos = screenHeight * (windowX + x) + windowY + y1;

                for (var y = y1; y <= y2; y++)
                {
                    var distance = planeHeight * flatYSlope[y];
                    ceilingXStep[y] = distance * flatBaseXScale;
                    ceilingYStep[y] = distance * flatBaseYScale;

                    var length = distance * flatDistScale[x];
                    var angle = cameraAngle + xToAngle[x];
                    var xFrac = cameraX + Trig.Cos(angle) * length;
                    var yFrac = -cameraY - Trig.Sin(angle) * length;
                    ceilingXFrac[y] = xFrac;
                    ceilingYFrac[y] = yFrac;

                    var index = distance.Data >> LightZShift;
                    if (index < 0)
                    {
                        index = 0;
                    }
                    if (index >= MaxLightZ)
                    {
                        index = MaxLightZ - 1;
                    }
                    var table = planezlight[index];
                    ceilingLight[y] = table;

                    var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                    screenData[pos] = table[flat.Data[spot]];
                    pos++;
                }
            }

            ceilingPrevX = x;
            ceilingPrevHeight = planeHeight;
            ceilingPrevY1 = y1;
            ceilingPrevY2 = y2;
        }

        public void DrawFloorColumn(Flat flat, Fixed height, int lightlevel, int x, int y1, int y2)
        {
            if (y2 - y1 < 0)
            {
                return;
            }

            y1 = Math.Max(y1, 0);
            y2 = Math.Min(y2, screenHeight - 1);

            var planeHeight = Fixed.Abs(height - cameraZ);

            var light = lightlevel >> LightSegShift;
            //light = Math.Max(light, 0);
            light = Math.Min(light, LightLevels - 1);
            var planezlight = zLight[light];

            if (floorPrevX == x - 1 && floorPrevHeight == planeHeight && floorPrevY1 <= y2 && y1 <= floorPrevY2)
            {
                var p1 = Math.Max(y1, floorPrevY1);
                var p2 = Math.Min(y2, floorPrevY2);

                var pos = screenHeight * (windowX + x) + windowY + y1;

                for (var y = y1; y < p1; y++)
                {
                    var distance = planeHeight * flatYSlope[y];
                    floorXStep[y] = distance * flatBaseXScale;
                    floorYStep[y] = distance * flatBaseYScale;

                    var length = distance * flatDistScale[x];
                    var angle = cameraAngle + xToAngle[x];
                    var xFrac = cameraX + Trig.Cos(angle) * length;
                    var yFrac = -cameraY - Trig.Sin(angle) * length;
                    floorXFrac[y] = xFrac;
                    floorYFrac[y] = yFrac;

                    var index = distance.Data >> LightZShift;
                    index = Math.Max(index, 0);
                    index = Math.Min(index, MaxLightZ - 1);
                    var table = planezlight[index];
                    floorLight[y] = table;

                    var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                    screenData[pos] = table[flat.Data[spot]];
                    pos++;
                }

                for (var y = p1; y <= p2; y++)
                {
                    var xFrac = floorXFrac[y] + floorXStep[y];
                    var yFrac = floorYFrac[y] + floorYStep[y];

                    var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                    screenData[pos] = floorLight[y][flat.Data[spot]];
                    pos++;

                    floorXFrac[y] = xFrac;
                    floorYFrac[y] = yFrac;
                }

                for (var y = p2 + 1; y <= y2; y++)
                {
                    var distance = planeHeight * flatYSlope[y];
                    floorXStep[y] = distance * flatBaseXScale;
                    floorYStep[y] = distance * flatBaseYScale;

                    var length = distance * flatDistScale[x];
                    var angle = cameraAngle + xToAngle[x];
                    var xFrac = cameraX + Trig.Cos(angle) * length;
                    var yFrac = -cameraY - Trig.Sin(angle) * length;
                    floorXFrac[y] = xFrac;
                    floorYFrac[y] = yFrac;

                    var index = distance.Data >> LightZShift;
                    index = Math.Max(index, 0);
                    index = Math.Min(index, MaxLightZ - 1);
                    var table = planezlight[index];
                    floorLight[y] = table;

                    var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                    screenData[pos] = table[flat.Data[spot]];
                    pos++;
                }
            }
            else
            {
                var pos = screenHeight * (windowX + x) + windowY + y1;

                for (var y = y1; y <= y2; y++)
                {
                    var distance = planeHeight * flatYSlope[y];
                    floorXStep[y] = distance * flatBaseXScale;
                    floorYStep[y] = distance * flatBaseYScale;

                    var length = distance * flatDistScale[x];
                    var angle = cameraAngle + xToAngle[x];
                    var xFrac = cameraX + Trig.Cos(angle) * length;
                    var yFrac = -cameraY - Trig.Sin(angle) * length;
                    floorXFrac[y] = xFrac;
                    floorYFrac[y] = yFrac;

                    var index = distance.Data >> LightZShift;
                    index = Math.Max(index, 0);
                    index = Math.Min(index, MaxLightZ - 1);
                    var table = planezlight[index];
                    floorLight[y] = table;

                    var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                    screenData[pos] = table[flat.Data[spot]];
                    pos++;
                }
            }

            floorPrevX = x;
            floorPrevHeight = planeHeight;
            floorPrevY1 = y1;
            floorPrevY2 = y2;
        }

        private void DrawColumn(Column column, byte[] map, int x, int y1, int y2, Fixed iScale, Fixed textureMid)
        {
            if (y2 - y1 < 0)
            {
                return;
            }

            y1 = Math.Max(y1, 0);
            y2 = Math.Min(y2, screenHeight - 1);

            // Framebuffer destination address.
            // Use ylookup LUT to avoid multiply with ScreenWidth.
            // Use columnofs LUT for subwindows? 
            var pos = screenHeight * (windowX + x) + windowY + y1;

            // Determine scaling,
            //  which is the only mapping to be done.
            var fracStep = iScale;
            var frac = textureMid + (y1 - centerY) * fracStep;

            // Inner loop that does the actual texture mapping,
            //  e.g. a DDA-lile scaling.
            // This is as fast as it gets.
            var source = column.Data;
            var offset = column.Offset;
            for (var y = y1; y <= y2; y++)
            {
                // Re-map color indices from wall texture column
                //  using a lighting/special effects LUT.
                screenData[pos] = map[source[offset + ((frac.Data >> Fixed.FracBits) & 127)]];
                pos++;

                frac += fracStep;

            }
        }

        private void DrawMaskedColumn(Column[] columns, byte[] map, int x, Fixed iScale, Fixed textureMid, Fixed spryscale, Fixed sprtopscreen, int ceilClip, int floorClip)
        {
            foreach (var column in columns)
            {
                var topscreen = sprtopscreen + spryscale * column.TopDelta;
                var bottomscreen = topscreen + spryscale * column.Length;
                var y1 = (topscreen.Data + Fixed.FracUnit - 1) >> Fixed.FracBits;
                var y2 = (bottomscreen.Data - 1) >> Fixed.FracBits;

                y1 = Math.Max(y1, ceilClip + 1);
                y2 = Math.Min(y2, floorClip - 1);

                if (y1 <= y2)
                {
                    var mid = new Fixed(textureMid.Data - (column.TopDelta << Fixed.FracBits));
                    DrawColumn(column, map, x, y1, y2, iScale, mid);
                }
            }
        }




        /*
        public void DrawScreen(Bitmap target)
        {
            for (var y = 0; y < screenHeight; y++)
            {
                for (var x = 0; x < screenWidth; x++)
                {
                    var value = screenData[screenHeight * x + y];
                    var r = palette[3 * value];
                    var g = palette[3 * value + 1];
                    var b = palette[3 * value + 2];
                    var color = Color.FromArgb(r, g, b);
                    target.SetPixel(x, y, color);
                }
            }
        }
        */


        public byte[] GlTextureData
        {
            get
            {
                return glTextureData;
            }
        }


        private class ClipRange
        {
            public int First;
            public int Last;

            public void CopyFrom(ClipRange range)
            {
                First = range.First;
                Last = range.Last;
            }
        }

        private class VisSeg
        {
            public Seg Seg;
            public int X1;
            public int X2;
            public Fixed Scale1;
            public Fixed Scale2;
            public Fixed ScaleStep;
            public int TopClip;
            public int BottomClip;
            public int MaskedTextureColumn;
        }
    }
}
