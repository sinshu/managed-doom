using System;

namespace ManagedDoom
{
	public sealed class MapInteraction
	{
		private World world;

		public MapInteraction(World world)
		{
			this.world = world;
		}

		//
		// P_UseSpecialLine
		// Called when a thing uses a special line.
		// Only the front sides of lines are usable.
		//
		public bool UseSpecialLine(Mobj thing, LineDef line, int side)
		{
			var sa = world.SectorAction;

			// Err...
			// Use the back sides of VERY SPECIAL lines...
			if (side != 0)
			{
				switch ((int)line.Special)
				{
					case 124:
						// Sliding door open&close
						// UNUSED?
						break;

					default:
						return false;
				}
			}

			// Switches that other things can activate.
			if (thing.Player == null)
			{
				// never open secret doors
				if ((line.Flags & LineFlags.Secret) != 0)
				{
					return false;
				}

				switch ((int)line.Special)
				{
					case 1: // MANUAL DOOR RAISE
					case 32: // MANUAL BLUE
					case 33: // MANUAL RED
					case 34: // MANUAL YELLOW
						break;

					default:
						return false;
				}
			}

			// do something  
			switch ((int)line.Special)
			{
				// MANUALS
				case 1: // Vertical Door
				case 26: // Blue Door/Locked
				case 27: // Yellow Door /Locked
				case 28: // Red Door /Locked

				case 31: // Manual door open
				case 32: // Blue locked door open
				case 33: // Red locked door open
				case 34: // Yellow locked door open

				case 117: // Blazing door raise
				case 118: // Blazing door open
					sa.EV_VerticalDoor(line, thing);
					break;

				//UNUSED - Door Slide Open&Close
				// case 124:
				// EV_SlidingDoor (line, thing);
				// break;

				// SWITCHES
				case 7:
					// Build Stairs
					//if (EV_BuildStairs(line, build8))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 9:
					// Change Donut
					//if (EV_DoDonut(line))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 11:
					// Exit level
					//P_ChangeSwitchTexture(line, 0);
					//G_ExitLevel();
					break;

				case 14:
					// Raise Floor 32 and change texture
					if (sa.EV_DoPlat(line, PlatformType.RaiseAndChange, 32))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 15:
					// Raise Floor 24 and change texture
					if (sa.EV_DoPlat(line, PlatformType.RaiseAndChange, 24))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 18:
					// Raise Floor to next highest floor
					//if (EV_DoFloor(line, raiseFloorToNearest))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 20:
					// Raise Plat next highest floor and change texture
					if (sa.EV_DoPlat(line, PlatformType.RaiseToNearestAndChange, 0))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 21:
					// PlatDownWaitUpStay
					if (sa.EV_DoPlat(line, PlatformType.DownWaitUpStay, 0))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 23:
					// Lower Floor to Lowest
					//if (EV_DoFloor(line, lowerFloorToLowest))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 29:
					// Raise Door
					//if (EV_DoDoor(line, normal))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 41:
					// Lower Ceiling to Floor
					//if (EV_DoCeiling(line, lowerToFloor))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 71:
					// Turbo Lower Floor
					//if (EV_DoFloor(line, turboLower))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 49:
					// Ceiling Crush And Raise
					//if (EV_DoCeiling(line, crushAndRaise))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 50:
					// Close Door
					//if (EV_DoDoor(line, close))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 51:
					// Secret EXIT
					//P_ChangeSwitchTexture(line, 0);
					//G_SecretExitLevel();
					break;

				case 55:
					// Raise Floor Crush
					//if (EV_DoFloor(line, raiseFloorCrush))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 101:
					// Raise Floor
					//if (EV_DoFloor(line, raiseFloor))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 102:
					// Lower Floor to Surrounding floor height
					//if (EV_DoFloor(line, lowerFloor))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 103:
					// Open Door
					//if (EV_DoDoor(line, open))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 111:
					// Blazing Door Raise (faster than TURBO!)
					//if (EV_DoDoor(line, blazeRaise))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 112:
					// Blazing Door Open (faster than TURBO!)
					//if (EV_DoDoor(line, blazeOpen))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 113:
					// Blazing Door Close (faster than TURBO!)
					//if (EV_DoDoor(line, blazeClose))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 122:
					// Blazing PlatDownWaitUpStay
					if (sa.EV_DoPlat(line, PlatformType.BlazeDwus, 0))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 127:
					// Build Stairs Turbo 16
					//if (EV_BuildStairs(line, turbo16))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 131:
					// Raise Floor Turbo
					//if (EV_DoFloor(line, raiseFloorTurbo))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 133:
				// BlzOpenDoor BLUE
				case 135:
				// BlzOpenDoor RED
				case 137:
					// BlzOpenDoor YELLOW
					//if (EV_DoLockedDoor(line, blazeOpen, thing))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				case 140:
					// Raise Floor 512
					//if (EV_DoFloor(line, raiseFloor512))
					{
						//P_ChangeSwitchTexture(line, 0);
					}
					break;

				// BUTTONS
				case 42:
					// Close Door
					//if (EV_DoDoor(line, close))
					{
						//P_ChangeSwitchTexture(line, 1);
					}
					break;

				case 43:
					// Lower Ceiling to Floor
					//if (EV_DoCeiling(line, lowerToFloor))
					{
						//P_ChangeSwitchTexture(line, 1);
					}
					break;

				case 45:
					// Lower Floor to Surrounding floor height
					//if (EV_DoFloor(line, lowerFloor))
					{
						//P_ChangeSwitchTexture(line, 1);
					}
					break;

				case 60:
					// Lower Floor to Lowest
					//if (EV_DoFloor(line, lowerFloorToLowest))
					{
						//P_ChangeSwitchTexture(line, 1);
					}
					break;

				case 61:
					// Open Door
					//if (EV_DoDoor(line, open))
					{
						//P_ChangeSwitchTexture(line, 1);
					}
					break;

				case 62:
					// PlatDownWaitUpStay
					if (sa.EV_DoPlat(line, PlatformType.DownWaitUpStay, 1))
					{
						//P_ChangeSwitchTexture(line, 1);
					}
					break;

				case 63:
					// Raise Door
					//if (EV_DoDoor(line, normal))
					{
						//P_ChangeSwitchTexture(line, 1);
					}
					break;

				case 64:
					// Raise Floor to ceiling
					//if (EV_DoFloor(line, raiseFloor))
					{
						//P_ChangeSwitchTexture(line, 1);
					}
					break;

				case 66:
					// Raise Floor 24 and change texture
					if (sa.EV_DoPlat(line, PlatformType.RaiseAndChange, 24))
					{
						//P_ChangeSwitchTexture(line, 1);
					}
					break;

				case 67:
					// Raise Floor 32 and change texture
					if (sa.EV_DoPlat(line, PlatformType.RaiseAndChange, 32))
					{
						//P_ChangeSwitchTexture(line, 1);
					}
					break;

				case 65:
					// Raise Floor Crush
					//if (EV_DoFloor(line, raiseFloorCrush))
					{
						//P_ChangeSwitchTexture(line, 1);
					}
					break;

				case 68:
					// Raise Plat to next highest floor and change texture
					if (sa.EV_DoPlat(line, PlatformType.RaiseToNearestAndChange, 0))
					{
						//P_ChangeSwitchTexture(line, 1);
					}
					break;

				case 69:
					// Raise Floor to next highest floor
					//if (EV_DoFloor(line, raiseFloorToNearest))
					{
						//P_ChangeSwitchTexture(line, 1);
					}
					break;

				case 70:
					// Turbo Lower Floor
					//if (EV_DoFloor(line, turboLower))
					{
						//P_ChangeSwitchTexture(line, 1);
					}
					break;

				case 114:
					// Blazing Door Raise (faster than TURBO!)
					//if (EV_DoDoor(line, blazeRaise))
					{
						//P_ChangeSwitchTexture(line, 1);
					}
					break;

				case 115:
					// Blazing Door Open (faster than TURBO!)
					//if (EV_DoDoor(line, blazeOpen))
					{
						//P_ChangeSwitchTexture(line, 1);
					}
					break;

				case 116:
					// Blazing Door Close (faster than TURBO!)
					//if (EV_DoDoor(line, blazeClose))
					{
						//P_ChangeSwitchTexture(line, 1);
					}
					break;

				case 123:
					// Blazing PlatDownWaitUpStay
					if (sa.EV_DoPlat(line, PlatformType.BlazeDwus, 0))
					{
						//P_ChangeSwitchTexture(line, 1);
					}
					break;

				case 132:
					// Raise Floor Turbo
					//if (EV_DoFloor(line, raiseFloorTurbo))
					{
						//P_ChangeSwitchTexture(line, 1);
					}
					break;

				case 99:
				// BlzOpenDoor BLUE
				case 134:
				// BlzOpenDoor RED
				case 136:
					// BlzOpenDoor YELLOW
					//if (EV_DoLockedDoor(line, blazeOpen, thing))
					{
						//P_ChangeSwitchTexture(line, 1);
					}
					break;

				case 138:
					// Light Turn On
					//EV_LightTurnOn(line, 255);
					//P_ChangeSwitchTexture(line, 1);
					break;

				case 139:
					// Light Turn Off
					//EV_LightTurnOn(line, 35);
					//P_ChangeSwitchTexture(line, 1);
					break;
			}

			return true;
		}






