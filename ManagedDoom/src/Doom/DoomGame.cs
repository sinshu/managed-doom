using System;

namespace ManagedDoom
{
	public sealed class DoomGame
	{
		private CommonResource resource;

		private Player[] players;
		private World world;
		private Intermission intermission;
		private GameAction gameAction;

		private bool demoplayback;
		private bool demorecording;

		private int gametic;

		private GameOptions options;
		public GameState gameState;
		private GameMode gameMode;

		private bool paused;

		private int levelstarttic;

		private bool secretexit;

		public DoomGame(Player[] players, CommonResource resource, GameOptions options)
		{
			this.resource = resource;

			this.players = players;

			this.options = options;

			gameAction = GameAction.NewGame;
		}

		public void Update(TicCmd[] cmds)
		{
			// do player reborns if needed
			for (var i = 0; i < Player.MaxPlayerCount; i++)
			{
				if (players[i].InGame && players[i].PlayerState == PlayerState.Reborn)
				{
					G_DoReborn(i);
				}
			}

			// do things to change the game state
			while (gameAction != GameAction.Nothing)
			{
				switch (gameAction)
				{
					case GameAction.LoadLevel:
						G_DoLoadLevel();
						break;
					case GameAction.NewGame:
						G_DoNewGame();
						break;
					case GameAction.LoadGame:
						//G_DoLoadGame();
						break;
					case GameAction.SaveGame:
						//G_DoSaveGame();
						break;
					case GameAction.PlayDemo:
						//G_DoPlayDemo();
						break;
					case GameAction.Completed:
						G_DoCompleted();
						break;
					case GameAction.Victory:
						//F_StartFinale();
						break;
					case GameAction.WorldDone:
						G_DoWorldDone();
						break;
					case GameAction.ScreenShot:
						//M_ScreenShot();
						gameAction = GameAction.Nothing;
						break;
					case GameAction.Nothing:
						break;
				}
			}

			// get commands, check consistancy,
			// and build new consistancy check
			//buf = (gametic / ticdup) % BACKUPTICS;

			for (var i = 0; i < Player.MaxPlayerCount; i++)
			{
				if (players[i].InGame)
				{
					var cmd = players[i].Cmd;

					cmd.CopyFrom(cmds[i]);

					if (demoplayback)
					{
						//G_ReadDemoTiccmd(cmd);
					}

					if (demorecording)
					{
						//G_WriteDemoTiccmd(cmd);
					}

					// check for turbo cheats
					if (cmd.ForwardMove > GameConstants.TURBOTHRESHOLD.Data
						&& (gametic & 31) == 0
						&& ((gametic >> 5) & 3) == i)
					{
						players[options.ConsolePlayer].Message = "%s is turbo!";
					}

					/*
					if (options.NetGame && !netdemo && !(gametic%ticdup) ) 
					{ 
						if (gametic > BACKUPTICS 
							&& consistancy[i][buf] != cmd->consistancy) 
						{ 
							I_Error("consistency failure (%i should be %i)",
							cmd->consistancy, consistancy[i][buf]);
							
							if (players[i].mo) 
								consistancy[i][buf] = players[i].mo->x; 
							else 
								consistancy[i][buf] = rndindex; 
						} 
					}
					*/
				}
			}

			// check for special buttons
			for (var i = 0; i < Player.MaxPlayerCount; i++)
			{
				if (players[i].InGame)
				{
					if ((players[i].Cmd.Buttons & TicCmdButtons.Special) != 0)
					{
						switch (players[i].Cmd.Buttons & TicCmdButtons.SpecialMask)
						{
							case TicCmdButtons.Pause:
								paused = !paused;
								if (paused)
								{
									//S_PauseSound();
								}
								else
								{
									//S_ResumeSound();
								}
								break;

							case TicCmdButtons.SaveGame:
								//if (!savedescription[0])
								{
									//strcpy(savedescription, "NET GAME");
									//savegameslot =
									//    (players[i].cmd.buttons & BTS_SAVEMASK) >> BTS_SAVESHIFT;
									gameAction = GameAction.SaveGame;
								}
								break;
						}
					}
				}
			}

			// do main actions
			switch (gameState)
			{
				case GameState.Level:
					gameAction = world.Update();
					//ST_Ticker();
					//AM_Ticker();
					//HU_Ticker();
					break;

				case GameState.Intermission:
					var end = intermission.Update();
					if (end)
					{
						G_WorldDone();
					}
					break;

				case GameState.Finale:
					//F_Ticker();
					break;

				case GameState.DemoScreen:
					//D_PageTicker();
					break;
			}

			options.GameTic++;
		}



