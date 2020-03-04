using System;

namespace ManagedDoom
{
	public sealed partial class World
	{
		//
		// KillMobj
		//
		public void KillMobj(Mobj source, Mobj target)
		{
			target.Flags &= ~(MobjFlags.Shootable | MobjFlags.Float | MobjFlags.SkullFly);

			if (target.Type != MobjType.Skull)
			{
				target.Flags &= ~MobjFlags.NoGravity;
			}

			target.Flags |= MobjFlags.Corpse | MobjFlags.DropOff;
			target.Height = new Fixed(target.Height.Data >> 2);

			if (source != null && source.Player != null)
			{
				// count for intermission
				if ((target.Flags & MobjFlags.CountKill) != 0)
				{
					source.Player.KillCount++;
				}

				if (target.Player != null)
				{
					//source.Player.Frags[target->player - players]++;
				}
			}
			else if (!Options.NetGame && (target.Flags & MobjFlags.CountKill) != 0)
			{
				// count all monster deaths,
				// even those caused by other monsters
				//players[0].killcount++;
			}

			if (target.Player != null)
			{
				// count environment kills against you
				if (source == null)
				{
					//target->player->frags[target->player - players]++;
				}

				target.Flags &= ~MobjFlags.Solid;
				target.Player.PlayerState = PlayerState.Dead;
				//P_DropWeapon(target->player);

				/*
				if (target->player == &players[consoleplayer]
					&& automapactive)
				{
					// don't die in auto map,
					// switch view prior to dying
					AM_Stop();
				}
				*/
			}

			if (target.Health < -target.Info.SpawnHealth
				&& target.Info.XdeathState != 0)
			{
				SetMobjState(target, target.Info.XdeathState);
			}
			else
			{
				SetMobjState(target, target.Info.DeathState);
			}

			target.Tics -= random.Next() & 3;
			if (target.Tics < 1)
			{
				target.Tics = 1;
			}

			//	I_StartSound (&actor->r, actor->info->deathsound);


			// Drop stuff.
			// This determines the kind of object spawned
			// during the death frame of a thing.
			MobjType item;
			switch (target.Type)
			{
				case MobjType.Wolfss:
				case MobjType.Possessed:
					item = MobjType.Clip;
					break;

				case MobjType.Shotguy:
					item = MobjType.Shotgun;
					break;

				case MobjType.Chainguy:
					item = MobjType.Chaingun;
					break;

				default:
					return;
			}

			var mo = SpawnMobj(target.X, target.Y, Mobj.OnFloorZ, item);
			mo.Flags |= MobjFlags.Dropped; // special versions of items
		}








		private int BASETHRESHOLD = 100;


