using System;

namespace ManagedDoom
{
    public sealed class Button
    {
        public LineDef Line;
        public ButtonWhere Where;
        public int Texture;
        public int Timer;
        public Mobj SoundOrigin;

        public void Clear()
        {
            Line = null;
            Where = 0;
            Texture = 0;
            Timer = 0;
            SoundOrigin = null;
        }
    }
}
