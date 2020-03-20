using System;

namespace ManagedDoom
{
    public sealed class MonsterBehavior
    {
        private World world;

        public MonsterBehavior(World world)
        {
            this.world = world;
        }

        public void Fall(Mobj actor)
        {
            // actor is on ground, it can be walked over
            actor.Flags &= ~MobjFlags.Solid;

            // So change this if corpse objects
            // are meant to be obstacles.
        }


        //
        // P_LookForPlayers
        // If allaround is false, only look 180 degrees in front.
        // Returns true if a player is targeted.
        //
        private bool P_LookForPlayers(Mobj actor, bool allaround)
        {
            var sector = actor.Subsector.Sector;

            var c = 0;
            var stop = (actor.LastLook - 1) & 3;

            for (; ; actor.LastLook = (actor.LastLook + 1) & 3)
            {
                if (!world.Players[actor.LastLook].InGame)
                {
                    continue;
                }

                if (c++ == 2 || actor.LastLook == stop)
                {
                    // done looking
                    return false;
                }

                var player = world.Players[actor.LastLook];

                if (player.Health <= 0)
                {
                    // dead
                    continue;
                }

                if (!world.VisibilityCheck.CheckSight(actor, player.Mobj))
                {
                    // out of sight
                    continue;
                }

                if (!allaround)
                {
                    var an = Geometry.PointToAngle(
                        actor.X,
                        actor.Y,
                        player.Mobj.X,
                        player.Mobj.Y) - actor.Angle;

                    if (an > Angle.Ang90 && an < Angle.Ang270)
                    {
                        var dist = Geometry.AproxDistance(
                            player.Mobj.X - actor.X,
                            player.Mobj.Y - actor.Y);

                        // if real close, react anyway
                        if (dist > World.MELEERANGE)
                        {
                            // behind back
                            continue;
                        }
                    }
                }

                actor.Target = player.Mobj;

                return true;
            }
        }


        public void Look(Mobj actor)
        {
            // any shot will wake up
            actor.Threshold = 0;
            var targ = actor.Subsector.Sector.SoundTarget;

            if (targ != null && (targ.Flags & MobjFlags.Shootable) != 0)
            {
                actor.Target = targ;

                if ((actor.Flags & MobjFlags.Ambush) != 0)
                {
                    if (world.VisibilityCheck.CheckSight(actor, actor.Target))
                    {
                        goto seeyou;
                    }
                }
                else
                {
                    goto seeyou;
                }
            }

            if (!P_LookForPlayers(actor, false))
            {
                return;
            }

        // go into chase state
        seeyou:
            if (actor.Info.SeeSound != 0)
            {
                int sound;

                switch (actor.Info.SeeSound)
                {
                    case Sfx.POSIT1:
                    case Sfx.POSIT2:
                    case Sfx.POSIT3:
                        sound = (int)Sfx.POSIT1 + world.Random.Next() % 3;
                        break;

                    case Sfx.BGSIT1:
                    case Sfx.BGSIT2:
                        sound = (int)Sfx.BGSIT1 + world.Random.Next() % 2;
                        break;

                    default:
                        sound = (int)actor.Info.SeeSound;
                        break;
                }

                if (actor.Type == MobjType.Spider || actor.Type == MobjType.Cyborg)
                {
                    // full volume
                    world.StartSound(null, (Sfx)sound);
                }
                else
                {
                    world.StartSound(actor, (Sfx)sound);
                }
            }

            actor.SetState(actor.Info.SeeState);
        }



        //
        // P_NewChaseDir related LUT.
        //
        private static readonly Direction[] opposite =
        {
            Direction.west, Direction.Southwest, Direction.South, Direction.Southeast,
            Direction.East, Direction.Northeast, Direction.North, Direction.Northwest,
            Direction.None
        };

        private static readonly Direction[] diags =
        {
            Direction.Northwest, Direction.Northeast, Direction.Southwest, Direction.Southeast
        };


        //
        // P_Move
        // Move in the current direction,
        // returns false if the move is blocked.
        //
        private static readonly Fixed[] xspeed =
        {
            new Fixed(Fixed.FracUnit),
            new Fixed(47000),
            new Fixed(0),
            new Fixed(-47000),
            new Fixed(-Fixed.FracUnit),
            new Fixed(-47000),
            new Fixed(0),
            new Fixed(47000)
        };

