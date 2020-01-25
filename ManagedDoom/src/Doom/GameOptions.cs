using System;

namespace ManagedDoom
{
    public sealed class GameOptions
    {
        public Skill GameSkill = Skill.Hard;
        public GameMode GameMode = GameMode.Commercial;
        public bool NetGame = false;
        public bool Deathmatch = false;
        public bool NoMonsters = false;
    }
}
