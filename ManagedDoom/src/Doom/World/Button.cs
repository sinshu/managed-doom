using System;

namespace ManagedDoom
{
    public sealed class Button
    {
        public LineDef Line;
        public ButtonPosition Position;
        public int Texture;
        public int Timer;
        public Mobj SoundOrigin;

        public void Clear()
        {
            Line = null;
            Position = 0;
            Texture = 0;
            Timer = 0;
            SoundOrigin = null;
        }
    }
}
