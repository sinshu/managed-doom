using System;
using System.Linq;

namespace ManagedDoom
{
    public sealed partial class World
    {
        public GameOptions Options;
        public Player[] Players;

        private Map map;
        private DoomRandom random;

        private int consoleplayer = 0;

        private int validCount;

        public int totalKills = 0;
        public int totalItems = 0;

        public int levelTime = 0;


        private Thinkers thinkers;
        private ThingAllocation thingAllocation;
        private ThingMovement thingMovement;
        private MapCollision mapCollision;
        private PathTraversal pathTraversal;
        private Hitscan hitscan;
        private VisibilityCheck visibilityCheck;

        public World(CommonResource resorces, GameOptions options, Player[] players)
        {
            Options = options;
            Players = players;

            map = new Map(resorces, options);

            random = new DoomRandom();

            validCount = 0;

            thinkers = new Thinkers(this);
            thingAllocation = new ThingAllocation(this);
            thingMovement = new ThingMovement(this);
            mapCollision = new MapCollision(this);
            pathTraversal = new PathTraversal(this);
            hitscan = new Hitscan(this);
            visibilityCheck = new VisibilityCheck(this);



            LoadThings();
        }

        public void Update()
        {
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (Players[i].InGame)
                {
                    PlayerThink(Players[i]);
                }
            }

            thinkers.Run();

            levelTime++;
        }

        private void LoadThings()
        {
            var ta = ThingAllocation;

            totalKills = 0;
            totalItems = 0;

            for (var i = 0; i < map.Things.Length; i++)
            {
                var mt = map.Things[i];

                var spawn = true;

                // Do not spawn cool, new monsters if !commercial
                if (Options.GameMode != GameMode.Commercial)
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

                ta.SpawnMapThing(mt);
            }
        }

        



        

        public bool SetMobjState(Mobj mobj, State state)
        {
            var ta = ThingAllocation;

            StateDef st;

            do
            {
                if (state == State.Null)
                {
                    mobj.State = Info.States[(int)State.Null];
                    ta.RemoveMobj(mobj);
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

        public void StopSound(Mobj mobj)
        {
        }

        public int GetNewValidCount()
        {
            validCount++;
            return validCount;
        }

        public int GetMobjHash()
        {
            var hash = 0;
            foreach (var thinker in thinkers)
            {
                var mobj = thinker as Mobj;
                if (mobj != null)
                {
                    hash = DoomDebug.CombineHash(hash, mobj.GetHashCode());
                }
            }
            return hash;
        }

        public Map Map => map;
        public DoomRandom Random => random;

        public Thinkers Thinkers => thinkers;
        public ThingAllocation ThingAllocation => thingAllocation;
        public ThingMovement ThingMovement => thingMovement;
        public MapCollision MapCollision => mapCollision;
        public PathTraversal PathTraversal => pathTraversal;
        public Hitscan Hitscan => hitscan;
        public VisibilityCheck VisibilityCheck => visibilityCheck;


        public static readonly Fixed USERANGE = Fixed.FromInt(64);
        public static readonly Fixed MELEERANGE = Fixed.FromInt(64);
        public static readonly Fixed MISSILERANGE = Fixed.FromInt(32 * 64);
    }
}
