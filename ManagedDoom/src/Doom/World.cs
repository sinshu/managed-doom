using System;
using System.Linq;

namespace ManagedDoom
{
    public sealed partial class World
    {
        private GameOptions gameOptions;

        private Map map;
        private DoomRandom random;

        private int validCount;

        private int totalKills = 0;
        private int totalItems = 0;

        private Mobj player;
        private Fixed playerX;
        private Fixed playerY;
        private Fixed playerZ;
        private Angle playerViewAngle;

        private Func<Intercept, bool> interceptTest;

        public World(Resources resorces, string mapName, GameOptions options)
        {
            gameOptions = options;

            map = new Map(resorces, mapName);

            random = new DoomRandom();

            validCount = 0;

            InitThinker();
            InitThingMovement();
            InitPathTraversal();

            LoadThings();

            /*
            var curr = thinkerCap.Next;
            while (curr != thinkerCap)
            {
                var mobj = curr as Mobj;
                if (mobj != null)
                {
                    mobj.MomX = Fixed.FromInt(2);
                    mobj.MomY = Fixed.FromInt(8);
                    mobj.MomZ = Fixed.FromInt(8);
                }
                curr = curr.Next;
            }
            */

            var playerThing = map.Things.First(t => (int)t.Type == 1);
            playerX = playerThing.X;
            playerY = playerThing.Y;
            playerZ = Geometry.PointInSubsector(playerX, playerY, map).Sector.FloorHeight;
            playerViewAngle = Angle.FromDegree(playerThing.Angle);

            interceptTest = InterceptTest;
        }

        public void Update(bool up, bool down, bool left, bool right)
        {
            RunThinkers();

            var speed = 8.0;

            if (left)
            {
                playerViewAngle += Angle.FromDegree(speed / 2);
            }

            if (right)
            {
                playerViewAngle -= Angle.FromDegree(speed / 2);
            }

            for (var deg = 0; deg < 90; deg++)
            {
                {
                    var dx = Fixed.Zero;
                    var dy = Fixed.Zero;

                    var direction = playerViewAngle + Angle.FromDegree(deg);

                    if (up)
                    {
                        dx += Fixed.FromDouble(speed) * Trig.Cos(direction);
                        dy += Fixed.FromDouble(speed) * Trig.Sin(direction);
                    }

                    if (CheckPosition(player, playerX + dx, playerY + dy))
                    {
                        playerX += dx;
                        playerY += dy;
                        playerZ = tmfloorz;
                        break;
                    }
                }

                {
                    var dx = Fixed.Zero;
                    var dy = Fixed.Zero;

                    var direction = playerViewAngle - Angle.FromDegree(deg);

                    if (up)
                    {
                        dx += Fixed.FromDouble(speed) * Trig.Cos(direction);
                        dy += Fixed.FromDouble(speed) * Trig.Sin(direction);
                    }

                    if (CheckPosition(player, playerX + dx, playerY + dy))
                    {
                        playerX += dx;
                        playerY += dy;
                        playerZ = tmfloorz;
                        break;
                    }
                }
            }

            for (var deg = 0; deg < 90; deg++)
            {
                {
                    var dx = Fixed.Zero;
                    var dy = Fixed.Zero;

                    var direction = playerViewAngle + Angle.FromDegree(deg) + Angle.Ang180;

                    if (down)
                    {
                        dx += Fixed.FromDouble(speed) * Trig.Cos(direction);
                        dy += Fixed.FromDouble(speed) * Trig.Sin(direction);
                    }

                    if (CheckPosition(player, playerX + dx, playerY + dy))
                    {
                        playerX += dx;
                        playerY += dy;
                        playerZ = tmfloorz;
                        break;
                    }
                }

                {
                    var dx = Fixed.Zero;
                    var dy = Fixed.Zero;

                    var direction = playerViewAngle - Angle.FromDegree(deg) + Angle.Ang180;

                    if (down)
                    {
                        dx += Fixed.FromDouble(speed) * Trig.Cos(direction);
                        dy += Fixed.FromDouble(speed) * Trig.Sin(direction);
                    }

                    if (CheckPosition(player, playerX + dx, playerY + dy))
                    {
                        playerX += dx;
                        playerY += dy;
                        playerZ = tmfloorz;
                        break;
                    }
                }
            }

            var distance = Fixed.FromInt(1024);
            var x1 = playerX;
            var y1 = playerY;
            var x2 = x1 + distance * Trig.Cos(playerViewAngle);
            var y2 = y1 + distance * Trig.Sin(playerViewAngle);
            var flags = PathTraverseFlags.AddLines | PathTraverseFlags.AddThings;
            //PathTraverse(x1, y1, x2, y2, flags, interceptTest);
        }

