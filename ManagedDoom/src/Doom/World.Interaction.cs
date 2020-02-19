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
			else if (!gameOptions.NetGame && (target.Flags & MobjFlags.CountKill) != 0)
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
				target.Player.playerState = PlayerState.Dead;
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
			/*
			unsigned ang;
			int saved;
			player_t* player;
			fixed_t thrust;
			int temp;

			if (!(target->flags & MF_SHOOTABLE))
				return; // shouldn't happen...

			if (target->health <= 0)
				return;

			if (target->flags & MF_SKULLFLY)
			{
				target->momx = target->momy = target->momz = 0;
			}

			player = target->player;
			if (player && gameskill == sk_baby)
				damage >>= 1;   // take half damage in trainer mode


			// Some close combat weapons should not
			// inflict thrust and push the victim out of reach,
			// thus kick away unless using the chainsaw.
			if (inflictor
			&& !(target->flags & MF_NOCLIP)
			&& (!source
				|| !source->player
				|| source->player->readyweapon != wp_chainsaw))
			{
				ang = R_PointToAngle2(inflictor->x,
							inflictor->y,
							target->x,
							target->y);

				thrust = damage * (FRACUNIT >> 3) * 100 / target->info->mass;

				// make fall forwards sometimes
				if (damage < 40
					 && damage > target->health
					 && target->z - inflictor->z > 64 * FRACUNIT
					 && (P_Random() & 1))
				{
					ang += ANG180;
					thrust *= 4;
				}

				ang >>= ANGLETOFINESHIFT;
				target->momx += FixedMul(thrust, finecosine[ang]);
				target->momy += FixedMul(thrust, finesine[ang]);
			}

			// player specific
			if (player)
			{
				// end of game hell hack
				if (target->subsector->sector->special == 11
					&& damage >= target->health)
				{
					damage = target->health - 1;
				}


				// Below certain threshold,
				// ignore damage in GOD mode, or with INVUL power.
				if (damage < 1000
					 && ((player->cheats & CF_GODMODE)
					  || player->powers[pw_invulnerability]))
				{
					return;
				}

				if (player->armortype)
				{
					if (player->armortype == 1)
						saved = damage / 3;
					else
						saved = damage / 2;

					if (player->armorpoints <= saved)
					{
						// armor is used up
						saved = player->armorpoints;
						player->armortype = 0;
					}
					player->armorpoints -= saved;
					damage -= saved;
				}
				player->health -= damage;   // mirror mobj health here for Dave
				if (player->health < 0)
					player->health = 0;

				player->attacker = source;
				player->damagecount += damage;  // add damage after armor / invuln

				if (player->damagecount > 100)
					player->damagecount = 100;  // teleport stomp does 10k points...

				temp = damage < 100 ? damage : 100;

				if (player == &players[consoleplayer])
					I_Tactile(40, 10, 40 + temp * 2);
			}

			// do the damage	
			target->health -= damage;
			if (target->health <= 0)
			{
				P_KillMobj(source, target);
				return;
			}

			if ((P_Random() < target->info->painchance)
			 && !(target->flags & MF_SKULLFLY))
			{
				target->flags |= MF_JUSTHIT;    // fight back!

				P_SetMobjState(target, target->info->painstate);
			}

			target->reactiontime = 0;       // we're awake now...	

			if ((!target->threshold || target->type == MT_VILE)
			 && source && source != target
			 && source->type != MT_VILE)
			{
				// if not intent on another player,
				// chase after this one
				target->target = source;
				target->threshold = BASETHRESHOLD;
				if (target->state == &states[target->info->spawnstate]
					&& target->info->seestate != S_NULL)
					P_SetMobjState(target, target->info->seestate);
			}
			*/
		}
	}
}
