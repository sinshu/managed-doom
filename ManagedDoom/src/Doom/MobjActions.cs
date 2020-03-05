using System;

namespace ManagedDoom
{
    public static class MobjActions
    {
        public static void BFGSpray(this Mobj mobj)
        {
        }

        public static void Explode(this Mobj mobj)
        {
        }

        public static void Pain(this Mobj mobj)
        {
        }

        public static void PlayerScream(this Mobj mobj)
        {
        }

        public static void Fall(this Mobj actor)
        {
            // actor is on ground, it can be walked over
            actor.Flags &= ~MobjFlags.Solid;

            // So change this if corpse objects
            // are meant to be obstacles.
        }

        public static void XScream(this Mobj mobj)
        {
        }

        //
        // P_LookForPlayers
        // If allaround is false, only look 180 degrees in front.
        // Returns true if a player is targeted.
        //
        private static bool P_LookForPlayers(Mobj actor, bool allaround)
        {
            var world = actor.World;

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

                if (!world.CheckSight(actor, player.Mobj))
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

        public static void Look(this Mobj actor)
        {
            var world = actor.World;

            // any shot will wake up
            actor.Threshold = 0;
            var targ = actor.Subsector.Sector.SoundTarget;

            if (targ != null && (targ.Flags & MobjFlags.Shootable) != 0)
            {
                actor.Target = targ;

                if ((actor.Flags & MobjFlags.Ambush) != 0)
                {
                    if (world.CheckSight(actor, actor.Target))
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

            world.SetMobjState(actor, actor.Info.SeeState);
        }

        public static void Chase(this Mobj mobj)
        {
        }

        public static void FaceTarget(this Mobj mobj)
        {
        }

        public static void PosAttack(this Mobj mobj)
        {
        }

        public static void Scream(this Mobj mobj)
        {
        }

        public static void SPosAttack(this Mobj mobj)
        {
        }

        public static void VileChase(this Mobj mobj)
        {
        }

        public static void VileStart(this Mobj mobj)
        {
        }

        public static void VileTarget(this Mobj mobj)
        {
        }

        public static void VileAttack(this Mobj mobj)
        {
        }

        public static void StartFire(this Mobj mobj)
        {
        }

        public static void Fire(this Mobj mobj)
        {
        }

        public static void FireCrackle(this Mobj mobj)
        {
        }

        public static void Tracer(this Mobj mobj)
        {
        }

        public static void SkelWhoosh(this Mobj mobj)
        {
        }

        public static void SkelFist(this Mobj mobj)
        {
        }

        public static void SkelMissile(this Mobj mobj)
        {
        }

        public static void FatRaise(this Mobj mobj)
        {
        }

        public static void FatAttack1(this Mobj mobj)
        {
        }

        public static void FatAttack2(this Mobj mobj)
        {
        }

        public static void FatAttack3(this Mobj mobj)
        {
        }

        public static void BossDeath(this Mobj mobj)
        {
        }

        public static void CPosAttack(this Mobj mobj)
        {
        }

        public static void CPosRefire(this Mobj mobj)
        {
        }

        public static void TroopAttack(this Mobj mobj)
        {
        }

        public static void SargAttack(this Mobj mobj)
        {
        }

        public static void HeadAttack(this Mobj mobj)
        {
        }

        public static void BruisAttack(this Mobj mobj)
        {
        }

        public static void SkullAttack(this Mobj mobj)
        {
        }

        public static void Metal(this Mobj mobj)
        {
        }

        public static void SpidRefire(this Mobj mobj)
        {
        }

        public static void BabyMetal(this Mobj mobj)
        {
        }

        public static void BspiAttack(this Mobj mobj)
        {
        }

        public static void Hoof(this Mobj mobj)
        {
        }

        public static void CyberAttack(this Mobj mobj)
        {
        }

        public static void PainAttack(this Mobj mobj)
        {
        }

        public static void PainDie(this Mobj mobj)
        {
        }

        public static void KeenDie(this Mobj mobj)
        {
        }

        public static void BrainPain(this Mobj mobj)
        {
        }

        public static void BrainScream(this Mobj mobj)
        {
        }

        public static void BrainDie(this Mobj mobj)
        {
        }

        public static void BrainAwake(this Mobj mobj)
        {
        }

        public static void BrainSpit(this Mobj mobj)
        {
        }

        public static void SpawnSound(this Mobj mobj)
        {
        }

        public static void SpawnFly(this Mobj mobj)
        {
        }

        public static void BrainExplode(this Mobj mobj)
        {
        }
    }
}
