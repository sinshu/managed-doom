using System;

namespace ManagedDoom
{
    public class StateDef
    {
        private Sprite sprite;
        private int frame;
        private int tics;
        private Action action;
        private State next;
        private int misc1;
        private int misc2;

        public StateDef(
            Sprite sprite,
            int frame,
            int tics,
            Action action,
            State next,
            int misc1,
            int misc2)
        {
            this.sprite = sprite;
            this.frame = frame;
            this.tics = tics;
            this.action = action;
            this.next = next;
            this.misc1 = misc1;
            this.misc2 = misc2;
        }

        public Sprite Sprite => sprite;
        public int Frame => frame;
        public int Tics => tics;
        public Action Action => action;
        public State Next => next;
        public int Misc1 => misc1;
        public int Misc2 => misc2;
    }
}
