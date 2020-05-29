using System;

namespace ManagedDoom
{
	public sealed class SectorAction
	{
		private World world;

		public SectorAction(World world)
		{
			this.world = world;

			InitSectorChange();
		}


		private bool crushChange;
		private bool noFit;
		private Func<Mobj, bool> crushThingFunc;

		private void InitSectorChange()
		{
			crushThingFunc = CrushThing;
		}

		private bool ThingHeightClip(Mobj thing)
		{
			var onFloor = (thing.Z == thing.FloorZ);

			var tm = world.ThingMovement;

			tm.CheckPosition(thing, thing.X, thing.Y);
			// What about stranding a monster partially off an edge?

			thing.FloorZ = tm.CurrentFloorZ;
			thing.CeilingZ = tm.CurrentCeilingZ;

			if (onFloor)
			{
				// Walking monsters rise and fall with the floor.
				thing.Z = thing.FloorZ;
			}
			else
			{
				// Don't adjust a floating monster unless forced to.
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

		private bool CrushThing(Mobj thing)
		{
			if (ThingHeightClip(thing))
			{
				// Keep checking.
				return true;
			}

			// Crunch bodies to giblets.
			if (thing.Health <= 0)
			{
				thing.SetState(MobjState.Gibs);

				thing.Flags &= ~MobjFlags.Solid;
				thing.Height = Fixed.Zero;
				thing.Radius = Fixed.Zero;

				// Keep checking.
				return true;
			}

			// Crunch dropped items.
			if ((thing.Flags & MobjFlags.Dropped) != 0)
			{
				world.ThingAllocation.RemoveMobj(thing);

				// Keep checking.
				return true;
			}

			if ((thing.Flags & MobjFlags.Shootable) == 0)
			{
				// Assume it is bloody gibs or something.
				return true;
			}

			noFit = true;

			if (crushChange && (world.levelTime & 3) == 0)
			{
				world.ThingInteraction.DamageMobj(thing, null, null, 10);

				// Spray blood in a random direction.
				var blood = world.ThingAllocation.SpawnMobj(
					thing.X,
					thing.Y,
					thing.Z + thing.Height / 2,
					MobjType.Blood);

				var random = world.Random;

				blood.MomX = new Fixed((random.Next() - random.Next()) << 12);
				blood.MomY = new Fixed((random.Next() - random.Next()) << 12);
			}

			// Keep checking (crush other things).	
			return true;
		}

		public bool ChangeSector(Sector sector, bool crunch)
		{
			noFit = false;
			crushChange = crunch;

			// Re-check heights for all things near the moving sector.
			for (var x = sector.BlockBox[Box.Left]; x <= sector.BlockBox[Box.Right]; x++)
			{
				for (var y = sector.BlockBox[Box.Bottom]; y <= sector.BlockBox[Box.Top]; y++)
				{
					world.Map.BlockMap.IterateThings(x, y, crushThingFunc);
				}
			}

			return noFit;
		}


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
								var lastPos = sector.FloorHeight;
								sector.FloorHeight = dest;
								if (ChangeSector(sector, crush))
								{
									sector.FloorHeight = lastPos;
									ChangeSector(sector, crush);
								}
								return SectorActionResult.PastDestination;
							}
							else
							{
								var lastPos = sector.FloorHeight;
								sector.FloorHeight -= speed;
								if (ChangeSector(sector, crush))
								{
									sector.FloorHeight = lastPos;
									ChangeSector(sector, crush);
									return SectorActionResult.Crushed;
								}
							}
							break;

						case 1:
							// UP
							if (sector.FloorHeight + speed > dest)
							{
								var lastPos = sector.FloorHeight;
								sector.FloorHeight = dest;
								if (ChangeSector(sector, crush))
								{
									sector.FloorHeight = lastPos;
									ChangeSector(sector, crush);
								}
								return SectorActionResult.PastDestination;
							}
							else
							{
								// COULD GET CRUSHED
								var lastPos = sector.FloorHeight;
								sector.FloorHeight += speed;
								if (ChangeSector(sector, crush))
								{
									if (crush)
									{
										return SectorActionResult.Crushed;
									}
									sector.FloorHeight = lastPos;
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
								var lastPos = sector.CeilingHeight;
								sector.CeilingHeight = dest;
								if (ChangeSector(sector, crush))
								{
									sector.CeilingHeight = lastPos;
									ChangeSector(sector, crush);
								}
								return SectorActionResult.PastDestination;
							}
							else
							{
								// COULD GET CRUSHED
								var lastPos = sector.CeilingHeight;
								sector.CeilingHeight -= speed;
								if (ChangeSector(sector, crush))
								{
									if (crush)
									{
										return SectorActionResult.Crushed;
									}
									sector.CeilingHeight = lastPos;
									ChangeSector(sector, crush);
									return SectorActionResult.Crushed;
								}
							}
							break;

						case 1:
							// UP
							if (sector.CeilingHeight + speed > dest)
							{
								var lastPos = sector.CeilingHeight;
								sector.CeilingHeight = dest;
								if (ChangeSector(sector, crush))
								{
									sector.CeilingHeight = lastPos;
									ChangeSector(sector, crush);
								}
								return SectorActionResult.PastDestination;
							}
							else
							{
								sector.CeilingHeight += speed;
								ChangeSector(sector, crush);
							}
							break;
					}
					break;
			}

