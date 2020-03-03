using System;

namespace ManagedDoom
{
	public class VlDoor : Thinker
	{
		public VlDoorType Type;
		public Sector Sector;
		public Fixed TopHeight;
		public Fixed Speed;

		// 1 = up, 0 = waiting at top, -1 = down
		public int Direction;

		// tics to wait at the top
		public int TopWait;

		// (keep in case a door going down is reset)
		// when it reaches 0, start going down
		public int TopCountDown;

		public VlDoor(World world) : base(world)
		{
		}

		public override void Run()
		{
			SectorActionResult res;

			switch (Direction)
			{
				case 0:
					// WAITING
					if (--TopCountDown == 0)
					{
						switch (Type)
						{
							case VlDoorType.BlazeRaise:
								// time to go back down
								Direction = -1;
								World.StartSound(Sector.SoundOrigin, Sfx.BDCLS);
								break;

							case VlDoorType.Normal:
								// time to go back down
								Direction = -1;
								World.StartSound(Sector.SoundOrigin, Sfx.DORCLS);
								break;

							case VlDoorType.Close30ThenOpen:
								Direction = 1;
								World.StartSound(Sector.SoundOrigin, Sfx.DOROPN);
								break;

							default:
								break;
						}
					}
					break;

				case 2:
					//  INITIAL WAIT
					if (--TopCountDown == 0)
					{
						switch (Type)
						{
							case VlDoorType.RaiseIn5Mins:
								Direction = 1;
								Type = VlDoorType.Normal;
								World.StartSound(Sector.SoundOrigin, Sfx.DOROPN);
								break;

							default:
								break;
						}
					}
					break;

				case -1:
					// DOWN
					res = World.MovePlane(
						Sector,
						Speed,
						Sector.FloorHeight,
						false, 1, Direction);
					if (res == SectorActionResult.PastDestination)
					{
						switch (Type)
						{
							case VlDoorType.BlazeRaise:
							case VlDoorType.BlazeClose:
								Sector.SpecialData = null;
								// unlink and free
								World.RemoveThinker(this);
								World.StartSound(Sector.SoundOrigin, Sfx.BDCLS);
								break;

							case VlDoorType.Normal:
							case VlDoorType.Close:
								Sector.SpecialData = null;
								// unlink and free
								World.RemoveThinker(this);
								break;

							case VlDoorType.Close30ThenOpen:
								Direction = 0;
								TopCountDown = 35 * 30;
								break;

							default:
								break;
						}
					}
					else if (res == SectorActionResult.Crushed)
					{
						switch (Type)
						{
							case VlDoorType.BlazeClose:
							case VlDoorType.Close: // DO NOT GO BACK UP!
								break;

							default:
								Direction = 1;
								World.StartSound(Sector.SoundOrigin, Sfx.DOROPN);
								break;
						}
					}
					break;

				case 1:
					// UP
					res = World.MovePlane(
						Sector,
						Speed,
						TopHeight,
						false, 1, Direction);

					if (res == SectorActionResult.PastDestination)
					{
						switch (Type)
						{
							case VlDoorType.BlazeRaise:
							case VlDoorType.Normal:
								Direction = 0; // wait at top
								TopCountDown = TopWait;
								break;

							case VlDoorType.Close30ThenOpen:
							case VlDoorType.BlazeOpen:
							case VlDoorType.Open:
								Sector.SpecialData = null;
								World.RemoveThinker(this); // unlink and free
								break;

							default:
								break;
						}
					}
					break;
			}
		}
	}
}