        private static readonly Fixed[] yspeed =
        {
            new Fixed(0),
            new Fixed(47000),
            new Fixed(Fixed.FracUnit),
            new Fixed(47000),
            new Fixed(0),
            new Fixed(-47000),
            new Fixed(-Fixed.FracUnit),
            new Fixed(-47000)
        };

        private bool P_Move(Mobj actor)
        {
            var tm = world.ThingMovement;

            if (actor.MoveDir == Direction.None)
            {
                return false;
            }

            if ((int)actor.MoveDir >= 8)
            {
                throw new Exception("Weird actor->movedir!");
            }

            var tryx = actor.X + actor.Info.Speed * xspeed[(int)actor.MoveDir];
            var tryy = actor.Y + actor.Info.Speed * yspeed[(int)actor.MoveDir];

            var try_ok = tm.P_TryMove(actor, tryx, tryy);

            if (!try_ok)
            {
                // open any specials
                if ((actor.Flags & MobjFlags.Float) != 0 && tm.floatok)
                {
                    // must adjust height
                    if (actor.Z < tm.tmfloorz)
                    {
                        actor.Z += ThingMovement.FLOATSPEED;
                    }
                    else
                    {
                        actor.Z -= ThingMovement.FLOATSPEED;
                    }

                    actor.Flags |= MobjFlags.InFloat;

                    return true;
                }

                if (tm.numspechit == 0)
                {
                    return false;
                }

                actor.MoveDir = Direction.None;
                var good = false;
                while (tm.numspechit-- > 0)
                {
                    var ld = tm.spechit[tm.numspechit];
                    // if the special is not a door
                    // that can be opened,
                    // return false
                    if (world.MapInteraction.UseSpecialLine(actor, ld, 0))
                    {
                        good = true;
                    }
                }
                return good;
            }
            else
            {
                actor.Flags &= ~MobjFlags.InFloat;
            }


            if ((actor.Flags & MobjFlags.Float) == 0)
            {
                actor.Z = actor.FloorZ;
            }

            return true;
        }

        //
        // TryWalk
        // Attempts to move actor on
        // in its current (ob->moveangle) direction.
        // If blocked by either a wall or an actor
        // returns FALSE
        // If move is either clear or blocked only by a door,
        // returns TRUE and sets...
        // If a door is in the way,
        // an OpenDoor call is made to start it opening.
        //
        private bool P_TryWalk(Mobj actor)
        {
            if (!P_Move(actor))
            {
                return false;
            }

            actor.MoveCount = world.Random.Next() & 15;

            return true;
        }

        private readonly Direction[] d = new Direction[3];


        private void P_NewChaseDir(Mobj actor)
        {
            if (actor.Target == null)
            {
                throw new Exception("P_NewChaseDir: called with no target");
            }

            var olddir = actor.MoveDir;
            var turnaround = opposite[(int)olddir];

            var deltax = actor.Target.X - actor.X;
            var deltay = actor.Target.Y - actor.Y;

            if (deltax > Fixed.FromInt(10))
            {
                d[1] = Direction.East;
            }
            else if (deltax < Fixed.FromInt(-10))
            {
                d[1] = Direction.west;
            }
            else
            {
                d[1] = Direction.None;
            }

            if (deltay < Fixed.FromInt(-10))
            {
                d[2] = Direction.South;
            }
            else if (deltay > Fixed.FromInt(10))
            {
                d[2] = Direction.North;
            }
            else
            {
                d[2] = Direction.None;
            }

            // try direct route
            if (d[1] != Direction.None && d[2] != Direction.None)
            {
                var a = (deltay < Fixed.Zero) ? 1 : 0;
                var b = (deltax > Fixed.Zero) ? 1 : 0;
                actor.MoveDir = diags[(a << 1) + b];
                if (actor.MoveDir != turnaround && P_TryWalk(actor))
                {
                    return;
                }
            }

            // try other directions
            if (world.Random.Next() > 200
                || Fixed.Abs(deltay) > Fixed.Abs(deltax))
            {
                var tdir = d[1];
                d[1] = d[2];
                d[2] = tdir;
            }

            if (d[1] == turnaround)
            {
                d[1] = Direction.None;
            }

            if (d[2] == turnaround)
            {
                d[2] = Direction.None;
            }

            if (d[1] != Direction.None)
            {
                actor.MoveDir = d[1];
                if (P_TryWalk(actor))
                {
                    // either moved forward or attacked
                    return;
                }
            }

            if (d[2] != Direction.None)
            {
                actor.MoveDir = d[2];

                if (P_TryWalk(actor))
                {
                    return;
                }
            }

            // there is no direct path to the player,
            // so pick another direction.
            if (olddir != Direction.None)
            {
                actor.MoveDir = olddir;

                if (P_TryWalk(actor))
                {
                    return;
                }
            }

            // randomly determine direction of search
            if ((world.Random.Next() & 1) != 0)
            {
                for (var tdir = (int)Direction.East; tdir <= (int)Direction.Southeast; tdir++)
                {
                    if ((Direction)tdir != turnaround)
                    {
                        actor.MoveDir = (Direction)tdir;

                        if (P_TryWalk(actor))
                        {
                            return;
                        }
                    }
                }
            }
            else
            {
                for (var tdir = (int)Direction.Southeast; tdir != ((int)Direction.East - 1); tdir--)
                {
                    if ((Direction)tdir != turnaround)
                    {
                        actor.MoveDir = (Direction)tdir;

                        if (P_TryWalk(actor))
                        {
                            return;
                        }
                    }
                }
            }

            if (turnaround != Direction.None)
            {
                actor.MoveDir = turnaround;
                if (P_TryWalk(actor))
                {
                    return;
                }
            }

            // can not move
            actor.MoveDir = Direction.None;
        }


