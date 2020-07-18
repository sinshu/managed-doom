using System;

namespace ManagedDoom.Audio
{
    public sealed class NullSound : ISound
    {
        private static NullSound instance;

        public static NullSound GetInstance()
        {
            if (instance == null)
            {
                instance = new NullSound();
            }

            return instance;
        }

        public void SetListener(Mobj listerner)
        {
        }

        public void Update()
        {
        }

        public void StartSound(Sfx sfx)
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

        public void Pause()
        {
        }

        public void Resume()
        {
        }

        public int MaxVolume
        {
            get
            {
                return 15;
            }
        }

        public int Volume
        {
            get
            {
                return 8;
            }

            set
            {
            }
        }
    }
}
