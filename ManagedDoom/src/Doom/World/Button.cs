using System;

namespace ManagedDoom
{
    public sealed class Button
    {
        private LineDef line;
        private ButtonPosition position;
        private int texture;
        private int timer;
        private Mobj soundOrigin;

        public void Clear()
        {
            line = null;
            position = 0;
            texture = 0;
            timer = 0;
            soundOrigin = null;
        }

        public LineDef Line
        {
            get => line;
            set => line = value;
        }

        public ButtonPosition Position
        {
            get => position;
            set => position = value;
        }

        public int Texture
        {
            get => texture;
            set => texture = value;
        }

        public int Timer
        {
            get => timer;
            set => timer = value;
        }

        public Mobj SoundOrigin
        {
            get => soundOrigin;
            set => soundOrigin = value;
        }
    }
}
