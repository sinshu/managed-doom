using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace ManagedDoom
{
    public sealed partial class World
    {
        private GameOptions options;
        private DoomGame game;
        private DoomRandom random;

        private Map map;

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

        private int totalKills;
        private int totalItems;
        private int totalSecrets;

        private int levelTime;
        private bool secretExit;
        private bool completed;

        private int validCount;

        public World(CommonResource resorces, GameOptions options) : this(resorces, options, null)
        {
        }

        public World(CommonResource resorces, GameOptions options, DoomGame game)
        {
            this.options = options;
            this.game = game;
            this.random = options.Random;

            map = new Map(resorces, this);

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

            options.IntermissionInfo.TotalFrags = 0;
            options.IntermissionInfo.ParTime = 180;

            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                options.Players[i].KillCount = 0;
                options.Players[i].SecretCount = 0;
                options.Players[i].ItemCount = 0;
            }

            // Initial height of view will be set by player think.
            options.Players[options.ConsolePlayer].ViewZ = Fixed.Epsilon;

            totalKills = 0;
            totalItems = 0;
            totalSecrets = 0;

            LoadThings();

            // If deathmatch, randomly spawn the active players.
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

            levelTime = 0;
            secretExit = false;
            completed = false;

            validCount = 0;
        }

        public UpdateResult Update()
        {
            var players = options.Players;
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
            for (var i = 0; i < map.Things.Length; i++)
            {
                var mt = map.Things[i];

                var spawn = true;

                // Do not spawn cool, new monsters if not commercial.
                if (options.GameMode != GameMode.Commercial)
                {
                    switch (mt.Type)
                    {
                        case 68: // Arachnotron
                        case 64: // Archvile
                        case 88: // Boss Brain
                        case 89: // Boss Shooter
                        case 69: // Hell Knight
                        case 67: // Mancubus
                        case 71: // Pain Elemental
                        case 65: // Former Human Commando
                        case 66: // Revenant
                        case 84: // Wolf SS
                            spawn = false;
                            break;
                    }
                }

                if (!spawn)
                {
                    break;
                }

                thingAllocation.SpawnMapThing(mt);
            }
        }

        public void ExitLevel()
        {
            secretExit = false;
            completed = true;
        }

        public void SecretExitLevel()
        {
            secretExit = true;
            completed = true;
        }

        public void StartSound(Mobj mobj, Sfx sfx, SfxType type)
        {
            options.Audio.StartSound(mobj, sfx, type);
        }

        public void StartSound(Mobj mobj, Sfx sfx, SfxType type, int volume)
        {
            options.Audio.StartSound(mobj, sfx, type, volume);
        }

        public void StopSound(Mobj mobj)
        {
            options.Audio.StopSound(mobj);
        }

        public int GetNewValidCount()
        {
            validCount++;
            return validCount;
        }

        public bool DoEvent(DoomEvent e)
        {
            if (!options.NetGame && options.Skill != GameSkill.Nightmare)
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

        public GameOptions Options => options;
        public DoomGame Game => game;
        public DoomRandom Random => random;

        public Map Map => map;

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

        public int TotalKills
        {
            get => totalKills;
            set => totalKills = value;
        }

        public int TotalItems
        {
            get => totalItems;
            set => totalItems = value;
        }

        public int TotalSecrets
        {
            get => totalSecrets;
            set => totalSecrets = value;
        }

        public int LevelTime
        {
            get => levelTime;
            set => levelTime = value;
        }

        public bool SecretExit => secretExit;

        public Player ConsolePlayer => Options.Players[Options.ConsolePlayer];
        public bool FirstTicIsNotYetDone => ConsolePlayer.ViewZ == Fixed.Epsilon;
    }
}