		//
		// G_DoReborn 
		// 
		private void G_DoReborn(int playernum)
		{
			if (!options.NetGame)
			{
				// reload the level from scratch
				gameAction = GameAction.LoadLevel;
			}
			else
			{
				// respawn at the start

				// first dissasociate the corpse
				players[playernum].Mobj.Player = null;

				// spawn at random spot if in death match
				if (options.Deathmatch != 0)
				{
					world.G_DeathMatchSpawnPlayer(playernum);
					return;
				}

				if (world.G_CheckSpot(playernum, world.PlayerStarts[playernum]))
				{
					world.ThingAllocation.SpawnPlayer(world.PlayerStarts[playernum]);
					return;
				}

				// try to spawn at one of the other players spots
				for (var i = 0; i < Player.MaxPlayerCount; i++)
				{
					if (world.G_CheckSpot(playernum, world.PlayerStarts[i]))
					{
						// fake as other player
						world.PlayerStarts[i].Type = playernum + 1;

						world.ThingAllocation.SpawnPlayer(world.PlayerStarts[i]);

						// restore
						world.PlayerStarts[i].Type = i + 1;

						return;
					}
					// he's going to be inside something. Too bad.
				}

				world.ThingAllocation.SpawnPlayer(world.PlayerStarts[playernum]);
			}
		}
















		private void G_DoLoadLevel()
		{
			/*
			// Set the sky map.
			// First thing, we have a dummy sky texture name,
			//  a flat. The data is in the WAD only because
			//  we look for an actual index, instead of simply
			//  setting one.
			skyflatnum = R_FlatNumForName(SKYFLATNAME);

			// DOOM determines the sky texture to be used
			// depending on the current episode, and the game version.
			if ((gamemode == commercial)
			 || (gamemode == pack_tnt)
			 || (gamemode == pack_plut))
			{
				skytexture = R_TextureNumForName("SKY3");
				if (gamemap < 12)
					skytexture = R_TextureNumForName("SKY1");
				else
					if (gamemap < 21)
					skytexture = R_TextureNumForName("SKY2");
			}
			*/

			// for time calculation
			levelstarttic = gametic;

			/*
			if (wipegamestate == GS_LEVEL)
			{
				// force a wipe
				wipegamestate = -1;
			}
			*/

			gameState = GameState.Level;

			for (var i = 0; i < Player.MaxPlayerCount; i++)
			{
				if (players[i].InGame && players[i].PlayerState == PlayerState.Dead)
				{
					players[i].PlayerState = PlayerState.Reborn;
				}
				Array.Clear(players[i].Frags, 0, players[i].Frags.Length);
			}

			intermission = null;

			// P_SetupLevel(gameepisode, gamemap, 0, gameskill);
			world = new World(resource, options, players);
			world.Audio = audio;

			// view the guy you are playing
			// displayplayer = consoleplayer;

			// starttime = I_GetTime();
			gameAction = GameAction.Nothing;
			// Z_CheckHeap();

			// clear cmd building stuff
			/*
			memset(gamekeydown, 0, sizeof(gamekeydown));
			joyxmove = joyymove = 0;
			mousex = mousey = 0;
			sendpause = sendsave = paused = false;
			memset(mousebuttons, 0, sizeof(mousebuttons));
			memset(joybuttons, 0, sizeof(joybuttons));
			*/
		}



		private void G_DoNewGame()
		{
			demoplayback = false;
			//netdemo = false;
			//netgame = false;
			//deathmatch = false;
			//players[1].InGame = players[2].InGame = players[3].InGame = false;
			//respawnparm = false;
			//fastparm = false;
			//nomonsters = false;
			//consoleplayer = 0;
			//G_InitNew(d_skill, d_episode, d_map);
			G_InitNew(options.Skill, options.Episode, options.Map);
			gameAction = GameAction.Nothing;
		}


