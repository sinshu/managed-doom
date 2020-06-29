using System;

namespace ManagedDoom
{
	public sealed class CeilingMove : Thinker
	{
		private World world;

		public CeilingMoveType Type;
		public Sector Sector;
		public Fixed BottomHeight;
		public Fixed TopHeight;
		public Fixed Speed;
		public bool Crush;

		// 1 = up, 0 = waiting, -1 = down
		public int Direction;

		// ID
		public int Tag;
		public int OldDirection;

		public CeilingMove(World world)
		{
			this.world = world;
		}

		public override void Run()
		{
			SectorActionResult res;

			var sa = world.SectorAction;

			switch (Direction)
			{
				case 0:
					// IN STASIS
					break;
				case 1:
					// UP
					res = sa.MovePlane(
						Sector,
						Speed,
						TopHeight,
						false, 1, Direction);

					if ((world.levelTime & 7) == 0)
					{
						switch (Type)
						{
							case CeilingMoveType.SilentCrushAndRaise:
								break;
							default:
								world.StartSound(Sector.SoundOrigin, Sfx.STNMOV, SfxType.Misc);
								// ?
								break;
						}
					}

					if (res == SectorActionResult.PastDestination)
					{
						switch (Type)
						{
							case CeilingMoveType.RaiseToHighest:
								sa.RemoveActiveCeiling(this);
								break;

							case CeilingMoveType.SilentCrushAndRaise:
							case CeilingMoveType.FastCrushAndRaise:
							case CeilingMoveType.CrushAndRaise:
								if (Type == CeilingMoveType.SilentCrushAndRaise)
								{
									world.StartSound(Sector.SoundOrigin, Sfx.PSTOP, SfxType.Misc);
								}
								Direction = -1;
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
						BottomHeight,
						Crush, 1, Direction);

					if ((world.levelTime & 7) == 0)
					{
						switch (Type)
						{
							case CeilingMoveType.SilentCrushAndRaise:
								break;
							default:
								world.StartSound(Sector.SoundOrigin, Sfx.STNMOV, SfxType.Misc);
								break;
						}
					}

					if (res == SectorActionResult.PastDestination)
					{
						switch (Type)
						{
							case CeilingMoveType.SilentCrushAndRaise:
							case CeilingMoveType.CrushAndRaise:
							case CeilingMoveType.FastCrushAndRaise:
								if (Type == CeilingMoveType.SilentCrushAndRaise)
								{
									world.StartSound(Sector.SoundOrigin, Sfx.PSTOP, SfxType.Misc);
								}
								if (Type == CeilingMoveType.CrushAndRaise)
								{
									Speed = SectorAction.CeilingSpeed;
								}
								Direction = 1;
								break;

							case CeilingMoveType.LowerAndCrush:
							case CeilingMoveType.LowerToFloor:
								sa.RemoveActiveCeiling(this);
								break;

							default:
								break;
						}
					}
					else // ( res != pastdest )
					{
						if (res == SectorActionResult.Crushed)
						{
							switch (Type)
							{
								case CeilingMoveType.SilentCrushAndRaise:
								case CeilingMoveType.CrushAndRaise:
								case CeilingMoveType.LowerAndCrush:
									Speed = SectorAction.CeilingSpeed / 8;
									break;

								default:
									break;
							}
						}
					}
					break;
			}
		}
	}
}
