using System;

namespace ManagedDoom
{
	public sealed partial class World
	{
		//
		// P_ThingHeightClip
		// Takes a valid thing and adjusts the thing->floorz,
		// thing->ceilingz, and possibly thing->z.
		// This is called for all nearby monsters
		// whenever a sector changes height.
		// If the thing doesn't fit,
		// the z will be set to the lowest value
		// and false will be returned.
		//
		private bool P_ThingHeightClip(Mobj thing)
		{
			var onfloor = (thing.Z == thing.FloorZ);

			CheckPosition(thing, thing.X, thing.Y);
			// what about stranding a monster partially off an edge?

			thing.FloorZ = tmfloorz;
			thing.CeilingZ = tmceilingz;

			if (onfloor)
			{
				// walking monsters rise and fall with the floor
				thing.Z = thing.FloorZ;
			}
			else
			{
				// don't adjust a floating monster unless forced to
				if (thing.Z + thing.Height > thing.CeilingZ)
				{
					thing.Z = thing.CeilingZ - thing.Height;
				}
			}

			if (thing.CeilingZ - thing.FloorZ < thing.Height)
			{
				return false;
			}

			return true;
		}



		//
		// SECTOR HEIGHT CHANGING
		// After modifying a sectors floor or ceiling height,
		// call this routine to adjust the positions
		// of all things that touch the sector.
		//
		// If anything doesn't fit anymore, true will be returned.
		// If crunch is true, they will take damage
		//  as they are being crushed.
		// If Crunch is false, you should set the sector height back
		//  the way it was and call P_ChangeSector again
		//  to undo the changes.
		//
		public bool crushchange;
		public bool nofit;


		//
		// PIT_ChangeSector
		//
		private bool PIT_ChangeSector(Mobj thing)
		{
			if (P_ThingHeightClip(thing))
			{
				// keep checking
				return true;
			}


			// crunch bodies to giblets
			if (thing.Health <= 0)
			{
				SetMobjState(thing, State.Gibs);

				thing.Flags &= ~MobjFlags.Solid;
				thing.Height = Fixed.Zero;
				thing.Radius = Fixed.Zero;

				// keep checking
				return true;
			}

			// crunch dropped items
			if ((thing.Flags & MobjFlags.Dropped) != 0)
			{
				RemoveMobj(thing);

				// keep checking
				return true;
			}

			if ((thing.Flags & MobjFlags.Shootable) == 0)
			{
				// assume it is bloody gibs or something
				return true;
			}

			nofit = true;

			if (crushchange && (levelTime & 3) == 0)
			{
				DamageMobj(thing, null, null, 10);

				// spray blood in a random direction
				var mo = SpawnMobj(
					thing.X,
					thing.Y,
					thing.Z + thing.Height / 2, MobjType.Blood);

				mo.MomX = new Fixed((random.Next() - random.Next()) << 12);
				mo.MomY = new Fixed((random.Next() - random.Next()) << 12);
			}

			// keep checking (crush other things)	
			return true;
		}



		//
		// P_ChangeSector
		//
		public bool ChangeSector(Sector sector, bool crunch)
		{
			nofit = false;
			crushchange = crunch;

			// re-check heights for all things near the moving sector
			for (var x = sector.BlockBox[Box.Left]; x <= sector.BlockBox[Box.Right]; x++)
			{
				for (var y = sector.BlockBox[Box.Bottom]; y <= sector.BlockBox[Box.Top]; y++)
				{
					map.BlockMap.IterateThings(x, y, mo => PIT_ChangeSector(mo));
				}
			}

			return nofit;
		}






