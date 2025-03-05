using System;
using System.Collections.Generic;

namespace ManagedDoom {

    public sealed class WaveController {

        private int wave = 0;

        private int currentMonsterCount = 0; //Currently Spawned Monsters
        private int monsterSpawnCount = 0; //Monsters to be spawned
        private int maxMonsterCount = 10; //Max monsters at once
        private int currentMonstersPerWave = 2; //Monsters per wave

        private bool Started = false;

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

            if ( currentMonsterCount <= 0 ) {

                StartWave();

            }

            if ( currentMonsterCount >= maxMonsterCount || monsterSpawnCount <= 0 ) return;
            SpawnMonster( currentMonsterType );

        }

        public void StartWave() {

            wave++;
            currentMonsterCount = 0;

            currentMonstersPerWave += 3;

            //world.Options.Players[0].KillCount
            monsterSpawnCount = currentMonstersPerWave;

        }

        public void SpawnMonster( MobjType type ) {

            MapThing spawnPoint = spawnPoints[ new Random().Next( spawnPoints.Count ) ];

            if ( !CheckPosition( Fixed.FromInt(20), spawnPoint.X, spawnPoint.Y ) ) {
                Console.WriteLine( "Wave {0}: Spawnpoint blocked", wave );
                return;
            }

            Console.WriteLine( "Wave {0}: Spawning Monster", wave );
            var mobj =  world.ThingAllocation.SpawnMobj( spawnPoint.X, spawnPoint.Y, Mobj.OnFloorZ, type );
            mobj.SpawnPoint = spawnPoint;
            monsterSpawnCount--;
            currentMonsterCount++;

        }

        public bool CheckPosition( Fixed radius, Fixed x, Fixed y ) {

            var thinkers = world.Thinkers;
            foreach ( Thinker thinker in thinkers ) {

                if ( thinker is not Mobj mobj ) continue;
                if ( ( mobj.Flags & MobjFlags.Solid ) == 0) continue;

                if ( mobj.X >= x - radius &&
                    mobj.X <= x + radius &&
                    mobj.Y >= y - radius &&
                    mobj.Y <= y + radius ) {

                    Console.WriteLine( "Blocked at ({0}, {1})", x, y );
                    return false;

                }

            }

            return true;

        }



    }

}