		//
		// P_DamageMobj
		// Damages both enemies and players
		// "inflictor" is the thing that caused the damage
		//  creature or missile, can be NULL (slime, etc)
		// "source" is the thing to target after taking damage
		//  creature or NULL
		// Source and inflictor are the same for melee attacks.
		// Source can be NULL for slime, barrel explosions
		// and other environmental stuff.
		//
		public void DamageMobj(
			Mobj target,
			Mobj inflictor,
			Mobj source,
			int damage)
		{
			int temp;

			if ((target.Flags & MobjFlags.Shootable) == 0)
			{
				// shouldn't happen...
				return;
			}

			if (target.Health <= 0)
			{
				return;
			}

			if ((target.Flags & MobjFlags.SkullFly) != 0)
			{
				target.MomX = target.MomY = target.MomZ = Fixed.Zero;
			}

			var player = target.Player;
			if (player != null && Options.GameSkill == Skill.Baby)
			{
				// take half damage in trainer mode
				damage >>= 1;
			}

			// Some close combat weapons should not
			// inflict thrust and push the victim out of reach,
			// thus kick away unless using the chainsaw.
			if (inflictor != null
				&& (target.Flags & MobjFlags.NoClip) == 0
				&& (source == null
					|| source.Player == null
					|| source.Player.ReadyWeapon != WeaponType.Chainsaw))
			{
				var ang = Geometry.PointToAngle(
					inflictor.X,
					inflictor.Y,
					target.X,
					target.Y);

				var thrust = new Fixed(damage * (Fixed.FracUnit >> 3) * 100 / target.Info.Mass);

				// make fall forwards sometimes
				if (damage < 40
					&& damage > target.Health
					&& target.Z - inflictor.Z > Fixed.FromInt(64)
					&& (random.Next() & 1) != 0)
				{
					ang += Angle.Ang180;
					thrust *= 4;
				}

				//ang >>= ANGLETOFINESHIFT;
				target.MomX += thrust * Trig.Cos(ang); // finecosine[ang]);
				target.MomY += thrust * Trig.Sin(ang); // finesine[ang]);
			}

			// player specific
			if (player != null)
			{
				// end of game hell hack
				if (target.Subsector.Sector.Special == (SectorSpecial)11
					&& damage >= target.Health)
				{
					damage = target.Health - 1;
				}

				// Below certain threshold,
				// ignore damage in GOD mode, or with INVUL power.
				if (damage < 1000 && ((player.Cheats & CheatFlags.GodMode) != 0
					|| player.Powers[(int)PowerType.Invulnerability] > 0))
				{
					return;
				}

				int saved;
				if (player.ArmorType != 0)
				{
					if (player.ArmorType == 1)
					{
						saved = damage / 3;
					}
					else
						saved = damage / 2;

					if (player.ArmorPoints <= saved)
					{
						// armor is used up
						saved = player.ArmorPoints;
						player.ArmorType = 0;
					}
					player.ArmorPoints -= saved;
					damage -= saved;
				}

				// mirror mobj health here for Dave
				player.Health -= damage;
				if (player.Health < 0)
				{
					player.Health = 0;
				}

				player.Attacker = source;

				// add damage after armor / invuln
				player.DamageCount += damage;

				if (player.DamageCount > 100)
				{
					// teleport stomp does 10k points...
					player.DamageCount = 100;
				}

				temp = damage < 100 ? damage : 100;

				/*
				if (player == &players[consoleplayer])
				{
					I_Tactile(40, 10, 40 + temp * 2);
				}
				*/
			}

			// do the damage	
			target.Health -= damage;
			if (target.Health <= 0)
			{
				KillMobj(source, target);
				return;
			}

			if ((random.Next() < target.Info.PainChance)
				&& (target.Flags & MobjFlags.SkullFly) == 0)
			{
				// fight back!
				target.Flags |= MobjFlags.JustHit;

				SetMobjState(target, target.Info.PainState);
			}

			// we're awake now...
			target.ReactionTime = 0;

			if ((target.Threshold == 0 || target.Type == MobjType.Vile)
				&& source != null && source != target
				&& source.Type != MobjType.Vile)
			{
				// if not intent on another player,
				// chase after this one
				target.Target = source;
				target.Threshold = BASETHRESHOLD;
				if (target.State == Info.States[(int)target.Info.SpawnState]
					&& target.Info.SeeState != State.Null)
				{
					SetMobjState(target, target.Info.SeeState);
				}
			}

		}








