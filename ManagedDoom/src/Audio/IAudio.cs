using System;

namespace ManagedDoom
{
    public interface IAudio
    {
        public void SetListener(Mobj listener);
        public void Update();
        public void StartSound(Sfx sfx);
        public void StartSound(Mobj mobj, Sfx sfx, SfxType type);
        public void StartSound(Mobj mobj, Sfx sfx, SfxType type, int volume);
        public void StopSound(Mobj mobj);
        public void Reset();
        public void Pause();
        public void Resume();
    }
}
