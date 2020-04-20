using System;

namespace ManagedDoom
{
	public sealed class DoomGame
	{
		private Player[] players;
		private World world;
		private Intermission intermission;
		private GameAction gameAction;

		private bool demoplayback;
		private bool demorecording;

		private int gametic;

		private GameOptions options;
		private GameState gameState;

		private bool paused;

		public DoomGame(GameOptions options)
		{
			players = new Player[Player.MaxPlayerCount];
			for (var i = 0; i < Player.MaxPlayerCount; i++)
			{
				players[i] = new Player(i);
			}

			this.options = options;
		}

		public void Update()
		{
			// do player reborns if needed
			for (var i = 0; i < Player.MaxPlayerCount; i++)
			{
				if (players[i].InGame && players[i].PlayerState == PlayerState.Reborn)
				{
					//G_DoReborn(i);
				}
			}

			// do things to change the game state
			while (gameAction != GameAction.Nothing)
			{
				switch (gameAction)
				{
					case GameAction.LoadLevel:
						//G_DoLoadLevel();
						break;
					case GameAction.NewGame:
						//G_DoNewGame();
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
						//G_DoCompleted();
						break;
					case GameAction.Victory:
						//F_StartFinale();
						break;
					case GameAction.WorldDone:
						//G_DoWorldDone();
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

					//memcpy(cmd, &netcmds[i][buf], sizeof(ticcmd_t));

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
					world.Update();
					//ST_Ticker();
					//AM_Ticker();
					//HU_Ticker();
					break;

				case GameState.Intermission:
					//WI_Ticker();
					break;

				case GameState.Finale:
					//F_Ticker();
					break;

				case GameState.DemoScreen:
					//D_PageTicker();
					break;
			}
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
					//G_DeathMatchSpawnPlayer(playernum);
					return;
				}

				if (G_CheckSpot(playernum, world.PlayerStarts[playernum]))
				{
					world.ThingAllocation.SpawnPlayer(world.PlayerStarts[playernum]);
					return;
				}

				// try to spawn at one of the other players spots 
				for (var i = 0; i < Player.MaxPlayerCount; i++)
				{
					if (G_CheckSpot(playernum, world.PlayerStarts[i]))
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









		private static readonly int BODYQUESIZE = 32;
		private Mobj[] bodyque = new Mobj[BODYQUESIZE];
		private int bodyqueslot;

		//
		// G_CheckSpot  
		// Returns false if the player cannot be respawned
		// at the given mapthing_t spot  
		// because something is occupying it 
		//
		private bool G_CheckSpot(int playernum, Thing mthing)
		{
			if (players[playernum].Mobj == null)
			{
				// first spawn of level, before corpses
				for (var i = 0; i < playernum; i++)
				{
					if (players[i].Mobj.X == mthing.X && players[i].Mobj.Y == mthing.Y)
					{
						return false;
					}
				}
				return true;
			}

			var x = mthing.X;
			var y = mthing.Y;

			if (!world.ThingMovement.CheckPosition(players[playernum].Mobj, x, y))
			{
				return false;
			}

			// flush an old corpse if needed 
			if (bodyqueslot >= BODYQUESIZE)
			{
				world.ThingAllocation.RemoveMobj(bodyque[bodyqueslot % BODYQUESIZE]);
			}
			bodyque[bodyqueslot % BODYQUESIZE] = players[playernum].Mobj;
			bodyqueslot++;

			// spawn a teleport fog 
			var ss = Geometry.PointInSubsector(x, y, world.Map);
			var an = mthing.Angle;

			var mo = world.ThingAllocation.SpawnMobj(
				x + 20 * Trig.Cos(an), y + 20 * Trig.Sin(an),
				ss.Sector.FloorHeight,
				MobjType.Tfog);

			if (players[options.ConsolePlayer].ViewZ != new Fixed(1))
			{
				// don't start sound on first frame
				world.StartSound(mo, Sfx.TELEPT);
			}

			return true;
		}

	}
}
