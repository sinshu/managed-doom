using System;

namespace ManagedDoom
{
    public sealed class MobjInfo
    {
        private int doomEdNum;
        private MobjState spawnState;
        private int spawnHealth;
        private MobjState seeState;
        private Sfx seeSound;
        private int reactionTime;
        private Sfx attackSound;
        private MobjState painState;
        private int painChance;
        private Sfx painSound;
        private MobjState meleeState;
        private MobjState missileState;
        private MobjState deathState;
        private MobjState xdeathState;
        private Sfx deathSound;
        private int speed;
        private Fixed radius;
        private Fixed height;
        private int mass;
        private int damage;
        private Sfx activeSound;
        private MobjFlags flags;
        private MobjState raiseState;

        public MobjInfo(
            int doomEdNum,
            MobjState spawnState,
            int spawnHealth,
            MobjState seeState,
            Sfx seeSound,
            int reactionTime,
            Sfx attackSound,
            MobjState painState,
            int painChance,
            Sfx painSound,
            MobjState meleeState,
            MobjState missileState,
            MobjState deathState,
            MobjState xdeathState,
            Sfx deathSound,
            int speed,
            Fixed radius,
            Fixed height,
            int mass,
            int damage,
            Sfx activeSound,
            MobjFlags flags,
            MobjState raiseState)
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
        public MobjState SpawnState => spawnState;
        public int SpawnHealth => spawnHealth;
        public MobjState SeeState => seeState;
        public Sfx SeeSound => seeSound;
        public int ReactionTime => reactionTime;
        public Sfx AttackSound => attackSound;
        public MobjState PainState => painState;
        public int PainChance => painChance;
        public Sfx PainSound => painSound;
        public MobjState MeleeState => meleeState;
        public MobjState MissileState => missileState;
        public MobjState DeathState => deathState;
        public MobjState XdeathState => xdeathState;
        public Sfx DeathSound => deathSound;
        public int Speed => speed;
        public Fixed Radius => radius;
        public Fixed Height => height;
        public int Mass => mass;
        public int Damage => damage;
        public Sfx ActiveSound => activeSound;
        public MobjFlags Flags => flags;
        public MobjState Raisestate => raiseState;
    }
}
