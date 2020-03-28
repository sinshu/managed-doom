using System;

namespace ManagedDoom
{
	public sealed class ThingInteraction
	{
		private World world;

		public ThingInteraction(World world)
		{
			this.world = world;
		}

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
			else if (!world.Options.NetGame && (target.Flags & MobjFlags.CountKill) != 0)
			{
				// count all monster deaths,
				// even those caused by other monsters
				world.Players[0].KillCount++;
			}

			if (target.Player != null)
			{
				// count environment kills against you
				if (source == null)
				{
					target.Player.Frags[target.Player.Number]++;
				}

				target.Flags &= ~MobjFlags.Solid;
				target.Player.PlayerState = PlayerState.Dead;
				world.PlayerBehavior.DropWeapon(target.Player);

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
				target.SetState(target.Info.XdeathState);
			}
			else
			{
				target.SetState(target.Info.DeathState);
			}

			target.Tics -= world.Random.Next() & 3;
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

			var mo = world.ThingAllocation.SpawnMobj(target.X, target.Y, Mobj.OnFloorZ, item);
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
			if (player != null && world.Options.GameSkill == Skill.Baby)
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
					&& (world.Random.Next() & 1) != 0)
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

			if ((world.Random.Next() < target.Info.PainChance)
				&& (target.Flags & MobjFlags.SkullFly) == 0)
			{
				// fight back!
				target.Flags |= MobjFlags.JustHit;

				target.SetState(target.Info.PainState);
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
				if (target.State == DoomInfo.States[(int)target.Info.SpawnState]
					&& target.Info.SeeState != State.Null)
				{
					target.SetState(target.Info.SeeState);
				}
			}
		}



		//
		// P_ExplodeMissile  
		//
		public void ExplodeMissile(Mobj thing)
		{
			thing.MomX = thing.MomY = thing.MomZ = Fixed.Zero;

			thing.SetState(DoomInfo.MobjInfos[(int)thing.Type].DeathState);

			thing.Tics -= world.Random.Next() & 3;

			if (thing.Tics < 1)
			{
				thing.Tics = 1;
			}

			thing.Flags &= ~MobjFlags.Missile;

			if (thing.Info.DeathSound != 0)
			{
				world.StartSound(thing, thing.Info.DeathSound);
			}
		}




		//
		// RADIUS ATTACK
		//
		private Mobj bombsource;
		private Mobj bombspot;
		private int bombdamage;

		//
		// PIT_RadiusAttack
		// "bombsource" is the creature
		// that caused the explosion at "bombspot".
		//
		private bool PIT_RadiusAttack(Mobj thing)
		{
			if ((thing.Flags & MobjFlags.Shootable) == 0)
			{
				return true;
			}

			// Boss spider and cyborg
			// take no damage from concussion.
			if (thing.Type == MobjType.Cyborg || thing.Type == MobjType.Spider)
			{
				return true;
			}

			var dx = Fixed.Abs(thing.X - bombspot.X);
			var dy = Fixed.Abs(thing.Y - bombspot.Y);

			var dist = dx > dy ? dx : dy;
			dist = new Fixed((dist - thing.Radius).Data >> Fixed.FracBits);

			if (dist < Fixed.Zero)
			{
				dist = Fixed.Zero;
			}

			if (dist.Data >= bombdamage)
			{
				// out of range
				return true;
			}

			if (world.VisibilityCheck.CheckSight(thing, bombspot))
			{
				// must be in direct path
				DamageMobj(thing, bombspot, bombsource, bombdamage - dist.Data);
			}

			return true;
		}

		//
		// P_RadiusAttack
		// Source is the creature that caused the explosion at spot.
		//
		public void RadiusAttack(Mobj spot, Mobj source, int damage)
		{
			var bm = world.Map.BlockMap;
			var dist = Fixed.FromInt(damage + GameConstants.MaxThingRadius.ToIntFloor());
			var yh = (spot.Y + dist - bm.OriginY).Data >> BlockMap.MapBlockShift;
			var yl = (spot.Y - dist - bm.OriginY).Data >> BlockMap.MapBlockShift;
			var xh = (spot.X + dist - bm.OriginX).Data >> BlockMap.MapBlockShift;
			var xl = (spot.X - dist - bm.OriginX).Data >> BlockMap.MapBlockShift;
			bombspot = spot;
			bombsource = source;
			bombdamage = damage;

			for (var y = yl; y <= yh; y++)
			{
				for (var x = xl; x <= xh; x++)
				{
					bm.IterateThings(x, y, mo => PIT_RadiusAttack(mo));
				}
			}
		}


	}
}
