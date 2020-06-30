using System;

namespace ManagedDoom
{
    public interface IAudio
    {
        public void StartSound(Mobj mobj, Sfx sfx, SfxType type);
    }
}
