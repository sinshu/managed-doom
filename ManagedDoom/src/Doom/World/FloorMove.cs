using System;

namespace ManagedDoom
{
	public sealed class FloorMove : Thinker
	{
		private World world;

		private FloorMoveType type;
		private bool crush;
		private Sector sector;
		private int direction;
		private SectorSpecial newSpecial;
		private int texture;
		private Fixed floorDestHeight;
		private Fixed speed;

		public FloorMove(World world)
		{
			this.world = world;
		}

		public override void Run()
		{
			SectorActionResult result;

			var sa = world.SectorAction;

			result = sa.MovePlane(
				sector,
				speed,
				floorDestHeight,
				crush,
				0,
				direction);

			if (((world.levelTime + sector.Number) & 7) == 0)
			{
				world.StartSound(sector.SoundOrigin, Sfx.STNMOV, SfxType.Misc);
			}

			if (result == SectorActionResult.PastDestination)
			{
				sector.SpecialData = null;

				if (direction == 1)
				{
					switch (type)
					{
						case FloorMoveType.DonutRaise:
							sector.Special = newSpecial;
							sector.FloorFlat = texture;
							break;
					}
				}
				else if (direction == -1)
				{
					switch (type)
					{
						case FloorMoveType.LowerAndChange:
							sector.Special = newSpecial;
							sector.FloorFlat = texture;
							break;
					}
				}

				world.Thinkers.Remove(this);

				world.StartSound(sector.SoundOrigin, Sfx.PSTOP, SfxType.Misc);
			}
		}

		public FloorMoveType Type
		{
			get => type;
			set => type = value;
		}

		public bool Crush
		{
			get => crush;
			set => crush = value;
		}

		public Sector Sector
		{
			get => sector;
			set => sector = value;
		}

		public int Direction
		{
			get => direction;
			set => direction = value;
		}

		public SectorSpecial NewSpecial
		{
			get => newSpecial;
			set => newSpecial = value;
		}

		public int Texture
		{
			get => texture;
			set => texture = value;
		}

		public Fixed FloorDestHeight
		{
			get => floorDestHeight;
			set => floorDestHeight = value;
		}

		public Fixed Speed
		{
			get => speed;
			set => speed = value;
		}
	}
}