			return SectorActionResult.OK;
		}


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

		private Fixed FindLowestFloorSurrounding(Sector sec)
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

		private Fixed FindHighestFloorSurrounding(Sector sec)
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

		private Fixed FindLowestCeilingSurrounding(Sector sec)
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

		private Fixed FindHighestCeilingSurrounding(Sector sec)
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

		private int FindSectorFromLineTag(LineDef line, int start)
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


		private static readonly Fixed doorSpeed = Fixed.FromInt(2);
		private static readonly int doorWait = 150;

		public void VerticalDoor(LineDef line, Mobj thing)
		{
			//	Check for locks.
			var player = thing.Player;

			switch ((int)line.Special)
			{
				// Blue Lock.
				case 26:
				case 32:
					if (player == null)
					{
						return;
					}

					if (!player.Cards[(int)CardType.BlueCard] &&
						!player.Cards[(int)CardType.BlueSkull])
					{
						//player.Message = PD_BLUEK;
						world.StartSound(null, Sfx.OOF);
						return;
					}
					break;

				// Yellow Lock
				case 27:
				case 34:
					if (player == null)
					{
						return;
					}

					if (!player.Cards[(int)CardType.YellowCard] &&
						!player.Cards[(int)CardType.YellowSkull])
					{
						//player.Message = PD_YELLOWK;
						world.StartSound(null, Sfx.OOF);
						return;
					}
					break;

				// Red Lock
				case 28:
				case 33:
					if (player == null)
					{
						return;
					}

					if (!player.Cards[(int)CardType.RedCard] &&
						!player.Cards[(int)CardType.RedSkull])
					{
						//player.Message = PD_REDK;
						world.StartSound(null, Sfx.OOF);
						return;
					}
					break;
			}

			var sector = line.Side1.Sector;

			// If the sector has an active thinker, use it.
			if (sector.SpecialData != null)
			{
				var door = (VlDoor)sector.SpecialData;
				switch ((int)line.Special)
				{
					// Only for "raise" doors, not "open"s.
					case 1:
					case 26:
					case 27:
					case 28:
					case 117:
						if (door.Direction == -1)
						{
							// Go back up.
							door.Direction = 1;
						}
						else
						{
							if (thing.Player == null)
							{
								// Bad guys never close doors.
								return;
							}

							// Start going down immediately.
							door.Direction = -1;
						}
						return;
				}
			}

			// For proper sound.
			switch ((int)line.Special)
			{
				// Blazing door raise.
				case 117:

				// Blazing door open.
				case 118:
					world.StartSound(sector.SoundOrigin, Sfx.BDOPN);
					break;

				// Normal door sound.
				case 1:
				case 31:
					world.StartSound(sector.SoundOrigin, Sfx.DOROPN);
					break;

				// Locked door sound.
				default:
					world.StartSound(sector.SoundOrigin, Sfx.DOROPN);
					break;
			}

			// New door thinker.
			var newDoor = ThinkerPool.RentVlDoor(world);
			world.Thinkers.Add(newDoor);
			sector.SpecialData = newDoor;
			newDoor.Sector = sector;
			newDoor.Direction = 1;
			newDoor.Speed = doorSpeed;
			newDoor.TopWait = doorWait;

			switch ((int)line.Special)
			{
				case 1:
				case 26:
				case 27:
				case 28:
					newDoor.Type = VlDoorType.Normal;
					break;

				case 31:
				case 32:
				case 33:
				case 34:
					newDoor.Type = VlDoorType.Open;
					line.Special = 0;
					break;

				// Blazing door raise.
				case 117:
					newDoor.Type = VlDoorType.BlazeRaise;
					newDoor.Speed = doorSpeed * 4;
					break;

				// Blazing door open.
				case 118:
					newDoor.Type = VlDoorType.BlazeOpen;
					line.Special = 0;
					newDoor.Speed = doorSpeed * 4;
					break;
			}

			// Find the top and bottom of the movement range.
			newDoor.TopHeight = FindLowestCeilingSurrounding(sector);
			newDoor.TopHeight -= Fixed.FromInt(4);
		}