		//
		// P_UseSpecialLine
		// Called when a thing uses a special line.
		// Only the front sides of lines are usable.
		//
		public bool UseSpecialLine(Mobj thing, LineDef line, int side)
		{
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
					EV_VerticalDoor(line, thing);
					break;

					//UNUSED - Door Slide Open&Close
					// case 124:
					// EV_SlidingDoor (line, thing);
					// break;

					/*
					// SWITCHES
					case 7:
						// Build Stairs
						if (EV_BuildStairs(line, build8))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 9:
						// Change Donut
						if (EV_DoDonut(line))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 11:
						// Exit level
						P_ChangeSwitchTexture(line, 0);
						G_ExitLevel();
						break;

					case 14:
						// Raise Floor 32 and change texture
						if (EV_DoPlat(line, raiseAndChange, 32))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 15:
						// Raise Floor 24 and change texture
						if (EV_DoPlat(line, raiseAndChange, 24))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 18:
						// Raise Floor to next highest floor
						if (EV_DoFloor(line, raiseFloorToNearest))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 20:
						// Raise Plat next highest floor and change texture
						if (EV_DoPlat(line, raiseToNearestAndChange, 0))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 21:
						// PlatDownWaitUpStay
						if (EV_DoPlat(line, downWaitUpStay, 0))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 23:
						// Lower Floor to Lowest
						if (EV_DoFloor(line, lowerFloorToLowest))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 29:
						// Raise Door
						if (EV_DoDoor(line, normal))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 41:
						// Lower Ceiling to Floor
						if (EV_DoCeiling(line, lowerToFloor))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 71:
						// Turbo Lower Floor
						if (EV_DoFloor(line, turboLower))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 49:
						// Ceiling Crush And Raise
						if (EV_DoCeiling(line, crushAndRaise))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 50:
						// Close Door
						if (EV_DoDoor(line, close))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 51:
						// Secret EXIT
						P_ChangeSwitchTexture(line, 0);
						G_SecretExitLevel();
						break;

					case 55:
						// Raise Floor Crush
						if (EV_DoFloor(line, raiseFloorCrush))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 101:
						// Raise Floor
						if (EV_DoFloor(line, raiseFloor))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 102:
						// Lower Floor to Surrounding floor height
						if (EV_DoFloor(line, lowerFloor))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 103:
						// Open Door
						if (EV_DoDoor(line, open))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 111:
						// Blazing Door Raise (faster than TURBO!)
						if (EV_DoDoor(line, blazeRaise))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 112:
						// Blazing Door Open (faster than TURBO!)
						if (EV_DoDoor(line, blazeOpen))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 113:
						// Blazing Door Close (faster than TURBO!)
						if (EV_DoDoor(line, blazeClose))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 122:
						// Blazing PlatDownWaitUpStay
						if (EV_DoPlat(line, blazeDWUS, 0))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 127:
						// Build Stairs Turbo 16
						if (EV_BuildStairs(line, turbo16))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 131:
						// Raise Floor Turbo
						if (EV_DoFloor(line, raiseFloorTurbo))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 133:
					// BlzOpenDoor BLUE
					case 135:
					// BlzOpenDoor RED
					case 137:
						// BlzOpenDoor YELLOW
						if (EV_DoLockedDoor(line, blazeOpen, thing))
							P_ChangeSwitchTexture(line, 0);
						break;

					case 140:
						// Raise Floor 512
						if (EV_DoFloor(line, raiseFloor512))
							P_ChangeSwitchTexture(line, 0);
						break;

					// BUTTONS
					case 42:
						// Close Door
						if (EV_DoDoor(line, close))
							P_ChangeSwitchTexture(line, 1);
						break;

					case 43:
						// Lower Ceiling to Floor
						if (EV_DoCeiling(line, lowerToFloor))
							P_ChangeSwitchTexture(line, 1);
						break;

					case 45:
						// Lower Floor to Surrounding floor height
						if (EV_DoFloor(line, lowerFloor))
							P_ChangeSwitchTexture(line, 1);
						break;

					case 60:
						// Lower Floor to Lowest
						if (EV_DoFloor(line, lowerFloorToLowest))
							P_ChangeSwitchTexture(line, 1);
						break;

					case 61:
						// Open Door
						if (EV_DoDoor(line, open))
							P_ChangeSwitchTexture(line, 1);
						break;

					case 62:
						// PlatDownWaitUpStay
						if (EV_DoPlat(line, downWaitUpStay, 1))
							P_ChangeSwitchTexture(line, 1);
						break;

					case 63:
						// Raise Door
						if (EV_DoDoor(line, normal))
							P_ChangeSwitchTexture(line, 1);
						break;

					case 64:
						// Raise Floor to ceiling
						if (EV_DoFloor(line, raiseFloor))
							P_ChangeSwitchTexture(line, 1);
						break;

					case 66:
						// Raise Floor 24 and change texture
						if (EV_DoPlat(line, raiseAndChange, 24))
							P_ChangeSwitchTexture(line, 1);
						break;

					case 67:
						// Raise Floor 32 and change texture
						if (EV_DoPlat(line, raiseAndChange, 32))
							P_ChangeSwitchTexture(line, 1);
						break;

					case 65:
						// Raise Floor Crush
						if (EV_DoFloor(line, raiseFloorCrush))
							P_ChangeSwitchTexture(line, 1);
						break;

					case 68:
						// Raise Plat to next highest floor and change texture
						if (EV_DoPlat(line, raiseToNearestAndChange, 0))
							P_ChangeSwitchTexture(line, 1);
						break;

					case 69:
						// Raise Floor to next highest floor
						if (EV_DoFloor(line, raiseFloorToNearest))
							P_ChangeSwitchTexture(line, 1);
						break;

					case 70:
						// Turbo Lower Floor
						if (EV_DoFloor(line, turboLower))
							P_ChangeSwitchTexture(line, 1);
						break;

					case 114:
						// Blazing Door Raise (faster than TURBO!)
						if (EV_DoDoor(line, blazeRaise))
							P_ChangeSwitchTexture(line, 1);
						break;

					case 115:
						// Blazing Door Open (faster than TURBO!)
						if (EV_DoDoor(line, blazeOpen))
							P_ChangeSwitchTexture(line, 1);
						break;

					case 116:
						// Blazing Door Close (faster than TURBO!)
						if (EV_DoDoor(line, blazeClose))
							P_ChangeSwitchTexture(line, 1);
						break;

					case 123:
						// Blazing PlatDownWaitUpStay
						if (EV_DoPlat(line, blazeDWUS, 0))
							P_ChangeSwitchTexture(line, 1);
						break;

					case 132:
						// Raise Floor Turbo
						if (EV_DoFloor(line, raiseFloorTurbo))
							P_ChangeSwitchTexture(line, 1);
						break;

					case 99:
					// BlzOpenDoor BLUE
					case 134:
					// BlzOpenDoor RED
					case 136:
						// BlzOpenDoor YELLOW
						if (EV_DoLockedDoor(line, blazeOpen, thing))
							P_ChangeSwitchTexture(line, 1);
						break;

					case 138:
						// Light Turn On
						EV_LightTurnOn(line, 255);
						P_ChangeSwitchTexture(line, 1);
						break;

					case 139:
						// Light Turn Off
						EV_LightTurnOn(line, 35);
						P_ChangeSwitchTexture(line, 1);
						break;
					*/
			}

			return true;
		}






		//
		// USE LINES
		//
		Mobj usething;

		private bool PTR_UseTraverse(Intercept ic)
		{
			if (ic.Line.Special == 0)
			{
				LineOpening(ic.Line);
				if (openRange <= Fixed.Zero)
				{
					StartSound(usething, Sfx.NOWAY);

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
			usething = player.Mobj;

			var angle = player.Mobj.Angle; // >> ANGLETOFINESHIFT;

			var x1 = player.Mobj.X;
			var y1 = player.Mobj.Y;
			var x2 = x1 + (USERANGE.Data >> Fixed.FracBits) * Trig.Cos(angle); // finecosine[angle];
			var y2 = y1 + (USERANGE.Data >> Fixed.FracBits) * Trig.Sin(angle); // finesine[angle];

			PathTraverse(x1, y1, x2, y2, PathTraverseFlags.AddLines, ic => PTR_UseTraverse(ic));
		}

	}
}