		//
		// Move a plane (floor or ceiling) and check for crushing
		//
		public SectorActionResult MovePlane(
			Sector sector,
			Fixed speed,
			Fixed dest,
			bool crush,
			int floorOrCeiling,
			int direction)
		{
			switch (floorOrCeiling)
			{
				case 0:
					// FLOOR
					switch (direction)
					{
						case -1:
							// DOWN
							if (sector.FloorHeight - speed < dest)
							{
								var lastpos = sector.FloorHeight;
								sector.FloorHeight = dest;
								var flag = ChangeSector(sector, crush);
								if (flag)
								{
									sector.FloorHeight = lastpos;
									ChangeSector(sector, crush);
									//return crushed;
								}
								return SectorActionResult.PastDestination;
							}
							else
							{
								var lastpos = sector.FloorHeight;
								sector.FloorHeight -= speed;
								var flag = ChangeSector(sector, crush);
								if (flag)
								{
									sector.FloorHeight = lastpos;
									ChangeSector(sector, crush);
									return SectorActionResult.Crushed;
								}
							}
							break;

						case 1:
							// UP
							if (sector.FloorHeight + speed > dest)
							{
								var lastpos = sector.FloorHeight;
								sector.FloorHeight = dest;
								var flag = ChangeSector(sector, crush);
								if (flag)
								{
									sector.FloorHeight = lastpos;
									ChangeSector(sector, crush);
									//return crushed;
								}
								return SectorActionResult.PastDestination;
							}
							else
							{
								// COULD GET CRUSHED
								var lastpos = sector.FloorHeight;
								sector.FloorHeight += speed;
								var flag = ChangeSector(sector, crush);
								if (flag)
								{
									if (crush)
									{
										return SectorActionResult.Crushed;
									}
									sector.FloorHeight = lastpos;
									ChangeSector(sector, crush);
									return SectorActionResult.Crushed;
								}
							}
							break;
					}
					break;

				case 1:
					// CEILING
					switch (direction)
					{
						case -1:
							// DOWN
							if (sector.CeilingHeight - speed < dest)
							{
								var lastpos = sector.CeilingHeight;
								sector.CeilingHeight = dest;
								var flag = ChangeSector(sector, crush);

								if (flag)
								{
									sector.CeilingHeight = lastpos;
									ChangeSector(sector, crush);
									//return crushed;
								}
								return SectorActionResult.PastDestination;
							}
							else
							{
								// COULD GET CRUSHED
								var lastpos = sector.CeilingHeight;
								sector.CeilingHeight -= speed;
								var flag = ChangeSector(sector, crush);

								if (flag)
								{
									if (crush)
									{
										return SectorActionResult.Crushed;
									}
									sector.CeilingHeight = lastpos;
									ChangeSector(sector, crush);
									return SectorActionResult.Crushed;
								}
							}
							break;

						case 1:
							// UP
							if (sector.CeilingHeight + speed > dest)
							{
								var lastpos = sector.CeilingHeight;
								sector.CeilingHeight = dest;
								var flag = ChangeSector(sector, crush);
								if (flag)
								{
									sector.CeilingHeight = lastpos;
									ChangeSector(sector, crush);
									//return crushed;
								}
								return SectorActionResult.PastDestination;
							}
							else
							{
								var lastpos = sector.CeilingHeight;
								sector.CeilingHeight += speed;
								var flag = ChangeSector(sector, crush);
							}
							break;
					}
					break;

			}
			return SectorActionResult.OK;
		}




		//
		// getNextSector()
		// Return sector_t * of sector next to current.
		// NULL if not two-sided line
		//
		private Sector GetNextSector(LineDef line, Sector sec)
		{
			if ((line.Flags & LineFlags.TwoSided) == 0)
			{
				return null;
			}

			if (line.FrontSector == sec)
			{
				return line.BackSector;
			}

			return line.FrontSector;
		}



		//
		// P_FindLowestFloorSurrounding()
		// FIND LOWEST FLOOR HEIGHT IN SURROUNDING SECTORS
		//
		public Fixed FindLowestFloorSurrounding(Sector sec)
		{
			var floor = sec.FloorHeight;

			for (var i = 0; i < sec.Lines.Length; i++)
			{
				var check = sec.Lines[i];
				var other = GetNextSector(check, sec);

				if (other == null)
				{
					continue;
				}

				if (other.FloorHeight < floor)
				{
					floor = other.FloorHeight;
				}
			}
			return floor;
		}

		//
		// P_FindHighestFloorSurrounding()
		// FIND HIGHEST FLOOR HEIGHT IN SURROUNDING SECTORS
		//
		public Fixed FindHighestFloorSurrounding(Sector sec)
		{
			var floor = Fixed.FromInt(-500);

			for (var i = 0; i < sec.Lines.Length; i++)
			{
				var check = sec.Lines[i];
				var other = GetNextSector(check, sec);

				if (other == null)
				{
					continue;
				}

				if (other.FloorHeight > floor)
				{
					floor = other.FloorHeight;
				}
			}
			return floor;
		}



		//
		// FIND LOWEST CEILING IN THE SURROUNDING SECTORS
		//
		public Fixed FindLowestCeilingSurrounding(Sector sec)
		{
			var height = Fixed.MaxValue;

			for (var i = 0; i < sec.Lines.Length; i++)
			{
				var check = sec.Lines[i];
				var other = GetNextSector(check, sec);

				if (other == null)
				{
					continue;
				}

				if (other.CeilingHeight < height)
				{
					height = other.CeilingHeight;
				}
			}
			return height;
		}


		//
		// FIND HIGHEST CEILING IN THE SURROUNDING SECTORS
		//
		public Fixed FindHighestCeilingSurrounding(Sector sec)
		{
			var height = Fixed.Zero;

			for (var i = 0; i < sec.Lines.Length; i++)
			{
				var check = sec.Lines[i];
				var other = GetNextSector(check, sec);

				if (other == null)
				{
					continue;
				}

				if (other.CeilingHeight > height)
				{
					height = other.CeilingHeight;
				}
			}
			return height;
		}
	}
}
