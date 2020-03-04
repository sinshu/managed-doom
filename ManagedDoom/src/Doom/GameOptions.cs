using System;

namespace ManagedDoom
{
    public sealed class GameOptions
    {
        public Skill GameSkill = Skill.Hard;
        public GameMode GameMode = GameMode.Commercial;
        public bool NetGame = false;
        public int Deathmatch = 0;
        public bool NoMonsters = false;
    }
}
