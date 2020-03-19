using System;

namespace ManagedDoom
{
	public sealed class SectorAction
	{
		private World world;

		public SectorAction(World world)
		{
			this.world = world;
		}

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
			var tm = world.ThingMovement;

			var onfloor = (thing.Z == thing.FloorZ);

			tm.CheckPosition(thing, thing.X, thing.Y);
			// what about stranding a monster partially off an edge?

			thing.FloorZ = tm.tmfloorz;
			thing.CeilingZ = tm.tmceilingz;

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
				world.SetMobjState(thing, State.Gibs);

				thing.Flags &= ~MobjFlags.Solid;
				thing.Height = Fixed.Zero;
				thing.Radius = Fixed.Zero;

				// keep checking
				return true;
			}

			// crunch dropped items
			if ((thing.Flags & MobjFlags.Dropped) != 0)
			{
				world.ThingAllocation.RemoveMobj(thing);

				// keep checking
				return true;
			}

			if ((thing.Flags & MobjFlags.Shootable) == 0)
			{
				// assume it is bloody gibs or something
				return true;
			}

			nofit = true;

			if (crushchange && (world.levelTime & 3) == 0)
			{
				world.ThingInteraction.DamageMobj(thing, null, null, 10);

				// spray blood in a random direction
				var mo = world.ThingAllocation.SpawnMobj(
					thing.X,
					thing.Y,
					thing.Z + thing.Height / 2, MobjType.Blood);

				mo.MomX = new Fixed((world.Random.Next() - world.Random.Next()) << 12);
				mo.MomY = new Fixed((world.Random.Next() - world.Random.Next()) << 12);
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
					world.Map.BlockMap.IterateThings(x, y, mo => PIT_ChangeSector(mo));
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

		private static readonly Fixed VDOORSPEED = Fixed.FromInt(2);
		private static readonly int VDOORWAIT = 150;

		//
		// EV_VerticalDoor : open a door manually, no tag value
		//
		public void EV_VerticalDoor(LineDef line, Mobj thing)
		{
			VlDoor door;

			// only front sides can be used
			var side = 0;

			//	Check for locks
			var player = thing.Player;

			switch ((int)line.Special)
			{
				case 26: // Blue Lock
				case 32:
					if (player == null)
					{
						return;
					}

					if (!player.Cards[(int)CardType.BlueCard]
						&& !player.Cards[(int)CardType.BlueSkull])
					{
						//player.Message = PD_BLUEK;
						world.StartSound(null, Sfx.OOF);
						return;
					}
					break;

				case 27: // Yellow Lock
				case 34:
					if (player == null)
					{
						return;
					}

					if (!player.Cards[(int)CardType.YellowCard]
						&& !player.Cards[(int)CardType.YellowSkull])
					{
						//player.Message = PD_YELLOWK;
						world.StartSound(null, Sfx.OOF);
						return;
					}
					break;

				case 28: // Red Lock
				case 33:
					if (player == null)
					{
						return;
					}

					if (!player.Cards[(int)CardType.RedCard]
						&& !player.Cards[(int)CardType.RedSkull])
					{
						//player.Message = PD_REDK;
						world.StartSound(null, Sfx.OOF);
						return;
					}
					break;
			}

			// if the sector has an active thinker, use it
			//var sec = sides[line->sidenum[side ^ 1]].sector;
			var sec = line.Side1.Sector;
			//secnum = sec - sectors;

			if (sec.SpecialData != null)
			{
				door = (VlDoor)sec.SpecialData;
				switch ((int)line.Special)
				{
					case 1: // ONLY FOR "RAISE" DOORS, NOT "OPEN"s
					case 26:
					case 27:
					case 28:
					case 117:
						if (door.Direction == -1)
						{
							// go back up
							door.Direction = 1;
						}
						else
						{
							if (thing.Player == null)
							{
								// JDC: bad guys never close doors
								return;
							}

							// start going down immediately
							door.Direction = -1;
						}
						return;
				}
			}

			// for proper sound
			switch ((int)line.Special)
			{
				case 117: // BLAZING DOOR RAISE
				case 118: // BLAZING DOOR OPEN
					world.StartSound(sec.SoundOrigin, Sfx.BDOPN);
					break;

				case 1: // NORMAL DOOR SOUND
				case 31:
					world.StartSound(sec.SoundOrigin, Sfx.DOROPN);
					break;

				default:    // LOCKED DOOR SOUND
					world.StartSound(sec.SoundOrigin, Sfx.DOROPN);
					break;
			}


			// new door thinker
			door = ThinkerPool.RentVlDoor(world);
			world.Thinkers.Add(door);
			sec.SpecialData = door;
			door.Sector = sec;
			door.Direction = 1;
			door.Speed = VDOORSPEED;
			door.TopWait = VDOORWAIT;

			switch ((int)line.Special)
			{
				case 1:
				case 26:
				case 27:
				case 28:
					door.Type = VlDoorType.Normal;
					break;

				case 31:
				case 32:
				case 33:
				case 34:
					door.Type = VlDoorType.Open;
					line.Special = 0;
					break;

				case 117:   // blazing door raise
					door.Type = VlDoorType.BlazeRaise;
					door.Speed = VDOORSPEED * 4;
					break;
				case 118:   // blazing door open
					door.Type = VlDoorType.BlazeOpen;
					line.Special = 0;
					door.Speed = VDOORSPEED * 4;
					break;
			}

			// find the top and bottom of the movement range
			door.TopHeight = FindLowestCeilingSurrounding(sec);
			door.TopHeight -= Fixed.FromInt(4);
		}
	}
}
