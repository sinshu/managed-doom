using System;

namespace ManagedDoom
{
    public sealed class GameOptions
    {
        public GameSkill Skill = GameSkill.Medium;
        public GameMode GameMode = GameMode.Commercial;
        public bool NetGame = false;
        public int Episode = 1;
        public int Map = 1;
        public int Deathmatch = 0;
        public bool RespawnMonsters = false;
        public bool FastMonsters = false;
        public bool NoMonsters = false;
        public int ConsolePlayer = 0;
        public GameVersion Version = GameVersion.Doom19;
    }
}
