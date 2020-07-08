using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
            options.IntermissionInfo.TotalFrags = 0;
            options.IntermissionInfo.ParTime = 180;
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
                        thingAllocation.DeathMatchSpawnPlayer(i);
                    }
                }
            }

            specials.SpawnSpecials();

            completed = false;
        }

        public UpdateResult Update()
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

            if (completed)
            {
                return UpdateResult.Completed;
            }
            else
            {
                if (levelTime == 1)
                {
                    return UpdateResult.NeedWipe;
                }
                else
                {
                    return UpdateResult.None;
                }
            }
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

                thingAllocation.SpawnMapThing(mt);
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



        public void StartSound(Mobj mobj, Sfx sfx, SfxType type)
        {
            Options.Audio.StartSound(mobj, sfx, type);
        }

        public void StartSound(Mobj mobj, Sfx sfx, SfxType type, int volume)
        {
            Options.Audio.StartSound(mobj, sfx, type, volume);
        }

        public void StopSound(Mobj mobj)
        {
            Options.Audio.StopSound(mobj);
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

        public Player ConsolePlayer => Options.Players[Options.ConsolePlayer];
        public bool FirstTicIsNotYetDone => ConsolePlayer.ViewZ == Fixed.Epsilon;
        public bool SecretExit => secretexit;
    }
}
