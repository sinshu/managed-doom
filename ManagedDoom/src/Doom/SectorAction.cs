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

			thing.FloorZ = tm.CurrentFloorZ;
			thing.CeilingZ = tm.CurrentCeilingZ;

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
				thing.SetState(State.Gibs);

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

















		//
		// RETURN NEXT SECTOR # THAT LINE TAG REFERS TO
		//
		public int FindSectorFromLineTag(LineDef line, int start)
		{
			var sectors = world.Map.Sectors;

			for (var i = start + 1; i < sectors.Length; i++)
			{
				if (sectors[i].Tag == line.Tag)
				{
					return i;
				}
			}
			return -1;
		}











		//
		// P_FindNextHighestFloor
		// FIND NEXT HIGHEST FLOOR IN SURROUNDING SECTORS
		// Note: this should be doable w/o a fixed array.

		// 20 adjoining sectors max!
		private static readonly int MAX_ADJOINING_SECTORS = 20;
		private Fixed[] heightlist = new Fixed[MAX_ADJOINING_SECTORS];

		public Fixed FindNextHighestFloor(Sector sec, Fixed currentheight)
		{
			var height = currentheight;
			var h = 0;
			for (var i = 0; i < sec.Lines.Length; i++)
			{
				var check = sec.Lines[i];
				var other = GetNextSector(check, sec);

				if (other == null)
				{
					continue;
				}

				if (other.FloorHeight > height)
				{
					heightlist[h++] = other.FloorHeight;
				}

				// Check for overflow. Exit.
				if (h >= heightlist.Length)
				{
					throw new Exception("Sector with more than 20 adjoining sectors");
				}
			}

			// Find lowest height in list
			if (h == 0)
			{
				return currentheight;
			}

			var min = heightlist[0];

			// Range checking? 
			for (var i = 1; i < h; i++)
			{
				if (heightlist[i] < min)
				{
					min = heightlist[i];
				}
			}

			return min;
		}



		private static readonly Fixed DOORSPEED = Fixed.FromInt(2);

		public bool EV_DoDoor(LineDef line, VlDoorType type)
		{
			var secnum = -1;
			var rtn = false;

			var sectors = world.Map.Sectors;
			while ((secnum = FindSectorFromLineTag(line, secnum)) >= 0)
			{
				var sec = sectors[secnum];
				if (sec.SpecialData != null)
				{
					continue;
				}

				// new door thinker
				rtn = true;
				var door = ThinkerPool.RentVlDoor(world);
				world.Thinkers.Add(door);
				sec.SpecialData = door;

				door.Sector = sec;
				door.Type = type;
				door.TopWait = VDOORWAIT;
				door.Speed = VDOORSPEED;

				switch (type)
				{
					case VlDoorType.BlazeClose:
						door.TopHeight = FindLowestCeilingSurrounding(sec);
						door.TopHeight -= Fixed.FromInt(4);
						door.Direction = -1;
						door.Speed = VDOORSPEED * 4;
						world.StartSound(door.Sector.SoundOrigin, Sfx.BDCLS);
						break;

					case VlDoorType.Close:
						door.TopHeight = FindLowestCeilingSurrounding(sec);
						door.TopHeight -= Fixed.FromInt(4);
						door.Direction = -1;
						world.StartSound(door.Sector.SoundOrigin, Sfx.DORCLS);
						break;

					case VlDoorType.Close30ThenOpen:
						door.TopHeight = sec.CeilingHeight;
						door.Direction = -1;
						world.StartSound(door.Sector.SoundOrigin, Sfx.DORCLS);
						break;

					case VlDoorType.BlazeRaise:
					case VlDoorType.BlazeOpen:
						door.Direction = 1;
						door.TopHeight = FindLowestCeilingSurrounding(sec);
						door.TopHeight -= Fixed.FromInt(4);
						door.Speed = VDOORSPEED * 4;
						if (door.TopHeight != sec.CeilingHeight)
						{
							world.StartSound(door.Sector.SoundOrigin, Sfx.BDOPN);
						}
						break;

					case VlDoorType.Normal:
					case VlDoorType.Open:
						door.Direction = 1;
						door.TopHeight = FindLowestCeilingSurrounding(sec);
						door.TopHeight -= Fixed.FromInt(4);
						if (door.TopHeight != sec.CeilingHeight)
						{
							world.StartSound(door.Sector.SoundOrigin, Sfx.DOROPN);
						}
						break;

					default:
						break;
				}

			}
			return rtn;
		}









		private static readonly int PLATWAIT = 3;
		private static readonly Fixed PLATSPEED = Fixed.One;

		//
		// Do Platforms
		//  "amount" is only used for SOME platforms.
		//
		public bool EV_DoPlat(LineDef line, PlatformType type, int amount)
		{
			var secnum = -1;
			var rtn = false;

			//	Activate all <type> plats that are in_stasis
			switch (type)
			{
				case PlatformType.PerpetualRaise:
					ActivateInStasis(line.Tag);
					break;

				default:
					break;
			}

			var sectors = world.Map.Sectors;
			var sides = world.Map.Sides;
			while ((secnum = FindSectorFromLineTag(line, secnum)) >= 0)
			{
				var sec = sectors[secnum];

				if (sec.SpecialData != null)
				{
					continue;
				}

				// Find lowest & highest floors around sector
				rtn = true;
				var plat = ThinkerPool.RentPlatform(world);
				world.Thinkers.Add(plat);

				plat.Type = type;
				plat.Sector = sec;
				plat.Sector.SpecialData = plat;
				plat.Crush = false;
				plat.Tag = line.Tag;

				switch (type)
				{
					case PlatformType.RaiseToNearestAndChange:
						plat.Speed = PLATSPEED / 2;
						sec.FloorFlat = line.Side0.Sector.FloorFlat;
						plat.High = FindNextHighestFloor(sec, sec.FloorHeight);
						plat.Wait = 0;
						plat.Status = PlatformState.Up;
						// NO MORE DAMAGE, IF APPLICABLE
						sec.Special = 0;

						world.StartSound(sec.SoundOrigin, Sfx.STNMOV);
						break;

					case PlatformType.RaiseAndChange:
						plat.Speed = PLATSPEED / 2;
						sec.FloorFlat = line.Side0.Sector.FloorFlat;
						plat.High = sec.FloorHeight + amount * Fixed.One;
						plat.Wait = 0;
						plat.Status = PlatformState.Up;

						world.StartSound(sec.SoundOrigin, Sfx.STNMOV);
						break;

					case PlatformType.DownWaitUpStay:
						plat.Speed = PLATSPEED * 4;
						plat.Low = FindLowestFloorSurrounding(sec);

						if (plat.Low > sec.FloorHeight)
						{
							plat.Low = sec.FloorHeight;
						}

						plat.High = sec.FloorHeight;
						plat.Wait = 35 * PLATWAIT;
						plat.Status = PlatformState.Down;
						world.StartSound(sec.SoundOrigin, Sfx.PSTART);
						break;

					case PlatformType.BlazeDwus:
						plat.Speed = PLATSPEED * 8;
						plat.Low = FindLowestFloorSurrounding(sec);

						if (plat.Low > sec.FloorHeight)
						{
							plat.Low = sec.FloorHeight;
						}

						plat.High = sec.FloorHeight;
						plat.Wait = 35 * PLATWAIT;
						plat.Status = PlatformState.Down;
						world.StartSound(sec.SoundOrigin, Sfx.PSTART);
						break;

					case PlatformType.PerpetualRaise:
						plat.Speed = PLATSPEED;
						plat.Low = FindLowestFloorSurrounding(sec);

						if (plat.Low > sec.FloorHeight)
						{
							plat.Low = sec.FloorHeight;
						}

						plat.High = FindHighestFloorSurrounding(sec);

						if (plat.High < sec.FloorHeight)
						{
							plat.High = sec.FloorHeight;
						}

						plat.Wait = 35 * PLATWAIT;
						plat.Status = (PlatformState)(world.Random.Next() & 1);

						world.StartSound(sec.SoundOrigin, Sfx.PSTART);
						break;
				}

				AddActivePlat(plat);
			}
			return rtn;
		}



		private static readonly int MAXPLATS = 30;
		private Platform[] activeplats = new Platform[MAXPLATS];

		public void ActivateInStasis(int tag)
		{
			for (var i = 0; i < activeplats.Length; i++)
			{
				if (activeplats[i] != null
					&& activeplats[i].Tag == tag
					&& activeplats[i].Status == PlatformState.InStasis)
				{
					activeplats[i].Status = activeplats[i].Oldstatus;
					activeplats[i].Active = true;
				}
			}
		}

		public void EV_StopPlat(LineDef line)
		{
			for (var j = 0; j < activeplats.Length; j++)
			{
				if (activeplats[j] != null
					&& activeplats[j].Status != PlatformState.InStasis
					&& activeplats[j].Tag == line.Tag)
				{
					activeplats[j].Oldstatus = activeplats[j].Status;
					activeplats[j].Status = PlatformState.InStasis;
					activeplats[j].Active = false;
				}
			}
		}

		public void AddActivePlat(Platform plat)
		{
			for (var i = 0; i < activeplats.Length; i++)
			{
				if (activeplats[i] == null)
				{
					activeplats[i] = plat;
					return;
				}
			}

			throw new Exception("P_AddActivePlat: no more plats!");
		}

		public void RemoveActivePlat(Platform plat)
		{
			for (var i = 0; i < activeplats.Length; i++)
			{
				if (plat == activeplats[i])
				{
					activeplats[i].Sector.SpecialData = null;
					world.Thinkers.Remove(activeplats[i]);
					activeplats[i] = null;

					return;
				}
			}

			throw new Exception("P_RemoveActivePlat: can't find plat!");
		}









		//
		// TELEPORTATION
		//
		public bool EV_Teleport(LineDef line, int side, Mobj thing)
		{
			// don't teleport missiles
			if ((thing.Flags & MobjFlags.Missile) != 0)
			{
				return false;
			}

			// Don't teleport if hit back of line,
			//  so you can get out of teleporter.
			if (side == 1)
			{
				return false;
			}

			var tag = line.Tag;
			var sectors = world.Map.Sectors;
			for (var i = 0; i < sectors.Length; i++)
			{
				if (sectors[i].Tag == tag)
				{
					foreach (var thinker in world.Thinkers)
					{
						var m = thinker as Mobj;

						if (m == null)
						{
							// not a mobj
							continue;
						}

						// not a teleportman
						if (m.Type != MobjType.Teleportman)
						{
							continue;
						}

						var sector = m.Subsector.Sector;

						// wrong sector
						if (sector.Number != i)
						{
							continue;
						}

						var oldx = thing.X;
						var oldy = thing.Y;
						var oldz = thing.Z;

						if (!world.ThingMovement.TeleportMove(thing, m.X, m.Y))
						{
							return false;
						}

						thing.Z = thing.FloorZ; //fixme: not needed?
						if (thing.Player != null)
						{
							thing.Player.ViewZ = thing.Z + thing.Player.ViewHeight;
						}

						// spawn teleport fog at source and destination
						var fog = world.ThingAllocation.SpawnMobj(oldx, oldy, oldz, MobjType.Tfog);
						world.StartSound(fog, Sfx.TELEPT);
						var an = m.Angle; // >> ANGLETOFINESHIFT;
						fog = world.ThingAllocation.SpawnMobj(
							m.X + 20 * Trig.Cos(an),
							m.Y + 20 * Trig.Sin(an),
							thing.Z, MobjType.Tfog);

						// emit sound, where?
						world.StartSound(fog, Sfx.TELEPT);

						// don't move for a bit
						if (thing.Player != null)
						{
							thing.ReactionTime = 18;
						}

						thing.Angle = m.Angle;
						thing.MomX = thing.MomY = thing.MomZ = Fixed.Zero;
						return true;
					}
				}
			}
			return false;
		}









		//
		// Start strobing lights (usually from a trigger)
		//
		public void EV_StartLightStrobing(LineDef line)
		{
			var secnum = -1;
			while ((secnum = FindSectorFromLineTag(line, secnum)) >= 0)
			{
				var sec = world.Map.Sectors[secnum];
				if (sec.SpecialData != null)
				{
					continue;
				}

				world.LightingChange.SpawnStrobeFlash(sec, StrobeFlash.SLOWDARK, 0);
			}
		}



		//
		// TURN LINE'S TAG LIGHTS OFF
		//
		public void EV_TurnTagLightsOff(LineDef line)
		{
			var sectors = world.Map.Sectors;
			for (var j = 0; j < sectors.Length; j++)
			{
				var sector = sectors[j];
				if (sector.Tag == line.Tag)
				{
					var min = sector.LightLevel;
					for (var i = 0; i < sector.Lines.Length; i++)
					{
						var templine = sector.Lines[i];
						var tsec = GetNextSector(templine, sector);
						if (tsec == null)
						{
							continue;
						}
						if (tsec.LightLevel < min)
						{
							min = tsec.LightLevel;
						}
					}
					sector.LightLevel = min;
				}
			}
		}


		//
		// TURN LINE'S TAG LIGHTS ON
		//
		public void EV_LightTurnOn(LineDef line, int bright)
		{
			var sectors = world.Map.Sectors;
			for (var i = 0; i < sectors.Length; i++)
			{
				var sector = sectors[i];
				if (sector.Tag == line.Tag)
				{
					// bright = 0 means to search
					// for highest light level
					// surrounding sector
					if (bright == 0)
					{
						for (var j = 0; j < sector.Lines.Length; j++)
						{
							var templine = sector.Lines[j];
							var temp = GetNextSector(templine, sector);

							if (temp == null)
							{
								continue;
							}

							if (temp.LightLevel > bright)
							{
								bright = temp.LightLevel;
							}
						}
					}
					sector.LightLevel = bright;
				}
			}
		}










		private static readonly Fixed FLOORSPEED = Fixed.One;

		//
		// HANDLE FLOOR TYPES
		//
		public bool EV_DoFloor(LineDef line, FloorMoveType floortype)
		{
			var secnum = -1;
			var rtn = false;
			while ((secnum = FindSectorFromLineTag(line, secnum)) >= 0)
			{
				var sec = world.Map.Sectors[secnum];

				// ALREADY MOVING?  IF SO, KEEP GOING...
				if (sec.SpecialData != null)
				{
					continue;
				}

				// new floor thinker
				rtn = true;
				var floor = ThinkerPool.RentFloorMove(world);
				world.Thinkers.Add(floor);
				sec.SpecialData = floor;
				floor.Type = floortype;
				floor.Crush = false;

				switch (floortype)
				{
					case FloorMoveType.LowerFloor:
						floor.Direction = -1;
						floor.Sector = sec;
						floor.Speed = FLOORSPEED;
						floor.FloorDestHeight = FindHighestFloorSurrounding(sec);
						break;

					case FloorMoveType.LowerFloorToLowest:
						floor.Direction = -1;
						floor.Sector = sec;
						floor.Speed = FLOORSPEED;
						floor.FloorDestHeight = FindLowestFloorSurrounding(sec);
						break;

					case FloorMoveType.TurboLower:
						floor.Direction = -1;
						floor.Sector = sec;
						floor.Speed = FLOORSPEED * 4;
						floor.FloorDestHeight = FindHighestFloorSurrounding(sec);
						if (floor.FloorDestHeight != sec.FloorHeight)
						{
							floor.FloorDestHeight += Fixed.FromInt(8);
						}
						break;

					case FloorMoveType.RaiseFloorCrush:
					case FloorMoveType.RaiseFloor:
						if (floortype == FloorMoveType.RaiseFloorCrush)
						{
							floor.Crush = true;
						}
						floor.Direction = 1;
						floor.Sector = sec;
						floor.Speed = FLOORSPEED;
						floor.FloorDestHeight = FindLowestCeilingSurrounding(sec);
						if (floor.FloorDestHeight > sec.CeilingHeight)
						{
							floor.FloorDestHeight = sec.CeilingHeight;
						}
						floor.FloorDestHeight -= Fixed.FromInt(8) * (floortype == FloorMoveType.RaiseFloorCrush ? 1 : 0);
						break;

					case FloorMoveType.RaiseFloorTurbo:
						floor.Direction = 1;
						floor.Sector = sec;
						floor.Speed = FLOORSPEED * 4;
						floor.FloorDestHeight = FindNextHighestFloor(sec, sec.FloorHeight);
						break;

					case FloorMoveType.RaiseFloorToNearest:
						floor.Direction = 1;
						floor.Sector = sec;
						floor.Speed = FLOORSPEED;
						floor.FloorDestHeight = FindNextHighestFloor(sec, sec.FloorHeight);
						break;

					case FloorMoveType.RaiseFloor24:
						floor.Direction = 1;
						floor.Sector = sec;
						floor.Speed = FLOORSPEED;
						floor.FloorDestHeight = floor.Sector.FloorHeight + Fixed.FromInt(24);
						break;

					case FloorMoveType.RaiseFloor512:
						floor.Direction = 1;
						floor.Sector = sec;
						floor.Speed = FLOORSPEED;
						floor.FloorDestHeight = floor.Sector.FloorHeight + Fixed.FromInt(512);
						break;

					case FloorMoveType.RaiseFloor24AndChange:
						floor.Direction = 1;
						floor.Sector = sec;
						floor.Speed = FLOORSPEED;
						floor.FloorDestHeight = floor.Sector.FloorHeight + Fixed.FromInt(24);
						sec.FloorFlat = line.FrontSector.FloorFlat;
						sec.Special = line.FrontSector.Special;
						break;

					case FloorMoveType.RaiseToTexture:
						{
							var minsize = int.MaxValue;

							floor.Direction = 1;
							floor.Sector = sec;
							floor.Speed = FLOORSPEED;
							var textures = world.Map.Textures;
							for (var i = 0; i < sec.Lines.Length; i++)
							{
								if ((sec.Lines[i].Flags & LineFlags.TwoSided) != 0)
								{
									var side = sec.Lines[i].Side0;
									if (side.BottomTexture >= 0)
									{
										if (textures[side.BottomTexture].Height < minsize)
										{
											minsize = textures[side.BottomTexture].Height;
										}
									}
									side = sec.Lines[i].Side1;
									if (side.BottomTexture >= 0)
									{
										if (textures[side.BottomTexture].Height < minsize)
										{
											minsize = textures[side.BottomTexture].Height;
										}
									}
								}
							}
							floor.FloorDestHeight = floor.Sector.FloorHeight + Fixed.FromInt(minsize);
						}
						break;

					case FloorMoveType.LowerAndChange:
						floor.Direction = -1;
						floor.Sector = sec;
						floor.Speed = FLOORSPEED;
						floor.FloorDestHeight = FindLowestFloorSurrounding(sec);
						floor.Texture = sec.FloorFlat;

						for (var i = 0; i < sec.Lines.Length; i++)
						{
							if ((sec.Lines[i].Flags & LineFlags.TwoSided) != 0)
							{
								if (sec.Lines[i].Side0.Sector.Number == secnum)
								{
									sec = sec.Lines[i].Side1.Sector;

									if (sec.FloorHeight == floor.FloorDestHeight)
									{
										floor.Texture = sec.FloorFlat;
										floor.NewSpecial = sec.Special;
										break;
									}
								}
								else
								{
									sec = sec.Lines[i].Side0.Sector;

									if (sec.FloorHeight == floor.FloorDestHeight)
									{
										floor.Texture = sec.FloorFlat;
										floor.NewSpecial = sec.Special;
										break;
									}
								}
							}
						}
						break;
				}
			}
			return rtn;
		}
	}
}
