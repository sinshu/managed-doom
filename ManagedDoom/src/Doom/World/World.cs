using System;
using System.IO;
using System.Linq;

namespace ManagedDoom
{
    public sealed partial class World
    {
        public GameOptions Options;

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
        private StatusBar statusBar;
        private AutoMap autoMap;
        private Cheat cheat;

        private MapThing[] playerStarts;
        private MapThing[] deathmatchStarts;

        private bool completed;

        public World(CommonResource resorces, GameOptions options)
        {
            Options = options;

            map = new Map(resorces, this);

            random = options.Random;

            validCount = 0;

            thinkers = new Thinkers(this);
            specials = new Specials(this);

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
            statusBar = new StatusBar(this);
            autoMap = new AutoMap(this);
            cheat = new Cheat(this);

            totalKills = 0;
            totalItems = 0;
            totalSecrets = 0;
            options.wminfo.TotalFrags = 0;
            options.wminfo.ParTime = 180;
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                options.Players[i].KillCount = 0;
                options.Players[i].SecretCount = 0;
                options.Players[i].ItemCount = 0;
            }

            // Initial height of PointOfView
            // will be set by player think.
            options.Players[consoleplayer].ViewZ = Fixed.Epsilon;

            LoadThings();

            // if deathmatch, randomly spawn the active players
            if (options.Deathmatch != 0)
            {
                for (var i = 0; i < Player.MaxPlayerCount; i++)
                {
                    if (options.Players[i].InGame)
                    {
                        options.Players[i].Mobj = null;
                        G_DeathMatchSpawnPlayer(i);
                    }
                }
            }

            SpawnSpecials();

            completed = false;
        }

        public bool Update()
        {
            var players = Options.Players;
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (players[i].InGame)
                {
                    playerBehavior.PlayerThink(players[i]);
                }
            }

            thinkers.Run();
            specials.Update();
            thingAllocation.RespawnSpecials();

            statusBar.Update();
            autoMap.Update();

            levelTime++;

            /*
            var period = 3 * 35;
            if (levelTime % period == period - 1)
            {
                Console.WriteLine("SAVE!");
                var save = new SaveGame("TEST!");
                save.Save(this, "test.dsg");

                Console.WriteLine("LOAD!");
                var load = new LoadGame(File.ReadAllBytes("test.dsg"));
                load.Load(this);
            }
            */

            //var mobjHash = DoomDebug.GetMobjHash(this);
            //var sectorHash = DoomDebug.GetSectorHash(this);
            //Console.WriteLine(levelTime + ": " + mobjHash.ToString("x8") + ", " + sectorHash.ToString("x8"));

            return completed;
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

