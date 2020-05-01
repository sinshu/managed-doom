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

        public int consoleplayer = 0;

        private int validCount;

        public int totalKills = 0;
        public int totalItems = 0;
        public int totalSecrets = 0;

        public int levelTime = 0;

        private Thinkers thinkers;
        private Specials specials;

        private Thing[] playerStarts;

        private ThingAllocation thingAllocation;
        private ThingMovement thingMovement;
        private ThingInteraction thingInteraction;
        private MapCollision mapCollision;
        private MapInteraction mapInteraction;
        private PathTraversal pathTraversal;
        private Hitscan hitscan;
        private VisibilityCheck visibilityCheck;
        private SectorAction sectorAction;
        private PlayerBehavior playerBehavior;
        private ItemPickup itemPickup;
        private WeaponBehavior weaponBehavior;
        private MonsterBehavior monsterBehavior;
        private LightingChange lightingChange;

        private GameAction gameAction;

        public World(CommonResource resorces, GameOptions options, Player[] players)
        {
            Options = options;
            Players = players;

            map = new Map(resorces, options);

            random = options.Random;

            validCount = 0;

            thinkers = new Thinkers(this);
            specials = new Specials(this);

            playerStarts = new Thing[Player.MaxPlayerCount];

            thingAllocation = new ThingAllocation(this);
            thingMovement = new ThingMovement(this);
            thingInteraction = new ThingInteraction(this);
            mapCollision = new MapCollision(this);
            mapInteraction = new MapInteraction(this);
            pathTraversal = new PathTraversal(this);
            hitscan = new Hitscan(this);
            visibilityCheck = new VisibilityCheck(this);
            sectorAction = new SectorAction(this);
            playerBehavior = new PlayerBehavior(this);
            itemPickup = new ItemPickup(this);
            weaponBehavior = new WeaponBehavior(this);
            monsterBehavior = new MonsterBehavior(this);
            lightingChange = new LightingChange(this);

            totalKills = 0;
            totalItems = 0;
            totalSecrets = 0;
            options.wminfo.maxFrags = 0;
            options.wminfo.ParTime = 180;
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                players[i].KillCount = 0;
                players[i].SecretCount = 0;
                players[i].ItemCount = 0;
            }

            // Initial height of PointOfView
            // will be set by player think.
            players[consoleplayer].ViewZ = new Fixed(1);

            LoadThings();

            SpawnSpecials();
        }

        public GameAction Update()
        {
            gameAction = GameAction.Nothing;

            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (Players[i].InGame)
                {
                    playerBehavior.PlayerThink(Players[i]);
                }
            }

            thinkers.Run();
            specials.Update();

            levelTime++;

            var mobjHash = DoomDebug.GetMobjHash(this);
            var sectorHash = DoomDebug.GetSectorHash(this);
            //Console.WriteLine(levelTime + ": " + mobjHash.ToString("x8") + ", " + sectorHash.ToString("x8"));

            return gameAction;
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


        private void SpawnSpecials()
        {
            /*
            episode = 1;
            if (W_CheckNumForName("texture2") >= 0)
                episode = 2;


            // See if -TIMER needs to be used.
            levelTimer = false;

            i = M_CheckParm("-avg");
            if (i && deathmatch)
            {
                levelTimer = true;
                levelTimeCount = 20 * 60 * 35;
            }

            i = M_CheckParm("-timer");
            if (i && deathmatch)
            {
                int time;
                time = atoi(myargv[i + 1]) * 60 * 35;
                levelTimer = true;
                levelTimeCount = time;
            }
            */

            //	Init special SECTORs.
            foreach (var sector in map.Sectors)
            {
                if (sector.Special == 0)
                {
                    continue;
                }

                switch ((int)sector.Special)
                {
                    case 1:
                        // FLICKERING LIGHTS
                        lightingChange.SpawnLightFlash(sector);
                        break;

                    case 2:
                        // STROBE FAST
                        lightingChange.SpawnStrobeFlash(sector, StrobeFlash.FASTDARK, 0);
                        break;

                    case 3:
                        // STROBE SLOW
                        lightingChange.SpawnStrobeFlash(sector, StrobeFlash.SLOWDARK, 0);
                        break;

                    case 4:
                        // STROBE FAST/DEATH SLIME
                        lightingChange.SpawnStrobeFlash(sector, StrobeFlash.FASTDARK, 0);
                        sector.Special = (SectorSpecial)4;
                        break;

                    case 8:
                        // GLOWING LIGHT
                        lightingChange.SpawnGlowingLight(sector);
                        break;
                    case 9:
                        // SECRET SECTOR
                        totalSecrets++;
                        break;

                    case 10:
                        // DOOR CLOSE IN 30 SECONDS
                        sectorAction.P_SpawnDoorCloseIn30(sector);
                        break;

                    case 12:
                        // SYNC STROBE SLOW
                        lightingChange.SpawnStrobeFlash(sector, StrobeFlash.SLOWDARK, 1);
                        break;

                    case 13:
                        // SYNC STROBE FAST
                        lightingChange.SpawnStrobeFlash(sector, StrobeFlash.FASTDARK, 1);
                        break;

                    case 14:
                        // DOOR RAISE IN 5 MINUTES
                        sectorAction.P_SpawnDoorRaiseIn5Mins(sector);
                        break;

                    case 17:
                        lightingChange.SpawnFireFlicker(sector);
                        break;
                }
            }
        }

        public void G_ExitLevel()
        {
            //secretexit = false;
            gameAction = GameAction.Completed;
        }



        public void StartSound(Mobj mobj, Sfx sfx)
        {
            if (audio == null)
            {
                return;
            }

            audio.StartSound(mobj, sfx);
        }

        public void StopSound(Mobj mobj)
        {
            if (audio == null)
            {
                return;
            }

            audio.StopSound(mobj);
        }

        public int GetNewValidCount()
        {
            validCount++;
            return validCount;
        }

        public Map Map => map;
        public DoomRandom Random => random;

        public Thinkers Thinkers => thinkers;
        public Specials Specials => specials;

        public Thing[] PlayerStarts => playerStarts;

        public ThingAllocation ThingAllocation => thingAllocation;
        public ThingMovement ThingMovement => thingMovement;
        public ThingInteraction ThingInteraction => thingInteraction;
        public MapCollision MapCollision => mapCollision;
        public MapInteraction MapInteraction => mapInteraction;
        public PathTraversal PathTraversal => pathTraversal;
        public Hitscan Hitscan => hitscan;
        public VisibilityCheck VisibilityCheck => visibilityCheck;
        public SectorAction SectorAction => sectorAction;
        public PlayerBehavior PlayerBehavior => playerBehavior;
        public ItemPickup ItemPickup => itemPickup;
        public WeaponBehavior WeaponBehavior => weaponBehavior;
        public MonsterBehavior MonsterBehavior => monsterBehavior;
        public LightingChange LightingChange => lightingChange;


        public static readonly Fixed USERANGE = Fixed.FromInt(64);
        public static readonly Fixed MELEERANGE = Fixed.FromInt(64);
        public static readonly Fixed MISSILERANGE = Fixed.FromInt(32 * 64);



        private SfmlAudio audio;

        public SfmlAudio Audio
        {
            get => audio;
            set => audio = value;
        }
    }
}
