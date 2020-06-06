using System;

namespace ManagedDoom
{
	public sealed class Finale
	{
		public static readonly int TextSpeed = 3;
		public static readonly int TextWait = 250;

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

				if (i < Player.MaxPlayerCount && finalestage != 2)
				{
					if (options.Map == 30)
					{
						StartCast();
					}
					else
					{
						return true;
					}
				}
			}

			// advance animation
			finalecount++;

			if (finalestage == 2)
			{
				CastTicker();
				return false;
			}

			if (options.GameMode == GameMode.Commercial)
			{
				return false;
			}

			if (finalestage == 0 && finalecount > finaletext.Length * TextSpeed + TextWait)
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



		private static readonly CastInfo[] castorder = new CastInfo[]
		{
			new CastInfo(DoomInfo.Strings.CC_ZOMBIE, MobjType.Possessed),
			new CastInfo(DoomInfo.Strings.CC_SHOTGUN, MobjType.Shotguy),
			new CastInfo(DoomInfo.Strings.CC_HEAVY, MobjType.Chainguy),
			new CastInfo(DoomInfo.Strings.CC_IMP, MobjType.Troop),
			new CastInfo(DoomInfo.Strings.CC_DEMON, MobjType.Sergeant),
			new CastInfo(DoomInfo.Strings.CC_LOST, MobjType.Skull),
			new CastInfo(DoomInfo.Strings.CC_CACO, MobjType.Head),
			new CastInfo(DoomInfo.Strings.CC_HELL, MobjType.Knight),
			new CastInfo(DoomInfo.Strings.CC_BARON, MobjType.Bruiser),
			new CastInfo(DoomInfo.Strings.CC_ARACH, MobjType.Baby),
			new CastInfo(DoomInfo.Strings.CC_PAIN, MobjType.Pain),
			new CastInfo(DoomInfo.Strings.CC_REVEN, MobjType.Undead),
			new CastInfo(DoomInfo.Strings.CC_MANCU, MobjType.Fatso),
			new CastInfo(DoomInfo.Strings.CC_ARCH, MobjType.Vile),
			new CastInfo(DoomInfo.Strings.CC_SPIDER, MobjType.Spider),
			new CastInfo(DoomInfo.Strings.CC_CYBER, MobjType.Cyborg),
			new CastInfo(DoomInfo.Strings.CC_HERO, MobjType.Player)
		};

		private int castnum;
		private int casttics;
		private MobjStateDef caststate;
		private bool castdeath;
		private int castframes;
		private bool castonmelee;
		private bool castattacking;