            playerStarts = ta.PlayerStarts.ToArray();
            deathmatchStarts = ta.DeathmatchStarts.ToArray();
        }


        private static readonly int BODYQUESIZE = 32;
        private Mobj[] bodyque = new Mobj[BODYQUESIZE];
        private int bodyqueslot;

        //
        // G_CheckSpot  
        // Returns false if the player cannot be respawned
        // at the given mapthing_t spot  
        // because something is occupying it 
        //
        public bool G_CheckSpot(int playernum, MapThing mthing)
        {
            var players = Options.Players;

            if (players[playernum].Mobj == null)
            {
                // First spawn of level, before corpses.
                for (var i = 0; i < playernum; i++)
                {
                    if (players[i].Mobj.X == mthing.X && players[i].Mobj.Y == mthing.Y)
                    {
                        return false;
                    }
                }
                return true;
            }

            var x = mthing.X;
            var y = mthing.Y;

            if (!thingMovement.CheckPosition(players[playernum].Mobj, x, y))
            {
                return false;
            }

            // Flush an old corpse if needed.
            if (bodyqueslot >= BODYQUESIZE)
            {
                thingAllocation.RemoveMobj(bodyque[bodyqueslot % BODYQUESIZE]);
            }
            bodyque[bodyqueslot % BODYQUESIZE] = players[playernum].Mobj;
            bodyqueslot++;

            // Spawn a teleport fog.
            var ss = Geometry.PointInSubsector(x, y, map);

            var an = (Angle.Ang45.Data >> Trig.AngleToFineShift) * ((int)Math.Round(mthing.Angle.ToDegree()) / 45);

            Fixed xa;
            Fixed ya;

            switch (an)
            {
                case 4096:  // -4096:
                    xa = Trig.Tan(2048);    // finecosine[-4096]
                    ya = Trig.Tan(0);       // finesine[-4096]
                    break;
                case 5120:  // -3072:
                    xa = Trig.Tan(3072);    // finecosine[-3072]
                    ya = Trig.Tan(1024);    // finesine[-3072]
                    break;
                case 6144:  // -2048:
                    xa = Trig.Sin(0);          // finecosine[-2048]
                    ya = Trig.Tan(2048);    // finesine[-2048]
                    break;
                case 7168:  // -1024:
                    xa = Trig.Sin(1024);       // finecosine[-1024]
                    ya = Trig.Tan(3072);    // finesine[-1024]
                    break;
                case 0:
                case 1024:
                case 2048:
                case 3072:
                    xa = Trig.Cos((int)an);
                    ya = Trig.Sin((int)an);
                    break;
                default:
                    throw new Exception("G_CheckSpot: unexpected angle " + an);
            }

            var mo = thingAllocation.SpawnMobj(
                x + 20 * xa, y + 20 * ya,
                ss.Sector.FloorHeight,
                MobjType.Tfog);

            if (!FirstTicIsNotYetDone)
            {
                // Don't start sound on first frame.
                StartSound(mo, Sfx.TELEPT);
            }

            return true;
        }

        //
        // G_DeathMatchSpawnPlayer 
        // Spawns a player at one of the random death match spots 
        // called at level load and each death 
        //
        public void G_DeathMatchSpawnPlayer(int playernum)
        {
            var selections = deathmatchStarts.Length;
            if (selections < 4)
            {
                throw new Exception("Only " + selections + " deathmatch spots, 4 required");
            }

            for (var j = 0; j < 20; j++)
            {
                var i = random.Next() % selections;
                if (G_CheckSpot(playernum, deathmatchStarts[i]))
                {
                    deathmatchStarts[i].Type = playernum + 1;
                    thingAllocation.SpawnPlayer(deathmatchStarts[i]);
                    return;
                }
            }

            // no good spot, so the player will probably get stuck 
            thingAllocation.SpawnPlayer(playerStarts[playernum]);
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
                        sectorAction.SpawnDoorCloseIn30(sector);
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
                        sectorAction.SpawnDoorRaiseIn5Mins(sector);
                        break;

                    case 17:
                        lightingChange.SpawnFireFlicker(sector);
                        break;
                }
            }
        }


        private bool secretexit = false;

        public void G_ExitLevel()
        {
            secretexit = false;
            completed = true;
        }

        public void G_SecretExitLevel()
        {
            secretexit = true;
            completed = true;
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


        public bool DoEvent(DoomEvent e)
        {
            if (!Options.NetGame && Options.Skill != GameSkill.Nightmare)
            {
                cheat.DoEvent(e);
            }

            if (autoMap.Visible)
            {
                if (autoMap.DoEvent(e))
                {
                    return true;
                }
            }

            if (e.Key == DoomKeys.Tab && e.Type == EventType.KeyDown)
            {
                if (autoMap.Visible)
                {
                    autoMap.Close();
                }
                else
                {
                    autoMap.Open();
                }
                return true;
            }

            return false;
        }



        public Map Map => map;
        public DoomRandom Random => random;

        public Thinkers Thinkers => thinkers;
        public Specials Specials => specials;

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
        public StatusBar StatusBar => statusBar;
        public AutoMap AutoMap => autoMap;
        public Cheat Cheat => cheat;

        public MapThing[] PlayerStarts => playerStarts;
        public MapThing[] DeathmatchStarts => deathmatchStarts;

        public Player ConsolePlayer => Options.Players[Options.ConsolePlayer];
        public bool FirstTicIsNotYetDone => ConsolePlayer.ViewZ == Fixed.Epsilon;
        public bool SecretExit => secretexit;

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
