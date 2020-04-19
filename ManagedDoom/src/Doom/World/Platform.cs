using System;

namespace ManagedDoom
{
	public sealed class Platform : Thinker
	{
		private World world;

		public Sector Sector;
		public Fixed Speed;
		public Fixed Low;
		public Fixed High;
		public int Wait;
		public int Count;
		public PlatformState Status;
		public PlatformState Oldstatus;
		public bool Crush;
		public int Tag;
		public PlatformType Type;

		public Platform(World world)
		{
			this.world = world;
		}

		public override void Run()
		{
			var sa = world.SectorAction;

			SectorActionResult res;

			switch (Status)
			{
				case PlatformState.Up:
					res = sa.MovePlane(
						Sector,
						Speed,
						High,
						Crush, 0, 1);

					if (Type == PlatformType.RaiseAndChange
						|| Type == PlatformType.RaiseToNearestAndChange)
					{
						if ((world.levelTime & 7) == 0)
						{
							world.StartSound(Sector.SoundOrigin, Sfx.STNMOV);
						}
					}

					if (res == SectorActionResult.Crushed && !Crush)
					{
						Count = Wait;
						Status = PlatformState.Down;
						world.StartSound(Sector.SoundOrigin, Sfx.PSTART);
					}
					else
					{
						if (res == SectorActionResult.PastDestination)
						{
							Count = Wait;
							Status = PlatformState.Waiting;
							world.StartSound(Sector.SoundOrigin, Sfx.PSTOP);

							switch (Type)
							{
								case PlatformType.BlazeDwus:
								case PlatformType.DownWaitUpStay:
									sa.RemoveActivePlat(this);
									break;

								case PlatformType.RaiseAndChange:
								case PlatformType.RaiseToNearestAndChange:
									sa.RemoveActivePlat(this);
									break;

								default:
									break;
							}
						}
					}

					break;

				case PlatformState.Down:
					res = sa.MovePlane(Sector, Speed, Low, false, 0, -1);

					if (res == SectorActionResult.PastDestination)
					{
						Count = Wait;
						Status = PlatformState.Waiting;
						world.StartSound(Sector.SoundOrigin, Sfx.PSTOP);
					}

					break;

				case PlatformState.Waiting:
					if (--Count == 0)
					{
						if (Sector.FloorHeight == Low)
						{
							Status = PlatformState.Up;
						}
						else
						{
							Status = PlatformState.Down;
						}
						world.StartSound(Sector.SoundOrigin, Sfx.PSTART);
					}

					break;

				case PlatformState.InStasis:
					break;
			}
		}
	}
}
