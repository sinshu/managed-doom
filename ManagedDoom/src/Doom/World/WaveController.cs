using System;

namespace ManagedDoom {
    
    public sealed class WaveController {

        private int wave = 0;
        
        private int currentMonsterCount = 0; //Currently Spawned Monsters
        private int monsterSpawnCount = 0; //Monsters to be spawned
        private int maxMonsterCount = 10; //Max monsters at once
        private int currentMonstersPerWave = 2; //Monsters per wave

        private MobjType currentMonsterType = MobjType.Possessed;

        private World world;

        public WaveController( World world ) {

            this.world = world;

        }

        public void StartWave( ) {

            wave++;
            currentMonsterCount = 0;

            currentMonstersPerWave += 3;

            for (int i = 0; i < currentMonstersPerWave; i++ ) {

                world.SpawnMonster( currentMonsterType );

            }



        }

    }

}
