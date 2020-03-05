using System;

namespace ManagedDoom
{
	public sealed partial class World
	{
		// eye z of looker
		private Fixed sightzstart;

		// from t1 to t2
		private DivLine strace;
		private Fixed t2x;
		private Fixed t2y;

		private int[] sightcounts;

		private void InitSight()
		{
			strace = new DivLine();
			sightcounts = new int[2];
		}

		//
		// P_InterceptVector2
		// Returns the fractional intercept point
		// along the first divline.
		// This is only called by the addthings and addlines traversers.
		//
		private Fixed P_InterceptVector2(DivLine v2, DivLine v1)
		{
			var den = new Fixed(v1.Dy.Data >> 8) * v2.Dx - new Fixed(v1.Dx.Data >> 8) * v2.Dy;

			if (den == Fixed.Zero)
			{
				return Fixed.Zero;
			}

			var num =
			new Fixed((v1.X - v2.X).Data >> 8) * v1.Dy
			+ new Fixed((v2.Y - v1.Y).Data >> 8) * v1.Dx;

			var frac = num / den;

			return frac;
		}


		//
		// P_CrossSubsector
		// Returns true
		//  if strace crosses the given subsector successfully.
		//
		public bool CrossSubsector(int num, int validcount)
		{
			var sub = map.Subsectors[num];

			// check lines
			var count = sub.SegCount;

			for (var i = 0; i < count; i++)
			{
				var seg = map.Segs[sub.FirstSeg + i];
				var line = seg.LineDef;

				// allready checked other side?
				if (line.ValidCount == validcount)
				{
					continue;
				}

				line.ValidCount = validcount;

				var v1 = line.Vertex1;
				var v2 = line.Vertex2;
				var s1 = Geometry.DivLineSide(v1.X, v1.Y, strace);
				var s2 = Geometry.DivLineSide(v2.X, v2.Y, strace);

				// line isn't crossed?
				if (s1 == s2)
				{
					continue;
				}


				tempDiv.X = v1.X;
				tempDiv.Y = v1.Y;
				tempDiv.Dx = v2.X - v1.X;
				tempDiv.Dy = v2.Y - v1.Y;
				s1 = Geometry.DivLineSide(strace.X, strace.Y, tempDiv);
				s2 = Geometry.DivLineSide(t2x, t2y, tempDiv);

				// line isn't crossed?
				if (s1 == s2)
				{
					continue;
				}

				// stop because it is not two sided anyway
				// might do this after updating validcount?
				if ((line.Flags & LineFlags.TwoSided) == 0)
				{
					return false;
				}

				// crosses a two sided line
				var front = seg.FrontSector;
				var back = seg.BackSector;

				// no wall to block sight with?
				if (front.FloorHeight == back.FloorHeight
					&& front.CeilingHeight == back.CeilingHeight)
				{
					continue;
				}

				// possible occluder
				// because of ceiling height differences
				if (front.CeilingHeight < back.CeilingHeight)
				{
					openTop = front.CeilingHeight;
				}
				else
				{
					openTop = back.CeilingHeight;
				}

				// because of ceiling height differences
				if (front.FloorHeight > back.FloorHeight)
				{
					openBottom = front.FloorHeight;
				}
				else
				{
					openBottom = back.FloorHeight;
				}

				// quick test for totally closed doors
				if (openBottom >= openTop)
				{
					// stop
					return false;
				}

				var frac = P_InterceptVector2(strace, tempDiv);

				if (front.FloorHeight != back.FloorHeight)
				{
					var slope = (openBottom - sightzstart) / frac;
					if (slope > bottomslope)
					{
						bottomslope = slope;
					}
				}

				if (front.CeilingHeight != back.CeilingHeight)
				{
					var slope = (openTop - sightzstart) / frac;
					if (slope < topslope)
					{
						topslope = slope;
					}
				}

				if (topslope <= bottomslope)
				{
					// stop
					return false;
				}
			}

			// passed the subsector ok
			return true;
		}


		//
		// P_CrossBSPNode
		// Returns true
		//  if strace crosses the given node successfully.
		//
		public bool CrossBSPNode(int bspnum, int validCount)
		{
			if (Node.IsSubsector(bspnum))
			{
				if (bspnum == -1)
				{
					return CrossSubsector(0, validCount);
				}
				else
				{
					return CrossSubsector(Node.GetSubsector(bspnum), validCount);
				}
			}

			var bsp = map.Nodes[bspnum];

			// decide which side the start point is on
			var side = Geometry.DivLineSide(strace.X, strace.Y, bsp);
			if (side == 2)
			{
				// an "on" should cross both sides
				side = 0;
			}

			// cross the starting side
			if (!CrossBSPNode(bsp.Children[side], validCount))
			{
				return false;
			}

			// the partition plane is crossed here
			if (side == Geometry.DivLineSide(t2x, t2y, bsp))
			{
				// the line doesn't touch the other side
				return true;
			}

			// cross the ending side		
			return CrossBSPNode(bsp.Children[side ^ 1], validCount);
		}


		//
		// P_CheckSight
		// Returns true
		//  if a straight line between t1 and t2 is unobstructed.
		// Uses REJECT.
		//
		public bool CheckSight(Mobj t1, Mobj t2)
		{
			// First check for trivial rejection.
			// Check in REJECT table.
			if (map.Reject.Check(t1.Subsector.Sector, t2.Subsector.Sector))
			{
				sightcounts[0]++;

				// can't possibly be connected
				return false;
			}

			// An unobstructed LOS is possible.
			// Now look from eyes of t1 to any part of t2.
			sightcounts[1]++;

			sightzstart = t1.Z + t1.Height - new Fixed(t1.Height.Data >> 2);
			topslope = (t2.Z + t2.Height) - sightzstart;
			bottomslope = (t2.Z) - sightzstart;

			strace.X = t1.X;
			strace.Y = t1.Y;
			t2x = t2.X;
			t2y = t2.Y;
			strace.Dx = t2.X - t1.X;
			strace.Dy = t2.Y - t1.Y;

			// the head node is the last node output
			return CrossBSPNode(map.Nodes.Length - 1, GetNewValidCount());
		}
	}
}
