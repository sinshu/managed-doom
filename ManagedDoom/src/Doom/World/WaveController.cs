using System;
using System.Collections.Generic;

namespace ManagedDoom {

    public sealed class WaveController {

        private int wave = 0;

        private int monstersKilledTotal = 0;
        private int monstersKilledNeeeded = 0;
        private int monsterSpawnCount = 0;
        private int currentMonstersPerWave = 2;

        private bool Started = false;
        private int waveStartTime;
        private int waveDelay = 150;

        private MobjType currentMonsterType = MobjType.Troop;

        private World world;
        private List<MapThing> spawnPoints;

        public WaveController( World world ) {

            this.world = world;

            spawnPoints = new List<MapThing>();
            foreach ( var thing in world.Map.Things ) {

                if ( thing.Type != 3004 ) continue; //Replace with ID for Spawnpoint

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
            SpawnMonster( currentMonsterType );

        }

        public void StartWave() {

            wave++;

            currentMonstersPerWave += 3;

            monsterSpawnCount = currentMonstersPerWave;
            monstersKilledNeeeded = monstersKilledTotal + currentMonstersPerWave;

        }

        public void SpawnMonster( MobjType type ) {

            MapThing spawnPoint = spawnPoints[ new Random().Next( spawnPoints.Count ) ];

            if ( !CheckPosition( Fixed.FromInt( 20 ), spawnPoint.X, spawnPoint.Y ) ) return;

            var mobj =  world.ThingAllocation.SpawnMobj( spawnPoint.X, spawnPoint.Y, Mobj.OnFloorZ, type );
            mobj.SpawnPoint = spawnPoint;
            monsterSpawnCount--;

        }

        public bool CheckPosition( Fixed radius, Fixed x, Fixed y ) {

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
