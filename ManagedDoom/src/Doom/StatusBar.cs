using System;

namespace ManagedDoom
{
	public sealed class StatusBar
	{
		// used to use appopriately pained face
		private int st_oldhealth;

		// used for evil grin
		private bool[] oldweaponsowned;

		// count until face changes
		private int st_facecount;

		// current face index, used by w_faces
		private int st_faceindex;

		// a random number per tick
		private int st_randomnumber;

		private int lastattackdown;
		private int priority;

		private int lastcalc;
		private int oldhealth;

		private World world;
		private DoomRandom random;

		public StatusBar(World world)
		{
			this.world = world;

			st_oldhealth = -1;
			oldweaponsowned = new bool[DoomInfo.WeaponInfos.Length];
			st_facecount = 0;
			st_faceindex = 0;
			lastattackdown = -1;
			priority = 0;
			oldhealth = -1;

			Array.Copy(
				world.Players[world.Options.ConsolePlayer].WeaponOwned,
				oldweaponsowned,
				DoomInfo.WeaponInfos.Length);

			random = new DoomRandom();
		}

		public void Update()
		{
			//st_clock++;
			st_randomnumber = random.Next();
			UpdateFace();
			st_oldhealth = world.Players[world.Options.ConsolePlayer].Health;
		}

		private void UpdateFace()
		{
			var plyr = world.Players[world.Options.ConsolePlayer];

			if (priority < 10)
			{
				// dead
				if (plyr.Health == 0)
				{
					priority = 9;
					st_faceindex = DoomInfo.FaceInfos.ST_DEADFACE;
					st_facecount = 1;
				}
			}

			if (priority < 9)
			{
				if (plyr.BonusCount != 0)
				{
					// picking up bonus
					var doevilgrin = false;

					for (var i = 0; i < DoomInfo.WeaponInfos.Length; i++)
					{
						if (oldweaponsowned[i] != plyr.WeaponOwned[i])
						{
							doevilgrin = true;
							oldweaponsowned[i] = plyr.WeaponOwned[i];
						}
					}
					if (doevilgrin)
					{
						// evil grin if just picked up weapon
						priority = 8;
						st_facecount = DoomInfo.FaceInfos.ST_EVILGRINCOUNT;
						st_faceindex = ST_calcPainOffset() + DoomInfo.FaceInfos.ST_EVILGRINOFFSET;
					}
				}

			}

			if (priority < 8)
			{
				if (plyr.DamageCount != 0
					&& plyr.Attacker != null
					&& plyr.Attacker != plyr.Mobj)
				{
					// being attacked
					priority = 7;

					if (plyr.Health - st_oldhealth > DoomInfo.FaceInfos.ST_MUCHPAIN)
					{
						st_facecount = DoomInfo.FaceInfos.ST_TURNCOUNT;
						st_faceindex = ST_calcPainOffset() + DoomInfo.FaceInfos.ST_OUCHOFFSET;
					}
					else
					{
						var badguyangle = Geometry.PointToAngle(
							plyr.Mobj.X, plyr.Mobj.Y,
							plyr.Attacker.X, plyr.Attacker.Y);

						Angle diffang;
						bool i;

						if (badguyangle > plyr.Mobj.Angle)
						{
							// whether right or left
							diffang = badguyangle - plyr.Mobj.Angle;
							i = diffang > Angle.Ang180;
						}
						else
						{
							// whether left or right
							diffang = plyr.Mobj.Angle - badguyangle;
							i = diffang <= Angle.Ang180;
						} // confusing, aint it?

						st_facecount = DoomInfo.FaceInfos.ST_TURNCOUNT;
						st_faceindex = ST_calcPainOffset();

						if (diffang < Angle.Ang45)
						{
							// head-on    
							st_faceindex += DoomInfo.FaceInfos.ST_RAMPAGEOFFSET;
						}
						else if (i)
						{
							// turn face right
							st_faceindex += DoomInfo.FaceInfos.ST_TURNOFFSET;
						}
						else
						{
							// turn face left
							st_faceindex += DoomInfo.FaceInfos.ST_TURNOFFSET + 1;
						}
					}
				}
			}

			if (priority < 7)
			{
				// getting hurt because of your own damn stupidity
				if (plyr.DamageCount != 0)
				{
					if (plyr.Health - st_oldhealth > DoomInfo.FaceInfos.ST_MUCHPAIN)
					{
						priority = 7;
						st_facecount = DoomInfo.FaceInfos.ST_TURNCOUNT;
						st_faceindex = ST_calcPainOffset() + DoomInfo.FaceInfos.ST_OUCHOFFSET;
					}
					else
					{
						priority = 6;
						st_facecount = DoomInfo.FaceInfos.ST_TURNCOUNT;
						st_faceindex = ST_calcPainOffset() + DoomInfo.FaceInfos.ST_RAMPAGEOFFSET;
					}

				}

			}

			if (priority < 6)
			{
				// rapid firing
				if (plyr.AttackDown)
				{
					if (lastattackdown == -1)
					{
						lastattackdown = DoomInfo.FaceInfos.ST_RAMPAGEDELAY;
					}
					else if (--lastattackdown == 0)
					{
						priority = 5;
						st_faceindex = ST_calcPainOffset() + DoomInfo.FaceInfos.ST_RAMPAGEOFFSET;
						st_facecount = 1;
						lastattackdown = 1;
					}
				}
				else
				{
					lastattackdown = -1;
				}
			}

			if (priority < 5)
			{
				// invulnerability
				if ((plyr.Cheats & CheatFlags.GodMode) != 0
					|| plyr.Powers[(int)PowerType.Invulnerability] != 0)
				{
					priority = 4;

					st_faceindex = DoomInfo.FaceInfos.ST_GODFACE;
					st_facecount = 1;

				}

			}

			// look left or look right if the facecount has timed out
			if (st_facecount == 0)
			{
				st_faceindex = ST_calcPainOffset() + (st_randomnumber % 3);
				st_facecount = DoomInfo.FaceInfos.ST_STRAIGHTFACECOUNT;
				priority = 0;
			}

			st_facecount--;
		}

		private int ST_calcPainOffset()
		{
			var plyr = world.Players[world.Options.ConsolePlayer];

			var health = plyr.Health > 100 ? 100 : plyr.Health;

			if (health != oldhealth)
			{
				lastcalc = DoomInfo.FaceInfos.ST_FACESTRIDE
					* (((100 - health) * DoomInfo.FaceInfos.ST_NUMPAINFACES) / 101);
				oldhealth = health;
			}
			return lastcalc;
		}

		public int Face => st_faceindex;
	}
}
