using System;
using ManagedDoom.SoftwareRendering;

namespace ManagedDoom
{
    public sealed class GameOptions
    {
        public GameSkill Skill = GameSkill.Medium;
        public GameMode GameMode = GameMode.Commercial;
        public bool DemoPlayback = false;
        public bool NetGame = false;
        public int Episode = 1;
        public int Map = 1;
        public int Deathmatch = 0;
        public bool RespawnMonsters = false;
        public bool FastMonsters = false;
        public bool NoMonsters = false;
        public int ConsolePlayer = 0;
        public GameVersion Version = GameVersion.Doom19;
        //public bool[] PlayerInGame = new bool[Player.MaxPlayerCount];
        public MissionPack MissionPack = MissionPack.Doom2;

        public Player[] Players = new Player[Player.MaxPlayerCount];
        public DoomRandom Random = new DoomRandom();
        public int GameTic = 0;
        public IntermissionInfo IntermissionInfo = new IntermissionInfo();

        public Action ResetControl;

        public IRenderer Renderer = null;
        public IAudio Audio = NullAudio.GetInstance();

        public GameOptions()
        {
            for (var i = 0; i < Player.MaxPlayerCount; i++)
            {
                Players[i] = new Player(i);
            }
        }
    }
}
