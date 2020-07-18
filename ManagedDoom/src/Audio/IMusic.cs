using System;

namespace ManagedDoom.Audio
{
    public interface IMusic
    {
        void StartMusic(Bgm bgm, bool loop);
    }
}