        private bool InterceptTest(Intercept ic)
        {
            if (ic.Line != null)
            {
                ic.Line.Side0.RowOffset += Fixed.One;
            }

            if (ic.Thing != null)
            {
                SetMobjState(ic.Thing, ic.Thing.Info.PainState);
            }

            return true;
        }

        private void LoadThings()
        {
            totalKills = 0;
            totalItems = 0;

            for (var i = 0; i < map.Things.Length; i++)
            {
                var mt = map.Things[i];

                var spawn = true;

                // Do not spawn cool, new monsters if !commercial
                if (gameOptions.GameMode != GameMode.Commercial)
                {
                    switch (mt.Type)
                    {
                        case 68:    // Arachnotron
                        case 64:    // Archvile
                        case 88:    // Boss Brain
                        case 89:    // Boss Shooter
                        case 69:    // Hell Knight
                        case 67:    // Mancubus
                        case 71:    // Pain Elemental
                        case 65:    // Former Human Commando
                        case 66:    // Revenant
                        case 84:    // Wolf SS
                            spawn = false;
                            break;
                    }
                }

                if (spawn == false)
                {
                    break;
                }

                SpawnMapThing(mt);
            }
        }

        private void SpawnMapThing(Thing mthing)
        {
            /*
            // count deathmatch start positions
            if (mthing->type == 11)
            {
                if (deathmatch_p < &deathmatchstarts[10])
                {
                    memcpy(deathmatch_p, mthing, sizeof(*mthing));
                    deathmatch_p++;
                }
                return;
            }
            */

            /*
            // check for players specially
            if (mthing->type <= 4)
            {
                // save spots for respawning in network games
                playerstarts[mthing->type - 1] = *mthing;
                if (!deathmatch)
                    P_SpawnPlayer(mthing);

                return;
            }
            */

            // TEST
            if (mthing.Type == 1)
            {
                player = SpawnMobj(mthing.X, mthing.Y, Mobj.OnFloorZ, MobjType.Player);
                player.Player = new Player();
                player.Player.Mobj = player;
            }

            // The code below must be removed later
            // when the player related code above is correctly implemented.
            if (mthing.Type == 11 || mthing.Type <= 4)
            {
                return;
            }

            // check for apropriate skill level
            if (!gameOptions.NetGame && ((int)mthing.Flags & 16) != 0)
            {
                return;
            }

            int bit;
            if (gameOptions.GameSkill == Skill.Baby)
            {
                bit = 1;
            }
            else if (gameOptions.GameSkill == Skill.Nightmare)
            {
                bit = 4;
            }
            else
            {
                bit = 1 << ((int)gameOptions.GameSkill - 1);
            }

            if (((int)mthing.Flags & bit) == 0)
            {
                return;
            }

            int i;
            // find which type to spawn
            for (i = 0; i < Info.MobjInfos.Length; i++)
            {
                if (mthing.Type == Info.MobjInfos[i].DoomEdNum)
                {
                    break;
                }
            }

            if (i == Info.MobjInfos.Length)
            {
                throw new Exception("P_SpawnMapThing: Unknown type!");
            }

            // don't spawn keycards and players in deathmatch
            if (gameOptions.Deathmatch
                && (Info.MobjInfos[i].Flags & MobjFlags.NotDeathmatch) != 0)
            {
                return;
            }

            // don't spawn any monsters if -nomonsters
            if (gameOptions.NoMonsters
                && (i == (int)MobjType.Skull
                    || (Info.MobjInfos[i].Flags & MobjFlags.CountKill) != 0))
            {
                return;
            }

            // spawn it
            Fixed x = mthing.X;
            Fixed y = mthing.Y;
            Fixed z;
            if ((Info.MobjInfos[i].Flags & MobjFlags.SpawnCeiling) != 0)
            {
                z = Mobj.OnCeilingZ;
            }
            else
            {
                z = Mobj.OnFloorZ;
            }

            var mobj = SpawnMobj(x, y, z, (MobjType)i);

            mobj.SpawnPoint = mthing;

            if (mobj.Tics > 0)
            {
                mobj.Tics = 1 + (random.Next() % mobj.Tics);
            }

            if ((mobj.Flags & MobjFlags.CountKill) != 0)
            {
                totalKills++;
            }

            if ((mobj.Flags & MobjFlags.CountItem) != 0)
            {
                totalItems++;
            }

            // mobj->angle = ANG45 * (mthing->angle/45);
            mobj.Angle = new Angle(Angle.Ang45.Data * (uint)(mthing.Angle / 45));

            if ((mthing.Flags & ThingFlags.Ambush) != 0)
            {
                mobj.Flags |= MobjFlags.Ambush;
            }
        }