		//
		// USE LINES
		//
		Mobj usething;

		private bool PTR_UseTraverse(Intercept ic)
		{
			var mc = world.MapCollision;

			if (ic.Line.Special == 0)
			{
				mc.LineOpening(ic.Line);
				if (mc.OpenRange <= Fixed.Zero)
				{
					world.StartSound(usething, Sfx.NOWAY);

					// can't use through a wall
					return false;
				}

				// not a special line, but keep checking
				return true;
			}

			var side = 0;
			if (Geometry.PointOnLineSide(usething.X, usething.Y, ic.Line) == 1)
			{
				side = 1;
			}

			// don't use back side
			//return false;

			UseSpecialLine(usething, ic.Line, side);

			// can't use for than one special line in a row
			return false;
		}

		//
		// P_UseLines
		// Looks for special lines in front of the player to activate.
		//
		public void UseLines(Player player)
		{
			var pt = world.PathTraversal;

			usething = player.Mobj;

			var angle = player.Mobj.Angle; // >> ANGLETOFINESHIFT;

			var x1 = player.Mobj.X;
			var y1 = player.Mobj.Y;
			var x2 = x1 + (World.USERANGE.Data >> Fixed.FracBits) * Trig.Cos(angle); // finecosine[angle];
			var y2 = y1 + (World.USERANGE.Data >> Fixed.FracBits) * Trig.Sin(angle); // finesine[angle];

			pt.PathTraverse(x1, y1, x2, y2, PathTraverseFlags.AddLines, ic => PTR_UseTraverse(ic));
		}