		private static readonly int maxAdjoiningSectorCount = 40;
		private Fixed[] heightList = new Fixed[maxAdjoiningSectorCount];

		private Fixed FindNextHighestFloor(Sector sec, Fixed currentHeight)
		{
			var height = currentHeight;

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
					heightList[h++] = other.FloorHeight;
				}

				// Check for overflow.
				if (h >= heightList.Length)
				{
					// Exit.
					throw new Exception("Sector with more than 40 adjoining sectors");
				}
			}

			// Find lowest height in list.
			if (h == 0)
			{
				return currentHeight;
			}

			var min = heightList[0];

			// Range checking? 
			for (var i = 1; i < h; i++)
			{
				if (heightList[i] < min)
				{
					min = heightList[i];
				}
			}

			return min;
		}


		public bool DoDoor(LineDef line, VlDoorType type)
		{
			var setcorNumber = -1;
			var result = false;

			var sectors = world.Map.Sectors;

			while ((setcorNumber = FindSectorFromLineTag(line, setcorNumber)) >= 0)
			{
				var sector = sectors[setcorNumber];
				if (sector.SpecialData != null)
				{
					continue;
				}

				// New door thinker.

				result = true;

				var door = ThinkerPool.RentVlDoor(world);
				world.Thinkers.Add(door);
				sector.SpecialData = door;
				door.Sector = sector;
				door.Type = type;
				door.TopWait = doorWait;
				door.Speed = doorSpeed;

				switch (type)
				{
					case VlDoorType.BlazeClose:
						door.TopHeight = FindLowestCeilingSurrounding(sector);
						door.TopHeight -= Fixed.FromInt(4);
						door.Direction = -1;
						door.Speed = doorSpeed * 4;
						world.StartSound(door.Sector.SoundOrigin, Sfx.BDCLS);
						break;

					case VlDoorType.Close:
						door.TopHeight = FindLowestCeilingSurrounding(sector);
						door.TopHeight -= Fixed.FromInt(4);
						door.Direction = -1;
						world.StartSound(door.Sector.SoundOrigin, Sfx.DORCLS);
						break;

					case VlDoorType.Close30ThenOpen:
						door.TopHeight = sector.CeilingHeight;
						door.Direction = -1;
						world.StartSound(door.Sector.SoundOrigin, Sfx.DORCLS);
						break;

					case VlDoorType.BlazeRaise:
					case VlDoorType.BlazeOpen:
						door.Direction = 1;
						door.TopHeight = FindLowestCeilingSurrounding(sector);
						door.TopHeight -= Fixed.FromInt(4);
						door.Speed = doorSpeed * 4;
						if (door.TopHeight != sector.CeilingHeight)
						{
							world.StartSound(door.Sector.SoundOrigin, Sfx.BDOPN);
						}
						break;

					case VlDoorType.Normal:
					case VlDoorType.Open:
						door.Direction = 1;
						door.TopHeight = FindLowestCeilingSurrounding(sector);
						door.TopHeight -= Fixed.FromInt(4);
						if (door.TopHeight != sector.CeilingHeight)
						{
							world.StartSound(door.Sector.SoundOrigin, Sfx.DOROPN);
						}
						break;

					default:
						break;
				}

			}

			return result;
		}

		public bool DoLockedDoor(LineDef line, VlDoorType type, Mobj thing)
		{
			var player = thing.Player;

			if (player == null)
			{
				return false;
			}

			switch ((int)line.Special)
			{
				// Blue Lock
				case 99:
				case 133:
					if (player == null)
					{
						return false;
					}
					if (!player.Cards[(int)CardType.BlueCard] &&
						!player.Cards[(int)CardType.BlueSkull])
					{
						//p->message = PD_BLUEO;
						world.StartSound(null, Sfx.OOF);
						return false;
					}
					break;

				// Red Lock
				case 134:
				case 135:
					if (player == null)
					{
						return false;
					}
					if (!player.Cards[(int)CardType.RedCard] &&
						!player.Cards[(int)CardType.RedSkull])
					{
						//p->message = PD_REDO;
						world.StartSound(null, Sfx.OOF);
						return false;
					}
					break;

				// Yellow Lock
				case 136:
				case 137:
					if (player == null)
					{
						return false;
					}
					if (!player.Cards[(int)CardType.YellowCard] &&
						!player.Cards[(int)CardType.YellowSkull])
					{
						//p->message = PD_YELLOWO;
						world.StartSound(null, Sfx.OOF);
						return false;
					}
					break;
			}

			return DoDoor(line, type);
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
					//activeplats[i].Active = true;
					activeplats[i].ThinkerState = ThinkerState.Active;
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
					//activeplats[j].Active = false;
					activeplats[j].ThinkerState = ThinkerState.InStasis;
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




		//
		// BUILD A STAIRCASE!
		//
		public bool EV_BuildStairs(LineDef line, StairType type)
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
				floor.Direction = 1;
				floor.Sector = sec;
				Fixed speed;
				Fixed stairsize;
				switch (type)
				{
					case StairType.Build8:
						speed = FLOORSPEED / 4;
						stairsize = Fixed.FromInt(8);
						break;
					case StairType.Turbo16:
						speed = FLOORSPEED * 4;
						stairsize = Fixed.FromInt(16);
						break;
					default:
						throw new Exception("Unknown stair type!");
				}
				floor.Speed = speed;
				var height = sec.FloorHeight + stairsize;
				floor.FloorDestHeight = height;

				var texture = sec.FloorFlat;

				// Find next sector to raise
				// 1.	Find 2-sided line with same sector side[0]
				// 2.	Other side is the next sector to raise
				bool ok;
				do
				{
					ok = false;
					for (var i = 0; i < sec.Lines.Length; i++)
					{
						if (((sec.Lines[i]).Flags & LineFlags.TwoSided) == 0)
						{
							continue;
						}

						var tsec = (sec.Lines[i]).FrontSector;
						var newsecnum = tsec.Number;

						if (secnum != newsecnum)
						{
							continue;
						}

						tsec = (sec.Lines[i]).BackSector;
						newsecnum = tsec.Number;

						if (tsec.FloorFlat != texture)
						{
							continue;
						}

						height += stairsize;

						if (tsec.SpecialData != null)
						{
							continue;
						}

						sec = tsec;
						secnum = newsecnum;
						floor = ThinkerPool.RentFloorMove(world);

						world.Thinkers.Add(floor);

						sec.SpecialData = floor;
						floor.Direction = 1;
						floor.Sector = sec;
						floor.Speed = speed;
						floor.FloorDestHeight = height;
						ok = true;
						break;
					}
				} while (ok);
			}
			return rtn;
		}








		public static readonly Fixed CEILSPEED = Fixed.One;
		public static readonly int CEILWAIT = 150;
		private static readonly int MAXCEILINGS = 30;

		private CeilingMove[] activeceilings = new CeilingMove[MAXCEILINGS];

		//
		// Add an active ceiling
		//
		public void AddActiveCeiling(CeilingMove c)
		{
			for (var i = 0; i < activeceilings.Length; i++)
			{
				if (activeceilings[i] == null)
				{
					activeceilings[i] = c;
					return;
				}
			}
		}



		//
		// Remove a ceiling's thinker
		//
		public void RemoveActiveCeiling(CeilingMove c)
		{
			for (var i = 0; i < activeceilings.Length; i++)
			{
				if (activeceilings[i] == c)
				{
					activeceilings[i].Sector.SpecialData = null;
					world.Thinkers.Remove(activeceilings[i]);
					activeceilings[i] = null;
					break;
				}
			}
		}



		//
		// Restart a ceiling that's in-stasis
		//
		public void ActivateInStasisCeiling(LineDef line)
		{
			for (var i = 0; i < activeceilings.Length; i++)
			{
				if (activeceilings[i] != null
					&& (activeceilings[i].Tag == line.Tag)
					&& (activeceilings[i].Direction == 0))
				{
					activeceilings[i].Direction = activeceilings[i].OldDirection;
					//activeceilings[i].Active = true;
					activeceilings[i].ThinkerState = ThinkerState.Active;
				}
			}
		}



		//
		// EV_CeilingCrushStop
		// Stop a ceiling from crushing!
		//
		public bool EV_CeilingCrushStop(LineDef line)
		{
			var rtn = false;

			for (var i = 0; i < activeceilings.Length; i++)
			{
				if (activeceilings[i] != null
					&& (activeceilings[i].Tag == line.Tag)
					&& (activeceilings[i].Direction != 0))
				{
					activeceilings[i].OldDirection = activeceilings[i].Direction;
					//activeceilings[i].Active = false;
					activeceilings[i].ThinkerState = ThinkerState.InStasis;
					activeceilings[i].Direction = 0; // in-stasis
					rtn = true;
				}
			}

			return rtn;
		}




		//
		// EV_DoCeiling
		// Move a ceiling up/down and all around!
		//
		public bool EV_DoCeiling(LineDef line, CeilingMoveType type)
		{
			var secnum = -1;
			var rtn = false;

			//	Reactivate in-stasis ceilings...for certain types.
			switch (type)
			{
				case CeilingMoveType.FastCrushAndRaise:
				case CeilingMoveType.SilentCrushAndRaise:
				case CeilingMoveType.CrushAndRaise:
					ActivateInStasisCeiling(line);
					break;
				default:
					break;
			}

			while ((secnum = FindSectorFromLineTag(line, secnum)) >= 0)
			{
				var sec = world.Map.Sectors[secnum];
				if (sec.SpecialData != null)
				{
					continue;
				}

				// new door thinker
				rtn = true;
				var ceiling = ThinkerPool.RentCeiligMove(world);
				world.Thinkers.Add(ceiling);
				sec.SpecialData = ceiling;
				ceiling.Sector = sec;
				ceiling.Crush = false;

				switch (type)
				{
					case CeilingMoveType.FastCrushAndRaise:
						ceiling.Crush = true;
						ceiling.TopHeight = sec.CeilingHeight;
						ceiling.BottomHeight = sec.FloorHeight + Fixed.FromInt(8);
						ceiling.Direction = -1;
						ceiling.Speed = CEILSPEED * 2;
						break;

					case CeilingMoveType.SilentCrushAndRaise:
					case CeilingMoveType.CrushAndRaise:
					case CeilingMoveType.LowerAndCrush:
					case CeilingMoveType.LowerToFloor:
						if (type == CeilingMoveType.SilentCrushAndRaise
							|| type == CeilingMoveType.CrushAndRaise)
						{
							ceiling.Crush = true;
							ceiling.TopHeight = sec.CeilingHeight;
						}
						ceiling.BottomHeight = sec.FloorHeight;
						if (type != CeilingMoveType.LowerToFloor)
						{
							ceiling.BottomHeight += Fixed.FromInt(8);
						}
						ceiling.Direction = -1;
						ceiling.Speed = CEILSPEED;
						break;

					case CeilingMoveType.RaiseToHighest:
						ceiling.TopHeight = FindHighestCeilingSurrounding(sec);
						ceiling.Direction = 1;
						ceiling.Speed = CEILSPEED;
						break;
				}

				ceiling.Tag = sec.Tag;
				ceiling.Type = type;
				AddActiveCeiling(ceiling);
			}
			return rtn;
		}




		public bool EV_DoDonut(LineDef line)
		{
			var sectors = world.Map.Sectors;

			var secnum = -1;
			var rtn = false;
			while ((secnum = FindSectorFromLineTag(line, secnum)) >= 0)
			{
				var s1 = sectors[secnum];

				// ALREADY MOVING?  IF SO, KEEP GOING...
				if (s1.SpecialData != null)
				{
					continue;
				}

				rtn = true;

				var s2 = GetNextSector(s1.Lines[0], s1);

				if (s2 == null)
				{
					break;
				}

				for (var i = 0; i < s2.Lines.Length; i++)
				{
					var s3 = s2.Lines[i].BackSector;

					if (s3 == s1)
					{
						continue;
					}

					if (s3 == null)
					{
						return rtn;
					}

					var thinkers = world.Thinkers;

					//	Spawn rising slime.
					var floor1 = ThinkerPool.RentFloorMove(world);
					thinkers.Add(floor1);
					s2.SpecialData = floor1;
					floor1.Type = FloorMoveType.DonutRaise;
					floor1.Crush = false;
					floor1.Direction = 1;
					floor1.Sector = s2;
					floor1.Speed = FLOORSPEED / 2;
					floor1.Texture = s3.FloorFlat;
					floor1.NewSpecial = 0;
					floor1.FloorDestHeight = s3.FloorHeight;

					//	Spawn lowering donut-hole.
					var floor2 = ThinkerPool.RentFloorMove(world);
					thinkers.Add(floor2);
					s1.SpecialData = floor2;
					floor2.Type = FloorMoveType.LowerFloor;
					floor2.Crush = false;
					floor2.Direction = -1;
					floor2.Sector = s1;
					floor2.Speed = FLOORSPEED / 2;
					floor2.FloorDestHeight = s3.FloorHeight;

					break;
				}
			}

			return rtn;
		}











		//
		// Spawn a door that closes after 30 seconds
		//
		public void P_SpawnDoorCloseIn30(Sector sec)
		{
			var door = ThinkerPool.RentVlDoor(world);

			world.Thinkers.Add(door);

			sec.SpecialData = door;
			sec.Special = 0;

			door.Sector = sec;
			door.Direction = 0;
			door.Type = VlDoorType.Normal;
			door.Speed = doorSpeed;
			door.TopCountDown = 30 * 35;
		}

		//
		// Spawn a door that opens after 5 minutes
		//
		public void P_SpawnDoorRaiseIn5Mins(Sector sec)
		{
			var door = ThinkerPool.RentVlDoor(world);

			world.Thinkers.Add(door);

			sec.SpecialData = door;
			sec.Special = 0;

			door.Sector = sec;
			door.Direction = 2;
			door.Type = VlDoorType.RaiseIn5Mins;
			door.Speed = doorSpeed;
			door.TopHeight = FindLowestCeilingSurrounding(sec);
			door.TopHeight -= Fixed.FromInt(4);
			door.TopWait = doorWait;
			door.TopCountDown = 5 * 60 * 35;
		}
	}
}
