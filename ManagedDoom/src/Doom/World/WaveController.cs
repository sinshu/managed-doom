using System;
using System.Collections.Generic;

namespace ManagedDoom {

    public sealed class WaveController {

        private int wave = 0;

        private int monstersKilledTotal = 0;
        private int monstersKilledNeeeded = 0;
        private int monsterSpawnCount = 0;

        private int currentMonstersPerWave = 2;
        private int monstersPerWaveIncrease = 3;

        private int specialMonstersWaveInterval = 5;

        private float currentMonsterHealthMultiplyer = 0.5f;
        private float monsterHealthMultiplyerIncrease = 0.5f;

        private bool Started = false;
        private int waveStartTime;
        private int waveDelay = GameConst.TicRate * 5;

        private MobjType currentMonsterType = MobjType.Possessed;

        private World world;
        private List<MapThing> spawnPoints;

        public WaveController( World world ) {

            this.world = world;

            spawnPoints = new List<MapThing>();
            foreach ( var thing in world.Map.Things ) {

                if ( thing.Type != 3004 && thing.Type != 9 ) continue; //Replace with ID for Spawnpoint

                spawnPoints.Add( thing );

            }

        }

        public void Start() {

            if ( !Started ) {

                Started = true;

            }

        }

        public void Update() {

            if ( !Started ) return;


            monstersKilledTotal = world.Options.Players[0].KillCount;
            if ( monstersKilledNeeeded <= monstersKilledTotal ) {

                waveStartTime = world.LevelTime;
                StartWave();

            }


            if ( waveStartTime + waveDelay > world.LevelTime ) return;

            if ( monsterSpawnCount <= 0 ) return;
            SpawnMonster();

        }

        public void StartWave() {

            wave++;
            world.Options.Players[0].SendMessage( "Wave " + wave + " Starting..." );

            currentMonstersPerWave += monstersPerWaveIncrease;

            monsterSpawnCount = currentMonstersPerWave;
            monstersKilledNeeeded = monstersKilledTotal + currentMonstersPerWave;
            currentMonsterHealthMultiplyer += monsterHealthMultiplyerIncrease;

        }

        public void SpawnMonster() {

            MobjType type = currentMonsterType;
            if ( wave % specialMonstersWaveInterval == 0 ) type = MobjType.Troop;

            MapThing spawnPoint = spawnPoints[ new Random().Next( spawnPoints.Count ) ];

            if ( !CheckOpenPoint( Fixed.FromInt( 20 ), spawnPoint.X, spawnPoint.Y ) ) return;

            var mobj = world.ThingAllocation.SpawnMobj( spawnPoint.X, spawnPoint.Y, Mobj.OnFloorZ, type );
            mobj.SpawnPoint = spawnPoint;
            mobj.Health = (int) (float) currentMonsterHealthMultiplyer * mobj.Health;
            monsterSpawnCount--;

        }

        public bool CheckOpenPoint( Fixed radius, Fixed x, Fixed y ) {

            var thinkers = world.Thinkers;
            foreach ( Thinker thinker in thinkers ) {

                if ( thinker is not Mobj mobj ) continue;
                if ( ( mobj.Flags & MobjFlags.Solid ) == 0 ) continue;

                if ( mobj.X >= x - radius &&
                     mobj.X <= x + radius &&
                     mobj.Y >= y - radius &&
                     mobj.Y <= y + radius ) {

                    return false;

                }

            }

            return true;

        }



    }

}
