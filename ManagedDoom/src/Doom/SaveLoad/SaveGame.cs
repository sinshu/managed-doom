using System;

namespace ManagedDoom
{
    public sealed class SaveGame
    {
        private World world;

        private int save_p;
        private byte[] data;

        public SaveGame(World world)
        {
            this.world = world;

            save_p = 0;
            data = new byte[100];
        }

        private void PADSAVEP()
        {
            save_p += (4 - (save_p & 3)) & 3;
        }
    }
}
