using System;

namespace ManagedDoom
{
	public sealed class Finale
	{
		private static readonly int textWait = 3;
		private static readonly int textSpeed = 250;

		private DoomGame game;

		// Stage of animation:
		//  0 = text, 1 = art screen, 2 = character cast
		private int finalestage;
		private int finalecount;

		private string finaleflat;
		private string finaletext;

		public Finale(DoomGame game)
		{
			this.game = game;

			// Okay - IWAD dependend stuff.
			// This has been changed severly, and
			//  some stuff might have changed in the process.
			switch (game.Options.GameMode)
			{

				// DOOM 1 - E1, E3 or E4, but each nine missions
				case GameMode.Shareware:
				case GameMode.Registered:
				case GameMode.Retail:
					{
						//S_ChangeMusic(mus_victor, true);

						switch (game.Options.Episode)
						{
							case 1:
								finaleflat = "FLOOR4_8";
								finaletext = DoomInfo.Strings.E1TEXT;
								break;
							case 2:
								finaleflat = "SFLR6_1";
								finaletext = DoomInfo.Strings.E2TEXT;
								break;
							case 3:
								finaleflat = "MFLR8_4";
								finaletext = DoomInfo.Strings.E3TEXT;
								break;
							case 4:
								finaleflat = "MFLR8_3";
								finaletext = DoomInfo.Strings.E4TEXT;
								break;
							default:
								// Ouch.
								break;
						}
						break;
					}

				// DOOM II and missions packs with E1, M34
				case GameMode.Commercial:
					{
						//S_ChangeMusic(mus_read_m, true);

						switch (game.Options.Map)
						{
							case 6:
								finaleflat = "SLIME16";
								finaletext = DoomInfo.Strings.C1TEXT;
								break;
							case 11:
								finaleflat = "RROCK14";
								finaletext = DoomInfo.Strings.C2TEXT;
								break;
							case 20:
								finaleflat = "RROCK07";
								finaletext = DoomInfo.Strings.C3TEXT;
								break;
							case 30:
								finaleflat = "RROCK17";
								finaletext = DoomInfo.Strings.C4TEXT;
								break;
							case 15:
								finaleflat = "RROCK13";
								finaletext = DoomInfo.Strings.C5TEXT;
								break;
							case 31:
								finaleflat = "RROCK19";
								finaletext = DoomInfo.Strings.C6TEXT;
								break;
							default:
								// Ouch.
								break;
						}
						break;
					}


				// Indeterminate.
				default:
					//S_ChangeMusic(mus_read_m, true);
					finaleflat = "F_SKY1"; // Not used anywhere else.
					finaletext = DoomInfo.Strings.C1TEXT;  // FIXME - other text, music?
					break;
			}

			finalestage = 0;
			finalecount = 0;
		}


		public bool Update()
		{
			var options = game.Options;

			// check for skipping
			int i;
			if (options.GameMode == GameMode.Commercial && finalecount > 50)
			{
				// go on to the next level
				for (i = 0; i < Player.MaxPlayerCount; i++)
				{
					if (game.Players[i].Cmd.Buttons != 0)
					{
						break;
					}
				}

				if (i < Player.MaxPlayerCount)
				{
					if (options.Map == 30)
					{
						//F_StartCast();
					}
					else
					{
						//gameaction = ga_worlddone;
						return true;
					}
				}
			}

			// advance animation
			finalecount++;

			if (finalestage == 2)
			{
				//F_CastTicker();
				return false;
			}

			if (options.GameMode == GameMode.Commercial)
			{
				return false;
			}

			if (finalestage == 0 && finalecount > finaletext.Length * textSpeed + textWait)
			{
				finalecount = 0;
				finalestage = 1;
				//wipegamestate = -1;     // force a wipe
				if (options.Episode == 3)
				{
					//S_StartMusic(mus_bunny);
				}
			}

			return false;
		}

		public string Text => finaletext;
		public int Count => finalecount;
		public int Stage => finalestage;
		public string Flat => finaleflat;
	}
}