        public Mobj SpawnMobj(Fixed x, Fixed y, Fixed z, MobjType type)
        {
            var mobj = thinkerPool.RentMobj();

            var info = Info.MobjInfos[(int)type];

            mobj.Type = type;
            mobj.Info = info;
            mobj.X = x;
            mobj.Y = y;
            mobj.Radius = info.Radius;
            mobj.Height = info.Height;
            mobj.Flags = info.Flags;
            mobj.Health = info.SpawnHealth;

            if (gameOptions.GameSkill != Skill.Nightmare)
            {
                mobj.ReactionTime = info.ReactionTime;
            }

            mobj.LastLook = random.Next() % Player.MaxPlayerCount;

            // do not set the state with P_SetMobjState,
            // because action routines can not be called yet
            var st = Info.States[(int)info.SpawnState];

            mobj.State = st;
            mobj.Tics = st.Tics;
            mobj.Sprite = st.Sprite;
            mobj.Frame = st.Frame;

            // set subsector and/or block links
            SetThingPosition(mobj);

            mobj.FloorZ = mobj.Subsector.Sector.FloorHeight;
            mobj.CeilingZ = mobj.Subsector.Sector.CeilingHeight;

            if (z == Mobj.OnFloorZ)
            {
                mobj.Z = mobj.FloorZ;
            }
            else if (z == Mobj.OnCeilingZ)
            {
                mobj.Z = mobj.CeilingZ - mobj.Info.Height;
            }
            else
            {
                mobj.Z = z;
            }

            //mobj->thinker.function.acp1 = (actionf_p1)P_MobjThinker;

            AddThinker(mobj);

            return mobj;
        }

        public void RemoveMobj(Mobj mobj)
        {
            if ((mobj.Flags & MobjFlags.Special) != 0
                && (mobj.Flags & MobjFlags.Dropped) == 0
                && (mobj.Type != MobjType.Inv)
                && (mobj.Type != MobjType.Ins))
            {
                //itemrespawnque[iquehead] = mobj->spawnpoint;
                //itemrespawntime[iquehead] = leveltime;
                //iquehead = (iquehead + 1) & (ITEMQUESIZE - 1);

                // lose one off the end?
                //if (iquehead == iquetail)
                //    iquetail = (iquetail + 1) & (ITEMQUESIZE - 1);
            }

            // unlink from sector and block lists
            UnsetThingPosition(mobj);

            // stop any playing sound
            //S_StopSound(mobj);

            // free block
            RemoveThinker(mobj);
        }

        public bool SetMobjState(Mobj mobj, State state)
        {
            StateDef st;

            do
            {
                if (state == State.Null)
                {
                    mobj.State = Info.States[(int)State.Null];
                    RemoveMobj(mobj);
                    return false;
                }

                st = Info.States[(int)state];
                mobj.State = st;
                mobj.Tics = st.Tics;
                mobj.Sprite = st.Sprite;
                mobj.Frame = st.Frame;

                // Modified handling.
                // Call action functions when the state is set
                if (st.MobjAction != null)
                {
                    st.MobjAction(mobj);
                }

                state = st.Next;
            }
            while (mobj.Tics == 0);

            return true;
        }

        public void StartSound(Mobj mobj, Sfx sfx)
        {
            Console.WriteLine("StartSound: " + sfx);
        }

        public Map Map => map;
        public DoomRandom Random => random;

        public Fixed ViewX => playerX;
        public Fixed ViewY => playerY;
        public Fixed ViewZ => playerZ + Fixed.FromInt(41);
        public Angle ViewAngle => playerViewAngle;
    }
}