		private void StartCast()
		{
			// wipegamestate = -1; // force a screen wipe
			castnum = 0;
			caststate = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)castorder[castnum].Type].SeeState];
			casttics = caststate.Tics;
			castdeath = false;
			finalestage = 2;
			castframes = 0;
			castonmelee = false;
			castattacking = false;
			// S_ChangeMusic(mus_evil, true);
		}

		private void CastTicker()
		{
			if (--casttics > 0)
			{
				// not time to change state yet
				return;
			}

			if (caststate.Tics == -1 || caststate.Next == MobjState.Null)
			{
				// switch from deathstate to next monster
				castnum++;
				castdeath = false;
				if (castnum == castorder.Length)
				{
					castnum = 0;
				}
				if (DoomInfo.MobjInfos[(int)castorder[castnum].Type].SeeSound != 0)
				{
					//S_StartSound(NULL, mobjinfo[castorder[castnum].type].seesound);
				}
				caststate = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)castorder[castnum].Type].SeeState];
				castframes = 0;
			}
			else
			{
				// just advance to next state in animation
				if (caststate == DoomInfo.States[(int)MobjState.PlayAtk1])
				{
					// Oh, gross hack!
					castattacking = false;
					castframes = 0;
					caststate = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)castorder[castnum].Type].SeeState];
					goto stopattack;
				}
				var st = caststate.Next;
				caststate = DoomInfo.States[(int)st];
				castframes++;

				// sound hacks....
				Sfx sfx;
				switch (st)
				{
					case MobjState.PlayAtk1:
						sfx = Sfx.DSHTGN;
						break;
					case MobjState.PossAtk2:
						sfx = Sfx.PISTOL;
						break;
					case MobjState.SposAtk2:
						sfx = Sfx.SHOTGN;
						break;
					case MobjState.VileAtk2:
						sfx = Sfx.VILATK;
						break;
					case MobjState.SkelFist2:
						sfx = Sfx.SKESWG;
						break;
					case MobjState.SkelFist4:
						sfx = Sfx.SKEPCH;
						break;
					case MobjState.SkelMiss2:
						sfx = Sfx.SKEATK;
						break;
					case MobjState.FattAtk8:
					case MobjState.FattAtk5:
					case MobjState.FattAtk2:
						sfx = Sfx.FIRSHT;
						break;
					case MobjState.CposAtk2:
					case MobjState.CposAtk3:
					case MobjState.CposAtk4:
						sfx = Sfx.SHOTGN;
						break;
					case MobjState.TrooAtk3:
						sfx = Sfx.CLAW;
						break;
					case MobjState.SargAtk2:
						sfx = Sfx.SGTATK;
						break;
					case MobjState.BossAtk2:
					case MobjState.Bos2Atk2:
					case MobjState.HeadAtk2:
						sfx = Sfx.FIRSHT;
						break;
					case MobjState.SkullAtk2:
						sfx = Sfx.SKLATK;
						break;
					case MobjState.SpidAtk2:
					case MobjState.SpidAtk3:
						sfx = Sfx.SHOTGN;
						break;
					case MobjState.BspiAtk2:
						sfx = Sfx.PLASMA;
						break;
					case MobjState.CyberAtk2:
					case MobjState.CyberAtk4:
					case MobjState.CyberAtk6:
						sfx = Sfx.RLAUNC;
						break;
					case MobjState.PainAtk3:
						sfx = Sfx.SKLATK;
						break;
					default:
						sfx = 0;
						break;
				}

				if (sfx != 0)
				{
					// S_StartSound(NULL, sfx);
				}
			}

			if (castframes == 12)
			{
				// go into attack frame
				castattacking = true;
				if (castonmelee)
				{
					caststate = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)castorder[castnum].Type].MeleeState];
				}
				else
				{
					caststate = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)castorder[castnum].Type].MissileState];
				}
				castonmelee = !castonmelee;
				if (caststate == DoomInfo.States[(int)MobjState.Null])
				{
					if (castonmelee)
					{
						caststate = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)castorder[castnum].Type].MeleeState];
					}
					else
					{
						caststate = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)castorder[castnum].Type].MissileState];
					}
				}
			}

			if (castattacking)
			{
				if (castframes == 24 ||
					caststate == DoomInfo.States[(int)DoomInfo.MobjInfos[(int)castorder[castnum].Type].SeeState])
				{
					castattacking = false;
					castframes = 0;
					caststate = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)castorder[castnum].Type].SeeState];
				}
			}

		stopattack:

			casttics = caststate.Tics;
			if (casttics == -1)
			{
				casttics = 15;
			}
		}

		public bool DoEvent(DoomEvent e)
		{
			if (finalestage != 2)
			{
				return false;
			}

			if (e.Type == EventType.KeyDown)
			{
				if (castdeath)
				{
					// already in dying frames
					return true;
				}

				// go into death frame
				castdeath = true;
				caststate = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)castorder[castnum].Type].DeathState];
				casttics = caststate.Tics;
				castframes = 0;
				castattacking = false;
				if (DoomInfo.MobjInfos[(int)castorder[castnum].Type].DeathSound != 0)
				{
					// S_StartSound(NULL, mobjinfo[castorder[castnum].type].deathsound);
				}

				return true;
			}

			return false;
		}



		public string Text => finaletext;
		public int Count => finalecount;
		public int Stage => finalestage;
		public string Flat => finaleflat;
		public GameOptions Options => game.Options;
		public MobjStateDef CastState => caststate;



		private class CastInfo
		{
			public string Name;
			public MobjType Type;

			public CastInfo(string name, MobjType type)
			{
				Name = name;
				Type = type;
			}
		}
	}
}
