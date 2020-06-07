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

			var bm = world.Map.BlockMap;
			var blockBox = sector.BlockBox;

			// Re-check heights for all things near the moving sector.
			for (var x = blockBox.Left(); x <= blockBox.Right(); x++)
			{
				for (var y = blockBox.Bottom(); y <= blockBox.Top(); y++)
				{
					bm.IterateThings(x, y, crushThingFunc);
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


		private Sector GetNextSector(LineDef line, Sector sector)
		{
			if ((line.Flags & LineFlags.TwoSided) == 0)
			{
				return null;
			}

			if (line.FrontSector == sector)
			{
				return line.BackSector;
			}

			return line.FrontSector;
		}

		private Fixed FindLowestFloorSurrounding(Sector sector)
		{
			var floor = sector.FloorHeight;

			for (var i = 0; i < sector.Lines.Length; i++)
			{
				var check = sector.Lines[i];

				var other = GetNextSector(check, sector);
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

		private Fixed FindHighestFloorSurrounding(Sector sector)
		{
			var floor = Fixed.FromInt(-500);

			for (var i = 0; i < sector.Lines.Length; i++)
			{
				var check = sector.Lines[i];

				var other = GetNextSector(check, sector);
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

		private Fixed FindLowestCeilingSurrounding(Sector sector)
		{
			var height = Fixed.MaxValue;

			for (var i = 0; i < sector.Lines.Length; i++)
			{
				var check = sector.Lines[i];

				var other = GetNextSector(check, sector);
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

		private Fixed FindHighestCeilingSurrounding(Sector sector)
		{
			var height = Fixed.Zero;

			for (var i = 0; i < sector.Lines.Length; i++)
			{
				var check = sector.Lines[i];

				var other = GetNextSector(check, sector);
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
						player.SendMessage(DoomInfo.Strings.PD_BLUEK);
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
						player.SendMessage(DoomInfo.Strings.PD_YELLOWK);
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
						player.SendMessage(DoomInfo.Strings.PD_REDK);
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

		private Fixed FindNextHighestFloor(Sector sector, Fixed currentHeight)
		{
			var height = currentHeight;
			var h = 0;

			for (var i = 0; i < sector.Lines.Length; i++)
			{
				var check = sector.Lines[i];

				var other = GetNextSector(check, sector);
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
			var sectors = world.Map.Sectors;
			var setcorNumber = -1;
			var result = false;

			while ((setcorNumber = FindSectorFromLineTag(line, setcorNumber)) >= 0)
			{
				var sector = sectors[setcorNumber];
				if (sector.SpecialData != null)
				{
					continue;
				}

				result = true;

				// New door thinker.
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
						player.SendMessage(DoomInfo.Strings.PD_BLUEO);
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
						player.SendMessage(DoomInfo.Strings.PD_REDO);
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
						player.SendMessage(DoomInfo.Strings.PD_YELLOWO);
						world.StartSound(null, Sfx.OOF);
						return false;
					}
					break;
			}

			return DoDoor(line, type);
		}


		private static readonly int platformWait = 3;
		private static readonly Fixed platformSpeed = Fixed.One;

		public bool DoPlatform(LineDef line, PlatformType type, int amount)
		{
			//	Activate all <type> plats that are in stasis.
			switch (type)
			{
				case PlatformType.PerpetualRaise:
					ActivateInStasis(line.Tag);
					break;

				default:
					break;
			}

			var sectors = world.Map.Sectors;
			var sectorNumber = -1;
			var result = false;

			while ((sectorNumber = FindSectorFromLineTag(line, sectorNumber)) >= 0)
			{
				var sector = sectors[sectorNumber];
				if (sector.SpecialData != null)
				{
					continue;
				}

				result = true;

				// Find lowest & highest floors around sector.
				var plat = ThinkerPool.RentPlatform(world);
				world.Thinkers.Add(plat);
				plat.Type = type;
				plat.Sector = sector;
				plat.Sector.SpecialData = plat;
				plat.Crush = false;
				plat.Tag = line.Tag;

				switch (type)
				{
					case PlatformType.RaiseToNearestAndChange:
						plat.Speed = platformSpeed / 2;
						sector.FloorFlat = line.Side0.Sector.FloorFlat;
						plat.High = FindNextHighestFloor(sector, sector.FloorHeight);
						plat.Wait = 0;
						plat.Status = PlatformState.Up;
						// NO MORE DAMAGE, IF APPLICABLE.
						sector.Special = 0;
						world.StartSound(sector.SoundOrigin, Sfx.STNMOV);
						break;

					case PlatformType.RaiseAndChange:
						plat.Speed = platformSpeed / 2;
						sector.FloorFlat = line.Side0.Sector.FloorFlat;
						plat.High = sector.FloorHeight + amount * Fixed.One;
						plat.Wait = 0;
						plat.Status = PlatformState.Up;
						world.StartSound(sector.SoundOrigin, Sfx.STNMOV);
						break;

					case PlatformType.DownWaitUpStay:
						plat.Speed = platformSpeed * 4;
						plat.Low = FindLowestFloorSurrounding(sector);
						if (plat.Low > sector.FloorHeight)
						{
							plat.Low = sector.FloorHeight;
						}
						plat.High = sector.FloorHeight;
						plat.Wait = 35 * platformWait;
						plat.Status = PlatformState.Down;
						world.StartSound(sector.SoundOrigin, Sfx.PSTART);
						break;

					case PlatformType.BlazeDwus:
						plat.Speed = platformSpeed * 8;
						plat.Low = FindLowestFloorSurrounding(sector);
						if (plat.Low > sector.FloorHeight)
						{
							plat.Low = sector.FloorHeight;
						}
						plat.High = sector.FloorHeight;
						plat.Wait = 35 * platformWait;
						plat.Status = PlatformState.Down;
						world.StartSound(sector.SoundOrigin, Sfx.PSTART);
						break;

					case PlatformType.PerpetualRaise:
						plat.Speed = platformSpeed;
						plat.Low = FindLowestFloorSurrounding(sector);
						if (plat.Low > sector.FloorHeight)
						{
							plat.Low = sector.FloorHeight;
						}
						plat.High = FindHighestFloorSurrounding(sector);
						if (plat.High < sector.FloorHeight)
						{
							plat.High = sector.FloorHeight;
						}
						plat.Wait = 35 * platformWait;
						plat.Status = (PlatformState)(world.Random.Next() & 1);
						world.StartSound(sector.SoundOrigin, Sfx.PSTART);
						break;
				}

				AddActivePlatform(plat);
			}

			return result;
		}



		private static readonly int maxPlatformCount = 60;
		private Platform[] activePlatforms = new Platform[maxPlatformCount];

		public void ActivateInStasis(int tag)
		{
			for (var i = 0; i < activePlatforms.Length; i++)
			{
				if (activePlatforms[i] != null &&
					activePlatforms[i].Tag == tag &&
					activePlatforms[i].Status == PlatformState.InStasis)
				{
					activePlatforms[i].Status = activePlatforms[i].Oldstatus;
					activePlatforms[i].ThinkerState = ThinkerState.Active;
				}
			}
		}

		public void StopPlatform(LineDef line)
		{
			for (var j = 0; j < activePlatforms.Length; j++)
			{
				if (activePlatforms[j] != null &&
					activePlatforms[j].Status != PlatformState.InStasis &&
					activePlatforms[j].Tag == line.Tag)
				{
					activePlatforms[j].Oldstatus = activePlatforms[j].Status;
					activePlatforms[j].Status = PlatformState.InStasis;
					activePlatforms[j].ThinkerState = ThinkerState.InStasis;
				}
			}
		}

		public void AddActivePlatform(Platform platform)
		{
			for (var i = 0; i < activePlatforms.Length; i++)
			{
				if (activePlatforms[i] == null)
				{
					activePlatforms[i] = platform;

					return;
				}
			}

			throw new Exception("Too many active platforms!");
		}

		public void RemoveActivePlatform(Platform platform)
		{
			for (var i = 0; i < activePlatforms.Length; i++)
			{
				if (platform == activePlatforms[i])
				{
					activePlatforms[i].Sector.SpecialData = null;
					world.Thinkers.Remove(activePlatforms[i]);
					activePlatforms[i] = null;
					return;
				}
			}

			throw new Exception("The platform was not found.");
		}


		public bool Teleport(LineDef line, int side, Mobj thing)
		{
			// Don't teleport missiles.
			if ((thing.Flags & MobjFlags.Missile) != 0)
			{
				return false;
			}

			// Don't teleport if hit back of line, so you can get out of teleporter.
			if (side == 1)
			{
				return false;
			}

			var sectors = world.Map.Sectors;
			var tag = line.Tag;

			for (var i = 0; i < sectors.Length; i++)
			{
				if (sectors[i].Tag == tag)
				{
					foreach (var thinker in world.Thinkers)
					{
						var dest = thinker as Mobj;

						if (dest == null)
						{
							// Not a mobj.
							continue;
						}

						// Not a teleportman.
						if (dest.Type != MobjType.Teleportman)
						{
							continue;
						}

						var sector = dest.Subsector.Sector;

						// Wrong sector.
						if (sector.Number != i)
						{
							continue;
						}

						var oldX = thing.X;
						var oldY = thing.Y;
						var oldZ = thing.Z;

						if (!world.ThingMovement.TeleportMove(thing, dest.X, dest.Y))
						{
							return false;
						}

						thing.Z = thing.FloorZ;

						if (thing.Player != null)
						{
							thing.Player.ViewZ = thing.Z + thing.Player.ViewHeight;
						}

						var ta = world.ThingAllocation;

						// Spawn teleport fog at source position.
						var fog1 = ta.SpawnMobj(
							oldX,
							oldY,
							oldZ,
							MobjType.Tfog);
						world.StartSound(fog1, Sfx.TELEPT);

						// Destination position.
						var angle = dest.Angle;
						var fog2 = ta.SpawnMobj(
							dest.X + 20 * Trig.Cos(angle),
							dest.Y + 20 * Trig.Sin(angle),
							thing.Z,
							MobjType.Tfog);
						world.StartSound(fog2, Sfx.TELEPT);

						// Don't move for a bit.
						if (thing.Player != null)
						{
							thing.ReactionTime = 18;
						}

						thing.Angle = dest.Angle;
						thing.MomX = thing.MomY = thing.MomZ = Fixed.Zero;

						return true;
					}
				}
			}

			return false;
		}


		public void StartLightStrobing(LineDef line)
		{
			var sectors = world.Map.Sectors;
			var sectorNumber = -1;

			while ((sectorNumber = FindSectorFromLineTag(line, sectorNumber)) >= 0)
			{
				var sector = sectors[sectorNumber];

				if (sector.SpecialData != null)
				{
					continue;
				}

				world.LightingChange.SpawnStrobeFlash(sector, StrobeFlash.SLOWDARK, 0);
			}
		}

		public void TurnTagLightsOff(LineDef line)
		{
			var sectors = world.Map.Sectors;

			for (var i = 0; i < sectors.Length; i++)
			{
				var sector = sectors[i];

				if (sector.Tag == line.Tag)
				{
					var min = sector.LightLevel;

					for (var j = 0; j < sector.Lines.Length; j++)
					{
						var target = GetNextSector(sector.Lines[j], sector);
						if (target == null)
						{
							continue;
						}

						if (target.LightLevel < min)
						{
							min = target.LightLevel;
						}
					}

					sector.LightLevel = min;
				}
			}
		}

		public void LightTurnOn(LineDef line, int bright)
		{
			var sectors = world.Map.Sectors;

			for (var i = 0; i < sectors.Length; i++)
			{
				var sector = sectors[i];

				if (sector.Tag == line.Tag)
				{
					// bright = 0 means to search for highest light level surrounding sector.
					if (bright == 0)
					{
						for (var j = 0; j < sector.Lines.Length; j++)
						{
							var target = GetNextSector(sector.Lines[j], sector);
							if (target == null)
							{
								continue;
							}

							if (target.LightLevel > bright)
							{
								bright = target.LightLevel;
							}
						}
					}

					sector.LightLevel = bright;
				}
			}
		}


		private static readonly Fixed floorSpeed = Fixed.One;

		public bool DoFloor(LineDef line, FloorMoveType type)
		{
			var sectors = world.Map.Sectors;
			var sectorNumber = -1;
			var result = false;

			while ((sectorNumber = FindSectorFromLineTag(line, sectorNumber)) >= 0)
			{
				var sector = sectors[sectorNumber];

				// ALREADY MOVING?  IF SO, KEEP GOING...
				if (sector.SpecialData != null)
				{
					continue;
				}

				result = true;

				// New floor thinker.
				var floor = ThinkerPool.RentFloorMove(world);
				world.Thinkers.Add(floor);
				sector.SpecialData = floor;
				floor.Type = type;
				floor.Crush = false;

				switch (type)
				{
					case FloorMoveType.LowerFloor:
						floor.Direction = -1;
						floor.Sector = sector;
						floor.Speed = floorSpeed;
						floor.FloorDestHeight = FindHighestFloorSurrounding(sector);
						break;

					case FloorMoveType.LowerFloorToLowest:
						floor.Direction = -1;
						floor.Sector = sector;
						floor.Speed = floorSpeed;
						floor.FloorDestHeight = FindLowestFloorSurrounding(sector);
						break;

					case FloorMoveType.TurboLower:
						floor.Direction = -1;
						floor.Sector = sector;
						floor.Speed = floorSpeed * 4;
						floor.FloorDestHeight = FindHighestFloorSurrounding(sector);
						if (floor.FloorDestHeight != sector.FloorHeight)
						{
							floor.FloorDestHeight += Fixed.FromInt(8);
						}
						break;

					case FloorMoveType.RaiseFloorCrush:
					case FloorMoveType.RaiseFloor:
						if (type == FloorMoveType.RaiseFloorCrush)
						{
							floor.Crush = true;
						}
						floor.Direction = 1;
						floor.Sector = sector;
						floor.Speed = floorSpeed;
						floor.FloorDestHeight = FindLowestCeilingSurrounding(sector);
						if (floor.FloorDestHeight > sector.CeilingHeight)
						{
							floor.FloorDestHeight = sector.CeilingHeight;
						}
						floor.FloorDestHeight -= Fixed.FromInt(8) * (type == FloorMoveType.RaiseFloorCrush ? 1 : 0);
						break;

					case FloorMoveType.RaiseFloorTurbo:
						floor.Direction = 1;
						floor.Sector = sector;
						floor.Speed = floorSpeed * 4;
						floor.FloorDestHeight = FindNextHighestFloor(sector, sector.FloorHeight);
						break;

					case FloorMoveType.RaiseFloorToNearest:
						floor.Direction = 1;
						floor.Sector = sector;
						floor.Speed = floorSpeed;
						floor.FloorDestHeight = FindNextHighestFloor(sector, sector.FloorHeight);
						break;

					case FloorMoveType.RaiseFloor24:
						floor.Direction = 1;
						floor.Sector = sector;
						floor.Speed = floorSpeed;
						floor.FloorDestHeight = floor.Sector.FloorHeight + Fixed.FromInt(24);
						break;

					case FloorMoveType.RaiseFloor512:
						floor.Direction = 1;
						floor.Sector = sector;
						floor.Speed = floorSpeed;
						floor.FloorDestHeight = floor.Sector.FloorHeight + Fixed.FromInt(512);
						break;

					case FloorMoveType.RaiseFloor24AndChange:
						floor.Direction = 1;
						floor.Sector = sector;
						floor.Speed = floorSpeed;
						floor.FloorDestHeight = floor.Sector.FloorHeight + Fixed.FromInt(24);
						sector.FloorFlat = line.FrontSector.FloorFlat;
						sector.Special = line.FrontSector.Special;
						break;

					case FloorMoveType.RaiseToTexture:
						var min = int.MaxValue;
						floor.Direction = 1;
						floor.Sector = sector;
						floor.Speed = floorSpeed;
						var textures = world.Map.Textures;
						for (var i = 0; i < sector.Lines.Length; i++)
						{
							if ((sector.Lines[i].Flags & LineFlags.TwoSided) != 0)
							{
								var frontSide = sector.Lines[i].Side0;
								if (frontSide.BottomTexture >= 0)
								{
									if (textures[frontSide.BottomTexture].Height < min)
									{
										min = textures[frontSide.BottomTexture].Height;
									}
								}
								var backSide = sector.Lines[i].Side1;
								if (backSide.BottomTexture >= 0)
								{
									if (textures[backSide.BottomTexture].Height < min)
									{
										min = textures[backSide.BottomTexture].Height;
									}
								}
							}
						}
						floor.FloorDestHeight = floor.Sector.FloorHeight + Fixed.FromInt(min);
						break;

					case FloorMoveType.LowerAndChange:
						floor.Direction = -1;
						floor.Sector = sector;
						floor.Speed = floorSpeed;
						floor.FloorDestHeight = FindLowestFloorSurrounding(sector);
						floor.Texture = sector.FloorFlat;
						for (var i = 0; i < sector.Lines.Length; i++)
						{
							if ((sector.Lines[i].Flags & LineFlags.TwoSided) != 0)
							{
								if (sector.Lines[i].Side0.Sector.Number == sectorNumber)
								{
									sector = sector.Lines[i].Side1.Sector;
									if (sector.FloorHeight == floor.FloorDestHeight)
									{
										floor.Texture = sector.FloorFlat;
										floor.NewSpecial = sector.Special;
										break;
									}
								}
								else
								{
									sector = sector.Lines[i].Side0.Sector;
									if (sector.FloorHeight == floor.FloorDestHeight)
									{
										floor.Texture = sector.FloorFlat;
										floor.NewSpecial = sector.Special;
										break;
									}
								}
							}
						}
						break;
				}
			}

			return result;
		}


		public bool BuildStairs(LineDef line, StairType type)
		{
			var sectors = world.Map.Sectors;
			var sectorNumber = -1;
			var result = false;

			while ((sectorNumber = FindSectorFromLineTag(line, sectorNumber)) >= 0)
			{
				var sector = sectors[sectorNumber];

				// ALREADY MOVING?  IF SO, KEEP GOING...
				if (sector.SpecialData != null)
				{
					continue;
				}

				result = true;

				// New floor thinker.
				var floor = ThinkerPool.RentFloorMove(world);
				world.Thinkers.Add(floor);
				sector.SpecialData = floor;
				floor.Direction = 1;
				floor.Sector = sector;

				Fixed speed;
				Fixed stairSize;
				switch (type)
				{
					case StairType.Build8:
						speed = floorSpeed / 4;
						stairSize = Fixed.FromInt(8);
						break;
					case StairType.Turbo16:
						speed = floorSpeed * 4;
						stairSize = Fixed.FromInt(16);
						break;
					default:
						throw new Exception("Unknown stair type!");
				}

				floor.Speed = speed;
				var height = sector.FloorHeight + stairSize;
				floor.FloorDestHeight = height;

				var texture = sector.FloorFlat;

				// Find next sector to raise.
				//     1. Find 2-sided line with same sector side[0].
				//     2. Other side is the next sector to raise.
				bool ok;
				do
				{
					ok = false;

					for (var i = 0; i < sector.Lines.Length; i++)
					{
						if (((sector.Lines[i]).Flags & LineFlags.TwoSided) == 0)
						{
							continue;
						}

						var target = (sector.Lines[i]).FrontSector;
						var newSectorNumber = target.Number;

						if (sectorNumber != newSectorNumber)
						{
							continue;
						}

						target = (sector.Lines[i]).BackSector;
						newSectorNumber = target.Number;

						if (target.FloorFlat != texture)
						{
							continue;
						}

						height += stairSize;

						if (target.SpecialData != null)
						{
							continue;
						}

						sector = target;
						sectorNumber = newSectorNumber;
						floor = ThinkerPool.RentFloorMove(world);

						world.Thinkers.Add(floor);

						sector.SpecialData = floor;
						floor.Direction = 1;
						floor.Sector = sector;
						floor.Speed = speed;
						floor.FloorDestHeight = height;
						ok = true;
						break;
					}
				} while (ok);
			}

			return result;
		}








		public static readonly Fixed CeilingSpeed = Fixed.One;
		public static readonly int CeilingWwait = 150;

		private static readonly int maxCeilingCount = 30;

		private CeilingMove[] activeCeilings = new CeilingMove[maxCeilingCount];

		public void AddActiveCeiling(CeilingMove ceiling)
		{
			for (var i = 0; i < activeCeilings.Length; i++)
			{
				if (activeCeilings[i] == null)
				{
					activeCeilings[i] = ceiling;

					return;
				}
			}
		}

		public void RemoveActiveCeiling(CeilingMove ceiling)
		{
			for (var i = 0; i < activeCeilings.Length; i++)
			{
				if (activeCeilings[i] == ceiling)
				{
					activeCeilings[i].Sector.SpecialData = null;
					world.Thinkers.Remove(activeCeilings[i]);
					activeCeilings[i] = null;
					break;
				}
			}
		}

		public void ActivateInStasisCeiling(LineDef line)
		{
			for (var i = 0; i < activeCeilings.Length; i++)
			{
				if (activeCeilings[i] != null &&
					activeCeilings[i].Tag == line.Tag &&
					activeCeilings[i].Direction == 0)
				{
					activeCeilings[i].Direction = activeCeilings[i].OldDirection;
					activeCeilings[i].ThinkerState = ThinkerState.Active;
				}
			}
		}

		public bool CeilingCrushStop(LineDef line)
		{
			var result = false;

			for (var i = 0; i < activeCeilings.Length; i++)
			{
				if (activeCeilings[i] != null &&
					activeCeilings[i].Tag == line.Tag &&
					activeCeilings[i].Direction != 0)
				{
					activeCeilings[i].OldDirection = activeCeilings[i].Direction;
					activeCeilings[i].ThinkerState = ThinkerState.InStasis;
					activeCeilings[i].Direction = 0;
					result = true;
				}
			}

			return result;
		}

		public bool DoCeiling(LineDef line, CeilingMoveType type)
		{
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

			var sectors = world.Map.Sectors;
			var sectorNumber = -1;
			var result = false;

			while ((sectorNumber = FindSectorFromLineTag(line, sectorNumber)) >= 0)
			{
				var sector = sectors[sectorNumber];
				if (sector.SpecialData != null)
				{
					continue;
				}

				result = true;

				// New door thinker.
				var ceiling = ThinkerPool.RentCeiligMove(world);
				world.Thinkers.Add(ceiling);
				sector.SpecialData = ceiling;
				ceiling.Sector = sector;
				ceiling.Crush = false;

				switch (type)
				{
					case CeilingMoveType.FastCrushAndRaise:
						ceiling.Crush = true;
						ceiling.TopHeight = sector.CeilingHeight;
						ceiling.BottomHeight = sector.FloorHeight + Fixed.FromInt(8);
						ceiling.Direction = -1;
						ceiling.Speed = CeilingSpeed * 2;
						break;

					case CeilingMoveType.SilentCrushAndRaise:
					case CeilingMoveType.CrushAndRaise:
					case CeilingMoveType.LowerAndCrush:
					case CeilingMoveType.LowerToFloor:
						if (type == CeilingMoveType.SilentCrushAndRaise
							|| type == CeilingMoveType.CrushAndRaise)
						{
							ceiling.Crush = true;
							ceiling.TopHeight = sector.CeilingHeight;
						}
						ceiling.BottomHeight = sector.FloorHeight;
						if (type != CeilingMoveType.LowerToFloor)
						{
							ceiling.BottomHeight += Fixed.FromInt(8);
						}
						ceiling.Direction = -1;
						ceiling.Speed = CeilingSpeed;
						break;

					case CeilingMoveType.RaiseToHighest:
						ceiling.TopHeight = FindHighestCeilingSurrounding(sector);
						ceiling.Direction = 1;
						ceiling.Speed = CeilingSpeed;
						break;
				}

				ceiling.Tag = sector.Tag;
				ceiling.Type = type;
				AddActiveCeiling(ceiling);
			}

			return result;
		}


		public bool DoDonut(LineDef line)
		{
			var sectors = world.Map.Sectors;
			var sectorNumber = -1;
			var result = false;

			while ((sectorNumber = FindSectorFromLineTag(line, sectorNumber)) >= 0)
			{
				var s1 = sectors[sectorNumber];

				// ALREADY MOVING?  IF SO, KEEP GOING...
				if (s1.SpecialData != null)
				{
					continue;
				}

				result = true;

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
						// Undefined behavior in Vanilla Doom.
						return result;
					}

					var thinkers = world.Thinkers;

					// Spawn rising slime.
					var floor1 = ThinkerPool.RentFloorMove(world);
					thinkers.Add(floor1);
					s2.SpecialData = floor1;
					floor1.Type = FloorMoveType.DonutRaise;
					floor1.Crush = false;
					floor1.Direction = 1;
					floor1.Sector = s2;
					floor1.Speed = floorSpeed / 2;
					floor1.Texture = s3.FloorFlat;
					floor1.NewSpecial = 0;
					floor1.FloorDestHeight = s3.FloorHeight;

					// Spawn lowering donut-hole.
					var floor2 = ThinkerPool.RentFloorMove(world);
					thinkers.Add(floor2);
					s1.SpecialData = floor2;
					floor2.Type = FloorMoveType.LowerFloor;
					floor2.Crush = false;
					floor2.Direction = -1;
					floor2.Sector = s1;
					floor2.Speed = floorSpeed / 2;
					floor2.FloorDestHeight = s3.FloorHeight;

					break;
				}
			}

			return result;
		}


		public void SpawnDoorCloseIn30(Sector sector)
		{
			var door = ThinkerPool.RentVlDoor(world);

			world.Thinkers.Add(door);

			sector.SpecialData = door;
			sector.Special = 0;

			door.Sector = sector;
			door.Direction = 0;
			door.Type = VlDoorType.Normal;
			door.Speed = doorSpeed;
			door.TopCountDown = 30 * 35;
		}

		public void SpawnDoorRaiseIn5Mins(Sector sector)
		{
			var door = ThinkerPool.RentVlDoor(world);

			world.Thinkers.Add(door);

			sector.SpecialData = door;
			sector.Special = 0;

			door.Sector = sector;
			door.Direction = 2;
			door.Type = VlDoorType.RaiseIn5Mins;
			door.Speed = doorSpeed;
			door.TopHeight = FindLowestCeilingSurrounding(sector);
			door.TopHeight -= Fixed.FromInt(4);
			door.TopWait = doorWait;
			door.TopCountDown = 5 * 60 * 35;
		}
	}
}
