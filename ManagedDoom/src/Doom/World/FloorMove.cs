using System;

namespace ManagedDoom
{
	public sealed class FloorMove : Thinker
	{
		private World world;

		public FloorMoveType Type;
		public bool Crush;
		public Sector Sector;
		public int Direction;
		public SectorSpecial NewSpecial;
		public int Texture;
		public Fixed FloorDestHeight;
		public Fixed Speed;

		public FloorMove(World world)
		{
			this.world = world;
		}

		public override void Run()
		{
			SectorActionResult res;

			var sa = world.SectorAction;

			res = sa.MovePlane(
				Sector,
				Speed,
				FloorDestHeight,
				Crush, 0, Direction);

			if ((world.levelTime & 7) == 0)
			{
				world.StartSound(Sector.SoundOrigin, Sfx.STNMOV, SfxType.Misc);
			}

			if (res == SectorActionResult.PastDestination)
			{
				Sector.SpecialData = null;

				if (Direction == 1)
				{
					switch (Type)
					{
						case FloorMoveType.DonutRaise:
							Sector.Special = NewSpecial;
							Sector.FloorFlat = Texture;
							break;
					}
				}
				else if (Direction == -1)
				{
					switch (Type)
					{
						case FloorMoveType.LowerAndChange:
							Sector.Special = NewSpecial;
							Sector.FloorFlat = Texture;
							break;
					}
				}

				world.Thinkers.Remove(this);

				world.StartSound(Sector.SoundOrigin, Sfx.PSTOP, SfxType.Misc);
			}
		}
	}
}
