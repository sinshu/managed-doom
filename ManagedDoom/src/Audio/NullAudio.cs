using System;

namespace ManagedDoom
{
    public sealed class NullAudio : IAudio
    {
        private static NullAudio instance;

        public static NullAudio GetInstance()
        {
            if (instance == null)
            {
                instance = new NullAudio();
            }

            return instance;
        }

        public void SetListener(Mobj listerner)
        {
        }

        public void Update()
        {
        }

        public void StartSound(Mobj mobj, Sfx sfx, SfxType type)
        {
        }

        public void StartSound(Mobj mobj, Sfx sfx, SfxType type, int volume)
        {
        }

        public void StopSound(Mobj mobj)
        {
        }

        public void Reset()
        {
        }
    }
}