        //
        // P_CheckMeleeRange
        //
        private bool P_CheckMeleeRange(Mobj actor)
        {
            if (actor.Target == null)
            {
                return false;
            }

            var pl = actor.Target;
            var dist = Geometry.AproxDistance(pl.X - actor.X, pl.Y - actor.Y);

            if (dist >= World.MELEERANGE - Fixed.FromInt(20) + pl.Info.Radius)
            {
                return false;
            }

            if (!world.VisibilityCheck.CheckSight(actor, actor.Target))
            {
                return false;
            }

            return true;
        }


        //
        // P_CheckMissileRange
        //
        private bool P_CheckMissileRange(Mobj actor)
        {
            if (!world.VisibilityCheck.CheckSight(actor, actor.Target))
            {
                return false;
            }

            if ((actor.Flags & MobjFlags.JustHit) != 0)
            {
                // the target just hit the enemy,
                // so fight back!
                actor.Flags &= ~MobjFlags.JustHit;
                return true;
            }

            if (actor.ReactionTime > 0)
            {
                // do not attack yet
                return false;
            }

            // OPTIMIZE: get this from a global checksight
            var dist = Geometry.AproxDistance(
                actor.X - actor.Target.X,
                actor.Y - actor.Target.Y) - Fixed.FromInt(64);

            if (actor.Info.MeleeState == 0)
            {
                // no melee attack, so fire more
                dist -= Fixed.FromInt(128);
            }

            dist = new Fixed(dist.Data >> 16);

            if (actor.Type == MobjType.Vile)
            {
                if (dist.Data > 14 * 64)
                {
                    // too far away
                    return false;
                }
            }

            if (actor.Type == MobjType.Undead)
            {
                if (dist.Data < 196)
                {
                    // close for fist attack
                    return false;
                }

                dist = new Fixed(dist.Data >> 1);
            }


            if (actor.Type == MobjType.Cyborg
                || actor.Type == MobjType.Spider
                || actor.Type == MobjType.Skull)
            {
                dist = new Fixed(dist.Data >> 1);
            }

            if (dist.Data > 200)
            {
                dist = new Fixed(200);
            }

            if (actor.Type == MobjType.Cyborg && dist.Data > 160)
            {
                dist = new Fixed(160);
            }

            if (world.Random.Next() < dist.Data)
            {
                return false;
            }

            return true;
        }