		private void G_InitNew(GameSkill skill, int episode, int map)
		{
			if (paused)
			{
				paused = false;
				//S_ResumeSound();
			}


			if (skill > GameSkill.Nightmare)
			{
				skill = GameSkill.Nightmare;
			}

			// This was quite messy with SPECIAL and commented parts.
			// Supposedly hacks to make the latest edition work.
			// It might not work properly.
			if (episode < 1)
			{
				episode = 1;
			}

			if (gameMode == GameMode.Retail)
			{
				if (episode > 4)
				{
					episode = 4;
				}
			}
			else if (gameMode == GameMode.Shareware)
			{
				if (episode > 1)
				{
					// only start episode 1 on shareware
					episode = 1;
				}
			}
			else
			{
				if (episode > 3)
				{
					episode = 3;
				}
			}



			if (map < 1)
			{
				map = 1;
			}

			if ((map > 9) && (gameMode != GameMode.Commercial))
			{
				map = 9;
			}

			options.Random.Clear();

			/*
			if (skill == sk_nightmare || respawnparm)
			{
				respawnmonsters = true;
			}
			else
			{
				respawnmonsters = false;
			}
			*/

			// force players to be initialized upon first level load         
			for (var i = 0; i < Player.MaxPlayerCount; i++)
			{
				players[i].PlayerState = PlayerState.Reborn;
			}

			//usergame = true; // will be set false if a demo 
			//paused = false;
			demoplayback = false;
			//automapactive = false;
			//viewactive = true;
			//gameepisode = episode;
			//gamemap = map;
			//gameskill = skill;

			//viewactive = true;

			/*
			// set the sky map for the episode
			if (gamemode == commercial)
			{
				skytexture = R_TextureNumForName("SKY3");
				if (gamemap < 12)
					skytexture = R_TextureNumForName("SKY1");
				else
					if (gamemap < 21)
					skytexture = R_TextureNumForName("SKY2");
			}
			else
			{
				switch (episode)
				{
					case 1:
						skytexture = R_TextureNumForName("SKY1");
						break;
					case 2:
						skytexture = R_TextureNumForName("SKY2");
						break;
					case 3:
						skytexture = R_TextureNumForName("SKY3");
						break;
					case 4: // Special Edition sky
						skytexture = R_TextureNumForName("SKY4");
						break;
				}
			}
			*/

			G_DoLoadLevel();
		}





