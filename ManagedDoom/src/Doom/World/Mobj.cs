using System;

namespace ManagedDoom
{
    public class Mobj : Thinker
    {
        public static readonly Fixed OnFloorZ = Fixed.MinValue;
        public static readonly Fixed OnCeilingZ = Fixed.MaxValue;

        private World world;

        // Info for drawing: position.
        public Fixed X;
        public Fixed Y;
        public Fixed Z;

        // More list: links in sector (if needed)
        public Mobj SNext;
        public Mobj SPrev;

        //More drawing info: to determine current sprite.
        public Angle Angle; // orientation
        public Sprite Sprite; // used to find patch_t and flip value
        public int Frame; // might be ORed with FF_FULLBRIGHT

        // Interaction info, by BLOCKMAP.
        // Links in blocks (if needed).
        public Mobj BNext;
        public Mobj BPrev;

        public Subsector Subsector;

        // The closest interval over all contacted Sectors.
        public Fixed FloorZ;
        public Fixed CeilingZ;

        // For movement checking.
        public Fixed Radius;
        public Fixed Height;

        // Momentums, used to update position.
        public Fixed MomX;
        public Fixed MomY;
        public Fixed MomZ;

        // If == validcount, already checked.
        public int ValidCount;

        public MobjType Type;
        public MobjInfo Info; // &mobjinfo[mobj->type]

        public int Tics; // state tic counter
        public MobjStateDef State;
        public MobjFlags Flags;
        public int Health;

        // Movement direction, movement generation (zig-zagging).
        public Direction MoveDir; // 0-7
        public int MoveCount; // when 0, select a new dir

        // Thing being chased/attacked (or NULL),
        // also the originator for missiles.
        public Mobj Target;

        // Reaction time: if non 0, don't attack yet.
        // Used by player to freeze a bit after teleporting.
        public int ReactionTime;

        // If >0, the target will be chased
        // no matter what (even if shot)
        public int Threshold;

        // Additional info record for player avatars only.
        // Only valid if type == MT_PLAYER
        public Player Player;

        // Player number last looked for.
        public int LastLook;

        // For nightmare respawn.
        public MapThing SpawnPoint;

        // Thing being chased/attacked for tracers.
        public Mobj Tracer;



        public Mobj(World world)
        {
            this.world = world;
        }


        public override void Run()
        {
            // momentum movement
            if (MomX != Fixed.Zero
                || MomY != Fixed.Zero
                || (Flags & MobjFlags.SkullFly) != 0)
            {
                world.ThingMovement.XYMovement(this);

                // FIXME: decent NOP/NULL/Nil function pointer please.
                if (ThinkerState == ThinkerState.Removed)
                {
                    // mobj was removed
                    return;
                }
            }

            if ((Z != FloorZ) || MomZ != Fixed.Zero)
            {
                world.ThingMovement.ZMovement(this);

                // FIXME: decent NOP/NULL/Nil function pointer please.
                if (ThinkerState == ThinkerState.Removed)
                {
                    // mobj was removed
                    return;
                }
            }


            // cycle through states,
            // calling action functions at transitions
            if (Tics != -1)
            {
                Tics--;

                // you can cycle through multiple states in a tic
                if (Tics == 0)
                {
                    if (!SetState(State.Next))
                    {
                        // freed itself
                        return;
                    }
                }
            }
            else
            {
                // check for nightmare respawn
                if ((Flags & MobjFlags.CountKill) == 0)
                {
                    return;
                }

                var options = world.Options;
                if (!(options.Skill == GameSkill.Nightmare || options.RespawnMonsters))
                {
                    return;
                }

                MoveCount++;

                if (MoveCount < 12 * 35)
                {
                    return;
                }

                if ((world.levelTime & 31) != 0)
                {
                    return;
                }

                if (world.Random.Next() > 4)
                {
                    return;
                }

                NightmareRespawn();
            }
        }

        public bool SetState(MobjState state)
        {
            var ta = world.ThingAllocation;

            MobjStateDef st;

            do
            {
                if (state == MobjState.Null)
                {
                    State = DoomInfo.States[(int)MobjState.Null];
                    ta.RemoveMobj(this);
                    return false;
                }

                st = DoomInfo.States[(int)state];
                State = st;
                Tics = GetTics(st);
                Sprite = st.Sprite;
                Frame = st.Frame;

                // Modified handling.
                // Call action functions when the state is set
                if (st.MobjAction != null)
                {
                    st.MobjAction(world, this);
                }

                state = st.Next;
            }
            while (Tics == 0);

            return true;
        }

        private int GetTics(MobjStateDef state)
        {
            var options = world.Options;
            if (options.FastMonsters || options.Skill == GameSkill.Nightmare)
            {
                if ((int)MobjState.SargRun1 <= state.Number
                    && state.Number <= (int)MobjState.SargPain2)
                {
                    return state.Tics >> 1;
                }
                else
                {
                    return state.Tics;
                }
            }
            else
            {
                return state.Tics;
            }
        }

        //
        // P_NightmareRespawn
        //
        private void NightmareRespawn()
        {
            MapThing sp;
            if (SpawnPoint != null)
            {
                sp = SpawnPoint;
            }
            else
            {
                sp = MapThing.Empty;
            }

            var x = sp.X;
            var y = sp.Y;

            // somthing is occupying it's position?
            if (!world.ThingMovement.CheckPosition(this, x, y))
            {
                // no respwan
                return;
            }

            // spawn a teleport fog at old spot
            // because of removal of the body?
            var mo = world.ThingAllocation.SpawnMobj(
                X, Y, Subsector.Sector.FloorHeight, MobjType.Tfog);

            // initiate teleport sound
            world.StartSound(mo, Sfx.TELEPT);

            // spawn a teleport fog at the new spot
            var ss = Geometry.PointInSubsector(x, y, world.Map);

            mo = world.ThingAllocation.SpawnMobj(
                x, y, ss.Sector.FloorHeight, MobjType.Tfog);

            world.StartSound(mo, Sfx.TELEPT);

            // spawn the new monster
            var mthing = sp;

            // spawn it
            Fixed z;
            if ((Info.Flags & MobjFlags.SpawnCeiling) != 0)
            {
                z = OnCeilingZ;
            }
            else
            {
                z = OnFloorZ;
            }

            // inherit attributes from deceased one
            mo = world.ThingAllocation.SpawnMobj(x, y, z, Type);
            mo.SpawnPoint = SpawnPoint;
            mo.Angle = mthing.Angle;

            if ((mthing.Flags & ThingFlags.Ambush) != 0)
            {
                mo.Flags |= MobjFlags.Ambush;
            }

            mo.ReactionTime = 18;

            // remove the old monster,
            world.ThingAllocation.RemoveMobj(this);
        }

        public World World => world;
    }
}
