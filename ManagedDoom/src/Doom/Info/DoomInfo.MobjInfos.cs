using System;

namespace ManagedDoom
{
    public static partial class DoomInfo
    {
        public static readonly MobjInfo[] MobjInfos = new MobjInfo[]
        {
            new MobjInfo( // MobjType.Player
                -1, // doomEdNum
                State.Play, // spawnState
                100, // spawnHealth
                State.PlayRun1, // seeState
                Sfx.NONE, // seeSound
                0, // reactionTime
                Sfx.NONE, // attackSound
                State.PlayPain, // painState
                255, // painChance
                Sfx.PLPAIN, // painSound
                State.Null, // meleeState
                State.PlayAtk1, // missileState
                State.PlayDie1, // deathState
                State.PlayXdie1, // xdeathState
                Sfx.PLDETH, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(56), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.DropOff | MobjFlags.PickUp | MobjFlags.NotDeathmatch, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Possessed
                3004, // doomEdNum
                State.PossStnd, // spawnState
                20, // spawnHealth
                State.PossRun1, // seeState
                Sfx.POSIT1, // seeSound
                8, // reactionTime
                Sfx.PISTOL, // attackSound
                State.PossPain, // painState
                200, // painChance
                Sfx.POPAIN, // painSound
                State.Null, // meleeState
                State.PossAtk1, // missileState
                State.PossDie1, // deathState
                State.PossXdie1, // xdeathState
                Sfx.PODTH1, // deathSound
                8, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(56), // height
                100, // mass
                0, // damage
                Sfx.POSACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.PossRaise1 // raiseState
            ),

            new MobjInfo( // MobjType.Shotguy
                9, // doomEdNum
                State.SposStnd, // spawnState
                30, // spawnHealth
                State.SposRun1, // seeState
                Sfx.POSIT2, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.SposPain, // painState
                170, // painChance
                Sfx.POPAIN, // painSound
                State.Null, // meleeState
                State.SposAtk1, // missileState
                State.SposDie1, // deathState
                State.SposXdie1, // xdeathState
                Sfx.PODTH2, // deathSound
                8, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(56), // height
                100, // mass
                0, // damage
                Sfx.POSACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.SposRaise1 // raiseState
            ),

            new MobjInfo( // MobjType.Vile
                64, // doomEdNum
                State.VileStnd, // spawnState
                700, // spawnHealth
                State.VileRun1, // seeState
                Sfx.VILSIT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.VilePain, // painState
                10, // painChance
                Sfx.VIPAIN, // painSound
                State.Null, // meleeState
                State.VileAtk1, // missileState
                State.VileDie1, // deathState
                State.Null, // xdeathState
                Sfx.VILDTH, // deathSound
                15, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(56), // height
                500, // mass
                0, // damage
                Sfx.VILACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Fire
                -1, // doomEdNum
                State.Fire1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Undead
                66, // doomEdNum
                State.SkelStnd, // spawnState
                300, // spawnHealth
                State.SkelRun1, // seeState
                Sfx.SKESIT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.SkelPain, // painState
                100, // painChance
                Sfx.POPAIN, // painSound
                State.SkelFist1, // meleeState
                State.SkelMiss1, // missileState
                State.SkelDie1, // deathState
                State.Null, // xdeathState
                Sfx.SKEDTH, // deathSound
                10, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(56), // height
                500, // mass
                0, // damage
                Sfx.SKEACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.SkelRaise1 // raiseState
            ),

            new MobjInfo( // MobjType.Tracer
                -1, // doomEdNum
                State.Tracer, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.SKEATK, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Traceexp1, // deathState
                State.Null, // xdeathState
                Sfx.BAREXP, // deathSound
                10 * Fixed.FracUnit, // speed
                Fixed.FromInt(11), // radius
                Fixed.FromInt(8), // height
                100, // mass
                10, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Smoke
                -1, // doomEdNum
                State.Smoke1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Fatso
                67, // doomEdNum
                State.FattStnd, // spawnState
                600, // spawnHealth
                State.FattRun1, // seeState
                Sfx.MANSIT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.FattPain, // painState
                80, // painChance
                Sfx.MNPAIN, // painSound
                State.Null, // meleeState
                State.FattAtk1, // missileState
                State.FattDie1, // deathState
                State.Null, // xdeathState
                Sfx.MANDTH, // deathSound
                8, // speed
                Fixed.FromInt(48), // radius
                Fixed.FromInt(64), // height
                1000, // mass
                0, // damage
                Sfx.POSACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.FattRaise1 // raiseState
            ),

            new MobjInfo( // MobjType.Fatshot
                -1, // doomEdNum
                State.Fatshot1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.FIRSHT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Fatshotx1, // deathState
                State.Null, // xdeathState
                Sfx.FIRXPL, // deathSound
                20 * Fixed.FracUnit, // speed
                Fixed.FromInt(6), // radius
                Fixed.FromInt(8), // height
                100, // mass
                8, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Chainguy
                65, // doomEdNum
                State.CposStnd, // spawnState
                70, // spawnHealth
                State.CposRun1, // seeState
                Sfx.POSIT2, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.CposPain, // painState
                170, // painChance
                Sfx.POPAIN, // painSound
                State.Null, // meleeState
                State.CposAtk1, // missileState
                State.CposDie1, // deathState
                State.CposXdie1, // xdeathState
                Sfx.PODTH2, // deathSound
                8, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(56), // height
                100, // mass
                0, // damage
                Sfx.POSACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.CposRaise1 // raiseState
            ),

            new MobjInfo( // MobjType.Troop
                3001, // doomEdNum
                State.TrooStnd, // spawnState
                60, // spawnHealth
                State.TrooRun1, // seeState
                Sfx.BGSIT1, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.TrooPain, // painState
                200, // painChance
                Sfx.POPAIN, // painSound
                State.TrooAtk1, // meleeState
                State.TrooAtk1, // missileState
                State.TrooDie1, // deathState
                State.TrooXdie1, // xdeathState
                Sfx.BGDTH1, // deathSound
                8, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(56), // height
                100, // mass
                0, // damage
                Sfx.BGACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.TrooRaise1 // raiseState
            ),

            new MobjInfo( // MobjType.Sergeant
                3002, // doomEdNum
                State.SargStnd, // spawnState
                150, // spawnHealth
                State.SargRun1, // seeState
                Sfx.SGTSIT, // seeSound
                8, // reactionTime
                Sfx.SGTATK, // attackSound
                State.SargPain, // painState
                180, // painChance
                Sfx.DMPAIN, // painSound
                State.SargAtk1, // meleeState
                State.Null, // missileState
                State.SargDie1, // deathState
                State.Null, // xdeathState
                Sfx.SGTDTH, // deathSound
                10, // speed
                Fixed.FromInt(30), // radius
                Fixed.FromInt(56), // height
                400, // mass
                0, // damage
                Sfx.DMACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.SargRaise1 // raiseState
            ),

            new MobjInfo( // MobjType.Shadows
                58, // doomEdNum
                State.SargStnd, // spawnState
                150, // spawnHealth
                State.SargRun1, // seeState
                Sfx.SGTSIT, // seeSound
                8, // reactionTime
                Sfx.SGTATK, // attackSound
                State.SargPain, // painState
                180, // painChance
                Sfx.DMPAIN, // painSound
                State.SargAtk1, // meleeState
                State.Null, // missileState
                State.SargDie1, // deathState
                State.Null, // xdeathState
                Sfx.SGTDTH, // deathSound
                10, // speed
                Fixed.FromInt(30), // radius
                Fixed.FromInt(56), // height
                400, // mass
                0, // damage
                Sfx.DMACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.Shadow | MobjFlags.CountKill, // flags
                State.SargRaise1 // raiseState
            ),

            new MobjInfo( // MobjType.Head
                3005, // doomEdNum
                State.HeadStnd, // spawnState
                400, // spawnHealth
                State.HeadRun1, // seeState
                Sfx.CACSIT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.HeadPain, // painState
                128, // painChance
                Sfx.DMPAIN, // painSound
                State.Null, // meleeState
                State.HeadAtk1, // missileState
                State.HeadDie1, // deathState
                State.Null, // xdeathState
                Sfx.CACDTH, // deathSound
                8, // speed
                Fixed.FromInt(31), // radius
                Fixed.FromInt(56), // height
                400, // mass
                0, // damage
                Sfx.DMACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.Float | MobjFlags.NoGravity | MobjFlags.CountKill, // flags
                State.HeadRaise1 // raiseState
            ),

            new MobjInfo( // MobjType.Bruiser
                3003, // doomEdNum
                State.BossStnd, // spawnState
                1000, // spawnHealth
                State.BossRun1, // seeState
                Sfx.BRSSIT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.BossPain, // painState
                50, // painChance
                Sfx.DMPAIN, // painSound
                State.BossAtk1, // meleeState
                State.BossAtk1, // missileState
                State.BossDie1, // deathState
                State.Null, // xdeathState
                Sfx.BRSDTH, // deathSound
                8, // speed
                Fixed.FromInt(24), // radius
                Fixed.FromInt(64), // height
                1000, // mass
                0, // damage
                Sfx.DMACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.BossRaise1 // raiseState
            ),

            new MobjInfo( // MobjType.Bruisershot
                -1, // doomEdNum
                State.Brball1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.FIRSHT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Brballx1, // deathState
                State.Null, // xdeathState
                Sfx.FIRXPL, // deathSound
                15 * Fixed.FracUnit, // speed
                Fixed.FromInt(6), // radius
                Fixed.FromInt(8), // height
                100, // mass
                8, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Knight
                69, // doomEdNum
                State.Bos2Stnd, // spawnState
                500, // spawnHealth
                State.Bos2Run1, // seeState
                Sfx.KNTSIT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Bos2Pain, // painState
                50, // painChance
                Sfx.DMPAIN, // painSound
                State.Bos2Atk1, // meleeState
                State.Bos2Atk1, // missileState
                State.Bos2Die1, // deathState
                State.Null, // xdeathState
                Sfx.KNTDTH, // deathSound
                8, // speed
                Fixed.FromInt(24), // radius
                Fixed.FromInt(64), // height
                1000, // mass
                0, // damage
                Sfx.DMACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.Bos2Raise1 // raiseState
            ),

            new MobjInfo( // MobjType.Skull
                3006, // doomEdNum
                State.SkullStnd, // spawnState
                100, // spawnHealth
                State.SkullRun1, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.SKLATK, // attackSound
                State.SkullPain, // painState
                256, // painChance
                Sfx.DMPAIN, // painSound
                State.Null, // meleeState
                State.SkullAtk1, // missileState
                State.SkullDie1, // deathState
                State.Null, // xdeathState
                Sfx.FIRXPL, // deathSound
                8, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(56), // height
                50, // mass
                3, // damage
                Sfx.DMACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.Float | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Spider
                7, // doomEdNum
                State.SpidStnd, // spawnState
                3000, // spawnHealth
                State.SpidRun1, // seeState
                Sfx.SPISIT, // seeSound
                8, // reactionTime
                Sfx.SHOTGN, // attackSound
                State.SpidPain, // painState
                40, // painChance
                Sfx.DMPAIN, // painSound
                State.Null, // meleeState
                State.SpidAtk1, // missileState
                State.SpidDie1, // deathState
                State.Null, // xdeathState
                Sfx.SPIDTH, // deathSound
                12, // speed
                Fixed.FromInt(128), // radius
                Fixed.FromInt(100), // height
                1000, // mass
                0, // damage
                Sfx.DMACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Baby
                68, // doomEdNum
                State.BspiStnd, // spawnState
                500, // spawnHealth
                State.BspiSight, // seeState
                Sfx.BSPSIT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.BspiPain, // painState
                128, // painChance
                Sfx.DMPAIN, // painSound
                State.Null, // meleeState
                State.BspiAtk1, // missileState
                State.BspiDie1, // deathState
                State.Null, // xdeathState
                Sfx.BSPDTH, // deathSound
                12, // speed
                Fixed.FromInt(64), // radius
                Fixed.FromInt(64), // height
                600, // mass
                0, // damage
                Sfx.BSPACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.BspiRaise1 // raiseState
            ),

            new MobjInfo( // MobjType.Cyborg
                16, // doomEdNum
                State.CyberStnd, // spawnState
                4000, // spawnHealth
                State.CyberRun1, // seeState
                Sfx.CYBSIT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.CyberPain, // painState
                20, // painChance
                Sfx.DMPAIN, // painSound
                State.Null, // meleeState
                State.CyberAtk1, // missileState
                State.CyberDie1, // deathState
                State.Null, // xdeathState
                Sfx.CYBDTH, // deathSound
                16, // speed
                Fixed.FromInt(40), // radius
                Fixed.FromInt(110), // height
                1000, // mass
                0, // damage
                Sfx.DMACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Pain
                71, // doomEdNum
                State.PainStnd, // spawnState
                400, // spawnHealth
                State.PainRun1, // seeState
                Sfx.PESIT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.PainPain, // painState
                128, // painChance
                Sfx.PEPAIN, // painSound
                State.Null, // meleeState
                State.PainAtk1, // missileState
                State.PainDie1, // deathState
                State.Null, // xdeathState
                Sfx.PEDTH, // deathSound
                8, // speed
                Fixed.FromInt(31), // radius
                Fixed.FromInt(56), // height
                400, // mass
                0, // damage
                Sfx.DMACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.Float | MobjFlags.NoGravity | MobjFlags.CountKill, // flags
                State.PainRaise1 // raiseState
            ),

            new MobjInfo( // MobjType.Wolfss
                84, // doomEdNum
                State.SswvStnd, // spawnState
                50, // spawnHealth
                State.SswvRun1, // seeState
                Sfx.SSSIT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.SswvPain, // painState
                170, // painChance
                Sfx.POPAIN, // painSound
                State.Null, // meleeState
                State.SswvAtk1, // missileState
                State.SswvDie1, // deathState
                State.SswvXdie1, // xdeathState
                Sfx.SSDTH, // deathSound
                8, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(56), // height
                100, // mass
                0, // damage
                Sfx.POSACT, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.SswvRaise1 // raiseState
            ),

            new MobjInfo( // MobjType.Keen
                72, // doomEdNum
                State.Keenstnd, // spawnState
                100, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Keenpain, // painState
                256, // painChance
                Sfx.KEENPN, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Commkeen, // deathState
                State.Null, // xdeathState
                Sfx.KEENDT, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(72), // height
                10000000, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity | MobjFlags.Shootable | MobjFlags.CountKill, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Bossbrain
                88, // doomEdNum
                State.Brain, // spawnState
                250, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.BrainPain, // painState
                255, // painChance
                Sfx.BOSPN, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.BrainDie1, // deathState
                State.Null, // xdeathState
                Sfx.BOSDTH, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                10000000, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Bossspit
                89, // doomEdNum
                State.Braineye, // spawnState
                1000, // spawnHealth
                State.Braineyesee, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(32), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.NoSector, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Bosstarget
                87, // doomEdNum
                State.Null, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(32), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.NoSector, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Spawnshot
                -1, // doomEdNum
                State.Spawn1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.BOSPIT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.FIRXPL, // deathSound
                10 * Fixed.FracUnit, // speed
                Fixed.FromInt(6), // radius
                Fixed.FromInt(32), // height
                100, // mass
                3, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity | MobjFlags.NoClip, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Spawnfire
                -1, // doomEdNum
                State.Spawnfire1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Barrel
                2035, // doomEdNum
                State.Bar1, // spawnState
                20, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Bexp, // deathState
                State.Null, // xdeathState
                Sfx.BAREXP, // deathSound
                0, // speed
                Fixed.FromInt(10), // radius
                Fixed.FromInt(42), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.Shootable | MobjFlags.NoBlood, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Troopshot
                -1, // doomEdNum
                State.Tball1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.FIRSHT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Tballx1, // deathState
                State.Null, // xdeathState
                Sfx.FIRXPL, // deathSound
                10 * Fixed.FracUnit, // speed
                Fixed.FromInt(6), // radius
                Fixed.FromInt(8), // height
                100, // mass
                3, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Headshot
                -1, // doomEdNum
                State.Rball1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.FIRSHT, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Rballx1, // deathState
                State.Null, // xdeathState
                Sfx.FIRXPL, // deathSound
                10 * Fixed.FracUnit, // speed
                Fixed.FromInt(6), // radius
                Fixed.FromInt(8), // height
                100, // mass
                5, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Rocket
                -1, // doomEdNum
                State.Rocket, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.RLAUNC, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Explode1, // deathState
                State.Null, // xdeathState
                Sfx.BAREXP, // deathSound
                20 * Fixed.FracUnit, // speed
                Fixed.FromInt(11), // radius
                Fixed.FromInt(8), // height
                100, // mass
                20, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Plasma
                -1, // doomEdNum
                State.Plasball, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.PLASMA, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Plasexp, // deathState
                State.Null, // xdeathState
                Sfx.FIRXPL, // deathSound
                25 * Fixed.FracUnit, // speed
                Fixed.FromInt(13), // radius
                Fixed.FromInt(8), // height
                100, // mass
                5, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Bfg
                -1, // doomEdNum
                State.Bfgshot, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Bfgland, // deathState
                State.Null, // xdeathState
                Sfx.RXPLOD, // deathSound
                25 * Fixed.FracUnit, // speed
                Fixed.FromInt(13), // radius
                Fixed.FromInt(8), // height
                100, // mass
                100, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Arachplaz
                -1, // doomEdNum
                State.ArachPlaz, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.PLASMA, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.ArachPlex, // deathState
                State.Null, // xdeathState
                Sfx.FIRXPL, // deathSound
                25 * Fixed.FracUnit, // speed
                Fixed.FromInt(13), // radius
                Fixed.FromInt(8), // height
                100, // mass
                5, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.Missile | MobjFlags.DropOff | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Puff
                -1, // doomEdNum
                State.Puff1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Blood
                -1, // doomEdNum
                State.Blood1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Tfog
                -1, // doomEdNum
                State.Tfog, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Ifog
                -1, // doomEdNum
                State.Ifog, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Teleportman
                14, // doomEdNum
                State.Null, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.NoSector, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Extrabfg
                -1, // doomEdNum
                State.Bfgexp, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc0
                2018, // doomEdNum
                State.Arm1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc1
                2019, // doomEdNum
                State.Arm2, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc2
                2014, // doomEdNum
                State.Bon1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.CountItem, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc3
                2015, // doomEdNum
                State.Bon2, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.CountItem, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc4
                5, // doomEdNum
                State.Bkey, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.NotDeathmatch, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc5
                13, // doomEdNum
                State.Rkey, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.NotDeathmatch, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc6
                6, // doomEdNum
                State.Ykey, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.NotDeathmatch, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc7
                39, // doomEdNum
                State.Yskull, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.NotDeathmatch, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc8
                38, // doomEdNum
                State.Rskull, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.NotDeathmatch, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc9
                40, // doomEdNum
                State.Bskull, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.NotDeathmatch, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc10
                2011, // doomEdNum
                State.Stim, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc11
                2012, // doomEdNum
                State.Medi, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc12
                2013, // doomEdNum
                State.Soul, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.CountItem, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Inv
                2022, // doomEdNum
                State.Pinv, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.CountItem, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc13
                2023, // doomEdNum
                State.Pstr, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.CountItem, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Ins
                2024, // doomEdNum
                State.Pins, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.CountItem, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc14
                2025, // doomEdNum
                State.Suit, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc15
                2026, // doomEdNum
                State.Pmap, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.CountItem, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc16
                2045, // doomEdNum
                State.Pvis, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.CountItem, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Mega
                83, // doomEdNum
                State.Mega, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special | MobjFlags.CountItem, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Clip
                2007, // doomEdNum
                State.Clip, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc17
                2048, // doomEdNum
                State.Ammo, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc18
                2010, // doomEdNum
                State.Rock, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc19
                2046, // doomEdNum
                State.Brok, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc20
                2047, // doomEdNum
                State.Cell, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc21
                17, // doomEdNum
                State.Celp, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc22
                2008, // doomEdNum
                State.Shel, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc23
                2049, // doomEdNum
                State.Sbox, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc24
                8, // doomEdNum
                State.Bpak, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc25
                2006, // doomEdNum
                State.Bfug, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Chaingun
                2002, // doomEdNum
                State.Mgun, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc26
                2005, // doomEdNum
                State.Csaw, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc27
                2003, // doomEdNum
                State.Laun, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc28
                2004, // doomEdNum
                State.Plas, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Shotgun
                2001, // doomEdNum
                State.Shot, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Supershotgun
                82, // doomEdNum
                State.Shot2, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Special, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc29
                85, // doomEdNum
                State.Techlamp, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc30
                86, // doomEdNum
                State.Tech2Lamp, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc31
                2028, // doomEdNum
                State.Colu, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc32
                30, // doomEdNum
                State.Tallgrncol, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc33
                31, // doomEdNum
                State.Shrtgrncol, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc34
                32, // doomEdNum
                State.Tallredcol, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc35
                33, // doomEdNum
                State.Shrtredcol, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc36
                37, // doomEdNum
                State.Skullcol, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc37
                36, // doomEdNum
                State.Heartcol, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc38
                41, // doomEdNum
                State.Evileye, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc39
                42, // doomEdNum
                State.Floatskull, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc40
                43, // doomEdNum
                State.Torchtree, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc41
                44, // doomEdNum
                State.Bluetorch, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc42
                45, // doomEdNum
                State.Greentorch, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc43
                46, // doomEdNum
                State.Redtorch, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc44
                55, // doomEdNum
                State.Btorchshrt, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc45
                56, // doomEdNum
                State.Gtorchshrt, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc46
                57, // doomEdNum
                State.Rtorchshrt, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc47
                47, // doomEdNum
                State.Stalagtite, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc48
                48, // doomEdNum
                State.Techpillar, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc49
                34, // doomEdNum
                State.Candlestik, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                0, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc50
                35, // doomEdNum
                State.Candelabra, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc51
                49, // doomEdNum
                State.Bloodytwitch, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(68), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc52
                50, // doomEdNum
                State.Meat2, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(84), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc53
                51, // doomEdNum
                State.Meat3, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(84), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc54
                52, // doomEdNum
                State.Meat4, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(68), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc55
                53, // doomEdNum
                State.Meat5, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(52), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc56
                59, // doomEdNum
                State.Meat2, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(84), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc57
                60, // doomEdNum
                State.Meat4, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(68), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc58
                61, // doomEdNum
                State.Meat3, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(52), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc59
                62, // doomEdNum
                State.Meat5, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(52), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc60
                63, // doomEdNum
                State.Bloodytwitch, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(68), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc61
                22, // doomEdNum
                State.HeadDie6, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                0, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc62
                15, // doomEdNum
                State.PlayDie7, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                0, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc63
                18, // doomEdNum
                State.PossDie5, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                0, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc64
                21, // doomEdNum
                State.SargDie6, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                0, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc65
                23, // doomEdNum
                State.SkullDie6, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                0, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc66
                20, // doomEdNum
                State.TrooDie5, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                0, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc67
                19, // doomEdNum
                State.SposDie5, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                0, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc68
                10, // doomEdNum
                State.PlayXdie9, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                0, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc69
                12, // doomEdNum
                State.PlayXdie9, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                0, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc70
                28, // doomEdNum
                State.Headsonstick, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc71
                24, // doomEdNum
                State.Gibs, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                0, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc72
                27, // doomEdNum
                State.Headonastick, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc73
                29, // doomEdNum
                State.Headcandles, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc74
                25, // doomEdNum
                State.Deadstick, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc75
                26, // doomEdNum
                State.Livestick, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc76
                54, // doomEdNum
                State.Bigtree, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(32), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc77
                70, // doomEdNum
                State.Bbar1, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc78
                73, // doomEdNum
                State.Hangnoguts, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(88), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc79
                74, // doomEdNum
                State.Hangbnobrain, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(88), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc80
                75, // doomEdNum
                State.Hangtlookdn, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(64), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc81
                76, // doomEdNum
                State.Hangtskull, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(64), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc82
                77, // doomEdNum
                State.Hangtlookup, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(64), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc83
                78, // doomEdNum
                State.Hangtnobrain, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(16), // radius
                Fixed.FromInt(64), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.Solid | MobjFlags.SpawnCeiling | MobjFlags.NoGravity, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc84
                79, // doomEdNum
                State.Colongibs, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc85
                80, // doomEdNum
                State.Smallpool, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap, // flags
                State.Null // raiseState
            ),

            new MobjInfo( // MobjType.Misc86
                81, // doomEdNum
                State.Brainstem, // spawnState
                1000, // spawnHealth
                State.Null, // seeState
                Sfx.NONE, // seeSound
                8, // reactionTime
                Sfx.NONE, // attackSound
                State.Null, // painState
                0, // painChance
                Sfx.NONE, // painSound
                State.Null, // meleeState
                State.Null, // missileState
                State.Null, // deathState
                State.Null, // xdeathState
                Sfx.NONE, // deathSound
                0, // speed
                Fixed.FromInt(20), // radius
                Fixed.FromInt(16), // height
                100, // mass
                0, // damage
                Sfx.NONE, // activeSound
                MobjFlags.NoBlockMap, // flags
                State.Null // raiseState
            )

        };
    }
}
