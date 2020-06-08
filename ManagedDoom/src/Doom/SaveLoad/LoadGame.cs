using System;

namespace ManagedDoom
{
    public sealed class LoadGame
    {
        private byte[] data;
        private int save_p;

        public LoadGame(byte[] data)
        {
            this.data = data;
        }

        private void PADSAVEP()
        {
            save_p += (4 - (save_p & 3)) & 3;
        }
    }
}
