using System;

namespace ManagedDoom.Audio
{
    public sealed class NullMusic : IMusic
    {
        private static NullMusic instance;

        public static NullMusic GetInstance()
        {
            if (instance == null)
            {
                instance = new NullMusic();
            }

            return instance;
        }

        public void StartMusic(Bgm bgm, bool loop)
        {
        }
    }
}
