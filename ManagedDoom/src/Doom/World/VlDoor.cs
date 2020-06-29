using System;

namespace ManagedDoom
{
	public class VlDoor : Thinker
	{
		private World world;

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

		public VlDoor(World world)
		{
			this.world = world;
		}

		public override void Run()
		{
			var sa = world.SectorAction;

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
								world.StartSound(Sector.SoundOrigin, Sfx.BDCLS, SfxType.Misc);
								break;

							case VlDoorType.Normal:
								// time to go back down
								Direction = -1;
								world.StartSound(Sector.SoundOrigin, Sfx.DORCLS, SfxType.Misc);
								break;

							case VlDoorType.Close30ThenOpen:
								Direction = 1;
								world.StartSound(Sector.SoundOrigin, Sfx.DOROPN, SfxType.Misc);
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
								world.StartSound(Sector.SoundOrigin, Sfx.DOROPN, SfxType.Misc);
								break;

							default:
								break;
						}
					}
					break;

				case -1:
					// DOWN
					res = sa.MovePlane(
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
								world.Thinkers.Remove(this);
								world.StartSound(Sector.SoundOrigin, Sfx.BDCLS, SfxType.Misc);
								break;

							case VlDoorType.Normal:
							case VlDoorType.Close:
								Sector.SpecialData = null;
								// unlink and free
								world.Thinkers.Remove(this);
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
								world.StartSound(Sector.SoundOrigin, Sfx.DOROPN, SfxType.Misc);
								break;
						}
					}
					break;

				case 1:
					// UP
					res = sa.MovePlane(
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
								world.Thinkers.Remove(this); // unlink and free
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
