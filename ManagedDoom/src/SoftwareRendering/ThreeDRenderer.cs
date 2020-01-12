using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace ManagedDoom.SoftwareRendering
{
    public sealed class ThreeDRenderer
    {
        private ColorMap colorMap;
        private TextureLookup textures;
        private FlatLookup flats;
        private SpriteLookup sprites;

        private int screenWidth;
        private int screenHeight;
        private byte[] screenData;

        public ThreeDRenderer(
            ColorMap colorMap,
            TextureLookup textures,
            FlatLookup flats,
            SpriteLookup sprites,
            int screenWidth,
            int screenHeight,
            byte[] screenData)
        {
            this.colorMap = colorMap;
            this.textures = textures;
            this.flats = flats;
            this.sprites = sprites;

            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.screenData = screenData;

            InitWallRendering();
            InitPlaneRendering();
            InitSkyRendering();
            InitLighting();
            InitRenderingHistory();
            InitSpriteRendering();

            ResetWindow(0, 0, screenWidth, screenHeight);
            ResetWallRendering();
            ResetPlaneRendering();
            ResetSkyRendering();
            ResetLighting();
            ResetRenderingHistory();
        }



        //
        //
        // Window settings
        //
        //

        private int windowX;
        private int windowY;
        private int windowWidth;
        private int windowHeight;
        private int centerX;
        private int centerY;
        private Fixed centerXFrac;
        private Fixed centerYFrac;
        private Fixed projection;

        public void ResetWindow(int x, int y, int width, int height)
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
        }



        //
        //
        // Wall rendering
        //
        //

        private const int FineFov = 2048;

        private int[] angleToX;
        private Angle[] xToAngle;
        private Angle clipAngle;
        private Angle clipAngle2;

        public void InitWallRendering()
        {
            angleToX = new int[Trig.FineAngleCount / 2];
            xToAngle = new Angle[screenWidth];
        }

        public void ResetWallRendering()
        {
            var focalLength = centerXFrac / Trig.Tan(Trig.FineAngleCount / 4 + FineFov / 2);

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



        //
        //
        // Plane rendering
        //
        //

        private Fixed[] planeYSlope;
        private Fixed[] planeDistScale;
        private Fixed planeBaseXScale;
        private Fixed planeBaseYScale;

        private Sector ceilingPrevSector;
        private int ceilingPrevX;
        private int ceilingPrevY1;
        private int ceilingPrevY2;
        private Fixed[] ceilingXFrac;
        private Fixed[] ceilingYFrac;
        private Fixed[] ceilingXStep;
        private Fixed[] ceilingYStep;
        private byte[][] ceilingLights;

        private Sector floorPrevSector;
        private int floorPrevX;
        private int floorPrevY1;
        private int floorPrevY2;
        private Fixed[] floorXFrac;
        private Fixed[] floorYFrac;
        private Fixed[] floorXStep;
        private Fixed[] floorYStep;
        private byte[][] floorLights;

        public void InitPlaneRendering()
        {
            planeYSlope = new Fixed[screenHeight];
            planeDistScale = new Fixed[screenWidth];
            ceilingXFrac = new Fixed[screenHeight];
            ceilingYFrac = new Fixed[screenHeight];
            ceilingXStep = new Fixed[screenHeight];
            ceilingYStep = new Fixed[screenHeight];
            ceilingLights = new byte[screenHeight][];
            floorXFrac = new Fixed[screenHeight];
            floorYFrac = new Fixed[screenHeight];
            floorXStep = new Fixed[screenHeight];
            floorYStep = new Fixed[screenHeight];
            floorLights = new byte[screenHeight][];
        }

        public void ResetPlaneRendering()
        {
            for (int i = 0; i < windowHeight; i++)
            {
                var dy = Fixed.FromInt(i - windowHeight / 2) + Fixed.One / 2;
                dy = Fixed.Abs(dy);
                planeYSlope[i] = Fixed.FromInt(windowWidth / 2) / dy;
            }

            for (var i = 0; i < windowWidth; i++)
            {
                var cosadj = Fixed.Abs(Trig.Cos(xToAngle[i]));
                planeDistScale[i] = Fixed.One / cosadj;
            }
        }

        public void ClearPlaneRendering()
        {
            var angle = cameraAngle - Angle.Ang90;
            planeBaseXScale = Trig.Cos(angle) / centerXFrac;
            planeBaseYScale = -(Trig.Sin(angle) / centerXFrac);

            ceilingPrevSector = null;
            ceilingPrevX = int.MaxValue;

            floorPrevSector = null;
            floorPrevX = int.MaxValue;
        }



        //
        //
        // Sky rendering
        //
        //

        private const int AngleToSkyShift = 22;
        private Fixed skyTextureMid;
        private Fixed skyInvScale;

        private void InitSkyRendering()
        {
            skyTextureMid = Fixed.FromInt(100);
        }

        private void ResetSkyRendering()
        {
            skyInvScale = new Fixed((int)(((long)Fixed.FracUnit * screenWidth * 200) / (windowWidth * screenHeight)));
        }



        //
        //
        // Lighting
        //
        //

        private const int LightLevelCount = 16;
        private const int LightSegShift = 4;
        private const int MaxScaleLight = 48;
        private const int ScaleLightShift = 12;
        private const int MaxZLight = 128;
        private const int ZLightShift = 20;
        private const int ColorMapCount = 32;

        private byte[][][] scaleLight;
        private byte[][][] zLight;

        private void InitLighting()
        {
            scaleLight = new byte[LightLevelCount][][];
            zLight = new byte[LightLevelCount][][];
            for (var i = 0; i < LightLevelCount; i++)
            {
                scaleLight[i] = new byte[MaxScaleLight][];
                zLight[i] = new byte[MaxZLight][];
            }

            var distMap = 2;

            // Calculate the light levels to use for each level / distance combination.
            for (var i = 0; i < LightLevelCount; i++)
            {
                var startmap = ((LightLevelCount - 1 - i) * 2) * ColorMapCount / LightLevelCount;
                for (var j = 0; j < MaxZLight; j++)
                {
                    var scale = Fixed.FromInt(screenWidth / 2) / new Fixed((j + 1) << ZLightShift);
                    scale = new Fixed(scale.Data >> ScaleLightShift);

                    var level = startmap - scale.Data / distMap;
                    if (level < 0)
                    {
                        level = 0;
                    }
                    if (level >= ColorMapCount)
                    {
                        level = ColorMapCount - 1;
                    }

                    zLight[i][j] = colorMap[level];
                }
            }
        }

        private void ResetLighting()
        {
            var distMap = 2;

            // Calculate the light levels to use for each level / scale combination.
            for (var i = 0; i < LightLevelCount; i++)
            {
                var startmap = ((LightLevelCount - 1 - i) * 2) * ColorMapCount / LightLevelCount;
                for (var j = 0; j < MaxScaleLight; j++)
                {
                    var level = startmap - j * screenWidth / windowWidth / distMap;
                    if (level < 0)
                    {
                        level = 0;
                    }
                    if (level >= ColorMapCount)
                    {
                        level = ColorMapCount - 1;
                    }

                    scaleLight[i][j] = colorMap[level];
                }
            }
        }



        //
        //
        // Rendering history
        //
        //

        private short[] upperClip;
        private short[] lowerClip;

        private int negOneArray;
        private int windowHeightArray;

        private int clipRangeCount;
        private ClipRange[] clipRanges;

        private int clipDataLength;
        private short[] clipData;

        private int visWallRangeCount;
        private VisWallRange[] visWallRanges;

        private void InitRenderingHistory()
        {
            upperClip = new short[screenWidth];
            lowerClip = new short[screenWidth];

            clipRanges = new ClipRange[1024];
            for (var i = 0; i < clipRanges.Length; i++)
            {
                clipRanges[i] = new ClipRange();
            }

            clipData = new short[128 * screenWidth];

            visWallRanges = new VisWallRange[1024];
            for (var i = 0; i < visWallRanges.Length; i++)
            {
                visWallRanges[i] = new VisWallRange();
            }
        }

        private void ResetRenderingHistory()
        {
            for (var i = 0; i < windowWidth; i++)
            {
                clipData[i] = -1;
            }
            negOneArray = 0;

            for (var i = windowWidth; i < 2 * windowWidth; i++)
            {
                clipData[i] = (short)windowHeight;
            }
            windowHeightArray = windowWidth;
        }

        private void ClearRenderingHistory()
        {
            for (var x = 0; x < windowWidth; x++)
            {
                upperClip[x] = -1;
            }
            for (var x = 0; x < windowWidth; x++)
            {
                lowerClip[x] = (short)windowHeight;
            }

            clipRanges[0].First = -0x7fffffff;
            clipRanges[0].Last = -1;
            clipRanges[1].First = windowWidth;
            clipRanges[1].Last = 0x7fffffff;
            clipRangeCount = 2;

            clipDataLength = 2 * windowWidth;

            visWallRangeCount = 0;
        }



        //
        //
        // Sprite rendering
        //
        //

        private static readonly Fixed MinZ = Fixed.FromInt(4);

        private int visSpriteCount;
        private VisSprite[] visSprites;

        private void InitSpriteRendering()
        {
            visSprites = new VisSprite[1024];
            for (var i = 0; i < visSprites.Length; i++)
            {
                visSprites[i] = new VisSprite();
            }
        }

        private void ClearSpriteRendering()
        {
            visSpriteCount = 0;
        }



        //
        //
        // ???
        //
        //

        private World world;
        private Fixed cameraX;
        private Fixed cameraY;
        private Fixed cameraZ;
        private Angle cameraAngle;

        private Fixed viewSin;
        private Fixed viewCos;

        private int validCount;

        public void BindWorld(World world)
        {
            this.world = world;
        }

        public void UnbindWorld()
        {
            world = null;
        }

        public void Render()
        {
            cameraX = world.ViewX;
            cameraY = world.ViewY;
            cameraZ = world.ViewZ;
            cameraAngle = world.ViewAngle;

            ClearPlaneRendering();
            ClearRenderingHistory();
            ClearSpriteRendering();

            viewSin = Trig.Sin(cameraAngle);
            viewCos = Trig.Cos(cameraAngle);
            validCount++;

            RenderBspNode(world.Map.Nodes.Length - 1);

            DrawSprites();

            DrawMasked();
        }

        private void RenderBspNode(int node)
        {
            if (Node.IsSubsector(node))
            {
                if (node == -1)
                {
                    DrawSubsector(0);
                }
                else
                {
                    DrawSubsector(Node.GetSubsector(node));
                }
                return;
            }

            var bsp = world.Map.Nodes[node];

            // Decide which side the view point is on.
            var side = Geometry.PointOnSide(cameraX, cameraY, bsp);

            // Recursively divide front space.
            RenderBspNode(bsp.Children[side]);

            // Possibly divide back space.
            if (CheckBbox(bsp.Bbox[side ^ 1]))
            {
                RenderBspNode(bsp.Children[side ^ 1]);
            }
        }

        private void DrawSubsector(int subsector)
        {
            var target = world.Map.Subsectors[subsector];

            AddSprites(target.Sector, validCount);

            for (var i = 0; i < target.SegCount; i++)
            {
                DrawSeg(world.Map.Segs[target.FirstSeg + i]);
            }
        }

        private static readonly int[][] checkcoord =
        {
            new[] { 3, 0, 2, 1 },
            new[] { 3, 0, 2, 0 },
            new[] { 3, 1, 2, 0 },
            new[] { 0 },
            new[] { 2, 0, 2, 1 },
            new[] { 0, 0, 0, 0 },
            new[] { 3, 1, 3, 0 },
            new[] { 0 },
            new[] { 2, 0, 3, 1 },
            new[] { 2, 1, 3, 1 },
            new[] { 2, 1, 3, 0 }
        };

        public bool CheckBbox(Fixed[] bspcoord)
        {
            int boxx;
            int boxy;

            // Find the corners of the box
            // that define the edges from current viewpoint.
            if (cameraX <= bspcoord[Bbox.Left])
            {
                boxx = 0;
            }
            else if (cameraX < bspcoord[Bbox.Right])
            {
                boxx = 1;
            }
            else
            {
                boxx = 2;
            }

            if (cameraY >= bspcoord[Bbox.Top])
            {
                boxy = 0;
            }
            else if (cameraY > bspcoord[Bbox.Bottom])
            {
                boxy = 1;
            }
            else
            {
                boxy = 2;
            }

            var boxpos = (boxy << 2) + boxx;
            if (boxpos == 5)
            {
                return true;
            }

            var x1 = bspcoord[checkcoord[boxpos][0]];
            var y1 = bspcoord[checkcoord[boxpos][1]];
            var x2 = bspcoord[checkcoord[boxpos][2]];
            var y2 = bspcoord[checkcoord[boxpos][3]];

            // check clip list for an open space
            var angle1 = Geometry.PointToAngle(cameraX, cameraY, x1, y1) - cameraAngle;
            var angle2 = Geometry.PointToAngle(cameraX, cameraY, x2, y2) - cameraAngle;

            var span = angle1 - angle2;

            // Sitting on a line?
            if (span >= Angle.Ang180)
            {
                return true;
            }

            var tspan = angle1 + clipAngle;

            if (tspan > clipAngle2)
            {
                tspan -= clipAngle2;

                // Totally off the left edge?
                if (tspan >= span)
                {
                    return false;
                }

                angle1 = clipAngle;
            }

            tspan = clipAngle - angle2;
            if (tspan > clipAngle2)
            {
                tspan -= clipAngle2;

                // Totally off the left edge?
                if (tspan >= span)
                {
                    return false;
                }

                angle2 = -clipAngle;
            }

            // Find the first clippost
            //  that touches the source post
            //  (adjacent pixels are touching).
            var sx1 = angleToX[(angle1 + Angle.Ang90).Data >> Trig.AngleToFineShift];
            var sx2 = angleToX[(angle2 + Angle.Ang90).Data >> Trig.AngleToFineShift];

            // Does not cross a pixel.
            if (sx1 == sx2)
            {
                return false;
            }

            sx2--;

            var start = 0;
            while (clipRanges[start].Last < sx2)
            {
                start++;
            }

            if (sx1 >= clipRanges[start].First && sx2 <= clipRanges[start].Last)
            {
                // The clippost contains the new span.
                return false;
            }

            return true;
        }

        private void DrawSeg(Seg seg)
        {
            // OPTIMIZE: quickly reject orthogonal back sides.
            var angle1 = Geometry.PointToAngle(cameraX, cameraY, seg.Vertex1.X, seg.Vertex1.Y);
            var angle2 = Geometry.PointToAngle(cameraX, cameraY, seg.Vertex2.X, seg.Vertex2.Y);

            // Clip to view edges.
            // OPTIMIZE: make constant out of 2*clipangle (FIELDOFVIEW).
            var span = angle1 - angle2;

            // Back side? I.e. backface culling?
            if (span >= Angle.Ang180)
            {
                return;
            }

            // Global angle needed by segcalc.
            var rwAngle1 = angle1;

            angle1 -= cameraAngle;
            angle2 -= cameraAngle;

            var tSpan1 = angle1 + clipAngle;
            if (tSpan1 > clipAngle2)
            {
                tSpan1 -= clipAngle2;

                // Totally off the left edge?
                if (tSpan1 >= span)
                {
                    return;
                }

                angle1 = clipAngle;
            }

            var tSpan2 = clipAngle - angle2;
            if (tSpan2 > clipAngle2)
            {
                tSpan2 -= clipAngle2;

                // Totally off the left edge?
                if (tSpan2 >= span)
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

            var frontSector = seg.FrontSector;
            var backSector = seg.BackSector;

            // Single sided line?
            if (backSector == null)
            {
                DrawSolidWall(seg, rwAngle1, x1, x2 - 1);
                return;
            }

            // Closed door.
            if (backSector.CeilingHeight <= frontSector.FloorHeight
                || backSector.FloorHeight >= frontSector.CeilingHeight)
            {
                DrawSolidWall(seg, rwAngle1, x1, x2 - 1);
                return;
            }

            // Window.
            if (backSector.CeilingHeight != frontSector.CeilingHeight
                || backSector.FloorHeight != frontSector.FloorHeight)
            {
                DrawPassWall(seg, rwAngle1, x1, x2 - 1);
                return;
            }

            // Reject empty lines used for triggers
            // and special events.
            // Identical floor and ceiling on both sides,
            // identical light levels on both sides,
            // and no middle texture.
            if (backSector.CeilingFlat == frontSector.CeilingFlat
                && backSector.FloorFlat == frontSector.FloorFlat
                && backSector.LightLevel == frontSector.LightLevel
                && seg.SideDef.MiddleTexture == 0)
            {
                return;
            }

            DrawPassWall(seg, rwAngle1, x1, x2 - 1);
        }

        private void DrawSolidWall(Seg seg, Angle rwAngle1, int x1, int x2)
        {
            int next;
            int start;

            // Find the first range that touches the range
            // (adjacent pixels are touching).
            start = 0;
            while (clipRanges[start].Last < x1 - 1)
            {
                start++;
            }

            if (x1 < clipRanges[start].First)
            {
                if (x2 < clipRanges[start].First - 1)
                {
                    // Post is entirely visible (above start),
                    // so insert a new clippost.
                    DrawSolidWallRange(seg, rwAngle1, x1, x2);
                    next = clipRangeCount;
                    clipRangeCount++;

                    while (next != start)
                    {
                        clipRanges[next].CopyFrom(clipRanges[next - 1]);
                        next--;
                    }
                    clipRanges[next].First = x1;
                    clipRanges[next].Last = x2;
                    return;
                }

                // There is a fragment above *start.
                DrawSolidWallRange(seg, rwAngle1, x1, clipRanges[start].First - 1);

                // Now adjust the clip size.
                clipRanges[start].First = x1;
            }

            // Bottom contained in start?
            if (x2 <= clipRanges[start].Last)
            {
                return;
            }

            next = start;
            while (x2 >= clipRanges[next + 1].First - 1)
            {
                // There is a fragment between two posts.
                DrawSolidWallRange(seg, rwAngle1, clipRanges[next].Last + 1, clipRanges[next + 1].First - 1);
                next++;

                if (x2 <= clipRanges[next].Last)
                {
                    // Bottom is contained in next.
                    // Adjust the clip size.
                    clipRanges[start].Last = clipRanges[next].Last;
                    goto crunch;
                }
            }

            // There is a fragment after *next.
            DrawSolidWallRange(seg, rwAngle1, clipRanges[next].Last + 1, x2);

            // Adjust the clip size.
            clipRanges[start].Last = x2;

        // Remove start+1 to next from the clip list,
        // because start now covers their area.
        crunch:
            if (next == start)
            {
                // Post just extended past the bottom of one post.
                return;
            }

            while (next++ != clipRangeCount)
            {
                // Remove a post.
                clipRanges[++start].CopyFrom(clipRanges[next]);
            }

            clipRangeCount = start + 1;
        }

        private void DrawPassWall(Seg seg, Angle rwAngle1, int x1, int x2)
        {
            int start;

            // Find the first range that touches the range
            // (adjacent pixels are touching).
            start = 0;
            while (clipRanges[start].Last < x1 - 1)
            {
                start++;
            }

            if (x1 < clipRanges[start].First)
            {
                if (x2 < clipRanges[start].First - 1)
                {
                    // Post is entirely visible (above start).
                    DrawPassWallRange(seg, rwAngle1, x1, x2, false);
                    return;
                }

                // There is a fragment above *start.
                DrawPassWallRange(seg, rwAngle1, x1, clipRanges[start].First - 1, false);
            }

            // Bottom contained in start?
            if (x2 <= clipRanges[start].Last)
            {
                return;
            }

            while (x2 >= clipRanges[start + 1].First - 1)
            {
                // There is a fragment between two posts.
                DrawPassWallRange(seg, rwAngle1, clipRanges[start].Last + 1, clipRanges[start + 1].First - 1, false);
                start++;

                if (x2 <= clipRanges[start].Last)
                {
                    return;
                }
            }

            // There is a fragment after *next.
            DrawPassWallRange(seg, rwAngle1, clipRanges[start].Last + 1, x2, false);
        }

        public Fixed ScaleFromGlobalAngle(Angle visAngle, Angle viewAngle, Angle rwNormal, Fixed rwDistance)
        {
            var num = projection * Trig.Sin(Angle.Ang90 + (visAngle - rwNormal));
            var den = rwDistance * Trig.Sin(Angle.Ang90 + (visAngle - viewAngle));

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

        private const int HeightBits = 12;
        private const int HeightUnit = 1 << HeightBits;

        public void DrawSolidWallRange(Seg seg, Angle rwAngle1, int x1, int x2)
        {
            // Unlike the original code, the rendering functions for one / two-sided wall are separated.
            // Since DrawSolidWallRange() is for one-sided, DrawPassWallRange() must be used for two-sided.
            // This fallback is necessary because DrawSeg() treats closed doors as solid wall.
            if (seg.BackSector != null)
            {
                DrawPassWallRange(seg, rwAngle1, x1, x2, true);
                return;
            }

            // Make some aliases to shorten the following code.
            var line = seg.LineDef;
            var side = seg.SideDef;
            var frontSector = seg.FrontSector;

            // Calculate the relative plane heights of front and back sector.
            // These values are later 4 bits right shifted to calculate the rendering area.
            var worldFrontY1 = frontSector.CeilingHeight - cameraZ;
            var worldFrontY2 = frontSector.FloorHeight - cameraZ;

            // Check which parts must be rendered.
            var drawWall = side.MiddleTexture != 0;
            var drawCeiling = worldFrontY1 > Fixed.Zero || frontSector.CeilingFlat == flats.SkyFlatNumber;
            var drawFloor = worldFrontY2 < Fixed.Zero;



            //
            //
            // Determine how the wall textures are vertically aligned.
            //
            //

            var wallTexture = textures[side.MiddleTexture];
            var wallWidthMask = wallTexture.Width - 1;

            Fixed rwMiddleTextureMid;
            if ((line.Flags & LineFlags.DontPegBottom) != 0)
            {
                var vTop = frontSector.FloorHeight + Fixed.FromInt(wallTexture.Height);
                rwMiddleTextureMid = vTop - cameraZ;
            }
            else
            {
                rwMiddleTextureMid = worldFrontY1;
            }
            rwMiddleTextureMid += side.RowOffset;



            //
            //
            // Calculate the scaling factors of the left and right edges of the wall range.
            //
            //

            var rwNormalAngle = seg.Angle + Angle.Ang90;

            var offsetAngle = Angle.Abs(rwNormalAngle - rwAngle1);
            if (offsetAngle > Angle.Ang90)
            {
                offsetAngle = Angle.Ang90;
            }

            var distAngle = Angle.Ang90 - offsetAngle;

            var hypotenuse = Geometry.PointToDist(cameraX, cameraY, seg.Vertex1.X, seg.Vertex1.Y);

            var rwDistance = hypotenuse * Trig.Sin(distAngle);

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



            //
            //
            // Determine how the wall textures are horizontally aligned
            // and which color map is used according to the light level (if necessary).
            //
            //

            var textureOffsetAngle = rwNormalAngle - rwAngle1;
            if (textureOffsetAngle > Angle.Ang180)
            {
                textureOffsetAngle = -textureOffsetAngle;
            }
            if (textureOffsetAngle > Angle.Ang90)
            {
                textureOffsetAngle = Angle.Ang90;
            }

            var rwOffset = hypotenuse * Trig.Sin(textureOffsetAngle);
            if (rwNormalAngle - rwAngle1 < Angle.Ang180)
            {
                rwOffset = -rwOffset;
            }
            rwOffset += seg.Offset + side.TextureOffset;

            var rwCenterAngle = Angle.Ang90 + cameraAngle - rwNormalAngle;

            var wallLightLevel = (frontSector.LightLevel >> LightSegShift); // + extraLight;
            if (seg.Vertex1.Y == seg.Vertex2.Y)
            {
                wallLightLevel--;
            }
            else if (seg.Vertex1.X == seg.Vertex2.X)
            {
                wallLightLevel++;
            }

            var wallLights = scaleLight[Math.Clamp(wallLightLevel, 0, LightLevelCount - 1)];



            //
            //
            // Determine where on the screen the wall is drawn.
            //
            //

            // Now these values are right shifted to adjust the scale to the screen coord calculation.
            worldFrontY1 = new Fixed(worldFrontY1.Data >> 4);
            worldFrontY2 = new Fixed(worldFrontY2.Data >> 4);

            // The Y positions of the top / bottom edges of the wall.
            var wallY1Frac = new Fixed(centerYFrac.Data >> 4) - worldFrontY1 * rwScale;
            var wallY1Step = -(rwScaleStep * worldFrontY1);
            var wallY2Frac = new Fixed(centerYFrac.Data >> 4) - worldFrontY2 * rwScale;
            var wallY2Step = -(rwScaleStep * worldFrontY2);



            //
            //
            // Determine which color map is used for the plane according to the light level.
            //
            //

            var planeLightLevel = frontSector.LightLevel >> LightSegShift; // + extraLight;
            if (planeLightLevel >= LightLevelCount)
            {
                planeLightLevel = LightLevelCount - 1;
            }
            var planeLights = zLight[planeLightLevel];



            //
            //
            // Prepare to record the rendering history.
            //
            //

            var visSeg = visWallRanges[visWallRangeCount];
            visSeg.Seg = seg;
            visSeg.X1 = x1;
            visSeg.X2 = x2;
            visSeg.Scale1 = scale1;
            visSeg.Scale2 = scale2;
            visSeg.ScaleStep = rwScaleStep;
            visSeg.Silhouette = Silhouette.Both;
            visSeg.LowerSilHeight = Fixed.MaxValue;
            visSeg.UpperSilHeight = Fixed.MinValue;
            visSeg.MaskedTextureColumn = -1;
            visSeg.UpperClip = windowHeightArray;
            visSeg.LowerClip = negOneArray;


            //
            //
            // Now the rendering is carried out.
            //
            //

            for (var x = x1; x <= x2; x++)
            {
                var drawWallY1 = (wallY1Frac.Data + HeightUnit - 1) >> HeightBits;
                var drawWallY2 = wallY2Frac.Data >> HeightBits;

                if (drawCeiling)
                {
                    var cy1 = upperClip[x] + 1;
                    var cy2 = Math.Min(drawWallY1 - 1, lowerClip[x] - 1);
                    DrawCeilingColumn(frontSector, flats[frontSector.CeilingFlat], planeLights, x, cy1, cy2);
                }

                if (drawWall)
                {
                    var wy1 = Math.Max(drawWallY1, upperClip[x] + 1);
                    var wy2 = Math.Min(drawWallY2, lowerClip[x] - 1);

                    var angle = rwCenterAngle + xToAngle[x];
                    angle = new Angle(angle.Data & 0x7FFFFFFF);

                    var textureColumn = (rwOffset - Trig.Tan(angle) * rwDistance).Data >> Fixed.FracBits;
                    var source = wallTexture.Composite.Columns[textureColumn & wallWidthMask][0];

                    var lightScale = rwScale.Data >> ScaleLightShift;
                    if (lightScale >= MaxScaleLight)
                    {
                        lightScale = MaxScaleLight - 1;
                    }

                    var iScale = new Fixed((int)(0xffffffffu / (uint)rwScale.Data));
                    DrawColumn(source, wallLights[lightScale], x, wy1, wy2, iScale, rwMiddleTextureMid);
                }

                if (drawFloor)
                {
                    var fy1 = Math.Max(drawWallY2 + 1, upperClip[x] + 1);
                    var fy2 = lowerClip[x] - 1;
                    DrawFloorColumn(frontSector, flats[frontSector.FloorFlat], planeLights, x, fy1, fy2);
                }

                rwScale += rwScaleStep;
                wallY1Frac += wallY1Step;
                wallY2Frac += wallY2Step;
            }

            visWallRangeCount++;
        }



        public void DrawPassWallRange(Seg seg, Angle rwAngle1, int x1, int x2, bool drawAsSolidWall)
        {
            // Make some aliases to shorten the following code.
            var line = seg.LineDef;
            var side = seg.SideDef;
            var frontSector = seg.FrontSector;
            var backSector = seg.BackSector;

            // Calculate the relative plane heights of front and back sector.
            // These values are later 4 bits right shifted to calculate the rendering area.
            var worldFrontY1 = frontSector.CeilingHeight - cameraZ;
            var worldFrontY2 = frontSector.FloorHeight - cameraZ;
            var worldBackY1 = backSector.CeilingHeight - cameraZ;
            var worldBackY2 = backSector.FloorHeight - cameraZ;

            // The hack below enables ceiling height change in outdoor area without showing the upper wall.
            if (frontSector.CeilingFlat == flats.SkyFlatNumber
                && backSector.CeilingFlat == flats.SkyFlatNumber)
            {
                worldFrontY1 = worldBackY1;
            }



            //
            //
            // Check which parts must be rendered.
            //
            // Wall / plane rendering for two-sided line can be partially skipped under the following conditions.
            //
            //     - The line is not treated as a solid wall (like a closed door).
            //     - The wall texture is not set.
            //     - The front and back sectors have the identical height, flat texture and light level.
            //     - The camera sees the wrong side of the plane.
            //
            // These are important not only for performance but also to reproduce the following tricks.
            //
            //     - The entrance to the outdoor area in E1M1 has the different height from the adjacent walls.
            //     - The sky texture is shown where a wall should be (the chainsaw area in MAP01 for example).
            //     - Fake 3D bridges in REQUIEM.WAD.
            //
            //

            bool drawUpperWall;
            bool drawCeiling;
            if (drawAsSolidWall
                || worldFrontY1 != worldBackY1
                || frontSector.CeilingFlat != backSector.CeilingFlat
                || frontSector.LightLevel != backSector.LightLevel)
            {
                drawUpperWall = side.TopTexture != 0 && worldBackY1 < worldFrontY1;
                drawCeiling = worldFrontY1 > Fixed.Zero || frontSector.CeilingFlat == flats.SkyFlatNumber;
            }
            else
            {
                drawUpperWall = false;
                drawCeiling = false;
            }

            bool drawLowerWall;
            bool drawFloor;
            if (drawAsSolidWall
                || worldFrontY2 != worldBackY2
                || frontSector.FloorFlat != backSector.FloorFlat
                || frontSector.LightLevel != backSector.LightLevel)
            {
                drawLowerWall = side.BottomTexture != 0 && worldBackY2 > worldFrontY2;
                drawFloor = worldFrontY2 < Fixed.Zero;
            }
            else
            {
                drawLowerWall = false;
                drawFloor = false;
            }

            var drawMaskedTexture = side.MiddleTexture != 0;

            // If nothing must be rendered, we can skip this seg.
            if (!drawUpperWall && !drawCeiling && !drawLowerWall && !drawFloor && !drawMaskedTexture)
            {
                return;
            }

            var segTextured = drawUpperWall || drawLowerWall || drawMaskedTexture;



            //
            //
            // Determine how the wall textures are vertically aligned (if necessary).
            //
            //

            var upperWallTexture = default(Texture);
            var upperWallWidthMask = default(int);
            var rwUpperTextureMid = default(Fixed);
            if (drawUpperWall)
            {
                upperWallTexture = textures[side.TopTexture];
                upperWallWidthMask = upperWallTexture.Width - 1;

                if ((line.Flags & LineFlags.DontPegTop) != 0)
                {
                    rwUpperTextureMid = worldFrontY1;
                }
                else
                {
                    var vTop = backSector.CeilingHeight + Fixed.FromInt(upperWallTexture.Height);
                    rwUpperTextureMid = vTop - cameraZ;
                }
                rwUpperTextureMid += side.RowOffset;
            }

            var lowerWallTexture = default(Texture);
            var lowerWallWidthMask = default(int);
            var rwLowerTextureMid = default(Fixed);
            if (drawLowerWall)
            {
                lowerWallTexture = textures[side.BottomTexture];
                lowerWallWidthMask = lowerWallTexture.Width - 1;

                if ((line.Flags & LineFlags.DontPegBottom) != 0)
                {
                    rwLowerTextureMid = worldFrontY1;
                }
                else
                {
                    rwLowerTextureMid = worldBackY2;
                }
                rwLowerTextureMid += side.RowOffset;
            }



            //
            //
            // Calculate the scaling factors of the left and right edges of the wall range.
            //
            //

            var rwNormalAngle = seg.Angle + Angle.Ang90;

            var offsetAngle = Angle.Abs(rwNormalAngle - rwAngle1);
            if (offsetAngle > Angle.Ang90)
            {
                offsetAngle = Angle.Ang90;
            }

            var distAngle = Angle.Ang90 - offsetAngle;

            var hypotenuse = Geometry.PointToDist(cameraX, cameraY, seg.Vertex1.X, seg.Vertex1.Y);

            var rwDistance = hypotenuse * Trig.Sin(distAngle);

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



            //
            //
            // Determine how the wall textures are horizontally aligned
            // and which color map is used according to the light level (if necessary).
            //
            //

            var rwOffset = default(Fixed);
            var rwCenterAngle = default(Angle);
            var wallLights = default(byte[][]);
            if (segTextured)
            {
                var textureOffsetAngle = rwNormalAngle - rwAngle1;
                if (textureOffsetAngle > Angle.Ang180)
                {
                    textureOffsetAngle = -textureOffsetAngle;
                }
                if (textureOffsetAngle > Angle.Ang90)
                {
                    textureOffsetAngle = Angle.Ang90;
                }

                rwOffset = hypotenuse * Trig.Sin(textureOffsetAngle);
                if (rwNormalAngle - rwAngle1 < Angle.Ang180)
                {
                    rwOffset = -rwOffset;
                }
                rwOffset += seg.Offset + side.TextureOffset;

                rwCenterAngle = Angle.Ang90 + cameraAngle - rwNormalAngle;

                var wallLightLevel = (frontSector.LightLevel >> LightSegShift); // + extraLight;
                if (seg.Vertex1.Y == seg.Vertex2.Y)
                {
                    wallLightLevel--;
                }
                else if (seg.Vertex1.X == seg.Vertex2.X)
                {
                    wallLightLevel++;
                }

                wallLights = scaleLight[Math.Clamp(wallLightLevel, 0, LightLevelCount - 1)];
            }



            //
            //
            // Determine where on the screen the wall is drawn.
            //
            //

            // Now these values are right shifted to adjust the scale to the screen coord calculation.
            worldFrontY1 = new Fixed(worldFrontY1.Data >> 4);
            worldFrontY2 = new Fixed(worldFrontY2.Data >> 4);
            worldBackY1 = new Fixed(worldBackY1.Data >> 4);
            worldBackY2 = new Fixed(worldBackY2.Data >> 4);

            // The Y positions of the top / bottom edges of the wall.
            var wallY1Frac = new Fixed(centerYFrac.Data >> 4) - worldFrontY1 * rwScale;
            var wallY1Step = -(rwScaleStep * worldFrontY1);
            var wallY2Frac = new Fixed(centerYFrac.Data >> 4) - worldFrontY2 * rwScale;
            var wallY2Step = -(rwScaleStep * worldFrontY2);

            // The Y position of the top edge of the portal (if visible).
            var portalY1Frac = default(Fixed);
            var portalY1Step = default(Fixed);
            if (drawUpperWall)
            {
                if (worldBackY1 > worldFrontY2)
                {
                    portalY1Frac = new Fixed(centerYFrac.Data >> 4) - worldBackY1 * rwScale;
                    portalY1Step = -(rwScaleStep * worldBackY1);
                }
                else
                {
                    portalY1Frac = new Fixed(centerYFrac.Data >> 4) - worldFrontY2 * rwScale;
                    portalY1Step = -(rwScaleStep * worldFrontY2);
                }
            }

            // The Y position of the bottom edge of the portal (if visible).
            var portalY2Frac = default(Fixed);
            var portalY2Step = default(Fixed);
            if (drawLowerWall)
            {
                if (worldBackY2 < worldFrontY1)
                {
                    portalY2Frac = new Fixed(centerYFrac.Data >> 4) - worldBackY2 * rwScale;
                    portalY2Step = -(rwScaleStep * worldBackY2);
                }
                else
                {
                    portalY2Frac = new Fixed(centerYFrac.Data >> 4) - worldFrontY1 * rwScale;
                    portalY2Step = -(rwScaleStep * worldFrontY1);
                }
            }



            //
            //
            // Determine which color map is used for the plane according to the light level.
            //
            //

            var planeLightLevel = frontSector.LightLevel >> LightSegShift; // + extraLight;
            if (planeLightLevel >= LightLevelCount)
            {
                planeLightLevel = LightLevelCount - 1;
            }
            var planeLights = zLight[planeLightLevel];



            //
            //
            // Prepare to record the rendering history.
            //
            //

            var visSeg = visWallRanges[visWallRangeCount];
            visSeg.Seg = seg;
            visSeg.X1 = x1;
            visSeg.X2 = x2;
            visSeg.Scale1 = scale1;
            visSeg.Scale2 = scale2;
            visSeg.ScaleStep = rwScaleStep;

            visSeg.UpperClip = -1;
            visSeg.LowerClip = -1;
            visSeg.Silhouette = 0;

            if (frontSector.FloorHeight > backSector.FloorHeight)
            {
                visSeg.Silhouette = Silhouette.Lower;
                visSeg.LowerSilHeight = frontSector.FloorHeight;
            }
            else if (backSector.FloorHeight > cameraZ)
            {
                visSeg.Silhouette = Silhouette.Lower;
                visSeg.LowerSilHeight = Fixed.MaxValue;
                // ds_p->sprbottomclip = negonearray;
            }

            if (frontSector.CeilingHeight < backSector.CeilingHeight)
            {
                visSeg.Silhouette |= Silhouette.Upper;
                visSeg.UpperSilHeight = frontSector.CeilingHeight;
            }
            else if (backSector.CeilingHeight < cameraZ)
            {
                visSeg.Silhouette |= Silhouette.Upper;
                visSeg.UpperSilHeight = Fixed.MinValue;
                // ds_p->sprtopclip = screenheightarray;
            }

            if (backSector.CeilingHeight <= frontSector.FloorHeight)
            {
                visSeg.LowerClip = negOneArray;
                visSeg.LowerSilHeight = Fixed.MaxValue;
                visSeg.Silhouette |= Silhouette.Lower;
            }

            if (backSector.FloorHeight >= frontSector.CeilingHeight)
            {
                visSeg.UpperClip = windowHeightArray;
                visSeg.UpperSilHeight = Fixed.MinValue;
                visSeg.Silhouette |= Silhouette.Upper;
            }

            var range = x2 - x1 + 1;

            var maskedTextureColumn = default(int);
            if (drawMaskedTexture)
            {
                maskedTextureColumn = clipDataLength - x1;
                visSeg.MaskedTextureColumn = maskedTextureColumn;
                clipDataLength += range;
            }
            else
            {
                visSeg.MaskedTextureColumn = -1;
            }



            //
            //
            // Now the rendering is carried out.
            //
            //

            for (var x = x1; x <= x2; x++)
            {
                var drawWallY1 = (wallY1Frac.Data + HeightUnit - 1) >> HeightBits;
                var drawWallY2 = wallY2Frac.Data >> HeightBits;

                var textureColumn = default(int);
                var lightIndex = default(int);
                if (segTextured)
                {
                    var angle = rwCenterAngle + xToAngle[x];
                    angle = new Angle(angle.Data & 0x7FFFFFFF);
                    textureColumn = (rwOffset - Trig.Tan(angle) * rwDistance).Data >> Fixed.FracBits;

                    lightIndex = rwScale.Data >> ScaleLightShift;
                    if (lightIndex >= MaxScaleLight)
                    {
                        lightIndex = MaxScaleLight - 1;
                    }
                }

                if (drawUpperWall)
                {
                    var drawUpperWallY1 = (wallY1Frac.Data + HeightUnit - 1) >> HeightBits;
                    var drawUpperWallY2 = portalY1Frac.Data >> HeightBits;

                    if (drawCeiling)
                    {
                        var cy1 = upperClip[x] + 1;
                        var cy2 = Math.Min(drawWallY1 - 1, lowerClip[x] - 1);
                        DrawCeilingColumn(frontSector, flats[frontSector.CeilingFlat], planeLights, x, cy1, cy2);
                    }

                    var wy1 = Math.Max(drawUpperWallY1, upperClip[x] + 1);
                    var wy2 = Math.Min(drawUpperWallY2, lowerClip[x] - 1);
                    var source = upperWallTexture.Composite.Columns[textureColumn & upperWallWidthMask][0];
                    var iScale = new Fixed((int)(0xffffffffu / (uint)rwScale.Data));
                    DrawColumn(source, wallLights[lightIndex], x, wy1, wy2, iScale, rwUpperTextureMid);

                    if (upperClip[x] < wy2)
                    {
                        upperClip[x] = (short)wy2;
                    }

                    portalY1Frac += portalY1Step;
                }
                else if (drawCeiling)
                {
                    var cy1 = upperClip[x] + 1;
                    var cy2 = Math.Min(drawWallY1 - 1, lowerClip[x] - 1);
                    DrawCeilingColumn(frontSector, flats[frontSector.CeilingFlat], planeLights, x, cy1, cy2);

                    if (upperClip[x] < cy2)
                    {
                        upperClip[x] = (short)cy2;
                    }
                }

                if (drawLowerWall)
                {
                    var drawLowerWallY1 = (portalY2Frac.Data + HeightUnit - 1) >> HeightBits;
                    var drawLowerWallY2 = wallY2Frac.Data >> HeightBits;

                    var wy1 = Math.Max(drawLowerWallY1, upperClip[x] + 1);
                    var wy2 = Math.Min(drawLowerWallY2, lowerClip[x] - 1);
                    var source = lowerWallTexture.Composite.Columns[textureColumn & lowerWallWidthMask][0];
                    var iScale = new Fixed((int)(0xffffffffu / (uint)rwScale.Data));
                    DrawColumn(source, wallLights[lightIndex], x, wy1, wy2, iScale, rwLowerTextureMid);

                    if (drawFloor)
                    {
                        var fy1 = Math.Max(drawWallY2 + 1, upperClip[x] + 1);
                        var fy2 = lowerClip[x] - 1;
                        DrawFloorColumn(frontSector, flats[frontSector.FloorFlat], planeLights, x, fy1, fy2);
                    }

                    if (lowerClip[x] > wy1)
                    {
                        lowerClip[x] = (short)wy1;
                    }

                    portalY2Frac += portalY2Step;
                }
                else if (drawFloor)
                {
                    var fy1 = Math.Max(drawWallY2 + 1, upperClip[x] + 1);
                    var fy2 = lowerClip[x] - 1;
                    DrawFloorColumn(frontSector, flats[frontSector.FloorFlat], planeLights, x, fy1, fy2);

                    if (lowerClip[x] > drawWallY2 + 1)
                    {
                        lowerClip[x] = (short)fy1;
                    }
                }

                if (drawMaskedTexture)
                {
                    clipData[maskedTextureColumn + x] = (short)textureColumn;
                }

                rwScale += rwScaleStep;
                wallY1Frac += wallY1Step;
                wallY2Frac += wallY2Step;
            }

            //
            // save sprite clipping info
            //

            if (((visSeg.Silhouette & Silhouette.Upper) != 0 || drawMaskedTexture) && visSeg.UpperClip == -1)
            {
                Array.Copy(upperClip, x1, clipData, clipDataLength, range);
                visSeg.UpperClip = clipDataLength - x1;
                clipDataLength += range;
            }

            if (((visSeg.Silhouette & Silhouette.Lower) != 0 || drawMaskedTexture) && visSeg.LowerClip == -1)
            {
                Array.Copy(lowerClip, x1, clipData, clipDataLength, range);
                visSeg.LowerClip = clipDataLength - x1;
                clipDataLength += range;
            }

            if (drawMaskedTexture && (visSeg.Silhouette & Silhouette.Upper) == 0)
            {
                visSeg.Silhouette |= Silhouette.Upper;
                visSeg.UpperSilHeight = Fixed.MinValue;
            }

            if (drawMaskedTexture && (visSeg.Silhouette & Silhouette.Lower) == 0)
            {
                visSeg.Silhouette |= Silhouette.Lower;
                visSeg.LowerSilHeight = Fixed.MaxValue;
            }

            visWallRangeCount++;
        }



        public void DrawMasked()
        {
            for (var i = visWallRangeCount - 1; i >= 0; i--)
            {
                var drawSeg = visWallRanges[i];
                var seg = drawSeg.Seg;

                if (drawSeg.MaskedTextureColumn != -1)
                {
                    DrawMaskedRange(drawSeg, drawSeg.X1, drawSeg.X2);
                }
            }
        }

        private void DrawMaskedRange(VisWallRange drawSeg, int x1, int x2)
        {
            var seg = drawSeg.Seg;

            var wallLightLevel = (seg.FrontSector.LightLevel >> LightSegShift); // + extraLight;
            if (seg.Vertex1.Y == seg.Vertex2.Y)
            {
                wallLightLevel--;
            }
            else if (seg.Vertex1.X == seg.Vertex2.X)
            {
                wallLightLevel++;
            }

            var wallLights = scaleLight[Math.Clamp(wallLightLevel, 0, LightLevelCount - 1)];

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
            var spryscale = drawSeg.Scale1 + (x1 - drawSeg.X1) * rwScaleStep;


            for (var x = x1; x <= x2; x++)
            {
                var index = spryscale.Data >> ScaleLightShift;
                if (index >= MaxScaleLight)
                {
                    index = MaxScaleLight - 1;
                }

                var col = clipData[drawSeg.MaskedTextureColumn + x];

                if (col != short.MaxValue)
                {
                    var sprtopscreen = centerYFrac - rwMidTextureMid * spryscale;
                    var iScale = new Fixed((int)(0xffffffffu / (uint)spryscale.Data));
                    var ceilClip = clipData[drawSeg.UpperClip + x];
                    var floorClip = clipData[drawSeg.LowerClip + x];
                    DrawMaskedColumn(
                        wallTexture.Composite.Columns[col & mask],
                        wallLights[index],
                        x, iScale,
                        rwMidTextureMid,
                        spryscale,
                        sprtopscreen,
                        ceilClip,
                        floorClip);

                    clipData[drawSeg.MaskedTextureColumn + x] = short.MaxValue;
                }

                spryscale += rwScaleStep;
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

        public void DrawCeilingColumn(Sector sector, Flat flat, byte[][] planeLights, int x, int y1, int y2)
        {
            if (flat == flats.SkyFlat)
            {
                DrawSkyColumn(x, y1, y2);
                return;
            }

            if (y2 - y1 < 0)
            {
                return;
            }

            //y1 = Math.Max(y1, 0);
            //y2 = Math.Min(y2, screenHeight - 1);

            var height = Fixed.Abs(sector.CeilingHeight - cameraZ);

            var data = flat.Data;

            if (sector == ceilingPrevSector && ceilingPrevX == x - 1)
            {
                var p1 = Math.Max(y1, ceilingPrevY1);
                var p2 = Math.Min(y2, ceilingPrevY2);

                var pos = screenHeight * (windowX + x) + windowY + y1;

                for (var y = y1; y < p1; y++)
                {
                    var distance = height * planeYSlope[y];
                    ceilingXStep[y] = distance * planeBaseXScale;
                    ceilingYStep[y] = distance * planeBaseYScale;

                    var length = distance * planeDistScale[x];
                    var angle = cameraAngle + xToAngle[x];
                    var xFrac = cameraX + Trig.Cos(angle) * length;
                    var yFrac = -cameraY - Trig.Sin(angle) * length;
                    ceilingXFrac[y] = xFrac;
                    ceilingYFrac[y] = yFrac;

                    var colorMap = planeLights[Math.Min((uint)(distance.Data >> ZLightShift), MaxZLight - 1)];
                    ceilingLights[y] = colorMap;

                    var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                    screenData[pos] = colorMap[data[spot]];
                    pos++;
                }

                for (var y = p1; y <= p2; y++)
                {
                    var xFrac = ceilingXFrac[y] + ceilingXStep[y];
                    var yFrac = ceilingYFrac[y] + ceilingYStep[y];

                    var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                    screenData[pos] = ceilingLights[y][data[spot]];
                    pos++;

                    ceilingXFrac[y] = xFrac;
                    ceilingYFrac[y] = yFrac;
                }

                for (var y = p2 + 1; y <= y2; y++)
                {
                    var distance = height * planeYSlope[y];
                    ceilingXStep[y] = distance * planeBaseXScale;
                    ceilingYStep[y] = distance * planeBaseYScale;

                    var length = distance * planeDistScale[x];
                    var angle = cameraAngle + xToAngle[x];
                    var xFrac = cameraX + Trig.Cos(angle) * length;
                    var yFrac = -cameraY - Trig.Sin(angle) * length;
                    ceilingXFrac[y] = xFrac;
                    ceilingYFrac[y] = yFrac;

                    var colorMap = planeLights[Math.Min((uint)(distance.Data >> ZLightShift), MaxZLight - 1)];
                    ceilingLights[y] = colorMap;

                    var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                    screenData[pos] = colorMap[data[spot]];
                    pos++;
                }
            }
            else
            {
                var pos = screenHeight * (windowX + x) + windowY + y1;

                for (var y = y1; y <= y2; y++)
                {
                    var distance = height * planeYSlope[y];
                    ceilingXStep[y] = distance * planeBaseXScale;
                    ceilingYStep[y] = distance * planeBaseYScale;

                    var length = distance * planeDistScale[x];
                    var angle = cameraAngle + xToAngle[x];
                    var xFrac = cameraX + Trig.Cos(angle) * length;
                    var yFrac = -cameraY - Trig.Sin(angle) * length;
                    ceilingXFrac[y] = xFrac;
                    ceilingYFrac[y] = yFrac;

                    var colorMap = planeLights[Math.Min((uint)(distance.Data >> ZLightShift), MaxZLight - 1)];
                    ceilingLights[y] = colorMap;

                    var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                    screenData[pos] = colorMap[data[spot]];
                    pos++;
                }
            }

            ceilingPrevSector = sector;
            ceilingPrevX = x;
            ceilingPrevY1 = y1;
            ceilingPrevY2 = y2;
        }

        public void DrawFloorColumn(Sector sector, Flat flat, byte[][] planeLights, int x, int y1, int y2)
        {
            if (flat == flats.SkyFlat)
            {
                DrawSkyColumn(x, y1, y2);
                return;
            }

            if (y2 - y1 < 0)
            {
                return;
            }

            //y1 = Math.Max(y1, 0);
            //y2 = Math.Min(y2, screenHeight - 1);

            var height = Fixed.Abs(sector.FloorHeight - cameraZ);

            var data = flat.Data;

            if (sector == floorPrevSector && floorPrevX == x - 1)
            {
                var p1 = Math.Max(y1, floorPrevY1);
                var p2 = Math.Min(y2, floorPrevY2);

                var pos = screenHeight * (windowX + x) + windowY + y1;

                for (var y = y1; y < p1; y++)
                {
                    var distance = height * planeYSlope[y];
                    floorXStep[y] = distance * planeBaseXScale;
                    floorYStep[y] = distance * planeBaseYScale;

                    var length = distance * planeDistScale[x];
                    var angle = cameraAngle + xToAngle[x];
                    var xFrac = cameraX + Trig.Cos(angle) * length;
                    var yFrac = -cameraY - Trig.Sin(angle) * length;
                    floorXFrac[y] = xFrac;
                    floorYFrac[y] = yFrac;

                    var colorMap = planeLights[Math.Min((uint)(distance.Data >> ZLightShift), MaxZLight - 1)];
                    floorLights[y] = colorMap;

                    var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                    screenData[pos] = colorMap[data[spot]];
                    pos++;
                }

                for (var y = p1; y <= p2; y++)
                {
                    var xFrac = floorXFrac[y] + floorXStep[y];
                    var yFrac = floorYFrac[y] + floorYStep[y];

                    var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                    screenData[pos] = floorLights[y][data[spot]];
                    pos++;

                    floorXFrac[y] = xFrac;
                    floorYFrac[y] = yFrac;
                }

                for (var y = p2 + 1; y <= y2; y++)
                {
                    var distance = height * planeYSlope[y];
                    floorXStep[y] = distance * planeBaseXScale;
                    floorYStep[y] = distance * planeBaseYScale;

                    var length = distance * planeDistScale[x];
                    var angle = cameraAngle + xToAngle[x];
                    var xFrac = cameraX + Trig.Cos(angle) * length;
                    var yFrac = -cameraY - Trig.Sin(angle) * length;
                    floorXFrac[y] = xFrac;
                    floorYFrac[y] = yFrac;

                    var colorMap = planeLights[Math.Min((uint)(distance.Data >> ZLightShift), MaxZLight - 1)];
                    floorLights[y] = colorMap;

                    var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                    screenData[pos] = colorMap[data[spot]];
                    pos++;
                }
            }
            else
            {
                var pos = screenHeight * (windowX + x) + windowY + y1;

                for (var y = y1; y <= y2; y++)
                {
                    var distance = height * planeYSlope[y];
                    floorXStep[y] = distance * planeBaseXScale;
                    floorYStep[y] = distance * planeBaseYScale;

                    var length = distance * planeDistScale[x];
                    var angle = cameraAngle + xToAngle[x];
                    var xFrac = cameraX + Trig.Cos(angle) * length;
                    var yFrac = -cameraY - Trig.Sin(angle) * length;
                    floorXFrac[y] = xFrac;
                    floorYFrac[y] = yFrac;

                    var colorMap = planeLights[Math.Min((uint)(distance.Data >> ZLightShift), MaxZLight - 1)];
                    floorLights[y] = colorMap;

                    var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                    screenData[pos] = colorMap[data[spot]];
                    pos++;
                }
            }

            floorPrevSector = sector;
            floorPrevX = x;
            floorPrevY1 = y1;
            floorPrevY2 = y2;
        }

        private void DrawColumn(Column column, byte[] map, int x, int y1, int y2, Fixed iScale, Fixed textureMid)
        {
            if (y2 - y1 < 0)
            {
                return;
            }

            //y1 = Math.Max(y1, 0);
            //y2 = Math.Min(y2, screenHeight - 1);

            // Framebuffer destination address.
            // Use ylookup LUT to avoid multiply with ScreenWidth.
            // Use columnofs LUT for subwindows? 
            var pos1 = screenHeight * (windowX + x) + windowY + y1;
            var pos2 = pos1 + (y2 - y1);

            // Determine scaling,
            //  which is the only mapping to be done.
            var fracStep = iScale;
            var frac = textureMid + (y1 - centerY) * fracStep;

            // Inner loop that does the actual texture mapping,
            //  e.g. a DDA-lile scaling.
            // This is as fast as it gets.
            var source = column.Data;
            var offset = column.Offset;
            for (var pos = pos1; pos <= pos2; pos++)
            {
                // Re-map color indices from wall texture column
                //  using a lighting/special effects LUT.
                screenData[pos] = map[source[offset + ((frac.Data >> Fixed.FracBits) & 127)]];

                frac += fracStep;

            }
        }

        public void DrawSkyColumn(int x, int y1, int y2)
        {
            var angle = (cameraAngle + xToAngle[x]).Data >> AngleToSkyShift;
            var mask = world.Map.SkyTexture.Width - 1;
            var source = world.Map.SkyTexture.Composite.Columns[angle & mask];
            DrawColumn(source[0], colorMap[0], x, y1, y2, skyInvScale, skyTextureMid);
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



        private void AddSprites(Sector sector, int validCount)
        {
            // BSP is traversed by subsector.
            // A sector might have been split into several
            //  subsectors during BSP building.
            // Thus we check whether its already added.
            if (sector.ValidCount == validCount)
            {
                return;
            }

            // Well, now it will be done.
            sector.ValidCount = validCount;

            var lightnum = (sector.LightLevel >> LightSegShift); // + extralight;

            var spritelights = scaleLight[Math.Clamp(lightnum, 0, LightLevelCount - 1)];

            // Handle all things in sector.
            foreach (var thing in sector)
            {
                ProjectSprite(thing, spritelights);
            }
        }

        private void ProjectSprite(Mobj thing, byte[][] spritelights)
        {
            // transform the origin point
            var tr_x = thing.X - cameraX;
            var tr_y = thing.Y - cameraY;

            var gxt = (tr_x * viewCos);
            var gyt = -(tr_y * viewSin);

            var tz = gxt - gyt;

            // thing is behind view plane?
            if (tz < MinZ)
            {
                return;
            }

            var xscale = projection / tz;

            gxt = -tr_x * viewSin;
            gyt = tr_y * viewCos;
            var tx = -(gyt + gxt);

            // too far off the side?
            if (Fixed.Abs(tx) > new Fixed(tz.Data << 2))
            {
                return;
            }

            var sprdef = sprites[thing.Sprite];
            var framenum = thing.Frame & 0x7F;
            var sprframe = sprdef.Frames[framenum];

            Patch lump;
            bool flip;
            if (sprframe.Rotate)
            {
                // choose a different rotation based on player view
                var ang = Geometry.PointToAngle(cameraX, cameraY, thing.X, thing.Y);
                var rot = (ang.Data - thing.Angle.Data + (uint)(Angle.Ang45.Data / 2) * 9) >> 29;
                lump = sprframe.Patches[rot];
                flip = sprframe.Flip[rot];
            }
            else
            {
                // use single rotation for all views
                lump = sprframe.Patches[0];
                flip = sprframe.Flip[0];
            }

            // calculate edges of the shape
            tx -= Fixed.FromInt(lump.LeftOffset);
            var x1 = (centerXFrac + (tx * xscale)).Data >> Fixed.FracBits;

            // off the right side?
            if (x1 > windowWidth)
            {
                return;
            }

            tx += Fixed.FromInt(lump.Width);
            var x2 = ((centerXFrac + (tx * xscale)).Data >> Fixed.FracBits) - 1;

            // off the left side
            if (x2 < 0)
            {
                return;
            }

            // store information in a vissprite
            var vis = R_NewVisSprite();
            vis.MobjFlags = thing.Flags;
            vis.Scale = xscale;
            vis.GlobalX = thing.X;
            vis.GlobalY = thing.Y;
            vis.GlobalBottomZ = thing.Z;
            vis.GlobalTopZ = thing.Z + Fixed.FromInt(lump.TopOffset);
            vis.TextureMid = vis.GlobalTopZ - cameraZ;
            vis.X1 = x1 < 0 ? 0 : x1;
            vis.X2 = x2 >= windowWidth ? windowWidth - 1 : x2;
            var iscale = Fixed.One / xscale;

            if (flip)
            {
                vis.StartFrac = new Fixed(Fixed.FromInt(lump.Width).Data - 1);
                vis.XInvScale = -iscale;
            }
            else
            {
                vis.StartFrac = Fixed.Zero;
                vis.XInvScale = iscale;
            }

            if (vis.X1 > x1)
            {
                vis.StartFrac += vis.XInvScale * (vis.X1 - x1);
            }

            vis.Patch = lump;

            /*
            // get light level
            if (thing->flags & MF_SHADOW)
            {
                // shadow draw
                vis->colormap = NULL;
            }
            else if (fixedcolormap)
            {
                // fixed map
                vis->colormap = fixedcolormap;
            }
            else if (thing->frame & FF_FULLBRIGHT)
            {
                // full bright
                vis->colormap = colormaps;
            }

            else
            {
                // diminished light
                index = xscale >> (LIGHTSCALESHIFT - detailshift);

                if (index >= MAXLIGHTSCALE)
                    index = MAXLIGHTSCALE - 1;

                vis->colormap = spritelights[index];
            }
            */

            // diminished light
            if ((thing.Frame & 0x8000) == 0)
            {
                vis.ColorMap = spritelights[Math.Min(xscale.Data >> ScaleLightShift, MaxScaleLight - 1)];
            }
            else
            {
                vis.ColorMap = colorMap.FullBright;
            }
        }

        private VisSprite R_NewVisSprite()
        {
            var visSprite = visSprites[visSpriteCount];
            visSpriteCount++;
            return visSprite;
        }

        private void DrawSprites()
        {
            for (var i = 0; i < visSpriteCount - 1; i++)
            {
                for (var j = i + 1; j > 0; j--)
                {
                    if (visSprites[j - 1].Scale < visSprites[j].Scale)
                    {
                        var temp = visSprites[j - 1];
                        visSprites[j - 1] = visSprites[j];
                        visSprites[j] = temp;
                    }
                }
            }

            for (var i = visSpriteCount - 1; i >= 0; i--)
            {
                DrawSprite(visSprites[i]);
            }
        }

        private void DrawSprite(VisSprite vis)
        {
            for (var x = vis.X1; x <= vis.X2; x++)
            {
                lowerClip[x] = -2;
                upperClip[x] = -2;
            }

            // Scan drawsegs from end to start for obscuring segs.
            // The first drawseg that has a greater scale
            //  is the clip seg.
            for (var ds_p = visWallRangeCount - 1; ds_p >= 0; ds_p--)
            {
                var ds = visWallRanges[ds_p];

                // determine if the drawseg obscures the sprite
                if (ds.X1 > vis.X2 || ds.X2 < vis.X1 || (ds.Silhouette == 0 && ds.MaskedTextureColumn == -1))
                {
                    // does not cover sprite
                    continue;
                }

                var r1 = ds.X1 < vis.X1 ? vis.X1 : ds.X1;
                var r2 = ds.X2 > vis.X2 ? vis.X2 : ds.X2;

                Fixed lowscale;
                Fixed scale;
                if (ds.Scale1 > ds.Scale2)
                {
                    lowscale = ds.Scale2;
                    scale = ds.Scale1;
                }
                else
                {
                    lowscale = ds.Scale1;
                    scale = ds.Scale2;
                }

                if (scale < vis.Scale
                    || (lowscale < vis.Scale
                     && Geometry.PointOnSegSide(vis.GlobalX, vis.GlobalY, ds.Seg) == 0))
                {
                    // masked mid texture?
                    if (ds.MaskedTextureColumn != -1)
                    {
                        DrawMaskedRange(ds, r1, r2);
                    }
                    // seg is behind sprite
                    continue;
                }


                // clip this piece of the sprite
                var silhouette = ds.Silhouette;

                if (vis.GlobalBottomZ >= ds.LowerSilHeight)
                {
                    silhouette &= ~Silhouette.Lower;
                }

                if (vis.GlobalTopZ <= ds.UpperSilHeight)
                {
                    silhouette &= ~Silhouette.Upper;
                }

                if (silhouette == Silhouette.Lower)
                {
                    // bottom sil
                    for (var x = r1; x <= r2; x++)
                    {
                        if (lowerClip[x] == -2)
                        {
                            lowerClip[x] = clipData[ds.LowerClip + x];
                        }
                    }
                }
                else if (silhouette == Silhouette.Upper)
                {
                    // top sil
                    for (var x = r1; x <= r2; x++)
                    {
                        if (upperClip[x] == -2)
                        {
                            upperClip[x] = clipData[ds.UpperClip + x];
                        }
                    }
                }
                else if (silhouette == Silhouette.Both)
                {
                    // both
                    for (var x = r1; x <= r2; x++)
                    {
                        if (lowerClip[x] == -2)
                        {
                            lowerClip[x] = clipData[ds.LowerClip + x];
                        }
                        if (upperClip[x] == -2)
                        {
                            upperClip[x] = clipData[ds.UpperClip + x];
                        }
                    }
                }

            }

            // all clipping has been performed, so draw the sprite

            // check for unclipped columns
            for (var x = vis.X1; x <= vis.X2; x++)
            {
                if (lowerClip[x] == -2)
                {
                    lowerClip[x] = (short)windowHeight;
                }
                if (upperClip[x] == -2)
                {
                    upperClip[x] = -1;
                }
            }

            var dc_iscale = Fixed.Abs(vis.XInvScale);
            var dc_texturemid = vis.TextureMid;
            var frac = vis.StartFrac;
            var spryscale = vis.Scale;
            var sprtopscreen = centerYFrac - (dc_texturemid * spryscale);

            for (var dc_x = vis.X1; dc_x <= vis.X2; dc_x++)
            {
                var texturecolumn = frac.Data >> Fixed.FracBits;
                DrawMaskedColumn(
                    vis.Patch.Columns[texturecolumn],
                    vis.ColorMap,
                    dc_x,
                    dc_iscale,
                    dc_texturemid,
                    spryscale,
                    sprtopscreen,
                    upperClip[dc_x],
                    lowerClip[dc_x]);
                frac += vis.XInvScale;
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

        private class VisWallRange
        {
            public Seg Seg;

            public int X1;
            public int X2;

            public Fixed Scale1;
            public Fixed Scale2;
            public Fixed ScaleStep;

            public Silhouette Silhouette;
            public Fixed UpperSilHeight;
            public Fixed LowerSilHeight;

            public int UpperClip;
            public int LowerClip;
            public int MaskedTextureColumn;
        }

        [Flags]
        private enum Silhouette
        {
            Upper = 1,
            Lower = 2,
            Both = 3
        }

        private class VisSprite
        {
            // Doubly linked list.
            //public VisSprite Prev;
            //public VisSprite Next;

            public int X1;
            public int X2;

            // for line side calculation
            public Fixed GlobalX;
            public Fixed GlobalY;

            // global bottom / top for silhouette clipping
            public Fixed GlobalBottomZ;
            public Fixed GlobalTopZ;

            // horizontal position of x1
            public Fixed StartFrac;

            public Fixed Scale;

            // negative if flipped
            public Fixed XInvScale;

            public Fixed TextureMid;
            public Patch Patch;

            // for color translation and shadow draw,
            //  maxbright frames as well
            public byte[] ColorMap;

            public MobjFlags MobjFlags;
        }
    }
}