        public void Chase(Mobj actor)
        {
            if (actor.ReactionTime > 0)
            {
                actor.ReactionTime--;
            }

            // modify target threshold
            if (actor.Threshold > 0)
            {
                if (actor.Target == null
                    || actor.Target.Health <= 0)
                {
                    actor.Threshold = 0;
                }
                else
                {
                    actor.Threshold--;
                }
            }

            // turn towards movement direction if not there yet
            if ((int)actor.MoveDir < 8)
            {
                actor.Angle = new Angle((int)actor.Angle.Data & (7 << 29));
                var delta = (int)(actor.Angle - new Angle((int)actor.MoveDir << 29)).Data;

                if (delta > 0)
                {
                    actor.Angle -= new Angle(Angle.Ang90.Data / 2);
                }
                else if (delta < 0)
                {
                    actor.Angle += new Angle(Angle.Ang90.Data / 2);
                }
            }

            if (actor.Target == null
                || (actor.Target.Flags & MobjFlags.Shootable) == 0)
            {
                // look for a new target
                if (P_LookForPlayers(actor, true))
                {
                    // got a new target
                    return;
                }

                actor.SetState(actor.Info.SpawnState);

                return;
            }

            // do not attack twice in a row
            if ((actor.Flags & MobjFlags.JustAttacked) != 0)
            {
                actor.Flags &= ~MobjFlags.JustAttacked;
                if (world.Options.GameSkill != Skill.Nightmare
                    && !world.Options.FastMonsters)
                {
                    P_NewChaseDir(actor);
                }

                return;
            }

            // check for melee attack
            if (actor.Info.MeleeState != 0
                && P_CheckMeleeRange(actor))
            {
                if (actor.Info.AttackSound != 0)
                {
                    world.StartSound(actor, actor.Info.AttackSound);
                }

                actor.SetState(actor.Info.MeleeState);

                return;
            }

            // check for missile attack
            if (actor.Info.MissileState != 0)
            {
                if (world.Options.GameSkill < Skill.Nightmare
                    && !world.Options.FastMonsters && actor.MoveCount != 0)
                {
                    goto nomissile;
                }

                if (!P_CheckMissileRange(actor))
                {
                    goto nomissile;
                }

                actor.SetState(actor.Info.MissileState);
                actor.Flags |= MobjFlags.JustAttacked;

                return;
            }

        // ?
        nomissile:
            // possibly choose another target
            if (world.Options.NetGame
                && actor.Threshold == 0
                && !world.VisibilityCheck.CheckSight(actor, actor.Target))
            {
                if (P_LookForPlayers(actor, true))
                    return; // got a new target
            }

            // chase towards player
            if (--actor.MoveCount < 0 || !P_Move(actor))
            {
                P_NewChaseDir(actor);
            }

            // make active sound
            if (actor.Info.ActiveSound != 0 && world.Random.Next() < 3)
            {
                world.StartSound(actor, actor.Info.ActiveSound);
            }
        }



        public void FaceTarget(Mobj actor)
        {
            if (actor.Target == null)
            {
                return;
            }

            actor.Flags &= ~MobjFlags.Ambush;

            actor.Angle = Geometry.PointToAngle(
                actor.X,
                actor.Y,
                actor.Target.X,
                actor.Target.Y);

            if ((actor.Target.Flags & MobjFlags.Shadow) != 0)
            {
                actor.Angle += new Angle((world.Random.Next() - world.Random.Next()) << 21);
            }
        }


        public void PosAttack(Mobj actor)
        {
            if (actor.Target == null)
            {
                return;
            }

            FaceTarget(actor);
            var angle = actor.Angle;
            var slope = world.Hitscan.AimLineAttack(actor, angle, World.MISSILERANGE);

            world.StartSound(actor, Sfx.PISTOL);
            angle += new Angle((world.Random.Next() - world.Random.Next()) << 20);
            var damage = ((world.Random.Next() % 5) + 1) * 3;
            world.Hitscan.LineAttack(actor, angle, World.MISSILERANGE, slope, damage);
        }


        public void Scream(Mobj actor)
        {
            int sound;

            switch (actor.Info.DeathSound)
            {
                case 0:
                    return;

                case Sfx.PODTH1:
                case Sfx.PODTH2:
                case Sfx.PODTH3:
                    sound = (int)Sfx.PODTH1 + world.Random.Next() % 3;
                    break;

                case Sfx.BGDTH1:
                case Sfx.BGDTH2:
                    sound = (int)Sfx.BGDTH1 + world.Random.Next() % 2;
                    break;

                default:
                    sound = (int)actor.Info.DeathSound;
                    break;
            }

            // Check for bosses.
            if (actor.Type == MobjType.Spider || actor.Type == MobjType.Cyborg)
            {
                // full volume
                world.StartSound(null, (Sfx)sound);
            }
            else
            {
                world.StartSound(actor, (Sfx)sound);
            }
        }


        public void TroopAttack(Mobj actor)
        {
            if (actor.Target == null)
            {
                return;
            }

            FaceTarget(actor);
            if (P_CheckMeleeRange(actor))
            {
                world.StartSound(actor, Sfx.CLAW);
                var damage = (world.Random.Next() % 8 + 1) * 3;
                world.ThingInteraction.DamageMobj(actor.Target, actor, actor, damage);
                return;
            }

            // launch a missile
            world.ThingAllocation.SpawnMissile(actor, actor.Target, MobjType.Troopshot);
        }


    }
}
