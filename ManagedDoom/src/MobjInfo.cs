using System;

namespace ManagedDoom
{
    public sealed class MobjInfo
    {
        private int doomEdNum;
        private State spawnState;
        private int spawnHealth;
        private State seeState;
        private Sfx seeSound;
        private int reactionTime;
        private Sfx attackSound;
        private State painState;
        private int painChance;
        private Sfx painSound;
        private State meleeState;
        private State missileState;
        private State deathState;
        private State xdeathState;
        private Sfx deathSound;
        private int speed;
        private Fixed radius;
        private Fixed height;
        private int mass;
        private int damage;
        private Sfx activeSound;
        private MobjFlags flags;
        private State raiseState;

        public MobjInfo(
            int doomEdNum,
            State spawnState,
            int spawnHealth,
            State seeState,
            Sfx seeSound,
            int reactionTime,
            Sfx attackSound,
            State painState,
            int painChance,
            Sfx painSound,
            State meleeState,
            State missileState,
            State deathState,
            State xdeathState,
            Sfx deathSound,
            int speed,
            Fixed radius,
            Fixed height,
            int mass,
            int damage,
            Sfx activeSound,
            MobjFlags flags,
            State raiseState)
        {
            this.doomEdNum = doomEdNum;
            this.spawnState = spawnState;
            this.spawnHealth = spawnHealth;
            this.seeState = seeState;
            this.seeSound = seeSound;
            this.reactionTime = reactionTime;
            this.attackSound = attackSound;
            this.painState = painState;
            this.painChance = painChance;
            this.painSound = painSound;
            this.meleeState = meleeState;
            this.missileState = missileState;
            this.deathState = deathState;
            this.xdeathState = xdeathState;
            this.deathSound = deathSound;
            this.speed = speed;
            this.radius = radius;
            this.height = height;
            this.mass = mass;
            this.damage = damage;
            this.activeSound = activeSound;
            this.flags = flags;
            this.raiseState = raiseState;
        }

        public int DoomEdNum => doomEdNum;
        public State SpawnState => spawnState;
        public int SpawnHealth => spawnHealth;
        public State SeeState => seeState;
        public Sfx SeeSound => seeSound;
        public int ReactionTime => reactionTime;
        public Sfx AttackSound => attackSound;
        public State PainState => painState;
        public int PainChance => painChance;
        public Sfx PainSound => painSound;
        public State MeleeState => meleeState;
        public State MissileState => missileState;
        public State DeathState => deathState;
        public State XdeathState => xdeathState;
        public Sfx DeathSound => deathSound;
        public int Speed => speed;
        public Fixed Radius => radius;
        public Fixed Height => height;
        public int Mass => mass;
        public int Damage => damage;
        public Sfx ActiveSound => activeSound;
        public MobjFlags Flags => flags;
        public State Raisestate => raiseState;
    }
}