		private void G_DoCompleted()
		{
			gameAction = GameAction.Nothing;

			for (var i = 0; i < Player.MaxPlayerCount; i++)
			{
				if (players[i].InGame)
				{
					// take away cards and stuff
					G_PlayerFinishLevel(i);
				}
			}

			/*
			if (automapactive)
			{
				AM_Stop();
			}
			*/

			if (gameMode != GameMode.Commercial)
			{
				switch (options.Map)
				{
					case 8:
						gameAction = GameAction.Victory;
						return;
					case 9:
						for (var i = 0; i < Player.MaxPlayerCount; i++)
						{
							players[i].DidSecret = true;
						}
						break;
				}
			}

			//#if 0  Hmmm - why?
			if ((options.Map == 8) && (gameMode != GameMode.Commercial))
			{
				// victory 
				gameAction = GameAction.Victory;
				return;
			}

			if ((options.Map == 9) && (gameMode != GameMode.Commercial))
			{
				// exit secret level 
				for (var i = 0; i < Player.MaxPlayerCount; i++)
				{
					players[i].DidSecret = true;
				}
			}
			//#endif

			var wminfo = options.wminfo;

			wminfo.DidSecret = players[options.ConsolePlayer].DidSecret;
			wminfo.Epsd = options.Episode - 1;
			wminfo.Last = options.Map - 1;

			// wminfo.next is 0 biased, unlike gamemap
			if (gameMode == GameMode.Commercial)
			{
				if (secretexit)
				{
					switch (options.Map)
					{
						case 15:
							wminfo.Next = 30;
							break;
						case 31:
							wminfo.Next = 31;
							break;
					}
				}
				else
				{
					switch (options.Map)
					{
						case 31:
						case 32:
							wminfo.Next = 15;
							break;
						default:
							wminfo.Next = options.Map;
							break;
					}
				}
			}
			else
			{
				if (secretexit)
				{
					// go to secret level 
					wminfo.Next = 8;
				}
				else if (options.Map == 9)
				{
					// returning from secret level 
					switch (options.Episode)
					{
						case 1:
							wminfo.Next = 3;
							break;
						case 2:
							wminfo.Next = 5;
							break;
						case 3:
							wminfo.Next = 6;
							break;
						case 4:
							wminfo.Next = 2;
							break;
					}
				}
				else
				{
					// go to next level
					wminfo.Next = options.Map;
				}
			}

			wminfo.maxKills = world.totalKills;
			wminfo.maxItems = world.totalItems;
			wminfo.maxSecret = world.totalSecrets;
			wminfo.maxFrags = 0;
			if (gameMode == GameMode.Commercial)
			{
				wminfo.ParTime = 35 * 30; //cpars[gamemap - 1];
			}
			else
			{
				wminfo.ParTime = 35 * 30; //pars[gameepisode][gamemap];
			}
			wminfo.PNum = options.ConsolePlayer;

			for (var i = 0; i < Player.MaxPlayerCount; i++)
			{
				wminfo.Plyr[i].InGame = players[i].InGame;
				wminfo.Plyr[i].Skills = players[i].KillCount;
				wminfo.Plyr[i].SItems = players[i].ItemCount;
				wminfo.Plyr[i].SSecret = players[i].SecretCount;
				wminfo.Plyr[i].STime = world.levelTime;
				Array.Copy(players[i].Frags, wminfo.Plyr[i].Frags, Player.MaxPlayerCount);
			}

			gameState = GameState.Intermission;
			//viewactive = false;
			//automapactive = false;

			/*
			if (statcopy)
			{
				memcpy(statcopy, &wminfo, sizeof(wminfo));
			}
			*/

			//WI_Start(&wminfo);
			intermission = new Intermission(players, wminfo, options);
		}




		//
		// G_PlayerFinishLevel
		// Can when a player completes a level.
		//
		private void G_PlayerFinishLevel(int player)
		{
			var p = players[player];
			Array.Clear(p.Powers, 0, p.Powers.Length);
			Array.Clear(p.Cards, 0, p.Cards.Length);

			// cancel invisibility
			p.Mobj.Flags &= ~MobjFlags.Shadow;

			// cancel gun flashes
			p.ExtraLight = 0;

			// cancel ir gogles
			p.FixedColorMap = 0;

			// no palette changes
			p.DamageCount = 0;
			p.BonusCount = 0;
		}

		//
		// G_WorldDone 
		//
		private void G_WorldDone()
		{
			gameAction = GameAction.WorldDone;

			if (secretexit)
			{
				players[options.ConsolePlayer].DidSecret = true;
			}

			if (gameMode == GameMode.Commercial)
			{
				/*
				switch (options.Map)
				{
					case 15:
					case 31:
						if (!secretexit)
							break;
					case 6:
					case 11:
					case 20:
					case 30:
						F_StartFinale();
						break;
				}
				*/
			}
		}

		private void G_DoWorldDone()
		{
			gameState = GameState.Level;
			options.Map = options.wminfo.Next + 1;
			G_DoLoadLevel();
			gameAction = GameAction.Nothing;
			//viewactive = true;
		}

		public void DoAction(GameAction action)
		{
			gameAction = action;
		}




		public bool DoEvent(DoomEvent e)
		{
			if (gameState == GameState.Level)
			{
				var am = world.AutoMap;

				if (am.Visible)
				{
					if (am.DoEvent(e))
					{
						return true;
					}
				}

				if (e.Key == SFML.Window.Keyboard.Key.Tab && e.Type == EventType.KeyDown)
				{
					if (am.Visible)
					{
						am.Close();
					}
					else
					{
						am.Open();
					}
				}

				return true;
			}

			return false;
		}



		public Player[] Players => players;
		public GameOptions Options => options;
		public World World => world;
		public Intermission Intermission => intermission;



		private SfmlAudio audio;

		public SfmlAudio Audio
		{
			get => audio;
			set => audio = value;
		}
	}
}
