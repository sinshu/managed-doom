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
			var specials = world.Specials;
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
					sa.VerticalDoor(line, thing);
					break;

				//UNUSED - Door Slide Open&Close
				// case 124:
				// EV_SlidingDoor (line, thing);
				// break;

				// SWITCHES
				case 7:
					// Build Stairs
					if (sa.EV_BuildStairs(line, StairType.Build8))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 9:
					// Change Donut
					if (sa.EV_DoDonut(line))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 11:
					// Exit level
					specials.ChangeSwitchTexture(line, false);
					world.G_ExitLevel();
					break;

				case 14:
					// Raise Floor 32 and change texture
					if (sa.EV_DoPlat(line, PlatformType.RaiseAndChange, 32))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 15:
					// Raise Floor 24 and change texture
					if (sa.EV_DoPlat(line, PlatformType.RaiseAndChange, 24))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 18:
					// Raise Floor to next highest floor
					if (sa.EV_DoFloor(line, FloorMoveType.RaiseFloorToNearest))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 20:
					// Raise Plat next highest floor and change texture
					if (sa.EV_DoPlat(line, PlatformType.RaiseToNearestAndChange, 0))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 21:
					// PlatDownWaitUpStay
					if (sa.EV_DoPlat(line, PlatformType.DownWaitUpStay, 0))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 23:
					// Lower Floor to Lowest
					if (sa.EV_DoFloor(line, FloorMoveType.LowerFloorToLowest))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 29:
					// Raise Door
					if (sa.DoDoor(line, VlDoorType.Normal))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 41:
					// Lower Ceiling to Floor
					if (sa.EV_DoCeiling(line, CeilingMoveType.LowerToFloor))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 71:
					// Turbo Lower Floor
					if (sa.EV_DoFloor(line, FloorMoveType.TurboLower))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 49:
					// Ceiling Crush And Raise
					if (sa.EV_DoCeiling(line, CeilingMoveType.CrushAndRaise))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 50:
					// Close Door
					if (sa.DoDoor(line, VlDoorType.Close))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 51:
					// Secret EXIT
					//P_ChangeSwitchTexture(line, 0);
					//G_SecretExitLevel();
					break;

				case 55:
					// Raise Floor Crush
					if (sa.EV_DoFloor(line, FloorMoveType.RaiseFloorCrush))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 101:
					// Raise Floor
					if (sa.EV_DoFloor(line, FloorMoveType.RaiseFloor))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 102:
					// Lower Floor to Surrounding floor height
					if (sa.EV_DoFloor(line, FloorMoveType.LowerFloor))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 103:
					// Open Door
					if (sa.DoDoor(line, VlDoorType.Open))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 111:
					// Blazing Door Raise (faster than TURBO!)
					if (sa.DoDoor(line, VlDoorType.BlazeRaise))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 112:
					// Blazing Door Open (faster than TURBO!)
					if (sa.DoDoor(line, VlDoorType.BlazeOpen))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 113:
					// Blazing Door Close (faster than TURBO!)
					if (sa.DoDoor(line, VlDoorType.BlazeClose))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 122:
					// Blazing PlatDownWaitUpStay
					if (sa.EV_DoPlat(line, PlatformType.BlazeDwus, 0))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 127:
					// Build Stairs Turbo 16
					if (sa.EV_BuildStairs(line, StairType.Turbo16))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 131:
					// Raise Floor Turbo
					if (sa.EV_DoFloor(line, FloorMoveType.RaiseFloorTurbo))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 133:
				// BlzOpenDoor BLUE
				case 135:
				// BlzOpenDoor RED
				case 137:
					// BlzOpenDoor YELLOW
					if (sa.DoLockedDoor(line, VlDoorType.BlazeOpen, thing))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				case 140:
					// Raise Floor 512
					if (sa.EV_DoFloor(line, FloorMoveType.RaiseFloor512))
					{
						specials.ChangeSwitchTexture(line, false);
					}
					break;

				// BUTTONS
				case 42:
					// Close Door
					if (sa.DoDoor(line, VlDoorType.Close))
					{
						specials.ChangeSwitchTexture(line, true);
					}
					break;

				case 43:
					// Lower Ceiling to Floor
					if (sa.EV_DoCeiling(line, CeilingMoveType.LowerToFloor))
					{
						specials.ChangeSwitchTexture(line, true);
					}
					break;

				case 45:
					// Lower Floor to Surrounding floor height
					if (sa.EV_DoFloor(line, FloorMoveType.LowerFloor))
					{
						specials.ChangeSwitchTexture(line, true);
					}
					break;

				case 60:
					// Lower Floor to Lowest
					if (sa.EV_DoFloor(line, FloorMoveType.LowerFloorToLowest))
					{
						specials.ChangeSwitchTexture(line, true);
					}
					break;

				case 61:
					// Open Door
					if (sa.DoDoor(line, VlDoorType.Open))
					{
						specials.ChangeSwitchTexture(line, true);
					}
					break;

				case 62:
					// PlatDownWaitUpStay
					if (sa.EV_DoPlat(line, PlatformType.DownWaitUpStay, 1))
					{
						specials.ChangeSwitchTexture(line, true);
					}
					break;

				case 63:
					// Raise Door
					if (sa.DoDoor(line, VlDoorType.Normal))
					{
						specials.ChangeSwitchTexture(line, true);
					}
					break;

				case 64:
					// Raise Floor to ceiling
					if (sa.EV_DoFloor(line, FloorMoveType.RaiseFloor))
					{
						specials.ChangeSwitchTexture(line, true);
					}
					break;

				case 66:
					// Raise Floor 24 and change texture
					if (sa.EV_DoPlat(line, PlatformType.RaiseAndChange, 24))
					{
						specials.ChangeSwitchTexture(line, true);
					}
					break;

				case 67:
					// Raise Floor 32 and change texture
					if (sa.EV_DoPlat(line, PlatformType.RaiseAndChange, 32))
					{
						specials.ChangeSwitchTexture(line, true);
					}
					break;

				case 65:
					// Raise Floor Crush
					if (sa.EV_DoFloor(line, FloorMoveType.RaiseFloorCrush))
					{
						specials.ChangeSwitchTexture(line, true);
					}
					break;

				case 68:
					// Raise Plat to next highest floor and change texture
					if (sa.EV_DoPlat(line, PlatformType.RaiseToNearestAndChange, 0))
					{
						specials.ChangeSwitchTexture(line, true);
					}
					break;

				case 69:
					// Raise Floor to next highest floor
					if (sa.EV_DoFloor(line, FloorMoveType.RaiseFloorToNearest))
					{
						specials.ChangeSwitchTexture(line, true);
					}
					break;

				case 70:
					// Turbo Lower Floor
					if (sa.EV_DoFloor(line, FloorMoveType.TurboLower))
					{
						specials.ChangeSwitchTexture(line, true);
					}
					break;

				case 114:
					// Blazing Door Raise (faster than TURBO!)
					if (sa.DoDoor(line, VlDoorType.BlazeRaise))
					{
						specials.ChangeSwitchTexture(line, true);
					}
					break;

				case 115:
					// Blazing Door Open (faster than TURBO!)
					if (sa.DoDoor(line, VlDoorType.BlazeOpen))
					{
						specials.ChangeSwitchTexture(line, true);
					}
					break;

				case 116:
					// Blazing Door Close (faster than TURBO!)
					if (sa.DoDoor(line, VlDoorType.BlazeClose))
					{
						specials.ChangeSwitchTexture(line, true);
					}
					break;

				case 123:
					// Blazing PlatDownWaitUpStay
					if (sa.EV_DoPlat(line, PlatformType.BlazeDwus, 0))
					{
						specials.ChangeSwitchTexture(line, true);
					}
					break;

				case 132:
					// Raise Floor Turbo
					if (sa.EV_DoFloor(line, FloorMoveType.RaiseFloorTurbo))
					{
						specials.ChangeSwitchTexture(line, true);
					}
					break;

				case 99:
				// BlzOpenDoor BLUE
				case 134:
				// BlzOpenDoor RED
				case 136:
					// BlzOpenDoor YELLOW
					if (sa.DoLockedDoor(line, VlDoorType.BlazeOpen, thing))
					{
						specials.ChangeSwitchTexture(line, true);
					}
					break;

				case 138:
					// Light Turn On
					sa.EV_LightTurnOn(line, 255);
					specials.ChangeSwitchTexture(line, true);
					break;

				case 139:
					// Light Turn Off
					sa.EV_LightTurnOn(line, 35);
					specials.ChangeSwitchTexture(line, true);
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
					sa.DoDoor(line, VlDoorType.Open);
					line.Special = 0;
					break;

				case 3:
					// Close Door
					sa.DoDoor(line, VlDoorType.Close);
					line.Special = 0;
					break;

				case 4:
					// Raise Door
					sa.DoDoor(line, VlDoorType.Normal);
					line.Special = 0;
					break;

				case 5:
					// Raise Floor
					sa.EV_DoFloor(line, FloorMoveType.RaiseFloor);
					line.Special = 0;
					break;

				case 6:
					// Fast Ceiling Crush & Raise
					sa.EV_DoCeiling(line, CeilingMoveType.FastCrushAndRaise);
					line.Special = 0;
					break;

				case 8:
					// Build Stairs
					sa.EV_BuildStairs(line, StairType.Build8);
					line.Special = 0;
					break;

				case 10:
					// PlatDownWaitUp
					sa.EV_DoPlat(line, PlatformType.DownWaitUpStay, 0);
					line.Special = 0;
					break;

				case 12:
					// Light Turn On - brightest near
					sa.EV_LightTurnOn(line, 0);
					line.Special = 0;
					break;

				case 13:
					// Light Turn On 255
					sa.EV_LightTurnOn(line, 255);
					line.Special = 0;
					break;

				case 16:
					// Close Door 30
					sa.DoDoor(line, VlDoorType.Close30ThenOpen);
					line.Special = 0;
					break;

				case 17:
					// Start Light Strobing
					sa.EV_StartLightStrobing(line);
					line.Special = 0;
					break;

				case 19:
					// Lower Floor
					sa.EV_DoFloor(line, FloorMoveType.LowerFloor);
					line.Special = 0;
					break;

				case 22:
					// Raise floor to nearest height and change texture
					sa.EV_DoPlat(line, PlatformType.RaiseToNearestAndChange, 0);
					line.Special = 0;
					break;

				case 25:
					// Ceiling Crush and Raise
					sa.EV_DoCeiling(line, CeilingMoveType.CrushAndRaise);
					line.Special = 0;
					break;

				case 30:
					// Raise floor to shortest texture height
					//  on either side of lines.
					sa.EV_DoFloor(line, FloorMoveType.RaiseToTexture);
					line.Special = 0;
					break;

				case 35:
					// Lights Very Dark
					sa.EV_LightTurnOn(line, 35);
					line.Special = 0;
					break;

				case 36:
					// Lower Floor (TURBO)
					sa.EV_DoFloor(line, FloorMoveType.TurboLower);
					line.Special = 0;
					break;

				case 37:
					// LowerAndChange
					sa.EV_DoFloor(line, FloorMoveType.LowerAndChange);
					line.Special = 0;
					break;

				case 38:
					// Lower Floor To Lowest
					sa.EV_DoFloor(line, FloorMoveType.LowerFloorToLowest);
					line.Special = 0;
					break;

				case 39:
					// TELEPORT!
					sa.EV_Teleport(line, side, thing);
					line.Special = 0;
					break;

				case 40:
					// RaiseCeilingLowerFloor
					sa.EV_DoCeiling(line, CeilingMoveType.RaiseToHighest);
					sa.EV_DoFloor(line, FloorMoveType.LowerFloorToLowest);
					line.Special = 0;
					break;

				case 44:
					// Ceiling Crush
					sa.EV_DoCeiling(line, CeilingMoveType.LowerAndCrush);
					line.Special = 0;
					break;

				case 52:
					// EXIT!
					world.G_ExitLevel();
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
					sa.EV_DoFloor(line, FloorMoveType.RaiseFloorCrush);
					line.Special = 0;
					break;

				case 57:
					// Ceiling Crush Stop
					sa.EV_CeilingCrushStop(line);
					line.Special = 0;
					break;

				case 58:
					// Raise Floor 24
					sa.EV_DoFloor(line, FloorMoveType.RaiseFloor24);
					line.Special = 0;
					break;

				case 59:
					// Raise Floor 24 And Change
					sa.EV_DoFloor(line, FloorMoveType.RaiseFloor24AndChange);
					line.Special = 0;
					break;

				case 104:
					// Turn lights off in sector(tag)
					sa.EV_TurnTagLightsOff(line);
					line.Special = 0;
					break;

				case 108:
					// Blazing Door Raise (faster than TURBO!)
					sa.DoDoor(line, VlDoorType.BlazeRaise);
					line.Special = 0;
					break;

				case 109:
					// Blazing Door Open (faster than TURBO!)
					sa.DoDoor(line, VlDoorType.BlazeOpen);
					line.Special = 0;
					break;

				case 100:
					// Build Stairs Turbo 16
					sa.EV_BuildStairs(line, StairType.Turbo16);
					line.Special = 0;
					break;

				case 110:
					// Blazing Door Close (faster than TURBO!)
					sa.DoDoor(line, VlDoorType.BlazeClose);
					line.Special = 0;
					break;

				case 119:
					// Raise floor to nearest surr. floor
					sa.EV_DoFloor(line, FloorMoveType.RaiseFloorToNearest);
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
						sa.EV_Teleport(line, side, thing);
						line.Special = 0;
					}
					break;

				case 130:
					// Raise Floor Turbo
					sa.EV_DoFloor(line, FloorMoveType.RaiseFloorTurbo);
					line.Special = 0;
					break;

				case 141:
					// Silent Ceiling Crush & Raise
					sa.EV_DoCeiling(line, CeilingMoveType.SilentCrushAndRaise);
					line.Special = 0;
					break;

				// RETRIGGERS.  All from here till end.
				case 72:
					// Ceiling Crush
					sa.EV_DoCeiling(line, CeilingMoveType.LowerAndCrush);
					break;

				case 73:
					// Ceiling Crush and Raise
					sa.EV_DoCeiling(line, CeilingMoveType.CrushAndRaise);
					break;

				case 74:
					// Ceiling Crush Stop
					sa.EV_CeilingCrushStop(line);
					break;

				case 75:
					// Close Door
					sa.DoDoor(line, VlDoorType.Close);
					break;

				case 76:
					// Close Door 30
					sa.DoDoor(line, VlDoorType.Close30ThenOpen);
					break;

				case 77:
					// Fast Ceiling Crush & Raise
					sa.EV_DoCeiling(line, CeilingMoveType.FastCrushAndRaise);
					break;

				case 79:
					// Lights Very Dark
					sa.EV_LightTurnOn(line, 35);
					break;

				case 80:
					// Light Turn On - brightest near
					sa.EV_LightTurnOn(line, 0);
					break;

				case 81:
					// Light Turn On 255
					sa.EV_LightTurnOn(line, 255);
					break;

				case 82:
					// Lower Floor To Lowest
					sa.EV_DoFloor(line, FloorMoveType.LowerFloorToLowest);
					break;

				case 83:
					// Lower Floor
					sa.EV_DoFloor(line, FloorMoveType.LowerFloor);
					break;

				case 84:
					// LowerAndChange
					sa.EV_DoFloor(line, FloorMoveType.LowerAndChange);
					break;

				case 86:
					// Open Door
					sa.DoDoor(line, VlDoorType.Open);
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
					sa.DoDoor(line, VlDoorType.Normal);
					break;

				case 91:
					// Raise Floor
					sa.EV_DoFloor(line, FloorMoveType.RaiseFloor);
					break;

				case 92:
					// Raise Floor 24
					sa.EV_DoFloor(line, FloorMoveType.RaiseFloor24);
					break;

				case 93:
					// Raise Floor 24 And Change
					sa.EV_DoFloor(line, FloorMoveType.RaiseFloor24AndChange);
					break;

				case 94:
					// Raise Floor Crush
					sa.EV_DoFloor(line, FloorMoveType.RaiseFloorCrush);
					break;

				case 95:
					// Raise floor to nearest height
					// and change texture.
					sa.EV_DoPlat(line, PlatformType.RaiseToNearestAndChange, 0);
					break;

				case 96:
					// Raise floor to shortest texture height
					// on either side of lines.
					sa.EV_DoFloor(line, FloorMoveType.RaiseToTexture);
					break;

				case 97:
					// TELEPORT!
					sa.EV_Teleport(line, side, thing);
					break;

				case 98:
					// Lower Floor (TURBO)
					sa.EV_DoFloor(line, FloorMoveType.TurboLower);
					break;

				case 105:
					// Blazing Door Raise (faster than TURBO!)
					sa.DoDoor(line, VlDoorType.BlazeRaise);
					break;

				case 106:
					// Blazing Door Open (faster than TURBO!)
					sa.DoDoor(line, VlDoorType.BlazeOpen);
					break;

				case 107:
					// Blazing Door Close (faster than TURBO!)
					sa.DoDoor(line, VlDoorType.BlazeClose);
					break;

				case 120:
					// Blazing PlatDownWaitUpStay.
					sa.EV_DoPlat(line, PlatformType.BlazeDwus, 0);
					break;

				case 126:
					// TELEPORT MonsterONLY.
					if (thing.Player == null)
					{
						sa.EV_Teleport(line, side, thing);
					}
					break;

				case 128:
					// Raise To Nearest Floor
					sa.EV_DoFloor(line, FloorMoveType.RaiseFloorToNearest);
					break;

				case 129:
					// Raise Floor Turbo
					sa.EV_DoFloor(line, FloorMoveType.RaiseFloorTurbo);
					break;
			}
		}




		//
		// P_ShootSpecialLine - IMPACT SPECIALS
		// Called when a thing shoots a special line.
		//
		public void ShootSpecialLine(Mobj thing, LineDef line)
		{
			bool ok;

			//	Impacts that other things can activate.
			if (thing.Player == null)
			{
				ok = false;
				switch ((int)line.Special)
				{
					case 46:
						// OPEN DOOR IMPACT
						ok = true;
						break;
				}
				if (!ok)
				{
					return;
				}
			}

			var sa = world.SectorAction;
			var specials = world.Specials;

			switch ((int)line.Special)
			{
				case 24:
					// RAISE FLOOR
					sa.EV_DoFloor(line, FloorMoveType.RaiseFloor);
					specials.ChangeSwitchTexture(line, false);
					break;

				case 46:
					// OPEN DOOR
					sa.DoDoor(line, VlDoorType.Open);
					specials.ChangeSwitchTexture(line, true);
					break;

				case 47:
					// RAISE FLOOR NEAR AND CHANGE
					sa.EV_DoPlat(line, PlatformType.RaiseToNearestAndChange, 0);
					specials.ChangeSwitchTexture(line, false);
					break;
			}
		}
	}
}