		//
		// P_CrossSpecialLine - TRIGGER
		// Called every time a thing origin is about
		//  to cross a line with a non 0 special.
		//
		public void CrossSpecialLine(LineDef line, int side, Mobj thing)
		{
			//	Triggers that other things can activate
			if (thing.Player == null)
			{
				// Things that should NOT trigger specials...
				switch (thing.Type)
				{
					case MobjType.Rocket:
					case MobjType.Plasma:
					case MobjType.Bfg:
					case MobjType.Troopshot:
					case MobjType.Headshot:
					case MobjType.Bruisershot:
						return;
					default:
						break;
				}

				var ok = false;
				switch ((int)line.Special)
				{
					case 39:    // TELEPORT TRIGGER
					case 97:    // TELEPORT RETRIGGER
					case 125:   // TELEPORT MONSTERONLY TRIGGER
					case 126:   // TELEPORT MONSTERONLY RETRIGGER
					case 4: // RAISE DOOR
					case 10:    // PLAT DOWN-WAIT-UP-STAY TRIGGER
					case 88:    // PLAT DOWN-WAIT-UP-STAY RETRIGGER
						ok = true;
						break;
				}
				if (!ok)
				{
					return;
				}
			}

			var sa = world.SectorAction;

			// Note: could use some const's here.
			switch ((int)line.Special)
			{
				// TRIGGERS.
				// All from here to RETRIGGERS.
				case 2:
					// Open Door
					//EV_DoDoor(line, open);
					line.Special = 0;
					break;

				case 3:
					// Close Door
					//EV_DoDoor(line, close);
					line.Special = 0;
					break;

				case 4:
					// Raise Door
					//EV_DoDoor(line, normal);
					line.Special = 0;
					break;

				case 5:
					// Raise Floor
					//EV_DoFloor(line, raiseFloor);
					line.Special = 0;
					break;

				case 6:
					// Fast Ceiling Crush & Raise
					//EV_DoCeiling(line, fastCrushAndRaise);
					line.Special = 0;
					break;

				case 8:
					// Build Stairs
					//EV_BuildStairs(line, build8);
					line.Special = 0;
					break;

				case 10:
					// PlatDownWaitUp
					sa.EV_DoPlat(line, PlatformType.DownWaitUpStay, 0);
					line.Special = 0;
					break;

				case 12:
					// Light Turn On - brightest near
					//EV_LightTurnOn(line, 0);
					line.Special = 0;
					break;

				case 13:
					// Light Turn On 255
					//EV_LightTurnOn(line, 255);
					line.Special = 0;
					break;

				case 16:
					// Close Door 30
					//EV_DoDoor(line, close30ThenOpen);
					line.Special = 0;
					break;

				case 17:
					// Start Light Strobing
					//EV_StartLightStrobing(line);
					line.Special = 0;
					break;

				case 19:
					// Lower Floor
					//EV_DoFloor(line, lowerFloor);
					line.Special = 0;
					break;

				case 22:
					// Raise floor to nearest height and change texture
					sa.EV_DoPlat(line, PlatformType.RaiseToNearestAndChange, 0);
					line.Special = 0;
					break;

				case 25:
					// Ceiling Crush and Raise
					//EV_DoCeiling(line, crushAndRaise);
					line.Special = 0;
					break;

				case 30:
					// Raise floor to shortest texture height
					//  on either side of lines.
					//EV_DoFloor(line, raiseToTexture);
					line.Special = 0;
					break;

				case 35:
					// Lights Very Dark
					//EV_LightTurnOn(line, 35);
					line.Special = 0;
					break;

				case 36:
					// Lower Floor (TURBO)
					//EV_DoFloor(line, turboLower);
					line.Special = 0;
					break;

				case 37:
					// LowerAndChange
					//EV_DoFloor(line, lowerAndChange);
					line.Special = 0;
					break;

				case 38:
					// Lower Floor To Lowest
					//EV_DoFloor(line, lowerFloorToLowest);
					line.Special = 0;
					break;

				case 39:
					// TELEPORT!
					//EV_Teleport(line, side, thing);
					line.Special = 0;
					break;

				case 40:
					// RaiseCeilingLowerFloor
					//EV_DoCeiling(line, raiseToHighest);
					//EV_DoFloor(line, lowerFloorToLowest);
					line.Special = 0;
					break;

				case 44:
					// Ceiling Crush
					//EV_DoCeiling(line, lowerAndCrush);
					line.Special = 0;
					break;

				case 52:
					// EXIT!
					//G_ExitLevel();
					break;

				case 53:
					// Perpetual Platform Raise
					sa.EV_DoPlat(line, PlatformType.PerpetualRaise, 0);
					line.Special = 0;
					break;

				case 54:
					// Platform Stop
					sa.EV_StopPlat(line);
					line.Special = 0;
					break;

				case 56:
					// Raise Floor Crush
					//EV_DoFloor(line, raiseFloorCrush);
					line.Special = 0;
					break;

				case 57:
					// Ceiling Crush Stop
					//EV_CeilingCrushStop(line);
					line.Special = 0;
					break;

				case 58:
					// Raise Floor 24
					//EV_DoFloor(line, raiseFloor24);
					line.Special = 0;
					break;

				case 59:
					// Raise Floor 24 And Change
					//EV_DoFloor(line, raiseFloor24AndChange);
					line.Special = 0;
					break;

				case 104:
					// Turn lights off in sector(tag)
					//EV_TurnTagLightsOff(line);
					line.Special = 0;
					break;

				case 108:
					// Blazing Door Raise (faster than TURBO!)
					//EV_DoDoor(line, blazeRaise);
					line.Special = 0;
					break;

				case 109:
					// Blazing Door Open (faster than TURBO!)
					//EV_DoDoor(line, blazeOpen);
					line.Special = 0;
					break;

				case 100:
					// Build Stairs Turbo 16
					//EV_BuildStairs(line, turbo16);
					line.Special = 0;
					break;

				case 110:
					// Blazing Door Close (faster than TURBO!)
					//EV_DoDoor(line, blazeClose);
					line.Special = 0;
					break;

				case 119:
					// Raise floor to nearest surr. floor
					//EV_DoFloor(line, raiseFloorToNearest);
					line.Special = 0;
					break;

				case 121:
					// Blazing PlatDownWaitUpStay
					sa.EV_DoPlat(line, PlatformType.BlazeDwus, 0);
					line.Special = 0;
					break;

				case 124:
					// Secret EXIT
					//G_SecretExitLevel();
					break;

				case 125:
					// TELEPORT MonsterONLY
					if (thing.Player == null)
					{
						//EV_Teleport(line, side, thing);
						line.Special = 0;
					}
					break;

				case 130:
					// Raise Floor Turbo
					//EV_DoFloor(line, raiseFloorTurbo);
					line.Special = 0;
					break;

				case 141:
					// Silent Ceiling Crush & Raise
					//EV_DoCeiling(line, silentCrushAndRaise);
					line.Special = 0;
					break;

				// RETRIGGERS.  All from here till end.
				case 72:
					// Ceiling Crush
					//EV_DoCeiling(line, lowerAndCrush);
					break;

				case 73:
					// Ceiling Crush and Raise
					//EV_DoCeiling(line, crushAndRaise);
					break;

				case 74:
					// Ceiling Crush Stop
					//EV_CeilingCrushStop(line);
					break;

				case 75:
					// Close Door
					//EV_DoDoor(line, close);
					break;

				case 76:
					// Close Door 30
					//EV_DoDoor(line, close30ThenOpen);
					break;

				case 77:
					// Fast Ceiling Crush & Raise
					//EV_DoCeiling(line, fastCrushAndRaise);
					break;

				case 79:
					// Lights Very Dark
					//EV_LightTurnOn(line, 35);
					break;

				case 80:
					// Light Turn On - brightest near
					//EV_LightTurnOn(line, 0);
					break;

				case 81:
					// Light Turn On 255
					//EV_LightTurnOn(line, 255);
					break;

				case 82:
					// Lower Floor To Lowest
					//EV_DoFloor(line, lowerFloorToLowest);
					break;

				case 83:
					// Lower Floor
					//EV_DoFloor(line, lowerFloor);
					break;

				case 84:
					// LowerAndChange
					//EV_DoFloor(line, lowerAndChange);
					break;

				case 86:
					// Open Door
					//EV_DoDoor(line, open);
					break;

				case 87:
					// Perpetual Platform Raise
					sa.EV_DoPlat(line, PlatformType.PerpetualRaise, 0);
					break;

				case 88:
					// PlatDownWaitUp
					sa.EV_DoPlat(line, PlatformType.DownWaitUpStay, 0);
					break;

				case 89:
					// Platform Stop
					sa.EV_StopPlat(line);
					break;

				case 90:
					// Raise Door
					//EV_DoDoor(line, normal);
					break;

				case 91:
					// Raise Floor
					//EV_DoFloor(line, raiseFloor);
					break;

				case 92:
					// Raise Floor 24
					//EV_DoFloor(line, raiseFloor24);
					break;

				case 93:
					// Raise Floor 24 And Change
					//EV_DoFloor(line, raiseFloor24AndChange);
					break;

				case 94:
					// Raise Floor Crush
					//EV_DoFloor(line, raiseFloorCrush);
					break;

				case 95:
					// Raise floor to nearest height
					// and change texture.
					sa.EV_DoPlat(line, PlatformType.RaiseToNearestAndChange, 0);
					break;

				case 96:
					// Raise floor to shortest texture height
					// on either side of lines.
					//EV_DoFloor(line, raiseToTexture);
					break;

				case 97:
					// TELEPORT!
					//EV_Teleport(line, side, thing);
					break;

				case 98:
					// Lower Floor (TURBO)
					//EV_DoFloor(line, turboLower);
					break;

				case 105:
					// Blazing Door Raise (faster than TURBO!)
					//EV_DoDoor(line, blazeRaise);
					break;

				case 106:
					// Blazing Door Open (faster than TURBO!)
					//EV_DoDoor(line, blazeOpen);
					break;

				case 107:
					// Blazing Door Close (faster than TURBO!)
					//EV_DoDoor(line, blazeClose);
					break;

				case 120:
					// Blazing PlatDownWaitUpStay.
					sa.EV_DoPlat(line, PlatformType.BlazeDwus, 0);
					break;

				case 126:
					// TELEPORT MonsterONLY.
					if (thing.Player == null)
					{
						//EV_Teleport(line, side, thing);
					}
					break;

				case 128:
					// Raise To Nearest Floor
					//EV_DoFloor(line, raiseFloorToNearest);
					break;

				case 129:
					// Raise Floor Turbo
					//EV_DoFloor(line, raiseFloorTurbo);
					break;
			}
		}
	}
}
