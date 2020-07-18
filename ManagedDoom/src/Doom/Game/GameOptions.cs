using System;
using ManagedDoom.SoftwareRendering;

namespace ManagedDoom
{
    public sealed class GameOptions
    {
        private GameVersion gameVersion;
        private GameMode gameMode;
        private MissionPack missionPack;

        private Player[] players;
        private int consolePlayer;

        private int episode;
        private int map;
        private GameSkill skill;

        private bool demoPlayback;
        private bool netGame;

        private int deathmatch;
        private bool fastMonsters;
        private bool respawnMonsters;
        private bool noMonsters;

        private DoomRandom random;
        private int gameTic;

        private IntermissionInfo intermissionInfo;

        private IRenderer renderer;
        private IAudio sound;

        private Action resetControl;

        public GameOptions()
        {
            gameVersion = GameVersion.Doom19;
            gameMode = GameMode.Commercial;
            missionPack = MissionPack.Doom2;

            players = new Player[Player.MaxPlayerCount];
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                players[i] = new Player(i);
            }
            consolePlayer = 0;

            episode = 1;
            map = 1;
            skill = GameSkill.Medium;

            demoPlayback = false;
            netGame = false;

            deathmatch = 0;
            fastMonsters = false;
            respawnMonsters = false;
            noMonsters = false;

            random = new DoomRandom();
            gameTic = 0;

            intermissionInfo = new IntermissionInfo();

            renderer = null;
            sound = NullAudio.GetInstance();

            resetControl = null;
        }

        public GameVersion GameVersion
        {
            get => gameVersion;
            set => gameVersion = value;
        }

        public GameMode GameMode
        {
            get => gameMode;
            set => gameMode = value;
        }

        public MissionPack MissionPack
        {
            get => missionPack;
            set => missionPack = value;
        }

        public Player[] Players
        {
            get => players;
        }

        public int ConsolePlayer
        {
            get => consolePlayer;
            set => consolePlayer = value;
        }

        public int Episode
        {
            get => episode;
            set => episode = value;
        }

        public int Map
        {
            get => map;
            set => map = value;
        }

        public GameSkill Skill
        {
            get => skill;
            set => skill = value;
        }

        public bool DemoPlayback
        {
            get => demoPlayback;
            set => demoPlayback = value;
        }

        public bool NetGame
        {
            get => netGame;
            set => netGame = value;
        }

        public int Deathmatch
        {
            get => deathmatch;
            set => deathmatch = value;
        }

        public bool FastMonsters
        {
            get => fastMonsters;
            set => fastMonsters = value;
        }

        public bool RespawnMonsters
        {
            get => respawnMonsters;
            set => respawnMonsters = value;
        }

        public bool NoMonsters
        {
            get => noMonsters;
            set => noMonsters = value;
        }

        public DoomRandom Random
        {
            get => random;
        }

        public int GameTic
        {
            get => gameTic;
            set => gameTic = value;
        }

        public IntermissionInfo IntermissionInfo
        {
            get => intermissionInfo;
        }

        public IRenderer Renderer
        {
            get => renderer;
            set => renderer = value;
        }

        public IAudio Audio
        {
            get => sound;
            set => sound = value;
        }

        public Action ResetControl
        {
            get => resetControl;
            set => resetControl = value;
        }
    }
}
